using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using NUnit.Framework;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{

    public partial class FinSourcePlanningServer
    {

        #region начисление пени

        /// <summary>
        /// Начисление пени.
        /// </summary>
        private void CalculatePenalties(Credit credit, double ratePenalty, int baseYear,
            string factKey, string planKey, string chargePenaltyKey,
            string chargePenaltyAssociationKey, string percentName, Holidays holydays)
        {
            IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;

            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                // Получаем данные детали "Журнал ставок процентов"
                DataTable t_S_JournalPercentCI_Table = Utils.GetDetailTable(db, credit.ID, a_S_JournalPercent_RefCredit_Key);
                if (t_S_JournalPercentCI_Table.Rows.Count == 0)
                    throw new FinSourcePlanningException("Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"");

                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 7, baseYear);
                // Получаем данные детали "ИФ.Факт погашения основного долга"
                DataTable t_S_FactDebtCI_Table = Utils.GetDetailTable(db, credit.ID, factKey);
                // Получаем данные детали "ИФ.План погашения основного долга"
                DataTable t_S_PlanDebtCI_Table = Utils.GetDetailTable(db, credit.ID, planKey);
                
                #region Формируем деталь "ИФ.Начисление пени по основному долгу"

                DataTable t_S_ChargePenalty_Table = new DataTable();
                IEntity t_S_ChargePenalty_Entity = WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(chargePenaltyKey);
                string refMasterFieldName = t_S_ChargePenalty_Entity.Associations[chargePenaltyAssociationKey].RoleDataAttribute.Name;

                db.ExecQuery(string.Format("delete from {0} where {1} = {2}",
                                           t_S_ChargePenalty_Entity.FullDBName, refMasterFieldName, credit.ID),
                             QueryResultTypes.NonQuery);

                IDbDataParameter[] prms = new IDbDataParameter[] { new System.Data.OleDb.OleDbParameter(refMasterFieldName, credit.ID) };
                using (IDataUpdater du = t_S_ChargePenalty_Entity.GetDataUpdater(String.Format("{0} = ?", refMasterFieldName), null, prms))
                {
                    du.Fill(ref t_S_ChargePenalty_Table);
                    Penalties penalty = new Penalties(t_S_PlanDebtCI_Table, t_S_FactDebtCI_Table, credit.RefOKV);
                    DataRow[] planRows = t_S_PlanDebtCI_Table.Select(string.Empty, "EndDate Asc");

                    int payment = 1;
                    int refEkr = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_ChargePenalty_Entity.ObjectKey,
                        SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                    int refR = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_ChargePenalty_Entity.ObjectKey,
                        SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                    int refKD = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanService_Key,
                        SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);

                    PenaltiesHelper penaltiesHelper = new PenaltiesHelper(scheme);
                    penaltiesHelper.GetPenalties(t_S_PlanDebtCI_Table, t_S_FactDebtCI_Table, credit, percentName, ref t_S_ChargePenalty_Table);
                    foreach (DataRow penaltyRow in t_S_ChargePenalty_Table.Rows)
                    {
                        penaltyRow["ID"] = t_S_ChargePenalty_Entity.GetGeneratorNextValue;
                        penaltyRow["Payment"] = payment;
                        if (credit.CreditsType == CreditsTypes.BudgetOutcoming || credit.CreditsType == CreditsTypes.OrganizationOutcoming)
                            penaltyRow["RefKD"] = refKD;
                        else
                        {
                            penaltyRow["RefEKR"] = refEkr;
                            penaltyRow["RefR"] = refR;
                        }

                        penaltyRow["PenaltyRate"] = ratePenalty;
                        penaltyRow[refMasterFieldName] = credit.ID;
                        //t_S_ChargePenalty_Table.Rows.Add(penaltyRow);
                        payment++;
                    }
                    /*
                    foreach (DataRow row in planRows)
                    {
                        DateTime endPeriod = GetPenaltyEndPeriod(Convert.ToDateTime(row["EndDate"]), credit.CreditsType, percentName, holydays, scheme);
                        DataRow[] penaltyRows = penalty.CalculatePenalty(endPeriod, t_S_ChargePenalty_Table, percentName, credit);

                        if (penaltyRows != null)
                        {
                            foreach (DataRow penaltyRow in penaltyRows)
                            {
                                penaltyRow["ID"] = t_S_ChargePenalty_Entity.GetGeneratorNextValue;
                                penaltyRow["Payment"] = payment;
                                if (credit.CreditsType == CreditsTypes.BudgetOutcoming || credit.CreditsType == CreditsTypes.OrganizationOutcoming)
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
                    }*/
                    du.Update(ref t_S_ChargePenalty_Table);
                }

                #endregion Формируем деталь "ИФ.Начисление пени по основному долгу"
            }
        }

        private void FillMainDebtPenalty()
        {
            
        }

        private void FillPercentsPenalty()
        {
            
        }

        /// <summary>
        /// расчитываем дату окончания периода, по которому пени не считаются (до 10 числа месяца + выходные)
        /// </summary>
        private static DateTime GetPenaltyEndPeriod(DateTime endPeriod,
            CreditsTypes type, string penaltyName, Holidays holydays, IScheme scheme)
        {
            DateTime newEndPeriod = endPeriod;
            if (penaltyName == "PenaltyDebt")
                return newEndPeriod;

            if (type == CreditsTypes.BudgetOutcoming && string.Compare(penaltyName, "PenaltyPercent", true) == 0)
            {
                if (scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString() == "63 000 000")
                    return holydays.GetWorkDate(newEndPeriod).AddDays(1);

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
            }
            // учитываем субботы и воскресенья
            return holydays.GetWorkDate(newEndPeriod).AddDays(1);
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

            IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;

            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                IEntityAssociation entityAssociation =
                    WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(associationKey);
                IEntity entity = entityAssociation.RoleData;

                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 6, baseYear);
                int refEkr = scheme.FinSourcePlanningFace.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                int refR = scheme.FinSourcePlanningFace.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                int refKD = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanService_Key,
                    SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);

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
                    if (credit.CreditsType == CreditsTypes.BudgetOutcoming || credit.CreditsType == CreditsTypes.OrganizationOutcoming)
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
        public Dictionary<string, string> CalculatePenaltiesMainDebt(DataRow[] masterRows, int baseYear)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            Holidays holydays = new Holidays(WorkplaceSingleton.Workplace.ActiveScheme);
            foreach (DataRow masterRow in masterRows)
            {
                try
                {
                    if (Convert.ToInt32(masterRow["RefSStatusPlan"]) == 0 &&
                        Convert.ToInt32(masterRow["RefVariant"]) == FinSourcePlanningNavigation.Instance.CurrentVariantID)
                    {
                        double ratePenalty = masterRow["PenaltyDebtRate"] is DBNull ? 1 : Convert.ToDouble(masterRow["PenaltyDebtRate"]);
                        Credit credit = new Credit(masterRow);
                        CalculatePenalties(credit, ratePenalty, baseYear,
                                           a_S_FactDebt_RefCredit_Key,
                                           a_S_PlanDebt_RefCredit_Key,
                                           t_S_ChargePenaltyDebt_Key,
                                           a_S_ChargePenaltyDebt_RefCredit_Key,
                                           "PenaltyDebt", holydays);
                    }
                }
                catch (FinSourcePlanningException e)
                {
                    errors.Add(string.Format("{0} ({1})",masterRow["Num"], masterRow["ID"]), e.Message);
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
            Holidays holydays = new Holidays(WorkplaceSingleton.Workplace.ActiveScheme);
            foreach (DataRow masterRow in masterRows)
            {
                try
                {
                    if (Convert.ToInt32(masterRow["RefSStatusPlan"]) == 0 &&
                        Convert.ToInt32(masterRow["RefVariant"]) == FinSourcePlanningNavigation.Instance.CurrentVariantID)
                    {
                        double ratePenalty = masterRow["PenaltyDebtRate"] is DBNull ? 1 : Convert.ToDouble(masterRow["PenaltyDebtRate"]);
                        Credit credit = new Credit(masterRow);
                        CalculatePenalties(credit, ratePenalty, baseYear,
                                           a_S_FactPercent_RefCredit_Key,
                                           a_S_PlanService_RefCredit_Key,
                                           t_S_ChargePenaltyPercent_Key,
                                           a_S_ChargePenaltyPercent_RefCredit_Key,
                                           "PenaltyPercent", holydays);
                    }
                }
                catch (FinSourcePlanningException e)
                {
                    errors.Add(Convert.ToString(masterRow["Num"]), e.Message);
                }
            }
            return errors;
        }

        #endregion

    }

    public partial class CurrencyCreditServer : FinSourcePlanningServer
    {

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

            IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;

            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                IEntityAssociation entityAssociation =
                    WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(associationKey);
                IEntity entity = entityAssociation.RoleData;

                int sourceID = Utils.GetDataSourceID(db, "ФО", 29, 7, baseYear);
                // Получаем ссылку на классификатор "ЭКР.Планирование"
                int refEkr = scheme.FinSourcePlanningFace.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                int refR = scheme.FinSourcePlanningFace.GetCreditClassifierRef(entity.ObjectKey,
                    SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
                int refKD = scheme.FinSourcePlanningFace.GetCreditClassifierRef(t_S_PlanService_Key,
                    SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);

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
                    if (credit.CreditsType == CreditsTypes.BudgetOutcoming || credit.CreditsType == CreditsTypes.OrganizationOutcoming)
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

    }

    internal class PenaltiesHelper
    {
        private IScheme scheme;

        private Holidays holydays;

        internal PenaltiesHelper(IScheme scheme)
        {
            this.scheme = scheme;
            holydays = new Holidays(scheme);
        }

        internal void GetPenalties(DataTable planTable, DataTable factTable, Credit credit, string penaltyName, ref DataTable penaltyTable)
        {
            List<DateTime> periods = new List<DateTime>();
            foreach (DataRow row in planTable.Select(string.Format("EndDate <= '{0}'", DateTime.Today)))
            {
                DateTime date = GetPenaltyEndPeriod(Convert.ToDateTime(row["EndDate"]), credit.CreditsType, penaltyName, holydays, scheme).AddDays(-1);
                if (!periods.Contains(date))
                    periods.Add(date);
            }

            foreach (DataRow row in factTable.Select(string.Format("FactDate <= '{0}'", DateTime.Today)))
            {
                DateTime date = Convert.ToDateTime(row["FactDate"]);
                if (!periods.Contains(date))
                    periods.Add(date);
            }
            if (!periods.Contains(DateTime.Now))
                periods.Add(DateTime.Now);

            periods.Sort();

            for (int periodIndex = 0; periodIndex <= periods.Count - 2; periodIndex++)
            {
                DateTime date = periods[periodIndex];
                DateTime nextDate = periods[periodIndex + 1];

                decimal planSum = GetSum(planTable, date, "EndDate", credit.RefOKV == -1 ? "Sum" : "CurrencySum");
                decimal factSum = GetSum(factTable, date, "FactDate", credit.RefOKV == -1 ? "Sum" : "CurrencySum");
                if (planSum > factSum)
                {
                    // просроченный платеж, начисляем пени
                    decimal penaltyRate = GetPercent(credit, nextDate, penaltyName);
                    decimal k = Math.Round(penaltyRate / 100, 10, MidpointRounding.AwayFromZero);
                    decimal r = planSum - factSum;
                    // Внимание, постановка поменялась. Считаем пени конечная дата - начальная + 1 день
                    decimal penalty = r * ((nextDate - date).Days) * k;

                    DataRow penaltyRow = penaltyTable.NewRow();
                    if (credit.RefOKV == -1)
                    {
                        penaltyRow["Sum"] = penalty;
                    }
                    else
                    {
                        decimal exchangeRate = Utils.GetLastCurrencyExchange(DateTime.Today, credit.RefOKV);
                        penaltyRow["CurrencySum"] = penalty;
                        penaltyRow["Sum"] = penalty * exchangeRate;
                        penaltyRow["ExchangeRate"] = exchangeRate;
                        penaltyRow["IsPrognozExchRate"] = false;
                    }
                    // дату начисления ставим как последний день месяца, в котором начинаем начислять пени
                    penaltyRow["Penalty"] = k;
                    penaltyRow["StartDate"] = new DateTime(date.Year, date.Month,
                                                           DateTime.DaysInMonth(date.Year,
                                                                                date.Month));
                    penaltyRow["LateDate"] = (nextDate - date).Days;
                    penaltyRow["RefOKV"] = credit.RefOKV;
                    penaltyRow["PenaltySum"] = r;
                    penaltyRow["StartPenaltyDate"] = date;
                    penaltyRow["EndPenaltyDate"] = nextDate;
                    penaltyTable.Rows.Add(penaltyRow);
                }
            }
        }

        private decimal GetSum(DataTable table, DateTime time, string dateColumn, string sumColumn)
        {
            decimal sum = 0;
            foreach (DataRow row in table.Select(string.Format("{0} <= '{1}'", dateColumn, time)))
            {
                decimal value;
                if (!row.IsNull(sumColumn) && decimal.TryParse(row[sumColumn].ToString(), out value))
                {
                    sum += value;
                }
            }
            return sum;
        }

        /// <summary>
        /// расчитываем дату окончания периода, по которому пени не считаются (до 10 числа месяца + выходные)
        /// </summary>
        private static DateTime GetPenaltyEndPeriod(DateTime endPeriod,
            CreditsTypes type, string penaltyName, Holidays holydays, IScheme scheme)
        {
            DateTime newEndPeriod = endPeriod;
            // для пени по основному долгу пофиг на выходные, дата начисления пени=дате окончания периода
            if (penaltyName == "PenaltyDebt")
                return newEndPeriod.AddDays(0);

            if (type == CreditsTypes.BudgetOutcoming && string.Compare(penaltyName, "PenaltyPercent", true) == 0)
            {
                if (scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString() == "63 000 000")
                    return holydays.GetWorkDate(newEndPeriod).AddDays(1);

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
            }
            // учитываем субботы и воскресенья
            return holydays.GetWorkDate(newEndPeriod).AddDays(1);
        }

        /// <summary>
        /// получаем процент для пени на определенную дату
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="percentDate"></param>
        /// <param name="percentName"></param>
        /// <returns></returns>
        private static Decimal GetPercent(Credit credit, DateTime percentDate, string percentName)
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
        public DataRow[] CalculatePenalty(DateTime planDate, DataTable dtPenalty, string penaltyName, Credit credit)
        {
            if (planDate > DateTime.Today)
                planDate = DateTime.Today;

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
            if (factTable.Select(string.Format("FactDate >= '{0}'", DateTime.Today)).Length == 0)
            {
                DataRow emptyFact = factTable.NewRow();
                emptyFact["FactDate"] = DateTime.Today;
                emptyFact["Sum"] = 0;
                if (factTable.Columns.Contains("CurrencySum"))
                    emptyFact["CurrencySum"] = 0;
                factTable.Rows.Add(emptyFact);
            }

            factPayments = factTable.Select(string.Format("FactDate > '{0}'", planDate), "FactDate Asc");

            List<DataRow> penalties = new List<DataRow>();

            foreach (DataRow row in factPayments)
            {
                DateTime factDate = Convert.ToDateTime(row["FactDate"]);
                if (planDate <= DateTime.Today && currentPlanSum > currentFactSum)
                {
                    decimal penaltyRate = GetPercent(credit, factDate, penaltyName);

                    decimal k = Math.Round(penaltyRate / 100, 10, MidpointRounding.AwayFromZero);

                    decimal r = currentPlanSum - currentFactSum;
                    // Внимание, постановка поменялась. Считаем пени конечная дата - начальная + 1 день
                    decimal penalty = r * ((factDate - planDate).Days) * k;

                    if (prevEndPeriodDate >= factDate)
                    {
                        penalty -= prevPenalty / prevPeriodDaysCount * ((prevEndPeriodDate - factDate).Days);
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
                            penaltyRow["Sum"] = penalty*exchangeRate;
                            penaltyRow["ExchangeRate"] = exchangeRate;
                            penaltyRow["IsPrognozExchRate"] = false;
                        }
                        // дату начисления ставим как последний день месяца, в котором начинаем начислять пени
                        penaltyRow["Penalty"] = k;
                        penaltyRow["StartDate"] = new DateTime(planDate.Year, planDate.Month,
                                                               DateTime.DaysInMonth(planDate.Year,
                                                                                    planDate.Month));
                        penaltyRow["LateDate"] = (factDate - planDate).Days;
                        penaltyRow["RefOKV"] = refOKV;
                        penaltyRow["PenaltySum"] = r;
                        penaltyRow["StartPenaltyDate"] = planDate;
                        penaltyRow["EndPenaltyDate"] = factDate;
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

        /// <summary>
        /// получаем процент для пени на определенную дату
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="percentDate"></param>
        /// <param name="percentName"></param>
        /// <returns></returns>
        private static Decimal GetPercent(Credit credit, DateTime percentDate, string percentName)
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
    }


    /*
    [TestFixture]
    public class PenaltiesTest
    {
        [Test(Description = "Добавление платежа")]
        public void CalculatePenaltiesToDateTest()
        {
            decimal penalties;

            Penalties pen = new Penalties(1, new DateTime(2008, 9, 10), 30000, 12, -1);
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 9, 10));
            Assert.AreEqual(0, penalties);
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 9, 13));
            Assert.AreEqual(29.51, penalties);

            pen.AddPayment(new Payment(5000, new DateTime(2008, 9, 14), 0));
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 9, 14));
            Assert.AreEqual(39.34, penalties);
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 9, 15));
            Assert.AreEqual(40.98, penalties);
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 9, 19));
            Assert.AreEqual(73.77, penalties);

            pen.AddPayment(new Payment(7000, new DateTime(2008, 9, 19), 0));
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 9, 19));
            Assert.AreEqual(73.77, penalties);
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 9, 20));
            Assert.AreEqual(59.02, penalties);
            penalties = pen.CalculatePenaltiesToDate(new DateTime(2008, 10, 3));
            Assert.AreEqual(135.74, penalties);
        }
    }*/
}
