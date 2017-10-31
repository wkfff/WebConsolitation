using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK22.ReportFillers
{
    class ReportUFK22_000Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var columns = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13 };
            var sumColumns = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 9 - 1,
                StyleRowsCount = 5,
                FirstTotalColumn = 2,
                RemoveUnusedColumns = false,
                Columns = columns,
                SumColumns = sumColumns
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
