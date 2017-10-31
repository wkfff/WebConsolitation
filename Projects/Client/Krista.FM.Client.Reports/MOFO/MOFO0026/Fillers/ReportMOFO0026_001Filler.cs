using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0026.ReportFillers
{
    class ReportMOFO0026_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = ReportDataServer.GetColumnsList(3, 8);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7 - 1,
                StyleRowsCount = 6,
                TotalRowsCount = 3,
                FirstTotalColumn = 3,
                SumColumns = sumColumns
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
