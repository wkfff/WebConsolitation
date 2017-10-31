using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIssued;

namespace Krista.FM.Client.Reports.Planning.Data
{
    // объект для предоставленных кредитов
    class CreditIssuedDataObject : CommonDataObject
    {
        protected override void InitDetailDateLists()
        {
            const int detailCount = 10;
            dtlKeysList = new string[detailCount];
            loDatesList = new string[detailCount];
            hiDatesList = new string[detailCount];
            var listCounter = 0;
            // 0
            dtlKeysList[listCounter] = t_S_PlanAttractCO.MasterReference;
            loDatesList[listCounter] = t_S_PlanAttractCO.StartDate;
            hiDatesList[listCounter++] = t_S_PlanAttractCO.EndDate;
            // 1
            dtlKeysList[listCounter] = t_S_PlanDebtCO.MasterReference;
            loDatesList[listCounter] = t_S_PlanDebtCO.StartDate;
            hiDatesList[listCounter++] = t_S_PlanDebtCO.EndDate;
            // 2
            dtlKeysList[listCounter] = t_S_PlanServiceCO.MasterReference;
            loDatesList[listCounter] = t_S_PlanServiceCO.StartDate;
            hiDatesList[listCounter++] = t_S_PlanServiceCO.EndDate;
            // 3
            dtlKeysList[listCounter] = t_S_FactAttractCO.MasterReference;
            hiDatesList[listCounter++] = t_S_FactAttractCO.FactDate;
            // 4
            dtlKeysList[listCounter] = t_S_FactDebtCO.MasterReference;
            hiDatesList[listCounter++] = t_S_FactDebtCO.FactDate;
            // 5
            dtlKeysList[listCounter] = t_S_ChargePenaltyDebtCO.MasterReference;
            loDatesList[listCounter++] = t_S_ChargePenaltyDebtCO.StartDate;
            // 6
            dtlKeysList[listCounter] = t_S_COChargePenaltyPercent.MasterReference;
            loDatesList[listCounter++] = t_S_COChargePenaltyPercent.StartDate;
            // 7
            dtlKeysList[listCounter] = t_S_FactPercentCO.MasterReference;
            hiDatesList[listCounter++] = t_S_FactPercentCO.FactDate;
            // 8
            dtlKeysList[listCounter] = t_S_FactPenaltyDebtCO.MasterReference;
            hiDatesList[listCounter++] = t_S_FactPenaltyDebtCO.FactDate;
            // 9
            dtlKeysList[listCounter] = t_S_FactPenaltyPercentCO.MasterReference;
            hiDatesList[listCounter] = t_S_FactPenaltyPercentCO.FactDate;
        }

        protected override string GetContractDesc()
        {
            return "Кредит предоставленный";
        }

        public override string GetParentRefName()
        {
            return "RefCreditInc";
        }

        protected override string GetCollateralKey()
        {
            return t_S_CollateralCO.InternalKey;
        }

        protected override string GetMainTableKey()
        {
            return f_S_Creditissued.InternalKey;
        }

        protected override string GetJournalKey()
        {
            return t_S_JournalPercentCO.internalKey;
        }
    }
}
