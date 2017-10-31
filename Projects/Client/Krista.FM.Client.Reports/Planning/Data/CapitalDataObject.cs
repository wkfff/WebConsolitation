using System;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;

namespace Krista.FM.Client.Reports.Planning.Data
{
    // объект для ценных бумаг
    class CapitalDataObject : CommonDataObject
    {
        public bool ignoreNegativeFact;
        public bool nonEmptySum;

        protected override void InitDetailDateLists()
        {
            const int detailCount = 15;
            dtlKeysList = new string[detailCount];
            loDatesList = new string[detailCount];
            mdDatesList = new string[detailCount];
            hiDatesList = new string[detailCount];
            var listCounter = 0;
            // 0
            dtlKeysList[listCounter] = t_S_CPFactCapital.MasterReference;
            hiDatesList[listCounter++] = t_S_CPFactCapital.DateDoc;
            // 1
            dtlKeysList[listCounter] = t_S_CPFactDebt.MasterReference;
            hiDatesList[listCounter++] = t_S_CPFactDebt.DateDischarge;
            // 2
            dtlKeysList[listCounter] = t_S_CPRateSwitch.MasterReference;
            hiDatesList[listCounter++] = t_S_CPRateSwitch.DateCharge;
            // 3
            dtlKeysList[listCounter] = t_S_CPFactService.MasterReference;
            loDatesList[listCounter] = t_S_CPFactService.PlanDate;
            hiDatesList[listCounter++] = t_S_CPFactService.FactDate;
            // 4
            dtlKeysList[listCounter] = t_S_CPFactCost.MasterReference;
            hiDatesList[listCounter++] = t_S_CPFactCost.CostDate;
            // 5
            dtlKeysList[listCounter] = t_S_CPPlanCapital.MasterReference;
            loDatesList[listCounter] = t_S_CPPlanCapital.StartDate;
            hiDatesList[listCounter++] = t_S_CPPlanCapital.EndDate;
            // 6
            dtlKeysList[listCounter] = t_S_CPPlanDebt.MasterReference;
            hiDatesList[listCounter++] = t_S_CPPlanDebt.EndDate;
            // 7
            dtlKeysList[listCounter] = t_S_CPPlanService.MasterReference;
            loDatesList[listCounter] = t_S_CPPlanService.StartDate;
            mdDatesList[listCounter] = t_S_CPPlanService.PaymentDate;
            hiDatesList[listCounter++] = t_S_CPPlanService.EndDate;
            // 8
            dtlKeysList[listCounter] = t_S_CPChargePenaltyDeb.MasterReference;
            hiDatesList[listCounter++] = t_S_CPChargePenaltyDeb.StartDate;
            // 9
            dtlKeysList[listCounter] = t_S_CPFactPenaltyCap.MasterReference;
            hiDatesList[listCounter++] = t_S_CPFactPenaltyCap.FactDate;
            // 10
            dtlKeysList[listCounter] = t_S_CPFactPenaltyPer.MasterReference;
            hiDatesList[listCounter++] = t_S_CPFactPenaltyPer.FactDate;
            // 11
            dtlKeysList[listCounter] = t_S_CPChargePenaltyPer.MasterReference;
            loDatesList[listCounter++] = t_S_CPChargePenaltyPer.StartDate;
            // 12
            dtlKeysList[listCounter] = t_S_CPExtraIssue.MasterReference;
            hiDatesList[listCounter++] = t_S_CPExtraIssue.ExtraIssueDate;
            // 13
            dtlKeysList[listCounter] = t_S_CPFactCost.MasterReference;
            hiDatesList[listCounter++] = t_S_CPFactCost.CostDate;
            // 14
            dtlKeysList[listCounter] = t_S_CPPlanCost.MasterReference;
            hiDatesList[listCounter] = t_S_CPPlanCost.CostDate;
        }

        protected override string GetContractDesc()
        {
            return "Ценная бумага";
        }

        public override string GetParentRefName()
        {
            return "RefCap";
        }

        protected override string GetCollateralKey()
        {
            return t_S_CPCollateral.internalKey;
        }

        protected override string GetMainTableKey()
        {
            return f_S_Capital.InternalKey;
        }

        protected override string GetJournalKey()
        {
            return t_S_CPJournalPercent.internalKey;
        }

        protected override void MakeSafeSpot()
        {
            var factServiceIndex = Convert.ToInt32(t_S_CPFactService.key);

            if (dtDetail[factServiceIndex] != null)
            {
                return;
            }

            if (ignoreNegativeFact)
            {
                var fltStr = String.Format("{0}>0 or {1}>0",
                    t_S_CPFactService.Sum,
                    t_S_CPFactService.ChargeSum);

                dtDetail[factServiceIndex] = DataTableUtils.FilterDataSet(dtDetail[factServiceIndex], fltStr);
            }

            if (nonEmptySum)
            {
                var fltStr = String.Format("{0} > 0", t_S_CPFactService.Sum);

                dtDetail[factServiceIndex] = DataTableUtils.FilterDataSet(dtDetail[factServiceIndex], fltStr);
            }
        }
    }
}
