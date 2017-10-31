using System;
using System.Data;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public class DebtServer
    {
        /// <summary>
        ///  заполнение детали "задолженность"
        /// </summary>
        public void CalculateDebt(DateTime calculateDate, Credit credit)
        {
            IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;

            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.t_S_DebtCI_Key);
            string refMasterFieldName = entity.Associations[SchemeObjectsKeys.a_S_DebtCI_RefCreditInc_Key].RoleDataAttribute.Name;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                using (IDataUpdater upd = entity.GetDataUpdater("1 <> 2", null, null))
                {
                    DataTable dt = new DataTable();
                    upd.Fill(ref dt);

                    DataRow newRow = dt.NewRow();
                    newRow["ID"] = entity.GetGeneratorNextValue;
                    newRow["ReportingDate"] = calculateDate;
                    // типа данные по каждому кредиту. Выводим в некий отчет
                    decimal factMainDebt = GetMainFactDebts(db, credit.EndDate, credit.ID);
                    decimal factServiceDebt = GetServiceFactDebts(db, credit.EndDate, credit.ID);
                    // просроченная задолженность на расчетный период
                    decimal failPay = GetMainFactDebts(db, calculateDate, credit.ID) +
                                      GetServiceFactDebts(db, calculateDate, credit.ID);

                    newRow["CapitalDebt"] = factMainDebt;
                    newRow["ServiceDebt"] = factServiceDebt;
                    newRow["StaleDebt"] = failPay;

                    newRow[refMasterFieldName] = credit.ID;

                    dt.Rows.Add(newRow);
                    upd.Update(ref dt);
                }
            }
        }

        /// <summary>
        /// Фактическая задолженность по основному долгу
        /// </summary>
        public decimal GetMainFactDebts(IDatabase db, DateTime endPeriodDate, int masterID)
        {
            DataTable dtMainPlan = Utils.GetDetailTable(db, masterID, SchemeObjectsKeys.a_S_PlanDebtCI_RefCreditInc_Key);
            decimal planSum = 0;
            foreach (DataRow row in dtMainPlan.Rows)
            {
                if (Convert.ToDateTime(row["EndDate"]) <= endPeriodDate)
                    planSum += Convert.ToDecimal(row["Sum"]);
            }

            decimal factSum = 0;
            DataTable dtMainFact = Utils.GetDetailTable(db, masterID, SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key);
            foreach (DataRow row in dtMainFact.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= endPeriodDate)
                    factSum += Convert.ToDecimal(row["Sum"]);
            }

            return planSum - factSum;
        }

        /// <summary>
        /// Фактическая задолженность по обслуживанию
        /// </summary>
        public decimal GetServiceFactDebts(IDatabase db, DateTime endPeriodDate, int masterID)
        {
            DataTable dtMainPlan = Utils.GetDetailTable(db, masterID, SchemeObjectsKeys.a_S_PlanServiceCI_RefCreditInc_Key);
            // план обслуживания долга
            decimal planServiceSum = 0;
            foreach (DataRow row in dtMainPlan.Rows)
            {
                if (Convert.ToDateTime(row["EndDate"]) <= endPeriodDate)
                    planServiceSum += Convert.ToDecimal(row["Sum"]);
            }
            // факт обслуживания долга
            decimal factServiceSum = 0;
            DataTable dtMainFact = Utils.GetDetailTable(db, masterID, SchemeObjectsKeys.a_S_FactPercentCI_RefCreditInc_Key);
            foreach (DataRow row in dtMainFact.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= endPeriodDate)
                    factServiceSum += Convert.ToDecimal(row["Sum"]);
            }
            // начисленные пени
            decimal penaltiesSum = 0;
            DataTable dtPenalties = Utils.GetDetailTable(db, masterID,
                                                         SchemeObjectsKeys.a_S_ChargePenaltyDebtCI_RefCreditInc_Key);
            foreach (DataRow row in dtPenalties.Rows)
            {
                if (Convert.ToDateTime(row["StartDate"]) <= endPeriodDate)
                    penaltiesSum += Convert.ToDecimal(row["Sum"]);
            }
            dtPenalties = Utils.GetDetailTable(db, masterID,
                                                         SchemeObjectsKeys.a_S_ChargePenaltyPercentCI_RefCreditInc_Key);
            foreach (DataRow row in dtPenalties.Rows)
            {
                if (Convert.ToDateTime(row["StartDate"]) <= endPeriodDate)
                    penaltiesSum += Convert.ToDecimal(row["Sum"]);
            }

            // погашение пени
            decimal penaltiesFactSum = 0;
            DataTable dtPenaltiesPay = Utils.GetDetailTable(db, masterID,
                                                            SchemeObjectsKeys.a_S_FactPenaltyDebtCI_RefCreditInc_Key);
            foreach (DataRow row in dtPenaltiesPay.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= endPeriodDate)
                    penaltiesFactSum += Convert.ToDecimal(row["Sum"]);
            }
            dtPenaltiesPay = Utils.GetDetailTable(db, masterID,
                                                            SchemeObjectsKeys.a_S_FactPenaltyPercentCI_RefCreditInc_Key);
            foreach (DataRow row in dtPenaltiesPay.Rows)
            {
                if (Convert.ToDateTime(row["FactDate"]) <= endPeriodDate)
                    penaltiesFactSum += Convert.ToDecimal(row["Sum"]);
            }

            return (planServiceSum + penaltiesSum) - (factServiceSum + penaltiesFactSum);
        }
    }
}




