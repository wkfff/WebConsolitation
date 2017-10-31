using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public partial class FinSourcePlanningServer
    {
        // расчет процентов
        #region заполнение журнала процентных ставок

        /// <summary>
        /// Расчет ставок процентов.
        /// </summary>
        private void FillPercentsTable(DataTable dt, DataRow journalCBRow, Credit ci, decimal creditPercent)
        {
            DateTime date = Convert.ToDateTime(journalCBRow["InputDate"]);
            if (Convert.ToDateTime(journalCBRow["InputDate"]) < ci.ContractDate)
                date = ci.ContractDate;
            // Если запись на дату уже есть, то берем ее, иначе создаем новую
            DataRow[] rows = dt.Select(String.Format("ChargeDate = '{0}'", date));
            // если на дату заведена другая ставка, не меняем ее
            if (rows.Length > 0)
                return;
            DataRow row = dt.NewRow();

            if (ci.PercentRate != 0)
            {
                creditPercent = Convert.ToDecimal(journalCBRow["PercentRate"]) * ci.PercentRate;
            }
            row["ChargeDate"] = date;
            row["CreditPercent"] = creditPercent;

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
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
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
                    throw new FinSourcePlanningException(String.Format("Для рассчета ставок процентов необходимо заполнить классификатор \"{0}\".", d_S_JournalCB_Entity.FullCaption));

                foreach (DataRow masterRow in masterRows)
                {
                    int masterID = Convert.ToInt32(masterRow["ID"]);
                    Credit credit = new Credit(masterRow);
                    DateTime startDate = credit.ContractDate;
                    DateTime endDate = startDate;
                    if (!masterRow.IsNull("RenewalDate") || !masterRow.IsNull("EndDate"))
                        endDate = credit.EndDate;
                    if (endDate < startDate)
                        endDate = startDate;

                    // процент по кредиту
                    decimal creditPercent = masterRow.IsNull("CreditPercent") ?
                        -1 : Convert.ToDecimal(masterRow["CreditPercent"]);

                    IDbDataParameter[] prms = new IDbDataParameter[1];
                    prms[0] = new DbParameterDescriptor(refMasterFieldName, masterID);
                    using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                    {
                        DataTable dt = new DataTable();
                        du.Fill(ref dt);
                        // берем дату, меньше даты договора, но максимально близкую к дате заключения
                        DataRow[] rows = d_S_JournalCB_Table.Select(string.Format("InputDate <= '{0}'", startDate), "InputDate ASC");
                        if (rows.Length == 0 && !masterRow.IsNull("PercentRate"))
                        {
                            throw new FinSourcePlanningException(
                                string.Format("В журнале ставок ЦБ не найдено ни одной необходимой ставки на {0}", startDate));
                        }
                        rows = d_S_JournalCB_Table.Select(string.Format("InputDate >= '{0}' and InputDate <= '{1}'",
                            Convert.ToDateTime(rows[rows.Length - 1]["InputDate"]), endDate), "InputDate ASC");
                        if (d_S_JournalCB_RowID != -1)
                        {
                            FillPercentsTable(dt, d_S_JournalCB_Table.Select(string.Format("ID = {0}", d_S_JournalCB_RowID))[0], credit, creditPercent);
                        }
                        else if (masterRow.IsNull("PercentRate"))
                        {
                            FillPercentsTable(dt, rows[0], credit, creditPercent);
                        }
                        else
                        {
                            foreach (DataRow journalCBRow in rows)
                            {
                                FillPercentsTable(dt, journalCBRow, credit, creditPercent);
                            }
                        }
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
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(f_S_Сredit_Key);
            using (IDataUpdater du = entity.GetDataUpdater("RefVariant = ? and RefSStatusPlan = 0", null, new DbParameterDescriptor("RefVariant", variantID)))
            {
                DataTable dtCredits = new DataTable();
                du.Fill(ref dtCredits);
                CalcPercents(dtCredits.Select(), d_S_JournalCB_RowID, false);
            }
        }

        /// <summary>
        /// получаем процент для пени на определенную дату
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="percentDate"></param>
        /// <param name="percentName"></param>
        /// <returns></returns>
        protected static Decimal GetPercent(Credit credit, DateTime percentDate, string percentName)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            // Классификатор "ИФ.Журнал процентов ЦБ"
            IEntity d_S_JournalCB_Entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_JournalCB_Key);

            DataTable d_S_JournalCB_Table;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                // Получаем все строки из классификатора "ИФ.Журнал процентов ЦБ"
                d_S_JournalCB_Table = (DataTable)db.ExecQuery(String.Format("select ID, InputDate, PercentRate from {0}",
                    d_S_JournalCB_Entity.FullDBName), QueryResultTypes.DataTable);
                if (d_S_JournalCB_Table.Rows.Count == 0)
                    throw new FinSourcePlanningException(String.Format("Для рассчета ставок процентов необходимо заполнить классификатор \"{0}\".", d_S_JournalCB_Entity.FullCaption));

                DataRow[] rows = d_S_JournalCB_Table.Select(string.Format("InputDate <= '{0}'", percentDate), "InputDate DESC");
                if (rows.Length != 0)
                {
                    decimal percentRate = Convert.ToDecimal(rows[0]["PercentRate"]);
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
            return credit.Percent / 100;
        }

        #endregion

        #region расчеты плана обслуживания долга

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
                    ? (endDate - startPeriod).Days + 1 : (endOfYear - startPeriod).Days + 1;
                percentSum += Math.Round(sum * percentRate * daysCount / Utils.GetYearBase(currentYear), 4, MidpointRounding.ToEven);
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
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
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
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanService_Key);

            PercentTable = GetJournalPercentTable(credit.ID);
            using (IDataUpdater du = entity.GetDataUpdater("1 = -1", null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);
                // при незаполненной детали процентов расчитывать ничего не будем, возвращаем пустую таблицу
                if (PercentTable.Rows.Count == 0)
                    return dt;
                PercentCalculationParams calculationParams = new PercentCalculationParams();
                calculationParams.StartDate = startDate;
                calculationParams.EndDate = endDate;
                calculationParams.EndPeriodDay = payDay;
                calculationParams.PaymentDayCorrection = DayCorrection.NoCorrection;
                calculationParams.EndPeriodDay = 0;
                calculationParams.PaymentsPeriodicity = PayPeriodicity.Year;
                FillPlanService(credit, dt, false, calculationParams, new Holidays(scheme));
                return dt;
            }
        }

        /// <summary>
        /// план обслуживания долга(упрощеный случай - без вариантов выплаты, пользовательского периода и обновления записей БД)
        /// </summary>
        public DataTable CalcDebtServicePlanSingle(Credit credit, int baseYear,
            DateTime startDate, DateTime endDate, int payDay, DataTable percentTable, bool useOnlyFact)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanService_Key);

            this.PercentTable = percentTable;
            using (IDataUpdater du = entity.GetDataUpdater("1 = -1", null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);
                // при незаполненной детали процентов расчитывать ничего не будем, возвращаем пустую таблицу
                if (PercentTable.Rows.Count == 0)
                    return dt;
                PercentCalculationParams calculationParams = new PercentCalculationParams();
                calculationParams.StartDate = startDate;
                calculationParams.EndDate = endDate;
                calculationParams.EndPeriodDay = payDay;
                calculationParams.PaymentDayCorrection = DayCorrection.NoCorrection;
                calculationParams.EndPeriodDay = 0;
                calculationParams.PaymentsPeriodicity = PayPeriodicity.Year;
                FillPlanService(credit, dt, false, calculationParams, new Holidays(scheme));
                return dt;
            }
        }

        /// <summary>
        /// план обслуживания долга
        /// </summary>
        public void CalcDebtServicePlan(Credit credit, int baseYear, PercentCalculationParams calculationParams)
        {
            CalcDebtServicePlan(credit, baseYear, calculationParams, true);
        }

        /// <summary>
        /// план обслуживания долга
        /// </summary>
        public void CalcDebtServicePlan(Credit credit, int baseYear, PercentCalculationParams calculationParams, bool showWarnings)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            IEntity entity = scheme.RootPackage.FindEntityByName(t_S_PlanService_Key);
            string refMasterFieldName = entity.Associations[a_S_PlanService_RefCredit_Key].RoleDataAttribute.Name;
            // получаем ссылки на классификаторы
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 29, baseYear);

                DataTable percentTable = GetJournalPercentTable(credit.ID);
                if (percentTable.Rows.Count == 0)
                {
                    CalcPercents(new DataRow[] { credit.row }, -1, false);
                    percentTable = GetJournalPercentTable(credit.ID);
                }

                PercentTable = percentTable;
                // добавляем записи в план обслуживания 
                using (IDataUpdater du = entity.GetDataUpdater(String.Format("{0} = {1}", refMasterFieldName, credit.ID), null))
                {
                    // кредиты полученные удаляем с дополнительными фильтрами
                    if (credit.CreditsType == CreditsTypes.BudgetIncoming || credit.CreditsType == CreditsTypes.OrganizationIncoming)
                    {
                        db.ExecQuery(
                            string.Format("delete from {0} where {1} = {2} and EndDate >= ? and EstimtDate = ? and CalcComment = ?",
                            entity.FullDBName, refMasterFieldName, credit.ID), QueryResultTypes.NonQuery,
                            new DbParameterDescriptor("p0", calculationParams.StartDate),
                            new DbParameterDescriptor("p1", calculationParams.FormDate),
                            new DbParameterDescriptor("p2", calculationParams.CalculationComment));
                    }
                    else
                    {
                        db.ExecQuery(
                            string.Format("delete from {0} where {1} = {2} and EndDate >= ?", entity.FullDBName,
                                          refMasterFieldName, credit.ID), QueryResultTypes.NonQuery,
                            new DbParameterDescriptor("p0", calculationParams.StartDate));
                    }

                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    if (dt.Rows.Count > 0 &&
                        (credit.CreditsType == CreditsTypes.BudgetOutcoming || credit.CreditsType == CreditsTypes.OrganizationOutcoming))
                        if (showWarnings)
                            throw new FinSourcePlanningException("План обслуживания долга уже заполнен.");
                        else return;
                    dt.Clear();

                    percentOddsList = new Dictionary<int, decimal>();

                    DataTable dtFactAttract = GetAttractFact(credit.ID);

                    FillCommission(dt, dtFactAttract, credit, calculationParams);

                    FillPlanService(credit, dt, false, calculationParams, new Holidays(scheme));

                    int refEkr = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanService_Key,
                        SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                    int refR = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanService_Key,
                        SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                    int refKD = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanService_Key,
                        SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);

                    foreach (DataRow row in dt.Rows)
                    {
                        row["ID"] = entity.GetGeneratorNextValue;
                        if (credit.CreditsType == CreditsTypes.BudgetOutcoming || credit.CreditsType == CreditsTypes.OrganizationOutcoming)
                            row["RefKD"] = refKD;
                        else if (credit.CreditsType == CreditsTypes.BudgetIncoming || credit.CreditsType == CreditsTypes.OrganizationIncoming)
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
        protected Decimal GetPercentPeriods(DateTime startDate, DateTime endDate, DateTime formDate, decimal creditPercent)
        {
            DataRow[] rows = PercentTable.Select(string.Format("ChargeDate <= '{0}'", formDate), "ChargeDate DESC");
            List<PercentPeriod> periods = new List<PercentPeriod>();
            DateTime endPeriod = endDate.AddDays(1);
            decimal percent = 0;
            if (rows.Length == 0)
            {
                percent = creditPercent / 100;
            }
            else
            {
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
                percent = percent / dayCount;
            }
            return percent;
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
        protected Decimal GetAttractSum(Credit credit, DateTime date, DateTime formDate, bool useOnlyFact, DateTime periodEndDate, bool chargeFirstDay)
        {
            // если данных нет никаких, возвращаем сумму самого кредита
            DataTable dtFactAttract = GetAttractFact(credit.ID);
            DataTable dtPlanAttract = GetAttractPlan(credit.ID);
            if (dtFactAttract.Rows.Count == 0 && dtPlanAttract.Rows.Count == 0)
            {
                // при незаполненных планах и фактах всегда возвращаем ноль
                return 0;
            }
            // сравниваем суммы плановую и фактическую, привлеченные до определенного времени
            // возвращаем максимальное значение
            decimal factAttractSum = 0;

            bool hasFact = dtFactAttract.Select(string.Format("FactDate <= '{0}'", periodEndDate)).Length > 0;
            DataRow[] rows =
                dtFactAttract.Select(string.Format("FactDate <= '{0}'", formDate));
            /*DataRow[] rows = chargeFirstDay ?
                dtFactAttract.Select(string.Format("FactDate <= '{0}'", formDate)) :
                dtFactAttract.Select(string.Format("FactDate < '{0}'", formDate));*/
            foreach (DataRow row in rows)
            {
                if (credit.RefOKV == -1)
                    factAttractSum += Convert.ToDecimal(row["Sum"]);
                else
                    factAttractSum += row.IsNull("CurrencySum") ? 0 : Convert.ToDecimal(row["CurrencySum"]);
            }

            decimal planAttractSum = 0;
            if (!useOnlyFact)
            {
                rows = hasFact
                           ? dtPlanAttract.Select(string.Format("StartDate > '{0}' and StartDate <= '{1}'", formDate, date))
                           : dtPlanAttract.Select(string.Format("StartDate <= '{0}'", date));
                /*rows = chargeFirstDay
                           ? (hasFact ?
                           dtPlanAttract.Select(string.Format("StartDate > '{0}' and StartDate <= '{1}'", formDate, date)) :
                           dtPlanAttract.Select(string.Format("StartDate <= '{0}'", date)))
                           : (hasFact ? dtPlanAttract.Select(string.Format("StartDate > '{0}' and StartDate < '{1}'", formDate, date)) :
                           dtPlanAttract.Select(string.Format("StartDate < '{0}'", date)));*/
                foreach (DataRow row in rows)
                {
                    if (credit.RefOKV == -1)
                        planAttractSum += Convert.ToDecimal(row["Sum"]);
                    else

                        planAttractSum += row.IsNull("CurrencySum") ? 0 : Convert.ToDecimal(row["CurrencySum"]);
                }
            }
            if (hasFact && date <= formDate)
                return factAttractSum;

            return factAttractSum + planAttractSum;
        }

        /// <summary>
        ///  возвращает сумму выплаченного долга до определенной даты
        /// </summary>
        /// <returns></returns>
        protected decimal GetDebtPaymentSum(Credit credit, DateTime startDate, DateTime endDate, DateTime formDate, VersionParams debtVersion, bool useOnlyFact)
        {
            if (credit.PretermDischarge || useOnlyFact)
            {
                // для досрочного погашения используем план погашения и факт погашения
                DataTable dtFact = GetDebtFact(credit.ID);
                DataTable dtPlan = GetDebtPlan(credit.ID, debtVersion);
                decimal planSum = 0;
                decimal factSum = 0;
                DateTime newFormDate = endDate > formDate ? formDate.AddDays(1) : formDate;
                bool hasFact = dtFact.Select(string.Format("FactDate < '{0}'", newFormDate)).Length > 0;
                DataRow[] rows = dtFact.Select(string.Format("FactDate < '{0}'", newFormDate));
                foreach (DataRow row in rows)
                    factSum += credit.RefOKV == -1 ? Convert.ToDecimal(row["Sum"]) : Convert.ToDecimal(row["CurrencySum"]);
                if (!useOnlyFact)
                {
                    rows = hasFact ? dtPlan.Select(string.Format("EndDate > '{0}' and EndDate < '{1}'", formDate, endDate)) :
                        dtPlan.Select(string.Format("EndDate < '{0}'", endDate));
                    foreach (DataRow row in rows)
                        planSum += credit.RefOKV == -1
                                       ? Convert.ToDecimal(row["Sum"])
                                       : Convert.ToDecimal(row["CurrencySum"]);
                }
                if (endDate <= formDate)
                    return factSum;
                return planSum + factSum;
            }
            else
            {
                decimal paymentPlanSum = 0;
                // если нет досрочного погашения, то используем только план погашения
                DataTable dtPlan = GetDebtPlan(credit.ID, debtVersion);
                DataRow[] rows = dtPlan.Select(string.Format("EndDate < '{0}'", endDate));
                foreach (DataRow row in rows)
                    paymentPlanSum += credit.RefOKV == -1 ? Convert.ToDecimal(row["Sum"]) : Convert.ToDecimal(row["CurrencySum"]);
                return paymentPlanSum;
            }
        }

        protected void GetPeriodDate(ref DateTime startPeriod, ref DateTime endPeriod,
            ref DateTime correctDate, ref DateTime paymentDate,
            PercentCalculationParams calculationParams, int increment, Holidays holidays, Credit credit, int payment)
        {
            // расчитываем начало периода, конец периода и дату выплаты
            if (calculationParams.PaymentsPeriodicity == PayPeriodicity.Other)
            {
                startPeriod = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[payment]["StartDate"]);
                endPeriod = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[payment]["EndDate"]);
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

                if (calculationParams.SplitPercentPeriods)
                {
                    DateTime subPeriodDate = GetNearSubPeriodDate(startPeriod, endPeriod, calculationParams.FormDate,
                                         calculationParams.UseAllPercents, credit, calculationParams);

                    if (subPeriodDate != DateTime.MinValue)
                    {
                        endPeriod = subPeriodDate;
                        correctDate = endPeriod.AddDays(1);
                    }
                }

                paymentDate = Utils.GetEndPeriod(startPeriod, calculationParams.PaymentsPeriodicity,
                                                      calculationParams.PaymentDay, increment);
                holidays.GetWorkDate(calculationParams.PaymentDayCorrection, ref paymentDate);
            }
        }

        /// <summary>
        /// получение начала периода
        /// </summary>
        /// <param name="startPeriod"></param>
        /// <param name="endPeriod"></param>
        /// <param name="calculationParams"></param>
        /// <param name="credit"></param>
        /// <returns></returns>
        internal DateTime GetStartPeriod(DateTime startPeriod, DateTime endPeriod,
            PercentCalculationParams calculationParams, Credit credit)
        {
            DataTable dtPlanAttract = GetAttractPlan(credit.ID);
            DataTable dtFactAttract = GetAttractFact(credit.ID);
            bool hasFact = dtFactAttract.Select(string.Format("FactDate <= '{0}'", calculationParams.FormDate)).Length > 0;
            if (hasFact)
            {
                DataRow[] rows =
                    dtFactAttract.Select(string.Format("FactDate >= '{0}' and FactDate <= '{1}'", startPeriod, endPeriod), "FactDate asc");
                if (rows.Length > 0)
                    return calculationParams.FirstDayPayment ? Convert.ToDateTime(rows[0]["FactDate"]) : Convert.ToDateTime(rows[0]["FactDate"]).AddDays(1);
            }
            else
            {
                DataRow[] rows =
                    dtPlanAttract.Select(string.Format("StartDate >= '{0}' and StartDate <= '{1}'", startPeriod, endPeriod), "StartDate asc");
                if (rows.Length > 0)
                    return Convert.ToDateTime(rows[0]["StartDate"]);
            }

            return startPeriod;
        }

        /// <summary>
        /// заполнение детали план обслуживания долга
        /// </summary>
        protected virtual void FillPlanService(Credit credit, DataTable dtServicePlan, bool useOnlyFact,
            PercentCalculationParams calculationParams, Holidays holidays)
        {
            decimal percentRoundDif = 0;
            DateTime calcDate = DateTime.Now;
            int increment = -Utils.GetSubtractValue(calculationParams.PaymentsPeriodicity);
            if (calculationParams.PaymentsPeriodicity == PayPeriodicity.Other)
            {
                calculationParams.StartDate = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[0]["StartDate"]);
                calculationParams.EndDate = Convert.ToDateTime(calculationParams.CustomPeriods.Rows[calculationParams.CustomPeriods.Rows.Count - 1]["EndDate"]);
            }
            DateTime startPeriod = calculationParams.FirstDayPayment ? calculationParams.StartDate : calculationParams.StartDate.AddDays(1);
            DateTime endPeriod = DateTime.MinValue;
            DateTime paymentDay = DateTime.MinValue;

            decimal debtSum = 0;
            // корректирующая дата указывает, нужно ли считать следующий большой период
            DateTime correctDate = startPeriod;
            int i = dtServicePlan.Rows.Count;
            // получаем последний день месяца
            if (calculationParams.EndPeriodDay == 0)
                calculationParams.EndPeriodDay = DateTime.DaysInMonth(calculationParams.StartDate.Year, calculationParams.StartDate.Month);

            decimal percent;
            if (calculationParams.PaymentsPeriodicity != PayPeriodicity.Single && calculationParams.EndPeriodDay >= 0 &&
                calculationParams.PaymentsPeriodicity != PayPeriodicity.Other)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, credit, 1);

                DateTime newStartPeriod = GetStartPeriod(startPeriod, endPeriod, calculationParams, credit);
                bool firstPeriodDay = false;
                if (Scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString() == "19 000 000")
                {
                    while (newStartPeriod >= endPeriod)
                    {
                        GetPeriodDate(ref newStartPeriod, ref endPeriod, ref correctDate, ref paymentDay,
                                      calculationParams, increment, holidays, credit, i);
                        newStartPeriod = GetStartPeriod(newStartPeriod, endPeriod, calculationParams, credit);
                    }
                    firstPeriodDay = startPeriod == endPeriod;
                }
                if (endPeriod >= paymentDay.AddMonths(1))
                    paymentDay = paymentDay.AddMonths(1);
                DateTime percentDate = calculationParams.UseAllPercents ? endPeriod : endPeriod >= calculationParams.FormDate ? calculationParams.FormDate : endPeriod;
                if (!(!calculationParams.FirstDayPayment && firstPeriodDay))
                {
                    percent = GetPercentPeriods(startPeriod, endPeriod, percentDate, credit.Percent);
                    // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                    decimal percentPay = GetPercentPay(startPeriod, endPeriod, credit, percent, useOnlyFact, calculationParams, ref debtSum);
                    if (percentPay != 0)
                    {
                        startPeriod = newStartPeriod;//GetStartPeriod(startPeriod, endPeriod, calculationParams, credit);
                        DataRow servPayRow = dtServicePlan.NewRow();
                        servPayRow.BeginEdit();
                        servPayRow["Payment"] = i + 1;
                        servPayRow["StartDate"] = startPeriod;
                        servPayRow["EndDate"] = endPeriod;
                        servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                        servPayRow["CreditPercent"] = percent * 100;
                        if (dtServicePlan.Columns.Contains("PaymentDate"))
                            servPayRow["PaymentDate"] = paymentDay;
                        servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                        servPayRow["PercentRate"] = credit.PercentRate;
                        servPayRow["RefOKV"] = credit.RefOKV;
                        if (dtServicePlan.Columns.Contains("EstimtDate"))
                            servPayRow["EstimtDate"] = calculationParams.FormDate;
                        if (dtServicePlan.Columns.Contains("CalcComment"))
                            servPayRow["CalcComment"] = calculationParams.CalculationComment;
                        if (dtServicePlan.Columns.Contains("CalcDate"))
                            servPayRow["CalcDate"] = calcDate;
                        if (dtServicePlan.Columns.Contains("DebtSum"))
                            servPayRow["DebtSum"] = debtSum;
                        servPayRow.EndEdit();
                        dtServicePlan.Rows.Add(servPayRow);
                        i++;
                        percentRoundDif += (percentPay - Math.Round(percentPay, 2, MidpointRounding.ToEven));
                    }
                }
                startPeriod = endPeriod.AddDays(1);
            }

            while (endPeriod < calculationParams.EndDate)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, credit, i);
                if (endPeriod >= paymentDay.AddMonths(1))
                    paymentDay = paymentDay.AddMonths(1);

                if (i == 0 && !calculationParams.FirstDayPayment && startPeriod == endPeriod)
                    continue;

                DateTime percentDate = calculationParams.UseAllPercents ? endPeriod : endPeriod >= calculationParams.FormDate ? calculationParams.FormDate : endPeriod;

                percent = GetPercentPeriods(startPeriod, endPeriod, percentDate, credit.Percent);
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                decimal percentPay = GetPercentPay(startPeriod, endPeriod, credit, percent, useOnlyFact, calculationParams, ref debtSum);
                // добавление записи с параметрами
                if (percentPay != 0)
                {
                    //startPeriod = GetStartPeriod(startPeriod, endPeriod, calculationParams, credit);
                    DataRow servPayRow = dtServicePlan.NewRow();
                    servPayRow.BeginEdit();
                    servPayRow["Payment"] = i + 1;
                    servPayRow["StartDate"] = startPeriod;
                    servPayRow["EndDate"] = endPeriod;
                    servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                    servPayRow["CreditPercent"] = percent * 100;
                    if (dtServicePlan.Columns.Contains("PaymentDate"))
                        servPayRow["PaymentDate"] = paymentDay;
                    servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                    servPayRow["PercentRate"] = credit.PercentRate;
                    servPayRow["RefOKV"] = credit.RefOKV;
                    if (dtServicePlan.Columns.Contains("EstimtDate"))
                        servPayRow["EstimtDate"] = calculationParams.FormDate;
                    if (dtServicePlan.Columns.Contains("CalcComment"))
                        servPayRow["CalcComment"] = calculationParams.CalculationComment;
                    if (dtServicePlan.Columns.Contains("CalcDate"))
                        servPayRow["CalcDate"] = calcDate;
                    if (dtServicePlan.Columns.Contains("DebtSum"))
                        servPayRow["DebtSum"] = debtSum;
                    servPayRow.EndEdit();
                    dtServicePlan.Rows.Add(servPayRow);
                    i++;
                    percentRoundDif += (percentPay - Math.Round(percentPay, 2, MidpointRounding.ToEven));
                }
                startPeriod = endPeriod.AddDays(1);
            }
            // если у нас в плане обслуживания нет ни одной записи, просто выходим
            if (dtServicePlan.Rows.Count == 0)
                return;
            // делаем дату выплаты в последнем периоде равной окончанию периода
            if (dtServicePlan.Columns.Contains("PaymentDate"))
                dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["PaymentDate"] =
                    dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["EndDate"];
            
            if (calculationParams.RestRound == PercentRestRound.InLastPayment)
            {
                for (int j = dtServicePlan.Rows.Count - 1; j >= 0; j--)
                {
                    if (percentRoundDif == 0)
                        break;
                    decimal sum = Convert.ToDecimal(dtServicePlan.Rows[j]["Sum"]);
                    dtServicePlan.Rows[j]["Sum"] = GetRoundedSum(sum, ref percentRoundDif);
                }
            }
            //decimal lastSum = Convert.ToDecimal(dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["Sum"]);
            //dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["Sum"] = Math.Round(lastSum + percentRoundDif, 2,
            //                                                                         MidpointRounding.ToEven);
        }

        internal decimal GetRoundedSum(decimal sum, ref decimal percentRoundDif)
        {
            if (percentRoundDif == 0)
                return sum;

            decimal whole = Math.Floor(sum);
            decimal value = sum - whole;

            if (percentRoundDif > 0)
            {
                if (value + percentRoundDif > 1)
                {
                    value = new Decimal(0.99) - value;
                    percentRoundDif -= value;
                    sum += value;
                }
                else
                {
                    sum += percentRoundDif;
                    percentRoundDif = 0;
                }
            }
            else
            {
                if (value + percentRoundDif < 0)
                {
                    percentRoundDif += value;
                    sum -= value;
                }
                else
                {
                    sum += percentRoundDif;
                    percentRoundDif = 0;
                }
            }
            return Math.Round(sum, 2, MidpointRounding.ToEven);
        }

        /// <summary>
        /// расчет комиссии
        /// </summary>
        /// <param name="dtServicePlan"></param>
        /// <param name="dtFactAttract"></param>
        /// <param name="credit"></param>
        internal virtual void FillCommission(DataTable dtServicePlan, DataTable dtFactAttract, Credit credit, PercentCalculationParams calculationParams)
        {
            // если комиссия не заполнена, выходим
            if (credit.Commission == 0)
                return;
            if (dtFactAttract.Rows.Count > 0)
            {
                foreach (DataRow factRow in dtFactAttract.Select(string.Format("FactDate <= '{0}'", calculationParams.FormDate)))
                {
                    decimal sum = factRow.IsNull("Sum") ? 0 : Convert.ToDecimal(factRow["Sum"]);
                    DateTime factDate = Convert.ToDateTime(factRow["FactDate"]);
                    DataRow newRow = dtServicePlan.NewRow();
                    newRow["Payment"] = dtServicePlan.Rows.Count + 1;
                    newRow["StartDate"] = factDate;
                    newRow["EndDate"] = factDate;
                    newRow["PaymentDate"] = factDate;
                    newRow["DayCount"] = 0;
                    newRow["Sum"] = (sum * credit.Commission) / 100;
                    newRow["RefOKV"] = credit.RefOKV;
                    newRow["CreditPercent"] = credit.Commission;
                    if (dtServicePlan.Columns.Contains("EstimtDate"))
                        newRow["EstimtDate"] = calculationParams.FormDate;
                    if (dtServicePlan.Columns.Contains("CalcComment"))
                        newRow["CalcComment"] = calculationParams.CalculationComment;
                    if (dtServicePlan.Columns.Contains("CalcDate"))
                        newRow["CalcDate"] = DateTime.Now;
                    dtServicePlan.Rows.Add(newRow);
                }
            }
            else
            {
                DataRow newRow = dtServicePlan.NewRow();
                newRow["Payment"] = dtServicePlan.Rows.Count + 1;
                newRow["StartDate"] = credit.StartDate;
                newRow["EndDate"] = credit.StartDate;
                newRow["PaymentDate"] = credit.StartDate;
                newRow["DayCount"] = 0;
                newRow["Sum"] = (credit.Sum * credit.Commission) / 100;
                newRow["RefOKV"] = credit.RefOKV;
                newRow["CreditPercent"] = credit.Commission;
                if (dtServicePlan.Columns.Contains("EstimtDate"))
                    newRow["EstimtDate"] = calculationParams.FormDate;
                if (dtServicePlan.Columns.Contains("CalcComment"))
                    newRow["CalcComment"] = calculationParams.CalculationComment;
                if (dtServicePlan.Columns.Contains("CalcDate"))
                    newRow["CalcDate"] = DateTime.Now;
                dtServicePlan.Rows.Add(newRow);

            }
        }

        protected DateTime GetNearSubPeriodDate(DateTime startPeriod, DateTime endPeriod,
            DateTime formDate, bool useAllPercents, Credit credit, PercentCalculationParams calculationParams)
        {
            List<DateTime> subPeriods = new List<DateTime>();
            DataTable dtFactDebt = GetDebtFact(credit.ID);
            DataTable dtPlanDebt = GetDebtPlan(credit.ID, calculationParams.MainDebtVersion);
            DataTable dtPlanAttract = GetAttractPlan(credit.ID);
            DataTable dtFactAttract = GetAttractFact(credit.ID);
            DataTable dtPercents = PercentTable ?? GetPercentJournal(credit.ID);
            // периоды по плану погашения
            DataRow[] rows = dtPlanDebt.Select(string.Format("EndDate < '{0}' and EndDate >= '{1}'", endPeriod, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = Convert.ToDateTime(row["EndDate"]);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по плану привлечения
            string startPeriodCompareSign = /*!calculationParams.FirstDayPayment ? ">=" :*/ ">";
            rows = dtPlanAttract.Select(string.Format("StartDate < '{0}' and StartDate {1} '{2}'", endPeriod, startPeriodCompareSign, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = calculationParams.FirstDayPayment ?
                    Convert.ToDateTime(row["StartDate"]).AddDays(-1) :
                    Convert.ToDateTime(row["StartDate"]);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по факту привлечения
            DateTime endFactDate = endPeriod <= formDate ? endPeriod : formDate;
            rows = dtFactAttract.Select(string.Format("FactDate < '{0}' and FactDate {1} '{2}'", endFactDate, startPeriodCompareSign, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = calculationParams.FirstDayPayment ?
                    Convert.ToDateTime(row["FactDate"]).AddDays(-1) :
                    Convert.ToDateTime(row["FactDate"]);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по смене процентной ставки
            DateTime percentDate = useAllPercents ? endPeriod : formDate > endPeriod ? endPeriod : formDate;
            rows = dtPercents.Select(string.Format("ChargeDate < '{0}' and ChargeDate > '{1}'", percentDate, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = Convert.ToDateTime(row["ChargeDate"]).AddDays(-1);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // если стоит досрочное погашение, то периоды по факту погашения
            if (credit.PretermDischarge)
            {
                rows = dtFactDebt.Select(string.Format("FactDate < '{0}' and FactDate >= '{1}'", endFactDate, startPeriod));
                foreach (DataRow row in rows)
                {
                    DateTime factDate = Convert.ToDateTime(row["FactDate"]);
                    if (!subPeriods.Contains(factDate))
                        subPeriods.Add(factDate);
                }
            }
            subPeriods.Sort();
            return subPeriods.Count > 0 ? subPeriods[0] : DateTime.MinValue;
        }

        protected Decimal GetPercentPay(DateTime startPeriod, DateTime endPeriod, Credit credit, decimal percent,
            bool useOnlyFact, PercentCalculationParams calculationParams, ref decimal restDebtSum)
        {
            // получим сумму, выплаченную по плану выплаты на момент начисления процентов
            List<DateTime> subPeriods = new List<DateTime>();
            DataTable dtFactDebt = GetDebtFact(credit.ID);
            DataTable dtPlanDebt = GetDebtPlan(credit.ID, calculationParams.MainDebtVersion);
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
                DateTime planDate = calculationParams.FirstDayPayment ?
                    Convert.ToDateTime(row["StartDate"]).AddDays(-1) :
                    Convert.ToDateTime(row["StartDate"]);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по факту привлечения
            DateTime endFactDate = endPeriod <= calculationParams.FormDate ? endPeriod : calculationParams.FormDate;
            rows = dtFactAttract.Select(string.Format("FactDate <= '{0}' and FactDate > '{1}'", endFactDate, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = calculationParams.FirstDayPayment ?
                    Convert.ToDateTime(row["FactDate"]).AddDays(-1) :
                    Convert.ToDateTime(row["FactDate"]);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // периоды по смене процентной ставки
            DateTime percentDate = calculationParams.UseAllPercents ? endPeriod : calculationParams.FormDate > endPeriod ? endPeriod : calculationParams.FormDate;
            rows = dtPercents.Select(string.Format("ChargeDate <= '{0}' and ChargeDate >= '{1}'", percentDate, startPeriod));
            foreach (DataRow row in rows)
            {
                DateTime planDate = Convert.ToDateTime(row["ChargeDate"]).AddDays(-1);
                if (!subPeriods.Contains(planDate))
                    subPeriods.Add(planDate);
            }
            // дата формирования 
            if (calculationParams.FormDate <= endPeriod && calculationParams.FormDate >= startPeriod
                && !subPeriods.Contains(calculationParams.FormDate))
                subPeriods.Add(calculationParams.FormDate);
            // если стоит досрочное погашение, то периоды по факту погашения
            if (credit.PretermDischarge)
            {
                rows = dtFactDebt.Select(string.Format("FactDate <= '{0}' and FactDate > '{1}'", endFactDate, startPeriod));
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
                percentDate = calculationParams.UseAllPercents ? endPeriod : calculationParams.FormDate > endPeriod ? endPeriod : calculationParams.FormDate;
                endFactDate = endPeriod <= calculationParams.FormDate ? endPeriod : calculationParams.FormDate;
                decimal sum = GetAttractSum(credit, endPeriod, endFactDate, useOnlyFact, calculationParams.FormDate, calculationParams.FirstDayPayment);
                decimal debtSum = GetDebtPaymentSum(credit, startPeriod, endPeriod, endFactDate, calculationParams.MainDebtVersion, useOnlyFact);
                sum -= debtSum;
                // получаем ставку процентов из журнала процентных ставок
                percent = GetPercent(credit, dtPercents, percentDate);
                // получаем сумму процентов, которые уже попадают в таблицу
                percent = GetPercents(sum, startPeriod, endPeriod, percent);
                percents += percent;
                startPeriod = endPeriod.AddDays(1);
                restDebtSum = sum;
            }
            // расчет остатков процентов
            // сумма процентов, округленная до 2 знаков
            decimal roundPercent = Math.Round(percents, 2, MidpointRounding.ToEven);
            // разница между округлением и обычной суммой процентов
            decimal percentOdds = percents - roundPercent;
            // считаем текущие проценты 
            if (calculationParams.RestRound == PercentRestRound.OnAccumulation)
            {
                if (percentOddsList.ContainsKey(percentOddsList.Count - 1))
                {
                    decimal prevOdds = percentOddsList[percentOddsList.Count - 1];
                    percentOdds = percentOdds + prevOdds;
                    if (Math.Abs(percentOdds) > new Decimal(0.005))
                    {
                        roundPercent += percentOdds > 0 ? new Decimal(0.01) : new Decimal(-0.01);
                        percentOdds = Math.Round(percentOdds + (percentOdds > 0 ? new Decimal(-0.010001) : new Decimal(0.010001)), 4, MidpointRounding.ToEven);
                    }
                }
                percents = roundPercent;
            }
            percentOddsList.Add(percentOddsList.Count, percentOdds);
            return percents;
        }

        private Dictionary<int, decimal> percentOddsList;

        #endregion
    }

    public partial class CurrencyCreditServer : FinSourcePlanningServer
    {
        #region расчеты плана обслуживания долга

        /// <summary>
        /// План обслуживания долга. Начисление процентов на остаток
        /// </summary>
        protected override void FillPlanService(Credit credit, DataTable dtServicePlan, bool useOnlyFact,
            PercentCalculationParams calculationParams, Holidays holidays)
        {
            DateTime calcDate = DateTime.Now;
            decimal percentRoundDif = 0;
            decimal currencyPercentRoundDif = 0;
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

            decimal debtSum = 0;
            if (calculationParams.EndPeriodDay == 0)
                calculationParams.EndPeriodDay = DateTime.DaysInMonth(calculationParams.StartDate.Year, calculationParams.StartDate.Month);

            int i = 0;
            decimal percent;
            // если конец периода не совпадает с 
            if (calculationParams.PaymentsPeriodicity != PayPeriodicity.Single && calculationParams.EndPeriodDay >= 0 &&
                calculationParams.PaymentsPeriodicity != PayPeriodicity.Other)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, credit, 1);

                DateTime newStartPeriod = GetStartPeriod(startPeriod, endPeriod, calculationParams, credit);
                bool firstPeriodDay = false;
                if (Scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString() == "19 000 000")
                {
                    while (newStartPeriod >= endPeriod)
                    {
                        GetPeriodDate(ref newStartPeriod, ref endPeriod, ref correctDate, ref paymentDay,
                                      calculationParams, increment, holidays, credit, i);
                        newStartPeriod = GetStartPeriod(newStartPeriod, endPeriod, calculationParams, credit);
                    }
                    firstPeriodDay = startPeriod == endPeriod;
                }
                if (endPeriod >= paymentDay.AddMonths(1))
                    paymentDay = paymentDay.AddMonths(1);
                if (!(!calculationParams.FirstDayPayment && firstPeriodDay))
                {
                    // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                    percent = GetPercentPeriods(startPeriod, endPeriod, calculationParams.FormDate, credit.Percent);

                    decimal percentCurrencyPay = GetPercentPay(startPeriod, endPeriod, credit, percent, useOnlyFact, calculationParams, ref debtSum);
                    decimal percentPay = Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven) * credit.ExchangeRate;

                    if (percentCurrencyPay != 0)
                    {
                        startPeriod = newStartPeriod;
                        //startPeriod = GetStartPeriod(startPeriod, endPeriod, calculationParams, credit);
                        DataRow servPayRow = dtServicePlan.NewRow();
                        servPayRow.BeginEdit();
                        servPayRow["Payment"] = i + 1;
                        servPayRow["StartDate"] = startPeriod;
                        servPayRow["EndDate"] = endPeriod;
                        servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                        servPayRow["CreditPercent"] = percent * 100;
                        servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                        servPayRow["PercentRate"] = credit.PercentRate;
                        servPayRow["CurrencySum"] = Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven);
                        servPayRow["ExchangeRate"] = credit.ExchangeRate;
                        if (dtServicePlan.Columns.Contains("PaymentDate"))
                            servPayRow["PaymentDate"] = paymentDay;
                        servPayRow["RefOKV"] = credit.RefOKV;
                        servPayRow["IsPrognozExchRate"] = credit.IsPrognozExchRate ?? (object)DBNull.Value;
                        if (dtServicePlan.Columns.Contains("EstimtDate"))
                            servPayRow["EstimtDate"] = calculationParams.FormDate;
                        if (dtServicePlan.Columns.Contains("CalcComment"))
                            servPayRow["CalcComment"] = calculationParams.CalculationComment;
                        if (dtServicePlan.Columns.Contains("CalcDate"))
                            servPayRow["CalcDate"] = calcDate;
                        if (dtServicePlan.Columns.Contains("DebtSum"))
                            servPayRow["DebtSum"] = debtSum;
                        servPayRow.EndEdit();
                        dtServicePlan.Rows.Add(servPayRow);
                        i++;
                        percentRoundDif += (percentPay - Math.Round(percentPay, 2, MidpointRounding.ToEven));
                        currencyPercentRoundDif += (percentCurrencyPay - Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven));
                    }
                }
                startPeriod = endPeriod.AddDays(1);
            }

            while (endPeriod < calculationParams.EndDate)
            {
                GetPeriodDate(ref startPeriod, ref endPeriod, ref correctDate, ref paymentDay, calculationParams,
                              increment, holidays, credit, i);
                if (endPeriod >= paymentDay.AddMonths(1))
                    paymentDay = paymentDay.AddMonths(1);
                if (i == 0 && !calculationParams.FirstDayPayment && startPeriod == endPeriod)
                    continue;
                // получим сумму, выплаченную по плану выплаты на момент начисления процентов
                percent = GetPercentPeriods(startPeriod, endPeriod, calculationParams.FormDate, credit.Percent);

                decimal percentCurrencyPay = GetPercentPay(startPeriod, endPeriod, credit, percent, useOnlyFact, calculationParams, ref debtSum);
                decimal percentPay = Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven) * credit.ExchangeRate;

                // добавление записи с параметрами
                if (percentCurrencyPay != 0)
                {
                    //startPeriod = GetStartPeriod(startPeriod, endPeriod, calculationParams, credit);
                    DataRow servPayRow = dtServicePlan.NewRow();
                    servPayRow.BeginEdit();
                    servPayRow["Payment"] = i + 1;
                    servPayRow["StartDate"] = startPeriod;
                    servPayRow["EndDate"] = endPeriod;
                    servPayRow["DayCount"] = (endPeriod - startPeriod).Days + 1;
                    servPayRow["CreditPercent"] = percent * 100;
                    servPayRow["Sum"] = Math.Round(percentPay, 2, MidpointRounding.ToEven);
                    servPayRow["PercentRate"] = credit.PercentRate;
                    servPayRow["CurrencySum"] = Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven);
                    servPayRow["ExchangeRate"] = credit.ExchangeRate;
                    if (dtServicePlan.Columns.Contains("PaymentDate"))
                        servPayRow["PaymentDate"] = paymentDay;
                    servPayRow["RefOKV"] = credit.RefOKV;
                    servPayRow["IsPrognozExchRate"] = credit.IsPrognozExchRate ?? (object)DBNull.Value;
                    if (dtServicePlan.Columns.Contains("EstimtDate"))
                        servPayRow["EstimtDate"] = calculationParams.FormDate;
                    if (dtServicePlan.Columns.Contains("CalcComment"))
                        servPayRow["CalcComment"] = calculationParams.CalculationComment;
                    if (dtServicePlan.Columns.Contains("CalcDate"))
                        servPayRow["CalcDate"] = calcDate;
                    if (dtServicePlan.Columns.Contains("DebtSum"))
                        servPayRow["DebtSum"] = debtSum;
                    servPayRow.EndEdit();
                    dtServicePlan.Rows.Add(servPayRow);
                    i++;
                    percentRoundDif += (percentPay - Math.Round(percentPay, 2, MidpointRounding.ToEven));
                    currencyPercentRoundDif += (percentCurrencyPay - Math.Round(percentCurrencyPay, 2, MidpointRounding.ToEven));
                }
                startPeriod = endPeriod.AddDays(1);
            }
            // если у нас в плане обслуживания нет ни одной записи, просто выходим
            if (dtServicePlan.Rows.Count == 0)
                return;

            // делаем дату выплаты в последнем периоде равной окончанию периода
            if (dtServicePlan.Columns.Contains("PaymentDate"))
                dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["PaymentDate"] =
                    dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["EndDate"];
            
            if (calculationParams.RestRound == PercentRestRound.InLastPayment)
            {
                for (int j = dtServicePlan.Rows.Count - 1; j >= 0; j--)
                {
                    if (percentRoundDif == 0 && currencyPercentRoundDif == 0)
                        break;
                    decimal sum = Convert.ToDecimal(dtServicePlan.Rows[j]["Sum"]);
                    dtServicePlan.Rows[j]["Sum"] = GetRoundedSum(sum, ref percentRoundDif);
                    decimal curencySum = Convert.ToDecimal(dtServicePlan.Rows[j]["CurrencySum"]);
                    dtServicePlan.Rows[j]["CurrencySum"] = GetRoundedSum(curencySum, ref currencyPercentRoundDif);
                }
                /*
                decimal lastSum = Convert.ToDecimal(dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["Sum"]);
                dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["Sum"] = Math.Round(lastSum + percentRoundDif, 2,
                                                                                     MidpointRounding.ToEven);

                decimal lastCurrencySum = Convert.ToDecimal(dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["CurrencySum"]);
                dtServicePlan.Rows[dtServicePlan.Rows.Count - 1]["CurrencySum"] = Math.Round(lastCurrencySum + currencyPercentRoundDif, 2,
                                                                                     MidpointRounding.ToEven);*/
            }
        }

        internal override void FillCommission(DataTable dtServicePlan, DataTable dtFactAttract, Credit credit, PercentCalculationParams calculationParams)
        {

        }

        #endregion
    }
}
