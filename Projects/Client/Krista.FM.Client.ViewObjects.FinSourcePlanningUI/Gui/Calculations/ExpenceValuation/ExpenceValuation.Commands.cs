using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class ReportExpenceValuationCommand : ExcelMacrosCommand
    {
        private ExpenseValuationUI expVal;

        internal ReportExpenceValuationCommand(ExpenseValuationUI expVal)
        {
            key = "ReportChargesDebtInformation";
            caption = "Оценка расходов на обслуживание долга";
            this.expVal = expVal;
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return expVal.GetReportData();
        }
    }
}
