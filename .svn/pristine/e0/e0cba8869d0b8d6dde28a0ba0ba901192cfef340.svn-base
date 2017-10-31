using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common.Controls;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        #region Методы и свойства для работы со списком источников данных
        // активный фильтр на источники данных (по умолчанию отключен)
        #region данные по текущему источнику
        // ID активного источника данных
        private int currentDataSourceID;
        public int CurrentDataSourceID
        {
            get { return currentDataSourceID; }
            set
            { 
                currentDataSourceID = value;
                PassPresentationKeyToContext();
            }
        }
        private int _currentDataSourceYear = 0;
        /// <summary>
        /// год, источник по этому году по возможности
        /// будет выставляться при открытии классификатора
        /// </summary>
        public int CurrentDataSourceYear
        {
            get { return _currentDataSourceYear; }
            set { _currentDataSourceYear = value; }
        }

        private int currentDataSourceDataCode;
        private string currentDataSourceSuplierCode;
        #endregion
        // текстовка надписи для выбора нового источника
        private const string NewDataSourceName = "Новый источник ...";
        // текстовка надписи для выбора новой вурсии классификатора
        private const string NewDataVersionName = "Новая версия ...";

        private const string NewTaskName = "Выбрать задачу...";

        // текущая выбранная задача
        internal int CurrentTaskID = -1;


        // служебные константы для установки фильтра на источники
        // фильтр на пустые (SourceID is null)
        private const int NullDataSource = -1;
        // выбор нового источника (инициирует поках формы с выбором источников)
        private const int NewDataSource = -2;
        // нет источников, пустой фильтр (1 = 1)
        private const int NoDataSources = -3;
        // Признак текущей версии классификатора
        private bool isCurrentVersion = false;
        public bool IsCurrentVersion
        {
            get { return isCurrentVersion; }
            set { isCurrentVersion = value; }
        }

        // Определение наличия у объекта источников данных
        public virtual bool HasDataSources()
        {
            return false;
        }

        private bool _restoreSameDataSet = true;
        /// <summary>
        /// показывает, нужно ли восстанавливать текущий источник данных в других интерфейсах
        /// </summary>
        public bool RestoreDataSet
        {
            get { return _restoreSameDataSet; }
            set { _restoreSameDataSet = value; }
        }

        // Формирование строки фильтра на источники данных
        protected string GetDataSourcesFilter()
        {
            switch (CurrentDataSourceID)
            {
                case NoDataSources:
                    return "(1 = 1)";
                case NullDataSource:
                    return "(SourceID is NULL)";
                default:
                    return String.Format("(SourceID = {0})", CurrentDataSourceID);
            }
        }

        private int GetDataSourceID()
        {
            switch (CurrentDataSourceID)
            {
                case NoDataSources:
                case NullDataSource:
                    return -1;
                default:
                    return CurrentDataSourceID;
            }
        }

        /// <summary>
        /// Возвращает текущую строку.
        /// </summary>
        /// <returns></returns>
        public DataRow GetActiveDataRow()
        {
            int activeRowID = UltraGridHelper.GetActiveID(vo.ugeCls._ugData);
            DataRow[] rows = dsObjData.Tables[0].Select(String.Format("ID = {0}", activeRowID));
            return rows.GetLength(0) > 0 ? rows[0] : null;
        }

        /// <summary>
        /// Заполнение списка источников данных активного объекта-поставщика данных
        /// </summary>
        /// <param name="obj">активный объект</param>
        protected virtual void AttachDataSources(IEntity obj)
        {
            // если у объекта нет источников данных - выходим
            if (!HasDataSources())
            {
                CurrentDataSourceID = NoDataSources;
                return;
            }
            // иначе - получаем список источников в generic-коллекцию
            Dictionary<int, string> dataSourcesNames;
            switch (clsClassType)
            {
                case ClassTypes.clsFactData:
                    dataSourcesNames = ((IFactTable)obj).GetDataSourcesNames();
                    break;
                default:
                    dataSourcesNames = ((IClassifier)obj).GetDataSourcesNames();
                    break;
            }

            // заполняем список
            ComboBoxTool cbDataSources = (ComboBoxTool)vo.utbToolbarManager.Tools["cbDataSources"];
            vo.utbToolbarManager.BeginUpdate();
            try
            {
                ValueList vl = cbDataSources.ValueList;
                vl.ValueListItems.Clear();
                vl.DisplayStyle = ValueListDisplayStyle.DisplayTextAndPicture;
                // всегда добавляем пункт для выбора нового источника
                if (allowAddNewDataSource)
                    if (ActiveDataObj.SubClassType != SubClassTypes.Pump && !InInplaceMode)
                    {
                        bool needAddNewVersion = true;
                        if (obj is IBridgeClassifier && obj.Associations.Where(keyValuePair => keyValuePair.Value.AssociationClassType == AssociationClassTypes.BridgeBridge).Count() == 0)
                        {
                            needAddNewVersion = false;
                        }

                        if (needAddNewVersion)
                            vl.ValueListItems.Add(NewDataSource,
                                              (ActiveDataObj is IFactTable) ? NewDataSourceName : NewDataVersionName);
                    }

                // пишем все остальные
                foreach (KeyValuePair<int, string> kvp in dataSourcesNames)
                {
                    ValueListItem valueItem = new ValueListItem();
                    DataTable dtSource = DataSourcesHelper.GetDataSourcesInfo(kvp.Key, Workplace.ActiveScheme);
                    if (dtSource.Rows.Count > 0)
                    {
                        if (!(Convert.ToBoolean(dtSource.Rows[0]["Locked"])))
                        {
                            valueItem.Appearance.Image = vo.ilImages.Images[12];
                        }
                        else
                        {
                            valueItem.Appearance.Image = vo.ilImages.Images[13];
                        }
                    }

                    if (kvp.Key == 0)
                    {
                        valueItem.Appearance.Image = vo.ilImages.Images[12];
                    }

                    // информация о версии 
                    IDataVersion dataVersion =
                        Workplace.ActiveScheme.DataVersionsManager.DataVersions[obj.ObjectKey, kvp.Key];
                    if (dataVersion != null)
                    {
                        if (dataVersion.IsCurrent)
                        {
                            valueItem.Appearance.Image = Properties.Resources.currentVersion;
                        }
                    }

                    valueItem.DataValue = kvp.Key;
                    valueItem.DisplayText = kvp.Value;
                    vl.ValueListItems.Add(valueItem); //(kvp.Key, kvp.Value);
                }
                // сортируем список по возрастанию отображаемого текста
                vl.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
                // если в списке присутствуют источники (кроме стандартых)
                ValueListItem item = null;

                // если есть текущая версия, то выбираем ее
                if(!SaveLastSelectedDataSource)
                {
                    for (int i = 0; i < vl.ValueListItems.Count; i++)
                    {
                        // информация о версии 
                        IDataVersion dataVersion =
                            Workplace.ActiveScheme.DataVersionsManager.DataVersions[
                                obj.ObjectKey, Convert.ToInt32(vl.ValueListItems[i].DataValue)];
                        if (dataVersion != null)
                        {
                            if (dataVersion.IsCurrent)
                            {
                                item = vl.ValueListItems[i];
                                ChangeDataSource(Convert.ToInt32(vl.ValueListItems[i].DataValue), null);
                                break;
                            }
                        }
                    }
                }

                if (item == null)
                {
                    // Если уже был выбран какой то источник из списка, например если обновили данные, то вернемся к нему
                    for (int i = 0; i < vl.ValueListItems.Count; i++)
                    {
                        item = vl.ValueListItems[i];

                        if (Convert.ToInt32(item.DataValue) == CurrentSourceID)
                        {
                            break;
                        }

                        item = null;
                    }
                }

                //выбор источника по нескольким параметрам 
                if (item == null || (DataSourceContext.CurrentDataSourceYear != CurrentDataSourceYear && !InInplaceMode))
                {
                    DataTable dtSource = null;
                    int currentYear = 0;
                    if (InInplaceMode && CurrentDataSourceYear <= 0)
                        currentYear = DataSourceContext.CurrentDataSourceYear;
                    else
                        currentYear = CurrentDataSourceYear;

                    for (int i = 0; i < vl.ValueListItems.Count; i++)
                    {
                        int sourceID = Convert.ToInt32(vl.ValueListItems[i].DataValue);
                        dtSource = DataSourcesHelper.GetDataSourcesInfo(sourceID, Workplace.ActiveScheme);
                        if (dtSource.Rows.Count > 0)
                        {
                            int dataSourceYear = Convert.ToInt32(dtSource.Rows[0]["Year"]);
                            int dataSourceDataCode = Convert.ToInt32(dtSource.Rows[0]["DataCode"]);
                            string dataSourceSuplierCode = Convert.ToString(dtSource.Rows[0]["SupplierCode"]);
                            if (currentYear == dataSourceYear && dataSourceDataCode == DataSourceContext.CurrentDataSourceDataCode &&
                                dataSourceSuplierCode == DataSourceContext.CurrentDataSourceSuplierCode)
                            {
                                item = vl.ValueListItems[i];
                                break;
                            }

                            if (dataSourceYear == currentYear)
                            {
                                item = vl.ValueListItems[i];
                                break;
                            }
                        }
                    }
                }
                
                // Если вдруг не нашли такого, то устанавливаем в качестве активного последний нестандартный 
                List<ValueListItem> items = new List<ValueListItem>();
                if (item == null)
                {
                    for (int i = 0; i < vl.ValueListItems.Count; i++)
                    {
                        item = vl.ValueListItems[i];
                        if (Convert.ToInt32(item.DataValue) >= 0)
                            items.Add(item);
                    }

                    item = items.Count > 0 ? items[items.Count - 1] : null;
                }

                if (item != null)
                {
                    vl.SelectedItem = item;
                    cbDataSources.Text = item.DisplayText;
                    DataTable dtDataSource = DataSourcesHelper.GetDataSourcesInfo(Convert.ToInt32(item.DataValue), Workplace.ActiveScheme);
                    if (dtDataSource.Rows.Count > 0)
                    {
                        CurrentDataSourceID = Convert.ToInt32(dtDataSource.Rows[0]["ID"]);
                        CurrentDataSourceYear = Convert.ToInt32(dtDataSource.Rows[0]["Year"]);
                        currentDataSourceDataCode = Convert.ToInt32(dtDataSource.Rows[0]["DataCode"]);
                        currentDataSourceSuplierCode = Convert.ToString(dtDataSource.Rows[0]["SupplierCode"]);
                    }
                }
                else
                {
                    CurrentDataSourceID = NullDataSource;
                    if (vl.ValueListItems.Count > 0)
                        if (ActiveDataObj.SubClassType != SubClassTypes.Pump && !InInplaceMode)
                        {
                            vl.SelectedItem = vl.ValueListItems[0];
                            cbDataSources.Text = vl.ValueListItems[0].DisplayText;
                        }
                }

                // в любом случае фиксируем выбранный источник данных
                if (/*!InInplaceMode && */CurrentDataSourceID >= 0 && RestoreDataSet)
                {
                    FixDataSource();
                }
            }
            finally
            {
                vo.utbToolbarManager.EndUpdate();
            }
        }

        public void FixDataSource()
        {
            DataSourceContext.CurrentDataSourceID = CurrentDataSourceID;
            DataSourceContext.CurrentDataSourceYear = CurrentDataSourceYear;
            DataSourceContext.CurrentDataSourceDataCode = currentDataSourceDataCode;
            DataSourceContext.CurrentDataSourceSuplierCode = currentDataSourceSuplierCode;
        }

        #region Добавление списка задач, которые находятся в состоянии редактирования текущим пользователем

        DataTable taskTable = null;
        private string taskID = string.Empty;

        /// <summary>
        /// заполнение списка задач текущего пользователя
        /// </summary>
        void AttachTaskID()
        {
            try
            {
                taskTable = Workplace.ActiveScheme.TaskManager.Tasks.GetCurrentUserLockedTasks();
            }
            catch {}
            RefreshTaskIDVisible();
        }

        /// <summary>
        /// обновляется отображение строки выбора задачи
        /// </summary>
        internal void RefreshTaskIDVisible()
        {
            if (taskTable == null) return;
            ComboBoxTool cbTaskSelect = (ComboBoxTool)vo.utbToolbarManager.Tools["TaskIDCombo"];
            cbTaskSelect.ValueList.ValueListItems.Clear();
            ValueListItem val = cbTaskSelect.ValueList.ValueListItems.Add(-1, NewTaskName);

            foreach (DataRow row in taskTable.Rows)
            {
                cbTaskSelect.ValueList.ValueListItems.Add(row["ID"], row["ID"].ToString() + " <" + row["State"].ToString() + "> " + row["Headline"].ToString());
            }
            if (CurrentTaskID == -1)
            {
                cbTaskSelect.ValueList.SelectedIndex = 0;
                cbTaskSelect.Text = NewTaskName;
            }
            else
            {
                for (int i = 0; i <= cbTaskSelect.ValueList.ValueListItems.Count - 1; i++)
                {
                    if (Convert.ToInt32(cbTaskSelect.ValueList.ValueListItems[i].DataValue) == CurrentTaskID)
                    {
                        cbTaskSelect.ValueList.SelectedItem = cbTaskSelect.ValueList.ValueListItems[i];
                        cbTaskSelect.Text = cbTaskSelect.ValueList.ValueListItems[i].DisplayText;
                        break;
                    }

                }
            }
            SetTaskID();
        }

        /// <summary>
        /// получение ID выбранной задачи и отображение его
        /// </summary>
        private void SetTaskID()
        {
            LabelTool ltTaskID = (LabelTool)vo.utbToolbarManager.Tools["lbTaskID"];
            if (CurrentTaskID > 0)
                taskID = string.Format("(TaskID = {0})", CurrentTaskID);
            else
                taskID = "(TaskID is NULL)";
            ltTaskID.SharedProps.Caption = taskID;
        }

        #endregion

        /// <summary>
        /// Установка активного фильтра на источники данных
        /// </summary>
        private void SetDataSourcesFilter()
        {
            LabelTool laSource = (LabelTool)vo.utbToolbarManager.Tools["laSourceID"];
            laSource.SharedProps.Caption = GetDataSourcesFilter();
        }

        private void SetVersionID()
        {
            LabelTool lbPresentation = (LabelTool)vo.utbToolbarManager.Tools["lbPresentationKey"];

            if (ActiveDataObj is IFactTable || ActiveDataObj.Presentations.Count == 0)
            {
                lbPresentation.SharedProps.Caption = String.Empty;

                return;
            }

            lbPresentation.SharedProps.Caption = (string.Compare(GetCurrentPresentation(activeDataObj), string.Empty) == 0 || activeDataObj.Presentations[GetCurrentPresentation(activeDataObj)] == null)
                ? "(Presentation is NULL)"
                : String.Format("(Версия структуры: {0})", activeDataObj.Presentations[GetCurrentPresentation(activeDataObj)].Name);
        }

        /// <summary>
        /// Дейстивия, выполняемые перед тем выбором источника данных или задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void utbToolbarManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            ComboBoxTool cb = (ComboBoxTool)vo.utbToolbarManager.Tools["TaskIDCombo"];
            RefreshTaskIDVisible();
            if (cb.SharedProps.Visible)
            {
                if (cb.ValueList.ValueListItems.Count > 0)
                {
                    ValueListItem selectTaskItem = (ValueListItem)cb.ValueList.ValueListItems[0];
                    cb.ValueList.ValueListItems.Remove(selectTaskItem);
                }
            }
        }

        private bool SetExistDataSource(int tmpID)
        {
            ComboBoxTool cbSource = (ComboBoxTool)vo.utbToolbarManager.Tools["cbDataSources"];
            for (int i = 0; i <= cbSource.ValueList.ValueListItems.Count - 1; i++)
            {
                ValueListItem checkItem = cbSource.ValueList.ValueListItems[i];
                if (Convert.ToInt32(checkItem.DataValue) == tmpID)
                {
                    cbSource.ValueList.SelectedItem = checkItem;
                    cbSource.Text = checkItem.DisplayText;
                    return true;
                }
            }
            return false;
        }


        public void TrySetDataSource(int sourceId)
        {
            if (!HasDataSources()) return;

            int selectedId = 0;
            ComboBoxTool cbSource = (ComboBoxTool)vo.utbToolbarManager.Tools["cbDataSources"];
            for (int i = 0; i <= cbSource.ValueList.ValueListItems.Count - 1; i++)
            {
                ValueListItem checkItem = cbSource.ValueList.ValueListItems[i];
                selectedId = System.Convert.ToInt32(checkItem.DataValue);
                if (selectedId == sourceId)
                {

                    ChangeDataSource(selectedId, null);
                    cbSource.Text = checkItem.DisplayText;
                    return;
                }
            }
            if (selectedId >= 0 && RestoreDataSet)
                DataSourceContext.CurrentDataSourceID = selectedId;
        }


        // Обработчик выбора источника данных
        public void utbToolbarManager_AfterToolCloseup(object sender, Infragistics.Win.UltraWinToolbars.ToolDropdownEventArgs e)
        {
            if (e.Tool == null) return;
            ValueListItem activeItem = null;
            ComboBoxTool cb = null;
            switch (e.Tool.Key)
            {
                case "cbDataSources":
                    // сохраняем изменения по предыдущему источнику
                    SaveDataThenExit();
                    // получаем ID выбранного источника
                    cb = (ComboBoxTool)e.Tool;
                    activeItem = (ValueListItem)cb.ValueList.SelectedItem;
                    if (activeItem == null) break;
                    int tmpID = System.Convert.ToInt32(activeItem.DataValue);

                    ChangeDataSource(tmpID, cb);
                    break;
                case "TaskIDCombo":
                    cb = (ComboBoxTool)e.Tool;
                    // если у пользователя нет его задач, то выходим
                    if (cb.ValueList.ValueListItems.Count <= 0)
                    {
                        return;
                    }
                    ValueListItem selectTaskItem = (ValueListItem)cb.ValueList.ValueListItems[0];

                    activeItem = (ValueListItem)cb.ValueList.SelectedItem;
                    // если ничего не выбрано, ставим ID задачи -1
                    if (activeItem == null)
                    {
                        AttachTaskID();
                        CurrentTaskID = -1;
                    }
                    else
                        CurrentTaskID = System.Convert.ToInt32(activeItem.DataValue);
                    SetTaskID();
                    if (CurrentTaskID < 0)
                        AttachTaskID();

                    //vo.ugeCls.ReinitializeLayout();
                    if ((CheckAllowAddNew() == AllowAddNew.Yes))
                    {
                        SetPermissionsToClassifier(vo.ugeCls);
                        //vo.ugeCls.ReinitializeLayout();
                        if (isMasterDetail)
                            activeDetailGrid.IsReadOnly = false;
                    }
                    else
                    {
                        vo.ugeCls.IsReadOnly = true;
                        if (CheckAllowImportFromXml())
                        {
                            vo.ugeCls._utmMain.Tools["menuLoad"].SharedProps.Enabled = true;
                            vo.ugeCls._utmMain.Tools["ImportFromXML"].SharedProps.Enabled = true;
                        }
                    }

                    break;
            }
            vo.ugeCls.Focus();
            RefreshTaskIDVisible();
        }

        /// <summary>
        /// изменение источника данных
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="comboBox"></param>
        private void ChangeDataSource(int sourceId, ComboBoxTool comboBox)
        {
             // если выбрали текущий источник, то ничего не делаем
            if (CurrentDataSourceID == sourceId) return;
            // показывает, был ли выбран новый источник и стоит ли обновлять данные
            bool changeSource = false;
            switch (sourceId)
            {
                // если это выбор новго источника - показываем форму для выбора
                case NewDataSource:

                    string versionName = String.Empty;
                    string presentationKey = String.Empty;

                    if (!(activeObject is IFactTable))
                    {
                        if (Wizard.CreateVersionForm.ShowCreateVersionWizard(Workplace.ActiveScheme, activeDataObj,
                                                                             ref versionName, ref sourceId,
                                                                             ref presentationKey))
                        {
                            IDataVersion version = Workplace.ActiveScheme.DataVersionsManager.DataVersions.Create();
                            version.ObjectKey = activeDataObj.ObjectKey;
                            version.Name = versionName;
                            version.SourceID = sourceId;
                            version.PresentationKey = String.IsNullOrEmpty(presentationKey) ? Guid.Empty.ToString() : presentationKey;
                            version.IsCurrent = false;
                            Workplace.ActiveScheme.DataVersionsManager.DataVersions.Add(version);
                            changeSource = GetChangeSource(sourceId, comboBox);
                        }
                        else
                        {
                            SetExistDataSource(CurrentDataSourceID);
                            SetDataSourcesFilter();
                            SetVersionID();
                        }
                    }
                    else
                    {
                        if (DataSourcesHelper.SelectDataSources(Workplace.ActiveScheme, ((IDataSourceDividedClass)activeObject).DataSourceKinds, ref sourceId))
                            changeSource = GetChangeSource(sourceId, comboBox);
                    }
                    break;
                // иначе
                default:
                    // запоминаем ID
                    CurrentDataSourceID = sourceId;
                    // формируем строку фильтра
                    GetDataSourcesFilter();
                    changeSource = true;
                    break;
            }
            if (changeSource)
            {
                this.Workplace.OperationObj.Text = "Запрос данных";
                this.Workplace.OperationObj.StartOperation();
                try
                {
                    HierarchyInfo hi = vo.ugeCls.HierarchyInfo;
                    if (hi.LevelsCount > 1)
                        hi.CurViewState = ViewState.Hierarchy;
                    else
                        hi.CurViewState = ViewState.Flat;

                    InternalLoadData();
                    vo.ugeCls.DataSource = dsObjData;
                    SetDataSourcesFilter();
                    SetVersionID();
                    UpdateToolbar();

                    if (ActiveDataObj.ClassType == ClassTypes.clsBridgeClassifier ||
                        ActiveDataObj.ClassType == ClassTypes.clsDataClassifier)
                        vo.ugeCls.utmMain.Tools["ExportToXML"].SharedProps.Enabled = true;
                    if (_AfterSourceSelect != null)
                        _AfterSourceSelect();
                    if ((CheckAllowAddNew() == AllowAddNew.Yes))
                    {
                        SetPermissionsToClassifier(vo.ugeCls);
                        //vo.ugeCls.ReinitializeLayout();
                    }
                    else
                        vo.ugeCls.IsReadOnly = true;

                    if (this.selectDataSource != null)
                        selectDataSource();

                    DataTable dtDataSource = DataSourcesHelper.GetDataSourcesInfo(CurrentDataSourceID, Workplace.ActiveScheme);
                    // Проверяем блокировку.
                    if (!vo.ugeCls.IsReadOnly && dtDataSource.Rows.Count > 0)
                        vo.ugeCls.IsReadOnly = Convert.ToBoolean(dtDataSource.Rows[0]["Locked"]);

                    if (CurrentDataSourceID > 0 && RestoreDataSet)
                    {
                        CurrentDataSourceYear = Convert.ToInt32(dtDataSource.Rows[0]["Year"]);
                        currentDataSourceDataCode = Convert.ToInt32(dtDataSource.Rows[0]["DataCode"]);
                        currentDataSourceSuplierCode = Convert.ToString(dtDataSource.Rows[0]["SupplierCode"]);
                        FixDataSource();
                    }

                    if (CurrentSourceID == 0)
                    {
                        FixDataSource();
                    }

                    GetFileName();
                    if (dsObjData.Tables[0].Rows.Count > 0)
                        vo.ugeCls.ugData.Rows[0].Activate();
                    else
                        // для деталей если нету записей
                        if (isMasterDetail)
                        {
                        	LoadDetailData(GetActiveMasterRowID());
                        }

                    if (InAssociateMode)
                    {
                        vo.ugeCls.IsReadOnly = false;
                        vo.ugeCls.AllowDeleteRows = false;
                        vo.ugeCls.AllowAddNewRecords = false;
                        vo.ugeCls.AllowClearTable = false;
                        vo.ugeCls.AllowEditRows = true;
                    }
                }
                finally
                {
                    this.Workplace.OperationObj.StopOperation();
                }
            }
        }

        private bool GetChangeSource(int sourceId, ComboBoxTool comboBox)
        {
            bool changeSource;
            vo.ugeCls.ResetServerFilter();

            changeSource = true;
            // запоминаем ID
            CurrentDataSourceID = sourceId;
            // формируем строку фильтра
            GetDataSourcesFilter();
            bool SelectedDataSourceExist = false;

            SelectedDataSourceExist = SetExistDataSource(sourceId);
            if (!SelectedDataSourceExist)
            {
                string str = this.Workplace.ActiveScheme.DataSourceManager.GetDataSourceName(sourceId);
                ValueListItem item = comboBox.ValueList.ValueListItems.Add(sourceId, str);
                comboBox.ValueList.SelectedItem = item;
                comboBox.Text = item.DisplayText;
            }
            return changeSource;
        }

        /// <summary>
        /// Передача Guid представления в контекст
        /// </summary>
        private void PassPresentationKeyToContext()
        {
            LogicalCallContextData context = LogicalCallContextData.GetContext();

            // Сохраняем в контекст guid текущей версии классификатора
            // получаем текущую версию классификатора
            IDataVersion activeVersion = Workplace.ActiveScheme.DataVersionsManager.DataVersions[activeDataObj.ObjectKey, currentDataSourceID];
            // если для данного классификатора по данному источнику есть версия, то передаем в контекст presentationKey
            if (activeVersion != null)
            {
                IsCurrentVersion = activeVersion.IsCurrent;

                // сохраняем в контекст, только если задано конкретное представление
                if (!Equals(new Guid(activeVersion.PresentationKey), Guid.Empty))
                {
                    context[String.Format("{0}.Presentation", activeDataObj.FullDBName)] = activeVersion.PresentationKey;
                }
                    // в противном случае, если запись была в контексте, очищаем ее
                else if (context[String.Format("{0}.Presentation", activeDataObj.FullDBName)] != null)
                {
                    context[String.Format("{0}.Presentation", activeDataObj.FullDBName)] = null;
                }
            }

            LogicalCallContextData.SetContext(context);
        }

        #endregion
    }
}