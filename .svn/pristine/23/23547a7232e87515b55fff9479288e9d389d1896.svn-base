using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning
{
    class ParamPlanDebt : ParamPlanService
    {
        public virtual List<string> GetValuesList(object masterKey)
        {
            var planVariants = new List<string>();
            var fltCredit = String.Format("{0} = {1}", t_S_PlanDebtCI.RefCreditInc, masterKey);
            var tblPlanDebt = dbHelper.GetEntityData(t_S_PlanDebtCI.internalKey, fltCredit);
            tblPlanDebt = DataTableUtils.SortDataSet(tblPlanDebt, t_S_PlanDebtCI.EstimtDate);

            foreach (DataRow rowPlan in tblPlanDebt.Rows)
            {
                if (rowPlan[t_S_PlanDebtCI.EstimtDate] == DBNull.Value)
                {
                    continue;
                }

                var estmtDate = Convert.ToDateTime(rowPlan[t_S_PlanDebtCI.EstimtDate]).ToShortDateString();

                if (!planVariants.Contains(estmtDate))
                {
                    planVariants.Add(estmtDate);
                }
            }

            return planVariants;
        }
    }
}
