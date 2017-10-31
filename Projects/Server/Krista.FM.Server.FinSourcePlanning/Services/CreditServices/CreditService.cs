using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using Krista.FM.ServerLibrary;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using Krista.FM.Server.FinSourcePlanning.Constants;

namespace Krista.FM.Server.FinSourcePlanning.Services
{

    #region перечисления

    /// <summary>
    /// Переодичность выплат.
    /// </summary>
    public enum PayPeriodicity
    {
        [Description("День")]
        Day,
        [Description("Месяц")]
        Month,
        [Description("Квартал")]
        Quarter,
        [Description("Полгода")]
        HalfYear,
        [Description("Год")]
        Year,
        [Description("Единовременное погашение")]
        Single,
        [Description("Другая периодичность")]
        Other
    }

    /// <summary>
    /// Метод определения продолжительности финансовой операции (в днях).
    /// </summary>
    public enum YearTypes
    {
        [Description("365/365 (366/366)")]
        Year_365_365_366_366 = 1,
        [Description("365(366)/360")]
        Year_365_366_360 = 2,
        [Description("360/360")]
        Year_360_360 = 3,
        [Description("360/365(366)")]
        Year_360_365_366 = 4
    }

    public enum CreditType
    {
        [Description("Планируемый договор. Кредиты полученные")]
        PlanIncome,
        [Description("Планируемый договор. Кредиты предоставленные")]
        PlanOutcome,
        [Description("Действующий договор. Кредиты полученные")]
        ActiveIncome,
        [Description("Действующий договор. Кредиты предоставленные")]
        ActiveOutcome
    }

    public enum CreditsTypes
    {
        [Description("Кредиты полученные от других бюджетов")]
        BudgetIncoming,
        [Description("Кредиты предоставленные другим бюджетам")]
        BudgetOutcoming,
        [Description("Кредиты полученные от организаций")]
        OrganizationIncoming,
        [Description("Кредиты предоствленные огранизациям")]
        OrganizationOutcoming
    }

    #endregion

    /// <summary>
    /// класс для расчетов деталей кредитов
    /// </summary>
    internal class CreditServer
    {
        internal IScheme scheme;

        internal FinSourcesConstsHelper constsHelper;

        internal CreditServer(IScheme scheme)
        {
            this.scheme = scheme;
            constsHelper = new FinSourcesConstsHelper(scheme);
        }

        /// <summary>
        /// возвращает объект для расчета кредитов полученных валютных
        /// </summary>
        /// <returns></returns>
        public static CreditServer GetCurrencyPlaningIncomesServer(IScheme scheme)
        {
            CreditServer server = new CurrencyCreditServer(scheme);
            server.SetCreditIncomesParams();
            return server;
        }

        /// <summary>
        /// возвращает объект для расчета кредитов полученных рублевых
        /// </summary>
        /// <returns></returns>
        public static CreditServer GetPlaningIncomesServer(IScheme scheme)
        {
            CreditServer server = new CreditServer(scheme);
            server.SetCreditIncomesParams();
            return server;
        }

        /// <summary>
        /// возвращает объект для расчета кредитов предоставленных валютных
        /// </summary>
        /// <returns></returns>
        public static CreditServer GetCurrensyCreditIssuedServer(IScheme scheme)
        {
            CreditServer server = new CurrencyCreditServer(scheme);
            server.SetCreditIssuedParams();
            return server;
        }

        /// <summary>
        /// возвращает объект для расчета кредитов предоставленных рублевых
        /// </summary>
        /// <returns></returns>
        public static CreditServer GetCreditIssuedServer(IScheme scheme)
        {
            CreditServer server = new CreditServer(scheme);
            server.SetCreditIssuedParams();
            return server;
        }

        protected static CreditType GetCreditType(DataRow creditRow)
        {
            int statusPlan = Convert.ToInt32(creditRow["RefSStatusPlan"]);
            bool isIncome = Convert.ToInt32(creditRow["RefSTypeCredit"]) == 0 ||
                            Convert.ToInt32(creditRow["RefSTypeCredit"]) == 1;
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

        #region ключи объектов для работы с договорами (детали, ассоциации)
        /// <summary>
        /// мастер-таблица
        /// </summary>
        internal string f_S_Сredit_Key;

        /// <summary>
        /// деталь план привлечения
        /// </summary>
        internal string t_S_PlanAttractKey;

        /// <summary>
        /// ассоциация договор-план привлечения
        /// </summary>
        internal string a_S_PlanAttract_RefCredit_Key;

        /// <summary>
        /// деталь план погашения основного долга
        /// </summary>
        internal string t_S_PlanDebt_Key;

        /// <summary>
        /// ассоциация договор-план погашения основного долга
        /// </summary>
        internal string a_S_PlanDebt_RefCredit_Key;

        /// <summary>
        /// деталь план обслуживания долга
        /// </summary>
        internal string t_S_PlanService_Key;
        /// <summary>
        /// ассоциция договор-план обслуживания долга
        /// </summary>
        internal string a_S_PlanService_RefCredit_Key;

        /// <summary>
        /// деталь факт привлечения
        /// </summary>
        protected string t_S_FactAttractKey;

        /// <summary>
        /// ассоцифция договор-факт привлечения
        /// </summary>
        protected string a_S_FactAttract_RefCredit_Key;

        /// <summary>
        /// ассоциция договор-факт погашения основного долга
        /// </summary>
        internal string a_S_FactDebt_RefCredit_Key;

        /// <summary>
        /// ассоциция договор-факт погашения процентов
        /// </summary>
        internal string a_S_FactPercent_RefCredit_Key;

        /// <summary>
        /// ассоциция договор-начисление пени по основному долгу
        /// </summary>
        internal string a_S_ChargePenaltyDebt_RefCredit_Key;

        /// <summary>
        /// деталь начисления пени по основному долгу
        /// </summary>
        internal string t_S_ChargePenaltyDebt_Key;

        /// <summary>
        /// деталь начисления пени по процентам
        /// </summary>
        internal string t_S_ChargePenaltyPercent_Key;

        /// <summary>
        /// ассоциция договор-начисление пени по процентам
        /// </summary>
        internal string a_S_ChargePenaltyPercent_RefCredit_Key;

        /// <summary>
        /// деталь журнал процентов
        /// </summary>
        internal string t_S_JournalPercent_Key;

        /// <summary>
        /// ассоциация договор-журнал процентов
        /// </summary>
        internal string a_S_JournalPercent_RefCredit_Key;

        private void SetCreditIncomesParams()
        {
            f_S_Сredit_Key = SchemeObjectsKeys.f_S_Сreditincome_Key;
            t_S_PlanAttractKey = SchemeObjectsKeys.t_S_PlanAttractCI_Key;
            a_S_PlanAttract_RefCredit_Key = SchemeObjectsKeys.a_S_PlanAttractCI_RefCreditInc_Key;
            t_S_PlanDebt_Key = SchemeObjectsKeys.t_S_PlanDebtCI_Key;
            a_S_PlanDebt_RefCredit_Key = SchemeObjectsKeys.a_S_PlanDebtCI_RefCreditInc_Key;
            t_S_PlanService_Key = SchemeObjectsKeys.t_S_PlanServiceCI_Key;
            a_S_PlanService_RefCredit_Key = SchemeObjectsKeys.a_S_PlanServiceCI_RefCreditInc_Key;
            t_S_FactAttractKey = SchemeObjectsKeys.t_S_FactAttractCI_Key;
            a_S_FactAttract_RefCredit_Key = SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key;
            a_S_FactDebt_RefCredit_Key = SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key;
            a_S_FactPercent_RefCredit_Key = SchemeObjectsKeys.a_S_FactPercentCI_RefCreditInc_Key;
            a_S_ChargePenaltyDebt_RefCredit_Key = SchemeObjectsKeys.a_S_ChargePenaltyDebtCI_RefCreditInc_Key;
            t_S_ChargePenaltyDebt_Key = SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key;
            t_S_ChargePenaltyPercent_Key = SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key;
            a_S_ChargePenaltyPercent_RefCredit_Key = SchemeObjectsKeys.a_S_ChargePenaltyPercentCI_RefCreditInc_Key;
            t_S_JournalPercent_Key = SchemeObjectsKeys.t_S_JournalPercentCI_Key;
            a_S_JournalPercent_RefCredit_Key = SchemeObjectsKeys.a_S_JournalPercentCI_RefCreditInc_Key;
        }

        private void SetCreditIssuedParams()
        {
            f_S_Сredit_Key = CreditIssuedObjectsKeys.f_S_Creditissued;
            t_S_PlanAttractKey = CreditIssuedObjectsKeys.t_S_PlanAttractCO;
            a_S_PlanAttract_RefCredit_Key = CreditIssuedObjectsKeys.a_S_PlanAttractCO_RefCreditInc_Key;
            t_S_PlanDebt_Key = CreditIssuedObjectsKeys.t_S_PlanDebtCO;
            a_S_PlanDebt_RefCredit_Key = CreditIssuedObjectsKeys.a_S_PlanDebtCO_RefCreditInc_Key;
            t_S_PlanService_Key = CreditIssuedObjectsKeys.t_S_PlanServiceCO;
            a_S_PlanService_RefCredit_Key = CreditIssuedObjectsKeys.a_S_PlanServiceCO_RefCreditInc_Key;
            t_S_FactAttractKey = CreditIssuedObjectsKeys.t_S_FactAttractCO;
            a_S_FactAttract_RefCredit_Key = CreditIssuedObjectsKeys.a_S_FactAttractCO_RefCreditInc_Key;
            a_S_FactDebt_RefCredit_Key = CreditIssuedObjectsKeys.a_S_FactDebtCO_RefCreditInc_Key;
            a_S_FactPercent_RefCredit_Key = CreditIssuedObjectsKeys.a_S_FactPercentCO_RefCreditInc_Key;
            a_S_ChargePenaltyDebt_RefCredit_Key = CreditIssuedObjectsKeys.a_S_ChargePenaltyDebtCO_RefCreditInc_Key;
            t_S_ChargePenaltyDebt_Key = CreditIssuedObjectsKeys.t_S_ChargePenaltyDebtCO;
            t_S_ChargePenaltyPercent_Key = CreditIssuedObjectsKeys.t_S_ChargePenaltyPercentCO;
            a_S_ChargePenaltyPercent_RefCredit_Key = CreditIssuedObjectsKeys.a_S_ChargePenaltyPercentCO_RefCreditInc_Key;
            t_S_JournalPercent_Key = CreditIssuedObjectsKeys.t_S_JournalPercentCO;
            a_S_JournalPercent_RefCredit_Key = CreditIssuedObjectsKeys.a_S_JournalPercentCO_RefCreditInc_Key;
        }

        #endregion

        #region получение данных по деталям договора

        public virtual Credit GetCredit(DataRow creditRow)
        {
            return new Credit(creditRow);
        }

        public DataTable GetAttractFact(int masterID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, masterID, a_S_FactAttract_RefCredit_Key);
            }
        }

