using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{

    #region перечисления

    public enum PercentRestRound
    {
        No = 0,
        OnAccumulation = 1,
        InLastPayment = 2
    }

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

    public enum DayCorrection
    {
        /// <summary>
        /// коррекция на день назад
        /// </summary>
        LastDay = -1,
        /// <summary>
        /// без коррекции
        /// </summary>
        NoCorrection = 0,
        /// <summary>
        /// коррекция на день вперед
        /// </summary>
        NextDay = 1
    }

    #endregion

    /// <summary>
    /// класс для расчетов деталей кредитов
    /// </summary>
    public partial class FinSourcePlanningServer
    {
        protected FinSourcePlanningServer()
        {
            Scheme = WorkplaceSingleton.Workplace.ActiveScheme;
        }

        public IScheme Scheme
        {
            get; set;
        }

        /// <summary>
        /// возвращает объект для расчета кредитов полученных валютных
        /// </summary>
        /// <returns></returns>
        public static FinSourcePlanningServer GetCurrencyPlaningIncomesServer()
        {
            FinSourcePlanningServer server = new CurrencyCreditServer();
            server.SetCreditIncomesParams();
            return server;
        }

        /// <summary>
        /// возвращает объект для расчета кредитов полученных рублевых
        /// </summary>
        /// <returns></returns>
        public static FinSourcePlanningServer GetPlaningIncomesServer()
        {
            FinSourcePlanningServer server = new FinSourcePlanningServer();
            server.SetCreditIncomesParams();
            return server;
        }

        /// <summary>
        /// возвращает объект для расчета кредитов предоставленных валютных
        /// </summary>
        /// <returns></returns>
        public static FinSourcePlanningServer GetCurrensyCreditIssuedServer()
        {
            FinSourcePlanningServer server = new CurrencyCreditServer();
            server.SetCreditIssuedParams();
            return server;
        }

        /// <summary>
        /// возвращает объект для расчета кредитов предоставленных рублевых
        /// </summary>
        /// <returns></returns>
        public static FinSourcePlanningServer GetCreditIssuedServer()
        {
            FinSourcePlanningServer server = new FinSourcePlanningServer();
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
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, masterID, a_S_FactAttract_RefCredit_Key);
            }
        }

        public DataTable GetAttractPlan(int masterID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, masterID, a_S_PlanAttract_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали процентов договора
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetCreditPercents(int parentID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, a_S_JournalPercent_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали плана погашения
        /// </summary>
        /// <returns></returns>
        public DataTable GetDebtPlan(int parentID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, a_S_PlanDebt_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали плана погашения
        /// </summary>
        /// <returns></returns>
        public DataTable GetDebtPlan(int parentID, VersionParams debtVersion)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return debtVersion != null
                           ? Utils.GetDetailTable(db, parentID, a_S_PlanDebt_RefCredit_Key, debtVersion)
                           : Utils.GetDetailTable(db, parentID, a_S_PlanDebt_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали факта погашения
        /// </summary>
        /// <returns></returns>
        public DataTable GetDebtFact(int parentID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, a_S_FactDebt_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных плана обслуживания долга
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetServicePlan(int parentID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, a_S_PlanService_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных факта погашения процентов
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetPercentFact(int parentID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, a_S_FactPercent_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали начисления пени по основному долгу
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetDebtPenaltyTable(int parentID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, a_S_ChargePenaltyDebt_RefCredit_Key);
            }
        }

        /// <summary>
        /// получение данных детали начисления пени по процентам
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable GetPercentPenaltyTable(int parentID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, parentID, a_S_ChargePenaltyPercent_RefCredit_Key);
            }
        }

        public DataTable GetPercentJournal(int masterID)
        {
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                return Utils.GetDetailTable(db, masterID, a_S_JournalPercent_RefCredit_Key);
            }
        }

        #endregion

        #region план привлечения

        /// <summary>
        /// Запонтение таблицы "План привлечения".
        /// </summary>
        public void CalcAttractionPlan(Credit credit, int baseYear)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;

            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanAttractKey);

            string refMasterFieldName = entity.Associations[a_S_PlanAttract_RefCredit_Key].RoleDataAttribute.Name;
            IDbDataParameter[] prms = new IDbDataParameter[1];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                prms[0] = new System.Data.OleDb.OleDbParameter(refMasterFieldName, credit.ID);

                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 8, baseYear);

                using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    if (dt.Rows.Count > 0)
                        throw new FinSourcePlanningException(
                            "План привлечения уже заполнен. Очистите данные на вкладке \"План привлечения\" и повторите попытку.");

                    FillAttractionPlanTable(credit, dt, refMasterFieldName);

                    int refKif = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanAttractKey, SchemeObjectsKeys.d_KIF_Plan_Key,
                        sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);

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
        public void CalculateAcquittanceMainPlan(Credit credit, MainDebtPaiPlanParams paiParams)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;

            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanDebt_Key);

            string refMasterFieldName = entity.Associations[a_S_PlanDebt_RefCredit_Key].RoleDataAttribute.Name;
            IDbDataParameter[] prms = new IDbDataParameter[1];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                prms[0] = new DbParameterDescriptor("p0", credit.ID);
                using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    // если данные уже такие есть, удаляем их и формируем заново
                    if (credit.CreditsType == CreditsTypes.BudgetIncoming || credit.CreditsType == CreditsTypes.OrganizationIncoming)
                    {
                        db.ExecQuery(
                            string.Format("delete from {0} where {1} = ? and EstimtDate = ? and CalcComment = ?",
                                          entity.FullDBName, refMasterFieldName), QueryResultTypes.NonQuery,
                            new DbParameterDescriptor("p0", credit.ID),
                            new DbParameterDescriptor("p1", paiParams.FormDate),
                            new DbParameterDescriptor("p2", paiParams.CalculationName));
                        dt.Rows.Clear();
                    }
                    else if (dt.Rows.Count > 0)
                    {
                        throw new FinSourcePlanningException(
                            "План погашения основного долга уже заполнен. Очистите таблицу и повторите попытку.");
                    }
                    int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 8, paiParams.BaseYear);
                    FillAcquittanceMainPlanTable(credit, dt, refMasterFieldName, paiParams);
                    int refKif = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanDebt_Key, SchemeObjectsKeys.d_KIF_Plan_Key,
                        sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);

                    foreach (DataRow row in dt.Rows)
                    {
                        row["RefKIF"] = refKif;
                        row["ID"] = entity.GetGeneratorNextValue;
                        if (credit.CreditsType == CreditsTypes.BudgetIncoming || credit.CreditsType == CreditsTypes.OrganizationIncoming)
                        {
                            row["EstimtDate"] = paiParams.FormDate;
                            row["CalcComment"] = paiParams.CalculationName;
                        }
                    }
                    du.Update(ref dt);
                }
            }
        }

        /// <summary>
        /// заполнение плана погашения основного долга
        /// </summary>
        internal virtual void FillAcquittanceMainPlanTable(Credit credit, DataTable dt, string refMasterFieldName,
            MainDebtPaiPlanParams paiParams)
        {
            decimal sum = paiParams.HasAttractionFacts ? GetFactAttractSum(credit.ID, paiParams.FormDate) - GetFactMainDebtSum(credit.ID, paiParams.FormDate) : credit.Sum;
            int periodCount = Utils.GetPeriodCount(paiParams.StartDate, paiParams.EndDate, paiParams.PayPeriodicity,
                paiParams.PayDay, paiParams.Periods, credit.ChargeFirstDay);
            int subtract = -Utils.GetSubtractValue(paiParams.PayPeriodicity);

            if (paiParams.PayPeriodicity == PayPeriodicity.Other)
            {
                paiParams.StartDate = Convert.ToDateTime(paiParams.Periods.Rows[0]["StartDate"]);
                paiParams.EndDate = Convert.ToDateTime(paiParams.Periods.Rows[paiParams.Periods.Rows.Count - 1]["EndDate"]);
            }

            DateTime endPeriod = DateTime.MinValue;
            DateTime startPeriod = paiParams.StartDate;

            if (paiParams.PayDay == 0)
            {
                // получаем последний день месяца
                paiParams.PayDay = DateTime.DaysInMonth(paiParams.StartDate.Year, paiParams.StartDate.Month);
            }

            int i = 0;
            // если конец периода не совпадает с 
            if (paiParams.PayPeriodicity != PayPeriodicity.Single && paiParams.PayDay >= 0 && paiParams.PayPeriodicity != PayPeriodicity.Other)
            {
                endPeriod = Utils.GetEndPeriod(startPeriod, paiParams.PayPeriodicity, paiParams.PayDay, subtract);
                if (endPeriod > paiParams.EndDate) endPeriod = paiParams.EndDate;
                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                if (dt.Columns.Contains("PaymentDate"))
                    row["PaymentDay"] = endPeriod;
                row["Sum"] = Math.Round(sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["RefOKV"] = credit.RefOKV;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            // идем от даты заключения до даты закрытия (пролонгирования)
            while (endPeriod < paiParams.EndDate)
            {
                if (paiParams.PayPeriodicity == PayPeriodicity.Other)
                {
                    startPeriod = Convert.ToDateTime(paiParams.Periods.Rows[i]["StartDate"]);
                    endPeriod = Convert.ToDateTime(paiParams.Periods.Rows[i]["EndDate"]);
                }
                else if (paiParams.PayPeriodicity == PayPeriodicity.Single)
                    endPeriod = paiParams.EndDate;
                else
                    endPeriod = GetEndPeriod(startPeriod, subtract, paiParams.PayDay);

                if (endPeriod > paiParams.EndDate) endPeriod = paiParams.EndDate;

                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                if (dt.Columns.Contains("PaymentDate"))
                    row["PaymentDay"] = endPeriod;
                row["Sum"] = Math.Round(sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["RefOKV"] = credit.RefOKV;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            decimal restSum = 0;
            foreach(DataRow row in dt.Select(string.Format("Payment < {0}", i)))
            {
                restSum += Convert.ToDecimal(row["Sum"]);
            }
            if (dt.Select(string.Format("Payment = {0}", i)).Length > 0)
                dt.Select(string.Format("Payment = {0}", i))[0]["Sum"] = sum - restSum;
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

        #region получение версий планов погашений основного долга

        public DataTable GetMainDebtPlans(object refCredit)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                IEntity mainDebtEntity = Scheme.RootPackage.FindEntityByName(t_S_PlanDebt_Key);
                var dt = db.ExecQuery(string.Format("select distinct EstimtDate, CalcComment from {0} where RefCreditInc = ?", mainDebtEntity.FullDBName),
                    QueryResultTypes.DataTable, new DbParameterDescriptor("p0", refCredit)) as DataTable;
                if (dt != null)
                    return dt;
                return null;
            }
        }

        public VersionParams GetMainDebtPlanVersion(object refCredit, DateTime formDate)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                IEntity mainDebtEntity = Scheme.RootPackage.FindEntityByName(t_S_PlanDebt_Key);

                var dtVersions =
                    db.ExecQuery(string.Format("select EstimtDate, CalcComment from {0} where EstimtDate in(select max(EstimtDate) from {0} where RefCreditInc = ? and EstimtDate <= ?)", mainDebtEntity.FullDBName),
                                 QueryResultTypes.DataTable,
                                 new DbParameterDescriptor("p0", refCredit),
                                 new DbParameterDescriptor("p1", formDate)) as DataTable;
                if (dtVersions != null && dtVersions.Rows.Count > 0)
                {
                    return new VersionParams(Convert.ToDateTime(dtVersions.Rows[0]["EstimtDate"]), dtVersions.Rows[0]["CalcComment"].ToString());
                }

                return null;
            }
        }

        #endregion

        #region Курсовая разница

        /// <summary>
        /// Добавление записей в курсовую разницу
        /// </summary>
        public void FillCurrencyDiff(decimal diffSum, string currencyDiff, int baseYear, DateTime startPeriod, DateTime endPeriod, Credit credit)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;

            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.t_S_RateSwitchCI_Key);
            string refMasterFieldName =
                entity.Associations[SchemeObjectsKeys.a_S_RateSwitchCI_RefCreditIncome_Key].RoleDataAttribute.Name;

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 8, baseYear);
                int refKIF = scheme.FinSourcePlanningFace.GetCreditClassifierRef(entity.ObjectKey, SchemeObjectsKeys.d_KIF_Plan_Key,
                    sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
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
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                int refSExtension = credit.RefSExtension;

                IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;
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
                    DataTable dtFactAttract = Utils.GetDetailTable(db, childKey, a_S_FactAttract_RefCredit_Key);
                    DataTable dtFactDebt = Utils.GetDetailTable(db, childKey, a_S_FactDebt_RefCredit_Key);

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
                        DataTable dtFactAttract = Utils.GetDetailTable(db, childKey, a_S_FactAttract_RefCredit_Key);
                        DataTable dtFactDebt = Utils.GetDetailTable(db, childKey, a_S_FactDebt_RefCredit_Key);

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

        internal virtual decimal GetFactAttractSum(int refCredit)
        {
            DataTable dt = GetAttractFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                sum += Convert.ToDecimal(row["Sum"]);
            }
            return sum;
        }

        internal virtual decimal GetFactAttractSum(int refCredit, DateTime formDate)
        {
            DataTable dt = GetAttractFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= formDate)
                    sum += Convert.ToDecimal(row["Sum"]);
            }
            return sum;
        }

        internal virtual decimal GetFactMainDebtSum(int refCredit)
        {
            DataTable dt = GetDebtFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                sum += Convert.ToDecimal(row["Sum"]);
            }
            return sum;
        }

        internal virtual decimal GetFactMainDebtSum(int refCredit, DateTime formDate)
        {
            DataTable dt = GetDebtFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= formDate)
                    sum += Convert.ToDecimal(row["Sum"]);
            }
            return sum;
        }
    }

    #region валютный договор

    /// <summary>
    /// договор в валюте
    /// </summary>
    public partial class CurrencyCreditServer : FinSourcePlanningServer
    {
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
            MainDebtPaiPlanParams paiParams)
        {
            decimal sum = paiParams.HasAttractionFacts ? base.GetFactAttractSum(credit.ID, paiParams.FormDate) - base.GetFactMainDebtSum(credit.ID, paiParams.FormDate) : credit.Sum;
            decimal currencySum = paiParams.HasAttractionFacts ? GetFactAttractSum(credit.ID, paiParams.FormDate) - GetFactMainDebtSum(credit.ID, paiParams.FormDate) : credit.CurrencySum;

            int periodCount = Utils.GetPeriodCount(paiParams.StartDate, paiParams.EndDate, paiParams.PayPeriodicity,
                paiParams.PayDay, paiParams.Periods, credit.ChargeFirstDay);
            int subtract = -Utils.GetSubtractValue(paiParams.PayPeriodicity);

            if (paiParams.PayPeriodicity == PayPeriodicity.Other)
            {
                paiParams.StartDate = Convert.ToDateTime(paiParams.Periods.Rows[0]["StartDate"]);
                paiParams.EndDate = Convert.ToDateTime(paiParams.Periods.Rows[paiParams.Periods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = paiParams.StartDate;
            DateTime endPeriod = startPeriod;

            if (paiParams.PayDay == 0)
            {
                // получаем последний день месяца
                paiParams.PayDay = 31;
            }

            int i = 0;
            // если конец периода не совпадает с 
            if (paiParams.PayPeriodicity != PayPeriodicity.Single && paiParams.PayDay >= 0 && paiParams.PayPeriodicity != PayPeriodicity.Other)
            {
                endPeriod = Utils.GetEndPeriod(startPeriod, paiParams.PayPeriodicity, paiParams.PayDay, subtract);
                if (endPeriod > paiParams.EndDate) endPeriod = paiParams.EndDate;
                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                if (dt.Columns.Contains("PaymentDate"))
                    row["PaymentDay"] = endPeriod;
                row["Sum"] = Math.Round(sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["CurrencySum"] = Math.Round(currencySum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["ExchangeRate"] = credit.ExchangeRate;
                row["RefOKV"] = credit.RefOKV;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            // идем от даты заключения до даты закрытия (пролонгирования)
            while (endPeriod < paiParams.EndDate)
            {
                if (paiParams.PayPeriodicity == PayPeriodicity.Other)
                {
                    startPeriod = Convert.ToDateTime(paiParams.Periods.Rows[i]["StartDate"]);
                    endPeriod = Convert.ToDateTime(paiParams.Periods.Rows[i]["EndDate"]);
                }
                else if (paiParams.PayPeriodicity == PayPeriodicity.Single)
                    endPeriod = paiParams.EndDate;
                else
                    endPeriod = GetEndPeriod(startPeriod, subtract, paiParams.PayDay);

                if (endPeriod > paiParams.EndDate)
                    endPeriod = paiParams.EndDate;

                DataRow row = dt.NewRow();
                row.BeginEdit();
                row["Payment"] = i + 1;
                row["StartDate"] = startPeriod;
                row["EndDate"] = endPeriod;
                if (dt.Columns.Contains("PaymentDate"))
                    row["PaymentDay"] = endPeriod;
                row["Sum"] = Math.Round(sum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["CurrencySum"] = Math.Round(currencySum / periodCount, 2, MidpointRounding.AwayFromZero);
                row["ExchangeRate"] = credit.ExchangeRate;
                row["RefOKV"] = credit.RefOKV;
                row["IsPrognozExchRate"] = credit.IsPrognozExchRate ?? (object)DBNull.Value;
                row[refMasterFieldName] = credit.ID;
                row.EndEdit();
                dt.Rows.Add(row);
                startPeriod = endPeriod.AddDays(1);
                i++;
            }

            decimal restSum = 0;
            decimal restCurrencySum = 0;
            foreach (DataRow row in dt.Select(string.Format("Payment < {0}", i)))
            {
                restSum += Convert.ToDecimal(row["Sum"]);
                restCurrencySum += Convert.ToDecimal(row["CurrencySum"]);
            }
            if (dt.Select(string.Format("Payment = {0}", i)).Length > 0)
            {
                dt.Select(string.Format("Payment = {0}", i))[0]["Sum"] = sum - restSum;
                dt.Select(string.Format("Payment = {0}", i))[0]["CurrencySum"] = currencySum - restCurrencySum;
            }
        }

        #endregion

        #region Процентная ставка

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

        internal override decimal GetFactAttractSum(int refCredit)
        {
            DataTable dt = GetAttractFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                sum += Convert.ToDecimal(row["CurrencySum"]);
            }
            return sum;
        }

        internal override decimal GetFactAttractSum(int refCredit, DateTime formDate)
        {
            DataTable dt = GetAttractFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= formDate)
                    sum += Convert.ToDecimal(row["CurrencySum"]);
            }
            return sum;
        }

        internal override decimal GetFactMainDebtSum(int refCredit)
        {
            DataTable dt = GetDebtFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                sum += Convert.ToDecimal(row["CurrencySum"]);
            }
            return sum;
        }

        internal override decimal GetFactMainDebtSum(int refCredit, DateTime formDate)
        {
            DataTable dt = GetDebtFact(refCredit);
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= formDate)
                    sum += Convert.ToDecimal(row["CurrencySum"]);
            }
            return sum;
        }
    }

    #endregion

    /// <summary>
    /// общий класс для кредитов
    /// </summary>
    public class Credit
    {
        internal DataRow row;

        public Credit(DataRow cretidRow)
        {
            row = cretidRow;
            refTerrType = -1;
        }

        public int ID
        {
            get { return Convert.ToInt32(row["ID"]); }
        }

        public DateTime ContractDate
        {
            get
            {
                string contractDateColumnName = "ContractDate";
                if (!row.Table.Columns.Contains(contractDateColumnName))
                    contractDateColumnName = "DocDate";
                if (row.IsNull(contractDateColumnName))
                    return StartDate;
                return Convert.ToDateTime(row[contractDateColumnName]);
            }
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
                throw new FinSourcePlanningException(
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

        public CreditsTypes CreditsType
        {
            get
            {
                int creditType = Convert.ToInt32(row["RefSTypeCredit"]);
                switch (creditType)
                {
                    case 1:
                        return CreditsTypes.BudgetIncoming;
                    case 0:
                        return CreditsTypes.OrganizationIncoming;
                    case 3:
                        return CreditsTypes.BudgetOutcoming;
                    case 4:
                        return CreditsTypes.OrganizationOutcoming;
                }
                return CreditsTypes.Unknown;
            }
        }

        private int refTerrType;
        public int RefTerrType
        {
            get
            {
                if (refTerrType == -1)
                    using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
                    {
                        IEntity entity = CreditsType == CreditsTypes.BudgetIncoming || CreditsType == CreditsTypes.OrganizationIncoming
                            ? FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key)
                            : FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Creditissued_Key);
                        IEntity regionEntity = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Regions_Plan);

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

        public decimal Commission
        {
            get
            {
                if (row.Table.Columns.Contains("Commission"))
                    return row.IsNull("Commission") ? 0 : Convert.ToDecimal(row["Commission"]);
                return 0;
            }
        }

    }


}
