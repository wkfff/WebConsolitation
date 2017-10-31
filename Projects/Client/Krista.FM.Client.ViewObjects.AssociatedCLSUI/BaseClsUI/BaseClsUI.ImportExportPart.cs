using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{

    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        private DataSet LoadXMLDataSet = null;


        private bool isHierarchy;

        protected virtual string GetFileName()
        {
            ComboBoxTool cbTool = (ComboBoxTool)vo.utbToolbarManager.Tools["cbDataSources"];
            string currentFilterCaption = cbTool.Text;
            if (currentFilterCaption != string.Empty && HasDataSources())
                return GetDataObjSemanticRus(ActiveDataObj) + '_' + ActiveDataObj.Caption + '_' + currentFilterCaption;
            return GetDataObjSemanticRus(ActiveDataObj) + '_' + ActiveDataObj.Caption;
        }

        protected virtual IExportImporter GetExportImporter()
        {
            return Workplace.ActiveScheme.GetXmlExportImportManager().GetExportImporter(ObjectType.Classifier);
        }

        #region импорт из Excel и XML

        /// <summary>
        /// Загрузка из Excel
        /// </summary>
        /// <returns></returns>
        bool ugeCls_OnLoadFromExcel(object sender)
        {
            this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceImportClassifierData,
				FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("Начало операции импорта классификатора '{0}'", FullCaption));
            int tick = Environment.TickCount;
            LoadXMLDataSet = dsObjData.Clone();
            HierarchyInfo hi = ((UltraGridEx)sender).HierarchyInfo;
            // создем DataRelation и добавляем в DataSet, если классификатор иерархический
            // если у классификатора указаны поля ссылка на родителя и поле родителя, то классификатор иерархический
            if (isHierarchy)
                if (LoadXMLDataSet.Relations.Count == 0)
                {
                    DataColumn parentClmn = LoadXMLDataSet.Tables[0].Columns[hi.ParentClmnName];
                    DataColumn refClmn = LoadXMLDataSet.Tables[0].Columns[hi.ParentRefClmnName];
                    DataRelation rel = new DataRelation("rel", parentClmn, refClmn, false);
                    LoadXMLDataSet.Relations.Add(rel);
                }

            bool isLoadData = false;
            string impotrFileName = string.Empty;
            try
            {
                if (ExportImportHelper.GetFileName(GetFileName(), ExportImportHelper.fileExtensions.xls, false, ref impotrFileName))
                {
                    // записываем информацию о загружаемом файле
                    this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
						FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("Загрузка данных из файла '{0}'", impotrFileName));
                    
                    isLoadData = ExcelExportImportHelper.ImportFromExcel(LoadXMLDataSet, hi.ParentClmnName, hi.ParentRefClmnName,
                    cashedColumnsSettings[String.Format("{0}.{1}", activeDataObj.ObjectKey, GetCurrentPresentation(activeDataObj))], isHierarchy, 
                    activeObject.ClassType != ClassTypes.clsFactData, impotrFileName, this.Workplace, ((IEntity)ActiveDataObj).GeneratorName);
                    foreach(DataRow row in LoadXMLDataSet.Tables[0].Select(string.Empty, string.Empty,DataViewRowState.Added))
                    {
                        row["ID"] = GetNewId();
                    }
                }
                if (isLoadData)
                    this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceSuccefullFinished,
						FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Данные загрузились успешно");
                else
                    this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
						FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Операция импорта была отменена пользователем");

                
            }
            catch(Exception e)
            {
                MessageBox.Show("Операция импорта завершилась с ошибкой", "Импорт данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                isLoadData = false;
                this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceError,
						FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("Во время импорта произошла ошибка : {0}{1}", Environment.NewLine, e.Message));
            }
            return isLoadData;
        }


        private bool IsServerImportFile(FileStream stream)
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(stream);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName == "xmlExport")
                        {
                            return true;
                        }
                        else if (reader.LocalName == "diffgram")
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        /// <summary>
        /// загрузка данных из XML
        /// </summary>
        /// <returns></returns>
        public virtual bool ugeCls_OnLoadFromXML(object sender)
        {
            bool isLoadData = false;
            string importFileName = string.Empty;
            // открываем диалог для выбора имени файла
            if (ExportImportHelper.GetFileName(GetFileName(), ExportImportHelper.fileExtensions.xml, false, ref importFileName))
            {
                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceImportClassifierData,
                    activeDataObj.OlapName, -1, CurrentDataSourceID, (int)clsClassType, string.Format("Запуск операции импорта из файла '{0}'", importFileName));

                FileStream stream = new FileStream(importFileName, FileMode.Open, FileAccess.Read);

                bool serverImport = IsServerImportFile(stream);

                if (serverImport)
                {
                    Workplace.OperationObj.Text = "Загрузка данных";
                    Workplace.OperationObj.StartOperation();
                    //exportImporter = exportImportManager.GetExportImporter(objectType);
                    stream = new FileStream(importFileName, FileMode.Open, FileAccess.Read);
                    FileStream checkStream = null;
                    try
                    {
                        string objectKey = activeDataObj.ObjectKey;
                        DataTable table = null;
                        checkStream = new FileStream(importFileName, FileMode.Open, FileAccess.Read);
                        ImportPatams importParams = new ImportPatams();
                        using (IExportImporter exportImporter = GetExportImporter())
                        {
                            if (!exportImporter.CheckXml(checkStream, objectKey, ref importParams, ref table))
                            {
                                Workplace.OperationObj.StopOperation();
                                frmIncorrectXmlStructure.ShowIncorrectXmlStructure(table, parentWindow, false, GetObjectName());
                                return false;
                            }
                        }
                        // если можно импортировать данные
                        if (!importParams.restoreDataSource)
                        {
                            if (CheckAllowAddNew() == AllowAddNew.No)
                            {
                                // но не выбран источник, а XML без опции восстановления источника
                                // сообщаем об этом и выходим
                                Workplace.OperationObj.StopOperation();
                                MessageBox.Show("Импорт данных невозможен. XML не имеет опции восстановления источника или не содержит источника данных", "Импорт из XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                        // показываем, что какие то параметры импорта были установлены
                        // string importParamsInform = GetImportParamsInform(importParams);
                        if (table.Rows.Count > 0)
                        {
                            // показываем все несоответствия классификатора и XML
                            Workplace.OperationObj.StopOperation();
                            if (!frmIncorrectXmlStructure.ShowIncorrectXmlStructure(table, parentWindow, true, GetObjectName()))
                                return false;
                            Workplace.OperationObj.StartOperation();
                        }
                        ExportImportClsParams exportImportParams = new ExportImportClsParams();
                        exportImportParams.DataSource = CurrentSourceID;
                        exportImportParams.Filter = importDataQuery;
                        exportImportParams.FilterParams = null;
                        exportImportParams.TaskID = CurrentTaskID;                     
                        exportImportParams.ClsObjectKey = objectKey;
                        exportImportParams.RefVariant = RefVariant;
                        using (IExportImporter exportImporter = GetExportImporter())
                        {
                            if (isMasterDetail)
                            {
                                List<IEntity> details = GetDetails(activeDataObj);
                                Dictionary<string, Stream> detailsStreams = new Dictionary<string, Stream>();
                                foreach (IEntity detail in details)
                                {
                                    string filePath = Path.GetDirectoryName(importFileName);
                                    string fileName = filePath + "\\" + GetDetailFileName(detail) +
                                                      Path.GetExtension(importFileName);
                                    if (File.Exists(fileName))
                                    {
                                        FileStream detailStream = new FileStream(fileName, FileMode.Open,
                                                                                 FileAccess.Read);
                                        detailsStreams.Add(detail.ObjectKey, detailStream);
                                    }
                                }
                                try
                                {
                                    // после импорта можно выполнить над данными определенные действия
                                    AfterLoadMasterDetailDataActions(exportImporter.ImportMasterDetail(stream, detailsStreams, exportImportParams));
                                }
                                finally
                                {
                                    foreach (FileStream str in detailsStreams.Values)
                                    {
                                        if (str != null)
                                            str.Close();
                                    }
                                }
                            }
                            else
                            {
                                exportImporter.ImportData(stream, exportImportParams);
                            }
                        }
                        // выполним какие либо действия над загруженными данными перед тем, как показать их
                        AfterImportDataActions();
                        ugeCls_OnRefreshData(null);
                    }
                    catch (Exception exception)
                    {
                        Workplace.OperationObj.StopOperation();
                        if (exception.Message.Contains("ORA-02291"))
                            MessageBox.Show("Нарушено ограничение целостности. Исходный ключ не найден", "Импорт данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else if (exception.Message.Contains("Вариант заблокирован"))
                                MessageBox.Show("Вариант закрыт от изменений. Запись и изменение данных варианта запрещены", "Импорт данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            else if (exception.Message.Contains("ORA-02292") ||
                                exception.Message.Contains("Конфликт инструкции DELETE с ограничением"))
                                MessageBox.Show("Нарушено ограничение целостности. Обнаружена порожденная запись.", "Сохранение изменений", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            else
                                throw new Exception(exception.Message, exception);
                    }
                    finally
                    {
                        if (checkStream != null)
                            checkStream.Close();
                        if (stream != null)
                            stream.Close();
                        Workplace.OperationObj.StopOperation();
                    }
                }
                else
                {
                    if ((CheckAllowAddNew() == AllowAddNew.No) && CheckAllowImportFromXml())
                    {
                        // но не выбран источник, а XML без опции восстановления источника
                        // сообщаем об этом и выходим
                        Workplace.OperationObj.StopOperation();
                        MessageBox.Show("Импорт данных невозможен. XML не имеет опции восстановления источника или не содержит источника данных", "Импорт из XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        stream.Close();
                        return false;
                    }
                    try
                    {
                        Debug.Print("Create copy of source DataSet");
                        LoadXMLDataSet = dsObjData.Clone();
                        HierarchyInfo hi = ((UltraGridEx)sender).HierarchyInfo;
                        // создем DataRelation и добавляем в DataSet, если классификатор иерархический
                        // если у классификатора указаны поля ссылка на родителя и поле родителя, то классификатор иерархический
                        bool isHierarchy = !String.IsNullOrEmpty(hi.ParentClmnName) && !String.IsNullOrEmpty(hi.ParentRefClmnName);
                        if (isHierarchy)
                            if (LoadXMLDataSet.Relations.Count == 0)
                            {
                                DataColumn parentClmn = LoadXMLDataSet.Tables[0].Columns[hi.ParentClmnName];
                                DataColumn refClmn = LoadXMLDataSet.Tables[0].Columns[hi.ParentRefClmnName];
                                DataRelation rel = new DataRelation("rel", parentClmn, refClmn, false);
                                LoadXMLDataSet.Relations.Add(rel);
                            }

                        this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceImportClassifierData,
							FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Загрузка данных из XML");

                        isLoadData = ExportImportHelper.LoadFromXML(LoadXMLDataSet,
                            hi.ParentClmnName, hi.ParentRefClmnName, isHierarchy, ((IEntity)ActiveDataObj).GeneratorName, this.Workplace, importFileName);

                        if (isLoadData)
                            this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceSuccefullFinished,
								FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Данные загрузились успешно");
                        else
                            this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
								FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Операция импорта классификатора была отменена пользователем");
                    }
                    catch(Exception exception)
                    {
                        this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
								FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("Операция импорта закончилась с ошибкой {0}{1}", Environment.NewLine, exception.Message));
                    }
                }
            }
            return isLoadData;
        }

        public virtual void AfterLoadMasterDetailDataActions(Dictionary<string, Dictionary<int, int>> dictionary)
        {
            
        }

        public virtual void AfterImportDataActions()
        {
            
        }

        private List<IEntity> GetDetails(IEntity master)
        {
            List<IEntity> detailsList = new List<IEntity>();
            foreach (IEntityAssociation association in master.Associated.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.MasterDetail)
                {
                    detailsList.Add(association.RoleData);
                }
            }
            return detailsList;
        }

        private string GetImportParamsInform(ImportPatams importParams)
        {
            if (!(importParams.useOldID || importParams.restoreDataSource || importParams.refreshDataByUnique || importParams.refreshDataByAttributes || importParams.deleteDeveloperData))
                return string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Загрузка классификатора из XML выполняется с дополнительными параметрами:");
            if (importParams.deleteDataBeforeImport)
                builder.AppendLine("Удаление данных перед импортом");
            if (importParams.deleteDeveloperData)
                builder.AppendLine("Удаление данных разработчика");
            if (importParams.refreshDataByAttributes)
                builder.AppendLine("Обновление данных по атрибутам, указанным пользователем");
            if (importParams.refreshDataByUnique)
                builder.AppendLine("Обновление данных по уникальности в схеме");
            if (importParams.restoreDataSource)
                builder.AppendLine("Восстановление источника данных из XML");
            if (importParams.useOldID)
                builder.AppendLine("Сохранение старых ID записей");
            return builder.ToString();
        }


        private string GetDetailFileName(IEntity detail)
        {
            return detail.OlapName;
        }


        private ObjectType GetImportedObjectType()
        {
            if (clsClassType == ClassTypes.clsFactData)
                return ObjectType.FactTable;
            return ObjectType.Classifier;
        }

        #endregion

        #region экспорт в Excel и XML

        bool ugeCls_OnSaveToExcel(object sender)
        {
            DataSet ds = dsObjData.Clone();
            DataSet dsAllRows = dsObjData.Clone();

            HierarchyInfo hi = ((UltraGridEx)sender).HierarchyInfo;
            LoadMode oldLoadMode = hi.loadMode;
            hi.loadMode = LoadMode.AllRows;
            InternalLoadData(ref dsAllRows);
            DataTable source = dsAllRows.Tables[0];
            DataTable destination = ds.Tables[0];

            if (!String.IsNullOrEmpty(hi.ParentRefClmnName))
                DataTableHelper.CopyDataTable(source, ref destination, string.Format("{0} ASC", hi.ParentRefClmnName));
            else
                DataTableHelper.CopyDataTable(source, ref destination);

            ExcelExportImportHelper.ExportDataToExcel(ds, cashedColumnsSettings[String.Format("{0}.{1}", activeDataObj.ObjectKey,
                GetCurrentPresentation(activeDataObj))], GetFileName(), this.Workplace, isHierarchy, false);
            dsAllRows.Clear();
            ds.Clear();
            hi.loadMode = oldLoadMode;
            return true;
        }

        #warning Поле удалить и сделать функцию, которая формирует фильтр
        protected string importDataQuery;

        /// <summary>
        /// сохранение данных в XML
        /// </summary>
        /// <returns></returns>
        bool ugeCls_OnSaveToXML(object sender)
        {
            bool returnValue = false;

            ImportPatams importParams = new ImportPatams();
            List<string> uniqueAttributes = new List<string>();
            foreach (IDataAttribute attr in ActiveDataObj.Attributes.Values)
            {
                if (attr.Kind == DataAttributeKindTypes.Regular && attr.Name != "ParentID")
                    uniqueAttributes.Add(string.Format("{0} ({1})", attr.Caption, attr.Name));
            }

            bool exportAllRows = true;
            bool exportHierarchyRows = ((UltraGridEx)sender).HierarchyInfo.LevelsCount > 1;
            ObjectTypeParams objectTypeParams = GetObjectType();

            if (!FormImportParameters.ShowImportParams(parentWindow, String.Join(",", uniqueAttributes.ToArray()),
               objectTypeParams, Workplace.IsDeveloperMode, ref importParams, ref exportAllRows, ref exportHierarchyRows))
                return false;

            string tmpFileName = GetFileName();
            // некоторые параметры для экспорта только выделенных записей
            // список выделенных записей
            List<int> ids = new List<int>();
            if (!exportAllRows)
            {
                if (vo.ugeCls.ugData.Selected.Rows.Count == 0 && vo.ugeCls.ugData.ActiveRow == null)
                {
                    MessageBox.Show(Workplace.WindowHandle, "В таблице нет ни одной выбранной записи", "Сохранение в XML",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if(vo.ugeCls.ugData.Selected.Rows.Count == 0)
                    vo.ugeCls.ugData.ActiveRow.Selected = true;
                tmpFileName = tmpFileName + "_фрагмент";
                // получим список выделенных записей
                
                if (!exportHierarchyRows)
                    UltraGridHelper.GetSelectedIDs(vo.ugeCls.ugData, out ids);
                else
                    UltraGridHelper.GetSelectedIds(vo.ugeCls, vo.ugeCls.HierarchyInfo, out ids, true);
            }
            // показываем диалог с выбором файла
            if (ExportImportHelper.GetFileName(tmpFileName, ExportImportHelper.fileExtensions.xml, true, ref tmpFileName))
            {
                Workplace.OperationObj.Text = "Сохранение данных";
                Workplace.OperationObj.StartOperation();
                FileStream stream = null;
                try
                {
                    stream = new FileStream(tmpFileName, FileMode.Create);
                    string objectKey = activeDataObj.ObjectKey;
                    IDbDataParameter[] paramsArray = ParametersListToArray(GetServerFilterParameters());
                    // в фильтре в иерархических классификаторах убираем часть фильтра,
                    // по которой выбираются только записи, подчиненные текущей
                    if (!string.IsNullOrEmpty(dataQuery))
                    {
                        string[] queryParts = dataQuery.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries);
                        importDataQuery = queryParts[0];
                        for (int i = 1; i <= queryParts.Length - 1; i++)
                        {
                            if (!queryParts[i].Contains("ParentID"))
                                importDataQuery = importDataQuery + " and " + queryParts[i];
                        }
                    }

                    ExportImportClsParams exportImportParams = new ExportImportClsParams();
                    exportImportParams.DataSource = CurrentSourceID;
                    exportImportParams.Filter = importDataQuery;
                    exportImportParams.FilterParams = paramsArray;
                    exportImportParams.TaskID = CurrentTaskID;
                    exportImportParams.ClsObjectKey = objectKey;
                    exportImportParams.SelectedRowsId = ids;

                    using (IExportImporter exportImporter = GetExportImporter())
                    {
                        if (isMasterDetail)
                        {
                            List<IEntity> details = GetDetails(activeDataObj);
                            Dictionary<string, Stream> detailsStreams = new Dictionary<string, Stream>();
                            foreach (IEntity detail in details)
                            {
                                string filePath = Path.GetDirectoryName(tmpFileName);
                                string fileName = filePath + "\\" + GetDetailFileName(detail) +
                                                  Path.GetExtension(tmpFileName);
                                FileStream detailStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                                detailsStreams.Add(detail.ObjectKey, detailStream);
                            }
                            try
                            {
                                exportImporter.ExportMasterDetail(stream, detailsStreams, exportImportParams,
                                                                 importParams);
                            }
                            finally
                            {
                                foreach (FileStream str in detailsStreams.Values)
                                {
                                    if (str != null)
                                        str.Close();
                                }
                            }
                        }
                        else
                            exportImporter.ExportData(stream, importParams, exportImportParams);
                    }
                }
                catch (Exception exception)
                {
                    Workplace.OperationObj.StopOperation();
                    if (exception.Message.Contains("is denied") || exception.Message.Contains("Отказано в доступе"))
                    {
                        string errStr = "Приложение не может получить доступ к файлу. Возможно он используется другим процессом или защищен от записи.";
                        MessageBox.Show(errStr, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                    Workplace.OperationObj.StopOperation();
                }
            }
            return returnValue;
        }

        private ObjectTypeParams GetObjectType()
        {
            if (clsClassType == ClassTypes.clsFactData)
                return ObjectTypeParams.FactTable;
            if (HasDataSources())
                return ObjectTypeParams.DivSourceClassifier;
            return ObjectTypeParams.NonDivSourceClassifier;
        }

        #endregion

        /// <summary>
        /// действия, выполняемые после импорта данных из XML
        /// </summary>
        /// <param name="RowsCountBeforeImport"></param>
        protected virtual void ugeCls_OnAftertImportFromXML(object sender, int RowsCountBeforeImport)
        {
            Debug.Print("Proccesing data from XML started");

            this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
					FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Начало обработки данных");
            this.Workplace.OperationObj.Text = "Обработка данных";
            this.Workplace.OperationObj.StartOperation();

            DataTable importedTable = LoadXMLDataSet.Tables[0];
            try
            {
                importedTable.Constraints.Clear();
                importedTable.BeginLoadData();
                LoadXMLDataSet.EnforceConstraints = true;

                Dictionary<int, string> ColumnsName = new Dictionary<int, string>();
                Dictionary<int, int> ColumnsDataLength = new Dictionary<int, int>();
                int index = 0;
                Debug.Print("Create sequence of string attributes");

                GridColumnsStates states = this.ugeCls_OnGetGridColumnsState(vo.ugeCls);

                foreach (GridColumnState state in states.Values)
                {
                    if (importedTable.Columns.Contains(state.ColumnName))
                    {
                        if (importedTable.Columns[state.ColumnName].DataType == typeof(String))
                        {
                            ColumnsName.Add(index, state.ColumnName);
                            ColumnsDataLength.Add(index, state.ColumnWidth);
                            index++;
                        }
                    }
                }

                Debug.Print("Cutting of string values started");
                // для классификаторов обрезаем строковые поля до максимальной длины поля
                foreach (DataRow importedRow in importedTable.Rows)
                {
                    for (int i = 0; i <= ColumnsName.Count - 1; i++)
                    {
                        string strValue = importedRow[ColumnsName[i]].ToString();

                        strValue = StringHelper.GetNormalizeString(strValue);

                        if (strValue.Length > ColumnsDataLength[i])
                            importedRow[ColumnsName[i]] = strValue.Substring(0, ColumnsDataLength[i]);
                        else
                            importedRow[ColumnsName[i]] = strValue;
                    }
                }
                Debug.Print("Cutting of string values finished");
                // для сопоставимых классификаторов убираем источник данных, если вдруг такие есть
                Debug.Print("Set data sources to data classifiers started");
                int sourceIDIndex = importedTable.Columns.IndexOf("SourceID");
                if (sourceIDIndex != -1)
                    foreach (DataRow row in importedTable.Rows)
                    {
                        row[sourceIDIndex] = CurrentDataSourceID;
                    }
                Debug.Print("Setting data sources to data classifiers finished");
                // Для классификаторов данных "обнуляем" ссылки на сопоставимый классификатор
                /*
                if (ActiveDataObj.ClassType != ClassTypes.clsFactData)
                {
                    Debug.Print("Clearing references to associate classifiers started");
                    List<string> refClmnNames = new List<string>();
                    foreach (IAssociation item in ((IClassifier)ActiveDataObj).Associations.Values)
                    {
                        IDataAttribute attr = ActiveDataObj.Attributes[item.RoleDataAttribute.ObjectKey];
                        if (item.AssociationClassType == AssociationClassTypes.Bridge ||
                            item.AssociationClassType == AssociationClassTypes.BridgeBridge)
                            if (attr.Class == DataAttributeClassTypes.Reference && !attr.IsLookup)
                                refClmnNames.Add(item.FullDBName);
                    }

                    foreach (DataRow row in importedTable.Rows)
                        foreach (string refName in refClmnNames)
                            row[refName] = -1;

                    refClmnNames.Clear();
                    Debug.Print("Clearing references to associate classifiers finished");
                }*/
                if (ActiveDataObj.ClassType == ClassTypes.clsFactData)
                {
                    // для таблиц фактов установим текущую задачу, а закачкув -1
                    int pumpIDIndex = -1;
                    if (importedTable.Columns.Contains("PumpID"))
                        pumpIDIndex = importedTable.Columns.IndexOf("PumpID");
                    foreach (DataRow row in importedTable.Rows)
                    {
                        if (pumpIDIndex > -1)
                            row["PumpID"] = -1;
                        row["TaskID"] = CurrentTaskID;
                    }
                }

                importedTable.EndLoadData();

                Debug.Print("Copy proccesed loaded data to source DataSet started");
                // перенос данных в освновной DataSet
                DataTable destinationTable = dsObjData.Tables[0];
                DataTableHelper.CopyDataTable(importedTable, ref destinationTable);
                Debug.Print("Copy proccesed loaded data to source DataSet finished");
            }
            finally
            {
                this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
					FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("Обработанно {0} записей", LoadXMLDataSet.Tables[0].Rows.Count));
                LoadXMLDataSet.Clear();
                LoadXMLDataSet = null;
                this.Workplace.OperationObj.StopOperation();

                this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceSuccefullFinished,
					FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Обработка данных завершилась успешно");

                this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceSuccefullFinished,
					FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Импорт классификатора завершился успешно");

                Debug.Print("Proccesing data from XML finished");
            }
        }


        bool ugeCls_OnSaveToDataSetXML(object sender)
        {
            UltraGridEx gridEx = sender as UltraGridEx;
            if (gridEx == null)
                return false;

            HierarchyInfo hi = gridEx.HierarchyInfo;
            // созлдаем копию текущего источника данных
            DataSet dsAllRows = new DataSet("DataSet");
            dsAllRows.Tables.Add(new DataTable("Table"));
            // грузим полностью все данные для сохранения в XML
            hi.loadMode = Krista.FM.Client.Components.LoadMode.AllRows;
            InternalLoadData(ref dsAllRows);
            // сохраняем данные в XML
            ExportImportHelper.SaveToOuterFormatXML(dsAllRows, GetFileName(), GetObjectParams());
            dsAllRows.Dispose();
            return true;
        }

        /// <summary>
        /// возвращает параметры по текущему объекту данных
        /// </summary>
        /// <returns></returns>
        private List<string> GetObjectParams()
        {
            List<string> objectParams = new List<string>();

            objectParams.Add("name");
            objectParams.Add(activeDataObj.Name);

            objectParams.Add("semantic");
            objectParams.Add(activeDataObj.Semantic);

            objectParams.Add("rusName");
            objectParams.Add(ActiveDataObj.Caption);

            objectParams.Add("rusSemantic");
            objectParams.Add(GetDataObjSemanticRus(this.ActiveDataObj));

            objectParams.Add("fullName");
            objectParams.Add(activeDataObj.ObjectKey);

            return objectParams;
        }

        private string GetObjectName()
        {
            string objectName = string.Empty;
            switch (clsClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    objectName = "сопоставимого классификатора";
                    break;
                case ClassTypes.clsDataClassifier:
                    objectName = "классификатора данных";
                    break;
                case ClassTypes.clsFactData:
                    objectName = "таблицы фактов";
                    break;
                case ClassTypes.clsFixedClassifier:
                    objectName = "фиксированого классификатора";
                    break;
            }
            return objectName;
        }

    }
}