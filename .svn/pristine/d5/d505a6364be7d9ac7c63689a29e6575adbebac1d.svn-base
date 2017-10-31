using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK024Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8 - 1,
                StyleRowsCount = 1,
                TotalRowsCount = 0,
                RemoveUnusedColumns = false,
                Columns = new List<int> {0, 1, 3, 4, 6, 7},
                SumColumns = ReportDataServer.GetColumnsList(2, 6)
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
