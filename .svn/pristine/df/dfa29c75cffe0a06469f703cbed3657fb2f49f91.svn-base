using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0022.ReportFillers
{
    class ReportMOFO0022_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = ReportDataServer.GetColumnsList(3, 16);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6,
                StyleRowsCount = 5,
                TotalRowsCount = 3,
                FirstTotalColumn = 3,
                SumColumns = sumColumns
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
