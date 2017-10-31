using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK004Filler : CommonUFNSReportFiller
    {
        public virtual void FillUFKReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6,
                StyleRowsCount = 6,
                FirstTotalColumn = 1,
                RemoveUnusedColumns = false,
                Columns = new List<int> { 0, 2, 3, 5, 6, 7 },
                SumColumns = ReportDataServer.GetColumnsList(1, 7),
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
