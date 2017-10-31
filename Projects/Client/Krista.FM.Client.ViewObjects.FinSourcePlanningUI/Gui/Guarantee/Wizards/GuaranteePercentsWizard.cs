using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.Wizards
{
    public class GuaranteePercentsWizard : AcquittancePercentsWizard
    {
        private GuaranteeServer guaranteeServer;

        private PrincipalContract principalContract;

        public GuaranteePercentsWizard(PayPeriodicity payPeriodicity)
            : base(payPeriodicity)
        {
            dteFormDate.Value = DateTime.Today;
            //dteFormDate.Enabled = false;
            //uceVersionName.Enabled = false;
            cbSplitPercentPeriods.Checked = false;
            cbSplitPercentPeriods.Enabled = false;
        }

        internal override void InitializWizardData(PercentCalculationParams percentScheme)
        {
            ccStartDate.Value = principalContract.StartDate;
            ccEndDate.Value = principalContract.EndDate;
            cbPretermDischarge.Checked = principalContract.PretermDisharge;
            cbFirstDayPayment.Checked = true;
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
                    periodsCount = Utils.GetPeriodCount(principalContract.StartDate, principalContract.EndDate,
                        (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                        (cePayPeriodicity.SelectedItem.DataValue).ToString()), Convert.ToInt32(uceEndPeriodDay.Value), dataSet.Tables[0], true);
                    break;
            }
            if (periodsCount > 0)
                lPeriodsCount.Text = string.Format("Количество периодов - {0}", periodsCount);
        }

        internal override void InitializWizardData()
        {
            GuaranteeUI content = (GuaranteeUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeMasterRow = content.GetActiveDataRow();
            DataRow principalContractRow = GuaranteeServer.GetPrincipalContract(activeMasterRow);
            principalContract = new PrincipalContract(principalContractRow, new Server.Guarantees.Guarantee(activeMasterRow));
            guaranteeServer = GuaranteeServer.GetGuaranteeServer(principalContract.RefOkv, FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);

            ccStartDate.Value = principalContract.StartDate;
            ccEndDate.Value = principalContract.EndDate;
            cbPretermDischarge.Checked = principalContract.PretermDisharge;
            cbFirstDayPayment.Checked = true;//principalContract.Guarantee.;
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
                    periodsCount = Utils.GetPeriodCount(principalContract.StartDate, principalContract.EndDate,
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
                guaranteeServer.FillServicePlanTable(principalContract, FinSourcePlanningNavigation.BaseYear, calculationParams);
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
    }
}
