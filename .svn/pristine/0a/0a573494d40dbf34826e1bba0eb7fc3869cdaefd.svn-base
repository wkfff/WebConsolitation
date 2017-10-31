using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.TranslationsTables
{
	public partial class TranslationsTablesUI : BaseViewObj
	{
		private TranslationsTablesView TranslationsView;

		// Текущая таблица перекодировки
		private IConversionTable CurrentConversionTable;
	    private IAssociation currentAssociation;

		DataTable ConversionDBTable = new DataTable();
		DataSet ConversionDBDataSet = new DataSet();

		IDataUpdater duConversionTable = null;

        string currentObjName = string.Empty;

        object[] activeTranslationsParams = null;

        IWin32Window parentWindow;

        IClassifiersProtocol protocol;

		private enum LocalRowState { Added, Deleted, Modified, Unchanged };

		public TranslationsTablesUI(string key)
            : base(key)
		{
            Index = 4;
			Caption = "Таблицы перекодировки";
		}

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.cls_TranslationTable_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_TranslationTable_16; }
		}

		public override System.Drawing.Image TypeImage24
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_TranslationTable_24; }
		}

		protected override void SetViewCtrl()
		{
			fViewCtrl = new TranslationsTablesView();
			TranslationsView = (TranslationsTablesView)fViewCtrl;
		}

		public override void Initialize()
		{
			base.Initialize();

			// обработчики для основного грида
			TranslationsView.ugeTranslTables.StateRowEnable = true;            
            TranslationsView.ugeTranslTables.OnSaveChanges += new CC.SaveChanges(ugeTranslTables_OnSaveChanges);
            TranslationsView.ugeTranslTables.OnCancelChanges += new CC.DataWorking(ugeTranslTables_OnCancelChanges);
            TranslationsView.ugeTranslTables.OnRefreshData += new CC.RefreshData(ugeTranslTables_OnRefreshData);            
            TranslationsView.ugeTranslTables.OnSaveToXML += new CC.SaveLoadXML(ugeTranslTables_OnSaveToXML);
            TranslationsView.ugeTranslTables.OnLoadFromXML += new CC.SaveLoadXML(ugeTranslTables_OnLoadFromXML);

            TranslationsView.ugeTranslTables.OnLoadFromExcel += new Krista.FM.Client.Components.SaveLoadXML(ugeTranslTables_OnLoadFromExcel);
            TranslationsView.ugeTranslTables.OnSaveToExcel += new Krista.FM.Client.Components.SaveLoadXML(ugeTranslTables_OnSaveToExcel);

            TranslationsView.ugeTranslTables.OnClearCurrentTable += new CC.DataWorking(ugeTranslTables_OnClearCurrentTable);			
            TranslationsView.ugeTranslTables.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeTranslTables_OnGetGridColumnsState);
            TranslationsView.ugeTranslTables.OnGridCellError += new Krista.FM.Client.Components.GridCellError(ugeTranslTables_OnGridCellError);

            TranslationsView.ugeTranslTables.ugData.MouseClick += new MouseEventHandler(ugData_MouseClick);
            TranslationsView.ugeTranslTables.ugData.MouseEnterElement += new UIElementEventHandler(ugData_MouseEnterElement);

            TranslationsView.cmsAudit.ItemClicked += new ToolStripItemClickedEventHandler(cmsAudit_ItemClicked);

            TranslationsView.utcDataCls.ActiveTabChanged += new Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventHandler(utcDataCls_ActiveTabChanged);

            parentWindow = (IWin32Window)(Control)this.Workplace;

            protocol = (IClassifiersProtocol)this.Workplace.ActiveScheme.GetProtocol("Workplace.exe");

			// Получаем текущую таблицу перекодировки
			CurrentConversionTable = Workplace.ActiveScheme.ConversionTables[Key];

			fViewCtrl.Text = CurrentConversionTable.FullName;

            currentAssociation = (IAssociation)Workplace.ActiveScheme.Associations[CurrentConversionTable.Name];

			RefreshData();
        }


        bool ugeTraslTablesNavi_OnRefreshData(object sender)
        {
            RefreshData();
            return true;
        }

        void ugeTranslTables_OnGridCellError(object sender, CellDataErrorEventArgs e)
        {
            MessageBox.Show("Введенные данные не соответствуют маске.", "Ввод данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void utmRefreshData_ToolClick(object sender, ToolClickEventArgs e)
        {
            RefreshData();
        }

        private Dictionary<string, CC.GridColumnsStates> cashedColumnsSettings = new Dictionary<string, CC.GridColumnsStates>();

        CC.GridColumnsStates ugeTranslTables_OnGetGridColumnsState(object sender)
        {
            currentObjName = CurrentConversionTable.Name + "." + CurrentConversionTable.RuleName;
            if (cashedColumnsSettings.ContainsKey(currentObjName))
            {
                // если да - возвращаем копию
                return cashedColumnsSettings[currentObjName];
            }

            string mask = string.Empty;
            int clmnValueLength = 0;
            int mantissa = 0;
            IDataAttribute attr = null;
            DataAttributeTypes attrType = DataAttributeTypes.dtUnknown;

            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("ID", state);

            IEntity currentClassifier = null;
            for (int i = 1; i <= ConversionDBTable.Columns.Count - 1; i++)
            {
                DataColumn clmn = ConversionDBTable.Columns[i];

                string clsName = string.Empty;

                if (clmn.Caption != string.Empty && clmn.Caption != "ID")
                {
                    state = new CC.GridColumnState();
                    state.ColumnName = clmn.Caption;
                    string sign = string.Empty;
                    if (clmn.Caption.Contains(">"))
                    {
                        sign = ">";
                        currentClassifier = currentAssociation.RoleData;
                    }
                    else if (clmn.Caption.Contains("<"))
                    {
                        sign = "<";
                        currentClassifier = currentAssociation.RoleBridge;
                    }

                    if (sign != string.Empty)
                    {
                        clsName = clmn.Caption.Replace(sign, string.Empty);

                        foreach (IDataAttribute item in currentClassifier.Attributes.Values)
                        {
                            if (item.Name == clsName)
                            {
                                attr = item;
                                break;
                            }
                        }
                        if (attr != null)
                        {
                            state.ColumnCaption = sign + attr.Caption;
                            mask = attr.Mask;
                            clmnValueLength = attr.Size;
                            mantissa = attr.Scale;
                        }
                    }

                    attrType = attr.Type;

                    switch (attrType)
                    {
                        case DataAttributeTypes.dtInteger:
                            if (mask == string.Empty)
                                mask = String.Empty.PadRight(clmnValueLength, '9');
                            break;
                        case DataAttributeTypes.dtDouble:
                            mask = '-' + String.Empty.PadRight(clmnValueLength, 'n') + '.' + String.Empty.PadRight(mantissa, 'n');
                            break;
                    }
                    state.Mask = mask;
                    state.IsNullable = attr.IsNullable;
                    state.DefaultValue = attr.DefaultValue;
                    state.ColumnWidth = attr.Size;
                    states.Add(state.ColumnName, state);
                }
            }
            cashedColumnsSettings.Add(currentObjName, states);
            return states;
        }

        #region экспорт и импорт таблиц перекодировок
        // для перекодлировок нету экспорта выделенных записей
        // нужна форма параметров импорта с ограничением выбора параметров (оставить только очистку перед импортом)

        //IWin32Window _IWin32Window;

        bool ugeTranslTables_OnLoadFromXML(object sender)
        {
            bool isLoadData = false;
            string importFileName = TranslationsView.ugeTranslTables.SaveLoadFileName;
            bool serverImport = false;
            if (ExportImportHelper.GetFileName(importFileName, ExportImportHelper.fileExtensions.xml, false, ref importFileName))
            {
                this.protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceImportClassifierData,
                    currentObjName, -1, -1, 4, string.Format("Запуск операции импорта из файла '{0}'", importFileName));

                FileStream stream = new FileStream(importFileName, FileMode.Open, FileAccess.Read);
                XmlTextReader reader = new XmlTextReader(stream);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName == "xmlExport")
                        {
                            serverImport = true;
                            break;
                        }
                        else if (reader.LocalName == "diffgram")
                        {
                            serverImport = false;
                            break;
                        }
                    }
                }

                reader.Close();

                if (serverImport)
                {

                    IExportImportManager manager = null;//this.Workplace.ActiveScheme.GetXmlExportImportManager();               
                    try
                    {
                        manager = this.Workplace.ActiveScheme.GetXmlExportImportManager();
                        IConversionTableExportImporter exporter = (IConversionTableExportImporter)manager.GetExportImporter(ObjectType.ConversionTable);

                        this.Workplace.OperationObj.Text = "Загрузка данных";
                        this.Workplace.OperationObj.StartOperation();
                        stream = new FileStream(importFileName, FileMode.Open, FileAccess.Read);
                        try
                        {
                            DataTable table = null;
                            FileStream tmpStream = new FileStream(importFileName, FileMode.Open, FileAccess.Read);
                            ImportPatams importParams = new ImportPatams();
                            if (exporter.CheckXml(tmpStream, currentObjName, ref importParams, ref table))
                            {
                                if (table.Rows.Count > 0)
                                {
                                    this.Workplace.OperationObj.StopOperation();
                                    if (!frmIncorrectXmlStructure.ShowIncorrectXmlStructure(table, parentWindow, true, "таблицы перекодировки"))
                                        return false;
                                    this.Workplace.OperationObj.StartOperation();
                                }
                                exporter.ImportData(stream, this.CurrentConversionTable.Name, this.CurrentConversionTable.RuleName);
                                
                                RefreshCurrentTable();
                            }
                            else
                            {
                                this.Workplace.OperationObj.StopOperation();
                                frmIncorrectXmlStructure.ShowIncorrectXmlStructure(table, parentWindow, false, "таблицы перекодировки");
                            }
                        }
                        catch (Exception exception)
                        {
                            throw new Exception(exception.Message, exception);
                        }
                        finally
                        {
                            stream.Close();
                            this.Workplace.OperationObj.StopOperation();
                        }
                    }
                    finally
                    {
                        if (manager != null)
                            manager.Dispose();
                    }
                    //RefreshCurrentTable();
                }
                else
                    isLoadData = ExportImportHelper.LoadFromXML(ConversionDBDataSet, this.Workplace, importFileName);
            }

            //
            return isLoadData;
            
        }

        bool ugeTranslTables_OnSaveToXML(object sender)
        {
            //ExportImportHelper.SaveToXML(ConversionDBDataSet, TranslationsView.ugeTranslTables.SaveLoadFileName);

            //return true;

            IConversionTableExportImporter exporter = (IConversionTableExportImporter)Workplace.ActiveScheme.GetXmlExportImportManager().GetExportImporter(ObjectType.ConversionTable);
            bool returnValue = false;
            try
            {
                ImportPatams importParams = new ImportPatams();

                bool exportAllRows = true;
                bool exportHierarchy = false;
                string _uniqueAttributesNames = string.Empty;
                ObjectTypeParams objectType = ObjectTypeParams.ConversionTable;
                if (FormImportParameters.ShowImportParams(parentWindow, _uniqueAttributesNames, objectType, false, ref importParams, ref exportAllRows, ref exportHierarchy))
                {
                    importParams.useOldID = false;
                    importParams.restoreDataSource = false;
                    // для перекодировок уникальности уже есть, 
                    // к тому же не используется запись непосредственно в базу через запрос
                    importParams.refreshDataByUnique = false;
                    importParams.refreshDataByAttributes = false;
                    importParams.uniqueAttributesNames = string.Empty;

                    string tmpFileName = TranslationsView.ugeTranslTables.SaveLoadFileName;
                    if (!exportAllRows)
                    {
                        if (this.TranslationsView.ugeTranslTables.ugData.Selected.Rows.Count == 0)
                            this.TranslationsView.ugeTranslTables.ugData.ActiveRow.Selected = true;
                        tmpFileName = tmpFileName + "_фрагмент";
                    }
                    
                    if (ExportImportHelper.GetFileName(tmpFileName, ExportImportHelper.fileExtensions.xml, true, ref tmpFileName))
                    {
                        this.Workplace.OperationObj.Text = "Сохранение данных";
                        this.Workplace.OperationObj.StartOperation();
                        try
                        {
                            FileStream stream = new FileStream(tmpFileName, FileMode.Create);

                            if (!exportAllRows)
                            {
                                List<int> ids;
                                CC.UltraGridHelper.GetSelectedIDs(TranslationsView.ugeTranslTables.ugData, out ids);
                                exporter.ExportSelectedData(stream, CurrentConversionTable.Name, CurrentConversionTable.RuleName, importParams, ids.ToArray());
                            }
                            else
                                exporter.ExportData(stream, CurrentConversionTable.Name, CurrentConversionTable.RuleName, importParams);

                            stream.Close();
                        }
                        catch (Exception exception)
                        {
                            this.Workplace.OperationObj.StopOperation();
                            if (exception.Message.Contains("is denied") || exception.Message.Contains("Отказано в доступе"))
                            {
                                string errStr = "Приложение не может получить доступ к файлу. Возможно он используется другим процессом или защищен от записи.";
                                MessageBox.Show(errStr, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        finally
                        {
                            this.Workplace.OperationObj.StopOperation();
                        }
                    }
                }
            }
            finally
            {
                //exporter.Dispose();
            }
            return returnValue;
        }

        bool ugeTranslTables_OnSaveToExcel(object sender)
        {
            ExcelExportImportHelper.ExportDataToExcel(ConversionDBDataSet, cashedColumnsSettings[currentObjName], TranslationsView.ugeTranslTables.SaveLoadFileName, this.Workplace, false, false);
            return true;
        }

        bool ugeTranslTables_OnLoadFromExcel(object sender)
        {
            string importFileName = string.Empty;
            if (ExportImportHelper.GetFileName(TranslationsView.ugeTranslTables.SaveLoadFileName, ExportImportHelper.fileExtensions.xls, false, ref importFileName))
                return ExcelExportImportHelper.ImportFromExcel(ConversionDBDataSet, string.Empty, string.Empty,
                    cashedColumnsSettings[currentObjName], false, false,
                    importFileName, this.Workplace, string.Empty);
            
            return false;
        }

        #endregion

        /// <summary>
        /// очистка текущей таблицы перекодировок
        /// </summary>
        /// <param name="sender"></param>
        void ugeTranslTables_OnClearCurrentTable(object sender)
		{
			if (MessageBox.Show("Очистить текущую таблицу перекодировки?", "Очистка таблицы", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
                int deletedRowsCount = this.TranslationsView.ugeTranslTables._ugData.Rows.Count;
				CurrentConversionTable.Clear();

                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
                    currentObjName, -1, -1, 4, string.Format("{0} Удалено записей: {1}", "Операция очистки данных таблицы перекодировки", deletedRowsCount));
                CanDeactivate = true;
			    RefreshCurrentTable();
			}
		}

		public override void Activate(bool Activated)
		{
            if (Activated)
                Workplace.ViewObjectCaption = captionString;
            else
                SaveChangesWhenExit();

		}

        void SaveChangesWhenExit()
        {
            if (GetChanges())
                if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ugeTranslTables_OnSaveChanges(TranslationsView.ugeTranslTables);
                }
                else
                {
                    ugeTranslTables_OnCancelChanges(TranslationsView.ugeTranslTables);
                    TranslationsView.ugeTranslTables.ClearAllRowsImages();
                }

        }

		Dictionary<int, DataRow> DeletedRows = new Dictionary<int, DataRow>();

		int ugeTranslTables_OnGetHierarchyLevelsCount()
		{
			return 1;
		}

        bool ugeTranslTables_OnRefreshData(object sender)
		{
            // если были внесены изменения спросим, нужно ли их сохранять
            if (ConversionDBDataSet.GetChanges() != null)
                if (MessageBox.Show(parentWindow, "Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (TranslationsView.ugeTranslTables.ugData.ActiveRow != null)
                        TranslationsView.ugeTranslTables.ugData.ActiveRow.Update();
                    if (!TranslationsView.ugeTranslTables.PerfomAction("SaveChange"))
                        CanDeactivate = false;
                }
            if (CanDeactivate)
            {
                RefreshCurrentTable();
                return true;
            }
            return false;
		}

		void RefreshCurrentTable()
		{
            InitializeData();
		}

        void ugeTranslTables_OnCancelChanges(object sender)
		{
			ConversionDBTable.RejectChanges();
			DeletedRows.Clear();
            this.CanDeactivate = true;
		}

        bool GetChanges()
        {
            if (DeletedRows.Count > 0 || ConversionDBTable.GetChanges() != null)
                return true;
            else
                return false;
        }

        #region сохранение данных

        private bool SaveData()
        {
            bool SucceessSaveChanges = true;
            this.Workplace.OperationObj.Text = "Сохранение данных";
            this.Workplace.OperationObj.StartOperation();
            try
            {
                // ищем удаленные записи и записываем их в список
                foreach (DataRow row in ConversionDBDataSet.Tables[0].Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        DeletedRows.Add(Convert.ToInt32(row["ID", DataRowVersion.Original]), row);

                    }
                }
                // в этом списке удаляем записи из базы и из таблицы
                foreach (DataRow row in DeletedRows.Values)
                {
                    CurrentConversionTable.DeleteRow(Convert.ToInt32(row["ID", DataRowVersion.Original]));
                    row.AcceptChanges();
                }
                // очищаем список
                DeletedRows.Clear();
                // записываем остальные изменения в базу кроме удаления
                if (TranslationsView.ugeTranslTables.ugData.ActiveRow != null)
                {
                    if (TranslationsView.ugeTranslTables.ugData.ActiveRow.IsAddRow)
                        // для временной колонки - отменяем изменения
                        TranslationsView.ugeTranslTables.ugData.ActiveRow.CancelUpdate();
                    else
                        // иначе сохраняем изменения колонки
                        TranslationsView.ugeTranslTables.ugData.ActiveRow.Update();
                }
                DataTable dtAdded = ConversionDBDataSet.Tables[0].GetChanges(DataRowState.Added);
                if (dtAdded != null)
                {
                    string errMsg = string.Empty;
                    if (CheckNotNullValue(ref errMsg))
                    {
                        try
                        {
                            int count = duConversionTable.Update(ref dtAdded);
                        }
                        catch (Exception exception)
                        {
                            SucceessSaveChanges = false;
                            throw exception;
                        }
                        finally
                        {
                            dtAdded = null;
                        }
                    }
                    else
                    {
                        this.Workplace.OperationObj.StopOperation();
                        MessageBox.Show(errMsg, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SucceessSaveChanges = false;
                    }
                }

                DataTable dt = ConversionDBDataSet.Tables[0].GetChanges(DataRowState.Modified);

                if (dt != null)
                {
                    string errMsg = string.Empty;
                    if (CheckNotNullValue(ref errMsg))
                    {
                        try
                        {
                            int count = duConversionTable.Update(ref dt);
                        }
                        catch (Exception exception)
                        {
                            SucceessSaveChanges = false;
                            throw exception;
                        }
                        finally
                        {
                            dt = null;
                        }
                    }
                    else
                    {
                        this.Workplace.OperationObj.StopOperation();
                        MessageBox.Show(errMsg, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SucceessSaveChanges = false;
                    }
                }
            }
            catch (Exception exception)
            {
                SucceessSaveChanges = false;
                throw exception;
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
            return SucceessSaveChanges;
        }


        bool ugeTranslTables_OnSaveChanges(object sender)
		{
            SaveGridColumnsParams(TranslationsView.ugeTranslTables.ugData);
            CanDeactivate = false;
            if (SaveData())
            {
                ugeTranslTables_OnCancelChanges(TranslationsView.ugeTranslTables);
                RefreshCurrentTable();
                LoadGridColumnsParams(TranslationsView.ugeTranslTables.ugData);
            }
            return CanDeactivate;
        }
        #endregion

        private GridColumsParams[] columnsParams;
        /// <summary>
        /// сохраняет основные параметры колонок в гриде
        /// </summary>
        /// <param name="grid"></param>
        private void SaveGridColumnsParams(UltraGrid grid)
        {
            columnsParams = new GridColumsParams[grid.DisplayLayout.Bands[0].Columns.Count];
            for (int i = 0; i <= grid.DisplayLayout.Bands[0].Columns.Count - 1; i++)
            {
                columnsParams[i].ColumnName = grid.DisplayLayout.Bands[0].Columns[i].Key;
                columnsParams[i].ColumnPosition = grid.DisplayLayout.Bands[0].Columns[i].Header.VisiblePosition;
                columnsParams[i].ColumnWidth = grid.DisplayLayout.Bands[0].Columns[i].Width;
                columnsParams[i].ColumnWisible = !grid.DisplayLayout.Bands[0].Columns[i].Hidden;
            }
        }
        /// <summary>
        /// восстанавливает параметры колонок в гриде
        /// </summary>
        /// <param name="grid"></param>
        private void LoadGridColumnsParams(UltraGrid grid)
        {
            for (int i = 0; i <= grid.DisplayLayout.Bands[0].Columns.Count - 1; i++)
            {
                if (grid.DisplayLayout.Bands[0].Columns.Exists(columnsParams[i].ColumnName))
                {
                    grid.DisplayLayout.Bands[0].Columns[i].Header.VisiblePosition = columnsParams[i].ColumnPosition;
                    grid.DisplayLayout.Bands[0].Columns[i].Width = columnsParams[i].ColumnWidth;
                    grid.DisplayLayout.Bands[0].Columns[i].Hidden = !columnsParams[i].ColumnWisible;
                }
            }
        }

		void ugTranlationTables_BeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// перед уходом с текущей таблицы сохраним все изменения
            SaveChangesWhenExit();
            if (!this.CanDeactivate)
            {
                e.Cancel = true;
                return;
            }
            else
                e.Cancel = false;
			// удалим текущий Updater
			if (duConversionTable != null)
			{
				duConversionTable.Dispose();
				duConversionTable = null;
			}
		}

        string captionString;

		public override void InitializeData()
        {   
            // очищаем объекты, в которых находятся данные
            ConversionDBTable.Clear();
            ConversionDBTable.Columns.Clear();
			ConversionDBDataSet.Clear();
            ConversionDBDataSet.Tables.Clear();

            cashedColumnsSettings.Clear();

            this.Workplace.OperationObj.Text = "Запрос данных";
            this.Workplace.OperationObj.StartOperation();
            try
            {
                // Получаем Updater для работы с данными таблицы перекодировки
                duConversionTable = CurrentConversionTable.GetDataUpdater();
                duConversionTable.Fill(ref ConversionDBTable);
                // почему то данные передаются в виде DAtaTable, а нам надо DataSet. 
                // поэтому получаем DAtaTable и добавляем в текущий DataSet
                ConversionDBDataSet.Tables.Add(ConversionDBTable);
                //ConversionDBDataSet.Tables[0].TableName = "Table";
                // присваиваем гриду источник данных
                TranslationsView.ugeTranslTables.DataSource = ConversionDBDataSet;
                
                // получаем название перекодировки для сохранения файла и вывода заголовка
				TranslationsView.ugeTranslTables.SaveLoadFileName = fViewCtrl.Text.Replace('.', '_').Replace(" -> ", "_");
                TranslationsView.ugeTranslTables.ugData.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

                // название кнопки добавления новой записи
                TranslationsView.ugeTranslTables.ugData.DisplayLayout.Bands[0].AddButtonCaption = "Новая запись";
                // делаем активной первую запись
                if (TranslationsView.ugeTranslTables.ugData.Rows.Count > 0)
                    TranslationsView.ugeTranslTables.ugData.Rows[0].Activate();
                TranslationsView.ugeTranslTables.AllowAddNewRecords = true;
                TranslationsView.ugeTranslTables.AllowClearTable = true;
                TranslationsView.ugeTranslTables.AllowDeleteRows = true;
                TranslationsView.ugeTranslTables.AllowEditRows = true;
                TranslationsView.ugeTranslTables.AllowImportFromXML = true;

                CheckPermissions();
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
        }

		/// <summary>
		///  Обновление данных
		/// </summary>
		public override void ReloadData()
		{
			RefreshData();
		}

        private void RefreshData(params object[] moduleParams)
        {
            /*
            base.ReloadData();
            Workplace.OperationObj.Text = "Построение списка объектов";
            Workplace.OperationObj.StartOperation();
            TranslationsNavi.ugeTraslTablesNavi.ugData.BeginUpdate();
            TranslationsNavi.udsTranlationTables.Rows.Clear();
            TranslationsNavi.ugeTraslTablesNavi.ugData.DataSource = null;
            IConversionTableCollection ConvertTables = null;
            try
            {
                // Получаем коллекцию таблиц перекодировки
                ConvertTables = Workplace.ActiveScheme.ConversionTables;
                // В ней получаем коллекцию ключей. Через ключи получим доступ к таблицам перекодировки из коллекции
                // если нет ни одной таблицы перекодировки, то выходим
                ICollection keys = ConvertTables.Keys;

                TranslationsNavi.ugeTraslTablesNavi.DataSource = GetConversionsTable(ConvertTables);
                CommonMethods.HideUnusedButtons(TranslationsNavi.ugeTraslTablesNavi.utmMain);

                
                //if (TranslationsNavi.ugeTraslTablesNavi.ugData.Rows.Count > 0)
                //{
                //    if (!CommonMethods.SelectRow(TranslationsNavi.ugeTraslTablesNavi.ugData, TranslationsNavi.ugeTraslTablesNavi.ugData.DisplayLayout.Bands[0].Columns[6].Key,
                //        TranslationsNavi.ugeTraslTablesNavi.ugData.DisplayLayout.Bands[0].Columns[2].Key, moduleParams))
                //        TranslationsNavi.ugeTraslTablesNavi.ugData.Rows[0].Activate();
                //}
            }
            finally
            {
                ConvertTables = null;
                TranslationsNavi.ugeTraslTablesNavi.ugData.EndUpdate();
                Workplace.OperationObj.StopOperation();
            }
            */
        }


        private DataTable GetConversionsTable(IConversionTableCollection convertTables)
        {
            DataTable tmpTable = convertTables.GetDataTable();
            tmpTable.Columns.Add("FULLTRANSLTABLENAME", typeof(string));
            foreach (DataRow row in tmpTable.Rows)
            {
                row["FULLTRANSLTABLENAME"] = row[0] + "_" + row[6];
            }

            DataTable conversions = new DataTable();
            conversions.Columns.Add("Name", typeof(string));
            conversions.Columns.Add("Rule", typeof(string));
            conversions.Columns.Add("DataSemantic", typeof(string));
            conversions.Columns.Add("DataCaption", typeof(string));
            conversions.Columns.Add("BridgeSemantic", typeof(string));
            conversions.Columns.Add("BridgeCaption", typeof(string));
            conversions.Columns.Add("ObjectKey", typeof(string));

            foreach (DataRow row in tmpTable.Rows)
            {
                conversions.Rows.Add(row[0], row[5], row[1], row[2], row[3], row[4], row[6]);
            }

            tmpTable.Rows.Clear();
            tmpTable.Dispose();

            return conversions;
        }


        /// <summary>
        /// получение списка перекодировок
        /// </summary>
		private void RefreshData()
		{
            RefreshData(activeTranslationsParams);
            //CheckPermissions();
		}

		/// <summary>
		///  Сохранение изменений
		/// </summary>
		public override void SaveChanges()
		{
			
		}

		/// <summary>
		///  Отмена всех изменений
		/// </summary>
		public override void CancelChanges()
		{

		}

		public override void InternalFinalize()
		{
			if (duConversionTable != null)
			{
				duConversionTable.Dispose();
				duConversionTable = null;
			}
			if (CurrentConversionTable != null)
			{
				//CurrentConversionTable.Dispose();
				CurrentConversionTable = null;
			}
            protocol.Dispose();
			ConversionDBTable = null;
		}

		/// <summary>
		///  ставит нужную картинку на строку в зависимости от состояния
		/// </summary>
		/// <param name="row"></param>
		/// <param name="rowState"></param>
		private void SetRowAppearance(Infragistics.Win.UltraWinGrid.UltraGridRow row, LocalRowState rowState)
		{
			
		}

        /// <summary>
        /// проверка заполнения полей всех записей перекодировки
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool CheckNotNullValue(ref string errorMsg)
        {
            CC.GridColumnsStates currentState = cashedColumnsSettings[currentObjName];
            ConversionDBTable.BeginLoadData();
            try
            {
                foreach (DataRow row in ConversionDBTable.Rows)
                {
                    for (int j = 1; j <= row.ItemArray.Length - 1; j++)
                    {
                        CC.GridColumnState state = currentState[ConversionDBTable.Columns[j].Caption];
                        bool nulableField = state.IsNullable;
                        if (row.IsNull(j) || row[j].ToString() == string.Empty)
                        {
                            // добавим значение по умалчанию
                            if (state.DefaultValue != null)
                                row[j] = state.DefaultValue;
                            // если все таки не заполнено и 
                            if (!nulableField && ((row.IsNull(j) || row[j].ToString() == string.Empty)))
                            {
                                errorMsg = string.Format("Обязательное поле '{1}' не заполнено", row["ID"], GetRusColumnCaptionFromEng(ConversionDBTable.Columns[j].Caption));
                                return false;
                            }
                        }
                    }
                }
            }
            finally
            {
                ConversionDBTable.EndLoadData();
            }
            return true;
        }

        /// <summary>
        /// получение русского названия колонки
        /// </summary>
        /// <param name="engColumnCaption"></param>
        /// <returns></returns>
        string GetRusColumnCaptionFromEng(string engColumnCaption)
        {
            IAssociation currAssociate = (IAssociation)this.Workplace.ActiveScheme.Associations[CurrentConversionTable.Name];
            IDataAttribute attr = null;
            string clsName = string.Empty;

            if (engColumnCaption != string.Empty && engColumnCaption != "ID")
            {
                if (engColumnCaption.Contains(">"))
                {
                    clsName = engColumnCaption.Replace(">", string.Empty);
                    attr = currAssociate.RoleData.Attributes[clsName];
                    return ">" + attr.Caption;
                }

                if (engColumnCaption.Contains("<"))
                {
                    clsName = engColumnCaption.Replace("<", string.Empty);
                    attr = currAssociate.RoleBridge.Attributes[clsName];
                    return "<" + attr.Caption;
                }
            }
            return string.Empty;
        }

        #region Права на перекодировки

        private void CheckPermissions()
        {
            bool canAddToConversion = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject(currentAssociation.ObjectKey,
                (int)AssociateOperations.AddRecordIntoBridgeTable, false);
            bool canDeleteToConversion = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject(currentAssociation.ObjectKey,
                (int)AssociateOperations.DelRecordFromBridgeTable, false);

            TranslationsView.ugeTranslTables.AllowDeleteRows = canDeleteToConversion;
            TranslationsView.ugeTranslTables.AllowAddNewRecords = canAddToConversion;
            TranslationsView.ugeTranslTables.AllowImportFromXML = true;
        }

        #endregion

        #region показ аудита

        private enum AuditShowObject { Row, SchemeObject };

        private AuditShowObject _auditShowObject;

        private void cmsAudit_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "auditToolStripMenuItem")
            {
                TranslationsView.cmsAudit.Hide();
                switch (_auditShowObject)
                {
                    case AuditShowObject.Row:
                        frmAuditModal.ShowAudit(this.Workplace, this.CurrentConversionTable.FullName, this.captionString, CC.UltraGridHelper.GetActiveID(TranslationsView.ugeTranslTables.ugData), AuditShowObjects.RowObject);
                        break;
                    case AuditShowObject.SchemeObject:
                        frmAuditModal.ShowAudit(this.Workplace, this.CurrentConversionTable.FullName, this.captionString, -1, AuditShowObjects.ClsObject);
                        break;
                }
            }
        }

        // показывает, что курсор находится на элементе выбора записей в гриде с данными
        private bool _activeUIElementIsRow = false;
        // показывает, что курсор находится на элементе выюлра записей в навигационном гриде
        private bool _activeUIElementIsSchemeObject = false;

        private void ugData_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (!(e.Element is RowSelectorUIElement))
                _activeUIElementIsRow = false;
            if (e.Element is RowSelectorUIElement)
            {
                _activeUIElementIsRow = true;
                if (!this.TranslationsView.ugeTranslTables.Focused)
                    this.TranslationsView.ugeTranslTables.Focus();
            }
        }

        private void ugData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_activeUIElementIsRow)
                {
                    _auditShowObject = AuditShowObject.Row;
                    TranslationsView.cmsAudit.Show(TranslationsView.ugeTranslTables.ugData.PointToScreen(e.Location));
                }
            }
        }

        #endregion
    }

    public struct GridColumsParams
    {
        public int ColumnPosition;

        public int ColumnWidth;

        public bool ColumnWisible;

        public string ColumnName;

    }
}