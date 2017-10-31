using System.Collections.Generic;
using System.Data;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS006Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = ReportDataServer.GetColumnsList(1, 7);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 14,
                StyleRowsCount = 1,
                SumColumns = sumColumns,
                TotalRowsCount = 0
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }

    class ReportUFNS006_1Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = ReportDataServer.GetColumnsList(1, 13);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 14,
                StyleRowsCount = 1,
                SumColumns = sumColumns,
                TotalRowsCount = 0
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
