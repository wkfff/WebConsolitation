using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CreditIssued
{
    public class CreditIssuedUI : BaseCreditUI
    {

        public CreditIssuedUI(IFinSourceBaseService service, string key)
			: base(service, key)
		{
		}

        public override void Initialize()
        {
            base.Initialize();

            #region добавление кнопок на тулбар

            UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"];

            ButtonTool tool = CommandService.AttachToolbarTool(new ValidateCommand(GetConfiguration("Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits.CreditIssued.CreditIssuedValidations.xml")), toolbar);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[1];

            PopupMenuTool pmOperationsList = (PopupMenuTool)toolbar.Tools["OperationsList"];

            CommandService.AttachToolbarTool(new CalcAttractionPlanCommandCO("План предоставления"), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new CalcDebtPlanCommandCO(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new CalcServicePlanCommandCO(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new MassCalculateServicePlanCommand(), toolbar, "OperationsList");

            // добавление выпадающего списка для расчетов пени
            PopupMenuTool pmPenaltyWizards = new PopupMenuTool("PenaltyWizards");
            pmPenaltyWizards.SharedProps.Caption = "Мастер начисление пени";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmPenaltyWizards);
            pmOperationsList.Tools.AddTool("PenaltyWizards");

            PopupMenuTool pmPenaltyAutocalculate = new PopupMenuTool("PenaltyAutocalculate");
            pmPenaltyAutocalculate.SharedProps.Caption = "Автоматизированное начисление пени";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmPenaltyAutocalculate);
            pmOperationsList.Tools.AddTool("PenaltyAutocalculate");

            CommandService.AttachToolbarTool(new CalculatePenaltiesDebtCommandCO(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new CalculatePenaltiesDebtForAllCommandCO(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new CalculatePenaltiesPercentCreditIssuedCommand(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new CalculatePenaltiesPercentForAllCommandCO(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new DebtPenaltyWizardCommandCO(), toolbar, "PenaltyWizards");
            CommandService.AttachToolbarTool(new COPercentPenaltyWizardCommand(), toolbar, "PenaltyWizards");

            PopupMenuTool pmCalculate = new PopupMenuTool("Calculate");
            pmCalculate.SharedProps.Caption = "Расчет";

            pmCalculate.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            pmCalculate.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmCalculate);
            toolbar.Tools.AddTool("Calculate");

            tool = CommandService.AttachToolbarTool(new CalcPercentCommandCO(), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            tool = CommandService.AttachToolbarTool(new CreditCOVariantTransfer(), toolbar);
            tool.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.transfert;

            #endregion

            ((BaseClsView)ViewCtrl).ugeCls.ugData.AfterCellUpdate += new CellEventHandler(ugData_AfterCellUpdate);
            ((BaseClsView) ViewCtrl).ugeCls.ugData.InitializeLayout += new InitializeLayoutEventHandler(ugData_InitializeLayout);
            ((BaseClsView)ViewCtrl).ugeCls.OnInitializeRow += new InitializeRow(ugeCls_OnInitializeRow);
            ((BaseClsView)ViewCtrl).ugeCls.OnBeforeCellDeactivate += new BeforeCellDeactivate(ugeCls_OnBeforeCellDeactivate);
            DetailGridInitializeLayout += new GridInitializeLayout(CreditIssuedUI_DetailGridInitializeLayout);
        }

        internal override void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            if (row.IsDataRow)
            {
                #region Подсветка строк грида

                Color rowColor = Color.White;
                // Рассчитанные объекты не требующие расчета помечаются желтым
                if (!(row.Cells["RefSStatusPlan"].Value is DBNull))
                {
                    switch (Convert.ToInt32(row.Cells["RefSStatusPlan"].Value))
                    {
                        case 0: // принят
                            rowColor = Color.LightGreen;
                            break;
                        case 1: // Не принят
                            rowColor = Color.Orange;
                            break;
                        case 2: // Рефинансирован
                            rowColor = Color.LightGray;
                            break;
                        case 3: // Досрочно погашен
                            rowColor = Color.LightGray;
                            break;
                        case 4: // Закрыт
                            rowColor = Color.LightGray;
                            break;
                        case 5: // на рефинансирование
                            rowColor = Color.Pink;
                            break;
                    }
                }

                row.Appearance.BackColor = rowColor;

                #endregion Подсветка строк грида

                #region наглядное указание статуса договора в виде иконки

                Image image = il.Images[4];
                string toolTip = "Неуказанный статус договора";

                #endregion

                e.Row.Cells["CreditPercent"].Activation = row.Cells["PercentRate"].Value is DBNull
                        ? e.Row.Activation
                        : Activation.ActivateOnly;
            }

            SetContractStatus(row);
        }

        #region обработка данных в деталях

        protected override void ugDetailData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Row.Activation != Activation.AllowEdit)
                return;

            UltraGridCell cell = e.Cell;
            if (activeDetailObject.ObjectKey == CreditIssuedObjectsKeys.t_S_PlanServiceCO && 
                (string.Compare(cell.Column.Key, "STARTDATE", true) == 0 ||
                string.Compare(cell.Column.Key, "ENDDATE", true) == 0))
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                DateTime.TryParse(cell.Row.Cells["STARTDATE"].Value.ToString(), out startDate);
                DateTime.TryParse(cell.Row.Cells["ENDDATE"].Value.ToString(), out endDate);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    cell.Row.Cells["DayCount"].Value = (endDate - startDate).Days + 1;
                }
            }

            #region расчет просроченных дней

            if (string.Compare(e.Cell.Column.Key, "StartPenaltyDate", true) == 0 ||
                string.Compare(e.Cell.Column.Key, "EndPenaltyDate", true) == 0)
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                DateTime.TryParse(e.Cell.Row.Cells["StartPenaltyDate"].Value.ToString(), out startDate);
                DateTime.TryParse(e.Cell.Row.Cells["EndPenaltyDate"].Value.ToString(), out endDate);
                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    e.Cell.Row.Cells["LateDate"].Value = (endDate - startDate).Days;
                }
            }

            #endregion
        }

        #endregion

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            #region изменения дат
            UltraGridCell cell = e.Cell;
            if (string.Compare(cell.Column.Key, "STARTDATE", true) == 0 ||
                string.Compare(cell.Column.Key, "ENDDATE", true) == 0 ||
                string.Compare(cell.Column.Key, "RenewalDate", true) == 0)
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                DateTime.TryParse(cell.Row.Cells["STARTDATE"].Value.ToString(), out startDate);
                if (!DateTime.TryParse(cell.Row.Cells["RenewalDate"].Value.ToString(), out endDate))
                    DateTime.TryParse(cell.Row.Cells["ENDDATE"].Value.ToString(), out endDate);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    ChangePeriod(startDate, endDate, cell.Row);
                }
            }
            #endregion

            if (string.Compare(e.Cell.Column.Key, "RefSStatusPlan", true) == 0)
            {
                SetContractStatus(e.Cell.Row);
            }
        }

        void CreditIssuedUI_DetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid uge = (UltraGrid)sender;
            string key = ((UltraTabPageControl)uge.Parent.Parent.Parent.Parent).Tab.Key;

            // Добавляем Grand Summaries
            if (key == CreditIssuedObjectsKeys.t_S_PlanDebtCO ||
                key == CreditIssuedObjectsKeys.t_S_PlanServiceCO ||
                key == CreditIssuedObjectsKeys.t_S_FactDebtCO ||
                key == CreditIssuedObjectsKeys.t_S_FactPercentCO ||
                key == CreditIssuedObjectsKeys.t_S_ChargePenaltyDebtCO ||
                key == CreditIssuedObjectsKeys.t_S_ChargePenaltyPercentCO ||
                key == CreditIssuedObjectsKeys.t_S_FactPenaltyDebtCO ||
                key == CreditIssuedObjectsKeys.t_S_FactPenaltyPercentCO ||
                key == CreditIssuedObjectsKeys.t_S_PlanAttractCO ||
                key == CreditIssuedObjectsKeys.t_S_FactAttractCO ||
                key == CreditIssuedObjectsKeys.t_S_CollateralCO)
            {
                uge.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;

                SummarySettings s = uge.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["Sum"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
                if (uge.DisplayLayout.Bands[0].Columns.Exists("COST"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["COST"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
                if (uge.DisplayLayout.Bands[0].Columns.Exists("DayCount"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["DayCount"]);
                    s.DisplayFormat = "{0:##,##0}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
            }
        }


        void ugeCls_OnBeforeCellDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
        {
            #region вычисление процентов по долям ставки ЦБ

            UltraGridCell activeCell = ((BaseClsView)ViewCtrl).ugeCls.ugData.ActiveCell;
            if (string.Compare(activeCell.Column.Key, "PercentRate", true) == 0)
            {
                DateTime StartDate = string.IsNullOrEmpty(activeCell.Row.Cells["StartDate"].Text)
                    ? DateTime.MinValue
                    : Convert.ToDateTime(activeCell.Row.Cells["StartDate"].Value);

                decimal percentRate = activeCell.Row.Cells["PercentRate"].Value is DBNull ? -1 
                    : Convert.ToDecimal(activeCell.Row.Cells["PercentRate"].Value);

                if (percentRate >= 0 && StartDate != DateTime.MinValue)
                {
                    // получаем ставку на начало периода
                    decimal percentCB = GetCBPercent(StartDate);
                    activeCell.Row.Cells["CreditPercent"].Value = percentCB * percentRate;
                }
            }

            #endregion
        }

        /// <summary>
        /// Рассчитывает длительность финансовой операции, определяет срочность кредита и заполняет соответствующие поля.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="row"></param>
        private static void ChangePeriod(DateTime startDate, DateTime endDate, UltraGridRow row)
        {
            int periodicity = 0;

            int dayCount = (endDate - startDate).Days + 1;
            double yearLength = Math.Round(dayCount / (double)365, 2, MidpointRounding.AwayFromZero);

            if (yearLength <= 1)
            {
                // Краткосрочный
                periodicity = 1;
            }
            else if (yearLength > 1 && yearLength <= 5)
            {
                // Среднесрочный
                periodicity = 2;
            }
            else if (yearLength > 5 && yearLength <= 30)
            {
                // Долгосрочный
                periodicity = 3;
            }
            row.Cells["CurrencyBorrow"].Value = dayCount;
            row.Cells["RefSCreditPeriod"].Value = periodicity;
        }

        void ugData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            /*
            ((UltraGrid)sender).DisplayLayout.Bands[0].Columns["RefSStatusPlan"].Hidden = true;
            UltraGridColumn column = ((UltraGrid)sender).DisplayLayout.Bands[0].Columns.Add("PlanStatus");
            UltraGridHelper.SetLikelyButtonColumnsStyle(column, 0);
            column.Header.VisiblePosition = 2;
             * */
        }

        public override FinSourcePlanningServer GetFinSourcePlanningServer()
        {
            UltraGridRow row = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
            int refOkv = Convert.ToInt32(row.Cells["RefOKV"].Value);
            return refOkv == -1 ? FinSourcePlanningServer.GetCreditIssuedServer() :
                FinSourcePlanningServer.GetCurrensyCreditIssuedServer();
        }

        protected override string t_S_FactAttract
        {
            get { return CreditIssuedObjectsKeys.t_S_FactAttractCO; }
        }

        protected override string t_S_FactDebt
        {
            get { return CreditIssuedObjectsKeys.t_S_FactDebtCO; }
        }

        protected override string a_S_FactAttract
        {
            get
            {
                return CreditIssuedObjectsKeys.a_S_FactAttractCO_RefCreditInc_Key;
            }
        }

        protected override string a_S_FactDebt
        {
            get
            {
                return CreditIssuedObjectsKeys.a_S_FactDebtCO_RefCreditInc_Key;
            }
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            allowAddRecord = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.AddRecord, false);
            allowClearClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ClearData, false);
            allowDelRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.DelRecord, false);
            allowEditRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.EditRecord, false);
            allowImportClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ImportData, false);

            if (!allowAddRecord)
                allowAddRecord = um.CheckPermissionForSystemObject("CreditIssued", (int)FinSourcePlaningUIModuleOperations.AddRecord, false);
            if (!allowClearClassifier)
                allowClearClassifier = um.CheckPermissionForSystemObject("CreditIssued", (int)FinSourcePlaningUIModuleOperations.ClearData, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject("CreditIssued", (int)FinSourcePlaningUIModuleOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject("CreditIssued", (int)FinSourcePlaningUIModuleOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject("CreditIssued", (int)FinSourcePlaningUIModuleOperations.ImportData, false);

            if (!allowAddRecord && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;
        }

        public override void UpdateToolbar()
        {
            base.UpdateToolbar();

            ToolBase tool = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["VariantTransfer"];
            switch (FinSourcePlanningNavigation.Instance.CurrentVariantID)
            {
                case 0:
                    tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[9];
                    tool.SharedProps.ToolTipText = "Перенос договора в архив";
                    break;
                default:
                    tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[8];
                    tool.SharedProps.ToolTipText = "Перенос договора в действующие договора";
                    break;
            }
        }

        internal override bool HasFactAttractionData()
        {
            DataRow activeRow = GetActiveDataRow();
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {

                DataTable dt = Utils.GetDetailTable(db, Convert.ToInt32(activeRow["ID"]),
                                                    CreditIssuedObjectsKeys.a_S_FactAttractCO_RefCreditInc_Key);
                return dt.Rows.Count != 0;
            }
        }

        #region смена активного варианта

        internal override bool HideDetail(string detailKey)
        {
            if (detailKey == CreditIssuedObjectsKeys.t_S_PlanAttractCO ||
                detailKey == CreditIssuedObjectsKeys.t_S_PlanDebtCO ||
                detailKey == CreditIssuedObjectsKeys.t_S_PlanServiceCO ||
                detailKey == CreditIssuedObjectsKeys.t_S_JournalPercentCO)
                return false;
            return true;
        }

        public override string GetCurrentPresentation(IEntity clsObject)
        {
            if (RefVariant > 0 && clsObject.ObjectKey == SchemeObjectsKeys.f_S_Сreditincome_Key)
                PresentationKey = "ed2c1431-77c9-4c9e-8971-74cc12860c29";
            else
                PresentationKey = GetPresentationKey();
            return PresentationKey;
        }

        #endregion
    }
}
