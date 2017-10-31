using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class CalculatePenaltiesWizard : Form
    {
        private DataRow masterRow;

        private int masterRowID;

        protected bool _forDebtPenalty;

        protected int currentPenaltyPayment;

        protected decimal penaltyRate = -1;

        protected int refOKV = -1;

        protected int baseYear;

        private FinSourcePlanningServer finSourcePlanningServer;

        public CalculatePenaltiesWizard()
        {
            InitializeComponent();

            udtEndPeriod.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            udtStartPeriod.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            uceCurrencyRate.BeforeDropDown += new CancelEventHandler(uceCurrencyRate_BeforeDropDown);
            Load += new EventHandler(CalculatePenaltiesWizard_Load);
        }

        public CalculatePenaltiesWizard(FinSourcePlanningServer finSourcePlanningServer, DataRow activeRow, int baseYear, bool forMainPlan)
            : this()
        {
            masterRow = activeRow;
            masterRowID = Convert.ToInt32(activeRow["ID"]);
            _forDebtPenalty = forMainPlan;
            this.baseYear = baseYear;
            this.finSourcePlanningServer = finSourcePlanningServer;
        }

        void CalculatePenaltiesWizard_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        void uceCurrencyRate_BeforeDropDown(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            // показываем справочник с курсом текущей валюты
            Decimal exchangeRate = GetExchangeRate(Convert.ToInt32(masterRow["RefOKV"]));
            if (exchangeRate != 0)
            {
                uceCurrencyRate.Items.Clear();
                uceCurrencyRate.Items.Add(exchangeRate);
                uceCurrencyRate.SelectedIndex = 0;
            }
        }

        private Decimal GetExchangeRate(int refOKV)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {

                int count = Convert.ToInt32(db.ExecQuery(
                    "select Count(*) from d_S_ExchangeRate where RefOKV = ?",
                    QueryResultTypes.Scalar,
                    new System.Data.OleDb.OleDbParameter("RefOKV", refOKV)));
                if (count > 0)
                {
                    // получаем нужный классификатор
                    IClassifier cls = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.Classifiers[SchemeObjectsKeys.d_S_RateValue];
                    // создаем объект просмотра классификаторов нужного типа
                    RateValueDataClsUI clsUI = new RateValueDataClsUI(cls, refOKV);
                    clsUI.Workplace = FinSourcePlanningNavigation.Instance.Workplace;
                    clsUI.RestoreDataSet = false;
                    clsUI.Initialize();
                    clsUI.InitModalCls(-1);

                    // создаем форму
                    frmModalTemplate modalClsForm = new frmModalTemplate();
                    modalClsForm.AttachCls(clsUI);
                    ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);

                    // ...загружаем данные
                    clsUI.RefreshAttachedData();
                    clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["RefOKV"].FilterConditions.
                        Add(FilterComparisionOperator.Equals, refOKV);
                    clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["DateFixing"].SortIndicator
                        = SortIndicator.Descending;

                    if (modalClsForm.ShowDialog((Form)FinSourcePlanningNavigation.Instance.Workplace) == DialogResult.OK)
                    {
                        int clsID = modalClsForm.AttachedCls.GetSelectedID();
                        // если ничего не выбрали - считаем что функция завершилась неудачно
                        if (clsID == -10)
                            return 0;

                        decimal valueRate = Convert.ToDecimal(db.ExecQuery(
                            "select ExchangeRate from d_S_ExchangeRate where ID = ?",
                            QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("clsID", clsID)));
                        return valueRate;
                    }
                }
            }
            return 0;
        }


        private void ParametersPage_ValueChanged(object sender, EventArgs e)
        {
            DateTime startDate = (DateTime)udtStartPeriod.Value;
            DateTime endDate = (DateTime)udtEndPeriod.Value;
            uteDaysCount.Text = Convert.ToString((endDate - startDate).Days + 1);

        }

        private void wizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardParametersPage)
            {
                wizardFinalPage.Description2 = CalculatePenalty();
                wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
            }
        }

        private void wizard_Back(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {

        }

        /// <summary>
        /// загружает необходимые для расчета данные
        /// </summary>
        internal virtual void LoadData()
        {
            DateTime startPenaltyPeriod = DateTime.Now;
            if (masterRow["RenewalDate"] is DBNull)
                startPenaltyPeriod = Convert.ToDateTime(masterRow["EndDate"]);
            else
                startPenaltyPeriod = Convert.ToDateTime(masterRow["RenewalDate"]);
            DateTime endPenaltyPeriod = startPenaltyPeriod.AddMonths(1);

            uteDaysCount.Text = Convert.ToString((endPenaltyPeriod - startPenaltyPeriod).Days + 1);
            // получаем данные по деталям
            DataTable dtPlan;
            DataTable dtFact;
            DataTable dtPenalties;
            Decimal currentPercent;

            DataTable dtJournalPercent = finSourcePlanningServer.GetCreditPercents(masterRowID);

            // в зависимости от типа пеней (по основному долгу или по процентам) получим данные по деталям
            if (_forDebtPenalty)
            {
                dtPlan = finSourcePlanningServer.GetDebtPlan(masterRowID);
                dtFact = finSourcePlanningServer.GetDebtFact(masterRowID);
                dtPenalties = finSourcePlanningServer.GetDebtPenaltyTable(masterRowID);

                currentPercent = dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyDebt"] is DBNull ?
                    0 :
                    Convert.ToDecimal(dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyDebt"]);
                if (!(masterRow["PenaltyPercentRate"] is DBNull))
                    penaltyRate = Convert.ToDecimal(masterRow["PenaltyDebtRate"]);
                RenameWizardForMainDebt();
            }
            else
            {
                dtPlan = finSourcePlanningServer.GetServicePlan(masterRowID);
                dtFact = finSourcePlanningServer.GetPercentFact(masterRowID);
                dtPenalties = finSourcePlanningServer.GetPercentPenaltyTable(masterRowID);
                currentPercent = dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyPercent"] is DBNull ?
                    -1 :
                    Convert.ToDecimal(dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyPercent"]);
                if (!(masterRow["PenaltyPercentRate"] is DBNull))
                    penaltyRate = Convert.ToDecimal(masterRow["PenaltyPercentRate"]);
                RenameWizardForPercent();
            }
                
            // если поле валюты есть и валюта отличная от рубля
            refOKV = Convert.ToInt32(masterRow["RefOKV"]);
            if (refOKV != -1)
            {
                uteCurrency.Text = masterRow["REFOKV"].ToString();
                if (!(masterRow["ExchangeRate"] is DBNull))
                {
                    uceCurrencyRate.Items.Add(Convert.ToInt32(masterRow["ExchangeRate"]).ToString());
                    uceCurrencyRate.SelectedIndex = 0;
                }
            }

            currentPenaltyPayment = GetPaymentNumber(dtPenalties);
            udtStartPeriod.Value = startPenaltyPeriod;
            udtEndPeriod.Value = endPenaltyPeriod;
            unePercent.Value = currentPercent;
            uneOverdueSum.Value = GetSum(dtPlan) - GetSum(dtFact) >= 0 ?
                GetSum(dtPlan) - GetSum(dtFact) : 0;
            // настройка видимости
            ultraLabel11.Visible = refOKV != -1;
            uteCurrency.Visible = refOKV != -1;
            ultraLabel10.Visible = refOKV != -1;
            uceCurrencyRate.Visible = refOKV != -1;
        }

        protected static int GetPaymentNumber(DataTable penalties)
        {
            if (penalties.Rows.Count == 0)
                return 1;
            DataRow[] rows = penalties.Select(string.Empty, "Payment ASC");
            return Convert.ToInt32(rows[rows.Length - 1]["Payment"]) + 1;
        }

        /// <summary>
        /// получение общей суммы по детали
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        protected Decimal GetSum(DataTable table)
        {
            Decimal sum = 0;
            if (refOKV == -1)
            {
                foreach (DataRow row in table.Rows)
                {
                    sum += Convert.ToDecimal(row["Sum"]);
                }
            }
            else
            {
                foreach (DataRow row in table.Rows)
                {
                    sum += Convert.ToDecimal(row["CurrencySum"]);
                }
            }
            return sum;
        }

        /// <summary>
        /// расчитываем пени по полученным данным
        /// </summary>
        /// <returns></returns>
        protected virtual string CalculatePenalty()
        {
            Decimal exchangeRate = 0;
            Decimal currencyPenalty = 0;
            Decimal penalty = 0;
            DateTime startPeriod = Convert.ToDateTime(udtStartPeriod.Value);
            DateTime endPeriod = Convert.ToDateTime(udtEndPeriod.Value);
            int currentYear = startPeriod.Year;
            int daysCount;
            try
            {
                // для валютного договора считаем все в валюте
                if (refOKV != -1)
                {
                    exchangeRate = Convert.ToDecimal(uceCurrencyRate.Items[0].DataValue);
                    for (int i = 0; i <= endPeriod.Year - startPeriod.Year + 1; i++)
                    {
                        DateTime endOfYear = DateTime.MinValue.AddYears(startPeriod.Year - 1).AddMonths(11).AddDays(30);
                        daysCount = endOfYear > endPeriod
                                        ? (endPeriod - startPeriod).Days + 1
                                        : (endOfYear - startPeriod).Days + 1;
                        if (Convert.ToInt32(masterRow["RefSTypeCredit"]) == 3 ||
                            Convert.ToInt32(masterRow["RefSTypeCredit"]) == 4)
                            currencyPenalty += Convert.ToDecimal(uneOverdueSum.Value)*
                                               Convert.ToDecimal(unePercent.Value)*
                                               daysCount/100;
                        else
                            currencyPenalty += Convert.ToDecimal(uneOverdueSum.Value) *
                                               Convert.ToDecimal(unePercent.Value) *
                                               daysCount / GetYearBase(currentYear) / 100;

                        startPeriod = DateTime.MinValue.AddYears(currentYear);
                        currentYear++;
                    }
                    // переводим в рубли
                    penalty = currencyPenalty*exchangeRate;
                }
                else
                {
                    for (int i = 0; i <= endPeriod.Year - startPeriod.Year + 1; i++)
                    {
                        DateTime endOfYear = DateTime.MinValue.AddYears(startPeriod.Year - 1).AddMonths(11).AddDays(30);
                        daysCount = endOfYear > endPeriod
                                        ? (endPeriod - startPeriod).Days + 1
                                        : (endOfYear - startPeriod).Days + 1;

                        if (Convert.ToInt32(masterRow["RefSTypeCredit"]) == 3 ||
                            Convert.ToInt32(masterRow["RefSTypeCredit"]) == 4)
                            penalty += Convert.ToDecimal(uneOverdueSum.Value)*Convert.ToDecimal(unePercent.Value)*
                                   daysCount/100;
                        else
                            penalty += Convert.ToDecimal(uneOverdueSum.Value) * Convert.ToDecimal(unePercent.Value) *
                                   daysCount / GetYearBase(currentYear) / 100;
                        startPeriod = DateTime.MinValue.AddYears(currentYear);
                        currentYear++;
                    }
                }

                DateTime endOfMonth = new DateTime(endPeriod.Year, endPeriod.Month,
                    DateTime.DaysInMonth(endPeriod.Year, endPeriod.Month));

                finSourcePlanningServer.AddPenalty(new Credit(masterRow),
                    _forDebtPenalty, endOfMonth, baseYear, currentPenaltyPayment,
                    penalty, currencyPenalty, Convert.ToInt32(uteDaysCount.Text),
                    Convert.ToDecimal(unePercent.Value), penaltyRate,
                    exchangeRate, Convert.ToInt32(masterRow["RefOKV"]));

                return "Пени успешно начислены";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        protected static int GetYearBase(int year)
        {
            return (year % 4) == 0 ? 366 : 365;
        }

        private void wizard_Cancel(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void wizard_Finish(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            Close();
        }

        private void wizard_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void AcquittanceMainPlanWizard_Load(object sender, EventArgs e)
        {
            //DefaultFormState.Load(this);
        }

        private void AcquittanceMainPlanWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DefaultFormState.Save(this);
        }

        private void ultraNumericEditor1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void RenameWizardForMainDebt()
        {
            wizardWelcomePage1.Description = "Мастер начисления пени по основному долгу";
            wizardWelcomePage1.Title = "Мастер начисления пени по основному долгу";
            Text = "Начисление пени по основному долгу";
        }

        private void RenameWizardForPercent()
        {
            wizardWelcomePage1.Description = "Мастер начисления пени по процентам";
            wizardWelcomePage1.Title = "Мастер начисления пени по процентам";
            Text = "Начисление пени по процентам";

        }
    }
}