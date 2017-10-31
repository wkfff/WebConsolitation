using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Excel;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.References;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesEvalPlan
{
    public class IncomesEvalPlanUI : BaseViewObj
    {

        public IncomesEvalPlanUI(string key)
            : base(key)
        {
            Caption = "Оценка и прогноз по доходам";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new IncomesEvalPlanView();
            fViewCtrl.ViewContent = this;
        }

        public IncomesEvalPlanView ViewObject
        {
            get; set;
        }

        private IncomesEvalPlanService IncomesEvalPlanService
        {
            get; set;
        }

        public int Year
        {
            get; set;
        }

        public int SourceId
        {
            get; set;
        }

        public object IncomesVariant
        {
            get; set;
        }

        public object PlanIncomes
        {
            get; set;
        }

        public object KvsrPlan
        {
            get; set;
        }

        private DataTable evalPlanData;
        public DataTable EvalPlanData
        {
            get
            {
                return evalPlanData;
            } 
            set
            {
                evalPlanData = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            IncomesEvalPlanService = new IncomesEvalPlanService(Workplace.ActiveScheme);

            ViewObject = (IncomesEvalPlanView) fViewCtrl;
            List<string> budLevels = new List<string>(new string[] { "Консолидированный бюджет субъекта", "Областной бюджет", "Местный бюджет" });
            ViewObject.ucBudgetLevel.DataSource = budLevels;
            ViewObject.ucBudgetLevel.BeforeDropDown += uc_BeforeDropDown;
            ViewObject.ucBudgetLevel.AfterCloseUp += new EventHandler(ucBudgetLevel_AfterCloseUp);
            ViewObject.ucBudgetLevel.Rows[0].Activate();

            List<string> municipals = new List<string>(new string[] { "МР, ГО", "МР, ГО, поселения" });
            ViewObject.ucMunicipal.DataSource = municipals;
            ViewObject.ucMunicipal.BeforeDropDown += uc_BeforeDropDown;
            ViewObject.ucMunicipal.AfterDropDown += ucMunicipal_AfterDropDown;
            ViewObject.ucMunicipal.ValueChanged += ucMunicipal_ValueChanged;
            ViewObject.ucMunicipal.Rows[1].Activate();

            ViewObject.uteVariant.EditorButtonClick += uteVariant_EditorButtonClick;
            ViewObject.uteIncomesSource.Enabled = false;
            ViewObject.uteIncomesSource.EditorButtonClick += uteIncomesSource_EditorButtonClick;
            ViewObject.uteAdministrator.Enabled = false;
            ViewObject.uteAdministrator.EditorButtonClick += uteAdministrator_EditorButtonClick;
            
            ViewObject.uteIncomesSource.ValueChanged += ValueChanged;
            ViewObject.uteAdministrator.ValueChanged += ValueChanged;
            ViewObject.ucBudgetLevel.ValueChanged += ValueChanged;
            ViewObject.cbTaxResource.CheckedChanged += ValueChanged;
            ViewObject.cbForecast.CheckedChanged += ValueChanged;
            ViewObject.cbEstimate.CheckedChanged += ValueChanged;
            ViewObject.cbKmbResults.CheckedChanged += ValueChanged;

            ViewObject.tbManager.ToolClick += tbManager_ToolClick;

            ViewObject.ugData.InitializeLayout += ugData_InitializeLayout;
            ViewObject.ugData.InitializeRow += ugData_InitializeRow;
            ViewObject.ugData.AfterCellUpdate += ugData_AfterCellUpdate;
            ViewObject.ugData.BeforeCellUpdate += ugData_BeforeCellUpdate;
            ViewObject.ugData.PreviewKeyDown += ugData_PreviewKeyDown;
            ViewObject.ugData.KeyUp += ugData_KeyUp;

            ViewObject.btbKvsrClear.Click += btbKvsrClear_Click;
            ViewObject.btbKvsrClear.ButtonStyle = UIElementButtonStyle.Popup;
            ViewObject.btbKvsrClear.MouseEnter += btbKvsrClear_MouseEnter;
            ViewObject.btbKvsrClear.MouseLeave += btbKvsrClear_MouseLeave;
            ViewObject.uteVariant.MouseEnter += uteVariant_MouseEnter;
            ViewObject.uteVariant.MouseLeaveElement += uteVariant_MouseLeave;
            ViewObject.uteIncomesSource.MouseEnter += uteIncomesSource_MouseEnter;
            ViewObject.uteIncomesSource.MouseLeaveElement += uteIncomesSource_MouseLeave;
            ViewObject.uteAdministrator.MouseEnter += uteAdministrator_MouseEnter;
            ViewObject.uteAdministrator.MouseLeaveElement += uteAdministrator_MouseLeaveElement;

            SetDefaultVariant();

            // значения по умолчанию
            KvsrPlan = -1;
        }

        private void SetDefaultVariant()
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dataTable =
                    (DataTable)
                    db.ExecQuery(
                        "select ID, Name,RefYear,VariantDate from d_Variant_PlanIncomes where CurrentVariant = 1 order by id",
                        QueryResultTypes.DataTable);
                if (dataTable.Rows.Count > 0)
                {
                    SetVariantData(dataTable.Rows[0].ItemArray);
                }
            }
        }

        void btbKvsrClear_Click(object sender, EventArgs e)
        {
            KvsrPlan = -1;
            ViewObject.uteAdministrator.Tag = null;
            ViewObject.uteAdministrator.Text = string.Empty;
        }

        void ValueChanged(object sender, EventArgs e)
        {
            IncomesEvalPlanParams evalPlanParams = GetParams();
            BurnRefresh(evalPlanParams.CheckParams());
        }

        #region Настройка грида

        void ugData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.UseFixedHeaders = true;
            // настройка внешнего вида грида
            e.Layout.Bands[0].Groups.Add("Num", "№ П/П");
            e.Layout.Bands[0].Columns["Num"].Width = 40;
            e.Layout.Bands[0].Groups.Add("RegionName", "Наименование муниципальных образований");
            IncomesEvalPlanParams incomesEvalPlanParams = GetParams();
            int firstYear = incomesEvalPlanParams.IsEstimate
                                ? incomesEvalPlanParams.Year - 1
                                : incomesEvalPlanParams.Year;
            int lastYear = incomesEvalPlanParams.IsForecast || incomesEvalPlanParams.IsTaxResource
                               ? incomesEvalPlanParams.Year + 2
                               : incomesEvalPlanParams.Year - 1;

            for (int i = firstYear; i <= lastYear; i++)
            {
                string groupCaption = i < Year ? "Оценка " + i : "Прогноз " + i;

                e.Layout.Bands[0].Groups.Add(i.ToString(), groupCaption);
            }

            e.Layout.Bands[0].Columns["Num"].Group = e.Layout.Bands[0].Groups["Num"];
            e.Layout.Bands[0].Columns["TerritoryTypeName"].Group = e.Layout.Bands[0].Groups["RegionName"];
            e.Layout.Bands[0].Columns["TerritoryTypeName"].Width = 40;
            e.Layout.Bands[0].Columns["RegionName"].Width = 200;
            e.Layout.Bands[0].Columns["RegionName"].Group = e.Layout.Bands[0].Groups["RegionName"];

            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                string groupName = string.Empty;
                if (column.DataType == typeof(decimal) && column.Key.Contains("_"))
                {
                    column.CellMultiLine = DefaultableBoolean.False;
                    groupName = column.Key.Split('_')[0];
                    if (e.Layout.Bands[0].Groups.Exists(groupName))
                        column.Group = e.Layout.Bands[0].Groups[groupName];
                    column.Width = 100;
                    column.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                    column.CellAppearance.TextHAlign = HAlign.Right;
                    column.PadChar = '_';
                    column.MaskInput = "-nnn,nnn,nnn,nnn";
                    if (column.Key.Contains("KBS"))
                    {
                        column.CellActivation = Activation.NoEdit;
                        column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                    }
                    /*
                    if (blockedGroupsList[groupName])
                    {
                        column.CellActivation = Activation.NoEdit;
                        column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                    }*/
                }
                else
                {
                    column.CellActivation = Activation.NoEdit;
                    column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                }
            }

            e.Layout.Bands[0].Columns["Num"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["RegionName"].Header.Fixed = true;
            e.Layout.Bands[0].Columns["TerritoryTypeName"].Header.Fixed = true;

            e.Layout.Bands[0].Groups["Num"].Header.Fixed = true;
            e.Layout.Bands[0].Groups["RegionName"].Header.Fixed = true;

            e.Layout.Bands[0].GroupHeaderLines = 3;
            e.Layout.Bands[0].ColHeaderLines = 3;

            e.Layout.Override.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Override.AllowGroupMoving = AllowGroupMoving.NotAllowed;
            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            e.Layout.GroupByBox.Hidden = true;
            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                if (!column.Hidden)
                    column.SortIndicator = SortIndicator.Disabled;
                column.AllowRowFiltering = DefaultableBoolean.False;
            }
            e.Layout.Bands[0].Columns["ID"].SortIndicator = SortIndicator.Ascending;
        }

        void ugData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            int terType = Convert.ToInt32(row.Cells["TerritoryType"].Value);
            if (terType == 20 || terType == 40 || terType == 50 || terType == 60 || terType == 70)
            {
                row.Cells["RegionName"].Appearance.FontData.Bold = DefaultableBoolean.True;
            }

            if (terType == 30)
            {
                row.CellAppearance.FontData.Italic = DefaultableBoolean.True;
            }

            bool isResult = Convert.ToBoolean(row.Cells["IsResult"].Value);
            if (!isResult)
            {
                foreach (UltraGridColumn column in e.Row.Band.Columns)
                {
                    if (column.DataType == typeof(decimal))
                    {
                        if (column.Key.Contains("KBS"))
                            continue;
                        string columnName = column.Key + "Result";
                        if (Convert.ToBoolean(row.Cells[columnName].Value))
                        {
                            row.Cells[column].Activation = Activation.NoEdit;
                            row.Cells[column].Appearance.BackColor = Color.LightGoldenrodYellow;
                        }
                    }
                }
                return;
            }
            row.Activation = Activation.NoEdit;
            row.CellAppearance.BackColor = Color.LightGoldenrodYellow;
        }

        private object oldValue;

        void ugData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            oldValue = e.Cell.Value;
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            string columnKey = e.Cell.Column.Key;
            UltraGridRow row = e.Cell.Row;
            object parentRegionId = row.Cells["RegionParentId"].Value;

            bool isTaxColumn = columnKey.Contains("BMR") || columnKey.Contains("BPos");

            UltraGridRow parentRow = parentRegionId == null || parentRegionId == DBNull.Value ? null :
                UltraGridHelper.FindRow(ViewObject.ugData, new string[] { "RegionID" },
                new object[] { parentRegionId }, new Type[] { typeof(Int64) });

            decimal currentValue;
            decimal originalValue;
            decimal.TryParse(e.Cell.Value.ToString(), out currentValue);
            ViewObject.ugData.EventManager.SetEnabled(EventGroups.AllEvents, false);
            e.Cell.Value = currentValue;
            ViewObject.ugData.EventManager.SetEnabled(EventGroups.AllEvents, true);
            decimal.TryParse(oldValue.ToString(), out originalValue);
            string kbsColumnKey = columnKey.Split('_')[0] + "_KBS";
            if (currentValue == originalValue)
                return;
            bool isResultCell = columnKey.Contains("KBS");
            if (!isResultCell)
            {
                if (!isTaxColumn && e.Cell.Row.Band.Columns.Exists(kbsColumnKey))
                {
                    decimal kbsValue;
                    Decimal.TryParse(e.Cell.Row.Cells[kbsColumnKey].Value.ToString(), out kbsValue);
                    e.Cell.Row.Cells[kbsColumnKey].Value = kbsValue - originalValue + currentValue;
                }

                // вычисляем значение в родительской записи
                if (parentRow != null)
                {
                    if (parentRow.Cells[columnKey].Value == DBNull.Value)
                    {
                        parentRow.Cells[columnKey].Value = currentValue;
                    }
                    else
                    {
                        parentRow.Cells[columnKey].Value = Convert.ToDecimal(parentRow.Cells[columnKey].Value) -
                                                           originalValue +
                                                           currentValue;
                    }
                    parentRow.Update();
                }
                BurnChangeDataButtons(true);
            }
            row.Update();
        }

        #endregion

        #region обработчик нажатия кнопок на тулбаре

        void tbManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "RefreshData":
                    RefreshData();
                    BurnRefresh(false);
                    break;
                case "CancelChanges":
                    CancelData();
                    BurnChangeDataButtons(false);
                    break;
                case "SaveChanges":
                    SaveData();
                    BurnChangeDataButtons(false);
                    break;
                case "CreateReport":
                    CreateReport();
                    break;
                case "SaveToExcel":
                    SaveExcelData();
                    break;
                case "LoadDataFromExcel":
                    LoadDataFromExcel();
                    break;
                case "CopyVariant":
                    VariantCopy();
                    break;
            }
        }

        #endregion

        #region вызов справочников

        void uteVariant_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] {"ID", "Name", "RefYear", "VariantDate"});
            List<object> values = new List<object>();
            if (ChooseRef(ObjectKeys.d_Variant_PlanIncomes, -1, "Code", columns, ref values))
            {
                SetVariantData(values.ToArray());
            }
        }

        private void SetVariantData(object[] values)
        {
            ViewObject.uteVariant.Tag = values[3];
            ViewObject.uteIncomesSource.Enabled = true;
            ViewObject.uteAdministrator.Enabled = true;
            IncomesVariant = values[0];
            int year = Convert.ToInt32(values[2]);
            ViewObject.uteVariant.Text = values[1].ToString();
            if (Year != year)
            {
                Year = year;
                SourceId = CommonUtils.GetSourceId(Year, Workplace.ActiveScheme.SchemeDWH.DB);
                ViewObject.uteIncomesSource.Text = string.Empty;
                PlanIncomes = null;
                ViewObject.uteAdministrator.Text = string.Empty;
                KvsrPlan = -1;
            }
            BurnRefresh(true);
        }

        void uteIncomesSource_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] { "ID", "CodeStr", "Name" });
            List<object> values = new List<object>();
            if (ChooseRef(ObjectKeys.d_KD_PlanIncomes, SourceId, "CodeStr", columns, ref values))
            {
                ((UltraTextEditor)sender).Text = values[1].ToString();
                ((UltraTextEditor)sender).Tag = values[2];
                PlanIncomes = values[0];
                BurnRefresh(true);
            }
        }

        void uteAdministrator_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] { "ID", "Code", "Name" });
            List<object> values = new List<object>();
            if (ChooseRef(ObjectKeys.d_KVSR_Plan, SourceId, "Code", columns, ref values))
            {
                ((UltraTextEditor)sender).Text = values[1].ToString();
                ((UltraTextEditor)sender).Tag = values[2];
                KvsrPlan = values[0];
                BurnRefresh(true);
            }
        }

        private bool ChooseRef(string clsKey, int soirceId, string codeName, List<string> columns, ref List<object> values)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IEntity cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(clsKey);
            BaseClsUI clsUI = null;
            switch (clsKey)
            {
                case ObjectKeys.d_Variant_PlanIncomes:
                    clsUI = new IncomesVariantCls(cls);
                    break;
                case ObjectKeys.d_KD_PlanIncomes:
                    clsUI = new IncomesSourceCls(cls);
                    ((IncomesSourceCls) clsUI).SourceId = SourceId;
                    break;
                default:
                    clsUI = new DataClsUI(cls);
                    break;
            }
            
            clsUI.Workplace = ConsBudgetForecastNavigation.Instance.Workplace;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            if (soirceId >= 0)
                clsUI.RefreshAttachedData(soirceId);
            else
                clsUI.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUI);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns[codeName].SortIndicator =
                SortIndicator.Ascending;
            if (tmpClsForm.ShowDialog() == DialogResult.OK)
            {
                UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(clsUI.UltraGridExComponent.ugData);
                foreach (string columnName in columns)
                {
                    values.Add(activeRow.Cells[columnName].Value);
                }
                return true;
            }
            return false;
        }

        #endregion

        private void BurnRefresh(bool isBurn)
        {
            if (isBurn && IncomesVariant != null && PlanIncomes != null && KvsrPlan != null)
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["RefreshData"], true);
            }
            else
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["RefreshData"], false);
            }
        }

        private void BurnChangeDataButtons(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["SaveChanges"], burn);
            InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["CancelChanges"], burn);
        }

        void uc_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UltraCombo combo = (UltraCombo)sender;
            combo.DisplayLayout.Bands[0].ColHeadersVisible = false;
        }

        void ucMunicipal_AfterDropDown(object sender, EventArgs e)
        {
            
        }

        void ucMunicipal_ValueChanged(object sender, EventArgs e)
        {
            UltraCombo combo = (UltraCombo)sender;
            ViewObject.cbKmbResults.Visible = (Municipal)combo.ActiveRow.Index == Municipal.MrGoSettlement;
            IncomesEvalPlanParams evalPlanParams = GetParams();
            BurnRefresh(evalPlanParams.CheckParams());
        }

        void ucBudgetLevel_AfterCloseUp(object sender, EventArgs e)
        {
            UltraCombo combo = (UltraCombo) sender;
            ViewObject.cbTaxResource.Visible = (BudgetLevel) combo.ActiveRow.Index != BudgetLevel.RegionBudget;
            if (!ViewObject.cbTaxResource.Visible)
                ViewObject.cbTaxResource.Checked = false;
        }

        private IncomesEvalPlanParams GetParams()
        {
            var incomesEvalPlanParams = new IncomesEvalPlanParams();
            incomesEvalPlanParams.IncomesVariant = IncomesVariant;
            incomesEvalPlanParams.KvsrPlan = KvsrPlan;
            incomesEvalPlanParams.PlanIncomes = PlanIncomes;
            incomesEvalPlanParams.SourceId = SourceId;
            incomesEvalPlanParams.Year = Year;
            incomesEvalPlanParams.IsEstimate = ViewObject.cbEstimate.Checked;
            incomesEvalPlanParams.IsForecast = ViewObject.cbForecast.Checked;
            incomesEvalPlanParams.IsKmbResult = ViewObject.cbKmbResults.Checked;
            incomesEvalPlanParams.IsTaxResource = ViewObject.cbTaxResource.Checked;
            incomesEvalPlanParams.BudgetLevel = (BudgetLevel)ViewObject.ucBudgetLevel.ActiveRow.Index;
            incomesEvalPlanParams.Municipal = (Municipal)ViewObject.ucMunicipal.ActiveRow.Index;
            incomesEvalPlanParams.ShowResults = ViewObject.cbKmbResults.Checked;
            return incomesEvalPlanParams;
        }

        #region работа с данными (обновление, сохранение, отмена изменений)

        private void RefreshData()
        {
            IncomesEvalPlanParams incomesEvalPlanParams = GetParams();
            if (!incomesEvalPlanParams.CheckParams())
            {
                MessageBox.Show("Для получения данных не все параметры заполнены", "Получение данных",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Workplace.OperationObj.Text = "Обработка данных";
           // try
            {
                Workplace.OperationObj.StartOperation();
                EvalPlanData = IncomesEvalPlanService.GetData(incomesEvalPlanParams);
                ViewObject.ugData.DataSource = null;
                ViewObject.ugData.DataSource = EvalPlanData;
                Workplace.OperationObj.StopOperation();
            }
            //catch
            {
                Workplace.OperationObj.StopOperation();
                //throw;
            }
        }

        private void CancelData()
        {
            EvalPlanData.RejectChanges();
            ViewObject.ugData.DataSource = null;
            ViewObject.ugData.DataSource = EvalPlanData;
        }

        private void SaveData()
        {
            if (ViewObject.uteIncomesSource.Tag == null)
            {
                MessageBox.Show("Для сохранения данных необходимо выбрать доходный источник",
                                "Сохранение данных", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                return;
            }
            IncomesEvalPlanService.SaveData(EvalPlanData, GetParams());
            EvalPlanData.AcceptChanges();
        }

        #endregion

        #region перемещение по гриду

        void ugData_KeyUp(object sender, KeyEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            switch (e.KeyValue)
            {
                case 37:// стрелка влево
                case 38:// стрелка вверх
                case 39:// стрелка вправо
                case 40:// стрелка вниз
                    grid.PerformAction(UltraGridAction.EnterEditMode);
                    break;
            }
        }

        void ugData_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            if (grid.ActiveCell == null)
                return;
            switch (e.KeyValue)
            {
                case 37:// стрелка влево
                    EmbeddableEditorBase editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.PrevCell);
                    //grid.ActiveCell.))
                    break;
                case 38:// стрелка вверх
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.PageUpCell);
                    break;
                case 39:// стрелка вправо
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.NextCell);
                    break;
                case 40:// стрелка вниз
                    editor = grid.ActiveCell.EditorResolved;
                    editor.ExitEditMode(false, true);
                    //grid.PerformAction(UltraGridAction.PageDownCell);
                    break;
            }
        }

        #endregion

        #region тултипы

        private string toolTipCaption;

        private Infragistics.Win.ToolTip toolTipValue = null;

        public Infragistics.Win.ToolTip ToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new Infragistics.Win.ToolTip(ViewObject);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        void btbKvsrClear_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.Hide();
        }

        void btbKvsrClear_MouseEnter(object sender, EventArgs e)
        {
            ToolTip.ToolTipText = "Очистить код администратора";
            Point tooltipPos = new Point(ViewObject.btbKvsrClear.DisplayRectangle.Left, ViewObject.btbKvsrClear.DisplayRectangle.Bottom);
            ToolTip.Show(ViewObject.btbKvsrClear.PointToScreen(tooltipPos));
        }

        void uteIncomesSource_MouseLeave(object sender, UIElementEventArgs e)
        {
            Type type = e.Element.GetType();
            if (type == typeof(EditorWithTextUIElement))
                ToolTip.Hide();
        }

        void uteIncomesSource_MouseEnter(object sender, EventArgs e)
        {
            UltraTextEditor editor = (UltraTextEditor)sender;
            if (editor.Tag == null)
                return;
            ToolTip.ToolTipText = editor.Tag.ToString();
            Point tooltipPos = new Point(editor.UIElement.Rect.Left, editor.UIElement.Rect.Bottom);
            ToolTip.Show(editor.PointToScreen(tooltipPos));
        }

        void uteVariant_MouseLeave(object sender, UIElementEventArgs e)
        {
            Type type = e.Element.GetType();
            if (type == typeof(EditorWithTextUIElement))
                ToolTip.Hide();
        }

        void uteVariant_MouseEnter(object sender, EventArgs e)
        {
            UltraTextEditor editor = (UltraTextEditor)sender;
            if (editor.Tag == null)
                return;
            ToolTip.ToolTipText = editor.Tag.ToString();
            Point tooltipPos = new Point(editor.UIElement.Rect.Left, editor.UIElement.Rect.Bottom);
            ToolTip.Show(editor.PointToScreen(tooltipPos));
        }

        void uteAdministrator_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            Type type = e.Element.GetType();
            if (type == typeof(EditorWithTextUIElement))
                ToolTip.Hide();
        }

        void uteAdministrator_MouseEnter(object sender, EventArgs e)
        {
            UltraTextEditor editor = (UltraTextEditor)sender;
            if (editor.Tag == null)
                return;
            ToolTip.ToolTipText = editor.Tag.ToString();
            Point tooltipPos = new Point(editor.UIElement.Rect.Left, editor.UIElement.Rect.Bottom);
            ToolTip.Show(editor.PointToScreen(tooltipPos));
        }

        #endregion

        #region создание отчета

        private void CreateReport()
        {
            string fileName = "Оценка и прогноз по доходам";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                var exporter = new UltraGridExcelExporter();
                exporter.BeginExport += exporter_BeginExport;
                exporter.EndExport += exporter_EndExport;
                exporter.Export(ViewObject.ugData, fileName);
                Process.Start(fileName);
            }
        }

        private void exporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentRowIndex = 5;
            e.Layout.Appearance.FontData.Name = "Arial";
            e.Layout.BorderStyle = UIElementBorderStyle.Solid;
        }

        private void SetExportParamText(Worksheet sheet, int rowNum, int colNum, string caption, string value)
        {
            var row = sheet.Rows[rowNum];
            // название параметра
            var cellHeader = row.Cells[colNum + 0];
            cellHeader.Value = caption;
            cellHeader.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            cellHeader.CellFormat.WrapText = ExcelDefaultableBoolean.True;
            // значение параметра
            var cellValue = row.Cells[colNum + 1];
            cellValue.Value = value;
        }

        private void exporter_EndExport(object sender, EndExportEventArgs e)
        {
            var sheet = e.CurrentWorksheet;
            const int startRow = 1;
            const int startCol = 2;

            SetExportParamText(sheet, startRow, startCol, "Вариант: ",
                String.Format("{0} ({1})", ViewObject.uteVariant.Text, ViewObject.uteVariant.Tag));
            SetExportParamText(sheet, startRow + 1, startCol, "Доходный источник",
                String.Format("{0} ({1})", ViewObject.uteIncomesSource.Text, ViewObject.uteIncomesSource.Tag));
            SetExportParamText(sheet, startRow + 2, startCol, "Администратор", ViewObject.uteAdministrator.Text);

            for (var i = 0; i < e.CurrentRowIndex; i++)
            {
                var row = sheet.Rows[i];

                for (var j = 0; j < sheet.Columns.Count(); j++)
                {
                    var cellValue = row.Cells[j];
                    var cellStyle = row.Cells[2];

                    cellValue.CellFormat.FillPatternForegroundColor = Color.White;
                    cellValue.CellFormat.Font.Height = 160;

                    if (i > 5)
                    {
                        cellValue.CellFormat.Font.SetFontFormatting(cellStyle.CellFormat.Font);

                        cellValue.CellFormat.BottomBorderColor = Color.Black;
                        cellValue.CellFormat.LeftBorderColor = Color.Black;
                        cellValue.CellFormat.RightBorderColor = Color.Black;
                        cellValue.CellFormat.TopBorderColor = Color.Black;

                        cellValue.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
                    }
                }
            }
        }

        private void CreateTemplateForLoad()
        {
            string fileName = "Оценка и прогноз по доходам";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                var exporter = new UltraGridExcelExporter();
                exporter.BeginExport += exporter_BeginTemplateExport;
                exporter.EndExport += exporter_EndTemplateExport;
                exporter.Export(ViewObject.ugData, fileName);
                Process.Start(fileName);
            }
        }

        private void exporter_BeginTemplateExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentRowIndex = 0;
            e.Layout.Appearance.FontData.Name = "Arial";
            e.Layout.BorderStyle = UIElementBorderStyle.Solid;
        }

        private void exporter_EndTemplateExport(object sender, EndExportEventArgs e)
        {
            var sheet = e.CurrentWorksheet;
            var captionRow = sheet.Rows[1];

            int columnIndex = 3;
            var comment = new WorksheetCellComment();
            comment.Text = new FormattedString("Num");
            captionRow.Cells[0].Comment = comment;

            comment = new WorksheetCellComment();
            comment.Text = new FormattedString("TerritoryTypeName");
            captionRow.Cells[1].Comment = comment;

            comment = new WorksheetCellComment();
            comment.Text = new FormattedString("RegionName");
            captionRow.Cells[2].Comment = comment;

            foreach (DataColumn column in EvalPlanData.Columns)
            {
                if (column.DataType == typeof(decimal))
                {
                    comment = new WorksheetCellComment();
                    comment.Text = new FormattedString(column.ColumnName);
                    captionRow.Cells[columnIndex].Comment = comment;
                    columnIndex++;
                }
            }

            for (var i = 0; i < e.CurrentRowIndex; i++)
            {
                var row = sheet.Rows[i];

                for (var j = 0; j < sheet.Columns.Count(); j++)
                {
                    var cellValue = row.Cells[j];
                    var cellStyle = row.Cells[2];

                    cellValue.CellFormat.FillPatternForegroundColor = Color.White;
                    cellValue.CellFormat.Font.Height = 160;

                    if (i > 1)
                    {
                        cellValue.CellFormat.Font.SetFontFormatting(cellStyle.CellFormat.Font);

                        cellValue.CellFormat.BottomBorderColor = Color.Black;
                        cellValue.CellFormat.LeftBorderColor = Color.Black;
                        cellValue.CellFormat.RightBorderColor = Color.Black;
                        cellValue.CellFormat.TopBorderColor = Color.Black;

                        cellValue.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
                    }
                }
            }
        }


        #endregion

        #region загрузка выгрузка данных, отчет

        private void SaveExcelData()
        {
            CreateTemplateForLoad();
        }

        private void LoadDataFromExcel()
        {
            string fileName = "Оценка и прогноз по доходам";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, false, ref fileName))
            {

                GridColumnsStates columnsStates = new GridColumnsStates();
                foreach (DataColumn column in EvalPlanData.Columns)
                {
                    GridColumnState columnState = new GridColumnState(column.ColumnName);
                    columnState.ColumnCaption = column.Caption;
                    if (string.IsNullOrEmpty(columnState.ColumnCaption))
                    {
                        switch (columnState.ColumnName)
                        {
                            case "TerritoryTypeName":
                                columnState.ColumnCaption = "Тип территории";
                                break;
                            case "RegionName":
                                columnState.ColumnCaption = "Наименование района";
                                break;
                            default:
                                columnState.IsHiden = true;
                                break;
                        }
                    }
                    else
                        columnState.ColumnWidth = 100;
                    columnsStates.Add(columnState);
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(EvalPlanData.Clone());
                Client.Common.ExportImport.ExcelExportImportHelper.ImportFromExcel(ds, string.Empty, string.Empty, columnsStates, false, false,
                                                        fileName, Workplace, string.Empty, 2);

                Workplace.OperationObj.Text = "Обработка данных";
                Workplace.OperationObj.StartOperation();
                try
                {
                    for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        string id = EvalPlanData.Rows[i]["ID"].ToString();
                        UltraGridRow gridRow = UltraGridHelper.FindRow(ViewObject.ugData, "ID", id);
                        foreach (DataColumn column in ds.Tables[0].Columns)
                        {
                            if (column.DataType == typeof(decimal))
                            {
                                if (ds.Tables[0].Rows[i].IsNull(column))
                                    continue;
                                bool isResult = Convert.ToBoolean(EvalPlanData.Rows[i]["IsResult"]);
                                if (!isResult && gridRow.Cells[column.ColumnName].Activation == Activation.AllowEdit && gridRow.Activation == Activation.AllowEdit)
                                {
                                    EvalPlanData.Rows[i][column.ColumnName] = ds.Tables[0].Rows[i][column];
                                }
                                else if (isResult)
                                {
                                    EvalPlanData.Rows[i][column.ColumnName] = 0;
                                }
                            }
                        }
                    }
                    IncomesEvalPlanService.SetParentSum(ref evalPlanData);
                    ViewObject.ugData.DataSource = null;
                    ViewObject.ugData.DataSource = EvalPlanData;
                    BurnChangeDataButtons(true);
                    Workplace.OperationObj.StopOperation();
                }
                catch
                {
                    Workplace.OperationObj.StopOperation();
                    throw;
                }
            }
        }

        #endregion

        #region копирование данных с варианта на вариант

        private void VariantCopy()
        {
            object sourceVariant = null;
            object destVariant = null;
            CalculateValueType[] copyValues = null;
            if (frmVariantCopy.CopyVariant(Workplace, ref sourceVariant, ref destVariant, ref copyValues))
            {
                IncomesEvalPlanParams evalPlanParams = GetParams();
                if (!evalPlanParams.CheckParams())
                    return;
                Workplace.OperationObj.Text = "Получение и обработка данных";
                Workplace.OperationObj.StartOperation();
                try
                {
                    IncomesEvalPlanService.CopyVariantData(evalPlanParams, sourceVariant, destVariant, copyValues);
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        #endregion
    }
}
