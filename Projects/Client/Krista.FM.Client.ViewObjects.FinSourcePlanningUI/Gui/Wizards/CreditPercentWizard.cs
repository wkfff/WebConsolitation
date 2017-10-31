using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Wizards
{
    public class CreditPercentWizard : AcquittancePercentsWizard
    {

        private FinSourcePlanningServer finSourcePlanningServer;

        private Credit credit;

        public CreditPercentWizard(PayPeriodicity payPeriodicity)
            : base(payPeriodicity)
        {
            string oktmoConst = WorkplaceSingleton.Workplace.ActiveScheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Replace(" ", string.Empty);
            cbPaymentDate.SelectedIndex = oktmoConst == "19000000" && credit.CreditsType == CreditsTypes.OrganizationIncoming ? 26 : 31;
            cbSplitPercentPeriods.Checked = oktmoConst == "46000000";
            uceEndPeriodDay.SelectedIndex = 31;
            dteFormDate.ValueChanged += new EventHandler(dteFormDate_ValueChanged);
            if (credit.CreditsType == CreditsTypes.BudgetIncoming || credit.CreditsType == CreditsTypes.OrganizationIncoming)
            {
                GetMainDebtPlans();
                FindMainDebtVersion(DateTime.Today);
            }
        }

        void dteFormDate_ValueChanged(object sender, EventArgs e)
        {
            if (dteFormDate.Value != null)
            {
                DateTime formDate = Convert.ToDateTime(dteFormDate.Value);
                FindMainDebtVersion(formDate);
            }
        }

        private void FindMainDebtVersion(DateTime date)
        {
            var version = finSourcePlanningServer.GetMainDebtPlanVersion(credit.ID, date);
            if (version != null)
            {
                var item = MainDebtVersions.ValueList.FindByDataValue(version);
                if (item != null)
                    MainDebtVersions.SelectedItem = item;
                else
                {
                    if (MainDebtVersions.ValueList.ValueListItems.Count > 0)
                        MainDebtVersions.SelectedIndex = 0;
                    else
                        MainDebtVersions.SelectedIndex = -1;
                }
            }
            else
            {
                if (MainDebtVersions.ValueList.ValueListItems.Count > 0)
                    MainDebtVersions.SelectedIndex = 0;
                else
                    MainDebtVersions.SelectedIndex = -1;
            }
        }

        internal override void InitializWizardData(PercentCalculationParams percentScheme)
        {
            ccStartDate.Value = credit.StartDate;
            ccEndDate.Value = credit.EndDate;
            cbPretermDischarge.Checked = credit.PretermDischarge;
            cbFirstDayPayment.Checked = credit.ChargeFirstDay;
            cbPayDayCorrection.SelectedIndex = (int)percentScheme.PaymentDayCorrection;
            if (percentScheme.PaymentDayCorrection == DayCorrection.LastDay)
                cbPayDayCorrection.SelectedIndex = 2;
            uceEndPeriodDay.SelectedIndex = percentScheme.EndPeriodDay - 1;
            cbPaymentDate.SelectedIndex = percentScheme.PaymentDay - 1;
            cbEndDayShift.Checked = percentScheme.EndPeriodDayShift;
            uceRoundResultParam.SelectedIndex = (int)percentScheme.RestRound;

            int periodsCount = 0;
            switch (payPeriodicity)
            {
                case PayPeriodicity.Other:
                    lPeriodsCount.Text = string.Empty;
                    cePayPeriodicity.SelectedIndex = GetIndex(PayPeriodicity.Other);
                    break;
                case PayPeriodicity.Single:
                    periodsCount = 1;
                    cePayPeriodicity.SelectedIndex = GetIndex(PayPeriodicity.Single);
                    break;
                default:
                    cePayPeriodicity.SelectedIndex = (int)payPeriodicity - 1;
                    periodsCount = Utils.GetPeriodCount(credit.StartDate, credit.EndDate,
                        (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                        (cePayPeriodicity.SelectedItem.DataValue).ToString()), Convert.ToInt32(uceEndPeriodDay.Value), dataSet.Tables[0], true);
                    break;
            }
            if (periodsCount > 0)
                lPeriodsCount.Text = string.Format("Количество периодов - {0}", periodsCount);
            //GetMainDebtPlans();
            //FindMainDebtVersion(DateTime.Today);
        }

        internal override void InitializWizardData()
        {
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
            finSourcePlanningServer = content.GetFinSourcePlanningServer();
            credit = finSourcePlanningServer.GetCredit(content.GetActiveDataRow());
            if (credit.CreditsType == CreditsTypes.BudgetOutcoming || credit.CreditsType == CreditsTypes.OrganizationOutcoming || credit.CreditsType == CreditsTypes.Unknown)
            {
                dteFormDate.Value = DateTime.Today;
                dteFormDate.Enabled = false;
                uceVersionName.Enabled = false;
                MainDebtVersions.Visible = false;
                ultraLabel2.Visible = false;
            }
            ccStartDate.Value = credit.StartDate;
            ccEndDate.Value = credit.EndDate;
            cbPretermDischarge.Checked = credit.PretermDischarge;
            cbFirstDayPayment.Checked = credit.ChargeFirstDay;
            cbPayDayCorrection.SelectedIndex = 0;
            uceEndPeriodDay.SelectedIndex = 31;
            cbPaymentDate.SelectedIndex = 31;
            cePercentSchemes.SelectedIndex = 0;
            uceRoundResultParam.SelectedIndex = 0;
            int periodsCount = 0;
            switch (payPeriodicity)
            {
                case PayPeriodicity.Other:
                    lPeriodsCount.Text = string.Empty;
                    cePayPeriodicity.SelectedIndex = GetIndex(PayPeriodicity.Other);
                    break;
                case PayPeriodicity.Single:
                    periodsCount = 1;
                    cePayPeriodicity.SelectedIndex = GetIndex(PayPeriodicity.Single);
                    break;
                default:
                    cePayPeriodicity.SelectedIndex = (int)payPeriodicity - 1;
                    periodsCount = Utils.GetPeriodCount(credit.StartDate, credit.EndDate,
                        (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                        (cePayPeriodicity.SelectedItem.DataValue).ToString()), Convert.ToInt32(uceEndPeriodDay.Value), dataSet.Tables[0], true);
                    break;
            }
            if (periodsCount > 0)
                lPeriodsCount.Text = string.Format("Количество периодов - {0}", periodsCount);
        }

        internal override string CalculateAcquittanceMainPlan()
        {
            try
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Формирование плана погашения";
                PercentCalculationParams calculationParams = new PercentCalculationParams();
                calculationParams.StartDate = (DateTime)ccStartDate.Value;
                calculationParams.EndDate = (DateTime)ccEndDate.Value;
                calculationParams.PretermDischarge = cbPretermDischarge.Checked;
                calculationParams.EndPeriodDayShift = cbEndDayShift.Checked;
                calculationParams.EndPeriodDay = Convert.ToInt32(uceEndPeriodDay.Value);
                calculationParams.FirstDayPayment = cbFirstDayPayment.Checked;
                calculationParams.PaymentDay = Convert.ToInt32(cbPaymentDate.Value);
                calculationParams.FormDate = (DateTime)dteFormDate.Value;
                calculationParams.CalculationComment = uceVersionName.Text;
                calculationParams.UseAllPercents = cbUseAllPercents.Checked;
                calculationParams.SplitPercentPeriods = cbSplitPercentPeriods.Checked;
                if (MainDebtVersions.SelectedIndex != -1)
                    calculationParams.MainDebtVersion = MainDebtVersions.SelectedItem.DataValue as VersionParams;
                FormDate = calculationParams.FormDate;
                FormComment = calculationParams.CalculationComment;
                switch (cbPayDayCorrection.SelectedIndex)
                {
                    case 0:
                        calculationParams.PaymentDayCorrection = DayCorrection.NoCorrection;
                        break;
                    case 1:
                        calculationParams.PaymentDayCorrection = DayCorrection.NextDay;
                        break;
                    case 2:
                        calculationParams.PaymentDayCorrection = DayCorrection.LastDay;
                        break;
                }
                calculationParams.PaymentsPeriodicity = (PayPeriodicity)cePayPeriodicity.Value;
                calculationParams.RestRound = (PercentRestRound)uceRoundResultParam.SelectedIndex;
                finSourcePlanningServer.CalcDebtServicePlan(credit, FinSourcePlanningNavigation.BaseYear,
                                                            calculationParams, true);
                return "План обслуживания долга успешно заполнен.";
            }
            // TODO: Сделать обработку ошибок
            catch (FinSourcePlanningException ex)
            {
                return ex.Message;
            }
            finally
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
            }
        }

        private void GetMainDebtPlans()
        {
            MainDebtVersions.Items.Clear();
            DataTable dt = finSourcePlanningServer.GetMainDebtPlans(credit.ID);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row.IsNull("EstimtDate") || row.IsNull("CalcComment"))
                    {
                        var versionParams = new VersionParams("Без параметров");
                        MainDebtVersions.Items.Add(versionParams, versionParams.ToString());
                    }
                    else
                    {
                        var versionParams = new VersionParams(Convert.ToDateTime(row["EstimtDate"]), row["CalcComment"].ToString());
                        MainDebtVersions.Items.Add(versionParams, versionParams.ToString());
                    }
                }
            }
        }
    }
}
