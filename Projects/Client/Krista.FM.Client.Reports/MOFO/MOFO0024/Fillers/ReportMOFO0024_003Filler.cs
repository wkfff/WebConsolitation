using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0024.ReportFillers
{
    class ReportMOFO0024_003Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7,
                StyleRowsCount = 7,
                FirstTotalColumn = 2,
                Columns = new List<int>{0, 1, 3, 4, 6, 7},
                SumColumns = ReportDataServer.GetColumnsList(2, 9),
                RemoveUnusedColumns = false
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
