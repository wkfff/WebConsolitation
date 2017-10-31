using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.Wizards
{
    class GuaranteePenaltyWizard : CalculatePenaltiesWizard
    {
        protected GuaranteeServer guaranteeServer;

        protected PrincipalContract principalContract;

        protected GuaranteePenaltyWizard()
            : base()
        {
            
        }

        internal override void LoadData()
        {
            DateTime startPenaltyPeriod = principalContract.EndDate;
            DateTime endPenaltyPeriod = startPenaltyPeriod.AddMonths(1);

            uteDaysCount.Text = Convert.ToString((endPenaltyPeriod - startPenaltyPeriod).Days + 1);

            DataTable dtJournalPercent = guaranteeServer.GetCreditPercents(principalContract.Guarantee.ID);

            if (dtJournalPercent.Rows.Count == 0)
            {
                Close();
                throw new FinSourcePlanningException("Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"");
            }

            // получаем данные по деталям
            DataTable dtPlan = new DataTable();
            DataTable dtFact = new DataTable();
            DataTable dtPenalties = new DataTable();
            Decimal currentPercent = 0;

            GetDetailsData(dtJournalPercent, ref dtPlan, ref dtFact, ref dtPenalties, ref currentPercent);

            // если поле валюты есть и валюта отличная от рубля
            refOKV = principalContract.Guarantee.RefOKV;
            if (refOKV != -1)
            {
                if (principalContract.ExchangeRate != 0)
                {
                    uceCurrencyRate.Items.Add(principalContract.ExchangeRate.ToString());
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

        protected virtual void GetDetailsData(DataTable dtJournalPercent, ref DataTable dtPlan, ref DataTable dtFact,
            ref DataTable dtPenalties, ref Decimal currentPercent)
        {
        }

        /// <summary>
        /// расчитываем пени по полученным данным
        /// </summary>
        /// <returns></returns>
        protected override string CalculatePenalty()
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

                        currencyPenalty += Convert.ToDecimal(uneOverdueSum.Value) *
                            Convert.ToDecimal(unePercent.Value) * daysCount / 100;

                        startPeriod = DateTime.MinValue.AddYears(currentYear);
                        currentYear++;
                    }
                    // переводим в рубли
                    penalty = currencyPenalty * exchangeRate;
                }
                else
                {
                    for (int i = 0; i <= endPeriod.Year - startPeriod.Year + 1; i++)
                    {
                        DateTime endOfYear = DateTime.MinValue.AddYears(startPeriod.Year - 1).AddMonths(11).AddDays(30);
                        daysCount = endOfYear > endPeriod
                                        ? (endPeriod - startPeriod).Days + 1
                                        : (endOfYear - startPeriod).Days + 1;

                        penalty += Convert.ToDecimal(uneOverdueSum.Value) * Convert.ToDecimal(unePercent.Value) *
                            daysCount / 100;

                        startPeriod = DateTime.MinValue.AddYears(currentYear);
                        currentYear++;
                    }
                }

                DateTime endOfMonth = new DateTime(endPeriod.Year, endPeriod.Month,
                    DateTime.DaysInMonth(endPeriod.Year, endPeriod.Month));

                FillPenalty(endOfMonth, penalty, currencyPenalty, exchangeRate);
                /*
                finSourcePlanningServer.AddPenalty(Convert.ToInt32(masterRow.Cells["ID"].Value),
                    _forDebtPenalty, endOfMonth, baseYear, currentPenaltyPayment,
                    penalty, currencyPenalty, Convert.ToInt32(uteDaysCount.Text),
                    Convert.ToDecimal(unePercent.Value), penaltyRate,
                    exchangeRate, Convert.ToInt32(masterRow.Cells["RefOKV"].Value));
                */
                return "Пени успешно начислены";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        protected virtual void FillPenalty(DateTime endOfMonth, decimal penalty, decimal currencyPenalty, decimal exchangeRate)
        {
            
        }
    }
}
