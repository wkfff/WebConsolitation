using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    #region дополнительные классы для заполнения ссылок в деталях договора
    
    internal class FinSourcesRererencesUtils
    {
        private IScheme _scheme;
        private DataTable constTable;
        internal FinSourcesRererencesUtils(IScheme scheme)
        {
            _scheme = scheme;
            if (constTable == null)
            {
                IEntity constEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_Constant_Key);
                constTable = new DataTable();
                using (IDataUpdater upd = constEntity.GetDataUpdater())
                {
                    upd.Fill(ref constTable);
                }
            }
        }

        /// <summary>
        /// получение данных из таблицы с константами
        /// </summary>
        public object[] GetConstDataByName(string constName)
        {
            if (string.IsNullOrEmpty(constName))
                return null;

            DataRow[] rows = constTable.Select(string.Format("IDConst = '{0}'", constName));
            if (rows.Length == 0)
                return null;

            return new object[] { rows[0]["KBK"], rows[0]["Name"] };
        }

        /// <summary>
        /// получение данных из таблицы с константами
        /// </summary>
        private object[] GetConstDataByKey(string constKey)
        {
            if (string.IsNullOrEmpty(constKey))
                return null;

            DataRow[] rows = constTable.Select(string.Format("ConstKey = '{0}'", constKey));
            if (rows.Length == 0)
                return null;

            return new object[] { rows[0]["KBK"], rows[0]["Name"] };
        }

        #region Получение уникального ключа константы

        private string GetConstKey(string detailObjectKey, string classifierKey,
            CreditsTypes creditType, int refOKV, int refTerrType)
        {
            return string.Join("_", new string[]
                {((int) creditType).ToString(), detailObjectKey, classifierKey, refOKV.ToString(), refTerrType.ToString()});
        }

        private string GetConstKey(string detailObjectKey, string classifierKey, int refOKV)
        {
            return string.Join("_", new string[] { detailObjectKey, classifierKey, refOKV.ToString() });
        }

        private string GetConstKey(string classifierKey, bool isRegress)
        {
            return string.Join("_", new string[] { classifierKey, isRegress.ToString() });
        }

        #endregion

        private void UpdateConstKey(string constKey, string constName)
        {
            using (IDatabase db = _scheme.SchemeDWH.DB)
            {
                IEntity constEntity = _scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_Constant_Key);
                db.ExecQuery(string.Format("update {0} set ConstKey = '{1}' where IDConst = '{2}'", constEntity.FullDBName,
                    constKey, constName), QueryResultTypes.NonQuery);

                DataRow[] rows = constTable.Select(string.Format("IDConst = '{0}'", constName));

                foreach (DataRow row in rows)
                {
                    row["ConstKey"] = constKey;
                    row.AcceptChanges();
                }
            }
        }

        private string GetCreditConstName(string detailObjectKey, string classifierKey,
            CreditsTypes creditType, int refOKV, int refTerrType)
        {

            #region Кредиты полученные

            if ((detailObjectKey == SchemeObjectsKeys.t_S_PlanAttractCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactAttractCI_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.OrganizationIncoming && refOKV == -1)
                return "KIFCILendAgnc";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_PlanAttractCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactAttractCI_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.OrganizationIncoming && refOKV != -1)
                return "KIFCILendAgncForgn";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_PlanAttractCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactAttractCI_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetIncoming)
                return "KIFCIBudg";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_FactDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_PlanDebtCI_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.OrganizationIncoming && refOKV == -1)
                return "KIFCILendAgncRepay";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_FactDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_PlanDebtCI_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.OrganizationIncoming && refOKV != -1)
                return "KIFCILendAgncForgnRepay";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_FactDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_PlanDebtCI_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetIncoming)
                return "KIFCIBudgRepay";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_PlanServiceCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyPercentCI_Key)
                && classifierKey == SchemeObjectsKeys.d_R_Plan_Key &&
                (creditType == CreditsTypes.BudgetIncoming || creditType == CreditsTypes.OrganizationIncoming))
                return "RSeviceDebt";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_PlanServiceCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyPercentCI_Key)
                && classifierKey == SchemeObjectsKeys.d_EKR_PlanOutcomes_Key
                && creditType == CreditsTypes.BudgetIncoming)
                return "KOSGYSeviceDebt";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_PlanServiceCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyPercentCI_Key)
                && classifierKey == SchemeObjectsKeys.d_EKR_PlanOutcomes_Key
                && creditType == CreditsTypes.OrganizationIncoming && refOKV == -1)
                return "KOSGYSeviceDebt";

            if ((detailObjectKey == SchemeObjectsKeys.t_S_PlanServiceCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key
                || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyDebtCI_Key || detailObjectKey == SchemeObjectsKeys.t_S_FactPenaltyPercentCI_Key)
                && classifierKey == SchemeObjectsKeys.d_EKR_PlanOutcomes_Key
                && creditType == CreditsTypes.OrganizationIncoming && refOKV != -1)
                return "KOSGYSeviceForgnDebt";

            if (detailObjectKey == SchemeObjectsKeys.t_S_RateSwitchCI_Key
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && refOKV != -1)
                return "KIFDiffRate";

            #endregion

            #region кредиты предоставленные

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_FactAttractCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanAttractCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetOutcoming && refOKV == -1 && refTerrType == 0)
                return "KIFCOBudgExt";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_FactAttractCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanAttractCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetOutcoming && refOKV == -1 && (refTerrType == 7 || refTerrType == 4))
                return "KIFCOBudgExtMR";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_FactAttractCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanAttractCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetOutcoming && refOKV == -1 && (refTerrType == 5 || refTerrType == 6 || refTerrType == 11))
                return "KIFCOBudgExtPos";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_FactAttractCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanAttractCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.OrganizationOutcoming && refOKV == -1)
                return "KIFCOBudgExtPerson";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanDebtCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_FactDebtCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetOutcoming && refOKV == -1 && refTerrType == 0)
                return "KIFCOBudgReturn";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanDebtCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_FactDebtCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetOutcoming && refOKV == -1 && (refTerrType == 7 || refTerrType == 4))
                return "KIFCOBudgReturnMR";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanDebtCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_FactDebtCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.BudgetOutcoming && refOKV == -1 && (refTerrType == 5 || refTerrType == 6 || refTerrType == 11))
                return "KIFCOBudgReturnPos";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanDebtCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_FactDebtCO)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && creditType == CreditsTypes.OrganizationOutcoming && refOKV == -1)
                return "KIFCOReturnPerson";

            if ((detailObjectKey == CreditIssuedObjectsKeys.t_S_PlanServiceCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_FactPercentCO
                || detailObjectKey == CreditIssuedObjectsKeys.t_S_ChargePenaltyDebtCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_ChargePenaltyPercentCO
                || detailObjectKey == CreditIssuedObjectsKeys.t_S_FactPenaltyDebtCO || detailObjectKey == CreditIssuedObjectsKeys.t_S_FactPenaltyPercentCO)
                && classifierKey == SchemeObjectsKeys.d_KD_Plan_Key
                && (creditType == CreditsTypes.BudgetOutcoming || creditType == CreditsTypes.OrganizationOutcoming))
                return "KDRate";

            #endregion

            return string.Empty;
        }

        /// <summary>
        /// Код константы для ссылок в деталях кредитов
        /// </summary>
        private string GetSourcePlaningCreditConstName(string detailObjectKey, string classifierKey,
            CreditsTypes creditType, int refOKV, int refTerrType)
        {
            string constName = GetCreditConstName(detailObjectKey, classifierKey, creditType, refOKV, refTerrType);
            string constKey = GetConstKey(detailObjectKey, classifierKey, creditType, refOKV, refTerrType);

            if (!string.IsNullOrEmpty(constName))
                UpdateConstKey(constKey, constName);

            return constName;
        }

        /// <summary>
        /// Код константы для ссылок в деталях гарантий
        /// </summary>
        private string GetSourcePlaningGuaranteeConstName(string classifierKey, bool isRegress)
        {
            string constName = string.Empty;
            string constKey = GetConstKey(classifierKey, isRegress);
            if (classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key && isRegress)
                constName = "KIFGrnt";
            if (classifierKey == SchemeObjectsKeys.d_R_Plan_Key && !isRegress)
                constName = "RGrnt";
            if (classifierKey == SchemeObjectsKeys.d_EKR_PlanOutcomes_Key && !isRegress)
                constName = "KOSGYGrnt";
            if (!string.IsNullOrEmpty(constName))
                UpdateConstKey(constKey, constName);
            return constName;
        }

        /// <summary>
        /// Код константы для ссылок в деталях ценных бумаг
        /// </summary>
        private string GetSourcePlaningCapitalConstName(string detailObjectKey, string classifierKey, int refOKV)
        {
            string constName = string.Empty;
            string constKey = GetConstKey(detailObjectKey, classifierKey, refOKV);
            if ((detailObjectKey == CapitalObjectKeys.t_S_CPPlanCapital_Key || detailObjectKey == CapitalObjectKeys.t_S_CPFactCapital)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && refOKV == -1)
                constName = "KIFCapital";

            if ((detailObjectKey == CapitalObjectKeys.t_S_CPPlanCapital_Key || detailObjectKey == CapitalObjectKeys.t_S_CPFactCapital)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && refOKV != -1)
                constName = "KIFCapitalForgn";

            if ((detailObjectKey == CapitalObjectKeys.t_S_CPFactDebt || detailObjectKey == CapitalObjectKeys.t_S_CPPlanDebt_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && refOKV == -1)
                constName = "KIFRetireCapital";

            if ((detailObjectKey == CapitalObjectKeys.t_S_CPFactDebt || detailObjectKey == CapitalObjectKeys.t_S_CPPlanDebt_Key)
                && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key
                && refOKV != -1)
                constName = "KIFRetireCapitalForgn";

            if (classifierKey == SchemeObjectsKeys.d_R_Plan_Key)
                constName = "RSeviceDebt";

            if (classifierKey == SchemeObjectsKeys.d_EKR_PlanOutcomes_Key
                && refOKV == -1)
                constName = "KOSGYSeviceDebt";

            if (classifierKey == SchemeObjectsKeys.d_EKR_PlanOutcomes_Key
                && refOKV != -1)
                constName =  "KOSGYSeviceForgnDebt";

            if (detailObjectKey == CapitalObjectKeys.t_S_CPRateSwitch && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key)
                constName = "KIFDiffRate";

            if (!string.IsNullOrEmpty(constName))
                UpdateConstKey(constKey, constName);

            return constName;
        }

        /// <summary>
        /// проставляем курс в классификаторе киф планирование
        /// </summary>
        /// <param name="course"></param>
        /// <param name="rowId"></param>
        /// <param name="db"></param>
        private void SetCourse(int course, object rowId, IDatabase db)
        {
            if (course == 0)
                return;
            string query = string.Format("update d_KIF_Plan set RefKIF = {0} where ID = {1} and RefKIF = 0 or RefKIF is null",
                course, rowId);
            db.ExecQuery(query, QueryResultTypes.NonQuery);
        }
    }
    
    #endregion

}
