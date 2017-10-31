using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class CreditIncomeUI : BaseCreditUI
    {
        public CreditIncomeUI(IFinSourceBaseService service, string key)
            : base(service, key)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            #region добавление кнопок на тулбары

            UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"];

            ButtonTool tool = CommandService.AttachToolbarTool(new ValidateCommand(GetConfiguration("Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits.CreditIncomes.CreditIncomeValidations.xml")), toolbar);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[1];

            PopupMenuTool pmOperationsList = (PopupMenuTool)toolbar.Tools["OperationsList"];

            CommandService.AttachToolbarTool(new CalcAttractionPlanCommandCI("План привлечения"), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new CalcDebtPlanCommandCI(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new CalcServicePlanCommandCI(), toolbar, "OperationsList");

            // добавление выпадающего списка для расчетов пени
            PopupMenuTool pmPenaltyWizards = new PopupMenuTool("PenaltyWizards");
            pmPenaltyWizards.SharedProps.Caption = "Мастер начисление пени";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmPenaltyWizards);
            pmOperationsList.Tools.AddTool("PenaltyWizards");

            PopupMenuTool pmPenaltyAutocalculate = new PopupMenuTool("PenaltyAutocalculate");
            pmPenaltyAutocalculate.SharedProps.Caption = "Автоматизированное начисление пени";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmPenaltyAutocalculate);
            pmOperationsList.Tools.AddTool("PenaltyAutocalculate");

            CommandService.AttachToolbarTool(new CalculatePenaltiesDebtCommandCI(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new CalculatePenaltiesDebtForAllCommandCI(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new CalculatePenaltiesPercentCreditIncomesCommand(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new CalculatePenaltiesPercentForAllCommandCI(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new DebtPenaltyWizardCommandCI(), toolbar, "PenaltyWizards");
            CommandService.AttachToolbarTool(new CIPercentPenaltyWizardCommand(), toolbar, "PenaltyWizards");

            PopupMenuTool pmCalculate = new PopupMenuTool("Calculate");
            pmCalculate.SharedProps.Caption = "Расчет";

            pmCalculate.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            pmCalculate.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmCalculate);
            toolbar.Tools.AddTool("Calculate");

            tool = CommandService.AttachToolbarTool(new CalcPercentCommandCI(), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            tool = CommandService.AttachToolbarTool(new CalculateCurrencyDiffCommand(SchemeObjectsKeys.t_S_RateSwitchCI_Key), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            tool = CommandService.AttachToolbarTool(new CalculateDebtCommand(), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            tool = CommandService.AttachToolbarTool(new FillDebtRemainderCommand(), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            tool = CommandService.AttachToolbarTool(new CreditCIVariantTransfer(), toolbar);
            tool.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.transfert;

            #endregion
            ((BaseClsView)ViewCtrl).ugeCls.OnInitializeRow += new InitializeRow(ugeCls_OnInitializeRow);
            ((BaseClsView)ViewCtrl).ugeCls.OnGridInitializeLayout += new GridInitializeLayout(ugeCls_OnGridInitializeLayout);
            ((BaseClsView)ViewCtrl).ugeCls.OnBeforeCellDeactivate += new BeforeCellDeactivate(ugeCls_OnBeforeCellDeactivate);
            ((BaseClsView)ViewCtrl).ugeCls.ugData.AfterCellUpdate += uge_AfterCellUpdate;
            ((BaseClsView)ViewCtrl).ugeCls.OnAfterRowActivate += new AfterRowActivate(ugeCls_OnAfterRowActivate);

            DetailsSelectedTabChanging += new SelectedTabChangingEventHandler(CreditIncomeUI_DetailsSelectedTabChanging);
            DetailsSelectedTabChanged += new SelectedTabChangedEventHandler(CreditIncomeUI_DetailsSelectedTabChanged);
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

        private void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            /*
            ((UltraGrid)sender).DisplayLayout.Bands[0].Columns["RefSStatusPlan"].Hidden = true;
            UltraGridColumn column = ((UltraGrid)sender).DisplayLayout.Bands[0].Columns.Add("PlanStatus");
            UltraGridHelper.SetLikelyButtonColumnsStyle(column, 0);
            column.Header.VisiblePosition = 2;
             * */
        }

        private void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
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
                /*
                if (!(row.Cells["RefSStatusDog"].Value is DBNull))
                {
                    switch (Convert.ToInt32(row.Cells["RefSStatusDog"].Value))
                    {
                        case 0: // действующий
                            image = il.Images[5];
                            toolTip = "Действующий договор";
                            break;
                        case 1: // планируемый
                            image = il.Images[6];
                            toolTip = "Планируемый договор";
                            break;
                        case 2: // закрытый
                            image = il.Images[7];
                            toolTip = "Закрытый договор";
                            break;
                    }
                }
                */
                //row.Cells["clmnPlaningStatus"].Appearance.Image = image;
                //row.Cells["clmnPlaningStatus"].ToolTipText = toolTip;

                #endregion

                #region изменение возможности редактирования полей после смены валюты

                if (!(row.Cells["RefOKV"].Value is DBNull))
                {
                    int refOKV = Convert.ToInt32(row.Cells["RefOKV"].Value);
                    e.Row.Cells["Sum"].Activation = refOKV != -1 ?
                        Activation.ActivateOnly :
                        e.Row.Activation;

                    e.Row.Cells["CurrencySum"].Activation = refOKV == -1 ?
                        Activation.ActivateOnly :
                        e.Row.Activation;

                    e.Row.Cells["ExchangeRate"].Activation = refOKV == -1 ?
                        Activation.Disabled :
                        e.Row.Activation;

                    e.Row.Cells["CreditPercent"].Activation = row.Cells["PercentRate"].Value is DBNull ?
                        e.Row.Activation :
                        Activation.ActivateOnly;
                }

                #endregion
            }

            SetContractStatus(row);
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

        protected override void OnDetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.OnDetailGridInitializeLayout(sender, e);

            UltraGrid uge = (UltraGrid)sender;
            string key = ((UltraTabPageControl)uge.Parent.Parent.Parent.Parent).Tab.Key;

            // Добавляем Grand Summaries
            if (key == SchemeObjectsKeys.t_S_PlanDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_PlanServiceCI_Key ||
                key == SchemeObjectsKeys.t_S_FactDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_FactPercentCI_Key ||
                key == SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key ||
                key == SchemeObjectsKeys.t_S_FactPenaltyDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_FactPenaltyPercentCI_Key ||
                key == SchemeObjectsKeys.t_S_PlanAttractCI_Key ||
                key == SchemeObjectsKeys.t_S_FactAttractCI_Key ||
                key == SchemeObjectsKeys.t_S_RateSwitchCI_Key ||
                key == SchemeObjectsKeys.t_S_CollateralCI_Key ||
                key == SchemeObjectsKeys.t_S_CostAttractCI)
            {
                uge.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;

                SummarySettings s = uge.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["Sum"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
                if (uge.DisplayLayout.Bands[0].Columns.Exists("CURRENCYSUM"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["CURRENCYSUM"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
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

            // Добавляем обработчики рассчета сумм валют и подстановки курса валют
            if (key == SchemeObjectsKeys.t_S_PlanAttractCI_Key ||
                key == SchemeObjectsKeys.t_S_PlanDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_PlanServiceCI_Key ||
                key == SchemeObjectsKeys.t_S_FactDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_FactPercentCI_Key ||
                key == SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key ||
                key == SchemeObjectsKeys.t_S_FactPenaltyDebtCI_Key ||
                key == SchemeObjectsKeys.t_S_FactPenaltyPercentCI_Key ||
                key == SchemeObjectsKeys.t_S_FactAttractCI_Key)
            {
                UltraGridHelper.SetLikelyEditButtonColumnsStyle(
                    uge.DisplayLayout.Bands[0].Columns["EXCHANGERATE"], 0);
            }

            // Настройки в зависимости от детали. Расчет вычисляемых полей
            if (activeDetailObject.ObjectKey == SchemeObjectsKeys.t_S_FactDebtCI_Key)
            {
                e.Layout.Bands[0].Columns["FactDate"].SortIndicator = SortIndicator.Ascending;
            }
        }

        #region обработка данных в деталях

        protected override void ugDetailData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Row.Activation != Activation.AllowEdit)
                return;

            #region изменения дат
            UltraGridCell cell = e.Cell;
            if (activeDetailObject.ObjectKey == SchemeObjectsKeys.t_S_PlanServiceCI_Key &&
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
            #endregion

            decimal val = 0;
            decimal rate = 0;
            if (string.Compare(e.Cell.Column.Key, "CURRENCYSUM", true) == 0 ||
                string.Compare(e.Cell.Column.Key, "EXCHANGERATE", true) == 0)
            {
                if (!(e.Cell.Row.Cells["CurrencySum"].Value is DBNull))
                    val = Convert.ToDecimal(e.Cell.Row.Cells["CurrencySum"].Value);

                if (!(e.Cell.Row.Cells["ExchangeRate"].Value is DBNull))
                    rate = Convert.ToDecimal(e.Cell.Row.Cells["ExchangeRate"].Value);

                if (Convert.ToInt32(e.Cell.Row.Cells["RefOKV"].Value) != -1 && val != 0 && rate != 0)
                {
                    e.Cell.Row.Cells["Sum"].Value = val * rate;
                }
            }

            // Изменение вида валюты
            if (e.Cell.Column.Key == "REFOKV" && !(e.Cell.Value is DBNull) && Convert.ToInt32(e.Cell.Value) != -1)
            {
                CalculateCurrencyDiffVisible(true);
                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    string query = "select ExchangeRate from d_S_ExchangeRate where RefOKV = ? and DateFixing = ?";
                    // Пробуем получить курс выбранной валюты из классификатора "Курсы валют" на текущую дату
                    decimal rateValue = Convert.ToDecimal(db.ExecQuery(
                        query, QueryResultTypes.Scalar,
                        new DbParameterDescriptor("RefOKV", e.Cell.Value),
                        new DbParameterDescriptor("DateFixing", DateTime.Today)));
                    if (rateValue != 0)
                    {
                        e.Cell.Row.Cells["ExchangeRate"].Value = rateValue;
                    }
                    else
                    {
                        int count = Convert.ToInt32(db.ExecQuery(
                            "select Count(ID) from d_S_ExchangeRate where RefOKV = ?",
                            QueryResultTypes.Scalar,
                            new DbParameterDescriptor("RefOKV", e.Cell.Value)));
                        if (count > 0)
                        {
                            object[] exchangeValues = new object[2];
                            if (GetExchangeRate(Convert.ToInt32(e.Cell.Value), new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeValues))
                            {
                                e.Cell.Row.Cells["IsPrognozExchRate"].Value = exchangeValues[1];
                                e.Cell.Row.Cells["EXCHANGERATE"].Value = exchangeValues[0];
                                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                                {
                                    e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void AfterSetDetailVisible()
        {
            if (!DetailHasVersions(ActiveDetailKey))
                return;
            UltraTab tab = vo.utcDetails.Tabs[ActiveDetailKey];
            UltraGridEx grid = tab.TabPage.Controls[0] as UltraGridEx;
            if (grid != null)
            {
                UltraToolbar tb = grid.utmMain.Toolbars[2];
                var newCalculate = CommandService.AttachToolbarTool<ButtonTool>(new AddNewPercentCalculation(), tb);
                newCalculate.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Document_Add_icon;
                ComboBoxTool cb = new ComboBoxTool("CalculationResults");
                cb.DropDownStyle = DropDownStyle.DropDownList;
                cb.SharedProps.Width = 200;
                grid.utmMain.Tools.AddRange(new ToolBase[] { cb });
                tb.Tools.AddTool("CalculationResults");
            }
        }

        public void SetCalculationList()
        {
            if (!DetailHasVersions(ActiveDetailKey))
                return;
            UltraTab tab = vo.utcDetails.Tabs[ActiveDetailKey];
            var grid = tab.TabPage.Controls[0] as UltraGridEx;
            if (grid != null)
            {
                UltraToolbar tb = grid.utmMain.Toolbars[2];
                if (tb.Tools.Exists("CalculationResults"))
                    return;
                var newCalculate = CommandService.AttachToolbarTool<ButtonTool>(new AddNewPercentCalculation(), tb);
                newCalculate.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Document_Add_icon;
                var cb = new ComboBoxTool("CalculationResults");
                cb.DropDownStyle = DropDownStyle.DropDownList;
                cb.SharedProps.Width = 200;
                grid.utmMain.Tools.AddRange(new ToolBase[] { cb });
                tb.Tools.AddTool("CalculationResults");
            }
        }

        public override void AddNewPlanCalculation()
        {
            string comment = string.Empty;
            DateTime formDate = DateTime.Now;
            if (frmNewCalculate.GetCaclculationParams(ref comment, ref formDate))
            {
                var newPercent = new VersionParams(formDate, comment);
                UltraGridEx gridEx = GetDetailGridEx(ActiveDetailKey);
                UltraToolbar tb = gridEx.utmMain.Toolbars[2];
                var comboBoxTool = tb.Tools["CalculationResults"] as ComboBoxTool;
                string percentCaption = string.Format(calcListDataFormat, comment, formDate.ToShortDateString());
                int index = comboBoxTool.ValueList.FindString(percentCaption);
                if (index >= 0)
                    comboBoxTool.SelectedIndex = index;
                else
                {
                    var newItem = comboBoxTool.ValueList.ValueListItems.Add(newPercent, percentCaption);
                    comboBoxTool.SelectedItem = newItem;
                }
            }
        }

        #endregion

        /// <summary>
        /// Рассчет сумм валют и подстановка курса валют.
        /// </summary>
        private void uge_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Row.Activation != Activation.AllowEdit)
                return;

            decimal val = 0;
            decimal rate = 0;
            if (string.Compare(e.Cell.Column.Key, "CURRENCYSUM", true) == 0 ||
                string.Compare(e.Cell.Column.Key, "EXCHANGERATE", true) == 0)
            {
                if (!(e.Cell.Row.Cells["CurrencySum"].Value is DBNull))
                    val = Convert.ToDecimal(e.Cell.Row.Cells["CurrencySum"].Value);

                if (!(e.Cell.Row.Cells["ExchangeRate"].Value is DBNull))
                    rate = Convert.ToDecimal(e.Cell.Row.Cells["ExchangeRate"].Value);

                if (Convert.ToInt32(e.Cell.Row.Cells["RefOKV"].Value) != -1 && val != 0 && rate != 0)
                {
                    e.Cell.Row.Cells["Sum"].Value = val * rate;
                }
            }

            // Изменение вида валюты
            if (e.Cell.Column.Key == "REFOKV" && !(e.Cell.Value is DBNull) && Convert.ToInt32(e.Cell.Value) != -1)
            {
                CalculateCurrencyDiffVisible(true);
                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    string query = "select ExchangeRate from d_S_ExchangeRate where RefOKV = ? and DateFixing = ?";
                    // Пробуем получить курс выбранной валюты из классификатора "Курсы валют" на текущую дату
                    decimal rateValue = Convert.ToDecimal(db.ExecQuery(
                        query,
                        QueryResultTypes.Scalar,
                        new System.Data.OleDb.OleDbParameter("RefOKV", e.Cell.Value),
                        new System.Data.OleDb.OleDbParameter("DateFixing", DateTime.Today)));
                    if (rateValue != 0)
                    {
                        e.Cell.Row.Cells["ExchangeRate"].Value = rateValue;
                    }
                    else
                    {
                        int count = Convert.ToInt32(db.ExecQuery(
                            "select Count(ID) from d_S_ExchangeRate where RefOKV = ?",
                            QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("RefOKV", e.Cell.Value)));
                        if (count > 0)
                        {
                            object[] exchangeValues = new object[2];
                            if (GetExchangeRate(Convert.ToInt32(e.Cell.Value), new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeValues))
                            {
                                e.Cell.Row.Cells["IsPrognozExchRate"].Value = exchangeValues[1];
                                e.Cell.Row.Cells["EXCHANGERATE"].Value = exchangeValues[0];
                                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                                {
                                    e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                                }
                            }
                        }
                    }
                }
            }

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

            if (string.Compare(e.Cell.Column.Key, "RefSStatusPlan", true) == 0)
            {
                SetContractStatus(e.Cell.Row);
            }
        }

        protected override void ugeDetail_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (string.Compare(UltraGridEx.GetSourceColumnName(e.Cell.Column.Key), "EXCHANGERATE", true) == 0)
            {
                int refOKV = Convert.ToInt32(e.Cell.Row.Cells["REFOKV"].Value);
                object[] exchangeValues = new object[2];
                if (GetExchangeRate(refOKV, new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeValues))
                {
                    if (e.Cell.Row.Activation != Activation.AllowEdit)
                        return;

                    e.Cell.Row.Cells["IsPrognozExchRate"].Value = exchangeValues[1];
                    e.Cell.Row.Cells["EXCHANGERATE"].Value = exchangeValues[0];
                    if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                    {
                        e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                    }
                }
                return;
            }

            if (string.Compare(UltraGridEx.GetSourceColumnName(e.Cell.Column.Key), "RefColltrlObjects", true) == 0)
            {
                ModalClsManager manager = new ModalClsManager(Workplace);
                object refId = null;
                DataTable dtData = new DataTable();
                if (manager.ShowClsModal(SchemeObjectsKeys.d_S_CollateralObjects_Key, -1, CurrentDataSourceID, CurrentDataSourceYear, ref refId, ref dtData))
                {
                    e.Cell.Row.Cells["RefColltrlObjects"].Value = dtData.Rows[0]["ID"];
                    e.Cell.Row.Cells["Name"].Value = dtData.Rows[0]["Name"];
                    e.Cell.Row.Cells["Cost"].Value = dtData.Rows[0]["Cost"];
                    e.Cell.Row.Cells["Note"].Value = dtData.Rows[0].IsNull("Area") ?
                        string.Format("По адресу {0}", dtData.Rows[0]["Address"]) :
                        string.Format("По адресу {0}, площадью {1}", dtData.Rows[0]["Address"], dtData.Rows[0]["Area"]);
                }
                return;
            }

            base.ugeDetail_OnClickCellButton(sender, e);
        }

        private void CreditIncomeUI_DetailsSelectedTabChanging(object sender, SelectedTabChangingEventArgs e)
        {
            if (e.Tab == null)
                return;
            if (e.Tab.TabPage.Controls.Count == 0)
                return;
            Control ctrl = e.Tab.TabPage.Controls[0];
            if (ctrl is UltraGridEx)
            {
                //((UltraGridEx)ctrl).ugData.ClickCellButton -= ugeCls_OnClickCellButton;
            }
        }

        private void CreditIncomeUI_DetailsSelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.TabPage.Controls.Count == 0)
                return;
            Control ctrl = e.Tab.TabPage.Controls[0];
            if (ctrl is UltraGridEx)
            {
                //((UltraGridEx)ctrl).ugData.ClickCellButton += ugeCls_OnClickCellButton;
            }
        }

        protected override void InitializeDetailRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("RefOKV"))
            {
                UltraGridRow masterRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
                int refOKV = Convert.ToInt32(masterRow.Cells["RefOKV"].Value);
                e.Row.Cells["Sum"].Activation = refOKV != -1 ?
                     Activation.ActivateOnly :
                     e.Row.Activation;

                e.Row.Cells["CurrencySum"].Activation = refOKV == -1 ?
                    Activation.ActivateOnly :
                    e.Row.Activation;

                e.Row.Cells["ExchangeRate"].Activation = refOKV == -1 ?
                    Activation.Disabled :
                    e.Row.Activation;
            }
        }

        protected override bool ValidateDataTable(DataTable table, ref string errorString, GridColumnsStates states)
        {
            if (table.Columns.Contains("RefOKV"))
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        int refOkv = Convert.ToInt32(row["RefOKV"]);
                        if (refOkv != -1 && row.IsNull("CurrencySum"))
                        {
                            errorString = string.Format("Поле 'Сумма договора в валюте' записи с ID = {0} не заполнено",
                                                        row["ID"]);
                            return false;
                        }
                    }
                }
            }
            return base.ValidateDataTable(table, ref errorString, states);
        }

        public override FinSourcePlanningServer GetFinSourcePlanningServer()
        {
            UltraGridRow row = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
            int refOkv = Convert.ToInt32(row.Cells["RefOKV"].Value);
            return refOkv == -1 ? FinSourcePlanningServer.GetPlaningIncomesServer() :
                FinSourcePlanningServer.GetCurrencyPlaningIncomesServer();
        }

        protected override bool SaveDetailData(UltraGridEx gridEx)
        {
            if (base.SaveDetailData(gridEx))
            {
                // действия по сохранению данных детали одного договора в другой

                return true;
            }
            return false;
        }

        protected override string t_S_FactAttract
        {
            get { return SchemeObjectsKeys.t_S_FactAttractCI_Key; }
        }

        protected override string t_S_FactDebt
        {
            get { return SchemeObjectsKeys.t_S_FactDebtCI_Key; }
        }

        protected override string a_S_FactAttract
        {
            get
            {
                return SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key;
            }
        }

        protected override string a_S_FactDebt
        {
            get
            {
                return SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key;
            }
        }

        private void CalculateCurrencyDiffVisible(bool visible)
        {
            UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"];
            ButtonTool tool = (ButtonTool)toolbar.ToolbarsManager.Tools["CalculateCurrencyDiffCommand"];
            tool.SharedProps.Visible = visible;
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
                allowAddRecord = um.CheckPermissionForSystemObject("CreditIncomes", (int)FinSourcePlaningUIModuleOperations.AddRecord, false);
            if (!allowClearClassifier)
                allowClearClassifier = um.CheckPermissionForSystemObject("CreditIncomes", (int)FinSourcePlaningUIModuleOperations.ClearData, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject("CreditIncomes", (int)FinSourcePlaningUIModuleOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject("CreditIncomes", (int)FinSourcePlaningUIModuleOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject("CreditIncomes", (int)FinSourcePlaningUIModuleOperations.ImportData, false);

            if (!allowAddRecord && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;
        }

        protected override void ugeCls_OnAfterRowActivate(object sender, EventArgs e)
        {
            base.ugeCls_OnAfterRowActivate(sender, e);

            UltraGridRow gridRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
            if (gridRow.Cells["ID"].Value is DBNull)
                return;
            DataRow row = GetActiveDataRow();
            if (row == null)
                return;
            CalculateCurrencyDiffVisible(Convert.ToInt32(row["RefOKV"]) != -1);
        }

        internal override bool HasFactAttractionData()
        {
            DataRow activeRow = GetActiveDataRow();
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {

                DataTable dt = Utils.GetDetailTable(db, Convert.ToInt32(activeRow["ID"]),
                                                    SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key);
                return dt.Rows.Count != 0;
            }
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);

            vo.utcDetails.Tabs[SchemeObjectsKeys.t_S_ListContractCl].Visible = false;
        }

        #region список расчетов процентов

        private const string calcListDataFormat = "{0} ({1})";

        protected override void RefreshDetailCalculationList(string detailKey)
        {
            DataRow activeMasterRow = GetActiveDataRow();
            if (activeMasterRow == null)
                return;
            UltraGridEx gridEx = GetDetailGridEx(detailKey);
            UltraToolbar tb = gridEx.utmMain.Toolbars[2];
            if (!tb.Tools.Exists("CalculationResults"))
                return;
            ComboBoxTool comboBoxTool = tb.Tools["CalculationResults"] as ComboBoxTool;
            // очищаем список
            comboBoxTool.ValueList.ValueListItems.Clear();
            Dictionary<string, VersionParams> percents = GetPercentsCalculations(activeMasterRow);
            foreach (KeyValuePair<string, VersionParams> percent in percents)
            {
                comboBoxTool.ValueList.ValueListItems.Add(percent.Value, percent.Key);
            }
            int index =
                comboBoxTool.ValueList.FindString(string.Format(calcListDataFormat, FormComment,
                                                                FormDate.ToShortDateString()));
            int activeCalculationIndex = index != -1 ? index : comboBoxTool.ValueList.ValueListItems.Count - 1;
            comboBoxTool.SelectedIndex = activeCalculationIndex;
        }

        private Dictionary<string, VersionParams> GetPercentsCalculations(DataRow activeParentRow)
        {
            Dictionary<string, VersionParams> percentsCalculations = new Dictionary<string, VersionParams>();
            IEntity detail = Workplace.ActiveScheme.RootPackage.FindEntityByName(ActiveDetailKey);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt = (DataTable)db.ExecQuery(
                    string.Format("select distinct EstimtDate, CalcComment from {0} where RefCreditInc = ? order by EstimtDate", detail.FullDBName),
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", activeParentRow["ID"]));

                DataRow[] noParamsRow = dt.Select("EstimtDate is null");
                if (noParamsRow != null && noParamsRow.Length > 0)
                    noParamsRow[0]["EstimtDate"] = DateTime.MinValue;


                foreach (DataRow row in dt.Select(string.Empty, "EstimtDate Asc"))
                {
                    if (row.IsNull("EstimtDate") || row.IsNull("CalcComment"))
                    {
                        percentsCalculations.Add("Без параметров", new VersionParams("Без параметров"));
                        continue;
                    }

                    DateTime formDate = Convert.ToDateTime(row["EstimtDate"]);
                    string comment = row["CalcComment"].ToString();
                    var percents = new VersionParams(formDate, comment);
                    percentsCalculations.Add(string.Format(calcListDataFormat, comment, formDate.ToShortDateString()), percents);
                }
                return percentsCalculations;
            }
        }

        protected override void DetailGridSetup(UltraGridEx ugeDetail)
        {
            base.DetailGridSetup(ugeDetail);

            ugeDetail.utmMain.ToolValueChanged += new ToolEventHandler(utmMain_ToolValueChanged);
        }

        void utmMain_ToolValueChanged(object sender, ToolEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "CalculationResults":
                    string detailKey = activeDetailObject.ObjectKey;
                    ComboBoxTool comboBoxTool = e.Tool as ComboBoxTool;
                    if (comboBoxTool.SelectedIndex != -1)
                    {
                        var percents = (VersionParams)comboBoxTool.ValueList.ValueListItems[comboBoxTool.SelectedIndex].DataValue;
                        if (percents.CalculationDate == null)
                        {
                            detailFilters[detailKey] = " and EstimtDate is null and CalcComment is null";
                            detailFilterParams[detailKey] = null;
                            activeCalculationIndexes[detailKey] = comboBoxTool.SelectedIndex;
                        }
                        else
                        {
                            detailFilters[detailKey] = " and EstimtDate = ? and CalcComment = ?";
                            DbParameterDescriptor[] filterParams = new DbParameterDescriptor[2];
                            filterParams[0] = new DbParameterDescriptor("EstimtDate", percents.CalculationDate);
                            filterParams[1] = new DbParameterDescriptor("CalcComment", percents.CalculationComment);
                            detailFilterParams[detailKey] = filterParams;
                            activeCalculationIndexes[detailKey] = comboBoxTool.SelectedIndex;
                        }
                    }
                    IsRefreshDetailCalculationList = false;
                    RefreshDetail();
                    SortDetailData();
                    IsRefreshDetailCalculationList = true;
                    break;
            }
        }

        /// <summary>
        /// получение упдатера для получения данных
        /// </summary>
        protected override IDataUpdater GetDetailUpdater(IEntity activeDetailObject, object masterValue)
        {
            string detailKey = activeDetailObject.ObjectKey;
            string detailFilter = detailFilters.ContainsKey(detailKey) ? detailFilters[detailKey] : string.Empty;
            DbParameterDescriptor[] filterParams = detailFilterParams.ContainsKey(detailKey) ? detailFilterParams[detailKey] : null;
            if (Convert.ToInt64(masterValue) >= 0)
                return !string.IsNullOrEmpty(detailFilter) ?
                    activeDetailObject.GetDataUpdater(GetQueryFilter(masterValue) + detailFilter, null, filterParams) :
                    activeDetailObject.GetDataUpdater(GetQueryFilter(masterValue), null, null);
            return activeDetailObject.GetDataUpdater("1 = 2", null, null);
        }

        /// <summary>
        /// Возвращает фильтровое условие по ссылке на мастер-таблицу.
        /// </summary>
        // TODO перенести в BaseDetailTableUI
        private string GetQueryFilter(object masterValue)
        {
            return
                String.Format("({0} = {1})", DetailRefColumnNames[vo.utcDetails.ActiveTab.Key], masterValue);
        }

        public override void RefreshDetail(string detailKey)
        {
            if (DetailHasVersions(detailKey))
            {
                UltraTab tab = GetDetailTab(detailKey);
                tab.Selected = true;
                base.RefreshDetail(detailKey);
                
            }
            else
                base.RefreshDetail(detailKey);
            SortDetailData();
        }

        public void SortDetailData()
        {
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("Payment"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["Payment"].SortIndicator = SortIndicator.Ascending;
        }

        protected override void utcDetails_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab == null)
                return;
            ActiveDetailKey = e.Tab.Key;

            SetCalculationList();
            base.utcDetails_SelectedTabChanged(sender, e);

            if (!activeCalculationIndexes.ContainsKey(ActiveDetailKey))
            {
                activeCalculationIndexes.Add(ActiveDetailKey, -1);
                detailFilters.Add(ActiveDetailKey, string.Empty);
                detailFilterParams.Add(ActiveDetailKey, null);
            }
            if (DetailHasVersions(ActiveDetailKey))
            {
                UltraToolbar tb = activeDetailGrid.utmMain.Toolbars[2];
                ComboBoxTool comboBoxTool = tb.Tools["CalculationResults"] as ComboBoxTool;
                if (comboBoxTool.ValueList.ValueListItems.Count > 0)
                {
                    comboBoxTool.SelectedIndex = activeCalculationIndexes[ActiveDetailKey];
                    comboBoxTool.ValueList.SelectedIndex = activeCalculationIndexes[ActiveDetailKey];
                }
            }
        }

        protected override void ugeDetail_OnClearCurrentTable(object sender)
        {
            string deleteFilter = string.Format("where {0}", GetQueryFilter(GetActiveDataRow()["ID"]));

            if (detailFilters.ContainsKey(ActiveDetailKey))
                deleteFilter = string.Concat(deleteFilter, detailFilters[ActiveDetailKey]);

            if (MessageBox.Show("Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                try
                {
                    using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                    {
                        db.ExecQuery(string.Format("delete from {0} {1}", activeDetailObject.FullDBName, deleteFilter),
                                     QueryResultTypes.NonQuery, detailFilterParams[ActiveDetailKey]);
                    }
                    dsDetail.Tables[0].BeginLoadData();
                    dsDetail.Tables[0].AcceptChanges();
                    foreach (DataRow row in dsDetail.Tables[0].Rows)
                    {
                        row.Delete();
                    }
                    dsDetail.Tables[0].AcceptChanges();
                    dsDetail.Tables[0].EndLoadData();
                }
                finally
                {
                    activeDetailGrid.BurnChangesDataButtons(false);
                    Workplace.OperationObj.StopOperation();
                }
            }
            RefreshDetail(ActiveDetailKey);
        }

        protected override void AfterDetailRowInsert(object sender, UltraGridRow row)
        {
            base.AfterDetailRowInsert(sender, row);

            string detailKey = activeDetailObject.ObjectKey;
            if (detailFilterParams[activeDetailObject.ObjectKey] != null)
            {
                foreach (DbParameterDescriptor param in detailFilterParams[detailKey])
                {
                    row.Cells[param.ParameterName].Value = param.Value;
                }
            }
        }

        private Dictionary<string, int> activeCalculationIndexes = new Dictionary<string, int>();

        private Dictionary<string, string> detailFilters = new Dictionary<string, string>();

        private Dictionary<string, DbParameterDescriptor[]> detailFilterParams = new Dictionary<string, DbParameterDescriptor[]>();

        #endregion

        #region смена активного варианта

        internal override bool HideDetail(string detailKey)
        {
            if (detailKey == SchemeObjectsKeys.t_S_PlanAttractCI_Key ||
                detailKey == SchemeObjectsKeys.t_S_PlanDebtCI_Key ||
                detailKey == SchemeObjectsKeys.t_S_PlanServiceCI_Key ||
                detailKey == SchemeObjectsKeys.t_S_JournalPercentCI_Key)
                return false;
            return true;
        }

        #endregion

        public override string GetCurrentPresentation(IEntity clsObject)
        {
            if (RefVariant > 0 && clsObject.ObjectKey == SchemeObjectsKeys.f_S_Сreditincome_Key)
                PresentationKey = "6f02e051-c83e-4835-9ca0-f3ad572b10fe";
            else
                PresentationKey = GetPresentationKey();
            return PresentationKey;
        }

        internal override void HideCurrencyColumns(int currency)
        {
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("CurrencySum"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["CurrencySum"].Hidden = currency == -1;
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("ExchangeRate"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["ExchangeRate"].Hidden = currency == -1;
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("IsPrognozExchRate"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["IsPrognozExchRate"].Hidden = currency == -1;
        }
    }
}
