using System;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;

namespace Krista.FM.Client.Reports.Planning.Data
{
    // объект для гарантий
    class GarantDataObject : CommonDataObject
    {
        public bool filterLastPlanService = true;
        public string planServiceDate = DateTime.Now.ToShortDateString();

        protected override void InitDetailDateLists()
        {
            const int detailCount = 13;
            dtlKeysList = new string[detailCount];
            loDatesList = new string[detailCount];
            hiDatesList = new string[detailCount];
            var listCounter = 0;
            // 0
            dtlKeysList[listCounter] = t_S_FactAttractPrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_FactAttractPrGrnt.FactDate;
            // 1
            dtlKeysList[listCounter] = t_S_FactDebtPrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_FactDebtPrGrnt.FactDate;
            // 2
            dtlKeysList[listCounter] = t_S_FactAttractGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_FactAttractGrnt.FactDate;
            // 3
            dtlKeysList[listCounter] = t_S_PlanDebtPrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_PlanDebtPrGrnt.EndDate;
            // 4
            dtlKeysList[listCounter] = t_S_FactPercentPrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_FactPercentPrGrnt.FactDate;
            // 5
            dtlKeysList[listCounter] = t_S_PlanServicePrGrnt.MasterReference;
            loDatesList[listCounter] = t_S_PlanServicePrGrnt.StartDate;
            hiDatesList[listCounter++] = t_S_PlanServicePrGrnt.EndDate;
            // 6
            dtlKeysList[listCounter] = t_S_PlanAttractGrnt.MasterReference;
            loDatesList[listCounter] = t_S_PlanAttractGrnt.StartDate;
            hiDatesList[listCounter++] = t_S_PlanAttractGrnt.EndDate;
            // 7
            dtlKeysList[listCounter] = t_S_PlanAttractPrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_PlanAttractPrGrnt.StartDate;
            // 8
            dtlKeysList[listCounter] = t_S_ChargePenaltyDebtPrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_ChargePenaltyDebtPrGrnt.StartDate;
            // 9
            dtlKeysList[listCounter] = t_S_FactPenaltyPercentPrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_FactPenaltyPercentPrGrnt.FactDate;
            // 10
            dtlKeysList[listCounter] = t_S_PrincipalContrGrnt.MasterReference;
            hiDatesList[listCounter++] = t_S_PrincipalContrGrnt.EndDate;
            // 11
            dtlKeysList[listCounter] = t_S_PrGrntChargePenaltyPercent.MasterReference;
            loDatesList[listCounter] = t_S_PrGrntChargePenaltyPercent.StartDate;
            hiDatesList[listCounter++] = t_S_PrGrntChargePenaltyPercent.StartDate;
            // 12
            dtlKeysList[listCounter] = t_S_FactPenaltyDebtPrGrnt.MasterReference;
            hiDatesList[listCounter] = t_S_FactPenaltyDebtPrGrnt.FactDate;
        }

        protected override string GetContractDesc()
        {
            return "Гарантия";
        }

        public override string GetParentRefName()
        {
            return "RefGrnt";
        }

        protected override string GetCollateralKey()
        {
            return t_S_CollateralGrnt.internalKey;
        }

        protected override string GetMainTableKey()
        {
            return f_S_Guarantissued.InternalKey;
        }

        protected override string GetJournalKey()
        {
            return t_S_JournalPercentGrnt.internalKey;
        }

        protected override string GetDocListKey()
        {
            return t_S_ListContractGrnt.InternalKey;
        }

        protected override string GetAlterationKey()
        {
            return t_S_AlterationGrnt.internalKey;
        }

        protected override void MakeSafeSpot()
        {
            var planServiceIndex = Convert.ToInt32(t_S_PlanServicePrGrnt.key);
            safespotDetail.Add(planServiceIndex, dtDetail[planServiceIndex]);
        }

        private void FilterPlanService(object masterKey, string dateStr)
        {
            InternalDetailFilter(new ParamsDetailFilter
            {
                dateField = t_S_PlanServicePrGrnt.EstimtDate,
                onlyLastPlan = filterLastPlanService,
                detailIndex = t_S_PlanServicePrGrnt.key,
                isPartial = false,
                masterKey = masterKey,
                planMaxDate = dateStr
            });
        }

        public override void FilterDetailTables(object masterKey, string dateStr)
        {
            FilterPlanService(masterKey, dateStr);
        }

        public override void FilterDetailTables(object masterKey)
        {
            FilterPlanService(masterKey, planServiceDate);
        }
    }

    class GrntFactDebtPrDataObject : CommonDataObject
    {
        protected override string GetMainTableKey()
        {
            return t_S_FactDebtPrGrnt.internalKey;
        }
    }

    class GrntFactAttractDataObject : CommonDataObject
    {
        protected override string GetMainTableKey()
        {
            return t_S_FactAttractGrnt.internalKey;
        }
    }

    class GrntFactPercentPrDataObject : CommonDataObject
    {
        protected override string GetMainTableKey()
        {
            return t_S_FactPercentPrGrnt.internalKey;
        }
    }

}
