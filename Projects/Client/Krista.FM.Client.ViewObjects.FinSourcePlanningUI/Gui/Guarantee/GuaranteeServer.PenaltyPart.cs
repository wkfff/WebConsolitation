using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees
{
    partial class GuaranteeServer
    {
        /// <summary>
        /// заполнение журнала процентов
        /// </summary>
        public void CalcPercents(DataRow[] principalContractRows, int masterID, int d_S_JournalCB_RowID, bool clearJournalPercent)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;

            // Деталь "ИФ.Журнал ставок процентов"
            IEntity entity = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_JournalPercentGrnt_Key);

            // ИФ.Журнал ставок процентов -> ИФ.Кредиты полученные
            string refMasterFieldName = entity.Associations[GuaranteeIssuedObjectKeys.a_S_JournalPercentGrnt_RefGrnt_Key].RoleDataAttribute.Name;

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

                foreach (DataRow principalContractRow in principalContractRows)
                {
                    // если данных по гарантируемому договору нет, пропускаем договор по гарантии

                    DateTime startDate = Convert.ToDateTime(principalContractRow["StartDate"]);
                    decimal creditPercent = principalContractRow.IsNull("CreditPercent") ?
                        0 :
                        Convert.ToDecimal(principalContractRow["CreditPercent"]);
                    decimal ratePenalty = principalContractRow.IsNull("PenaltyDebtRate") ?
                        0 :
                        Convert.ToDecimal(principalContractRow["PenaltyDebtRate"]);
                    decimal ratePenPercent = principalContractRow.IsNull("PenaltyPercentRate") ?
                        0 :
                        Convert.ToDecimal(principalContractRow["PenaltyPercentRate"]);
                    
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
                                throw new FinSourcePlanningException(
                                    "Журнал ставок процентов уже заполнен. Очистите таблицу и повторите попытку.");
                            }
                        }

                        // берем дату, меньше даты договора, но максимально близкую к дате заключения
                        DataRow[] rows = d_S_JournalCB_Table.Select(string.Format("InputDate <= '{0}'", startDate), "InputDate ASC");

                        if (rows.Length == 0)
                        {
                            throw new FinSourcePlanningException(
                                string.Format("В журнале ставок ЦБ не найдено ни одной ставки на {0}", startDate));
                        }

                        FillPercentsTable(dt, rows[rows.Length - 1], Convert.ToDateTime(principalContractRow["StartDate"]), creditPercent, ratePenalty, ratePenPercent);
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
        /// Расчет ставок процентов.
        /// </summary>
        private void FillPercentsTable(DataTable dt, DataRow journalCBRow, DateTime startDate, decimal creditPercent, decimal ratePenalty, decimal ratePenPercent)
        {
            // Если запись на дату уже есть, то берем ее, иначе создаем новую
            DataRow[] rows = dt.Select(String.Format("ChargeDate = '{0}'", journalCBRow["InputDate"]));
            DataRow row = rows.Length == 0 ? dt.NewRow() : rows[0];

            decimal percentCB = Convert.ToDecimal(journalCBRow["PercentRate"]);
            object date = journalCBRow["InputDate"];
            if (ratePenalty < 0 && ratePenPercent < 0)
                date = startDate;
            row["ChargeDate"] = date;
            row["CreditPercent"] = creditPercent;
            row["PenaltyDebt"] = ratePenalty >= 0 ? (object)(ratePenalty * percentCB) : DBNull.Value;
            row["PenaltyPercent"] = ratePenPercent >= 0 ? (object)(ratePenPercent * percentCB) : DBNull.Value;

            if (row.RowState == DataRowState.Detached)
                dt.Rows.Add(row);
        }


        #region начисление пени

        /// <summary>
        /// добавляет запись в деталь "Пени по основному долгу"
        /// </summary>
        public void AddDebtPenalty(int masterID,
            DateTime paymentDate, int baseYear, int payment, decimal sum, decimal currencySum,
            int daysLate, decimal penalty, decimal penaltyRate, decimal exchangeRate, int refOkv)
        {
            string associationKey = GuaranteeIssuedObjectKeys.a_S_ChargePenaltyDebtPrGrnt_RefGrnt_Key;
            AddPenalty(masterID, associationKey, paymentDate, baseYear, payment, sum, currencySum,
                daysLate, penalty, penaltyRate, exchangeRate, refOkv);
        }

        /// <summary>
        /// добавляет запись в деталь "Пени по основному долгу"
        /// </summary>
        public void AddPercentPenalty(int masterID,
            DateTime paymentDate, int baseYear, int payment, decimal sum, decimal currencySum,
            int daysLate, decimal penalty, decimal penaltyRate, decimal exchangeRate, int refOkv)
        {
            string associationKey = GuaranteeIssuedObjectKeys.a_S_PrGrntChargePenaltyPercent_RefGrnt_Key;
            AddPenalty(masterID, associationKey, paymentDate, baseYear, payment, sum, currencySum,
                daysLate, penalty, penaltyRate, exchangeRate, refOkv);
        }

        /// <summary>
        /// добавляет запись в деталь "Пени по основному долгу"
        /// </summary>
        protected virtual void AddPenalty(int masterID, string associationKey,
            DateTime paymentDate, int baseYear, int payment, decimal sum, decimal currencySum,
            int daysLate, decimal penalty, decimal penaltyRate, decimal exchangeRate, int refOkv)
        {

            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                IEntityAssociation entityAssociation =
                    FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindAssociationByName(associationKey);
                IEntity entity = entityAssociation.RoleData;

                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 7, baseYear);
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
                    newRow["RefGrnt"] = masterID;
                    newRow["RefOKV"] = refOkv;
                    newRow.EndEdit();
                    t_S_ChargePenaltyCI_Table.Rows.Add(newRow);
                    // проставляем ссылки на классификаторы
                    //creditUtils.FillPenaltiesRef(t_S_ChargePenaltyCI_Table, db, sourceID);
                    du.Update(ref t_S_ChargePenaltyCI_Table);
                }
            }
        }

        /// <summary>
        /// Начисление пени по основному долгу.
        /// </summary>
        public Dictionary<string, string> CalculatePenaltiesMainDebt(DataRow[] masterRows, int baseYear)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            foreach (DataRow masterRow in masterRows)
            {
                try
                {
                    if (Convert.ToInt32(masterRow["RefSStatusPlan"]) == 0 &&
                        Convert.ToInt32(masterRow["RefVariant"]) == FinSourcePlanningNavigation.Instance.CurrentVariantID)
                    {
                        DataRow principalContractRow = GetPrincipalContract(masterRow);
                        if (principalContractRow != null)
                        {
                            PrincipalContract principalContract = new PrincipalContract(principalContractRow, new Guarantee(masterRow));
                            double ratePenalty = principalContractRow["PenaltyDebtRate"] is DBNull ? 0
                                : Convert.ToDouble(principalContractRow["PenaltyDebtRate"]);
                            CalculatePenalties(principalContract, ratePenalty, baseYear,
                                GuaranteeIssuedObjectKeys.a_S_FactDebtPrGrnt_RefGrnt_Key,
                                GuaranteeIssuedObjectKeys.a_S_PlanDebtPrGrnt_Key,
                                GuaranteeIssuedObjectKeys.t_S_ChargePenaltyDebtPrGrnt_Key,
                                GuaranteeIssuedObjectKeys.a_S_ChargePenaltyDebtPrGrnt_RefGrnt_Key,
                                "PenaltyDebt");
                        }
                    }
                }
                catch (FinSourcePlanningException e)
                {
                    errors.Add(Convert.ToString(masterRow["Num"]), e.Message);
                }
            }
            return errors;
        }

        /// <summary>
        /// Начисление пени по процентам.
        /// </summary>
        public Dictionary<string, string> CalculatePenaltiesPlanService(DataRow[] masterRows, int baseYear)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            foreach (DataRow masterRow in masterRows)
            {
                try
                {
                    if (Convert.ToInt32(masterRow["RefSStatusPlan"]) == 0 &&
                        Convert.ToInt32(masterRow["RefVariant"]) == FinSourcePlanningNavigation.Instance.CurrentVariantID)
                    {
                        DataRow principalContractRow = GetPrincipalContract(masterRow);
                        if (principalContractRow != null)
                        {
                            PrincipalContract principalContract = new PrincipalContract(principalContractRow, new Guarantee(masterRow));
                            double ratePenalty = principalContractRow["PenaltyDebtRate"] is DBNull ? 1
                                : Convert.ToDouble(principalContractRow["PenaltyDebtRate"]);
                            CalculatePenalties(principalContract, ratePenalty, baseYear,
                                GuaranteeIssuedObjectKeys.a_S_FactPercentPrGrnt_RefGrnt_Key,
                                GuaranteeIssuedObjectKeys.a_S_PlanServicePrGrnt_Key,
                                GuaranteeIssuedObjectKeys.t_S_PrGrntChargePenaltyPercent_Key,
                                GuaranteeIssuedObjectKeys.a_S_PrGrntChargePenaltyPercent_RefGrnt_Key,
                                "PenaltyPercent");
                        }
                    }
                }
                catch (FinSourcePlanningException e)
                {
                    errors.Add(Convert.ToString(masterRow["Num"]), e.Message);
                }
            }
            return errors;
        }

        /// <summary>
        /// Начисление пени.
        /// </summary>
        private void CalculatePenalties(PrincipalContract principalContract, double ratePenalty, int baseYear,
            string factKey, string planKey, string chargePenaltyKey,
            string chargePenaltyAssociationKey, string percentName)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                // Получаем данные детали "Журнал ставок процентов"
                DataTable t_S_JournalPercentCI_Table = Utils.GetDetailTable(db, principalContract.Guarantee.ID, GuaranteeIssuedObjectKeys.a_S_JournalPercentGrnt_RefGrnt_Key);
                if (t_S_JournalPercentCI_Table.Rows.Count == 0)
                    throw new FinSourcePlanningException("Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"");
                // Получаем данные детали "ИФ.Факт погашения основного долга"
                DataTable t_S_FactDebtCI_Table = Utils.GetDetailTable(db, principalContract.Guarantee.ID, factKey);
                if (t_S_FactDebtCI_Table.Rows.Count == 0)
                {
                    DataRow row = t_S_FactDebtCI_Table.NewRow();
                    row["FactDate"] = DateTime.Today;
                    row["Sum"] = 0;
                    row["CurrencySum"] = 0;
                    row["RefOKV"] = principalContract.Guarantee.RefOKV;
                    t_S_FactDebtCI_Table.Rows.Add(row);
                }
                // Получаем данные детали "ИФ.План погашения основного долга"
                DataTable t_S_PlanDebtCI_Table = Utils.GetDetailTable(db, principalContract.Guarantee.ID, planKey);

                #region Формируем деталь "ИФ.Начисление пени по основному долгу"

                DataTable t_S_ChargePenalty_Table = new DataTable();
                IEntity t_S_ChargePenalty_Entity = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(chargePenaltyKey);
                string refMasterFieldName = t_S_ChargePenalty_Entity.Associations[chargePenaltyAssociationKey].RoleDataAttribute.Name;

                IDbDataParameter[] prms = new IDbDataParameter[] { new System.Data.OleDb.OleDbParameter(refMasterFieldName, principalContract.Guarantee.ID) };
                using (IDataUpdater du = t_S_ChargePenalty_Entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                {
                    du.Fill(ref t_S_ChargePenalty_Table);
                    Penalties penalty = new Penalties(t_S_PlanDebtCI_Table, t_S_FactDebtCI_Table, principalContract.Guarantee.RefOKV);
                    DataRow[] planRows = t_S_PlanDebtCI_Table.Select(string.Empty, "EndDate Asc");

                    int payment = 1;
                    foreach (DataRow row in planRows)
                    {
                        // получить процент по пеням из расчета дат периода.
                        decimal penaltyRate = GetPercent(principalContract,
                            GetPenaltyEndPeriod(Convert.ToDateTime(row["EndDate"])), percentName);
                        DateTime endPeriod = GetPenaltyEndPeriod(Convert.ToDateTime(row["EndDate"]));
                        DataRow[] penaltyRows = penalty.CalculatePenalty(endPeriod,
                            GetNextEndPeriod(endPeriod, t_S_PlanDebtCI_Table), penaltyRate, t_S_ChargePenalty_Table);

                        if (penaltyRows != null)
                        {
                            foreach (DataRow penaltyRow in penaltyRows)
                            {
                                penaltyRow["ID"] = t_S_ChargePenalty_Entity.GetGeneratorNextValue;
                                penaltyRow["Payment"] = payment;
                                penaltyRow[refMasterFieldName] = principalContract.Guarantee.ID;
                                penaltyRow["PenaltyRate"] = ratePenalty;
                                t_S_ChargePenalty_Table.Rows.Add(penaltyRow);
                                payment++;
                            }
                        }
                    }
                    //creditUtils.FillPenaltiesRef(t_S_ChargePenalty_Table, db, sourceID);
                    du.Update(ref t_S_ChargePenalty_Table);
                }

                #endregion Формируем деталь "ИФ.Начисление пени по основному долгу"
            }
        }

        /// <summary>
        /// получаем процент для пени на определенную дату
        /// </summary>
        protected static Decimal GetPercent(PrincipalContract principalContract, DateTime percentDate, string percentName)
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
                    decimal penaltyRate = percentName == "PenaltyDebt"
                                              ? principalContract.PenaltyDebtRate
                                              : principalContract.PenaltyPercentRate;
      
                    return percentRate*penaltyRate;
                }
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime GetNextEndPeriod(DateTime endPeriod, DataTable dtPeriods)
        {
            DataRow[] rows = dtPeriods.Select(string.Format("EndDate > '{0}'", endPeriod), "EndDate Asc");
            if (rows == null || rows.Length == 0)
                return endPeriod;
            return Convert.ToDateTime(rows[0]["EndDate"]);
        }

        /// <summary>
        /// расчитываем дату окончания периода, по которому пени не считаются (до 10 числа месяца + выходные)
        /// </summary>
        /// <param name="endPeriod"></param>
        /// <returns></returns>
        private static DateTime GetPenaltyEndPeriod(DateTime endPeriod)
        {
            return endPeriod;
        }

        #endregion
    }

    public class Penalties
    {
        private decimal currentPlanSum;
        private decimal currentFactSum;
        private DataTable planTable;
        private DataTable factTable;
        private int refOKV;
        private decimal prevPenalty;
        private DateTime prevEndPeriodDate;
        private int prevPeriodDaysCount;

        public Penalties(DataTable planTable, DataTable factTable, int refOKV)
        {
            currentPlanSum = 0;
            currentFactSum = 0;
            this.planTable = planTable.Copy();
            this.factTable = factTable;
            this.refOKV = refOKV;
            prevPenalty = 0;
            prevEndPeriodDate = DateTime.MinValue;
            prevPeriodDaysCount = 0;
        }

        /// <summary>
        /// расчет пени на определенную плановую дату
        /// </summary>
        public DataRow[] CalculatePenalty(DateTime planDate, DateTime nextPlanDate, decimal penaltyRate, DataTable dtPenalty)
        {
            // для кредитов предоставленных считается без деления на  количество дней в году
            decimal k = Math.Round(penaltyRate/100, 10, MidpointRounding.AwayFromZero);

            currentPlanSum = 0;
            currentFactSum = 0;
            // получаем разницу на текущий момент времени разницу в плане и факте
            DataRow[] planPayments = planTable.Select(string.Format("EndDate <= '{0}'", planDate), "EndDate Asc");
            DataRow[] factPayments = factTable.Select(string.Format("FactDate <= '{0}'", planDate), "FactDate Asc");

            foreach (DataRow row in planPayments)
            {
                if (refOKV == -1)
                    currentPlanSum += Convert.ToDecimal(row["Sum"]);
                else
                    currentPlanSum += Convert.ToDecimal(row["CurrencySum"]);

            }
            foreach (DataRow row in factPayments)
            {
                if (refOKV == -1)
                    currentFactSum += Convert.ToDecimal(row["Sum"]);
                else
                    currentFactSum += Convert.ToDecimal(row["CurrencySum"]);
            }

            // получаем дату фактического платежа, следующего за расчетной датой.
            // до нее будем начислять пени
            factPayments = factTable.Select(string.Format("FactDate > '{0}'", planDate), "FactDate Asc");

            List<DataRow> penalties = new List<DataRow>();

            foreach (DataRow row in factPayments)
            {
                DateTime factDate = Convert.ToDateTime(row["FactDate"]);

                if (planDate <= DateTime.Today && currentPlanSum > currentFactSum)
                {
                    decimal r = currentPlanSum - currentFactSum;
                    decimal penalty = r * ((factDate - planDate).Days) * k;

                    if (prevEndPeriodDate >= factDate)
                    {
                        penalty -= prevPenalty / prevPeriodDaysCount * ((prevEndPeriodDate - factDate).Days + 1);
                    }
                    if (penalty != 0 && (factDate - planDate).Days != 0)
                    {
                        DataRow penaltyRow = dtPenalty.NewRow();
                        if (refOKV == -1)
                        {
                            penaltyRow["Sum"] = penalty;
                        }
                        else
                        {
                            decimal exchangeRate = Utils.GetLastCurrencyExchange(DateTime.Today, refOKV);
                            penaltyRow["CurrencySum"] = penalty;
                            penaltyRow["Sum"] = penalty * exchangeRate;
                            penaltyRow["ExchangeRate"] = exchangeRate;
                            penaltyRow["IsPrognozExchRate"] = false;
                        }
                        // дату начисления ставим как последний день месяца, в котором начинаем начислять пени
                        penaltyRow["Penalty"] = k;
                        penaltyRow["StartDate"] = new DateTime(planDate.Year, planDate.Month,
                                                               DateTime.DaysInMonth(planDate.Year, planDate.Month));
                        penaltyRow["LateDate"] = (factDate - planDate).Days;
                        penaltyRow["RefOKV"] = refOKV;

                        penalties.Add(penaltyRow);
                    }
                    prevPenalty = penalty;
                    prevEndPeriodDate = factDate;
                    prevPeriodDaysCount = (factDate - planDate).Days;
                    planDate = factDate;
                    if (refOKV == -1)
                    {
                        currentFactSum += Convert.ToDecimal(row["Sum"]);
                    }
                    else
                        currentFactSum += Convert.ToDecimal(row["CurrencySum"]);
                }
            }

            // если разница есть, считаем пени до даты начисления
            return penalties.ToArray();
        }
    }
}
