using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server
{
    public class IncomesEvalPlanParams
    {
        public int Year
        {
            get; set;
        }

        public int SourceId
        {
            get; set;
        }

        public object IncomesVariant
        {
            get; set;
        }

        public object PlanIncomes
        {
            get; set;
        }

        public object KvsrPlan
        {
            get; set;
        }

        public Municipal Municipal
        {
            get;
            set;
        }

        public BudgetLevel BudgetLevel
        {
            get;
            set;
        }

        public bool IsEstimate
        {
            get; set;
        }

        public bool IsForecast
        {
            get; set;
        }

        public bool IsKmbResult
        {
            get; set;
        }

        public bool IsTaxResource
        {
            get; set;
        }

        public bool ShowResults
        {
            get; set;
        }

        public bool CheckParams()
        {
            if (IncomesVariant == null || PlanIncomes == null)
                return false;
            return true;
        }
    }
}
