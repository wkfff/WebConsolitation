using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Server.FinSourcePlanning.Services;
using Krista.FM.ServerLibrary;
using CreditsTypes = Krista.FM.ServerLibrary.FinSourcePlanning.CreditsTypes;

namespace Krista.FM.Server.FinSourcePlanning.Constants
{

    #region дополнительные классы для заполнения ссылок в деталях договора

    internal class FinSourcesConstsHelper
    {
        private readonly IScheme scheme;
        private DataTable constTable;

        internal FinSourcesConstsHelper(IScheme scheme)
        {
            this.scheme = scheme;
            constTable = new DataTable();
            FillConstsTable();
        }

        internal void FillConstsTable()
        {
            IEntity constEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_Constant_Key);
            constTable.Clear();
            using (IDataUpdater upd = constEntity.GetDataUpdater())
            {
                upd.Fill(ref constTable);
            }
        }

        /// <summary>
        /// получение данных из таблицы с константами
        /// </summary>
        private object[] GetConstDataByName(string constName)
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
            return string.Join("_", new string[] { ((int)creditType).ToString(), detailObjectKey, classifierKey, refOKV.ToString(), refTerrType.ToString() });
        }

        private string GetConstKey(string detailObjectKey, string classifierKey, int refOKV)
        {
            return string.Join("_", new string[] { detailObjectKey, classifierKey, refOKV.ToString() });
        }

        private string GetConstKey(string classifierKey, bool isRegress)
        {
            return string.Join("_", new string[] { classifierKey, isRegress.ToString() });
        }

        private string GetConstKey(string detailObjectKey, string classifierKey, bool isRegress)
        {
            return string.Join("_", new string[] { detailObjectKey, classifierKey, isRegress.ToString() });
        }

        #endregion

        private void UpdateConstKey(string constKey, string constName)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntity constEntity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_Constant_Key);
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
                && creditType == CreditsTypes.BudgetIncoming/* && refOKV == -1*/)
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
                && creditType == CreditsTypes.BudgetIncoming/* && refOKV == -1*/)
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
        private string GetSourcePlaningGuaranteeConstName(string detailObjectKey, string classifierKey, bool isRegress)
        {
            string constName = string.Empty;
            string constKey = GetConstKey(detailObjectKey, classifierKey, isRegress);
            if ((detailObjectKey == GuaranteeIssuedObjectKeys.t_S_PlanAttractGrnt_Key || detailObjectKey == GuaranteeIssuedObjectKeys.t_S_FactAttractGrnt_Key) &&
                classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key && isRegress)
                constName = "KIFGrnt";

            if ((detailObjectKey == GuaranteeIssuedObjectKeys.t_S_PlanDebtPrGrnt_Key ||
                detailObjectKey == GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key ||
                detailObjectKey == GuaranteeIssuedObjectKeys.t_S_FactDebtPrGrnt_Key ||
                detailObjectKey == GuaranteeIssuedObjectKeys.t_S_FactPercentPrGrnt_Key) &&
                classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key && isRegress)
                constName = "KIFGrntPr";

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
                constName = "KOSGYSeviceForgnDebt";

            if (detailObjectKey == CapitalObjectKeys.t_S_CPRateSwitch && classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key)
                constName = "KIFDiffRate";

            if (!string.IsNullOrEmpty(constName))
                UpdateConstKey(constKey, constName);

            return constName;
        }

        /// <summary>
        /// получение ссылки на классификатор по данным константы
        /// </summary>
        private int GetClassifierRef(string classifierKey, int sourceID, string code, string name, string constName, bool addNewRow, IDatabase db)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
                return -1;

            switch (classifierKey)
            {
                case SchemeObjectsKeys.d_KIF_Plan_Key:
                    int rowId = Utils.GetClassifierRowID(scheme, db, classifierKey, sourceID, "CodeStr", code, name, addNewRow);
                    SetCourse(GetCourse(constName), rowId, db);
                    return rowId;
                case SchemeObjectsKeys.d_R_Plan_Key:
                    return Utils.GetClassifierRowID(scheme,
                        db, classifierKey, sourceID, "Code", code, name, addNewRow);
                case SchemeObjectsKeys.d_EKR_PlanOutcomes_Key:
                    return Utils.GetClassifierRowID(scheme,
                    db, classifierKey, sourceID, "Code", code, name, addNewRow);
                case SchemeObjectsKeys.d_KD_Plan_Key:
                    return Utils.GetClassifierRowID(scheme, db, classifierKey, sourceID, "CodeStr", code, name, addNewRow);
            }
            return -1;
        }

        internal int GetCreditClassifierRef(string detailObjectKey, string classifierKey,
            int sourceID, int refOKV, CreditsTypes creditType, int refTerrType, string programCode, IDatabase db)
        {
            // ищем наименование константы, потом получаем ссылку на классификатор
            string constName = GetSourcePlaningCreditConstName(detailObjectKey,
                classifierKey, creditType, refOKV, refTerrType);
            object[] classifierData = GetConstDataByName(constName);
            if (classifierData != null)
            {
                // добавим код администратора новый, если он есть
                if (classifierKey == SchemeObjectsKeys.d_KIF_Plan_Key &&
                    !string.IsNullOrEmpty(programCode))
                {
                    string newCode = string.Concat(classifierData[0].ToString().Substring(0, 13), programCode,
                                                      classifierData[0].ToString().Substring(17, 3));
                    int id = GetClassifierRef(classifierKey, sourceID, newCode,
                                        classifierData[1].ToString(), constName, false, db);
                    if (id != -1)
                        return id;
                }
                return GetClassifierRef(classifierKey, sourceID, classifierData[0].ToString(),
                                        classifierData[1].ToString(), constName, true, db);
            }
            return -1;
        }

        internal int GetGuaranteeClassifierRef(string detailObjectKey, string classifierKey, bool isRegress, int sourceID, IDatabase db)
        {
            string constName = GetSourcePlaningGuaranteeConstName(detailObjectKey, classifierKey, isRegress);
            object[] classifierData = GetConstDataByName(constName);
            if (classifierData != null)
                return GetClassifierRef(classifierKey, sourceID, classifierData[0].ToString(), classifierData[1].ToString(), constName, true, db);
            return -1;
        }

        internal int GetCapitalClassifierRef(string detailObjectKey, string classifierKey, int sourceID, int refOKV, IDatabase db)
        {
            string constName = GetSourcePlaningCapitalConstName(detailObjectKey, classifierKey, refOKV);
            object[] classifierData = GetConstDataByName(constName);
            if (classifierData != null)
                return GetClassifierRef(classifierKey, sourceID, classifierData[0].ToString(), classifierData[1].ToString(), constName, true, db);
            return -1;
        }

        /// <summary>
        /// установка ссылок для всех типов договоров
        /// </summary>
        /// <returns></returns>
        internal void SetReferences(int sourceId, IDatabase db)
        {
            FillConstsTable();

            if (db != null)
            {
                SetCreditsIncomingReferences(sourceId, db);
                SetCreditsIssuedReferences(sourceId, db);
                SetGuaranteesReferences(sourceId, db);
                SetCapitalsReferences(sourceId, db);
            }
            else
            {
                using (IDatabase innerDb = scheme.SchemeDWH.DB)
                {
                    SetCreditsIncomingReferences(sourceId, innerDb);
                    SetCreditsIssuedReferences(sourceId, innerDb);
                    SetGuaranteesReferences(sourceId, innerDb);
                    SetCapitalsReferences(sourceId, innerDb);
                }
            }
        }

        #region установка ссылок для всех кредитов

        private void SetCreditsIncomingReferences(int sourceId, IDatabase db)
        {
            IEntity creditsIncoming = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key);
            List<IEntityAssociation> masterDetailAssociations = GetMasterDetailAssociations(creditsIncoming);
            using (IDataUpdater du = creditsIncoming.GetDataUpdater())
            {
                DataTable dtCredits = new DataTable();
                du.Fill(ref dtCredits);
                foreach (DataRow masterRow in dtCredits.Rows)
                {
                    Credit credit = new Credit(masterRow, scheme);
                    foreach (IEntityAssociation association in masterDetailAssociations)
                        SetCreditReferences(association.RoleData, sourceId, credit, db);
                }
            }
        }

        private void SetCreditsIssuedReferences(int sourceId, IDatabase db)
        {
            IEntity creditsIssued = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Creditissued_Key);
            List<IEntityAssociation> masterDetailAssociations = GetMasterDetailAssociations(creditsIssued);
            using (IDataUpdater du = creditsIssued.GetDataUpdater())
            {
                DataTable dtCredits = new DataTable();
                du.Fill(ref dtCredits);
                foreach (DataRow masterRow in dtCredits.Rows)
                {
                    Credit credit = new Credit(masterRow, scheme);
                    foreach (IEntityAssociation association in masterDetailAssociations)
                        SetCreditReferences(association.RoleData, sourceId, credit, db);
                }
            }
        }

        private void SetCreditReferences(IEntity detailEntity, int sourceID, Credit credit, IDatabase db)
        {
            using (IDataUpdater duDetail = detailEntity.GetDataUpdater(string.Format("RefCreditInc = {0}", credit.ID), null))
            {
                DataTable dtDetail = new DataTable();
                duDetail.Fill(ref dtDetail);
                foreach (DataRow detailRow in dtDetail.Rows)
                {
                    if (detailRow.Table.Columns.Contains("RefKIF"))
                        detailRow["RefKIF"] = GetCreditClassifierRef(detailEntity.ObjectKey, SchemeObjectsKeys.d_KIF_Plan_Key,
                            sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode, db);
                    if (detailRow.Table.Columns.Contains("RefEKR"))
                        detailRow["RefEKR"] = GetCreditClassifierRef(detailEntity.ObjectKey,
                            SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, string.Empty, db);
                    if (detailRow.Table.Columns.Contains("RefR"))
                        detailRow["RefR"] = GetCreditClassifierRef(detailEntity.ObjectKey,
                            SchemeObjectsKeys.d_R_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, string.Empty, db);
                    if (detailRow.Table.Columns.Contains("RefKD"))
                        detailRow["RefKD"] = GetCreditClassifierRef(detailEntity.ObjectKey,
                            SchemeObjectsKeys.d_KD_Plan_Key, sourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, string.Empty, db);
                }
                DataTable dtChanges = dtDetail.GetChanges();
                if (dtChanges != null)
                    duDetail.Update(ref dtChanges);
            }
        }

        #endregion

        #region установка ссылок во всех деталях для гарантий

        private void SetGuaranteesReferences(int sourceId, IDatabase db)
        {
            IEntity guarantees = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Guarantissued_Key);
            List<IEntityAssociation> masterDetailAssociations = GetMasterDetailAssociations(guarantees);

            using (IDataUpdater du = guarantees.GetDataUpdater())
            {
                DataTable dtCredits = new DataTable();
                du.Fill(ref dtCredits);
                foreach (DataRow masterRow in dtCredits.Rows)
                {
                    bool isRegress = Convert.ToBoolean(masterRow["Regress"]);
                    foreach (IEntityAssociation association in masterDetailAssociations)
                        SetGuaranteeReferences(association.RoleData, sourceId, masterRow["ID"], isRegress, db);
                }
            }
        }

        private void SetGuaranteeReferences(IEntity detailEntity, int sourceId, object masterId, bool isRegress, IDatabase db)
        {
            using (IDataUpdater duDetail = detailEntity.GetDataUpdater(string.Format("RefGrnt = {0}", masterId), null))
            {
                DataTable dtDetail = new DataTable();
                duDetail.Fill(ref dtDetail);
                foreach (DataRow detailRow in dtDetail.Rows)
                {
                    if (detailRow.Table.Columns.Contains("RefKIF"))
                        detailRow["RefKIF"] = GetGuaranteeClassifierRef(detailEntity.ObjectKey, SchemeObjectsKeys.d_KIF_Plan_Key, isRegress, sourceId, db);
                    if (detailRow.Table.Columns.Contains("RefEKR"))
                        detailRow["RefEKR"] = GetGuaranteeClassifierRef(detailEntity.ObjectKey, SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, isRegress, sourceId, db);
                    if (detailRow.Table.Columns.Contains("RefR"))
                        detailRow["RefR"] = GetGuaranteeClassifierRef(detailEntity.ObjectKey, SchemeObjectsKeys.d_R_Plan_Key, isRegress, sourceId, db);
                }
                DataTable dtChanges = dtDetail.GetChanges();
                if (dtChanges != null)
                    duDetail.Update(ref dtChanges);
            }
        }

        #endregion

        #region установка ссылок для ценных бумаг

        private void SetCapitalsReferences(int sourceId, IDatabase db)
        {
            IEntity capitals = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Capital_Key);
            IList<IEntityAssociation> masterDetailAssociations = GetMasterDetailAssociations(capitals);

            using (IDataUpdater du = capitals.GetDataUpdater())
            {
                DataTable dtCredits = new DataTable();
                du.Fill(ref dtCredits);
                foreach (DataRow masterRow in dtCredits.Rows)
                {
                    foreach (IEntityAssociation association in masterDetailAssociations)
                        SetCapitalReferences(association.RoleData, sourceId, masterRow["ID"], Convert.ToInt32(masterRow["REfOKV"]), db);
                }
            }
        }

        private void SetCapitalReferences(IEntity detailEntity, int sourceId, object masterId, int refOkv, IDatabase db)
        {
            using (IDataUpdater duDetail = detailEntity.GetDataUpdater(string.Format("RefCap = {0}", masterId), null))
            {
                DataTable dtDetail = new DataTable();
                duDetail.Fill(ref dtDetail);
                foreach (DataRow detailRow in dtDetail.Rows)
                {
                    if (detailRow.Table.Columns.Contains("RefKIF"))
                        detailRow["RefKIF"] =
                            GetCapitalClassifierRef(detailEntity.ObjectKey, SchemeObjectsKeys.d_KIF_Plan_Key, sourceId, refOkv, db);
                    if (detailRow.Table.Columns.Contains("RefEKR"))
                        detailRow["RefEKR"] =
                            GetCapitalClassifierRef(detailEntity.ObjectKey, SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, sourceId, refOkv, db);
                    if (detailRow.Table.Columns.Contains("RefR"))
                        detailRow["RefR"] = GetCapitalClassifierRef(detailEntity.ObjectKey, SchemeObjectsKeys.d_R_Plan_Key,
                            sourceId, refOkv, db);
                }
                DataTable dtChanges = dtDetail.GetChanges();
                if (dtChanges != null)
                    duDetail.Update(ref dtChanges);
            }
        }

        #endregion

        /// <summary>
        /// получение списка ассоциаций типа мастер-деталь
        /// </summary>
        /// <param name="masterEntity"></param>
        /// <returns></returns>
        private List<IEntityAssociation> GetMasterDetailAssociations(IEntity masterEntity)
        {
            List<IEntityAssociation> masterDetailAssociations = new List<IEntityAssociation>();
            foreach (IEntityAssociation association in masterEntity.Associated.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.MasterDetail)
                {
                    if (association.RoleData.Attributes.ContainsKey("RefKIF") ||
                        association.RoleData.Attributes.ContainsKey("RefEKR") ||
                        association.RoleData.Attributes.ContainsKey("RefR") ||
                        association.RoleData.Attributes.ContainsKey("RefKD"))
                        masterDetailAssociations.Add(association);
                }
            }
            return masterDetailAssociations;
        }

        #region установка направления в классификаторе Киф планирование

        private int GetCourse(string constName)
        {
            switch (constName)
            {
                case "KIFCILendAgnc":
                case "KIFCILendAgncForgn":
                case "KIFCOBudgReturn":
                case "KIFCIBudg":
                case "KIFCOBudgReturnMR":
                case "KIFCOBudgReturnPos":
                case "KIFCOReturnPerson":
                case "KIFCapital":
                case "KIFCapitalForgn":
                case "KIFStockSale":
                case "KIFGrntPr":
                    return 1;
                case "KIFCILendAgncRepay":
                case "KIFCILendAgncForgnRepay":
                case "KIFCIBudgRepay":
                case "KIFCOBudgExt":
                case "KIFCOBudgExtMR":
                case "KIFCOBudgExtPos":
                case "KIFCOBudgExtPerson":
                case "KIFGrnt":
                case "KIFRetireCapital":
                case "KIFRetireCapitalForgn":
                    return -1;
                default:
                    return 0;
            }
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
            string query = string.Format("update d_KIF_Plan set RefKIF = {0} where ID = {1}",
                course, rowId);
            db.ExecQuery(query, QueryResultTypes.NonQuery);
        }

        #endregion
    }

    #endregion

}
