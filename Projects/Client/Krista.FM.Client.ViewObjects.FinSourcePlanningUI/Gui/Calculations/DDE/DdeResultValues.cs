using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE
{
    public class DdeResultValues
    {
        public DdeResultValues(DataRow resultRow)
        {
            Income = Convert.ToDecimal(resultRow["Income"]);
            CurrentCharge = Convert.ToDecimal(resultRow["CurrentCharge"]);
            ChangeRemains = Convert.ToDecimal(resultRow["ChangeRemains"]);
            SafetyStock = Convert.ToDecimal(resultRow["SafetyStock"]);
            PlanDebt = Convert.ToDecimal(resultRow["PlanDebt"]);
            PlanService = Convert.ToDecimal(resultRow["PlanService"]);
            ContLiability = Convert.ToDecimal(resultRow["ContLiability"]);
            ConsPlan = PlanDebt + PlanService + ContLiability;
            ContentDebt = Income - CurrentCharge + ChangeRemains + SafetyStock;
            AvailableDebt = ContentDebt - ConsPlan;
        }

        public void AddResults(DdeResultValues resultValues)
        {
            Income += resultValues.Income;
            CurrentCharge += resultValues.CurrentCharge;
            ChangeRemains += resultValues.ChangeRemains;
            SafetyStock += resultValues.SafetyStock;
            PlanDebt += resultValues.PlanDebt;
            PlanService += resultValues.PlanService;
            ContLiability += resultValues.ContLiability;
            ConsPlan += resultValues.ConsPlan;
            ContentDebt += resultValues.ContentDebt;
            AvailableDebt += resultValues.AvailableDebt;
        }

        public decimal Income
        {
            get; set;
        }

        public decimal CurrentCharge
        {
            get; set;
        }

        public decimal ChangeRemains
        {
            get; set;
        }

        public decimal SafetyStock
        {
            get; set;
        }

        public decimal PlanDebt
        {
            get; set;
        }

        public decimal PlanService
        {
            get; set;
        }

        public decimal ContLiability
        {
            get; set;
        }

        public decimal ConsPlan
        {
            get; set;
        }

        public decimal ContentDebt
        {
            get; set;
        }

        public decimal AvailableDebt
        {
            get; set;
        }
    }
}
