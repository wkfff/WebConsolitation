using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital
{
    public class CapitalServer
    {
        protected static DateTime GetEndPeriod(DateTime startPeriod, int addMonthsCount, int day)
        {
            if (startPeriod.Day < day)
            {
                addMonthsCount--;
            }
            DateTime endPeriod = startPeriod.AddMonths(addMonthsCount);
            endPeriod = new DateTime(endPeriod.Year, endPeriod.Month, 1);
            int dayOfMonth = DateTime.DaysInMonth(endPeriod.Year, endPeriod.Month);
            if (dayOfMonth < day)
                return endPeriod.AddDays(dayOfMonth - 1);
            return endPeriod.AddDays(day - 1);
        }

        /// <summary>
        /// заполнение детали с траншами
        /// </summary>
        /// <param name="transhCount"></param>
        /// <param name="isEqual"></param>
        /// <param name="parentRow"></param>
        public void FillTransh(int transhCount, bool isEqual, DataRow parentRow)
        {
            // получаем классификатор, получаем пустую таблицу DataTable
            DataTable dtTransh = new DataTable();
            int refOKV = Convert.ToInt32(parentRow["RefOKV"]);

            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPTransh_Key);
            string refMasterFieldName = entity.Associations[CapitalObjectKeys.a_S_CPTransh_RefCap_Key].RoleDataAttribute.Name;

            using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
            {
                du.Fill(ref dtTransh);
                // добавляем записи в таблицу
                for (int i = 0; i <= transhCount - 1; i++)
                {
                    DataRow newRow = dtTransh.NewRow();
                    newRow.BeginEdit();
                    // заполняем данными
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    newRow["CodeTransh"] = i + 1;
                    int quantity = Convert.ToInt32(parentRow["Count"]) / transhCount;
                    if (isEqual)
                    {
                        newRow["Quantity"] = quantity;
                        if (refOKV == -1)
                        {
                            newRow["Sum"] = parentRow.IsNull("Nominal") ? 0 : Convert.ToInt32(parentRow["Nominal"]) * quantity;
                            newRow["CurrencySum"] = DBNull.Value;
                        }
                        else
                        {
                            newRow["CurrencySum"] = parentRow.IsNull("NominalCurrency") ? 0 : Convert.ToDecimal(parentRow["NominalCurrency"]) * quantity;
                            newRow["Sum"] = parentRow.IsNull("NominalCurrency") || parentRow.IsNull("ExchangeRate") ? 0 :
                                Convert.ToDecimal(newRow["CurrencySum"]) * Convert.ToDecimal(parentRow["ExchangeRate"]) * quantity;
                        }
                    }
                    else
                        newRow["Quantity"] = 0;
                    newRow["RefOKV"] = parentRow["RefOKV"];
                    newRow["ExchangeRate"] = parentRow["ExchangeRate"];
                    newRow["StartDate"] = parentRow["StartDate"];
                    newRow["EndDate"] = parentRow["EndDate"];
                    newRow["Discount"] = parentRow["Discount"];
                    newRow["DiscountCurrency"] = parentRow["DiscountCurrency"];
                    newRow[refMasterFieldName] = parentRow["ID"];
                    newRow.EndEdit();
                    dtTransh.Rows.Add(newRow);
                }
                du.Update(ref dtTransh);
            }
        }

        /// <summary>
        /// формирование плана размещения
        /// </summary>
        /// <param name="masterRow"></param>
        public void FillAllocationPlan(DataRow masterRow, int baseYear)
        {
            // проверяем заполненность детали "Транши"
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntity entity = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPPlanCapital_Key);
                string refMasterFieldName = entity.Associations[CapitalObjectKeys.a_S_CPPlanCapital_RefCap_Key].RoleDataAttribute.Name;
                int refOKV = Convert.ToInt32(masterRow["RefOKV"]);
                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 29, baseYear);
                int refKif = scheme.FinSourcePlanningFace.GetCapitalClassifierRef(CapitalObjectKeys.t_S_CPPlanCapital_Key,
                                                 SchemeObjectsKeys.d_KIF_Plan_Key, sourceID, refOKV);

                Stock stock = new Stock(masterRow);

                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
                {
                    DataTable dtAttraction = new DataTable();
                    du.Fill(ref dtAttraction);
                    // если нет, по мастер таблице
                    DataRow newRow = dtAttraction.NewRow();
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    newRow["Sum"] = stock.Sum;
                    //newRow["CurrencySum"] = stock.CurrencySum;
                    newRow["StartDate"] = stock.StartDate ?? (object)DBNull.Value;
                    newRow["Quantity"] = stock.Nominal == 0 ? 0 : stock.Sum / stock.Nominal;
                    newRow["Price"] = 100;
                    //if (stock.RefOkv != -1)
                    //    newRow["CurrencyPrice"] = 100*stock.ExchangeRate;
                    //newRow["ExchangeRate"] = stock.ExchangeRate;
                    newRow["IsPrognozExchRate"] = 0;
                    newRow["RefOKV"] = stock.RefOkv;
                    newRow["RefKIF"] = refKif;
                   
                    newRow[refMasterFieldName] = masterRow["ID"];
                    dtAttraction.Rows.Add(newRow);
                    du.Update(ref dtAttraction);
                }
            }
        }

        #region План выплаты номинальной стоимости

        /// <summary>
        /// план выплаты номинальной стоимости 
        /// </summary>
        internal static void FillNominalValueRepaymentPlan(Stock stock, DateTime startDate, DataTable dtPeriods, int sourceId, IScheme scheme)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                // данные из детали "факт размещения"
                DataTable dtFactCapital = Utils.GetDetailTable(db, stock.ID, CapitalObjectKeys.a_S_CPFactCapital_RefCap_Key);

                IEntity entity = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPPlanDebt_Key);
                string refMasterFieldName = entity.Associations[CapitalObjectKeys.a_S_CPPlanDebt_RefCap_Key].RoleDataAttribute.Name;
                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    if (dtPeriods.Rows.Count == 0)
                    {
                        if (stock.DischargeDate != null)
                        {
                            DataRow newRow = dt.NewRow();
                            newRow["Payment"] = dt.Rows.Count + 1;
                            newRow["EndDate"] = stock.DischargeDate;
                            newRow["Sum"] = stock.Sum;
                            newRow["CurrencySum"] = stock.CurrencySum;
                            newRow["PercentNom"] = 100;
                            bool isPrognozRate;
                            decimal exchRate = Utils.GetLastCurrencyExchange((DateTime) stock.DischargeDate,
                                                                             stock.RefOkv, out isPrognozRate);
                            newRow["ExchangeRate"] = exchRate == -1 ? (object)DBNull.Value : exchRate;
                            newRow["IsPrognozExchRate"] = isPrognozRate;
                            newRow["RefOkv"] = stock.RefOkv;
                            dt.Rows.Add(newRow);
                        }
                    }
                    else
                    {
                        foreach (DataRow periodRow in dtPeriods.Rows)
                        {
                            DataRow newRow = dt.NewRow();
                            newRow["Payment"] = dt.Rows.Count + 1;
                            newRow["EndDate"] = periodRow.IsNull("PayDate")
                                ? startDate.AddDays(Convert.ToInt32(periodRow["DaysCount"]))
                                : Convert.ToDateTime(periodRow["PayDate"]);
                            newRow["Sum"] = stock.Sum * Convert.ToInt32(periodRow["Percent"]) / 100;
                            newRow["CurrencySum"] = stock.CurrencySum * Convert.ToInt32(periodRow["Percent"]) / 100;
                            newRow["PercentNom"] = periodRow["Percent"];
                            bool isPrognozRate;
                            decimal exchRate = Utils.GetLastCurrencyExchange((DateTime)stock.DischargeDate,
                                                                             stock.RefOkv, out isPrognozRate);
                            newRow["ExchangeRate"] = exchRate == -1 ? (object)DBNull.Value : exchRate;
                            newRow["IsPrognozExchRate"] = isPrognozRate;
                            newRow["RefOkv"] = stock.RefOkv;
                            dt.Rows.Add(newRow);
                        }
                    }

                    int refKif = scheme.FinSourcePlanningFace.GetCapitalClassifierRef(CapitalObjectKeys.t_S_CPPlanDebt_Key, SchemeObjectsKeys.d_KIF_Plan_Key, sourceId, stock.RefOkv);

                    foreach (DataRow row in dt.Rows)
                    {
                        row["ID"] = entity.GetGeneratorNextValue;
                        row["RefKIF"] = refKif;
                        row[refMasterFieldName] = stock.ID;
                    }
                    
                    du.Update(ref dt);
                }
            }
        }

        private void FillNominalCostPlan(DataTable dtNominalCostPlan, DataRow masterRow, DataRow factCapitalRow,
            DateTime startDate, DateTime endDate, int refOkv, bool isPrognozExchRate,
            PayPeriodicity payPeriodicity, int dayCount, DataTable periods, decimal sum)
        {
            int increment = -Utils.GetSubtractValue(payPeriodicity);
            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
            }

            int payDay = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            int periodsCount = Utils.GetPeriodCount(startDate, endDate, payPeriodicity, payDay, periods, true);

            DateTime startPeriod = startDate;
            DateTime endPeriod = DateTime.MinValue;
            
            int paymentNum = 1;
            // добавим запись по первому периоду
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0)
            {
                if (payPeriodicity == PayPeriodicity.Other)
                    endPeriod = Convert.ToDateTime(periods.Rows[0]["EndDate"]);
                else
                    endPeriod = payPeriodicity == PayPeriodicity.Day ?
                        startPeriod.AddDays(dayCount) :
                        Utils.GetEndPeriod(startPeriod, payPeriodicity, payDay, increment);

                // добавляем запись, заполняем данными
                DataRow newRow = dtNominalCostPlan.NewRow();
                newRow.BeginEdit();
                newRow["Payment"] = paymentNum;
                newRow["EndDate"] = startPeriod;
                newRow["PercentNom"] = periodsCount/100;
                newRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                if (refOkv == 1 || refOkv == 643)
                    newRow["Sum"] = Math.Round(sum/periodsCount);
                else
                {
                    newRow["Sum"] = Math.Round(sum/periodsCount);
                    newRow["ExchangeRate"] = masterRow != null ?
                        (masterRow.IsNull("ExchangeRate") ? 0 : Convert.ToDecimal(masterRow["ExchangeRate"])) :
                        (factCapitalRow.IsNull("ExchangeRate") ? 0 : Convert.ToDecimal(factCapitalRow["ExchangeRate"]));
                    newRow["IsPrognozExchRate"] = isPrognozExchRate;
                }
                // если мы заполняем значения по детали "Итоги размещения" заполним поля, которые берем только оттуда
                if (factCapitalRow != null)
                {
                    newRow["NumDoc"] = factCapitalRow["NumDoc"];
                    newRow["Org"] = factCapitalRow["RefOrg"];
                    newRow["DateDoc"] = factCapitalRow["DateDoc"];
                    newRow["Quantity"] = factCapitalRow["Quantity"];
                }
                newRow["RefOKV"] = refOkv;
                newRow.EndEdit();
                dtNominalCostPlan.Rows.Add(newRow);
                paymentNum++;
                startPeriod = endPeriod.AddDays(1);
            }
            // добавляем записи до тех пор, пока не дойдем до конца периода расчета
            while (endPeriod < endDate)
            {
                if (payPeriodicity == PayPeriodicity.Other)
                {
                    startPeriod = Convert.ToDateTime(periods.Rows[paymentNum - 1]["StartDate"]);
                    endPeriod = Convert.ToDateTime(periods.Rows[paymentNum- 1]["EndDate"]);
                }
                else if (payPeriodicity == PayPeriodicity.Single)
                {
                    endPeriod = endDate;
                }
                else
                {
                    endPeriod = GetEndPeriod(startPeriod, increment, payDay);
                }

                if (endPeriod > endDate)
                {
                    endPeriod = endDate;
                }
                // добавляем запись, заполняем данными
                DataRow newRow = dtNominalCostPlan.NewRow();
                newRow.BeginEdit();
                newRow["Payment"] = paymentNum;
                newRow["EndDate"] = startPeriod;
                newRow["PercentNom"] = periodsCount / 100;
                newRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                if (refOkv == 1 || refOkv == 643)
                    newRow["Sum"] = Math.Round(sum / periodsCount);
                else
                {
                    newRow["Sum"] = Math.Round(sum / periodsCount);
                    newRow["ExchangeRate"] = masterRow != null ?
                        (masterRow.IsNull("ExchangeRate") ? 0 : Convert.ToDecimal(masterRow["ExchangeRate"])) :
                        (factCapitalRow.IsNull("ExchangeRate") ? 0 : Convert.ToDecimal(factCapitalRow["ExchangeRate"]));
                    newRow["IsPrognozExchRate"] = isPrognozExchRate;
                }
                if (factCapitalRow != null)
                {
                    newRow["NumDoc"] = factCapitalRow["NumDoc"];
                    newRow["Org"] = factCapitalRow["RefOrg"];
                    newRow["DateDoc"] = factCapitalRow["DateDoc"];
                    newRow["Quantity"] = factCapitalRow["Quantity"];
                }
                newRow["RefOKV"] = refOkv;
                newRow.EndEdit();
                dtNominalCostPlan.Rows.Add(newRow);
                paymentNum++;
                startPeriod = endPeriod.AddDays(1);
            }
        }

        private decimal GetNominalPlanSum(DataRow masterRow, DataRow factCapitalRow, int refOkv)
        {
            if (factCapitalRow == null)
            {
                return refOkv == -1 || refOkv == 643
                           ? Convert.ToDecimal(masterRow["Sum"])
                           : Convert.ToDecimal(masterRow["CurrencySum"]);
            }
            return refOkv == -1 || refOkv == 643
                ? Convert.ToDecimal(factCapitalRow["Sum"])
                : Convert.ToDecimal(factCapitalRow["CurrencySum"]);
        }

        #endregion

        /// <summary>
        /// план выплаты дохода
        /// </summary>
        public void FillPaymentIncomePlan(Stock stock, int sourceId, IScheme scheme)
        {
            IEntity t_S_CPJournalPercent = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPJournalPercent);
            IEntity t_S_CPPlanCapital = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPPlanCapital_Key);
            IEntity t_S_CPPlanService = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPPlanService);

            DataTable dtCPJournalPercent = new DataTable();
            DataTable dtCPPlanCapital = new DataTable();
            using (IDataUpdater du = t_S_CPJournalPercent.GetDataUpdater("RefCap = ?", null, new DbParameterDescriptor("p0", stock.ID)))
            {
                du.Fill(ref dtCPJournalPercent);
            }
            using (IDataUpdater du = t_S_CPPlanCapital.GetDataUpdater("RefCap = ?", null, new DbParameterDescriptor("p0", stock.ID)))
            {
                du.Fill(ref dtCPPlanCapital);
            }

            int refR = scheme.FinSourcePlanningFace.GetCapitalClassifierRef(CapitalObjectKeys.t_S_CPPlanService,
                SchemeObjectsKeys.d_R_Plan_Key, sourceId, stock.RefOkv);
            int refEkr = scheme.FinSourcePlanningFace.GetCapitalClassifierRef(CapitalObjectKeys.t_S_CPPlanService,
                SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceId, stock.RefOkv);

            using (IDataUpdater du = t_S_CPPlanService.GetDataUpdater("1 = 2", null))
            {
                DataTable dtCPPlanService = new DataTable();
                du.Fill(ref dtCPPlanService);
                DateTime startDate = (DateTime)stock.StartDate;
                foreach (DataRow percentRow in dtCPJournalPercent.Rows)
                {
                    DataRow newRow = dtCPPlanService.NewRow();
                    newRow["ID"] = t_S_CPPlanService.GetGeneratorNextValue;
                    newRow["Payment"] = dtCPPlanService.Rows.Count + 1;
                    newRow["StartDate"] = startDate;
                    DateTime endDate = Convert.ToDateTime(percentRow["ChargeDate"]);
                    newRow["EndDate"] = endDate;
                    newRow["Period"] = (endDate - startDate).Days;
                    newRow["Per"] = percentRow["CreditPercent"];
                    decimal income = Convert.ToDecimal(percentRow["Coupon"]);
                    newRow["Income"] = income;
                    decimal quantity = GetFieldSum(dtCPPlanCapital, "Quantity",
                                                   string.Format("StartDate <= '{0}'", endDate));
                    newRow["Sum"] = income*quantity;
                    newRow["Quantity"] = quantity;
                    newRow["RefOKV"] = stock.RefOkv;
                    newRow["RefCap"] = stock.ID;
                    newRow["RefR"] = refR;
                    newRow["RefEKR"] = refEkr;
                    dtCPPlanService.Rows.Add(newRow);
                    startDate = endDate;
                }

                du.Update(ref dtCPPlanService);
            }
        }

        /// <summary>
        /// Заполнение детали Журнал ставок
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="dtPercents"></param>
        /// <param name="scheme"></param>
        public void FillPercents(Stock stock, DataTable dtPercents, IScheme scheme)
        {
            IEntity t_S_CPJournalPercent = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPJournalPercent);
            IEntity t_S_CPPlanCapital = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPPlanCapital_Key);
            IEntity t_S_CPPlanDebt = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.t_S_CPPlanDebt_Key);
            DataTable dtCPPlanCapital = new DataTable();
            DataTable dtCPPlanDebt = new DataTable();
            using (IDataUpdater du = t_S_CPPlanCapital.GetDataUpdater("RefCap = ?", null, new DbParameterDescriptor("p0", stock.ID)))
            {
                du.Fill(ref dtCPPlanCapital);
            }
            using (IDataUpdater du = t_S_CPPlanDebt.GetDataUpdater("RefCap = ?", null, new DbParameterDescriptor("p0", stock.ID)))
            {
                du.Fill(ref dtCPPlanDebt);
            }

            using (IDataUpdater du = t_S_CPJournalPercent.GetDataUpdater("1 = 2", null))
            {
                DataTable dtJournaPercents = new DataTable();
                du.Fill(ref dtJournaPercents);
                foreach (DataRow row in dtPercents.Rows)
                {
                    DataRow newRow = dtJournaPercents.NewRow();
                    newRow["ChargeDate"] = row["EndDate"];
                    newRow["CreditPercent"] = row["PercentRate"];

                    decimal percent = Convert.ToDecimal(row["PercentRate"]) / 100;
                    decimal remain =
                        GetFieldSum(dtCPPlanCapital, "Sum", string.Format("StartDate <= '{0}'", row["EndDate"])) -
                        GetFieldSum(dtCPPlanDebt, "Sum", string.Format("EndDate < '{0}'", row["EndDate"]));
                    int periodDaysCount = (Convert.ToDateTime(row["EndDate"]) - Convert.ToDateTime(row["StartDate"])).Days;
                    int yearDaysCount = 365;//Utils.GetYearBase(Convert.ToDateTime(row["EndDate"]).Year);
                    decimal couponsCount = GetFieldSum(dtCPPlanCapital, "Quantity",
                                                       string.Format("StartDate <= '{0}'", row["EndDate"]));

                    newRow["Coupon"] = percent*remain*periodDaysCount/yearDaysCount/couponsCount;
                    newRow["ID"] = t_S_CPJournalPercent.GetGeneratorNextValue;
                    newRow["RefCap"] = stock.ID;
                    dtJournaPercents.Rows.Add(newRow);
                }
                du.Update(ref dtJournaPercents);
            }
        }

        private decimal GetFieldSum(DataTable dataTable, string fieldName, string filter)
        {
            decimal sum = 0;
            foreach (DataRow row in dataTable.Select(filter))
            {
                if (!row.IsNull(fieldName))
                    sum += Convert.ToDecimal(row[fieldName]);
            }
            return sum;
        }

        public decimal FillCurrentBalance(IScheme scheme, object capitalId)
        {
            using (var db = scheme.SchemeDWH.DB)
            {
                var s1 = db.ExecQuery("select sum(Sum) from t_S_CPFactCapital where RefCap = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", capitalId));
                var s2 = db.ExecQuery("select sum(Sum) from t_S_CPFactDebt where RefCap = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", capitalId));
                if (s1 != null && s1 != DBNull.Value && s2 != null && s2 != DBNull.Value)
                {
                    var remainder = Convert.ToDecimal(s1) - Convert.ToDecimal(s2);
                    db.ExecQuery("update f_S_Capital set CDebtRemainder = ? where id= ?", QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("p0", remainder),
                                 new DbParameterDescriptor("p1", capitalId));
                    return remainder;
                }
                return -1;
            }
        }
    }
}
