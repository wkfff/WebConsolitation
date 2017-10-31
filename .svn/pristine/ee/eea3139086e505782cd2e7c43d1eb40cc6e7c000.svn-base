using System.Collections.Generic;
using System.Data;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS009Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7,
                StyleRowsCount = 1,
                SumColumns = sumColumns,
                TotalRowsCount = 0
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
