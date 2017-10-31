using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Presentations;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Resources;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Domain;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class FinSourcePlanningUI : BaseClsUI
    {
        private IFinSourceBaseService service;

        //internal FinSourcesRererencesUtils finSourcesRererencesUtils;

        public FinSourcePlanningUI(IFinSourceBaseService service)
            : base(service.Data)
        {
            this.service = service;
            clsClassType = ClassTypes.clsFactData;

            InfragisticsRusification.LocalizeAll();
        }

        public FinSourcePlanningUI(IFinSourceBaseService service, string key)
            : base(service.Data, key)
        {
            this.service = service;
            clsClassType = ClassTypes.clsFactData;

            InfragisticsRusification.LocalizeAll();
        }

        // содержит все визуализаторы, которые при проверке загорятся
        private MessagesVisualizator messagesVisualizator;
        internal MessagesVisualizator MessagesVisualizator
        {
            get { return messagesVisualizator; }
            set { messagesVisualizator = value; }
        }

        public IFinSourceBaseService Service
        {
            get { return service; }
        }

        internal ImageList il;

        /// <summary>
        /// Инициализация.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            il = new ImageList();
            il.TransparentColor = Color.Magenta;
            il.Images.Add(Resources.ru.calculator);
            il.Images.Add(Resources.ru.CheckDisinRules);
            il.Images.Add(Resources.ru.Succefull);
            il.Images.Add(Resources.ru.Error);

            il.Images.Add(Resources.ru.unknown);
            il.Images.Add(Resources.ru.current);
            il.Images.Add(Resources.ru.planing);
            il.Images.Add(Resources.ru.closed);
            il.Images.Add(Resources.ru.Active);
            il.Images.Add(Resources.ru.Archive);

            vo.ugeCls.ugData.DisplayLayout.Override.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;

            ((BaseClsView)ViewCtrl).utcDataCls.Tabs[1].Visible = false;

            #region Инициализация панели инструментов
            UltraToolbar utbFinSourcePlanning = new UltraToolbar("FinSourcePlanning");
            utbFinSourcePlanning.DockedColumn = 0;
            utbFinSourcePlanning.DockedRow = 1;
            utbFinSourcePlanning.Text = "FinSourcePlanning";
            utbFinSourcePlanning.Visible = true;

            UltraToolbar utbMain = vo.ugeCls.utmMain.Toolbars["utbMain"];
            ButtonTool addNew = CommandService.AttachToolbarTool(new AddNewRowCommand(), utbMain);
            addNew.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Document_Add_icon;
            vo.ugeCls.utmMain.Toolbars["utbMain"].Tools["btnAddNew"].InstanceProps.IsFirstInGroup = true;
            ButtonTool delRow = (ButtonTool)vo.ugeCls.utmMain.Tools["DeleteSelectedRows"];
            delRow.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Document_Delete_icon;
            utbMain.Tools.Remove(delRow);
            utbMain.Tools.Add(delRow);
            vo.ugeCls.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;

            #region Панель выбора варианта
            LabelTool lbSelectedVariantName = new LabelTool("lbSelectedVariantName");
            lbSelectedVariantName.InstanceProps.Caption = "Вариант не задан";
            lbSelectedVariantName.InstanceProps.AppearancesSmall.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
            utbFinSourcePlanning.NonInheritedTools.AddRange(new ToolBase[] { lbSelectedVariantName });
            #endregion Панель выбора варианта

            #region Вылетающий список операций

            PopupMenuTool pmtOperationsList = new PopupMenuTool("OperationsList");

            pmtOperationsList.InstanceProps.IsFirstInGroup = true;
            pmtOperationsList.SharedProps.Caption = "Сформировать";
            pmtOperationsList.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            pmtOperationsList.SharedProps.ToolTipText = "Сформировать";
            pmtOperationsList.SharedProps.AppearancesSmall.Appearance = new Infragistics.Win.Appearance();
            pmtOperationsList.Key = "OperationsList";

            utbFinSourcePlanning.NonInheritedTools.AddRange(new ToolBase[] { pmtOperationsList });

            // список отчетов
            PopupMenuTool pmtTemplates = new PopupMenuTool("Templates");
            pmtTemplates.InstanceProps.IsFirstInGroup = true;
            pmtTemplates.SharedProps.Caption = "Отчеты";
            pmtTemplates.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            pmtTemplates.SharedProps.ToolTipText = "Отчеты";
            pmtTemplates.Key = "Templates";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmtTemplates);
            utbFinSourcePlanning.NonInheritedTools.AddRange(new ToolBase[] { pmtTemplates });

            #endregion Вылетающий список операций

            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars.AddRange(new UltraToolbar[] { utbFinSourcePlanning });

            #endregion Инициализация панели инструментов

            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ((BaseClsView)ViewCtrl).ugeCls.OnInitializeRow += new InitializeRow(ugeCls_OnInitializeRow);

            FinSourcePlanningNavigation.Instance.VariantChangedNew += new VariantEventHandler(Instance_VariantChangedNew);

            ((BaseClsView)ViewCtrl).ugeCls.OnBeforeRowDeactivate += new BeforeRowDeactivate(ugeCls_OnBeforeRowDeactivate);

            SetDataParams(FinSourcePlanningNavigation.Instance.CurrentVariantID,
                          FinSourcePlanningNavigation.Instance.CurrentSourceID);
            SetVariantCaption(FinSourcePlanningNavigation.Instance.CurrentVariantCaption, false);
            IsRefreshDetailCalculationList = true;
        }

        protected override void ugeCls_OnAfterRowActivate(object sender, EventArgs e)
        {
            base.ugeCls_OnAfterRowActivate(sender, e);
            if (ActiveDetailGrid == null)
                return;
            UltraToolbar utbMain = ActiveDetailGrid.utmMain.Toolbars["utbMain"];
            DataRow masterRow = GetActiveDataRow();
            utbMain.Tools["btnAddNew"].SharedProps.Enabled = !(masterRow == null || masterRow.RowState == DataRowState.Added || masterRow.RowState == DataRowState.Deleted);

        }

        internal virtual void HideCurrencyColumns(int currency)
        {
            
        }

        protected override void DetailGridSetup(UltraGridEx ugeDetail)
        {
            base.DetailGridSetup(ugeDetail);
            ugeDetail.ugData.DisplayLayout.Override.CellMultiLine = DefaultableBoolean.False;
            UltraToolbar utbMain = ugeDetail.utmMain.Toolbars["utbMain"];
            ButtonTool addNew = CommandService.AttachToolbarTool(new AddNewDetailRowCommand(ugeDetail), utbMain);
            addNew.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.add_icon;
            ugeDetail.utmMain.Toolbars["utbMain"].Tools["btnAddNew"].InstanceProps.IsFirstInGroup = true;
        }

        protected override GridColumnsStates ugeCls_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = base.ugeCls_OnGetGridColumnsState(sender);
            foreach (GridColumnState state in states.Values)
            {
                state.isWrapWord = false;
            }
            return states;
        }

        protected override GridColumnsStates ugeDetail_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = base.ugeDetail_OnGetGridColumnsState(sender);
            foreach (GridColumnState state in states.Values)
            {
                state.isWrapWord = false;
            }
            return states;
        }

        public override bool Refresh()
        {
            if (MessagesVisualizator != null)
            {
                MessagesVisualizator.Hide();
                MessagesVisualizator = null;
            }
            return base.Refresh();
        }

        private void ugeCls_OnBeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessagesVisualizator != null)
            {
                MessagesVisualizator.Hide();
                MessagesVisualizator = null;
            }
        }

        public DataRow GetDataRow(int ID)
        {
            DataRow[] rows = dsObjData.Tables[0].Select(String.Format("ID = {0}", ID));
            return rows.GetLength(0) > 0 ? rows[0] : null;
        }

        /// <summary>
        /// Возвращает загруженные в грид строчки.
        /// </summary>
        public DataRow[] GetDataRows()
        {
            return ((DataView)((BindingSource)vo.ugeCls.ugData.DataSource).DataSource).ToTable().Select();
        }

        protected override AllowAddNew CheckAllowAddNew()
        {
            return AllowAddNew.Yes;
        }

        // Определение наличия у объекта источников данных
        public override bool HasDataSources()
        {
            return true;
        }

        protected override void AttachDataSources(IEntity obj)
        {
            base.AttachDataSources(obj);
            CurrentDataSourceID = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            FixDataSource();
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = 
                String.Format(" {0} = {1}", "RefVariant", FinSourcePlanningNavigation.Instance.CurrentVariantID);
            AddFilter();
            filterStr = dataQuery;

            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            InfragisticsHelper.BurnTool(((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"].Tools["lbSelectedVariantName"], false);
            base.LoadData(sender, e);
            vo.ugeCls.ugData.DisplayLayout.AddNewBox.Hidden = true;
            vo.ugeCls.utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;
            ButtonTool delRow = (ButtonTool)vo.ugeCls.utmMain.Tools["DeleteSelectedRows"];
            delRow.SharedProps.ToolTipText = "Удалить договор";
            if (ActiveDetailGrid == null)
                return;
            UltraToolbar utbMain = ActiveDetailGrid.utmMain.Toolbars["utbMain"];
            DataRow masterRow = GetActiveDataRow();
            utbMain.Tools["btnAddNew"].SharedProps.Enabled = !(masterRow == null || masterRow.RowState == DataRowState.Added || masterRow.RowState == DataRowState.Deleted);
            SetDetailVisible(RefVariant);
            HideMasterId();
            vo.ugeCls.ugData.DisplayLayout.UseFixedHeaders = true;
            vo.ugeCls.ugData.DisplayLayout.Override.FixedHeaderIndicator = FixedHeaderIndicator.None;
            if (vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns.Exists("Num"))
                vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["Num"].Header.Fixed = true;
            if (vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups.Count > 0)
                vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups[1].Header.Fixed = true;

            if (vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns.Exists("OfficialNumber"))
                vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["OfficialNumber"].Header.Fixed = true;

            if (vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns.Exists("PlanStatus"))
                vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["PlanStatus"].Header.Fixed = true;
        }

        protected override void LoadDetailData(int masterValue)
        {
            base.LoadDetailData(masterValue);
            if (masterValue <= 0)
                return;
            DataRow masterRow = GetActiveDataRow();
            HideCurrencyColumns(Convert.ToInt32(masterRow["RefOKV"]));
            HideDetailId();
            if (IsRefreshDetailCalculationList)
                RefreshDetailCalculationList(activeDetailObject.ObjectKey);
        }

        public override void LoadLoadPersistence()
        {
            
        }

        protected override void LoadDetailPersistence()
        {
            
        }

        protected bool IsRefreshDetailCalculationList
        {
            get; set;
        }

        protected virtual void RefreshDetailCalculationList(string detailKey)
        {
 
        }

        protected override void AfterLoadDetailData(ref DataSet detail)
        {
            activeDetailGrid.ugData.DisplayLayout.AddNewBox.Hidden = true;
            activeDetailGrid.utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;
            UltraToolbar utbMain = activeDetailGrid.utmMain.Toolbars["utbMain"];
            ButtonTool delRow = (ButtonTool)activeDetailGrid.utmMain.Tools["DeleteSelectedRows"];
            delRow.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.delete_icon;
            utbMain.Tools.Remove(delRow);
            utbMain.Tools.Add(delRow);

            ButtonTool clearData = (ButtonTool)activeDetailGrid.utmMain.Tools["ClearCurrentTable"]; 
            utbMain.Tools.Remove(clearData);
            utbMain.Tools.Add(clearData);
            clearData.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Table_delete_icon;
        }

        internal virtual void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (!e.ReInitialize)
            {
                if (e.Row.Cells["RefVariant"].Value is DBNull)
                {
                    e.Row.Cells["RefVariant"].Value = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                }
            }
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
            vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
            vo.ugeCls.utmMain.Tools["ColumnsVisible"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
            
        }

        public override ObjectType GetClsObjectType()
        {
            return ObjectType.FactTable;
        }

        public override object GetNewId()
        {
            if (string.Compare(Workplace.ActiveScheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                return DBNull.Value;
            return ActiveDataObj.GetGeneratorNextValue;
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefVariant"].Value = FinSourcePlanningNavigation.Instance.CurrentVariantID;
            row.Cells["TASKID"].Value = -1;
        }

        protected override IExportImporter GetExportImporter()
        {
            return Workplace.ActiveScheme.GetXmlExportImportManager().GetExportImporter(ObjectType.FactTable);
        }

        public override bool SaveData(object sender, EventArgs e)
        {
            // для оракла просто сохраняем данные
            if (Workplace.ActiveScheme.SchemeDWH.FactoryName == "Krista.FM.Providers.MSOracleDataAccess")
                return base.SaveData(sender, e);
            // для SQL сервера сохраняем и обновляем данные
            if (Workplace.ActiveScheme.SchemeDWH.FactoryName == "System.Data.SqlClient")
            {
                bool refreshData = dsObjData.Tables[0].GetChanges(DataRowState.Added) != null;
                if (base.SaveData(sender, e))
                {
                    return refreshData ? Refresh() : true;
                }
            }
            return false;
        }

        #region Работа с вариантом

        private void SetDataParams(int variantId, int sourceId)
        {
            CurrentDataSourceID = sourceId;
            RefVariant = variantId;
        }

        private void SetVariantCaption(string caption, bool burnButtons)
        {
            ToolBase tool = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"].Tools["lbSelectedVariantName"];
            tool.InstanceProps.Caption = String.Format("Вариант: {0}", caption);
            InfragisticsHelper.BurnTool(tool, burnButtons);
            vo.ugeCls.BurnRefreshDataButton(burnButtons);
        }

        /// <summary>
        /// Изменение варианта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Instance_VariantChangedNew(object sender, VariantChangeEventHandler e)
        {
            if (MessagesVisualizator != null)
                MessagesVisualizator.Hide();
            if (RefVariant != e.VariantID)
            {
                SetDataParams(e.VariantID, e.SourceID);
                SetVariantCaption(e.VariantCaption, true);
            }
        }

        /// <summary>
        /// изменение набора видимых деталей
        /// </summary>
        /// <param name="variantId"></param>
        internal void SetDetailVisible(int variantId)
        {
            utcDetails.BeginUpdate();
            try
            {
                switch (variantId)
                {
                    case 0:
                    case -2:
                        foreach (var tab in utcDetails.Tabs)
                        {
                            tab.Visible = true;
                        }
                        break;
                    default:
                        foreach (var tab in utcDetails.Tabs)
                        {
                            tab.Visible = !HideDetail(tab.Key);
                        }
                        break;
                }
            }
            finally
            {
                utcDetails.EndUpdate();
            }
        }

        internal virtual bool HideDetail(string detailKey)
        {
            return false;
        }

        #endregion Работа с вариантом

        #region Получение курса валюты

        public bool GetExchangeRate(int refOkv, string[] columnValues, ref object[] values)
        {
            // получаем нужный классификатор
            IClassifier cls = Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_RateValue];
            // создаем объект просмотра классификаторов нужного типа
            RateValueDataClsUI clsUI = new RateValueDataClsUI(cls, refOkv);
            clsUI.Workplace = Workplace;
            clsUI.RestoreDataSet = false;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);

            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            modalClsForm.AttachCls(clsUI);
            ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
            // ...загружаем данные
            clsUI.RefreshAttachedData();
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["RefOKV"].FilterConditions.Add(FilterComparisionOperator.Equals, refOkv);
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["DateFixing"].SortIndicator
                = SortIndicator.Descending;

            if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
            {
                int clsID = modalClsForm.AttachedCls.GetSelectedID();
                modalClsForm.AttachedCls.GetColumnsValues(columnValues, ref values);
                // если ничего не выбрали - считаем что функция завершилась неудачно
                if (clsID == -10)
                    return false;
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// обновление данных в детали. Отображение этих данных
        /// </summary>
        /// <param name="detailKey"></param>
        public virtual void RefreshDetail(string detailKey)
        {
            UltraTab tab = GetDetailTab(detailKey);
            tab.Selected = true;
            RefreshDetail();
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("Payment"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["Payment"].SortIndicator = SortIndicator.Ascending;
        }

        public override void SetPermissionsToClassifier(UltraGridEx gridEx)
        {
            if (!allowAddRecord && !allowChangeHierarchy && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;

            // настройка прав на очистку классификатора
            gridEx.AllowClearTable = allowClearClassifier;
            // настройка прав на добавление записей в классификатор
            gridEx.AllowAddNewRecords = allowAddRecord;
            // настройка прав на удаление записей из классификатора
            gridEx.AllowDeleteRows = allowDelRecords;
            // настройка прав на редактирование классификатора
            gridEx.AllowEditRows = allowEditRecords;
            // настройка прав на импортирование классификатора их XML
            gridEx.LoadMenuVisible = allowImportClassifier;
            gridEx.IsReadOnly = viewOnly;
            gridEx.SetComponentSettings();
        }

        protected virtual void SetTransfertContractButton()
        {
            if (!((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Exists("VariantTransfer"))
                return;
            ToolBase tool = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["VariantTransfer"];
            // показываем или скрываем кнопку переноса записи с варианта на вариант
            UltraGridRow activeGridRow = ((BaseClsView) ViewCtrl).ugeCls.ugData.ActiveRow;
            if (activeGridRow == null || activeGridRow.IsAddRow)
            {
                // если нет ни одной выделенной записи, просто делаем кнопку недоступной
                tool.SharedProps.Enabled = false;
                tool.SharedProps.ToolTipText = string.Empty;
                return;
            }

            DataRow activeRow = GetActiveDataRow();
            // скрываем кнопку для записей, которые как либо были изменены.
            if (activeRow == null || activeRow.RowState == DataRowState.Deleted)
            {
                tool.SharedProps.Enabled = false;
                tool.SharedProps.ToolTipText = "Перенос удаленных договоров запрещен";
                return;
            }

            int contractState = Convert.ToInt32(activeRow["RefSStatusPlan"]);
            // запрещаем перенос дочерних договоров
            if (activeRow.Table.Columns.Contains("ParentID"))
            {
                if (!activeRow.IsNull("ParentID"))
                {
                    tool.SharedProps.Enabled = false;
                    tool.SharedProps.ToolTipText = "Перенос дочерних договоров запрещен";
                    return;
                }
                // обработка подчиненных записей
                bool allowTransfert = false;
                DataRow[] chldRows = activeRow.Table.Select(string.Format("ParentID = {0}", activeRow["ID"]));
                DataRow[] chldDelRows = activeRow.Table.Select(string.Format("ParentID = {0}", activeRow["ID"]), string.Empty, DataViewRowState.Deleted);
                DataRow[] allChldRows = new DataRow[chldRows.Length + chldDelRows.Length];
                chldRows.CopyTo(allChldRows, 0);
                chldDelRows.CopyTo(allChldRows, chldRows.Length);
                chldRows = null;
                chldDelRows = null;
                foreach (DataRow chldRow in allChldRows)
                {
                    // добавленные и удаленные записи не проверяем
                    if (chldRow.RowState != DataRowState.Added && chldRow.RowState != DataRowState.Deleted)
                    {
                        contractState = Convert.ToInt32(chldRow["RefSStatusPlan"]);
                        switch (FinSourcePlanningNavigation.Instance.CurrentVariantID)
                        {
                            case 0:
                                allowTransfert = (contractState == 2 || contractState == 3 || contractState == 4);
                                break;
                            case -2:
                                allowTransfert = true;
                                break;
                            default:
                                allowTransfert = (contractState == 0 || contractState == 2 || contractState == 3
                                    || contractState == 4 || contractState == 5);
                                break;
                        }
                    }
                    // если хотя бы для одной из подчиненных записей не выполняется условие переноса, возвращаемся и ничего не делаем
                    if (!allowTransfert)
                    {
                        tool.SharedProps.Enabled = false;
                        tool.SharedProps.ToolTipText = "Один из дочерних договоров не соответствует условию переноса";
                        return;
                    }
                }
            }
            
            tool.SharedProps.ToolTipText = "Перенос договора в действующие договора";
            switch (FinSourcePlanningNavigation.Instance.CurrentVariantID)
            {
                case 0:
                    tool.SharedProps.Enabled = (contractState == 2 || contractState == 3 || contractState == 4);
                    tool.SharedProps.ToolTipText = "Перенос договора в архив";
                    break;
                case -2:
                    tool.SharedProps.Enabled = true;
                    break;
                default:
                    tool.SharedProps.Enabled = (contractState == 0 || contractState == 2 || contractState == 3
                        || contractState == 4 || contractState == 5);
                    break;
            }
        }

        protected override void ugeCls_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (string.Compare(UltraGridEx.GetSourceColumnName(e.Cell.Column.Key), "RefOrganizations", true) == 0 ||
                string.Compare(UltraGridEx.GetSourceColumnName(e.Cell.Column.Key), "RefOrganizationsPlan3", true) == 0 ||
                string.Compare(UltraGridEx.GetSourceColumnName(e.Cell.Column.Key), "RefOrganizations", true) == 0)
            {
                object[] values = new object[1];
                if (GetOrganization(new string[1] { "ID" }, ref values))
                    e.Cell.Row.Cells[UltraGridEx.GetSourceColumnName(e.Cell.Column.Key)].Value = values[0];
            }
            else
                base.ugeCls_OnClickCellButton(sender, e);
        }

        #region вызов справочников вручную

        public bool GetOrganization(string[] columnValues, ref object[] values)
        {
            ModalClsManager clsManager = new ModalClsManager(Workplace);
            int sourceId = -1;
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object qr =
                    db.ExecQuery(
                        "select id from DataSources where SupplierCode = 'ФО' and DataCode = 29 and DataName = 'Анализ данных' and Year = ?",
                        QueryResultTypes.Scalar, new DbParameterDescriptor("p0", DateTime.Today.Year));
                if (qr != null && qr != DBNull.Value)
                    sourceId = Convert.ToInt32(qr);
            }
            object refId = -1;
            if (clsManager.ShowClsModal(SchemeObjectsKeys.d_Organizations_Plan_Key, -1, sourceId, DateTime.Today.Year, ref refId))
            {
                values[0] = refId;
                return true;
            }
            return false;
        }

        /// <summary>
        /// статус при планировании для кредитов и гарантий
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool GetStatusPlan(ref object[] values)
        {
            ModalClsManager clsManager = new ModalClsManager(Workplace);
            int sourceId = -1;
            object refId = -1;
            if (clsManager.ShowClsModal(SchemeObjectsKeys.fx_S_StatusPlan_Key, -1, sourceId, DateTime.Today.Year, ref refId))
            {
                values[0] = refId;
                return true;
            }
            return false;
        }

        /// <summary>
        /// статус при планировании для ценных бумаг
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool GetCapitalStatusPlan(ref object[] values)
        {
            ModalClsManager clsManager = new ModalClsManager(Workplace);
            int sourceId = -1;
            object refId = -1;
            if (clsManager.ShowClsModal(CapitalObjectKeys.fx_S_StatusPlanCap, -1, sourceId, DateTime.Today.Year, ref refId))
            {
                values[0] = refId;
                return true;
            }
            return false;
        }

        #endregion

        #region скрытие id

        internal void HideMasterId()
        {
            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                band.Columns["ID"].Hidden = true;
            }
        }

        internal void HideDetailId()
        {
            foreach(UltraGridBand band in ActiveDetailGrid.ugData.DisplayLayout.Bands)
            {
                band.Columns["ID"].Hidden = true;
            }
        }

        #endregion

        #region работа с несколькими вариантами планов обслуживания

        /// <summary>
        /// добавление нового расчета плана обслуживания
        /// </summary>
        public virtual void AddNewPlanCalculation()
        {
            
        }

        #endregion

        #region представления

        private Presentations Presentations
        {
            get; set;
        }

        public virtual string GetPresentationKey()
        {
            if (Presentations == null)
                Presentations = new Presentations();
            return Presentations.GetPresentationKey(this);
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);

            if (((UltraGrid)sender).DisplayLayout.Bands[0].Columns.Exists("RefSStatusPlan"))
                ((UltraGrid)sender).DisplayLayout.Bands[0].Columns["RefSStatusPlan"].Hidden = true;
            else
                ((UltraGrid)sender).DisplayLayout.Bands[0].Columns["RefStatusPlan"].Hidden = true;

            UltraGridColumn statusColumn = ((UltraGrid)sender).DisplayLayout.Bands[0].Columns.Add("PlanStatus");
            UltraGridHelper.SetLikelyButtonColumnsStyle(statusColumn, 0);
            statusColumn.Header.VisiblePosition = 2;

            e.Layout.Override.WrapHeaderText = DefaultableBoolean.True;

            var statusGroup = e.Layout.Bands[0].Groups.Add("Status", string.Empty);
            statusGroup.Header.Fixed = true;
            e.Layout.Bands[0].Columns[UltraGridEx.StateColumnName].Group = statusGroup;
            e.Layout.Bands[0].Columns["PlanStatus"].Group = statusGroup;

            foreach (string group in vo.ugeCls.Groups.Keys)
            {
                e.Layout.Bands[0].Groups.Add(group, group);
            }

            foreach (GridColumnState state in vo.ugeCls.CurrentStates.Values)
            {
                if (e.Layout.Bands[0].Groups.Exists(state.GroupName))
                {
                    string columnName = vo.ugeCls.GetGridColumnName(state.ColumnName);
                    e.Layout.Bands[0].Columns[columnName].Group = e.Layout.Bands[0].Groups[state.GroupName];
                    e.Layout.Bands[0].Columns[columnName].CellMultiLine = DefaultableBoolean.False;
                }
            }

            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                if (column.DataType == typeof(String))
                {
                    column.Header.Tag = CheckState.Unchecked;
                }
            }

            if (e.Layout.Bands[0].Groups.Count == 1)
            {
                e.Layout.Bands[0].Groups.Clear();
            }

            CollapceGroups();
        }

        private List<string> collapsedGroups = new List<string>();

        protected override void ugeCls_OnCreateUIElement(object sender, UIElement parent)
        {
            base.ugeCls_OnCreateUIElement(sender, parent);

            if (parent is HeaderUIElement)
            {
                HeaderUIElement headerUIElement = (HeaderUIElement)parent;
                HeaderBase aHeader = ((HeaderUIElement)parent).Header;
                if (aHeader.Group != null && aHeader.Column == null)
                {
                    if (aHeader.Group.Columns.Count < 2)
                    {
                        aHeader.Group.Header.Caption = string.Empty;
                        return;
                    }
                        
                    CreateGroupButton(headerUIElement);
                }
            }
        }

        private void ButtonUIElementElementClick(object sender, UIElementEventArgs e)
        {
            ImageAndTextButtonUIElement button = e.Element as ImageAndTextButtonUIElement;
            if (button == null)
                return;
            GroupHeader groupHeader = (GroupHeader)button.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(GroupHeader));
            HeaderUIElement header = (HeaderUIElement)groupHeader.GetUIElement();
            bool collapseGroup = !collapsedGroups.Contains(groupHeader.Group.Key);
            vo.ugeCls.CollapseGroup(groupHeader.Group.Key, collapseGroup);
            if (collapseGroup)
            {
                collapsedGroups.Add(groupHeader.Group.Key);
                CreateGroupButton(header);
            }
            else
            {
                collapsedGroups.Remove(groupHeader.Group.Key);
                CreateGroupButton(header);
            }
        }

        private void CreateGroupButton(HeaderUIElement groupHeader)
        {
            var buttonElement = (ImageAndTextButtonUIElement)groupHeader.GetDescendant(typeof(ImageAndTextButtonUIElement));
            if (buttonElement != null)
            {
                groupHeader.ChildElements.Remove(buttonElement);
                buttonElement.Dispose();
            }

            if (string.IsNullOrEmpty(groupHeader.Header.Group.Header.Caption))
                return;

            string groupKey = groupHeader.Header.Group.Key;
            TextUIElement aTextUIElement = (TextUIElement)groupHeader.GetDescendant(typeof(TextUIElement));
            ImageAndTextButtonUIElement collapceButtonUIElement = new ImageAndTextButtonUIElement(groupHeader);
            groupHeader.ChildElements.Add(collapceButtonUIElement);
            collapceButtonUIElement.Rect = new Rectangle(groupHeader.Rect.X, groupHeader.Rect.Y, groupHeader.Rect.Height, groupHeader.Rect.Height);
            collapceButtonUIElement.Image = collapsedGroups.Contains(groupKey) ?
                ru.toggle_small_expand_icon :
                ru.minus_small_white_icon;
            collapceButtonUIElement.ElementClick += ButtonUIElementElementClick;
            aTextUIElement.Rect = new Rectangle(collapceButtonUIElement.Rect.Right + 3,
                aTextUIElement.Rect.Y, groupHeader.Rect.Width - collapceButtonUIElement.Rect.Width, aTextUIElement.Rect.Height);
        }

        private void CollapceGroups()
        {
            foreach (UltraGridGroup group in vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups)
            {
                vo.ugeCls.CollapseGroup(group.Key, true);
                collapsedGroups.Add(group.Key);
            }
            if (vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups.Count > 0)
            {
                collapsedGroups.Remove(vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups[1].Key);
                collapsedGroups.Remove(vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups[2].Key);
                vo.ugeCls.CollapseGroup(vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups[1].Key, false);
                vo.ugeCls.CollapseGroup(vo.ugeCls.ugData.DisplayLayout.Bands[0].Groups[2].Key, false);
            }
        }

        #endregion
    }
}
