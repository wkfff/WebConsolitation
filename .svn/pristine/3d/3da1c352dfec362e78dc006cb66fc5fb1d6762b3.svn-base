using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees
{

    public partial class GuaranteeServer
    {
        private IScheme scheme;

        public static GuaranteeServer GetGuaranteeServer(int refOkv, IScheme scheme)
        {
            return refOkv == -1 ? new GuaranteeServer(scheme) : new GuaranteeCurrencyServer(scheme);
        }

        //private FinSourcesRererencesUtils finSourcesRererencesUtils;

        /// <summary>
        /// скрытый конструктор
        /// </summary>
        internal GuaranteeServer(IScheme scheme)
        {
            this.scheme = scheme;
        }

        #region План привлечения

        public void FillAttractPlan(Guarantee guarantee, int baseYear, IScheme scheme)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PlanAttractPrGrnt_Key);
            string refMasterFieldName = entity.Associations[GuaranteeIssuedObjectKeys.a_S_PlanAttractPrGrnt_RefGrnt_Key].RoleDataAttribute.Name;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                using (IDataUpdater du = entity.GetDataUpdater(string.Format("{0} = {1}", refMasterFieldName, guarantee.ID), null))
                {
                    DataTable dtAttractPlan = new DataTable();
                    du.Fill(ref dtAttractPlan);
                    if (dtAttractPlan.Rows.Count > 0)
                        throw new FinSourcePlanningException(
                           "План привлечения уже заполнен. Очистите данные на вкладке 'План привлечения' и повторите попытку");

                    DataTable dtContract = Utils.GetDetailTable(db, guarantee.ID, GuaranteeIssuedObjectKeys.a_S_PrincipalContrGrnt_RefGrnt_Key);
                    PrincipalContract principalContract = new PrincipalContract(dtContract.Rows[0], guarantee);

                    DataRow newRow = dtAttractPlan.NewRow();
                    newRow.BeginEdit();
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    newRow["StartDate"] = dtContract.Rows[0]["StartDate"];
                    newRow["Sum"] = principalContract.Sum;
                    newRow["CurrencySum"] = principalContract.CurrencySum;
                    newRow["RefOKV"] = principalContract.RefOkv;
                    newRow["ExchangeRate"] = newRow.IsNull("StartDate") || principalContract.RefOkv == -1 ?
                        principalContract.ExchangeRate :
                        Utils.GetLastCurrencyExchange(Convert.ToDateTime(newRow["StartDate"]), principalContract.RefOkv);
                    newRow["Sum"] = principalContract.RefOkv == -1 ? principalContract.Sum : Convert.ToDecimal(newRow["ExchangeRate"]) * principalContract.CurrencySum;

                    newRow["IsPrognozExchRate"] = principalContract.IsPrognozExchRate;
                    newRow[refMasterFieldName] = guarantee.ID;
                    newRow.EndEdit();
                    dtAttractPlan.Rows.Add(newRow);
                    du.Update(ref dtAttractPlan);
                }
            }
        }

        #endregion

        /// <summary>
        /// план исполнения обязателтьств
        /// </summary>
        /// <param name="guarantee"></param>
        /// <param name="baseYear"></param>
        public void CalcObligationExecutionPlan(Guarantee guarantee, int baseYear)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;

            IEntity entity = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PlanAttractGrnt_Key);

            string refMasterFieldName = entity.Associations[GuaranteeIssuedObjectKeys.a_S_PlanAttractGrnt_RefGuarantIsd_Key].RoleDataAttribute.Name;
            IDbDataParameter[] prms = new IDbDataParameter[1];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                prms[0] = new System.Data.OleDb.OleDbParameter(refMasterFieldName, guarantee.ID);
                // получаем ссылку на классификатор КИФ
                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 29, baseYear);

                int refKif = scheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_KIF_Plan_Key,
                    guarantee.Regress, sourceID);
                int refEkr = scheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_EKR_PlanOutcomes_Key,
                    guarantee.Regress, sourceID);
                int refR = scheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_R_Plan_Key,
                    guarantee.Regress, sourceID);

                using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    if (dt.Rows.Count > 0)
                        throw new FinSourcePlanningException(
                            "План исполнения по гарантии уже заполнен. Очистите данные на вкладке \"План исполнения по гарантии\" и повторите попытку.");

                    FillObligationExecutionPlan(guarantee, refKif, dt, refMasterFieldName);

                    foreach (DataRow row in dt.Rows)
                    {
                        row["ID"] = entity.GetGeneratorNextValue;
                        row["RefKIF"] = refKif;
                        row["RefEKR"] = refEkr;
                        row["RefR"] = refR;
                        row["RefTypSum"] = 0;
                    }
                    du.Update(ref dt);
                }
            }
        }

        private static void FillObligationExecutionPlan(Guarantee guarantee, int refKif, DataTable dt, string refMasterFieldName)
        {
            DateTime endPeriod = new DateTime(guarantee.StartDate.Year, 12, 31);
            DateTime startPeriod = guarantee.StartDate;
            while (endPeriod.Year <= guarantee.EndDate.Year)
            {
                if (endPeriod > guarantee.EndDate)
                    endPeriod = guarantee.EndDate;
                DataRow row = dt.NewRow();
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = guarantee.Sum;
                row["CurrencySum"] = guarantee.CurrencySum != -1 ? (object)guarantee.CurrencySum : DBNull.Value;
                row["ExchangeRate"] = guarantee.ExchangeRate != -1 ? (object)guarantee.ExchangeRate : DBNull.Value;
                row["IsPrognozExchRate"] = guarantee.IsPrognozExchRate == null ? DBNull.Value : (object)guarantee.IsPrognozExchRate;
                row["RefKIF"] = refKif;
                row["RefOKV"] = guarantee.RefOKV;
                row[refMasterFieldName] = guarantee.ID;
                dt.Rows.Add(row);
                endPeriod = endPeriod.AddYears(1);
                startPeriod = new DateTime(endPeriod.Year, 01, 01);
            }
        }

        #region Формирование плана погашения основного долга

        public void FillMainPlanTable(PrincipalContract principalContract, int baseYear, DateTime startDate, DateTime endDate,
            PayPeriodicity payPeriodicity, int payDay, DataTable periods)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;

            IEntity entity = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PlanDebtPrGrnt_Key);
            string refMasterFieldName = entity.Associations[GuaranteeIssuedObjectKeys.a_S_PlanDebtPrGrnt_Key].RoleDataAttribute.Name;

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                principalContract.Guarantee.SourceID = Utils.GetDataSourceID(db, "ФО", 29, 8, baseYear);

                using (IDataUpdater du = entity.GetDataUpdater(string.Format("{0} = {1}", refMasterFieldName, principalContract.Guarantee.ID), null))
                {
                    DataTable dtMainPlan = new DataTable();
                    du.Fill(ref dtMainPlan);
                    FillMainPlanTable(principalContract, dtMainPlan, refMasterFieldName, startDate, endDate, payPeriodicity, payDay, periods, entity);
                    int refKif = scheme.FinSourcePlanningFace.GetGuaranteeClassifierRef(entity.ObjectKey,
                        SchemeObjectsKeys.d_KIF_Plan_Key, principalContract.Guarantee.Regress, principalContract.Guarantee.SourceID);
                    foreach (DataRow row in dtMainPlan.Rows)
                    {
                        row["RefKIF"] = refKif;
                    }
                    du.Update(ref dtMainPlan);
                }
            }
        }

        /// <summary>
        /// заполнение плана погашения основного долга
        /// </summary>
        protected virtual void FillMainPlanTable(PrincipalContract principalContract, DataTable dtData, string refMasterFieldName,
            DateTime startDate, DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods, IEntity entity)
        {
            int periodCount = Utils.GetPeriodCount(startDate, endDate, payPeriodicity, payDay, periods, true);
            int subtract = -Utils.GetSubtractValue(payPeriodicity);
            bool singlePeriod = false;
            decimal guaranteeSum = principalContract.Sum;

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
                singlePeriod = periods.Rows.Count == 1;
            }

            DateTime endPeriod = DateTime.MinValue;
            DateTime startPeriod = startDate;

            if (payDay == 0)
            {
                // получаем последний день месяца
                payDay = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            }

            int i = 0;
            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0 && payPeriodicity != PayPeriodicity.Other)
            {
                endPeriod = GetEndPeriod(startPeriod, subtract, payDay);
                DataRow row = dtData.NewRow();
                row.BeginEdit();
                row["ID"] = entity.GetGeneratorNextValue;
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(guaranteeSum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["RefOKV"] = principalContract.RefOkv;
                row[refMasterFieldName] = principalContract.Guarantee.ID;
                row.EndEdit();
                dtData.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            // идем от даты заключения до даты закрытия (пролонгирования)
            while (endPeriod < endDate)
            {
                if (payPeriodicity == PayPeriodicity.Other)
                {
                    startPeriod = Convert.ToDateTime(periods.Rows[i]["StartDate"]);
                    endPeriod = Convert.ToDateTime(periods.Rows[i]["EndDate"]);
                }
                else if (payPeriodicity == PayPeriodicity.Single)
                {
                    endPeriod = endDate;
                }
                else
                {
                    endPeriod = GetEndPeriod(startPeriod, subtract, payDay);
                }

                if (endPeriod > endDate)
                    endPeriod = endDate;

                DataRow row = dtData.NewRow();
                row.BeginEdit();
                row["ID"] = entity.GetGeneratorNextValue;
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(guaranteeSum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["RefOKV"] = principalContract.RefOkv;
                row[refMasterFieldName] = principalContract.Guarantee.ID;
                row.EndEdit();
                dtData.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            decimal sum = 0;
            foreach (DataRow row in dtData.Select(string.Format("Payment < {0}", i - 1)))
            {
                sum += Convert.ToDecimal(row["Sum"]);
            }
            if (dtData.Select(string.Format("Payment = {0}", i)).Length > 0)
                dtData.Select(string.Format("Payment = {0}", i))[0]["Sum"] = guaranteeSum - sum;
        }

        #endregion

        #region Формирование плана обсуживания долга

        #region новый расчет процентов

        protected Decimal GetPercentPay(DateTime startPeriod, DateTime endPeriod,
            PayPeriodicity payPeriodicity, PrincipalContract principalContract, decimal percent, PercentRestRound restRound)
        {
            // получим сумму, выплаченную по плану выплаты на момент начисления процентов
            List<DateTime> subPeriods = new List<DateTime>();
            DataTable dtFactDebt = GetDebtFact(principalContract.Guarantee.ID);
            DataTable dtPlanDebt = GetDebtPlan(principalContract.Guarantee.ID);
            DataTable dtPlanAttract = GetAttractPlan(principalContract.Guarantee.ID);
            DataTable dtFactAttract = GetAttractFact(principalContract.Guarantee.ID);
            DataTable dtPercents = PercentTable ?? GetPercentJournal(principalContract.Guarantee.ID);
            // периоды по плану погашения
            DataRow[] rows = dtPlanDebt.Select(string.Format("EndDate <= '{0}' and EndDate > '{1}'", endPeriod, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = Convert.ToDateTime(row["EndDate"]);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по плану привлечения
            rows = dtPlanAttract.Select(string.Format("StartDate <= '{0}' and StartDate > '{1}'", endPeriod, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = Convert.ToDateTime(row["StartDate"]).AddDays(-1);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по факту привлечения
            rows = dtFactAttract.Select(string.Format("FactDate <= '{0}' and FactDate > '{1}'", endPeriod, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = Convert.ToDateTime(row["FactDate"]).AddDays(-1);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по смене процентной ставки
            rows = dtPercents.Select(string.Format("ChargeDate <= '{0}' and ChargeDate >= '{1}'", endPeriod, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = Convert.ToDateTime(row["ChargeDate"]).AddDays(-1);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // текущая дата 
            if (DateTime.Today <= endPeriod && DateTime.Today >= startPeriod && !subPeriods.Contains(DateTime.Today))
                subPeriods.Add(DateTime.Today);
            // если стоит досрочное погашение, то периоды по факту погашения
            if (principalContract.PretermDisharge)
            {
                rows = dtFactDebt.Select(string.Format("FactDate <= '{0}' and FactDate > '{1}'", endPeriod, startPeriod));
                foreach (DataRow row in rows)
                {
                    DateTime factDate = Convert.ToDateTime(row["FactDate"]);
                    if (!subPeriods.Contains(factDate))
                        subPeriods.Add(factDate);
                }
            }
            if (!subPeriods.Contains(startPeriod))
                subPeriods.Add(startPeriod);

            if (principalContract.StartDate <= endPeriod && principalContract.StartDate >= startPeriod && !subPeriods.Contains(principalContract.StartDate))
                subPeriods.Add(principalContract.StartDate);

            if (!subPeriods.Contains(endPeriod))
                subPeriods.Add(endPeriod);
            subPeriods.Sort();
            decimal percents = 0;

            foreach (DateTime date in subPeriods)
            {
                endPeriod = date;
                decimal sum = GetAttractSum(principalContract, endPeriod);
                decimal debtSum = GetDebtPaymentSum(principalContract, startPeriod, endPeriod);
                sum -= debtSum;
                // получаем ставку процентов из журнала процентных ставок
                percent = GetPercent(principalContract, dtPercents, endPeriod);
                // получаем сумму процентов, которые уже попадают в таблицу
                percent = GetPercents(sum, startPeriod, endPeriod, percent);
                percents += percent;
                startPeriod = endPeriod.AddDays(1);
            }
            // расчет остатков процентов
            // сумма процентов, округленная до 2 знаков
            decimal roundPercent = Math.Round(percents, 2, MidpointRounding.AwayFromZero);
            // разница между округлением и обычной суммой процентов
            decimal percentOdds = percents - roundPercent;
            // считаем текущие проценты 
            if (restRound == PercentRestRound.OnAccumulation)
            {
                if (percentOddsList.ContainsKey(percentOddsList.Count - 1))
                {
                    decimal prevOdds = percentOddsList[percentOddsList.Count - 1];
                    percentOdds = percentOdds + prevOdds;
                    if (Math.Abs(percentOdds) > new Decimal(0.005))
                    {
                        roundPercent += percentOdds > 0 ? new Decimal(0.01) : new Decimal(-0.01);
                        percentOdds = Math.Round(percentOdds + (percentOdds > 0 ? new Decimal(-0.010001) : new Decimal(0.010001)), 4, MidpointRounding.AwayFromZero);
                    }
                }
                percents = roundPercent;
            }
            percentOddsList.Add(percentOddsList.Count, percentOdds);
            return percents;
        }

        /// <summary>
        /// возвращает сумму, привлеченную до определенной даты
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        protected Decimal GetAttractSum(PrincipalContract principalContract, DateTime date)
        {
            // если данных нет никаких, возвращаем сумму самого кредита
            DataTable dtFactAttract = GetAttractFact(principalContract.Guarantee.ID);
            DataTable dtPlanAttract = GetAttractPlan(principalContract.Guarantee.ID);
            if (dtFactAttract.Rows.Count == 0 && dtPlanAttract.Rows.Count == 0)
            {
                // при незаполненных планах и фактах всегда возвращаем ноль
                return 0;
            }
            // сравниваем суммы плановую и фактическую, привлеченные до определенного времени
            // возвращаем максимальное значение
            decimal factAttractSum = 0;

            bool hasFact = dtFactAttract.Rows.Count > 0;
            DataRow[] rows = dtFactAttract.Select(string.Format("FactDate <= '{0}'", date));

            foreach (DataRow row in rows)
            {
                if (principalContract.RefOkv == -1)
                    factAttractSum += Convert.ToDecimal(row["Sum"]);
                else
                    factAttractSum += row.IsNull("CurrencySum") ? 0 : Convert.ToDecimal(row["CurrencySum"]);
            }

            decimal planAttractSum = 0;

            rows = hasFact
                        ? dtPlanAttract.Select(string.Format("StartDate > '{0}' and StartDate <= '{1}'", DateTime.Today, date))
                        : dtPlanAttract.Select(string.Format("StartDate <= '{0}'", date));
            
            foreach (DataRow row in rows)
            {
                if (principalContract.RefOkv == -1)
                    planAttractSum += Convert.ToDecimal(row["Sum"]);
                else
                    planAttractSum += row.IsNull("CurrencySum") ? 0 : Convert.ToDecimal(row["CurrencySum"]);
            }
            
            if (hasFact && date <= DateTime.Today)
                return factAttractSum;

            return factAttractSum + planAttractSum;
        }

        /// <summary>
        /// получаем процент для пени на определенную дату
        /// </summary>
        /// <returns></returns>
        protected static Decimal GetPercent(PrincipalContract principalContract, DataTable percentTable, DateTime percentDate)
        {
            DataRow[] rows = percentTable.Select(string.Format("ChargeDate <= '{0}'", percentDate), "ChargeDate DESC");
            if (rows.Length != 0)
            {
                return Convert.ToDecimal(rows[0]["CreditPercent"]) / 100;
            }
            return 0;
        }

        /// <summary>
        ///  возвращает сумму выплаченного долга до определенной даты
        /// </summary>
        /// <returns></returns>
        protected decimal GetDebtPaymentSum(PrincipalContract principalContract, DateTime startDate, DateTime endDate)
        {
            if (principalContract.PretermDisharge)
            {
                // для досрочного погашения используем план погашения и факт погашения
                DataTable dtFact = GetDebtFact(principalContract.Guarantee.ID);
                DataTable dtPlan = GetDebtPlan(principalContract.Guarantee.ID);
                decimal planSum = 0;
                decimal factSum = 0;
                bool hasFact = dtFact.Rows.Count > 0;
                DataRow[] rows = dtFact.Select(string.Format("FactDate < '{0}'", endDate));
                foreach (DataRow row in rows)
                    factSum += principalContract.RefOkv == -1 ? Convert.ToDecimal(row["Sum"]) : Convert.ToDecimal(row["CurrencySum"]);

                rows = hasFact ? dtPlan.Select(string.Format("EndDate > '{0}' and EndDate < '{1}'", DateTime.Today, endDate)) :
                    dtPlan.Select(string.Format("EndDate < '{0}'", endDate));
                foreach (DataRow row in rows)
                    planSum += principalContract.RefOkv == -1
                                    ? Convert.ToDecimal(row["Sum"])
                                    : Convert.ToDecimal(row["CurrencySum"]);

                if (endDate <= DateTime.Today)
                    return factSum;

                return planSum + factSum;
            }
            else
            {
                decimal paymentPlanSum = 0;
                // если нет досрочного погашения, то используем только план погашения
                DataTable dtPlan = GetDebtPlan(principalContract.Guarantee.ID);
                DataRow[] rows = dtPlan.Select(string.Format("EndDate < '{0}'", endDate));
                foreach (DataRow row in rows)
                    paymentPlanSum += principalContract.RefOkv == -1 ? Convert.ToDecimal(row["Sum"]) : Convert.ToDecimal(row["CurrencySum"]);
                return paymentPlanSum;
            }
        }

        public DataTable GetAttractFact(int masterID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, masterID, GuaranteeIssuedObjectKeys.a_S_FactAttractPrGrnt_RefGrnt);
            }
        }

        public DataTable GetAttractPlan(int masterID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, masterID, GuaranteeIssuedObjectKeys.a_S_PlanAttractPrGrnt_RefGrnt);
            }
        }

        public DataTable GetPercentJournal(int masterID)
        {
            using (IDatabase db = scheme .SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, masterID, GuaranteeIssuedObjectKeys.a_S_JournalPercentGrnt_RefGrnt_Key);
            }
        }

        #endregion

        protected Decimal GetPercentPay(ref decimal sum, DateTime startPeriod, DateTime endPeriod,
            PayPeriodicity payPeriodicity, List<int> payments, List<int> payments2, 
            PrincipalContract principalContract, DataTable mainDebtPlan, DataTable mainDebtFact, decimal percentRate, bool allowRound)
        {
            if (payPeriodicity == PayPeriodicity.Single)
                return GetPercents(principalContract.Sum, principalContract.StartDate, endPeriod, percentRate);

            DataRow[] planRows =
                mainDebtPlan.Select(string.Format("EndDate < '{0}'", startPeriod));
            if (payPeriodicity == PayPeriodicity.Single)
                startPeriod = principalContract.StartDate;

            DataRow[] factRows = mainDebtFact.Select(string.Format("FactDate < '{0}'", startPeriod));

            foreach (DataRow row in planRows)
            {
                if (!payments.Contains(Convert.ToInt32(row["ID"])))
                {
                    payments.Add(Convert.ToInt32(row["ID"]));
                    sum -= Convert.ToDecimal(row["Sum"]);
                }
            }

            foreach (DataRow row in factRows)
            {
                if (!payments2.Contains(Convert.ToInt32(row["ID"])))
                {
                    payments2.Add(Convert.ToInt32(row["ID"]));
                    sum -= Convert.ToDecimal(row["Sum"]);
                }
            }

            decimal percents = GetPercents(sum, startPeriod, endPeriod, percentRate);
            // расчет остатков процентов
            // сумма процентов, округленная до 2 знаков
            decimal roundPercent = Math.Round(percents, 2, MidpointRounding.AwayFromZero);
            // разница между округлением и обычной суммой процентов
            decimal percentOdds = percents - roundPercent;
            // считаем текущие проценты 
            if (allowRound)
            {
                if (percentOddsList.ContainsKey(percentOddsList.Count - 1))
                {
                    decimal prevOdds = percentOddsList[percentOddsList.Count - 1];
                    percentOdds = percentOdds + prevOdds;
                    if (Math.Abs(percentOdds) > new Decimal(0.005))
                    {
                        roundPercent += percentOdds > 0 ? new Decimal(0.01) : new Decimal(-0.01);
                        percentOdds = Math.Round(percentOdds + (percentOdds > 0 ? new Decimal(-0.010001) : new Decimal(0.010001)), 4, MidpointRounding.AwayFromZero);
                    }
                }
                percents = roundPercent;
            }
            percentOddsList.Add(percentOddsList.Count, percentOdds);

            return percents;
        }

        private DataTable dtPercentTable;
        private DataTable PercentTable
        {
            get { return dtPercentTable; }
            set { dtPercentTable = value; }
        }

        protected Decimal GetPercentPeriods(DateTime startDate, DateTime endDate)
        {
            DataRow[] rows = PercentTable.Select(string.Format("ChargeDate <= '{0}'", endDate), "ChargeDate DESC");
            List<PercentPeriod> periods = new List<PercentPeriod>();
            DateTime endPeriod = endDate.AddDays(1);
            decimal percent = 0;
            foreach (DataRow row in rows)
            {
                percent = Convert.ToDecimal(row["CreditPercent"]) / 100;
                DateTime currentPercentDate = Convert.ToDateTime(row["ChargeDate"]);
                int periodDays = startDate > currentPercentDate
                     ? (endPeriod - startDate).Days
                     : (endPeriod - currentPercentDate).Days;
                PercentPeriod percentPeriod = new PercentPeriod(periodDays, percent);
                periods.Add(percentPeriod);
                if (currentPercentDate > startDate)
                {
                    endPeriod = currentPercentDate;
                }
                else
                    break;
            }
            percent = 0;
            foreach (PercentPeriod period in periods)
            {
                percent += period.DaysCount * period.Percent;
            }
            return percent / ((endDate - startDate).Days + 1);
        }

        public struct PercentPeriod
        {
            private readonly int daysCount;
            private readonly decimal percent;
            public PercentPeriod(int daysCount, decimal percent)
            {
                this.daysCount = daysCount;
                this.percent = percent;
            }
            public int DaysCount
            {
                get { return daysCount; }
            }
            public Decimal Percent
            {
                get { return percent; }
            }
        }

        #endregion

        #region вспомогательные методы
        /// <summary>
        /// определяет, заполнен ли гарантируемый договор у гарантии
        /// </summary>
        /// <param name="guaranteeID"></param>
        /// <returns></returns>
        public static bool PrincipalContractFilled(int guaranteeID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt = Utils.GetDetailTable(db, guaranteeID, GuaranteeIssuedObjectKeys.a_S_PrincipalContrGrnt_RefGrnt_Key);
                return dt.Rows.Count != 0;
            }
        }

        public static DataRow GetPrincipalContract(DataRow guaranteeRow)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt = Utils.GetDetailTable(db, Convert.ToInt32(guaranteeRow["ID"]), GuaranteeIssuedObjectKeys.a_S_PrincipalContrGrnt_RefGrnt_Key);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        protected static Decimal GetGuaranteeSum(PrincipalContract principalContract)
        {
            return principalContract.RefOkv == -1 ?
                principalContract.Sum : principalContract.CurrencySum;
        }

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

        protected Decimal GetPercentPay(ref decimal sum, DateTime startPeriod, DateTime endPeriod,
            PayPeriodicity payPeriodicity, List<int> payments, PrincipalContract principalContract,
            DataTable dtDebtPlan, decimal percentRate, bool allowRound)
        {
            DataRow[] rows =
                dtDebtPlan.Select(string.Format("EndDate < '{0}'", startPeriod));
            if (payPeriodicity == PayPeriodicity.Single)
                startPeriod = principalContract.StartDate;
            foreach (DataRow row in rows)
            {
                if (!payments.Contains(Convert.ToInt32(row["ID"])))
                {
                    payments.Add(Convert.ToInt32(row["ID"]));
                    sum -= Convert.ToDecimal(row["Sum"]);
                }
            }

            decimal percents = payPeriodicity == PayPeriodicity.Single ?
                    GetPercents(principalContract.Sum, principalContract.StartDate, endPeriod, percentRate) :
                    GetPercents(sum, startPeriod, endPeriod, percentRate);

            // расчет остатков процентов
            // сумма процентов, округленная до 2 знаков
            decimal roundPercent = Math.Round(percents, 2, MidpointRounding.AwayFromZero);
            // разница между округлением и обычной суммой процентов
            decimal percentOdds = percents - roundPercent;
            // считаем текущие проценты 
            if (allowRound)
            {
                if (percentOddsList.ContainsKey(percentOddsList.Count - 1))
                {
                    decimal prevOdds = percentOddsList[percentOddsList.Count - 1];
                    percentOdds = percentOdds + prevOdds;
                    if (Math.Abs(percentOdds) > new Decimal(0.005))
                    {
                        roundPercent += percentOdds > 0 ? new Decimal(0.01) : new Decimal(-0.01);
                        percentOdds = Math.Round(percentOdds + (percentOdds > 0 ? new Decimal(-0.010001) : new Decimal(0.010001)), 4, MidpointRounding.AwayFromZero);
                    }
                }
                percents = roundPercent;
            }
            percentOddsList.Add(percentOddsList.Count, percentOdds);

            return percents;
        }

        protected Dictionary<int, decimal> percentOddsList;

        /// <summary>
        ///  расчет процентов исходя из того, что период может содержать разные года
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="percentRate"></param>
        /// <returns></returns>
        protected static decimal GetPercents(decimal sum, DateTime startDate, DateTime endDate, decimal percentRate)
        {
            DateTime startPeriod = startDate;
            decimal percentSum = 0;
            int currentYear = startPeriod.Year;

            for (int i = 0; i <= endDate.Year - startDate.Year; i++)
            {
                DateTime endOfYear = DateTime.MinValue.AddYears(startPeriod.Year - 1).AddMonths(11).AddDays(30);
                // при подсчете количества дней до конца года, учитываем все дни включая последний день года
                int daysCount = endOfYear > endDate
                                ? (endDate - startPeriod).Days + 1
                                : (endOfYear - startPeriod).Days + 1;
                percentSum += sum * percentRate * daysCount / Utils.GetYearBase(currentYear);
                startPeriod = DateTime.MinValue.AddYears(currentYear);
                currentYear++;
            }
            return percentSum >= 0 ? percentSum : 0;
        }

        protected DataTable GetMainDebtPlan(int guaranteeID, Decimal sum, DateTime startDate,
            DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods)
        {
            DataTable dt = GetDebtPlan(guaranteeID);
            if (dt.Rows.Count == 0)
                dt = GetVirtualMainPlanTable(sum, startDate, endDate, payPeriodicity, payDay, periods);
            return dt;
        }

        /// <summary>
        /// построение плана погашения долга исходя из указанных параметров
        /// </summary>
        protected static DataTable GetVirtualMainPlanTable(Decimal sum, DateTime startDate,
            DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods)
        {
            DataTable dtPlan = new DataTable();
            dtPlan.Columns.Add("ID", typeof(Int32));
            dtPlan.Columns.Add("StartDate", typeof(DateTime));
            dtPlan.Columns.Add("EndDate", typeof(DateTime));
            dtPlan.Columns.Add("Sum", typeof(Decimal));

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
            }

            int periodCount = Utils.GetPeriodCount(startDate, endDate, payPeriodicity, payDay, periods, true);
            DateTime endPeriod = DateTime.MinValue;
            DateTime startPeriod = startDate;
            Decimal innerSum = sum;
            int addition = -Utils.GetSubtractValue(payPeriodicity);
            int i = 0;

            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0)
            {
                endPeriod = GetEndPeriod(startPeriod, addition, payDay);
                DataRow row = dtPlan.NewRow();
                row[0] = i;
                row[1] = startPeriod;
                row[2] = endPeriod;
                row[3] = Math.Round(innerSum / periodCount, 2, MidpointRounding.AwayFromZero);
                row.EndEdit();
                dtPlan.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            while (endPeriod < endDate)
            {
                if (payPeriodicity == PayPeriodicity.Other)
                {
                    startPeriod = Convert.ToDateTime(periods.Rows[i]["StartDate"]);
                    endPeriod = Convert.ToDateTime(periods.Rows[i]["EndDate"]);
                }
                else if (payPeriodicity == PayPeriodicity.Single)
                {
                    endPeriod = endDate;
                    startPeriod = endDate.AddMonths(-1);
                }
                else
                {
                    endPeriod = GetEndPeriod(startPeriod, addition, payDay);
                }

                if (endPeriod > endDate)
                {
                    endPeriod = endDate;
                }

                DataRow row = dtPlan.NewRow();
                row.BeginEdit();
                row[0] = i;
                row[1] = startPeriod;
                row[2] = endPeriod;
                row[3] = Math.Round(innerSum / periodCount, 2, MidpointRounding.AwayFromZero);
                row.EndEdit();
                dtPlan.Rows.Add(row);

                startPeriod = endPeriod.AddDays(1);
                i++;
            }
            dtPlan.AcceptChanges();
            return dtPlan;
        }

        /// <summary>
        /// получение данных детали процентов договора
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetCreditPercents(int parentID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, GuaranteeIssuedObjectKeys.a_S_JournalPercentGrnt_RefGrnt_Key);
            }
        }

        /// <summary>
        /// получение данных детали плана погашения
        /// </summary>
        /// <returns></returns>
        public DataTable GetDebtPlan(int parentID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, GuaranteeIssuedObjectKeys.a_S_PlanDebtPrGrnt_Key);
            }
        }

        /// <summary>
        /// получение данных плана обслуживания долга
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetServicePlan(int parentID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, GuaranteeIssuedObjectKeys.a_S_PlanServicePrGrnt_Key);
            }
        }

        /// <summary>
        /// получение данных детали факта погашения
        /// </summary>
        /// <returns></returns>
        public DataTable GetDebtFact(int parentID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, GuaranteeIssuedObjectKeys.a_S_FactDebtPrGrnt_RefGrnt_Key);
            }
        }

        /// <summary>
        /// получение данных факта погашения процентов
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetPercentFact(int parentID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, GuaranteeIssuedObjectKeys.a_S_FactPercentPrGrnt_RefGrnt_Key);
            }
        }

        /// <summary>
        /// получение данных детали начисления пени по основному долгу
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetDebtPenaltyTable(int parentID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, GuaranteeIssuedObjectKeys.a_S_ChargePenaltyDebtPrGrnt_RefGrnt_Key);
            }
        }

        /// <summary>
        /// получение данных детали начисления пени по процентам
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetPercentPenaltyTable(int parentID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, GuaranteeIssuedObjectKeys.a_S_PrGrntChargePenaltyPercent_RefGrnt_Key);
            }
        }

        #endregion

        public virtual decimal FillCurrentBalance(IScheme scheme, object guarantyId)
        {
            using (var db = scheme.SchemeDWH.DB)
            {
                var s1 = db.ExecQuery("select sum(Sum) from t_S_FactAttractPrGrnt where RefGrnt = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", guarantyId));
                var s2 = db.ExecQuery("select sum(Sum) from t_S_FactDebtPrGrnt where RefGrnt = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", guarantyId));
                var s3 = db.ExecQuery("select sum(Sum) from t_S_FactAttractGrnt where RefGrnt = ? and (RefTypSum = -1 or RefTypSum = 1)", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", guarantyId));
                if (s1 != null && s1 != DBNull.Value && s2 != null && s2 != DBNull.Value && s3 != null && s3 != DBNull.Value)
                {
                    var remainder = Convert.ToDecimal(s1) - (Convert.ToDecimal(s2) + Convert.ToDecimal(s3));
                    db.ExecQuery("update f_S_Guarantissued set DebtRemainder = ? where id= ?", QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("p0", remainder),
                                 new DbParameterDescriptor("p1", guarantyId));
                    return remainder;
                }
                return -1;
            }
        }

    }

    public partial class GuaranteeCurrencyServer : GuaranteeServer
    {
        internal GuaranteeCurrencyServer(IScheme scheme)
            : base(scheme)
        {
            
        }

        #region Расчет погашения основного долга

        protected override void FillMainPlanTable(PrincipalContract principalContract, DataTable dtData, string refMasterFieldName,
            DateTime startDate, DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods, IEntity entity)
        {
            int periodCount = Utils.GetPeriodCount(startDate, endDate, payPeriodicity, payDay, periods, true);
            int subtract = -Utils.GetSubtractValue(payPeriodicity);

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = startDate;
            DateTime endPeriod = startPeriod;

            decimal guaranteeCurrencySum = principalContract.CurrencySum;
            decimal guaranteeSum = principalContract.Sum;

            if (payDay == 0)
            {
                // получаем последний день месяца
                payDay = 31;
            }

            int i = 0;
            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0 && payPeriodicity != PayPeriodicity.Other)
            {
                endPeriod = GetEndPeriod(startPeriod, subtract, payDay);
                if (endPeriod > endDate) endPeriod = endDate;
                DataRow row = dtData.NewRow();
                row.BeginEdit();
                row["ID"] = entity.GetGeneratorNextValue;
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(guaranteeSum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["CurrencySum"] = Math.Round(guaranteeCurrencySum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["ExchangeRate"] = principalContract.ExchangeRate;
                row["RefOKV"] = principalContract.RefOkv;
                row["IsPrognozExchRate"] = principalContract.IsPrognozExchRate;
                row[refMasterFieldName] = principalContract.Guarantee.ID;
                row.EndEdit();
                dtData.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            // идем от даты заключения до даты закрытия (пролонгирования)
            while (endPeriod < endDate)
            {
                if (payPeriodicity == PayPeriodicity.Other)
                {
                    startPeriod = Convert.ToDateTime(periods.Rows[i]["StartDate"]);
                    endPeriod = Convert.ToDateTime(periods.Rows[i]["EndDate"]);
                }
                else if (payPeriodicity == PayPeriodicity.Single)
                    endPeriod = endDate;
                else
                    endPeriod = GetEndPeriod(startPeriod, subtract, payDay);

                if (endPeriod > endDate)
                    endPeriod = endDate;

                DataRow row = dtData.NewRow();
                row.BeginEdit();
                row["ID"] = entity.GetGeneratorNextValue;
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(guaranteeSum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["CurrencySum"] = Math.Round(guaranteeCurrencySum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["ExchangeRate"] = principalContract.ExchangeRate;
                row["RefOKV"] = principalContract.RefOkv;
                row["IsPrognozExchRate"] = principalContract.IsPrognozExchRate;
                row[refMasterFieldName] = principalContract.Guarantee.ID;
                row.EndEdit();
                dtData.Rows.Add(row);
                startPeriod = endPeriod.AddDays(1);
                i++;
            }

            decimal sum = 0;
            decimal currencySum = 0;
            foreach (DataRow row in dtData.Select(string.Format("Payment < {0}", i)))
            {
                sum += Convert.ToDecimal(row["Sum"]);
                currencySum += Convert.ToDecimal(row["CurrencySum"]);
            }
            if (dtData.Select(string.Format("Payment = {0}", i)).Length > 0)
            {
                dtData.Select(string.Format("Payment = {0}", i))[0]["Sum"] = guaranteeSum - sum;
                dtData.Select(string.Format("Payment = {0}", i))[0]["CurrencySum"] = guaranteeCurrencySum - currencySum;
            }
        }

        #endregion

        public override decimal FillCurrentBalance(IScheme scheme, object guarantyId)
        {
            using (var db = scheme.SchemeDWH.DB)
            {
                var s1 = db.ExecQuery("select sum(CurrencySum) from t_S_FactAttractPrGrnt where RefGrnt = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", guarantyId));
                var s2 = db.ExecQuery("select sum(CurrencySum) from t_S_FactDebtPrGrnt where RefGrnt = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", guarantyId));
                var s3 = db.ExecQuery("select sum(CurrencySum) from t_S_FactAttractGrnt where RefGrnt = ? and (RefTypSum = -1 or RefTypSum = 1)", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", guarantyId));
                if (s1 != null && s1 != DBNull.Value && s2 != null && s2 != DBNull.Value && s3 != null && s3 != DBNull.Value)
                {
                    var remainder = Convert.ToDecimal(s1) - (Convert.ToDecimal(s2) + Convert.ToDecimal(s3));
                    db.ExecQuery("update f_S_Guarantissued set GCurrDebtRemainder = ? where id= ?", QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("p0", remainder),
                                 new DbParameterDescriptor("p1", guarantyId));
                    return remainder;
                }
                return -1;
            }
        }
    }

    public class Guarantee
    {
        private DataRow _row;

        public Guarantee(DataRow row)
        {
            _row = row;
        }

        public int ID
        {
            get { return Convert.ToInt32(_row["ID"]); }
        }

        public DateTime StartDate
        {
            get { return Convert.ToDateTime(_row["StartDate"]); }
        }

        public DateTime EndDate
        {
            get
            {

                DateTime endDate = DateTime.MinValue;
                if (!_row.IsNull("RenewalDate"))
                    return Convert.ToDateTime(_row["RenewalDate"]);
                if (DateTime.TryParse(_row["EndDate"].ToString(), out endDate))
                    return endDate;
                throw new FinSourcePlanningException(
                    "Срок действия гарантии и дата пролонгации не заполнены");
            }
        }

        public int RefOKV
        {
            get { return Convert.ToInt32(_row["RefOKV"]); }
        }

        public Decimal Sum
        {
            get { return Convert.ToDecimal(_row["Sum"]); }
        }

        public Decimal CurrencySum
        {
            get
            {
                return RefOKV != -1 ? Convert.ToDecimal(_row["CurrencySum"]) : -1;
            }
        }

        public Decimal ExchangeRate
        {
            get { return RefOKV != -1 ? Convert.ToDecimal(_row["ExchangeRate"]) : -1; }
        }

        public bool? IsPrognozExchRate
        {
            get
            {
                if (_row.IsNull("IsPrognozExchRate"))
                    return null;
                return Convert.ToBoolean(_row["IsPrognozExchRate"]);
            }
        }

        public bool Regress
        {
            get { return Convert.ToBoolean(_row["Regress"]); }
        }

        public int SourceID
        {
            get { return Convert.ToInt32(_row["SourceID"]); }
            set { _row["SourceID"] = value; }
        }

        public CreditType CreditType
        {
            get
            {
                int statusPlan = Convert.ToInt32(_row["RefSStatusPlan"]);
                bool isIncome = Convert.ToInt32(_row["RefSStatusPlan"]) == 0 ||
                                Convert.ToInt32(_row["RefSStatusPlan"]) == 1;
                if (statusPlan == 0)
                {
                    if (isIncome)
                        return CreditType.ActiveIncome;
                    return CreditType.ActiveOutcome;
                }
                if (isIncome)
                    return CreditType.PlanIncome;
                return CreditType.PlanOutcome;
            }
        }
    }

    public class PrincipalContract
    {
        private DataRow principalContractData;
        private Guarantee guarantee;

        public PrincipalContract(DataRow principalContractData, Guarantee guarantee)
        {
            this.guarantee = guarantee;
            if (principalContractData == null)
                throw new FinSourcePlanningException(
                    "Данные по гарантируемому договору не заполнены. Заполните Данные и повторите попытку");
            this.principalContractData = principalContractData;
        }

        public Guarantee Guarantee
        {
            get { return guarantee; }
        }

        public decimal Sum
        {
            get { return principalContractData.IsNull("Sum") ? 0 : Convert.ToDecimal(principalContractData["Sum"]); }
        }

        public decimal CurrencySum
        {
            get { return principalContractData.IsNull("CurrencySum") ? 0 : Convert.ToDecimal(principalContractData["CurrencySum"]); }
        }

        public DateTime StartDate
        {
            get { return Convert.ToDateTime(principalContractData["StartDate"]); }
        }

        public DateTime EndDate
        {
            get { return Convert.ToDateTime(principalContractData["EndDate"]); }
        }

        public decimal PercentRate
        {
            get { return Convert.ToDecimal(principalContractData["CreditPercent"]); }
        }

        public decimal ExchangeRate
        {
            get { return principalContractData.IsNull("ExchangeRate") ? 0 : Convert.ToDecimal(principalContractData["ExchangeRate"]); }
        }

        public bool IsPrognozExchRate
        {
            get { return principalContractData.IsNull("IsPrognozExchRate") ? false : Convert.ToBoolean(principalContractData["IsPrognozExchRate"]); }
        }

        public bool PretermDisharge
        {
            get { return principalContractData.IsNull("PretermDisharge") ? false : Convert.ToBoolean(principalContractData["PretermDisharge"]); }
        }

        public int RefOkv
        {
            get { return Convert.ToInt32(principalContractData["RefOKV"]); }
        }

        public decimal PenaltyPercentRate
        {
            get {
                return principalContractData.IsNull("PenaltyPercentRate")
                           ? 0
                           : Convert.ToDecimal(principalContractData["PenaltyPercentRate"]); }
        }

        public decimal PenaltyDebtRate
        {
            get
            {
                return principalContractData.IsNull("PenaltyDebtRate")
                           ? 0
                           : Convert.ToDecimal(principalContractData["PenaltyDebtRate"]);
            }
        }
    }
}