        public DataTable GetAttractPlan(int masterID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, masterID, a_S_PlanAttract_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали процентов договора
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetCreditPercents(int parentID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, parentID, a_S_JournalPercent_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали плана погашения
        /// </summary>
        /// <returns></returns>
        public DataTable GetDebtPlan(int parentID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, parentID, a_S_PlanDebt_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали факта погашения
        /// </summary>
        /// <returns></returns>
        public DataTable GetDebtFact(int parentID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, parentID, a_S_FactDebt_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных плана обслуживания долга
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetServicePlan(int parentID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, parentID, a_S_PlanService_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных факта погашения процентов
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetPercentFact(int parentID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, parentID, a_S_FactPercent_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали начисления пени по основному долгу
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetDebtPenaltyTable(int parentID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, parentID, a_S_ChargePenaltyDebt_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали начисления пени по процентам
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetPercentPenaltyTable(int parentID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, parentID, a_S_ChargePenaltyPercent_RefCredit_Key);
            }
        }

        public DataTable GetPercentJournal(int masterID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(scheme, db, masterID, a_S_JournalPercent_RefCredit_Key);
            }
        }

        #endregion

        #region план привлечения

        /// <summary>
        /// Запонтение таблицы "План привлечения".
        /// </summary>
        public void CalcAttractionPlan(Credit credit, int baseYear)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanAttractKey);

            string refMasterFieldName = entity.Associations[a_S_PlanAttract_RefCredit_Key].RoleDataAttribute.Name;
            IDbDataParameter[] prms = new IDbDataParameter[1];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                prms[0] = new System.Data.OleDb.OleDbParameter(refMasterFieldName, credit.ID);

                int sourceID = Utils.GetDataSourceID(scheme, db, "ФО", 29, 8, baseYear);

                using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    if (dt.Rows.Count > 0)
                        return;
                        //throw new FinSourcePlanningException(
                        //    "План привлечения уже заполнен. Очистите данные на вкладке \"План привлечения\" и повторите попытку.");

                    FillAttractionPlanTable(credit, dt, refMasterFieldName);

                    int refKif = constsHelper.GetCreditClassifierRef(t_S_PlanAttractKey, SchemeObjectsKeys.d_KIF_Plan_Key,
                        sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);

                    foreach (DataRow row in dt.Rows)
                    {
                        row["ID"] = entity.GetGeneratorNextValue;
                        row["RefKIF"] = refKif;
                    }

                    du.Update(ref dt);
                }
            }
        }

        /// <summary>
        /// Заполнение таблицы "План привлечения".
        /// </summary>
        internal virtual void FillAttractionPlanTable(Credit credit, DataTable dt, string refMasterFieldName)
        {
            DataRow row = dt.NewRow();
            row["StartDate"] = credit.StartDate;
            row["EndDate"] = credit.EndDate;
            row["Sum"] = credit.Sum;
            row["RefOKV"] = credit.RefOKV;
            row[refMasterFieldName] = credit.ID;
            dt.Rows.Add(row);
        }

        #endregion

        #region План погашения основного долга

        /// <summary>
        /// Формула расчета периодической выплаты основной суммы долга.
        /// </summary>
        /// <param name="S">Сумма основного долга.</param>
        /// <param name="i">Годовая процентная ставка.</param>
        /// <param name="r">Коэффициент корректировки процентной ставки.</param>
        /// <param name="n">Количество периодов.</param>
        /// <param name="k">Номер очередного платежа.</param>
        /// <returns>Сумма периодической выплаты основной суммы долга.</returns>
        protected static decimal CalcPeriodPayMainDebt(decimal S, decimal i, decimal r, int n, int k)
        {
            return ((1 + i * r * (n - k + 1)) / n) * S;
        }

        /// <summary>
        /// Вычисление плана погашения основного долга.
        /// </summary>
        public void CalculateAcquittanceMainPlan(Credit credit, int baseYear, DateTime startDate,
            DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanDebt_Key);

            string refMasterFieldName = entity.Associations[a_S_PlanDebt_RefCredit_Key].RoleDataAttribute.Name;
            IDbDataParameter[] prms = new IDbDataParameter[1];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                prms[0] = new System.Data.OleDb.OleDbParameter(refMasterFieldName, credit.ID);

                int sourceID = Utils.GetDataSourceID(scheme, db, "ФО", 29, 8, baseYear);

