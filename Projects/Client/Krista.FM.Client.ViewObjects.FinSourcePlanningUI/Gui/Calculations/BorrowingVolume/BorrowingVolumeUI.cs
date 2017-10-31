using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.ServerLibrary;
using Infragistics.Win.UltraWinToolbars;
using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using System.Drawing;
using Krista.FM.Client.Workplace.Services;
using Infragistics.Win.UltraWinTabControl;
using Krista.FM.Client.Components;
using System.Data;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.BorrowingVolume
{
    public class BorrowingVolumeUI : CalculationBaseUI
    {
        public BorrowingVolumeUI(IEntity entity)
            : base(entity)
        {
            Caption = "Определение объема необходимого заимствования";
        }

        public override void Initialize()
        {
            base.Initialize();

            UltraToolbar tb = vo.ugeCls.utmMain.Toolbars[0];
            ImageList il = new ImageList();
            il.TransparentColor = Color.Magenta;
            il.Images.Add(Resources.ru.calculator);
            il.Images.Add(Resources.ru.excelDocument);

            ButtonTool tool = CommandService.AttachToolbarTool(new CalculateBorrowingValumeCommand(), tb);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];
            InfragisticsHelper.BurnTool(tool, true);

            bool calcVisible = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("FinSourcePlaning",
                (int)FinSourcePlaningOperations.Calculate, false);
            if (!calcVisible)
                calcVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("VolumeHoldings",
                (int)FinSourcePlaningCalculateUIModuleOperations.Calculate, false);
            tool.SharedProps.Visible = calcVisible;

            tool = CommandService.AttachToolbarTool(new BorrowingValumeReportCommand(), tb);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[1];

            ComboBoxTool cb = new ComboBoxTool("CalculationResults");
            cb.DropDownStyle = DropDownStyle.DropDownList;
            cb.SharedProps.Width = 200;
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.AddRange(new ToolBase[] { cb });
            tb.Tools.AddTool("CalculationResults");

            FinSourcePlanningNavigation.Instance.VariantChanged += ChangeVariant;
            vo.ugeCls.ugData.AfterCellUpdate += ugData_AfterCellUpdate;

            vo.ugeCls.utmMain.ToolValueChanged += utmActions_ToolValueChanged;

            foreach (UltraTab tab in vo.utcDataCls.Tabs)
            {
                if (tab.Index == 0)
                    continue;
                tab.Visible = false;
            }
            SetCurrentVariant(FinSourcePlanningNavigation.Instance.CurrentVariantID);
            clsClassType = ClassTypes.clsFactData;
            // получаем список параметров с которыми можно будет загрузить данные
            ReloadCalculationList();
        }

        public override bool Refresh()
        {
            if (base.Refresh())
            {
                ReloadCalculationList();
                return true;
            }
            return false;
        }

        private void ReloadCalculationList()
        {
            ((ComboBoxTool) vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Clear();
            // получаем список параметров с которыми можно будет загрузить данные
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dtParams =
                    (DataTable)db.ExecQuery(string.Format("select Distinct CalcComment from {0} where RefBrwVariant = {1}",
                    ActiveDataObj.FullDBName, FinSourcePlanningNavigation.Instance.CurrentVariantID), QueryResultTypes.DataTable);
                // добавляем списки ранее сохраненных данных
                ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Add("Новые данные для расчета", "Новые данные для расчета");
                foreach (DataRow row in dtParams.Rows)
                {
                    ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Add(
                        string.Format("{0}", row[0]));
                }
                ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).SelectedIndex = 0;
            }
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            #region выделение некоторых колонок жирным шрифтом
            e.Layout.Bands[0].Columns["RefYearDayUNV"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["Income"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["Charge"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["Deficit"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["SafetyStock"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["RemainsChange"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["Capital"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["Credit"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["BudgetCredit"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["Borrowing"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["NameGuarantee"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["VolumeHoldings"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["NonBorrow"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            #endregion
        }

        private string currentComment;

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            if (!string.IsNullOrEmpty(currentComment))
            dataQuery = String.Concat(
                GetDataSourcesFilter(),
                String.Format(" and {0} = '{1}'", "CalcComment", currentComment));
            else
                dataQuery = String.Concat(
                GetDataSourcesFilter()," and 1 = 2");

            filterStr = dataQuery;
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }
  
        private void ChangeVariant(object sender, EventArgs e)
        {
            SetCurrentVariant(FinSourcePlanningNavigation.Instance.CurrentVariantID);
            vo.ugeCls.BurnRefreshDataButton(true);
        }

        private void SetCurrentVariant(int refVariant)
        {
            CurrentDataSourceID = GetDataSourceIDByVariantID();
            RefVariant = refVariant;
        }

        private int GetDataSourceIDByVariantID()
        {
            return FinSourcePlanningNavigation.Instance.CurrentSourceID;
        }

        #region обработка тулбара

        void utmActions_ToolValueChanged(object sender, ToolEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "CalculationResults":
                    if (dsObjData.Tables.Count == 0)
                        break;
                    // получим параметры расчетов, которые проводились ранее для их загрузки
                    if (((ComboBoxTool)e.Tool).SelectedIndex == 0)
                    {
                        dsObjData.Tables[0].Clear();
                        dsObjData.Tables[0].AcceptChanges();
                        break;
                    }
                    currentComment = ((ComboBoxTool) e.Tool).Value.ToString();
                    LoadData(vo.ugeCls, new EventArgs());
                    break;
            }
        }

        #endregion
     
        public DataTable GetBorrowingData()
        {
            return dsObjData.Tables[0];
        }

        public override void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            base.ugeCls_OnInitializeRow(sender, e);

            // подсвечиваем ячейку с данными, если она содержит значение, при которых заимствования не нужны

            UltraGridRow row = UltraGridHelper.GetRowCells(e.Row);
            decimal incomes = GetCellValue("Income", row);
            decimal charge = GetCellValue("Charge", row);
            if (incomes >= charge)
            {
                e.Row.Cells["DEFICIT"].ToolTipText = "Профицит";
                e.Row.Cells["DEFICIT"].Appearance.BackColor = Color.Green;
                e.Row.Cells["DEFICIT"].Appearance.BackColor2 = Color.White;
                e.Row.Cells["DEFICIT"].Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
            else
            {
                e.Row.Cells["DEFICIT"].ToolTipText = "Дефицит";
                e.Row.Cells["DEFICIT"].Appearance.BackColor = Color.Red;
                e.Row.Cells["DEFICIT"].Appearance.BackColor2 = Color.White;
                e.Row.Cells["DEFICIT"].Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
            decimal volumeHoldings = GetCellValue("VolumeHoldings", row);
            if (volumeHoldings <= 0)
            {
                e.Row.Cells["VolumeHoldings"].ToolTipText = "Доходы и поступления по источникам финансирования дефицита покрывают текущие расходы и расходы на обслуживание государственного долга. Заимствования не требуются";
                e.Row.Cells["VolumeHoldings"].Appearance.BackColor = Color.Green;
                e.Row.Cells["VolumeHoldings"].Appearance.BackColor2 = Color.White;
                e.Row.Cells["VolumeHoldings"].Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
            else
            {
                e.Row.Cells["VolumeHoldings"].ToolTipText = string.Empty;
                e.Row.Cells["VolumeHoldings"].Appearance.BackColor = Color.Red;
                e.Row.Cells["VolumeHoldings"].Appearance.BackColor2 = Color.White;
                e.Row.Cells["VolumeHoldings"].Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.DataType != typeof(Decimal))
                return;
            if (e.Cell.Value is DBNull)
                e.Cell.Value = 0;

            UltraGridRow gridRow = UltraGridHelper.GetRowCells(e.Cell.Row);

            decimal value1 = 0;
            decimal value2 = 0;
            decimal value3 = 0;
            decimal value4 = 0;
            string columnName = e.Cell.Column.Key.ToUpper();
            switch (columnName)
            {
                case "INCOME":
                case "CHARGE":
                    value1 = GetCellValue("Income", gridRow);
                    value2 = GetCellValue("Charge", gridRow);
                    gridRow.Cells["Deficit"].Value = Math.Abs(value1 - value2);
                    break;
                case "DEFICIT":
                case "SAFETYSTOCK":
                case "REMAINSCHANGE":
                case "CAPITAL":
                case "CREDIT":
                case "BUDGETCREDIT":
                case "BORROWING":
                case "NAMEGUARANTEE":
                    gridRow.Cells["VolumeHoldings"].Value = GetVolumeHoldings(gridRow);
                    break;
                case "ISSUECAPITAL":
                case "DISCHARGECAPITAL":
                    value1 = GetCellValue("DischargeCapital", gridRow);
                    value2 = GetCellValue("IssueCapital", gridRow);
                    gridRow.Cells["Capital"].Value = value2 - value1;
                    break;
                case "RECEIPTCREDIT":
                case "REPAYCREDIT":
                    value1 = GetCellValue("ReceiptCredit", gridRow);
                    value2 = GetCellValue("RepayCredit", gridRow);
                    gridRow.Cells["Credit"].Value = value1 - value2;
                    break;
                case "RECEIPTBUDGCREDIT":
                case "REPAYBUDGCREDIT":
                    value1 = GetCellValue("ReceiptBudgCredit", gridRow);
                    value2 = GetCellValue("RepayBudgCredit", gridRow);
                    gridRow.Cells["BudgetCredit"].Value = value1 - value2;
                    break;
                case "CREDITEXTENSIONBUDGET":
                case "CREDITEXTENSIONPERSON":
                case "CREDITRETURNBUDGET":
                case "CREDITRETURNPERSON":
                    value1 = GetCellValue("CreditExtensionBudget", gridRow);
                    value2 = GetCellValue("CreditExtensionPerson", gridRow);
                    value3 = GetCellValue("CreditReturnBudget", gridRow);
                    value4 = GetCellValue("CreditReturnPerson", gridRow);
                    gridRow.Cells["Borrowing"].Value = -value1 - value2 + value3 + value4;
                    break;
                case "VOLUMEHOLDINGS":
                case "ISSUECAPITALPLAN":
                case "RECEIPTCREDITPLAN":
                case "RECEIPTBUDGCREDITPLAN":
                    value1 = GetCellValue("VolumeHoldings", gridRow);
                    value2 = GetCellValue("IssueCapitalPlan", gridRow);
                    value3 = GetCellValue("ReceiptCreditPlan", gridRow);
                    value4 = GetCellValue("ReceiptBudgCreditPlan", gridRow);
                    gridRow.Cells["NonBorrow"].Value = value1 - (value2 + value3 + value4);
                    break;
            }
        }

        private decimal GetVolumeHoldings(UltraGridRow gridRow)
        {
            decimal value1 = GetCellValue("RemainsChange", gridRow);
            decimal value2 = GetCellValue("SafetyStock", gridRow);
            decimal value3 = GetCellValue("Capital", gridRow);
            decimal value4 = GetCellValue("Credit", gridRow);
            decimal value5 = GetCellValue("BudgetCredit", gridRow);
            decimal value6 = GetCellValue("Borrowing", gridRow);
            decimal value7 = GetCellValue("Income", gridRow) - GetCellValue("Charge", gridRow);
            decimal value8 = GetCellValue("NameGuarantee", gridRow);
            return -(value1 + value2 + value3 + value4 + value5 + value6 + value7 - value8);
        }

        private decimal GetCellValue(string columnName, UltraGridRow gridRow)
        {
            if (gridRow.Cells[columnName].Value is DBNull)
                return 0;
            return Convert.ToDecimal(gridRow.Cells[columnName].Value);
        }
    }
}
