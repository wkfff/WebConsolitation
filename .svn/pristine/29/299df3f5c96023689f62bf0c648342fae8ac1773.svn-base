using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees
{
    public partial class GuaranteeServer
    {
        public void FillServicePlanTable(PrincipalContract principalContract, int baseYear, PercentCalculationParams calculationParams)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key);
            string refMasterFieldName = entity.Associations[GuaranteeIssuedObjectKeys.a_S_PlanServicePrGrnt_Key].RoleDataAttribute.Name;

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string percentQuery = "select CreditPercent, ChargeDate from t_S_JournalPercentGrnt where RefGrnt = {0}";
                PercentTable = (DataTable)db.ExecQuery(string.Format(percentQuery, principalContract.Guarantee.ID), QueryResultTypes.DataTable);
                if (PercentTable.Rows.Count == 0)
                    throw new FinSourcePlanningException(
                    "Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"");

                db.ExecQuery(
                    String.Format("delete from {0} where {1} = {2} and EstimtDate = ? and CalcComment = ?", entity.FullDBName, refMasterFieldName,
                    principalContract.Guarantee.ID), QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("p1", calculationParams.FormDate),
                    new DbParameterDescriptor("p2", calculationParams.CalculationComment));
            }

            using (IDataUpdater du = entity.GetDataUpdater("0 = 1", null))
            {
                DataTable dtMainPlan = new DataTable();
                du.Fill(ref dtMainPlan);
                percentOddsList = new Dictionary<int, decimal>();
                Holidays holidays = new Holidays(scheme);
                FillServicePlanTable(principalContract, dtMainPlan, refMasterFieldName, entity, calculationParams, holidays);
                int refKif = scheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(entity.ObjectKey,
                        SchemeObjectsKeys.d_KIF_Plan_Key, principalContract.Guarantee.Regress, principalContract.Guarantee.SourceID);
                foreach (DataRow row in dtMainPlan.Rows)
                {
                    row["CalcComment"] = calculationParams.CalculationComment;
                    row["EstimtDate"] = calculationParams.FormDate;
                    row["CalcDate"] = DateTime.Today;
                    row["RefKIF"] = refKif;
                }
                du.Update(ref dtMainPlan);
            }
        }

        protected virtual void FillServicePlanTable(PrincipalContract principalContract, DataTable dtData, string refMasterFieldName,
            IEntity entity, PercentCalculationParams calculationParams, Holidays holidays)
        {
            int increment = -Utils.GetSubtractValue(calculationParams.PaymentsPeriodicity);

            if (calculationParams.PaymentsPeriodicity == PayPeriodicity.Other)
            {
                calculationParams.StartDate = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[0]["StartDate"]);
                calculationParams.EndDate = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[calculationParams.CustomPeriods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = calculationParams.FirstDayPayment ? calculationParams.StartDate : calculationParams.StartDate.AddDays(1);
            DateTime endPeriod = DateTime.MinValue;
            DateTime paymentDay = DateTime.MinValue;
            DateTime correctDate = startPeriod;

            decimal percentRate = 0;
            int i = dtData.Rows.Count;
            // получаем последний день месяца
            if (calculationParams.EndPeriodDay == 0)
                calculationParams.EndPeriodDay = DateTime.DaysInMonth(calculationParams.StartDate.Year, calculationParams.StartDate.Month);

            // если конец периода не совпадает с 
            if (calculationParams.PaymentsPeriodicity != PayPeriodicity.Single && calculationParams.EndPeriodDay >= 0 &&
                calculationParams.PaymentsPeriodicity != PayPeriodicity.Other)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, 1);
                percentRate = GetPercentPeriods(startPeriod, endPeriod);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                decimal percentPay = GetPercentPay(startPeriod, endPeriod, calculationParams.PaymentsPeriodicity, principalContract,
                    percentRate, calculationParams.RestRound);

                if (percentPay != 0)
                {
                    DataRow servPayRow = dtData.NewRow();
                    servPayRow.BeginEdit();
                    servPayRow["ID"] = entity.GetGeneratorNextValue;
                    servPayRow["Payment"] = i + 1;
                    servPayRow["StartDate"] = startPeriod;
                    servPayRow["EndDate"] = endPeriod;
                    servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                    if (dtData.Columns.Contains("PaymentDate"))
                        servPayRow["PaymentDate"] = paymentDay;
                    servPayRow["CreditPercent"] = principalContract.PercentRate;
                    servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                    servPayRow["PercentRate"] = principalContract.PercentRate;
                    servPayRow["RefOKV"] = principalContract.RefOkv;
                    servPayRow[refMasterFieldName] = principalContract.Guarantee.ID;
                    servPayRow.EndEdit();
                    dtData.Rows.Add(servPayRow);
                    i++;
                }
                startPeriod = endPeriod.AddDays(1);
            }

            while (endPeriod < calculationParams.EndDate)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, i);

                percentRate = GetPercentPeriods(startPeriod, endPeriod);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                decimal percentPay = GetPercentPay(startPeriod, endPeriod, calculationParams.PaymentsPeriodicity, principalContract,
                    percentRate, calculationParams.RestRound);

                // добавление записи с параметрами
                if (percentPay != 0)
                {
                    DataRow servPayRow = dtData.NewRow();
                    servPayRow.BeginEdit();
                    servPayRow["ID"] = entity.GetGeneratorNextValue;
                    servPayRow["Payment"] = i + 1;
                    servPayRow["StartDate"] = startPeriod;
                    servPayRow["EndDate"] = endPeriod;
                    servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                    if (dtData.Columns.Contains("PaymentDate"))
                        servPayRow["PaymentDate"] = paymentDay;
                    servPayRow["CreditPercent"] = principalContract.PercentRate;
                    servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                    servPayRow["PercentRate"] = principalContract.PercentRate;
                    servPayRow["RefOKV"] = principalContract.RefOkv;
                    servPayRow[refMasterFieldName] = principalContract.Guarantee.ID;
                    servPayRow.EndEdit();
                    dtData.Rows.Add(servPayRow);
                    i++;
                }
                startPeriod = endPeriod.AddDays(1);
            }
            // если у нас в плане обслуживания нет ни одной записи, просто выходим
            if (dtData.Rows.Count == 0)
                return;
            // делаем дату выплаты в последнем периоде равной окончанию периода
            if (dtData.Columns.Contains("PaymentDate"))
                dtData.Rows[dtData.Rows.Count - 1]["PaymentDate"] =
                    dtData.Rows[dtData.Rows.Count - 1]["EndDate"];
        }

        protected void GetPeriodDate(ref DateTime startPeriod, ref DateTime endPeriod,
            ref DateTime correctDate, ref DateTime paymentDate,
            PercentCalculationParams calculationParams, int increment, Holidays holidays, int payment)
        {
            // расчитываем начало периода, конец периода и дату выплаты
            if (calculationParams.PaymentsPeriodicity == PayPeriodicity.Other)
            {
                startPeriod = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[payment]["StartDate"]);
                endPeriod = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[payment]["EndDate"]);
                //paymentDay = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[i]["PaymentDate"]);
            }
            else
            {
                if (calculationParams.PaymentsPeriodicity == PayPeriodicity.Single)
                {
                    endPeriod = calculationParams.EndDate;
                }
                else
                {
                    endPeriod = correctDate <= startPeriod ?
                        Utils.GetEndPeriod(startPeriod, calculationParams.PaymentsPeriodicity, calculationParams.EndPeriodDay, increment) :
                        Utils.GetEndPeriod(correctDate, calculationParams.PaymentsPeriodicity, calculationParams.EndPeriodDay, increment);
                }
                // получаем корректирующую дату для расчета периода выплаты
                correctDate = endPeriod.AddDays(1);

                if (calculationParams.EndPeriodDayShift && endPeriod < calculationParams.EndDate)
                    holidays.GetWorkDate(calculationParams.PaymentDayCorrection, ref endPeriod);

                if (endPeriod > calculationParams.EndDate)
                    endPeriod = calculationParams.EndDate;

                if (calculationParams.EndPeriodDayShift)
                {
                    paymentDate = endPeriod;
                    if (endPeriod == calculationParams.EndDate)
                        holidays.GetWorkDate(calculationParams.PaymentDayCorrection, ref paymentDate);
                }
                else
                {
                    paymentDate = Utils.GetEndPeriod(startPeriod, calculationParams.PaymentsPeriodicity,
                                                      calculationParams.PaymentDay, increment);
                    holidays.GetWorkDate(calculationParams.PaymentDayCorrection, ref paymentDate);
                }
            }
        }
    }

    public partial class GuaranteeCurrencyServer : GuaranteeServer
    {

        #region Расчет процентов

        protected override void FillServicePlanTable(PrincipalContract principalContract, DataTable dtData, string refMasterFieldName,
            IEntity entity, PercentCalculationParams calculationParams, Holidays holidays)
        {
            int increment = -Utils.GetSubtractValue(calculationParams.PaymentsPeriodicity);

            if (calculationParams.PaymentsPeriodicity == PayPeriodicity.Other)
            {
                calculationParams.StartDate = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[0]["StartDate"]);
                calculationParams.EndDate = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[calculationParams.CustomPeriods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = calculationParams.FirstDayPayment ? calculationParams.StartDate : calculationParams.StartDate.AddDays(1);
            DateTime endPeriod = DateTime.MinValue;
            DateTime paymentDay = DateTime.MinValue;
            DateTime correctDate = startPeriod;

            decimal percentRate = 0;
            int i = dtData.Rows.Count;
            // получаем последний день месяца
            if (calculationParams.EndPeriodDay == 0)
                calculationParams.EndPeriodDay = DateTime.DaysInMonth(calculationParams.StartDate.Year, calculationParams.StartDate.Month);

            // если конец периода не совпадает с 
            if (calculationParams.PaymentsPeriodicity != PayPeriodicity.Single && calculationParams.EndPeriodDay >= 0 &&
                calculationParams.PaymentsPeriodicity != PayPeriodicity.Other)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, 1);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                percentRate = GetPercentPeriods(startPeriod, endPeriod);
                decimal percentCurrencyPay = GetPercentPay(startPeriod, endPeriod, calculationParams.PaymentsPeriodicity, principalContract,
                    percentRate, calculationParams.RestRound);
                decimal percentPay = Math.Round(percentCurrencyPay, 2, MidpointRounding.AwayFromZero) * principalContract.Guarantee.ExchangeRate;

                if (percentCurrencyPay != 0)
                {
                    DataRow servPayRow = dtData.NewRow();
                    servPayRow.BeginEdit();
                    servPayRow["ID"] = entity.GetGeneratorNextValue;
                    servPayRow["Payment"] = i + 1;
                    servPayRow["StartDate"] = startPeriod;
                    servPayRow["EndDate"] = endPeriod;
                    servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                    if (dtData.Columns.Contains("PaymentDate"))
                        servPayRow["PaymentDate"] = paymentDay;
                    servPayRow["CreditPercent"] = principalContract.PercentRate;
                    servPayRow["CurrencySum"] = Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven);
                    servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                    servPayRow["PercentRate"] = principalContract.PercentRate;
                    servPayRow["IsPrognozExchRate"] = principalContract.IsPrognozExchRate;
                    servPayRow["RefOKV"] = principalContract.RefOkv;
                    servPayRow[refMasterFieldName] = principalContract.Guarantee.ID;
                    servPayRow.EndEdit();
                    dtData.Rows.Add(servPayRow);
                    i++;
                }
                startPeriod = endPeriod.AddDays(1);
            }

            while (endPeriod < calculationParams.EndDate)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, i);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                percentRate = GetPercentPeriods(startPeriod, endPeriod);
                decimal percentCurrencyPay = GetPercentPay(startPeriod, endPeriod, calculationParams.PaymentsPeriodicity, principalContract,
                    percentRate, calculationParams.RestRound);
                decimal percentPay = Math.Round(percentCurrencyPay, 2, MidpointRounding.AwayFromZero) * principalContract.Guarantee.ExchangeRate;
                // добавление записи с параметрами
                if (percentCurrencyPay != 0)
                {
                    DataRow servPayRow = dtData.NewRow();
                    servPayRow.BeginEdit();
                    servPayRow["ID"] = entity.GetGeneratorNextValue;
                    servPayRow["Payment"] = i + 1;
                    servPayRow["StartDate"] = startPeriod;
                    servPayRow["EndDate"] = endPeriod;
                    servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                    if (dtData.Columns.Contains("PaymentDate"))
                        servPayRow["PaymentDate"] = paymentDay;
                    servPayRow["CreditPercent"] = principalContract.PercentRate;
                    servPayRow["CurrencySum"] = Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven);
                    servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                    servPayRow["PercentRate"] = principalContract.PercentRate;
                    servPayRow["IsPrognozExchRate"] = principalContract.IsPrognozExchRate;
                    servPayRow["RefOKV"] = principalContract.RefOkv;
                    servPayRow[refMasterFieldName] = principalContract.Guarantee.ID;
                    servPayRow.EndEdit();
                    dtData.Rows.Add(servPayRow);
                    i++;
                }
                startPeriod = endPeriod.AddDays(1);
            }

            // если у нас в плане обслуживания нет ни одной записи, просто выходим
            if (dtData.Rows.Count == 0)
                return;
            // делаем дату выплаты в последнем периоде равной окончанию периода
            if (dtData.Columns.Contains("PaymentDate"))
                dtData.Rows[dtData.Rows.Count - 1]["PaymentDate"] =
                    dtData.Rows[dtData.Rows.Count - 1]["EndDate"];
        }

        #endregion

    }
}