                using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms)
                    )
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    if (dt.Rows.Count > 0)
                        return;
                        //throw new FinSourcePlanningException(
                        //    "План погашения основного долга уже заполнен. Очистите таблицу и повторите попытку.");

                    FillAcquittanceMainPlanTable(credit, dt, refMasterFieldName, startDate, endDate, payPeriodicity, payDay, periods);

                    int refKif = constsHelper.GetCreditClassifierRef(t_S_PlanDebt_Key, SchemeObjectsKeys.d_KIF_Plan_Key,
                        sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);

                    foreach (DataRow row in dt.Rows)
                    {
                        row["RefKIF"] = refKif;
                        row["ID"] = entity.GetGeneratorNextValue;
                    }

                    du.Update(ref dt);
                }
            }
        }

        /// <summary>
        /// заполнение плана погашения основного долга
        /// </summary>
        internal virtual void FillAcquittanceMainPlanTable(Credit credit, DataTable dt, string refMasterFieldName,
            DateTime startDate, DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods)
        {
            int periodCount = Utils.GetPeriodCount(startDate, endDate, payPeriodicity, payDay, periods, credit.ChargeFirstDay);
            int subtract = -Utils.GetSubtractValue(payPeriodicity);

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
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
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0)
            {
                endPeriod = Utils.GetEndPeriod(startPeriod, payPeriodicity, payDay, subtract);
                if (endPeriod > endDate) endPeriod = endDate;
                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(credit.Sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["RefOKV"] = credit.RefOKV;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
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

                if (endPeriod > endDate) endPeriod = endDate;

                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(credit.Sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["RefOKV"] = credit.RefOKV;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            decimal sum = 0;
            foreach (DataRow row in dt.Select(string.Format("Payment < {0}", i)))
            {
                sum += Convert.ToDecimal(row["Sum"]);
            }
            if (dt.Select(string.Format("Payment = {0}", i)).Length > 0)
                dt.Select(string.Format("Payment = {0}", i))[0]["Sum"] = credit.Sum - sum;
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

        #endregion

        #region расчеты плана обслуживания долга

        internal virtual Decimal GetCreditSum(Credit credit)
        {
            return credit.Sum;
        }

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
            return percentSum;
        }

        /// <summary>
        /// расчет суммы процентов
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="startPeriod"></param>
        /// <param name="endPeriod"></param>
        /// <param name="payPeriodicity"></param>
        /// <param name="payments"></param>
        /// <param name="credit"></param>
        /// <param name="percent"></param>
        /// <param name="dtDebtPlan"></param>
        /// <returns></returns>
        protected static Decimal GetPercentPay(ref decimal sum, DateTime startPeriod, DateTime endPeriod,
            PayPeriodicity payPeriodicity, List<int> payments, Credit credit, decimal percent, DataTable dtDebtPlan)
        {
            // получим сумму, выплаченную по плану выплаты на момент начисления процентов
            DataRow[] rows = dtDebtPlan.Select(string.Format("EndDate <= '{0}'", startPeriod));
            if (payPeriodicity == PayPeriodicity.Single)
                startPeriod = credit.StartDate;

            foreach (DataRow row in rows)
            {
                if (!payments.Contains(Convert.ToInt32(row["ID"])))
                {
                    payments.Add(Convert.ToInt32(row["ID"]));
                    sum -= Convert.ToDecimal(row["Sum"]);
                }
            }

            return payPeriodicity == PayPeriodicity.Single ?
                    GetPercents(credit.Sum, credit.StartDate, endPeriod, percent) :
                    GetPercents(sum, startPeriod, endPeriod, percent);
        }

        /// <summary>
        /// Вытягиваем из БД все записи из журнала процентов для данного кредита
        /// </summary>
        /// <param name="creditID"></param>
        private DataTable GetJournalPercentTable(int creditID)
        {
            // Получаем годовую процентную ставку из детали "Журнал ставок процентов"
            // ИФ.Журнал ставок процентов
            IEntity t_S_JournalPercent_Entity =
                scheme.RootPackage.FindEntityByName(t_S_JournalPercent_Key);
            // ИФ.Журнал ставок процентов -> ИФ.Кредиты полученные
            string refMasterFieldNameJournal =
                t_S_JournalPercent_Entity.Associations[a_S_JournalPercent_RefCredit_Key].RoleDataAttribute.Name;

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable percentTable = (DataTable)db.ExecQuery(String.Format(
                    "select CreditPercent, ChargeDate from {0} where {1} = {2}",
                    t_S_JournalPercent_Entity.FullDBName, refMasterFieldNameJournal, creditID),
                    QueryResultTypes.DataTable);
                return percentTable;
            }
        }

        /// <summary>
        /// план обслуживания долга(упрощеный случай - без вариантов выплаты, пользовательского периода и обновления записей БД)
        /// </summary>
        public DataTable CalcDebtServicePlanSingle(Credit credit, PayPeriodicity payPeriodicity, int baseYear,
            DateTime startDate, DateTime endDate, int payDay, bool useOnlyFact)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanService_Key);
            PercentTable = GetJournalPercentTable(credit.ID);
            using (IDataUpdater du = entity.GetDataUpdater("1 = -1", null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);
                // при незаполненной детали процентов расчитывать ничего не будем, возвращаем пустую таблицу
                if (PercentTable.Rows.Count == 0)
                    return dt;
                FillPlanService(credit, dt, startDate, endDate, payPeriodicity, payDay, null, useOnlyFact);
                return dt;
            }
        }

        /// <summary>
        /// план обслуживания долга(упрощеный случай - без вариантов выплаты, пользовательского периода и обновления записей БД)
        /// </summary>
        public DataTable CalcDebtServicePlanSingle(Credit credit, int baseYear,
            DateTime startDate, DateTime endDate, int payDay, DataTable percentTable, bool useOnlyFact)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanService_Key);
            this.PercentTable = percentTable;
            using (IDataUpdater du = entity.GetDataUpdater("1 = -1", null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);
                // при незаполненной детали процентов расчитывать ничего не будем, возвращаем пустую таблицу
                if (PercentTable.Rows.Count == 0)
                    return dt;
                FillPlanService(credit, dt, startDate, endDate, PayPeriodicity.Year, payDay, null, useOnlyFact);
                return dt;
            }
        }

        //private FileStream stream;
        //private StreamWriter writer;

        /// <summary>
        /// план обслуживания долга
        /// </summary>
        public void CalcDebtServicePlan(Credit credit, int baseYear,
            DateTime startDate, DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanService_Key);
            string refMasterFieldName = entity.Associations[a_S_PlanService_RefCredit_Key].RoleDataAttribute.Name;
            // получаем ссылки на классификаторы
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                int sourceID = Utils.GetDataSourceID(scheme, db, "ФО", 29, 29, baseYear);

                DataTable percentTable = GetJournalPercentTable(credit.ID);
                if (percentTable.Rows.Count == 0)
                    throw new Exception(
                        "Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"");

                PercentTable = percentTable;
                // добавляем записи в план обслуживания 
                using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = {1}", refMasterFieldName, credit.ID), null))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);

                    if (dt.Rows.Count > 0)
                        throw new Exception("План обслуживания долга уже заполнен.");

                    FillPlanService(credit, dt, startDate, endDate, payPeriodicity, payDay, periods, false);

                    int refEkr = constsHelper.GetCreditClassifierRef(t_S_PlanService_Key,
                        SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                    int refR = constsHelper.GetCreditClassifierRef(t_S_PlanService_Key,
                        SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                    int refKD = constsHelper.GetCreditClassifierRef(t_S_PlanService_Key,
                        SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);

                    foreach (DataRow row in dt.Rows)
                    {
                        row["ID"] = entity.GetGeneratorNextValue;
                        if (credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.BudgetOutcoming ||
                            credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.OrganizationOutcoming)
                            row["RefKD"] = refKD;
                        else
                        {
                            row["RefEKR"] = refEkr;
                            row["RefR"] = refR;
                        }
                        row[refMasterFieldName] = credit.ID;
                    }
                    du.Update(ref dt);
                }
            }
        }

        private DataTable dtPercentTable;
        private DataTable PercentTable
        {
            get { return dtPercentTable; }
            set { dtPercentTable = value; }
        }

        /// <summary>
        /// Расчет процентной ставки на определенный период времени
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
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
            int dayCount = ((endDate - startDate).Days + 1) == 0 ? 1 : ((endDate - startDate).Days + 1);
            return percent / dayCount;
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

        /// <summary>
        /// возвращает сумму, привлеченную до определенной даты
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        protected Decimal GetAttractSum(Credit credit, DateTime date, bool useOnlyFact)
        {
            // если данных нет никаких, возвращаем сумму самого кредита
            DataTable dtFactAttract = GetAttractFact(credit.ID);
            DataTable dtPlanAttract = GetAttractPlan(credit.ID);
            if (dtFactAttract.Rows.Count == 0 && dtPlanAttract.Rows.Count == 0)
            {
                if (credit.StartDate == date && !credit.ChargeFirstDay)
                    return 0;
                return credit.RefOKV == -1 ? credit.Sum : credit.CurrencySum;
            }
            // сравниваем суммы плановую и фактическую, привлеченные до определенного времени
            // возвращаем максимальное значение
            decimal factAttractSum = 0;

            bool hasFact = dtFactAttract.Rows.Count > 0;
            DataRow[] rows = credit.ChargeFirstDay ?
                dtFactAttract.Select(string.Format("FactDate <= '{0}'", date)) :
                dtFactAttract.Select(string.Format("FactDate < '{0}'", date));
            foreach (DataRow row in rows)
            {
                if (credit.RefOKV == -1)
                    factAttractSum += Convert.ToDecimal(row["Sum"]);
                else
                    factAttractSum += Convert.ToDecimal(row["CurrencySum"]);
            }

            decimal planAttractSum = 0;
            if (!useOnlyFact)
            {
                rows = credit.ChargeFirstDay
                           ? dtPlanAttract.Select(string.Format("StartDate <= '{0}'", date))
                           : dtPlanAttract.Select(string.Format("StartDate < '{0}'", date));
                foreach (DataRow row in rows)
                {
                    if (credit.RefOKV == -1)
                        planAttractSum += Convert.ToDecimal(row["Sum"]);
                    else
                        planAttractSum += Convert.ToDecimal(row["CurrencySum"]);
                }
            }
            if (hasFact && date <= DateTime.Today)
                return factAttractSum;

            return factAttractSum > planAttractSum ? factAttractSum : planAttractSum;
        }

        /// <summary>
        ///  возвращает сумму выплаченного долга до определенной даты
        /// </summary>
        /// <returns></returns>
        protected decimal GetDebtPaymentSum(Credit credit, DateTime startDate, DateTime endDate, bool useOnlyFact)
        {
            if (credit.PretermDischarge || useOnlyFact)
            {
                // для досрочного погашения используем план погашения и факт погашения
                DataTable dtFact = GetDebtFact(credit.ID);
                DataTable dtPlan = GetDebtPlan(credit.ID);
                decimal planSum = 0;
                decimal factSum = 0;
                DataRow[] rows = dtFact.Select(string.Format("FactDate < '{0}'", endDate));
                foreach (DataRow row in rows)
                    factSum += credit.RefOKV == -1 ? Convert.ToDecimal(row["Sum"]) : Convert.ToDecimal(row["CurrencySum"]);
                if (!useOnlyFact)
                {
                    rows = dtPlan.Select(string.Format("EndDate < '{0}'", endDate));
                    foreach (DataRow row in rows)
                        planSum += credit.RefOKV == -1
                                       ? Convert.ToDecimal(row["Sum"])
                                       : Convert.ToDecimal(row["CurrencySum"]);
                }
                if (endDate <= DateTime.Today)
                    return factSum;

                return planSum > factSum ? planSum : factSum;
            }
            else
            {
                decimal paymentPlanSum = 0;
                // если нет досрочного погашения, то используем только план погашения
                DataTable dtPlan = GetDebtPlan(credit.ID);
                DataRow[] rows = dtPlan.Select(string.Format("EndDate < '{0}'", endDate));
                foreach (DataRow row in rows)
                    paymentPlanSum += credit.RefOKV == -1 ? Convert.ToDecimal(row["Sum"]) : Convert.ToDecimal(row["CurrencySum"]);
                return paymentPlanSum;
            }
        }

        /// <summary>
        /// заполнение детали план обслуживания долга
        /// </summary>
        protected virtual void FillPlanService(Credit credit, DataTable dtServicePlan,
            DateTime startDate, DateTime endDate,
            PayPeriodicity payPeriodicity, int payDay, DataTable periods, bool useOnlyFact)
        {
            int increment = -Utils.GetSubtractValue(payPeriodicity);

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = startDate;
            DateTime endPeriod = DateTime.MinValue;
            int i = 0;
            // получаем последний день месяца
            if (payDay == 0)
                payDay = DateTime.DaysInMonth(startDate.Year, startDate.Month);

            decimal percent;
            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0)
            {
                endPeriod = Utils.GetEndPeriod(startPeriod, payPeriodicity, payDay, increment);
                if (endPeriod > endDate) endPeriod = endDate;
                percent = GetPercentPeriods(startPeriod, endPeriod);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                decimal percentPay = GetPercentPay(startPeriod, endPeriod, payPeriodicity, credit, percent, useOnlyFact);
                DataRow servPayRow = dtServicePlan.NewRow();
                servPayRow.BeginEdit();
                servPayRow["Payment"] = i + 1;
                servPayRow["StartDate"] = startPeriod;
                servPayRow["EndDate"] = endPeriod;
                servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                servPayRow["CreditPercent"] = percent * 100;
                servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.AwayFromZero);
                servPayRow["PercentRate"] = credit.PercentRate;
                servPayRow["RefOKV"] = credit.RefOKV;
                servPayRow.EndEdit();
                dtServicePlan.Rows.Add(servPayRow);
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
                else
                    endPeriod = payPeriodicity == PayPeriodicity.Single ? endDate : GetEndPeriod(startPeriod, increment, payDay);
                if (endPeriod > endDate)
                    endPeriod = endDate;

                percent = GetPercentPeriods(startPeriod, endPeriod);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                decimal percentPay = GetPercentPay(startPeriod, endPeriod, payPeriodicity, credit, percent, useOnlyFact);
                // добавление записи с параметрами
                DataRow servPayRow = dtServicePlan.NewRow();
                servPayRow.BeginEdit();
                servPayRow["Payment"] = i + 1;
                servPayRow["StartDate"] = startPeriod;
                servPayRow["EndDate"] = endPeriod;
                servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                servPayRow["CreditPercent"] = percent * 100;
                servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.AwayFromZero);
                servPayRow["PercentRate"] = credit.PercentRate;
                servPayRow["RefOKV"] = credit.RefOKV;
                servPayRow.EndEdit();
                dtServicePlan.Rows.Add(servPayRow);
                startPeriod = endPeriod.AddDays(1);
                i++;
            }
        }

        protected Decimal GetPercentPay(DateTime startPeriod, DateTime endPeriod,
            PayPeriodicity payPeriodicity, Credit credit, decimal percent, bool useOnlyFact)
        {
            // получим сумму, выплаченную по плану выплаты на момент начисления процентов
            List<DateTime> subPeriods = new List<DateTime>();
            DataTable dtFactDebt = GetDebtFact(credit.ID);
            DataTable dtPlanDebt = GetDebtPlan(credit.ID);
            DataTable dtPlanAttract = GetAttractPlan(credit.ID);
            DataTable dtFactAttract = GetAttractFact(credit.ID);
            DataTable dtPercents = PercentTable ?? GetPercentJournal(credit.ID);
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
                DateTime planDate = credit.ChargeFirstDay ?
                    Convert.ToDateTime(row["StartDate"]).AddDays(-1) :
                    Convert.ToDateTime(row["StartDate"]);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по факту привлечения
            rows = dtFactAttract.Select(string.Format("FactDate <= '{0}' and FactDate > '{1}'", endPeriod, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = credit.ChargeFirstDay ?
                    Convert.ToDateTime(row["FactDate"]).AddDays(-1) :
                    Convert.ToDateTime(row["FactDate"]);
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
            if (credit.PretermDischarge)
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

            if (credit.StartDate <= endPeriod && credit.StartDate >= startPeriod && !subPeriods.Contains(credit.StartDate))
                subPeriods.Add(credit.StartDate);

            if (!subPeriods.Contains(endPeriod))
                subPeriods.Add(endPeriod);
            subPeriods.Sort();
            decimal percents = 0;

            foreach (DateTime date in subPeriods)
            {
                endPeriod = date;
                decimal sum = GetAttractSum(credit, endPeriod, useOnlyFact);
                decimal debtSum = GetDebtPaymentSum(credit, startPeriod, endPeriod, useOnlyFact);
                sum -= debtSum;
                // получаем ставку процентов из журнала процентных ставок
                percent = GetPercent(credit, dtPercents, endPeriod);
                // получаем сумму процентов, которые уже попадают в таблицу
                percent = GetPercents(sum, startPeriod, endPeriod, percent);
                percents += percent;
                startPeriod = endPeriod.AddDays(1);
            }
            return percents;
        }

        #endregion

        #region заполнение журнала процентных ставок

        /// <summary>
        /// Расчет ставок процентов.
        /// </summary>
        private void FillPercentsTable(DataTable dt, DataRow journalCBRow, Credit ci, decimal creditPercent, decimal ratePenalty, decimal ratePenPercent)
        {
            // Если запись на дату уже есть, то берем ее, иначе создаем новую
            DataRow[] rows = dt.Select(String.Format("ChargeDate = '{0}'", journalCBRow["InputDate"]));
            DataRow row = rows.Length == 0 ? dt.NewRow() : rows[0];

            decimal percentCB = Convert.ToDecimal(journalCBRow["PercentRate"]);
            object date = journalCBRow["InputDate"];
            if (ratePenalty < 0 && ratePenPercent < 0)
                date = ci.StartDate;
            row["ChargeDate"] = date;
            row["CreditPercent"] = creditPercent;
            row["PenaltyDebt"] = ratePenalty >= 0 ? (object)(ratePenalty * percentCB) : DBNull.Value;
            row["PenaltyPercent"] = ratePenPercent >= 0 ? (object)(ratePenPercent * percentCB) : DBNull.Value;

            if (row.RowState == DataRowState.Detached)
                dt.Rows.Add(row);
        }

        /// <summary>
        /// Расчет ставок процентов для кредитных договоров.
        /// </summary>
        /// <param name="masterRows">
        /// Строки кредитных договоров, для которых необходимо рассчитать ставки процентов.</param>
        /// <param name="d_S_JournalCB_RowID">
        /// ID записи журнала процентов ЦБ по которой нужно рассчитать ставку процента.
        /// Усли ID = -1, то расчет будет производиться по всему журналу ЦБ.
        /// </param>
        /// <param name="clearJournalPercent">
        /// Если true, то ранее рассчитанные процентные ставки удаляются. 
        /// Если false, то, если Журнал ставок процентов уже заполнен, тогда произойдет исключение.
        /// </param>
        public void CalcPercents(DataRow[] masterRows, int d_S_JournalCB_RowID, bool clearJournalPercent)
        {
            // Деталь "ИФ.Журнал ставок процентов"
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_JournalPercent_Key);

            // ИФ.Журнал ставок процентов -> ИФ.Кредиты полученные
            string refMasterFieldName = entity.Associations[a_S_JournalPercent_RefCredit_Key].RoleDataAttribute.Name;

            // Классификатор "ИФ.Журнал процентов ЦБ"
            IEntity d_S_JournalCB_Entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_JournalCB_Key);

            DataTable d_S_JournalCB_Table;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                // Получаем все строки из классификатора "ИФ.Журнал процентов ЦБ"
                d_S_JournalCB_Table = (DataTable)db.ExecQuery(
                    String.Format("select ID, InputDate, PercentRate from {0}", d_S_JournalCB_Entity.FullDBName),
                    QueryResultTypes.DataTable);
                if (d_S_JournalCB_Table.Rows.Count == 0)
                    throw new Exception(String.Format("Для рассчета ставок процентов необходимо заполнить классификатор \"{0}\".", d_S_JournalCB_Entity.FullCaption));

                foreach (DataRow masterRow in masterRows)
                {
                    int masterID = Convert.ToInt32(masterRow["ID"]);
                    DateTime startDate = (DateTime)masterRow["StartDate"];
                    // DateTime endDate = masterRow["RenewalDate"] is DBNull ? (DateTime)masterRow["EndDate"] : (DateTime)masterRow["RenewalDate"];
                    decimal creditPercent = masterRow.IsNull("CreditPercent") ?
                        -1 :
                        Convert.ToDecimal(masterRow["CreditPercent"]);
                    decimal ratePenalty = masterRow.IsNull("PenaltyDebtRate") ?
                        -1 :
                        Convert.ToDecimal(masterRow["PenaltyDebtRate"]);
                    decimal ratePenPercent = masterRow.IsNull("PenaltyPercentRate") ?
                        -1 :
                        Convert.ToDecimal(masterRow["PenaltyPercentRate"]);

                    IDbDataParameter[] prms = new IDbDataParameter[1];
                    prms[0] = new System.Data.OleDb.OleDbParameter(refMasterFieldName, masterID);
                    using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                    {
                        DataTable dt = new DataTable();
                        du.Fill(ref dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (clearJournalPercent)
                            {
                                // Очищаем только в том случае, 
                                // если рассчет идет по всему журналу процентов ЦБ
                                if (d_S_JournalCB_RowID == -1)
                                    foreach (DataRow item in dt.Rows)
                                        item.Delete();
                            }
                            else
                            {
                                throw new Exception(
                                    "Журнал ставок процентов уже заполнен. Очистите таблицу и повторите попытку.");
                            }
                        }

                        Credit credit = new Credit(masterRow);
                        // берем дату, меньше даты договора, но максимально близкую к дате заключения
                        DataRow[] rows = d_S_JournalCB_Table.Select(string.Format("InputDate <= '{0}'", startDate), "InputDate ASC");
                        if (rows.Length == 0)
                        {
                            throw new Exception(
                                string.Format("В журнале ставок ЦБ не найдено ни одной ставки на {0}", startDate));
                        }
                        FillPercentsTable(dt, rows[rows.Length - 1], credit, creditPercent, ratePenalty, ratePenPercent);
                        // Проставляем значения ID и ссылку на мастер запись
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row.RowState == DataRowState.Added)
                            {
                                row["ID"] = entity.GetGeneratorNextValue;
                                row[refMasterFieldName] = masterID;
                            }
                        }

                        du.Update(ref dt);
                    }
                }
            }
        }

        /// <summary>
        /// Расчет ставок процентов для группы гредитов.
        /// </summary>
        public void CalcPercentsForCredits(int variantID, int d_S_JournalCB_RowID)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(f_S_Сredit_Key);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query =
                    "select ID, StartDate, EndDate, RenewalDate, PercentRate, PenaltyDebtRate, " +
                    "PenaltyPercentRate, CreditPercent from {0} where RefVariant = ? and RefSStatusPlan = 0";
                DataTable f_S_Сreditincome_Table = (DataTable)db.ExecQuery(
                    String.Format(query, entity.FullDBName),
                    QueryResultTypes.DataTable,
                    new System.Data.OleDb.OleDbParameter("RefVariant", variantID));

                CalcPercents(f_S_Сreditincome_Table.Select(), d_S_JournalCB_RowID, true);
            }
        }

        /// <summary>
        /// получаем процент для пени на определенную дату
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="percentDate"></param>
        /// <param name="percentName"></param>
        /// <returns></returns>
        internal Decimal GetPercent(Credit credit, DateTime percentDate, string percentName)
        {
            // Классификатор "ИФ.Журнал процентов ЦБ"
            IEntity d_S_JournalCB_Entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_JournalCB_Key);

            DataTable d_S_JournalCB_Table;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                // Получаем все строки из классификатора "ИФ.Журнал процентов ЦБ"
                d_S_JournalCB_Table = (DataTable)db.ExecQuery(String.Format("select ID, InputDate, CreditPercent from {0}",
                    d_S_JournalCB_Entity.FullDBName), QueryResultTypes.DataTable);
                if (d_S_JournalCB_Table.Rows.Count == 0)
                    throw new Exception(String.Format("Для рассчета ставок процентов необходимо заполнить классификатор \"{0}\".", d_S_JournalCB_Entity.FullCaption));

                DataRow[] rows = d_S_JournalCB_Table.Select(string.Format("InputDate <= '{0}'", percentDate), "InputDate DESC");
                if (rows.Length != 0)
                {
                    decimal percentRate = Convert.ToDecimal(rows[0]["CreditPercent"]);
                    if (percentName == "PenaltyDebt")
                        return percentRate * credit.PenaltyDebtRate;
                    return percentRate * credit.PenaltyPercentRate;
                }
                return 0;
            }
        }

        /// <summary>
        /// получаем процент для пени на определенную дату
        /// </summary>
        /// <returns></returns>
        protected static Decimal GetPercent(Credit credit, DataTable percentTable, DateTime percentDate)
        {
            DataRow[] rows = percentTable.Select(string.Format("ChargeDate <= '{0}'", percentDate), "ChargeDate DESC");
            if (rows.Length != 0)
            {
                return Convert.ToDecimal(rows[0]["CreditPercent"]) / 100;
            }
            return 0;
        }

        #endregion

        #region начисление пени

        /// <summary>
        /// Начисление пени.
        /// </summary>
        private void CalculatePenalties(Credit credit, double ratePenalty, int baseYear,
            string factKey, string planKey, string chargePenaltyKey,
            string chargePenaltyAssociationKey, string percentName)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                // Получаем данные детали "Журнал ставок процентов"
                DataTable t_S_JournalPercentCI_Table = Utils.GetDetailTable(scheme ,db, credit.ID, a_S_JournalPercent_RefCredit_Key);
                if (t_S_JournalPercentCI_Table.Rows.Count == 0)
                    throw new Exception("Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"");

                int sourceID = Utils.GetDataSourceID(scheme, db, "ФО", 29, 7, baseYear);
                // Получаем данные детали "ИФ.Факт погашения основного долга"
                DataTable t_S_FactDebtCI_Table = Utils.GetDetailTable(scheme, db, credit.ID, factKey);
                if (t_S_FactDebtCI_Table.Rows.Count == 0)
                {
                    DataRow row = t_S_FactDebtCI_Table.NewRow();
                    row["ID"] = 0;
                    row["Sum"] = 0;
                    row["CurrencySum"] = 0;
                    row["FactDate"] = credit.EndDate;
                    t_S_FactDebtCI_Table.Rows.Add(row);
                }
                // Получаем данные детали "ИФ.План погашения основного долга"
                DataTable t_S_PlanDebtCI_Table = Utils.GetDetailTable(scheme, db, credit.ID, planKey);
                /*
                #region Формируем деталь "ИФ.Начисление пени по основному долгу"

                DataTable t_S_ChargePenalty_Table = new DataTable();
                IEntity t_S_ChargePenalty_Entity = scheme.RootPackage.FindEntityByName(chargePenaltyKey);
                string refMasterFieldName = t_S_ChargePenalty_Entity.Associations[chargePenaltyAssociationKey].RoleDataAttribute.Name;

                IDbDataParameter[] prms = new IDbDataParameter[] { new System.Data.OleDb.OleDbParameter(refMasterFieldName, credit.ID) };
                using (IDataUpdater du = t_S_ChargePenalty_Entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                {
                    du.Fill(ref t_S_ChargePenalty_Table);
                    Penalties penalty = new Penalties(t_S_PlanDebtCI_Table, t_S_FactDebtCI_Table, credit.RefOKV);
                    DataRow[] planRows = t_S_PlanDebtCI_Table.Select(string.Empty, "EndDate Asc");

                    int payment = 1;
                    int refEkr = constsHelper.GetCreditClassifierRef(t_S_ChargePenalty_Entity.ObjectKey, SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType);
                    int refR = constsHelper.GetCreditClassifierRef(t_S_ChargePenalty_Entity.ObjectKey, SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType);
                    int refKD = constsHelper.GetCreditClassifierRef(t_S_PlanService_Key, SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType);
                    foreach (DataRow row in planRows)
                    {
                        // получить процент по пеням из расчета дат периода.
                        decimal penaltyRate = GetPercent(credit,
                            GetPenaltyEndPeriod(Convert.ToDateTime(row["EndDate"]),
                            credit.CreditType), percentName);
                        DateTime endPeriod = GetPenaltyEndPeriod(Convert.ToDateTime(row["EndDate"]), credit.CreditType);
                        DataRow[] penaltyRows = penalty.CalculatePenalty(endPeriod,
                            GetNextEndPeriod(endPeriod, credit.CreditType, t_S_PlanDebtCI_Table),
                            credit.CreditType, penaltyRate, t_S_ChargePenalty_Table);

                        if (penaltyRows != null)
                        {
                            foreach (DataRow penaltyRow in penaltyRows)
                            {
                                penaltyRow["ID"] = t_S_ChargePenalty_Entity.GetGeneratorNextValue;
                                penaltyRow["Payment"] = payment;
                                if (credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.BudgetOutcoming ||
                                    credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.OrganizationOutcoming)
                                    penaltyRow["RefKD"] = refKD;
                                else
                                {
                                    penaltyRow["RefEKR"] = refEkr;
                                    penaltyRow["RefR"] = refR;
                                }

                                penaltyRow["PenaltyRate"] = ratePenalty;
                                penaltyRow[refMasterFieldName] = credit.ID;
                                t_S_ChargePenalty_Table.Rows.Add(penaltyRow);
                                payment++;
                            }
                        }
                    }
                    du.Update(ref t_S_ChargePenalty_Table);
                }

                #endregion Формируем деталь "ИФ.Начисление пени по основному долгу"
                 * */
            }
        }

        /// <summary>
        /// расчитываем дату окончания периода, по которому пени не считаются (до 10 числа месяца + выходные)
        /// </summary>
        /// <param name="endPeriod"></param>
        /// <returns></returns>
        private static DateTime GetPenaltyEndPeriod(DateTime endPeriod, CreditType creditType)
        {
            if (creditType != CreditType.ActiveOutcome)
                return endPeriod;
            DateTime newEndPeriod = endPeriod;

            if (endPeriod.Day <= 10)
                // если дата конца периода меньше или равна 10 числу, то берем 10 день текущего месяца
                newEndPeriod = new DateTime(endPeriod.Year, endPeriod.Month, 10);
            else
            {
                // если больше, то берем 10 день следующего месяца
                newEndPeriod = new DateTime(endPeriod.Year, endPeriod.AddMonths(1).Month, 10);
                // для декабря берем январь следующего года
                if (endPeriod.Month == 12)
                    newEndPeriod = newEndPeriod.AddYears(1);
            }
            // учитываем субботы и воскресенья
            if (newEndPeriod.DayOfWeek == DayOfWeek.Saturday)
                newEndPeriod = newEndPeriod.AddDays(1);
            if (newEndPeriod.DayOfWeek == DayOfWeek.Friday)
                newEndPeriod = newEndPeriod.AddDays(2);
            return newEndPeriod;
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime GetNextEndPeriod(DateTime endPeriod, CreditType creditType, DataTable dtPeriods)
        {
            DataRow[] rows = dtPeriods.Select(string.Format("EndDate > '{0}'", endPeriod), "EndDate Asc");
            if (rows == null || rows.Length == 0)
                return endPeriod;
            return GetPenaltyEndPeriod(Convert.ToDateTime(rows[0]["EndDate"]), creditType);
        }

        /// <summary>
        /// добавляет запись в деталь "Пени по основному долгу"
        /// </summary>
        public virtual void AddPenalty(Credit credit, bool forDebtPenalty,
            DateTime paymentDate, int baseYear, int payment, decimal sum, decimal currencySum,
            int daysLate, decimal penalty, decimal penaltyRate, decimal exchangeRate, int refOkv)
        {
            string associationKey = a_S_ChargePenaltyPercent_RefCredit_Key;
            if (forDebtPenalty)
                associationKey = a_S_ChargePenaltyDebt_RefCredit_Key;

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntityAssociation entityAssociation = scheme.RootPackage.FindAssociationByName(associationKey);
                IEntity entity = entityAssociation.RoleData;

                int sourceID = Utils.GetDataSourceID(scheme, db, "ФО", 29, 6, baseYear);
                int refEkr = constsHelper.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                int refR = constsHelper.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                int refKD = constsHelper.GetCreditClassifierRef(t_S_PlanService_Key,
                    SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);

                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
                {
                    DataTable t_S_ChargePenaltyCI_Table = new DataTable();
                    du.Fill(ref t_S_ChargePenaltyCI_Table);
                    // добавляем запись пеней
                    DataRow newRow = t_S_ChargePenaltyCI_Table.NewRow();
                    newRow.BeginEdit();
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    newRow["Payment"] = payment;
                    newRow["StartDate"] = paymentDate;//DateTime.Today;
                    newRow["Penalty"] = penalty;
                    newRow["PenaltyRate"] = penaltyRate == -1 ? DBNull.Value : (object)penaltyRate;
                    newRow["Sum"] = sum;
                    newRow["LateDate"] = daysLate;
                    newRow["RefCreditInc"] = credit.ID;
                    if (credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.BudgetOutcoming ||
                        credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.OrganizationOutcoming)
                        newRow["RefKD"] = refKD;
                    else
                    {
                        newRow["RefEKR"] = refEkr;
                        newRow["RefR"] = refR;
                    }
                    newRow["RefOKV"] = refOkv;
                    newRow.EndEdit();
                    t_S_ChargePenaltyCI_Table.Rows.Add(newRow);
                    // проставляем ссылки на классификаторы
                    du.Update(ref t_S_ChargePenaltyCI_Table);
                }
            }
        }

        /// <summary>
        /// Начисление пени по основному долгу.
        /// </summary>
        public Dictionary<string, string> CalculatePenaltiesMainDebt(DataRow[] masterRows, int baseYear, int variant)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            foreach (DataRow masterRow in masterRows)
            {
                try
                {
                    if (Convert.ToInt32(masterRow["RefSStatusPlan"]) == 0 &&
                        Convert.ToInt32(masterRow["RefVariant"]) == variant)
                    {
                        double ratePenalty = masterRow["PenaltyDebtRate"] is DBNull ? 1 : Convert.ToDouble(masterRow["PenaltyDebtRate"]);
                        Credit credit = new Credit(masterRow);
                        CalculatePenalties(credit, ratePenalty, baseYear,
                                           a_S_FactDebt_RefCredit_Key,
                                           a_S_PlanDebt_RefCredit_Key,
                                           t_S_ChargePenaltyDebt_Key,
                                           a_S_ChargePenaltyDebt_RefCredit_Key,
                                           "PenaltyDebt");
                    }
                }
                catch (Exception e)
                {
                    errors.Add(Convert.ToString(masterRow["Num"]), e.Message);
                }
            }
            return errors;
        }

        /// <summary>
        /// Начисление пени по процентам.
        /// </summary>
        public Dictionary<string, string> CalculatePenaltiesPlanService(DataRow[] masterRows, int baseYear, int variant)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            foreach (DataRow masterRow in masterRows)
            {
                try
                {
                    if (Convert.ToInt32(masterRow["RefSStatusPlan"]) == 0 &&
                        Convert.ToInt32(masterRow["RefVariant"]) == variant)
                    {
                        double ratePenalty = masterRow["PenaltyDebtRate"] is DBNull ? 1 : Convert.ToDouble(masterRow["PenaltyDebtRate"]);
                        Credit credit = new Credit(masterRow);
                        CalculatePenalties(credit, ratePenalty, baseYear,
                                           a_S_FactPercent_RefCredit_Key,
                                           a_S_PlanService_RefCredit_Key,
                                           t_S_ChargePenaltyPercent_Key,
                                           a_S_ChargePenaltyPercent_RefCredit_Key,
                                           "PenaltyPercent");
                    }
                }
                catch (Exception e)
                {
                    errors.Add(Convert.ToString(masterRow["Num"]), e.Message);
                }
            }
            return errors;
        }

        #endregion

        #region Курсовая разница

        /// <summary>
        /// Добавление записей в курсовую разницу
        /// </summary>
        public void FillCurrencyDiff(decimal diffSum, string currencyDiff, int baseYear, DateTime startPeriod, DateTime endPeriod, Credit credit)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.t_S_RateSwitchCI_Key);
            string refMasterFieldName =
                entity.Associations[SchemeObjectsKeys.a_S_RateSwitchCI_RefCreditIncome_Key].RoleDataAttribute.Name;

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                int sourceID = Utils.GetDataSourceID(scheme, db, "ФО", 29, 8, baseYear);
                int refKIF = constsHelper.GetCreditClassifierRef(entity.ObjectKey, SchemeObjectsKeys.d_KIF_Plan_Key,
                    sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                using (
                    IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = {1}", refMasterFieldName, credit.ID),
                                                            null, null))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    DataRow row = dt.NewRow();
                    row.BeginEdit();
                    row["ID"] = entity.GetGeneratorNextValue;
                    row["StartDate"] = startPeriod;
                    row["EndDate"] = endPeriod;
                    row["Sum"] = diffSum;
                    row["ExchangeRate"] = currencyDiff;
                    row["RefKIF"] = refKIF;
                    row[refMasterFieldName] = credit.ID;
                    row.EndEdit();
                    dt.Rows.Add(row);
                    du.Update(ref dt);
                }
            }
        }

        #endregion

        #region получение данных по кредитной линии

        public DataTable GetCreditLineInform(Credit credit)
        {
            DataTable dt = new DataTable();
            DataColumn column = dt.Columns.Add("ID", typeof(Int32));
            column.Caption = "№";

            column = dt.Columns.Add("Date", typeof(String));
            column.Caption = "Дата";

            column = dt.Columns.Add("DiffAttr", typeof(Decimal));
            column.Caption = "Привлечено";

            column = dt.Columns.Add("DiffDebt", typeof(Decimal));
            column.Caption = "Погашено";

            column = dt.Columns.Add("Debt", typeof(Decimal));
            column.Caption = "Ссудная задолженность";

            column = dt.Columns.Add("Rest", typeof(Decimal));
            column.Caption = "Неиспользованный лимит кредитной линии";
            FillCreditLineData(credit, ref dt);

            return dt;
        }

        /// <summary>
        /// Сливание общего списка дат изменений
        /// </summary>
        private Collection<string> FillDetailDateList(Collection<string> datesList, DataRow[] dtDetail)
        {
            Collection<string> result = datesList;
            foreach (DataRow row in dtDetail)
            {
                string dateStr = Convert.ToDateTime(row["FactDate"]).ToShortDateString();
                if (dateStr != string.Empty && result.IndexOf(dateStr) == -1)
                {
                    if (result.Count == 0)
                    {
                        result.Add(dateStr);
                    }
                    else
                    {
                        int indexCollection = 0;
                        for (int j = 0; j < result.Count; j++)
                        {
                            if (Convert.ToDateTime(result[j]) > Convert.ToDateTime(dateStr)) break;
                            indexCollection++;
                        }
                        result.Insert(indexCollection, dateStr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// получение данных по кредитной линии
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="dtCreditLine"></param>
        protected virtual void InternalFillCreditLineData(Credit credit, ref DataTable dtCreditLine, string sumFieldName)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                int refSExtension = credit.RefSExtension;
                IEntity credits = scheme.RootPackage.FindEntityByName(f_S_Сredit_Key);
                DataTable dtChilds = (DataTable)scheme.SchemeDWH.DB.ExecQuery(
                    string.Format("select id from {0} where parentid = {1}", credits.FullDBName, credit.ID),
                    QueryResultTypes.DataTable);

                Collection<int> childs = new Collection<int>();
                foreach (DataRow row in dtChilds.Rows)
                    childs.Add(Convert.ToInt32(row[0]));

                if (childs.Count == 0) childs.Add(credit.ID);

                Collection<string> dateList = new Collection<string>();
                dateList.Add(credit.StartDate.ToShortDateString());
                foreach (int childKey in childs)
                {
                    DataTable dtFactAttract = Utils.GetDetailTable(scheme, db, childKey, a_S_FactAttract_RefCredit_Key);
                    DataTable dtFactDebt = Utils.GetDetailTable(scheme, db, childKey, a_S_FactDebt_RefCredit_Key);

                    DataRow[] attractRows = dtFactAttract.Select(string.Format("FactDate < '{0}'", DateTime.Now.ToShortDateString()), "FactDate desc");
                    DataRow[] debtRows = dtFactDebt.Select(string.Format("FactDate < '{0}'", DateTime.Now.ToShortDateString()), "FactDate desc");

                    dateList = FillDetailDateList(dateList, attractRows);
                    dateList = FillDetailDateList(dateList, debtRows);
                }
                if (!dateList.Contains(DateTime.Now.ToShortDateString())) dateList.Add(DateTime.Now.ToShortDateString());

                decimal creditRemnant = credit.Sum;
                for (int i = 0; i < dateList.Count; i++)
                {
                    decimal attr = 0;
                    decimal debt = 0;
                    foreach (int childKey in childs)
                    {
                        DataTable dtFactAttract = Utils.GetDetailTable(scheme, db, childKey, a_S_FactAttract_RefCredit_Key);
                        DataTable dtFactDebt = Utils.GetDetailTable(scheme, db, childKey, a_S_FactDebt_RefCredit_Key);

                        DataRow[] attrSingleRows = dtFactAttract.Select(string.Format("FactDate = '{0}'", dateList[i]));
                        DataRow[] debtSingleRows = dtFactDebt.Select(string.Format("FactDate = '{0}'", dateList[i]));
                        // вычисляем разницу между суммой договора, привлечениями и выплатами
                        foreach (DataRow row in attrSingleRows)
                            attr += Convert.ToDecimal(row[sumFieldName]);
                        foreach (DataRow row in debtSingleRows)
                            debt += Convert.ToDecimal(row[sumFieldName]);
                    }

                    creditRemnant = creditRemnant + attr - debt;
                    // добавляем запись по каждому месяцу
                    DataRow newRow = dtCreditLine.Rows.Add();
                    newRow["ID"] = i + 1;
                    newRow["Date"] = dateList[i];
                    newRow["DiffAttr"] = attr;
                    newRow["DiffDebt"] = debt;
                    newRow["Debt"] = attr - debt;
                    if (refSExtension == 3 || refSExtension == 5 || refSExtension == 6)
                    {
                        creditRemnant = creditRemnant - attr;
                    }
                    else if (refSExtension == 4)
                    {
                        creditRemnant = creditRemnant - attr + debt;
                    }
                    newRow["Rest"] = creditRemnant;
                }
            }
        }

        /// <summary>
        /// получение данных по кредитной линии
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="dtCreditLine"></param>
        protected virtual void FillCreditLineData(Credit credit, ref DataTable dtCreditLine)
        {
            InternalFillCreditLineData(credit, ref dtCreditLine, "Sum");
        }

        #endregion

        /// <summary>
        /// получение количества дней в периоде
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        protected static int PeriodDayCount(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days;
        }

        /// <summary>
        /// получение количества дней в периоде
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        protected static int PeriodDayCount(object startDate, object endDate)
        {
            return PeriodDayCount(Convert.ToDateTime(endDate), Convert.ToDateTime(startDate));
        }
    }

    #region валютный договор

    /// <summary>
    /// договор в валюте
    /// </summary>
    internal class CurrencyCreditServer : CreditServer
    {
        internal CurrencyCreditServer(IScheme scheme)
            : base(scheme)
        {
            
        }

        #region План привлечения

        /// <summary>
        /// Запонтение таблицы "План привлечения".
        /// </summary>
        internal override void FillAttractionPlanTable(Credit credit, DataTable dt, string refMasterFieldName)
        {
            DataRow row = dt.NewRow();
            row["StartDate"] = credit.StartDate;
            row["EndDate"] = credit.EndDate;
            row["Sum"] = credit.Sum;
            row["CurrencySum"] = credit.CurrencySum;
            row["ExchangeRate"] = credit.ExchangeRate;
            row["IsPrognozExchRate"] = credit.IsPrognozExchRate ?? (object)DBNull.Value;
            row["RefOKV"] = credit.RefOKV;
            row[refMasterFieldName] = credit.ID;
            dt.Rows.Add(row);
        }

        #endregion

        #region План погашения основного долга

        /// <summary>
        /// заполнение плана погашения основного долга
        /// </summary>
        internal override void FillAcquittanceMainPlanTable(Credit credit, DataTable dt, string refMasterFieldName,
            DateTime startDate, DateTime endDate, PayPeriodicity payPeriodicity, int payDay, DataTable periods)
        {
            int periodCount = Utils.GetPeriodCount(startDate, endDate, payPeriodicity, payDay, periods, credit.ChargeFirstDay);
            int subtract = -Utils.GetSubtractValue(payPeriodicity);

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = startDate;
            DateTime endPeriod = startPeriod;

            if (payDay == 0)
            {
                // получаем последний день месяца
                payDay = 31;
            }

            int i = 0;
            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0)
            {
                endPeriod = Utils.GetEndPeriod(startPeriod, payPeriodicity, payDay, subtract);
                if (endPeriod > endDate) endPeriod = endDate;
                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(credit.Sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["CurrencySum"] = Math.Round(credit.CurrencySum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["ExchangeRate"] = credit.ExchangeRate;
                row["RefOKV"] = credit.RefOKV;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
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

                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                row["Sum"] = Math.Round(credit.Sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["CurrencySum"] = Math.Round(credit.CurrencySum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["ExchangeRate"] = credit.ExchangeRate;
                row["RefOKV"] = credit.RefOKV;
                row["IsPrognozExchRate"] = credit.IsPrognozExchRate ?? (object)DBNull.Value;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
                startPeriod = endPeriod.AddDays(1);
                i++;
            }

            decimal sum = 0;
            decimal currencySum = 0;
            foreach (DataRow row in dt.Select(string.Format("Payment < {0}", i)))
            {
                sum += Convert.ToDecimal(row["Sum"]);
                currencySum += Convert.ToDecimal(row["CurrencySum"]);
            }
            if (dt.Select(string.Format("Payment = {0}", i)).Length > 0)
            {
                dt.Select(string.Format("Payment = {0}", i))[0]["Sum"] = credit.Sum - sum;
                dt.Select(string.Format("Payment = {0}", i))[0]["CurrencySum"] = credit.CurrencySum - currencySum;
            }
        }

        #endregion

        #region расчеты плана обслуживания долга

        /// <summary>
        /// План обслуживания долга. Начисление процентов на остаток
        /// </summary>
        protected override void FillPlanService(Credit credit, DataTable dtServicePlan,
            DateTime startDate, DateTime endDate,
            PayPeriodicity payPeriodicity, int payDay, DataTable periods, bool useOnlyFact)
        {
            int increment = -Utils.GetSubtractValue(payPeriodicity);
            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = startDate;
            DateTime endPeriod = DateTime.MinValue;

            if (payDay == 0)
                payDay = DateTime.DaysInMonth(startDate.Year, startDate.Month);

            int i = 0;
            decimal percent;
            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0)
            {
                endPeriod = Utils.GetEndPeriod(startPeriod, payPeriodicity, payDay, increment);
                //if (!credit.ChargeFirstDay)
                //   startPeriod = startPeriod.AddDays(1);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                percent = GetPercentPeriods(startPeriod, endPeriod);

                decimal percentCurrencyPay = GetPercentPay(startPeriod, endPeriod, payPeriodicity, credit, percent, useOnlyFact);
                decimal percentPay = Math.Round(percentCurrencyPay, 2, MidpointRounding.AwayFromZero) * credit.ExchangeRate;

                DataRow servPayRow = dtServicePlan.NewRow();
                servPayRow.BeginEdit();
                servPayRow["Payment"] = i + 1;
                servPayRow["StartDate"] = startPeriod;
                servPayRow["EndDate"] = endPeriod;
                servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                servPayRow["CreditPercent"] = percent * 100;
                servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.AwayFromZero);
                servPayRow["PercentRate"] = credit.PercentRate;
                servPayRow["CurrencySum"] = Math.Round(percentCurrencyPay, 2, MidpointRounding.AwayFromZero);
                servPayRow["ExchangeRate"] = credit.ExchangeRate;
                servPayRow["RefOKV"] = credit.RefOKV;
                servPayRow["IsPrognozExchRate"] = credit.IsPrognozExchRate ?? (object)DBNull.Value;
                servPayRow.EndEdit();
                dtServicePlan.Rows.Add(servPayRow);
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
                else endPeriod = payPeriodicity == PayPeriodicity.Single ? endDate : GetEndPeriod(startPeriod, increment, payDay);

                if (endPeriod > endDate)
                    endPeriod = endDate;

                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                percent = GetPercentPeriods(startPeriod, endPeriod);
                decimal percentCurrencyPay = GetPercentPay(startPeriod, endPeriod, payPeriodicity, credit, percent, useOnlyFact);
                decimal percentPay = Math.Round(percentCurrencyPay, 2, MidpointRounding.AwayFromZero) * credit.ExchangeRate;

                // добавление записи с параметрами
                DataRow servPayRow = dtServicePlan.NewRow();
                servPayRow.BeginEdit();
                servPayRow["Payment"] = i + 1;
                servPayRow["StartDate"] = startPeriod;
                servPayRow["EndDate"] = endPeriod;
                servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                servPayRow["CreditPercent"] = percent * 100;
                servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.AwayFromZero);
                servPayRow["PercentRate"] = credit.PercentRate;
                servPayRow["CurrencySum"] = Math.Round(percentCurrencyPay, 2, MidpointRounding.AwayFromZero);
                servPayRow["ExchangeRate"] = credit.ExchangeRate;
                servPayRow["RefOKV"] = credit.RefOKV;
                servPayRow["IsPrognozExchRate"] = credit.IsPrognozExchRate ?? (object)DBNull.Value;
                servPayRow.EndEdit();
                dtServicePlan.Rows.Add(servPayRow);
                startPeriod = endPeriod.AddDays(1);
                i++;
            }
        }

        #endregion

        #region Процентная ставка

        #endregion

        #region Пени

        /// <summary>
        /// добавляет запись в деталь "Пени по основному долгу"
        /// </summary>
        public override void AddPenalty(Credit credit, bool forDebtPenalty,
            DateTime paymentDate, int baseYear, int payment, decimal sum, decimal currencySum,
            int daysLate, decimal penalty, decimal penaltyRate, decimal exchangeRate, int refOkv)
        {
            string associationKey = a_S_ChargePenaltyPercent_RefCredit_Key;
            if (forDebtPenalty)
                associationKey = a_S_ChargePenaltyDebt_RefCredit_Key;

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntityAssociation entityAssociation =
                    scheme.RootPackage.FindAssociationByName(associationKey);
                IEntity entity = entityAssociation.RoleData;

                int sourceID = Utils.GetDataSourceID(scheme, db, "ФО", 29, 7, baseYear);
                // Получаем ссылку на классификатор "ЭКР.Планирование"
                int refEkr = constsHelper.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                int refR = constsHelper.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                int refKD = constsHelper.GetCreditClassifierRef(t_S_PlanService_Key,
                    SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);

                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null, null))
                {
                    DataTable t_S_ChargePenaltyCI_Table = new DataTable();
                    du.Fill(ref t_S_ChargePenaltyCI_Table);
                    DataRow newRow = t_S_ChargePenaltyCI_Table.NewRow();

                    newRow.BeginEdit();
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    newRow["Payment"] = payment;
                    newRow["StartDate"] = DateTime.Today;
                    newRow["Penalty"] = penalty;
                    newRow["PenaltyRate"] = penaltyRate == -1 ? DBNull.Value : (object)penaltyRate;
                    newRow["Sum"] = sum;
                    newRow["CurrencySum"] = currencySum;
                    newRow["ExchangeRate"] = exchangeRate;
                    newRow["LateDate"] = daysLate;
                    newRow["RefOKV"] = refOkv;
                    if (credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.BudgetOutcoming || 
                        credit.CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.OrganizationOutcoming)
                        newRow["RefKD"] = refKD;
                    else
                    {
                        newRow["RefEKR"] = refEkr;
                        newRow["RefR"] = refR;
                    }
                    newRow["RefCreditInc"] = credit.ID;
                    newRow["IsPrognozExchRate"] = false;
                    newRow.EndEdit();

                    t_S_ChargePenaltyCI_Table.Rows.Add(newRow);
                    du.Update(ref t_S_ChargePenaltyCI_Table);
                }
            }
        }

        #endregion

        /// <summary>
        /// получение данных по кредитной линии
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="dtCreditLine"></param>
        protected override void FillCreditLineData(Credit credit, ref DataTable dtCreditLine)
        {
            InternalFillCreditLineData(credit, ref dtCreditLine, "CurrencySum");
        }
    }

    #endregion

    /// <summary>
    /// общий класс для кредитов
    /// </summary>
    public class Credit
    {
        private DataRow row;
        private IScheme scheme;

        public Credit(DataRow cretidRow)
        {
            row = cretidRow;
            //this.scheme = scheme;
            refTerrType = -1;
        }

        public Credit(DataRow cretidRow, IScheme scheme)
        {
            row = cretidRow;
            this.scheme = scheme;
            refTerrType = -1;
        }

        public int ID
        {
            get { return Convert.ToInt32(row["ID"]); }
        }

        /// <summary>
        /// Дата заключения.
        /// </summary>
        public DateTime StartDate
        {
            get { return Convert.ToDateTime(row["StartDate"]); }
        }

        /// <summary>
        /// Дата закрытия/пролонгирования
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                if (RenewalDate != DateTime.MinValue)
                    return RenewalDate;
                DateTime endDate;
                if (DateTime.TryParse(row["EndDate"].ToString(), out endDate))
                    return endDate;
                throw new Exception(
                    "Поле \"Дата окончательного погашения кредита\" не заполнено. Заполните это поле и повторите попытку");
            }
        }

        /// <summary>
        /// Дата пролонгирования.
        /// </summary>
        public DateTime RenewalDate
        {
            get
            {
                if (row.IsNull("RenewalDate"))
                    return DateTime.MinValue;
                return Convert.ToDateTime(row["RenewalDate"]);
            }
        }

        /// <summary>
        /// Общая сумма договора в рублях.
        /// </summary>
        public decimal Sum
        {
            get
            {
                return Math.Round(Convert.ToDecimal(row["Sum"]), 2, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Сумма договора в валюте.
        /// </summary>
        public decimal CurrencySum
        {
            get
            {
                return row["CurrencySum"] is DBNull ?
                    0 :
                    Math.Round(Convert.ToDecimal(row["CurrencySum"]), 2, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Доли от ставок ЦБ_процент.
        /// </summary>
        public decimal PercentRate
        {
            get { return row["PercentRate"] is DBNull ? 0 : Convert.ToDecimal(row["PercentRate"]); }
        }

        /// <summary>
        /// Доли от ставок ЦБ_пеня по основному долгу.
        /// </summary>
        public decimal PenaltyDebtRate
        {
            get { return row["PenaltyDebtRate"] is DBNull ? 0 : Convert.ToDecimal(row["PenaltyDebtRate"]); }
        }

        /// <summary>
        /// Доли от ставок ЦБ_пеня по процентам.
        /// </summary>
        public decimal PenaltyPercentRate
        {
            get { return row["PenaltyPercentRate"] is DBNull ? 0 : Convert.ToDecimal(row["PenaltyPercentRate"]); }
        }

        /// <summary>
        /// процент кредита
        /// </summary>
        public decimal Percent
        {
            get { return Convert.ToDecimal(row["CreditPercent"]); }
        }

        /// <summary>
        /// тип кредита
        /// </summary>
        public int RefSTypeCredit
        {
            get { return Convert.ToInt32(row["RefSTypeCredit"]); }
        }

        /// <summary>
        /// ссылка на валюту
        /// </summary>
        public int RefOKV
        {
            get { return Convert.ToInt32(row["RefOKV"]); }
        }

        /// <summary>
        /// Курс валюты.
        /// </summary>
        public decimal ExchangeRate
        {
            get
            {
                return row["ExchangeRate"] is DBNull ? 0 : Convert.ToDecimal(row["ExchangeRate"]);
            }
        }

        /// <summary>
        /// прогнозный курс валюты 
        /// </summary>
        public bool? IsPrognozExchRate
        {
            get
            {
                if (row.IsNull("IsPrognozExchRate"))
                    return null;
                return Convert.ToBoolean(row["IsPrognozExchRate"]);
            }
        }

        /// <summary>
        /// Досрочное погашение
        /// </summary>
        public Boolean PretermDischarge
        {
            get { return Convert.ToBoolean(row["PretermDischarge"]); }
        }

        /// <summary>
        /// начисление процентов с первого дня
        /// </summary>
        public Boolean ChargeFirstDay
        {
            get { return Convert.ToBoolean(row["ChargeFirstDay"]); }
        }

        public CreditType CreditType
        {
            get
            {
                int statusPlan = Convert.ToInt32(row["RefSStatusPlan"]);
                bool isIncome = Convert.ToInt32(row["RefSTypeCredit"]) == 0 ||
                                Convert.ToInt32(row["RefSTypeCredit"]) == 1;
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

        public ServerLibrary.FinSourcePlanning.CreditsTypes CreditsType
        {
            get
            {
                int creditType = Convert.ToInt32(row["RefSTypeCredit"]);
                switch (creditType)
                {
                    case 1:
                        return ServerLibrary.FinSourcePlanning.CreditsTypes.BudgetIncoming;
                    case 0:
                        return ServerLibrary.FinSourcePlanning.CreditsTypes.OrganizationIncoming;
                    case 3:
                        return ServerLibrary.FinSourcePlanning.CreditsTypes.BudgetOutcoming;
                    case 4:
                        return ServerLibrary.FinSourcePlanning.CreditsTypes.OrganizationOutcoming;
                    default :
                        return ServerLibrary.FinSourcePlanning.CreditsTypes.Unknown;
                }
            }
        }

        private int refTerrType;
        public int RefTerrType
        {
            get
            {
                if (refTerrType == -1)
                    using (IDatabase db = scheme.SchemeDWH.DB)
                    {
                        IEntity entity = CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.BudgetIncoming ||
                            CreditsType == ServerLibrary.FinSourcePlanning.CreditsTypes.OrganizationIncoming
                            ? scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key)
                            : scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Creditissued_Key);
                        IEntity regionEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Regions_Plan);

                        object queryresult =
                            db.ExecQuery(
                                string.Format("select reg.REFTERR from {0} reg, {1} credit where credit.ID = {2} and credit.RefRegions = reg.ID",
                                regionEntity.FullDBName, entity.FullDBName, ID),
                                QueryResultTypes.Scalar);
                        if (!(queryresult is DBNull))
                            refTerrType = Convert.ToInt32(queryresult);
                    }
                return refTerrType;
            }
        }

        /// <summary>
        /// Периодичность уплаты осн. долга
        /// </summary>
        public PayPeriodicity PeriodDebt
        {
            get
            {
                return GetPeriodicity(Convert.ToInt32(row["RefPeriodDebt"]));
            }
        }

        /// <summary>
        /// Периодичность уплаты процентов
        /// </summary>
        public PayPeriodicity PeriodRate
        {
            get
            {
                return GetPeriodicity(Convert.ToInt32(row["RefPeriodRate"]));
            }
        }

        private static PayPeriodicity GetPeriodicity(int period)
        {
            switch (period)
            {
                case 1:
                    return PayPeriodicity.Month;
                case 2:
                    return PayPeriodicity.Quarter;
                case 3:
                    return PayPeriodicity.HalfYear;
                case 4:
                    return PayPeriodicity.Year;
                case 5:
                    return PayPeriodicity.Other;
            }
            return PayPeriodicity.Single;
        }

        /// <summary>
        /// Способ предоставления кредита
        /// </summary>
        public int RefSExtension
        {
            get
            {
                return Convert.ToInt32(row["RefSExtension"]);
            }
        }

        /// <summary>
        /// код программы
        /// </summary>
        public string ProgramCode
        {
            get
            {
                if (row.Table.Columns.Contains("ProgramCode"))
                    return row.IsNull("ProgramCode") ? string.Empty : row["ProgramCode"].ToString();
                return string.Empty;
            }
        }

    }
}
