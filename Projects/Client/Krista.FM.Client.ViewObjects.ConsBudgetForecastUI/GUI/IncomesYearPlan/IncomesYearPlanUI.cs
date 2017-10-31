using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Forms;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Handbooks;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.References;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Infragistics.Excel;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesYearPlan
{
    public class IncomesYearPlanUI : BaseViewObj
    {
        internal IncomesYearPlanView ViewObject
        {
            get { return (IncomesYearPlanView) fViewCtrl; }
        }

        internal IncomesYearPlanService YearPlanService
        {
            get; set;
        }

        internal int SourceId
        {
            get; set;
        }

        private DataTable incomesData;
        internal DataTable IncomesData
        {
            get
            {
                return incomesData;
            } 
            set
            {
                incomesData = value;
            }
        }

        internal int Month
        {
            get; set;
        }

        internal Municipal Municipal
        {
            get; set;
        }

        private IncomesYearPlanParams dataParams;
        internal IncomesYearPlanParams DataParams
        {
            get { return dataParams; }
            set { dataParams = value; }
        }

        internal int Year
        {
            get; set;
        }

        internal long IncomesCode
        {
            get; set;
        }

        private long KvsrPlan
        {
            get; set;
        }

        public IncomesYearPlanUI(string key)
            : base(key)
        {
            Caption = "Годовой план по доходам";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new IncomesYearPlanView();
            fViewCtrl.ViewContent = this;
        }

        public override void Initialize()
        {
            base.Initialize();
            YearPlanService = new IncomesYearPlanService(Workplace.ActiveScheme);
            Year = DateTime.Today.Year;
            ViewObject.cbYears.SelectedIndex = ViewObject.cbYears.FindStringExact(DateTime.Today.Year.ToString());
            SourceId = GetSourceId(DateTime.Today.Year);
            ViewObject.cbYears.SelectedIndexChanged += cbYears_SelectedIndexChanged;
            ViewObject.cbMonth.SelectedIndexChanged += cbMonth_SelectedIndexChanged;
            ViewObject.cbMunicipality.SelectedIndexChanged += cbMunicipality_SelectedIndexChanged;
            ViewObject.cbBudetLevels.SelectedIndex = 0;
            ViewObject.cbMunicipality.SelectedIndex = 1;
            ViewObject.cbMonth.SelectedIndex = 0;
            ViewObject.uteIncomesSource.EditorButtonClick += uteIncomesSource_EditorButtonClick;
            ViewObject.ToolBar.ToolClick += ToolBar_ToolClick;
            ViewObject.cbBudetLevels.SelectedIndexChanged += cbBudetLevels_SelectedIndexChanged;
            ViewObject.cbResults.CheckedChanged += cbResults_CheckedChanged;
            ViewObject.uteAdministrator.EditorButtonClick += uteAdministrator_EditorButtonClick;
            ViewObject.btbKvsrClear.Click += btbKvsrClear_Click;

            ViewObject.uteIncomesSource.MouseEnter += uteIncomesSource_MouseEnter;
            ViewObject.uteIncomesSource.MouseLeaveElement += uteIncomesSource_MouseLeaveElement;
            ViewObject.uteAdministrator.MouseEnter += uteIncomesSource_MouseEnter;
            ViewObject.uteAdministrator.MouseLeaveElement += uteIncomesSource_MouseLeaveElement;
            ViewObject.btbKvsrClear.MouseEnter += btbKvsrClear_MouseEnter;
            ViewObject.btbKvsrClear.MouseLeave += btbKvsrClear_MouseLeave;

            ViewObject.gridControl.ugData.InitializeLayout += ugData_InitializeLayout;
            ViewObject.gridControl.ugData.AfterCellUpdate += ugData_AfterCellUpdate;
            ViewObject.gridControl.ugData.BeforeCellUpdate += ugData_BeforeCellUpdate;
            ViewObject.gridControl.ugData.InitializeRow += ugData_InitializeRow;
            ViewObject.gridControl.OnCreateUIElement += ugeCls_OnCreateUIElement;
            ViewObject.gridControl.ugData.MouseEnterElement += ugData_MouseEnterElement;
            ViewObject.gridControl.ugData.MouseLeaveElement += UgData_MouseLeaveElement;
            ViewObject.gridControl.ugData.KeyPress += new KeyPressEventHandler(ugData_KeyPress);
            ViewObject.gridControl.ugData.PreviewKeyDown += new PreviewKeyDownEventHandler(ugData_PreviewKeyDown);
            ViewObject.gridControl.ugData.KeyUp += new KeyEventHandler(ugData_KeyUp);

            ViewObject.ToolBar.Tools["Refresh"].SharedProps.AppearancesSmall.Appearance.Image =
                Properties.Resources.RefreshData;
            ViewObject.ToolBar.Tools["Save"].SharedProps.AppearancesSmall.Appearance.Image =
                Properties.Resources.SaveChanges;
            ViewObject.ToolBar.Tools["Cancel"].SharedProps.AppearancesSmall.Appearance.Image =
                Properties.Resources.CancelChanges;
            ViewObject.ToolBar.Tools["MonthCopy"].SharedProps.AppearancesSmall.Appearance.Image =
                Properties.Resources.page_copy_icon;
            ViewObject.ToolBar.Tools["CreateReport"].SharedProps.AppearancesSmall.Appearance.Image =
                Properties.Resources.Document_Microsoft_Excel_icon;
            ViewObject.ToolBar.Tools["SaveDataToExcel"].SharedProps.AppearancesSmall.Appearance.Image =
                Properties.Resources.Save_icon;
            ViewObject.ToolBar.Tools["LoadDataFromExcel"].SharedProps.AppearancesSmall.Appearance.Image =
                Properties.Resources.Folder_Open_icon;
            KvsrPlan = -1;
        }

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

        void ugData_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        void UgData_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is ImageAndTextButtonUIElement)
            {
                ToolTip.Hide();
            }
        }

        void ugData_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is ImageAndTextButtonUIElement)
            {
                ImageAndTextButtonUIElement button = (ImageAndTextButtonUIElement)e.Element;
                if (button.Parent is HeaderUIElement)
                {
                    HeaderBase header = ((HeaderUIElement)button.Parent).Header;
                    if (header.Group != null)
                    {
                        ToolTip.ToolTipText = blockedGroupsList[header.Group.Key] ? "Разблокировать данные за месяц" : "Блокировать данные за месяц";
                        Point tooltipPos = new Point(header.GetUIElement().Rect.Left, header.GetUIElement().Rect.Bottom);
                        ToolTip.Show(ViewObject.gridControl.ugData.PointToScreen(tooltipPos));
                    }
                }
            }
        }

        void cbResults_CheckedChanged(object sender, EventArgs e)
        {

        }

        void cbBudetLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            BurnRefreshData(ViewObject.uteIncomesSource.Tag != null);
        }

        void cbMunicipality_SelectedIndexChanged(object sender, EventArgs e)
        {
            Municipal = ViewObject.cbMunicipality.SelectedIndex == 0
                            ? Municipal.MrGo
                            : Municipal.MrGoSettlement;
            ViewObject.cbResults.Visible = Municipal == Municipal.MrGoSettlement;
            BurnRefreshData(ViewObject.uteIncomesSource.Tag != null);
        }

        void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            Month = ViewObject.cbMonth.SelectedIndex;
            BurnRefreshData(ViewObject.uteIncomesSource.Tag != null);
        }

        void cbYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Year = Convert.ToInt32(ViewObject.cbYears.SelectedItem);
            int newSourceId = GetSourceId(Year);
            if (SourceId != newSourceId)
            {
                ViewObject.uteIncomesSource.Tag = null;
                ViewObject.uteIncomesSource.Text = string.Empty;
                SourceId = GetSourceId(Convert.ToInt32(ViewObject.cbYears.SelectedItem));
                BurnRefreshData(ViewObject.uteIncomesSource.Tag != null);
            }
        }

        void ToolBar_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "Refresh":
                    RefreshData();
                    BurnRefreshData(false);
                    break;
                case "Save":
                    SaveData();
                    BurnChangeDataButtons(false);
                    break;
                case "Cancel":
                    CancelData();
                    BurnChangeDataButtons(false);
                    break;
                case "MonthCopy":
                    CopyMonthData();
                    break;
                case "CreateReport":
                    // создаем отчет
                    CreateReport();
                    break;
                case "BlockMonth":
                    // блокируем данные за месяц
                    break;
                case "SaveDataToExcel":
                    SaveExcelData();
                    break;
                case "LoadDataFromExcel":
                    LoadDataFromExcel();
                    break;
                case "TransfertPrognozData":
                    TransfertData();
                    if (IncomesCode > 0)
                    {
                        RefreshData();
                    }
                    else
                    {
                        BurnRefreshData(true);
                    }
                    break;
            }
        }

        private void TransfertData()
        {
            long transfertvariant = 0;
            if (TransfertDataForm.ShowTransfertData(Year, ref transfertvariant))
            {
                Workplace.OperationObj.Text = "Обработка данных";
                Workplace.OperationObj.StartOperation();
                try
                {
                    YearPlanService.TransfertData(transfertvariant, Year);
                    Workplace.OperationObj.StopOperation();
                }
                catch (Exception)
                {
                    Workplace.OperationObj.StopOperation();
                    throw;
                }
            }
        }

        void uteIncomesSource_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            // вызов справочника
            List<string> columns = new List<string>(new string[] { "ID", "CodeStr", "Name" });
            List<object> values = new List<object>();
            if (ChooseRef(ObjectKeys.d_KD_PlanIncomes, SourceId, "CodeStr", columns, ref values))
            {
                ((UltraTextEditor)sender).Text = values[1].ToString();
                ((UltraTextEditor)sender).Tag = values[2];
                IncomesCode = Convert.ToInt64(values[0]);
                BurnRefreshData(true);
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
                KvsrPlan = Convert.ToInt64(values[0]);
                BurnRefreshData(true);
            }
        }

        void btbKvsrClear_Click(object sender, EventArgs e)
        {
            KvsrPlan = -1;
            ViewObject.uteAdministrator.Tag = null;
            ViewObject.uteAdministrator.Text = string.Empty;
        }

        private int GetSourceId(int year)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt =
                    (DataTable)
                    db.ExecQuery(
                        "select ID from DataSources where SupplierCode = 'ФО' and DataCode = 29 and Year = ? and deleted = 0",
                        QueryResultTypes.DataTable,
                        new DbParameterDescriptor("p0", year));

                if (dt.Rows.Count > 0)
                    return Convert.ToInt32(dt.Rows[0][0]);
                return -1;
            }
        }

        private int GetClsData(string clsKey, ref string caption)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IClassifier cls = Workplace.ActiveScheme.Classifiers[clsKey];
            BaseClsUI clsUI = new KdPlanIncomesCls(cls, cls.ObjectKey);
            clsUI.Workplace = Workplace;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            clsUI.RefreshAttachedData(SourceId);
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
            if (tmpClsForm.ShowDialog(Workplace.WindowHandle) == DialogResult.OK)
            {
                toolTipCaption = clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["Name"].Value.ToString();
                caption = 
                    clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["CodeStr"].Value.ToString();
                BurnRefreshData(true);
                return Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
            }
            return -1;
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
                    ((IncomesSourceCls)clsUI).SourceId = SourceId;
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
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortSingle;
            if (clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns.Exists(codeName + UltraGridEx.REMASKED_COLUMN_POSTFIX))
                codeName = codeName + UltraGridEx.REMASKED_COLUMN_POSTFIX;
            foreach (var ultraBand in clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands)
            {
                ultraBand.Columns[codeName].SortIndicator = SortIndicator.Ascending;
            }

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

        #region работа с данными

        private void RefreshData()
        {
            if (ViewObject.uteIncomesSource.Tag == null)
            {
                MessageBox.Show("Для получения данных необходимо выбрать доходный источник",
                                "Обновление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                return;
            }
            Workplace.OperationObj.Text = "Получение и обработка данных";
            try
            {
                Workplace.OperationObj.StartOperation();
                IncomesYearPlanParams dataParams = GetDataParams();
                IncomesData = YearPlanService.GetIncomesData(dataParams);
                blockedGroupsList.Clear();
                foreach(DataColumn column in IncomesData.Columns.Cast<DataColumn>().
                    Where(w => w.DataType == typeof(bool) && w.ColumnName.Contains("IsBlocked")))
                {
                    blockedGroupsList.Add(column.ColumnName.Split('_')[0],
                                          IncomesData.Rows.Count > 0 && IncomesData.Rows[0].Field<bool>(column));
                }
                ViewObject.gridControl.ugData.DataSource = null;
                ViewObject.gridControl.ugData.DataSource = IncomesData;
                Workplace.OperationObj.StopOperation();
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        private void CancelData()
        {
            IncomesData.RejectChanges();
            ViewObject.gridControl.ugData.DataSource = null;
            ViewObject.gridControl.ugData.DataSource = IncomesData;
        }

        private void SaveData()
        {
            if (ViewObject.uteIncomesSource.Tag == null)
            {
                MessageBox.Show("Для сохранения данных необходимо выбрать доходный источник",
                                "Сохранение данных", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                return;
            }
            YearPlanService.SaveData(IncomesData, GetDataParams());
            IncomesData.AcceptChanges();
        }

        #endregion

        #region настройка грида

        void ugData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.UseFixedHeaders = true;
            // настройка внешнего вида грида
            e.Layout.Bands[0].Groups.Add("Num", "№ П/П");
            e.Layout.Bands[0].Columns["Num"].Width = 40;
            e.Layout.Bands[0].Groups.Add("RegionName", "Наименование муниципальных образований");
            IncomesYearPlanParams incomesYearPlanParams = GetDataParams();
            int firstMonth = incomesYearPlanParams.Month == 0 ? 1 : incomesYearPlanParams.Month;
            int lastMonth = incomesYearPlanParams.Month == 0 ? 12 : incomesYearPlanParams.Month;

            for (int i = firstMonth; i <= lastMonth; i++)
            {
                e.Layout.Bands[0].Groups.Add(i.ToString(), GetMonthGroupName(i)); 
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
                    if (blockedGroupsList[groupName])
                    {
                        column.CellActivation = Activation.NoEdit;
                        column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                    }
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

        private string GetMonthGroupName(int month)
        {
            int year = Convert.ToInt32(ViewObject.cbYears.SelectedItem);
            switch (month)
            {
                case 12:
                    return string.Format("На 01.01.{0}", year + 1);
                default:
                    return string.Format("На 01.{0}.{1}", (month + 1).ToString().PadLeft(2, '0'), year);
            }
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

            UltraGridRow parentRow = parentRegionId == null || parentRegionId == DBNull.Value ? null :
                UltraGridHelper.FindRow(ViewObject.gridControl.ugData, new string[] { "RegionID" },
                new object[] {parentRegionId}, new Type[] {typeof (Int64)});

            decimal currentValue;
            decimal originalValue;
            decimal.TryParse(e.Cell.Value.ToString(), out currentValue);
            ViewObject.gridControl.ugData.EventManager.SetEnabled( EventGroups.AllEvents, false);
            e.Cell.Value = currentValue;
            ViewObject.gridControl.ugData.EventManager.SetEnabled(EventGroups.AllEvents, true);
            decimal.TryParse(oldValue.ToString(), out originalValue);
            string kbsColumnKey = columnKey.Split('_')[0] + "_KBS";
            if (currentValue == originalValue)
                return;
            bool isResultCell = columnKey.Contains("KBS");
            if (!isResultCell)
            {
                if (e.Cell.Row.Band.Columns.Exists(kbsColumnKey))
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

        private UltraGridRow MonthRepRow
        {
            get; set;
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

            var resultRows = IncomesData.Select("TerritoryType = 70");
            if (resultRows.Length == 0)
                return;
            DataRow resultRow = resultRows[0];
            MonthRepRow = UltraGridHelper.FindGridRow(ViewObject.gridControl.ugData, "TerritoryType", 80);
            foreach (UltraGridColumn column in e.Row.Band.Columns)
            {
                if (column.DataType == typeof(decimal))
                {
                    decimal val1 = Math.Round(Convert.ToDecimal(resultRow[column.Key]), 0, MidpointRounding.ToEven);
                    decimal val2 = Math.Round(Convert.ToDecimal(MonthRepRow.Cells[column].Value), 0, MidpointRounding.ToEven);
                    BurnCell(MonthRepRow.Cells[column],
                                val1 != val2);
                }
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

        private void BurnChangeDataButtons(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.ToolBar.Tools["Save"], burn);
            InfragisticsHelper.BurnTool(ViewObject.ToolBar.Tools["Cancel"], burn);
        }

        private void BurnRefreshData(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.ToolBar.Tools["Refresh"], burn);
        }

        #endregion

        private bool CheckParams()
        {
            return !(ViewObject.cbBudetLevels.SelectedIndex == -1 ||
                    ViewObject.cbMunicipality.SelectedIndex == -1 ||
                    ViewObject.uteIncomesSource.Value == null ||
                    ViewObject.cbYears.SelectedIndex == -1 ||
                    ViewObject.cbMonth.SelectedIndex == -1);
        }

        private IncomesYearPlanParams GetDataParams()
        {
            var planParams = new IncomesYearPlanParams();
            planParams.BudgetLevel = (BudgetLevel)ViewObject.cbBudetLevels.SelectedIndex;
            planParams.Municipal = (Municipal) ViewObject.cbMunicipality.SelectedIndex;
            planParams.SourceId = SourceId;
            planParams.IncomesSource = IncomesCode;
            planParams.IncomesCode = ViewObject.uteIncomesSource.Value.ToString();
            planParams.Year = Convert.ToInt32(ViewObject.cbYears.SelectedItem);
            planParams.ShowResults = ViewObject.cbResults.Visible && ViewObject.cbResults.Checked;
            planParams.Month = ViewObject.cbMonth.SelectedIndex;
            planParams.Kvsr = KvsrPlan;
            DataParams = planParams;
            return planParams;
        }

        #region копирование данных с месяца на месяц

        private void CopyMonthData()
        {
            if (!CheckParams())
            {
                MessageBox.Show(
                    "Не все параметры, необходимые для копирования данных, были введены",
                    "Копирование данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (YearPlanService == null)
                return;
            if (MessageBox.Show("Внимание! Операция копирования может привести к удалению данных за месяц, на который копируются данные. Хотите продолжить операцию", 
                "Копирование данных",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            int sourceMonth = 0;
            int destMonth = 0;
            if (frmMonthCopy.ShowMonthCopyForm(ref sourceMonth, ref destMonth))
            {
                CopyData(sourceMonth, destMonth);
            }
        }

        private void CopyData(int sourceMonth, int destMonth)
        {
            Workplace.OperationObj.Text = "Копирование данных";
            try
            {
                Workplace.OperationObj.StartOperation();
                YearPlanService.CopyMonthData(GetDataParams(), sourceMonth, destMonth);
                Workplace.OperationObj.StopOperation();
                MessageBox.Show(
                    "Операция копирования данных с месяца на месяц прошла успешно",
                    "Копирование данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshData();
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        #endregion

        #region тултипы для компонентов

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

        void uteIncomesSource_MouseLeaveElement(object sender, UIElementEventArgs e)
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

        #endregion

        #region блокировка данных по месяцу

        private Dictionary<string, bool> blockedGroupsList = new Dictionary<string, bool>();

        protected void ugeCls_OnCreateUIElement(UIElement parent)
        {
            if (parent is HeaderUIElement)
            {
                HeaderUIElement headerUIElement = (HeaderUIElement)parent;
                HeaderBase aHeader = ((HeaderUIElement)parent).Header;
                if (aHeader.Group != null && aHeader.Column == null)
                {
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

            blockedGroupsList[groupHeader.Group.Key] = !blockedGroupsList[groupHeader.Group.Key];
            Workplace.OperationObj.Text = "Обработка данных";
            try
            {
                bool isBlock = blockedGroupsList[groupHeader.Group.Key];
                Workplace.OperationObj.StartOperation();
                YearPlanService.BlockMonth(GetDataParams(), Convert.ToInt32(groupHeader.Group.Key), isBlock);
                CreateGroupButton(header);
                BlockGridGroup(groupHeader.Group);
                Workplace.OperationObj.StopOperation();
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
            
        }

        private void CreateGroupButton(HeaderUIElement groupHeader)
        {
            ImageAndTextButtonUIElement buttonElement = (ImageAndTextButtonUIElement)groupHeader.GetDescendant(typeof(ImageAndTextButtonUIElement));
            if (buttonElement != null)
            {
                groupHeader.ChildElements.Remove(buttonElement);
                buttonElement.Dispose();
            }

            if (string.IsNullOrEmpty(groupHeader.Header.Group.Header.Caption))
                return;

            string groupKey = groupHeader.Header.Group.Key;
            int groupMonth;
            if (!Int32.TryParse(groupKey, out groupMonth))
                return;
            TextUIElement aTextUIElement = (TextUIElement)groupHeader.GetDescendant(typeof(TextUIElement));
            ImageAndTextButtonUIElement collapceButtonUIElement = new ImageAndTextButtonUIElement(groupHeader);
            groupHeader.ChildElements.Add(collapceButtonUIElement);
            collapceButtonUIElement.Rect = new Rectangle(groupHeader.Rect.X, groupHeader.Rect.Y, 20/*groupHeader.Rect.Height*/, groupHeader.Rect.Height);
            collapceButtonUIElement.Image = blockedGroupsList[groupKey] ?
                Properties.Resources.Unlock_icon :
                Properties.Resources.lock_icon;
            collapceButtonUIElement.ElementClick += ButtonUIElementElementClick;
            aTextUIElement.Rect = new Rectangle(collapceButtonUIElement.Rect.Right + 3,
                aTextUIElement.Rect.Y, groupHeader.Rect.Width - collapceButtonUIElement.Rect.Width, aTextUIElement.Rect.Height);
        }

        private void BlockGridGroup(UltraGridGroup group)
        {
            foreach (UltraGridColumn column in group.Columns)
            {
                if (column.Key.Contains("KBS"))
                {
                    column.CellActivation = Activation.NoEdit;
                    column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                }
                else
                {
                    if (blockedGroupsList[group.Key])
                    {
                        column.CellActivation = Activation.NoEdit;
                        column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                    }
                    else
                    {
                        column.ResetCellActivation();
                        column.CellAppearance.ResetBackColor();
                    }
                }
            }
        }

        #endregion

        #region создание отчета

        private void CreateReport()
        {
            string fileName = "Годовой план по доходам";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                var exporter = new UltraGridExcelExporter();
                exporter.BeginExport += exporter_BeginExport;
                exporter.EndExport += exporter_EndExport;
                exporter.Export(ViewObject.gridControl.ugData, fileName);
                Process.Start(fileName);
            }
        }

        private void exporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentRowIndex = 4;
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

            SetExportParamText(sheet, startRow + 0, startCol, "Доходный источник",
                String.Format("{0} {1}", ViewObject.uteIncomesSource.Text, toolTipCaption));
            SetExportParamText(sheet, startRow + 1, startCol, "Уровни бюджета", ViewObject.cbBudetLevels.Text);

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
            string fileName = "Годовой план по доходам";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                var exporter = new UltraGridExcelExporter();
                exporter.BeginExport += exporter_BeginTemplateExport;
                exporter.EndExport += exporter_EndTemplateExport;
                exporter.Export(ViewObject.gridControl.ugData, fileName);
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

            foreach (DataColumn column in IncomesData.Columns)
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

        #region сохранение и загрузка данных

        private void SaveExcelData()
        {
            CreateTemplateForLoad();
        }

        private void LoadDataFromExcel()
        {
            string fileName = "Годовой план по доходам";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, false, ref fileName))
            {

                GridColumnsStates columnsStates = new GridColumnsStates();
                foreach (DataColumn column in IncomesData.Columns)
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
                ds.Tables.Add(IncomesData.Clone());
                Client.Common.ExportImport.ExcelExportImportHelper.ImportFromExcel(ds, string.Empty, string.Empty, columnsStates, false, false,
                                                        fileName, Workplace, string.Empty, 2);

                Workplace.OperationObj.Text = "Обработка данных";
                Workplace.OperationObj.StartOperation();
                try
                {
                    for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        string id = IncomesData.Rows[i]["ID"].ToString();
                        UltraGridRow gridRow = UltraGridHelper.FindRow(ViewObject.gridControl.ugData, "ID", id);
                        foreach (DataColumn column in ds.Tables[0].Columns)
                        {
                            if (column.DataType == typeof (decimal))
                            {
                                if  (ds.Tables[0].Rows[i].IsNull(column))
                                    continue;
                                bool isResult = Convert.ToBoolean(IncomesData.Rows[i]["IsResult"]);
                                if (!isResult && gridRow.Cells[column.ColumnName].Activation == Activation.AllowEdit && gridRow.Activation == Activation.AllowEdit)
                                {
                                    IncomesData.Rows[i][column.ColumnName] = ds.Tables[0].Rows[i][column];
                                }
                                else if (isResult)
                                {
                                    IncomesData.Rows[i][column.ColumnName] = 0;
                                }
                            }
                        }
                    }
                    YearPlanService.SetParentSum(ref incomesData);
                    ViewObject.gridControl.ugData.DataSource = null;
                    ViewObject.gridControl.ugData.DataSource = IncomesData;
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

        public static void BurnCell(UltraGridCell cell, bool burn)
        {
            if (!burn)
            {
                cell.Appearance.BackColor = Color.Empty;
                cell.Appearance.BackColor2 = Color.Empty;
                cell.Appearance.BackGradientStyle = GradientStyle.None;
            }
            else
            {
                cell.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
                cell.Appearance.BackColor2 = Color.FromKnownColor(KnownColor.Red);
                cell.Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
        }
    }
}
