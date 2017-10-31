using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning;

namespace Krista.FM.Client.Reports.Planning.Data
{
    class CreditDataObject : CommonDataObject
    {
        public bool onlyLastPlanService;
        public bool onlyLastPlanDebt = true;
        public string planServiceDate = String.Empty;
        public string planDebtDate = DateTime.Now.ToShortDateString();

        protected override void InitDetailDateLists()
        {
            const int detailCount = 11;
            dtlKeysList = new string[detailCount];
            loDatesList = new string[detailCount];
            mdDatesList = new string[detailCount];
            hiDatesList = new string[detailCount];
            var listCounter = 0;
            // 0
            dtlKeysList[listCounter] = t_S_FactAttractCI.MasterReference;
            hiDatesList[listCounter++] = t_S_FactAttractCI.FactDate;
            // 1
            dtlKeysList[listCounter] = t_S_FactDebtCI.MasterReference;
            hiDatesList[listCounter++] = t_S_FactDebtCI.FactDate;
            // 2
            dtlKeysList[listCounter] = t_S_PlanDebtCI.MasterReference;
            loDatesList[listCounter] = t_S_PlanDebtCI.StartDate;
            hiDatesList[listCounter++] = t_S_PlanDebtCI.EndDate;
            // 3
            dtlKeysList[listCounter] = t_S_PlanAttractCI.MasterReference;
            loDatesList[listCounter] = t_S_PlanAttractCI.StartDate;
            hiDatesList[listCounter++] = t_S_PlanAttractCI.StartDate;
            // 4
            dtlKeysList[listCounter] = t_S_FactPercentCI.MasterReference;
            hiDatesList[listCounter++] = t_S_FactPercentCI.FactDate;
            // 5
            dtlKeysList[listCounter] = t_S_PlanServiceCI.MasterReference;
            loDatesList[listCounter] = t_S_PlanServiceCI.StartDate;
            mdDatesList[listCounter] = t_S_PlanServiceCI.PaymentDate;
            hiDatesList[listCounter++] = t_S_PlanServiceCI.EndDate;
            // 6
            dtlKeysList[listCounter] = t_S_ChargePenaltyDebtCI.MasterReference;
            hiDatesList[listCounter++] = t_S_ChargePenaltyDebtCI.StartDate;
            // 7
            dtlKeysList[listCounter] = t_S_CIChargePenaltyPercent.MasterReference;
            hiDatesList[listCounter++] = t_S_CIChargePenaltyPercent.StartDate;
            // 8
            dtlKeysList[listCounter] = t_S_FactPenaltyDebtCI.MasterReference;
            hiDatesList[listCounter++] = t_S_FactPenaltyDebtCI.FactDate;
            // 9
            dtlKeysList[listCounter] = t_S_FactPenaltyPercentCI.MasterReference;
            hiDatesList[listCounter++] = t_S_FactPenaltyPercentCI.FactDate;
            // 10
            dtlKeysList[listCounter] = t_S_RateSwitchCI.MasterReference;
            loDatesList[listCounter] = t_S_RateSwitchCI.EndDate;
            hiDatesList[listCounter] = t_S_RateSwitchCI.DateCharge;
        }

        protected override string GetContractDesc()
        {
            return mainFilter[f_S_Creditincome.RefSTypeCredit] == ReportConsts.BudCreditCode ? 
                "Бюджетный кредит" : 
                "Кредит";
        }

        public override string GetParentRefName()
        {
            return "RefCreditInc";
        }

        protected override string GetCollateralKey()
        {
            return t_S_CollateralCI.InternalKey;
        }

        protected override string GetMainTableKey()
        {
            return f_S_Creditincome.internalKey;
        }

        protected override string GetJournalKey()
        {
            return t_S_JournalPercentCI.internalKey;
        }

        protected override string GetDocListKey()
        {
            return t_S_ListContractCl.InternalKey;
        }

        protected override string GetAlterationKey()
        {
            return t_S_AlterationCl.internalKey;
        }

        protected override void MakeSafeSpot()
        {
            var planServiceIndex = Convert.ToInt32(t_S_PlanServiceCI.key);
            var planDebtIndex = Convert.ToInt32(t_S_PlanDebtCI.key);
            safespotDetail.Add(planServiceIndex, dtDetail[planServiceIndex]);
            safespotDetail.Add(planDebtIndex, dtDetail[planDebtIndex]);
        }

        private void FilterPlanDebt(object masterKey, string dateStr, bool isPartial)
        {
            InternalDetailFilter(new ParamsDetailFilter
            {
                dateField = t_S_PlanDebtCI.EstimtDate,
                onlyLastPlan = onlyLastPlanService,
                detailIndex = t_S_PlanDebtCI.key,
                isPartial = isPartial,
                masterKey = masterKey,
                planMaxDate = dateStr,
                Items = new ParamPlanDebt().GetValuesList(masterKey)
            });
        }

        private void FilterPlanService(object masterKey, string dateStr, bool isPartial)
        {
            var tbl = new ParamPlanService { key = masterKey}.CreateTable();
            var list = tbl.Rows.Cast<DataRow>().Select(row => Convert.ToString(row[0])).ToList();

            InternalDetailFilter(new ParamsDetailFilter
            {
                dateField = t_S_PlanServiceCI.EstimtDate,
                onlyLastPlan = onlyLastPlanService,
                detailIndex = t_S_PlanServiceCI.key,
                isPartial = isPartial,
                masterKey = masterKey,
                planMaxDate = dateStr,
                Items = list
            });
        }

        public override void FilterDetailTables(object masterKey, string dateStr)
        {
            FilterPlanDebt(masterKey, dateStr, true);
            FilterPlanService(masterKey, dateStr, true);
        }

        public override void FilterDetailTables(object masterKey)
        {
            FilterPlanDebt(masterKey, planDebtDate, false);
            FilterPlanService(masterKey, planServiceDate, false);
        }
    }
}
