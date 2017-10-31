using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands.GuarantyCommands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.Client.Reports;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.GuaranteeIssued
{
    class GuaranteeIssuedUI : GuaranteeUI
    {
        public GuaranteeIssuedUI(IFinSourceBaseService service, string key)
			: base(service, key)
		{
		}

        public override void Initialize()
        {
            base.Initialize();

            UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"];
            CommandService.AttachToolbarTool(new FillAttractPlanCommand(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new FillPlanDebtCommand(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new FillPlanServiceCommand(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new CalcObligationExecutionPlanCommand(), toolbar, "OperationsList");

            ButtonTool tool = CommandService.AttachToolbarTool(new ValidateCommand(GetConfiguration("Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.GuaranteeIssued.GuaranteeIssuedValidation.xml")), toolbar);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[1];

            PopupMenuTool pmOperationsList = (PopupMenuTool)toolbar.Tools["OperationsList"];
            // добавление выпадающего списка для расчетов пени
            PopupMenuTool pmPenaltyWizards = new PopupMenuTool("PenaltyWizards");
            pmPenaltyWizards.SharedProps.Caption = "Мастер начисление пени";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmPenaltyWizards);
            pmOperationsList.Tools.AddTool("PenaltyWizards");

            PopupMenuTool pmPenaltyAutocalculate = new PopupMenuTool("PenaltyAutocalculate");
            pmPenaltyAutocalculate.SharedProps.Caption = "Автоматизированное начисление пени";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmPenaltyAutocalculate);
            pmOperationsList.Tools.AddTool("PenaltyAutocalculate");

            CommandService.AttachToolbarTool(new CalculateDebtPenaltyCommand(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new CalculatePercentPenaltyCommand(), toolbar, "PenaltyAutocalculate");
            CommandService.AttachToolbarTool(new GuaranteeDebtPenaltyWizardCommand(), toolbar, "PenaltyWizards");
            CommandService.AttachToolbarTool(new GuaranteePercentPenaltyWizardCommand(), toolbar, "PenaltyWizards");

            GovernmentGuaranteeReportCommand reportCommand = new GovernmentGuaranteeReportCommand();
            reportCommand.scheme = Workplace.ActiveScheme;
            reportCommand.operationObj = Workplace.OperationObj;
            reportCommand.window = Workplace.WindowHandle;
            tool = CommandService.AttachToolbarTool(reportCommand, toolbar, "Templates");
            tool.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.excelDocument;

            var pmCalculate = new PopupMenuTool("Calculate");
            pmCalculate.SharedProps.Caption = "Расчет";
            pmCalculate.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            pmCalculate.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmCalculate);
            toolbar.Tools.AddTool("Calculate");

            tool = CommandService.AttachToolbarTool(new CalcGuaranteePercentCommand(), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            tool = CommandService.AttachToolbarTool(new CalculateGuarantyBalanceCommand(), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            tool = CommandService.AttachToolbarTool(new GuaranteeVariantTransferCommand(), toolbar);
            tool.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.transfert;

            // переопределяем основные события, обработка которых понадобится в работе
            ((BaseClsView)ViewCtrl).ugeCls.OnClickCellButton += ugeCls_OnClickCellButton;
            ((BaseClsView)ViewCtrl).ugeCls.OnGridInitializeLayout += ugeCls_OnGridInitializeLayout;
            ((BaseClsView)ViewCtrl).ugeCls.ugData.AfterCellUpdate += ugData_AfterCellUpdate;

            DetailGridInitializeLayout += CreditIncomeUI_DetailGridInitializeLayout;
            DetailsSelectedTabChanging += CreditIncomeUI_DetailsSelectedTabChanging;
            DetailsSelectedTabChanged += CreditIncomeUI_DetailsSelectedTabChanged;
        }

		public override bool  CheckVisibility(string key)
		{
			if (key == "2916269c-7343-4953-8ac9-3cb8601e6a78" || //semantic="S" name="FactDebtGrnt" caption="Фактические выплаты принципала" description="Отражаются фактические выплаты принципала."
				key == "0eeb8c2e-33d6-41ea-bf1b-90f0ee41a3ff")   //semantic="S" name="PlanDebtGrnt" caption="Плановые выплаты принципала" description="Отражаются плановые выплаты между принципалом и бенефициаром."
				return false;
			return true;
		}
	
        void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            /*if (!e.Layout.Bands[0].Columns.Exists("clmnValidate"))
            {
                UltraGridColumn clmn = e.Layout.Bands[0].Columns.Add("clmnValidate", string.Empty);
                clmn.Header.VisiblePosition = 1;
                clmn.Header.Caption = string.Empty;
                UltraGridHelper.SetLikelyImageColumnsStyle(clmn, -1);
            }*/

            UltraGridHelper.SetLikelyEditButtonColumnsStyle(
                ((UltraGrid)sender).DisplayLayout.Bands[0].Columns["EXCHANGERATE"], 0);
            /*
            ((UltraGrid) sender).DisplayLayout.Bands[0].Columns["RefSStatusPlan"].Hidden = true;
            UltraGridColumn column = ((UltraGrid) sender).DisplayLayout.Bands[0].Columns.Add("PlanStatus");
            UltraGridHelper.SetLikelyButtonColumnsStyle(column, 0);
            column.Header.VisiblePosition = 2;*/
            //column.CellButtonAppearance.Image = Resources.ru.Error;
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            string cellKey = e.Cell.Column.Key;

            if (e.Cell.Row.Activation != Activation.AllowEdit)
                return;
            decimal val = 0;
            decimal rate = 0;
            int refOKV = e.Cell.Row.Cells["RefOKV"].Value is DBNull ? -1 : Convert.ToInt32(e.Cell.Row.Cells["RefOKV"].Value);

            #region изменение курса валюты и (или) суммы валюты
            if (e.Cell.Column.Key == "CURRENCYSUM" || e.Cell.Column.Key == "EXCHANGERATE")
            {
                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull)
                {}
                else
                    val = Convert.ToDecimal(e.Cell.Row.Cells["CurrencySum"].Value);

                if (e.Cell.Row.Cells["ExchangeRate"].Value is DBNull)
                {}
                else
                    rate = Convert.ToDecimal(e.Cell.Row.Cells["ExchangeRate"].Value);

                if (refOKV != -1 && val != 0 && rate != 0)
                {
                    e.Cell.Row.Cells["Sum"].Value = val * rate;
                }
            }
            #endregion

            #region Изменение вида валюты
            if (e.Cell.Column.Key == "REFOKV" && !(e.Cell.Value is DBNull) && Convert.ToInt32(e.Cell.Value) != -1)
            {
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
                            "select Count(*) from d_S_ExchangeRate where RefOKV = ?",
                            QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("RefOKV", e.Cell.Value)));
                        if (count > 0)
                        {
                            // получаем нужный классификатор
                            IClassifier cls = Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_RateValue];
                            // создаем объект просмотра классификаторов нужного типа
                            RateValueDataClsUI clsUI = new RateValueDataClsUI(cls, refOKV);
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
                            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["RefOKV"].FilterConditions.Add(FilterComparisionOperator.Equals, e.Cell.Value);
                            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["DateFixing"].SortIndicator
                                = SortIndicator.Descending;

                            if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
                            {
                                int clsID = modalClsForm.AttachedCls.GetSelectedID();
                                object[] exchangeRate = new object[2];
                                modalClsForm.AttachedCls.GetColumnsValues(new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeRate);
                                // если ничего не выбрали - считаем что функция завершилась неудачно
                                if (clsID == -10)
                                    return;
                                e.Cell.Row.Cells["IsPrognozExchRate"].Value = exchangeRate[1];
                                e.Cell.Row.Cells["EXCHANGERATE"].Value = exchangeRate[0];

                                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                                {
                                    e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

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

            if (string.Compare(cellKey, "RefSStatusPlan", true) == 0)
            {
                SetContractStatus(e.Cell.Row);
            }
        }

        private void SetContractStatus(UltraGridRow  contractRow)
        {
            if (contractRow.Cells["RefSStatusPlan"].Value == DBNull.Value || contractRow.Cells["RefSStatusPlan"].Value == null)
                return;
            int contractStatus = Convert.ToInt32(contractRow.Cells["RefSStatusPlan"].Value);
            switch (contractStatus)
            {
                case -1:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.UnknownContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Неуказанный статус договора";
                    break;
                case 0:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.ApplyContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Принят";
                    break;
                case 1:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.Not_ApplyContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Не принят";
                    break;
                case 2:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.lock_icon;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Рефинансирован";
                    break;
                case 3:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.lock_icon;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Досрочно погашен";
                    break;
                case 4:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.ClosedContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Закрыт";
                    break;
                case 5:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.coins_icon;
                    contractRow.Cells["PlanStatus"].ToolTipText = "На рефинансирование";
                    break;
            }
        }

        protected override void ugDetailData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Row.Activation != Activation.AllowEdit)
                return;

            decimal val = 0;
            decimal rate = 0;
            int refOKV = 0;
            if (dsDetail.Tables[0].Columns.Contains("RefOKV"))
                refOKV = e.Cell.Row.Cells["RefOKV"].Value is DBNull ? -1 : Convert.ToInt32(e.Cell.Row.Cells["RefOKV"].Value);

            #region изменение курса валюты и (или) суммы валюты
            if (e.Cell.Column.Key == "CURRENCYSUM" || e.Cell.Column.Key == "EXCHANGERATE")
            {
                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull)
                { }
                else
                    val = Convert.ToDecimal(e.Cell.Row.Cells["CurrencySum"].Value);

                if (e.Cell.Row.Cells["ExchangeRate"].Value is DBNull)
                { }
                else
                    rate = Convert.ToDecimal(e.Cell.Row.Cells["ExchangeRate"].Value);

                if (refOKV != -1 && val != 0 && rate != 0)
                {
                    e.Cell.Row.Cells["Sum"].Value = val * rate;
                }
            }
            #endregion

            #region Изменение вида валюты
            if (e.Cell.Column.Key == "REFOKV" && !(e.Cell.Value is DBNull) && Convert.ToInt32(e.Cell.Value) != -1)
            {
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
                            "select Count(*) from d_S_ExchangeRate where RefOKV = ?",
                            QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("RefOKV", e.Cell.Value)));
                        if (count > 0)
                        {
                            // получаем нужный классификатор
                            IClassifier cls = Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_RateValue];
                            // создаем объект просмотра классификаторов нужного типа
                            RateValueDataClsUI clsUI = new RateValueDataClsUI(cls, refOKV);
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
                            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["RefOKV"].FilterConditions.Add(FilterComparisionOperator.Equals, e.Cell.Value);
                            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["DateFixing"].SortIndicator
                                = SortIndicator.Descending;

                            if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
                            {
                                int clsID = modalClsForm.AttachedCls.GetSelectedID();
                                object[] exchangeRate = new object[2];
                                modalClsForm.AttachedCls.GetColumnsValues(new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeRate);
                                // если ничего не выбрали - считаем что функция завершилась неудачно
                                if (clsID == -10)
                                    return;
                                e.Cell.Row.Cells["IsPrognozExchRate"].Value = exchangeRate[1];
                                e.Cell.Row.Cells["EXCHANGERATE"].Value = exchangeRate[0];

                                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                                {
                                    e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region изменения дат
            if (activeDetailObject.ObjectKey != GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key)
                return;
            UltraGridCell cell = e.Cell;
            if (string.Compare(cell.Column.Key, "STARTDATE", true) == 0 ||
                string.Compare(cell.Column.Key, "ENDDATE", true) == 0)
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                DateTime.TryParse(cell.Row.Cells["STARTDATE"].Value.ToString(), out startDate);
                DateTime.TryParse(cell.Row.Cells["ENDDATE"].Value.ToString(), out endDate);

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    ChangePeriod(startDate, endDate, cell.Row);
                }
            }
            #endregion
        }

        private void ugeCls_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (string.Compare(e.Cell.Column.Key, "PlanStatus", true) == 0)
            {
                object[] planStatus = new object[1];
                if (GetStatusPlan(ref planStatus))
                {
                    e.Cell.Row.Cells["RefSStatusPlan"].Value = planStatus[0];
                    e.Cell.Row.Update();
                }
            }

            if (UltraGridEx.GetSourceColumnName(e.Cell.Column.Key) != "EXCHANGERATE")
                return;

            int refOKV = Convert.ToInt32(e.Cell.Row.Cells["REFOKV"].Value);

            // получаем нужный классификатор
            IClassifier cls = Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_RateValue];
            // создаем объект просмотра классификаторов нужного типа
            RateValueDataClsUI clsUI = new RateValueDataClsUI(cls, refOKV);
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
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["REFOKV"].FilterConditions.Add(FilterComparisionOperator.Equals, refOKV);
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["DateFixing"].SortIndicator
                = SortIndicator.Descending;

            if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
            {
                if (e.Cell.Row.Activation != Activation.AllowEdit)
                    return;
                int clsID = modalClsForm.AttachedCls.GetSelectedID();
                object[] rate = new object[2];
                modalClsForm.AttachedCls.GetColumnsValues(new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref rate);
                // если ничего не выбрали - считаем что функция завершилась неудачно
                if (clsID == -10)
                    return;
                e.Cell.Row.Cells["IsPrognozExchRate"].Value = rate[1];
                e.Cell.Row.Cells["EXCHANGERATE"].Value = rate[0];
                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                {
                    e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                }
            }
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
                ((UltraGridEx)ctrl).ugData.ClickCellButton -= ugeCls_OnClickCellButton;
            }
        }

        private void CreditIncomeUI_DetailsSelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.TabPage.Controls.Count == 0)
                return;
            Control ctrl = e.Tab.TabPage.Controls[0];
            if (ctrl is UltraGridEx)
            {
                ((UltraGridEx)ctrl).ugData.ClickCellButton += ugeCls_OnClickCellButton;
            }
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
                }

                #endregion

                if (!(row.Cells["Irrevocability"].Value is DBNull))
                {
                    e.Row.Cells["Condition"].Activation = Convert.ToBoolean(row.Cells["Irrevocability"].Value) ?
                        Activation.ActivateOnly :
                        e.Row.Activation;
                }

                SetContractStatus(row);
            }
        }

        /// <summary>
        /// Рассчитывает длительность финансовой операции, определяет срочность кредита и заполняет соответствующие поля.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="row"></param>
        private static void ChangePeriod(DateTime startDate, DateTime endDate, UltraGridRow row)
        {
            int dayCount = (endDate - startDate).Days + 1;
            if (row.Cells.Exists("DayCount"))
                row.Cells["DayCount"].Value = dayCount;
            if (row.Cells.Exists("GuarantPeriod"))
                row.Cells["GuarantPeriod"].Value = dayCount;
        }

        private void CreditIncomeUI_DetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid uge = (UltraGrid)sender;
            string key = ((UltraTabPageControl)uge.Parent.Parent.Parent.Parent).Tab.Key;

            // Добавляем Grand Summaries
            if (key == GuaranteeIssuedObjectKeys.t_S_PlanAttractPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactAttractPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PlanDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactPercentPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_ChargePenaltyDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PrGrntChargePenaltyPercent_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactPenaltyDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactPenaltyPercentPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactAttractGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PlanAttractGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_CollateralGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FineSanctionGrnt)
            {
                uge.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
                SummarySettings s = null;
                if (uge.DisplayLayout.Bands[0].Columns.Exists("SUM"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["Sum"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
                if (uge.DisplayLayout.Bands[0].Columns.Exists("CURRENCYSUM"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["CURRENCYSUM"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
                if (uge.DisplayLayout.Bands[0].Columns.Exists("CURRENCYMARGIN"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["CURRENCYMARGIN"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
                if (uge.DisplayLayout.Bands[0].Columns.Exists("MARGIN"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["MARGIN"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
                if (uge.DisplayLayout.Bands[0].Columns.Exists("CURRENCYCOMMISSION"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["CURRENCYCOMMISSION"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
                if (uge.DisplayLayout.Bands[0].Columns.Exists("COMMISSION"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["COMMISSION"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }

                if (uge.DisplayLayout.Bands[0].Columns.Exists("LATEDATE"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["LATEDATE"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }

                if (uge.DisplayLayout.Bands[0].Columns.Exists("DAYCOUNT"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["DAYCOUNT"]);
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

                if (uge.DisplayLayout.Bands[0].Columns.Exists("SUMFINE"))
                {
                    s = uge.DisplayLayout.Bands[0].Summaries.Add(
                        SummaryType.Sum, uge.DisplayLayout.Bands[0].Columns["SUMFINE"]);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
            }

            // Добавляем обработчики рассчета сумм валют и подстановки курса валют
            if (key == GuaranteeIssuedObjectKeys.t_S_PlanAttractGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactAttractGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PrincipalContrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PlanDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactPercentPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_ChargePenaltyDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PrGrntChargePenaltyPercent_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactPenaltyDebtPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactPenaltyPercentPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_FactAttractPrGrnt_Key ||
                key == GuaranteeIssuedObjectKeys.t_S_PlanAttractPrGrnt_Key)
            {
                UltraGridHelper.SetLikelyEditButtonColumnsStyle(
                    uge.DisplayLayout.Bands[0].Columns["EXCHANGERATE"], 0);
            }
        }

        protected override void AfterDetailRowInsert(object sender, UltraGridRow row)
        {
            if (row.Cells.Exists("RefOKV"))
            {
                UltraGridRow masterRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
                int refOKV = Convert.ToInt32(masterRow.Cells["RefOKV"].Value);
                row.Cells["RefOKV"].Value = refOKV;
            }
            Server.Guarantees.Guarantee guarantee = new Server.Guarantees.Guarantee(GetActiveDataRow());

            if (row.Cells.Exists("RefKIF"))
            {
                row.Cells["RefKIF"].Value =
                    Workplace.ActiveScheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(activeDetailObject.ObjectKey,
                        SchemeObjectsKeys.d_KIF_Plan_Key, guarantee.Regress, CurrentDataSourceID);
            }
            if (row.Cells.Exists("RefEKR"))
            {
                row.Cells["RefEKR"].Value =
                    Workplace.ActiveScheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(activeDetailObject.ObjectKey,
                    SchemeObjectsKeys.d_EKR_PlanOutcomes_Key,
                    guarantee.Regress, CurrentDataSourceID);
            }
            if (row.Cells.Exists("RefR"))
            {
                row.Cells["RefR"].Value =
                    Workplace.ActiveScheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(activeDetailObject.ObjectKey,
                    SchemeObjectsKeys.d_R_Plan_Key,
                    guarantee.Regress, CurrentDataSourceID);
            }

            string detailKey = activeDetailObject.ObjectKey;
            if (detailFilterParams[detailKey] != null)
            {
                foreach (DbParameterDescriptor param in detailFilterParams[detailKey])
                {
                    row.Cells[param.ParameterName].Value = param.Value;
                }
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
                        if (row.IsNull("RefOKV"))
                        {
                            errorString = string.Format("Поле 'Валюта' записи с ID = {0} не заполнено",
                                row["ID"]);
                            return false;
                        }
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

        protected override void InitializeDetailRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("RefOKV"))
            {
                UltraGridRow masterRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
                int refOKV = e.Row.Cells["RefOKV"].Value is DBNull
                                 ? Convert.ToInt32(masterRow.Cells["RefOKV"].Value)
                                 : Convert.ToInt32(e.Row.Cells["RefOKV"].Value);
            if (e.Row.Cells.Exists("Sum"))
                e.Row.Cells["Sum"].Activation = refOKV != -1 ?
                     Activation.ActivateOnly :
                     e.Row.Activation;
            if (e.Row.Cells.Exists("CurrencySum"))
                    e.Row.Cells["CurrencySum"].Activation = refOKV == -1 ?
                        Activation.ActivateOnly :
                        e.Row.Activation;
            if (e.Row.Cells.Exists("ExchangeRate"))
                e.Row.Cells["ExchangeRate"].Activation = refOKV == -1 ?
                    Activation.Disabled :
                    e.Row.Activation;
            }
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RegDate"].Value = DateTime.Today;

            base.SetTaskId(ref row);
        }

        #region смена активного варианта

        internal override bool HideDetail(string detailKey)
        {
            if (detailKey == GuaranteeIssuedObjectKeys.t_S_PlanAttractGrnt_Key ||
                detailKey == GuaranteeIssuedObjectKeys.t_S_PlanAttractPrGrnt_Key ||
                detailKey == GuaranteeIssuedObjectKeys.t_S_PlanDebtPrGrnt_Key ||
                detailKey == GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key ||
                detailKey == GuaranteeIssuedObjectKeys.t_S_JournalPercentGrnt_Key ||
                detailKey == GuaranteeIssuedObjectKeys.t_S_PrincipalContrGrnt_Key)
                return false;
            return true;
        }

        public override string GetCurrentPresentation(IEntity clsObject)
        {
            if (RefVariant > 0 && clsObject.ObjectKey == SchemeObjectsKeys.f_S_Guarantissued_Key)
                PresentationKey = "9dd856fe-9d83-4a7a-ba32-7e823b832b58";
            else
                PresentationKey = GetPresentationKey();
            return PresentationKey;
        }

        #endregion

        internal override void HideCurrencyColumns(int currency)
        {
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("CurrencySum"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["CurrencySum"].Hidden = currency == -1;
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("ExchangeRate"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["ExchangeRate"].Hidden = currency == -1;
            if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("IsPrognozExchRate"))
                ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["IsPrognozExchRate"].Hidden = currency == -1;
        }

        #region работа с несколькими планами обслуживания

        private Dictionary<string, int> activeCalculationIndexes = new Dictionary<string, int>();

        private Dictionary<string, string> detailFilters = new Dictionary<string, string>();

        private Dictionary<string, DbParameterDescriptor[]> detailFilterParams = new Dictionary<string, DbParameterDescriptor[]>();

        protected override void DetailGridSetup(UltraGridEx ugeDetail)
        {
            base.DetailGridSetup(ugeDetail);

            ugeDetail.utmMain.ToolValueChanged += new ToolEventHandler(utmMain_ToolValueChanged);
        }

        public override void AddNewPlanCalculation()
        {
            string comment = string.Empty; 
            DateTime formDate = DateTime.Now;
            if (frmNewCalculate.GetCaclculationParams(ref comment, ref formDate))
            {
                var newPercent = new Percents(formDate, comment);
                UltraGridEx gridEx = GetDetailGridEx(GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key);
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

        public override void RefreshDetail(string detailKey)
        {
            if (detailKey == GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key)
            {
                UltraTab tab = GetDetailTab(detailKey);
                tab.Selected = true;
                base.RefreshDetail(detailKey);
                if (ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns.Exists("Payment"))
                    ActiveDetailGrid.ugData.DisplayLayout.Bands[0].Columns["Payment"].SortIndicator = SortIndicator.Ascending;
            }
            else
                base.RefreshDetail(detailKey);
        }

        protected override void utcDetails_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            base.utcDetails_SelectedTabChanged(sender, e);

            if (e.Tab == null)
                return;
            string tabKey = e.Tab.Key;

            if (!activeCalculationIndexes.ContainsKey(tabKey))
            {
                activeCalculationIndexes.Add(tabKey, -1);
                detailFilters.Add(tabKey, string.Empty);
                detailFilterParams.Add(tabKey, null);
            }

            switch (tabKey)
            {
                case GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key:
                    UltraToolbar tb = activeDetailGrid.utmMain.Toolbars[2];
                    ComboBoxTool comboBoxTool = tb.Tools["CalculationResults"] as ComboBoxTool;
                    comboBoxTool.SelectedIndex = activeCalculationIndexes[tabKey];
                    comboBoxTool.ValueList.SelectedIndex = activeCalculationIndexes[tabKey];
                    break;
            }
        }

        public override void AfterSetDetailVisible()
        {
            UltraTab tab = vo.utcDetails.Tabs[GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key];
            UltraGridEx grid = tab.TabPage.Controls[0] as UltraGridEx;
            if (grid != null)
            {
                UltraToolbar tb = grid.utmMain.Toolbars[2];
                var newCalculate = CommandService.AttachToolbarTool<ButtonTool>(new AddNewPercentCalculation(), tb);
                newCalculate.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Document_Add_icon;
                //newCalculate.InstanceProps.IsFirstInGroup = true;

                ComboBoxTool cb = new ComboBoxTool("CalculationResults");
                cb.DropDownStyle = DropDownStyle.DropDownList;
                cb.SharedProps.Width = 200;
                grid.utmMain.Tools.AddRange(new ToolBase[] { cb });
                tb.Tools.AddTool("CalculationResults");
            }
        }

        void utmMain_ToolValueChanged(object sender, ToolEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "CalculationResults":
                    ComboBoxTool comboBoxTool = e.Tool as ComboBoxTool;
                    if (comboBoxTool.SelectedIndex != -1)
                    {
                        var percents = (Percents)comboBoxTool.ValueList.ValueListItems[comboBoxTool.SelectedIndex].DataValue;
                        if (string.IsNullOrEmpty(percents.comment))
                        {
                            detailFilters[GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key] = " and EstimtDate is null and CalcComment is null";
                            detailFilterParams[GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key] = null;
                            activeCalculationIndexes[GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key] = comboBoxTool.SelectedIndex;
                        }
                        else
                        {
                            detailFilters[GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key] = " and EstimtDate = ? and CalcComment = ?";
                            DbParameterDescriptor[] filterParams = new DbParameterDescriptor[2];
                            filterParams[0] = new DbParameterDescriptor("EstimtDate", percents.formDate);
                            filterParams[1] = new DbParameterDescriptor("CalcComment", percents.comment);
                            detailFilterParams[GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key] = filterParams;
                            activeCalculationIndexes[GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key] = comboBoxTool.SelectedIndex;
                        }
                    }
                    IsRefreshDetailCalculationList = false;
                    RefreshDetail();
                    IsRefreshDetailCalculationList = true;
                    break;
            }
        }

        public DateTime FormDate
        {
            get;
            set;
        }

        public string FormComment
        {
            get;
            set;
        }

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
            Dictionary<string, Percents> percents = GetPercentsCalculations(activeMasterRow);
            foreach (KeyValuePair<string, Percents> percent in percents)
            {
                comboBoxTool.ValueList.ValueListItems.Add(percent.Value, percent.Key);
            }
            int index =
                comboBoxTool.ValueList.FindString(string.Format(calcListDataFormat, FormComment,
                                                                FormDate.ToShortDateString()));
            int activeCalculationIndex = index != -1 ? index : comboBoxTool.ValueList.ValueListItems.Count - 1;
            comboBoxTool.SelectedIndex = activeCalculationIndex;
        }

        private Dictionary<string, Percents> GetPercentsCalculations(DataRow activeParentRow)
        {
            Dictionary<string, Percents> percentsCalculations = new Dictionary<string, Percents>();

            IEntity detail = Workplace.ActiveScheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt = (DataTable)db.ExecQuery(
                    string.Format("select distinct EstimtDate, CalcComment from {0} where RefGrnt = ? order by EstimtDate", detail.FullDBName),
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", activeParentRow["ID"]));

                DataRow[] noParamsRow = dt.Select("EstimtDate is null");
                if (noParamsRow != null && noParamsRow.Length > 0)
                    noParamsRow[0]["EstimtDate"] = DateTime.MinValue;

                foreach (DataRow row in dt.Select(string.Empty, "EstimtDate Asc"))
                {
                    if (row.IsNull("EstimtDate") || row.IsNull("CalcComment"))
                    {
                        percentsCalculations.Add("Без параметров", new Percents());
                        continue;
                    }

                    DateTime formDate = Convert.ToDateTime(row["EstimtDate"]);
                    string comment = row["CalcComment"].ToString();
                    Percents percents = new Percents(formDate, comment);
                    percentsCalculations.Add(string.Format(calcListDataFormat, comment, formDate.ToShortDateString()), percents);
                }
                return percentsCalculations;
            }
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

        /// <summary>
        /// получение упдатера для получения данных
        /// </summary>
        protected override IDataUpdater GetDetailUpdater(IEntity activeDetailObject, object masterValue)
        {
            string detailKey = activeDetailObject.ObjectKey;
            string detailFilter = detailFilters.ContainsKey(detailKey) ? detailFilters[detailKey] : string.Empty;
            DbParameterDescriptor[] filterParams = detailFilterParams.ContainsKey(detailKey) ? detailFilterParams[detailKey] : null;
            if (Convert.ToInt64(masterValue) >= 0)
                return string.IsNullOrEmpty(detailFilter) ?
                    activeDetailObject.GetDataUpdater(GetQueryFilter(masterValue), null, null) :
                    activeDetailObject.GetDataUpdater(GetQueryFilter(masterValue) + detailFilter, null, filterParams);
            return activeDetailObject.GetDataUpdater("1 = 2", null, null);
        }

        protected override void ugeDetail_OnClearCurrentTable(object sender)
        {
            string deleteFilter = string.Format("where {0}", GetQueryFilter(GetActiveDataRow()["ID"]));
            string detailKey = activeDetailObject.ObjectKey;

            if (detailFilters.ContainsKey(detailKey))
                deleteFilter = string.Concat(deleteFilter, detailFilters[detailKey]);

            if (MessageBox.Show("Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                try
                {
                    using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                    {
                        db.ExecQuery(string.Format("delete from {0} {1}", activeDetailObject.FullDBName, deleteFilter),
                                     QueryResultTypes.NonQuery, detailFilterParams[detailKey]);
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
            RefreshDetail(detailKey);
        }

        internal class Percents
        {
            internal Percents()
            {

            }

            internal Percents(DateTime formDate, string comment)
            {
                this.formDate = formDate;
                this.comment = comment;
            }

            internal DateTime formDate;

            internal string comment;
        }

        #endregion
    }
}
