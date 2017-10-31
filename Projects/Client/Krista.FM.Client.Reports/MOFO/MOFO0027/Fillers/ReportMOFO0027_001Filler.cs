using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0027.ReportFillers
{
    class ReportMOFO0027_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int firstSumColumn = 3;
            const int columnsCount = 32;
            var sheet = wb.GetSheetAt(0);
            var columns = tableList[0].Columns.Count == columnsCount + 1
                              ? ReportDataServer.GetColumnsList(0, columnsCount)
                              : new List<int> {0, 1, 2, 7, 12, 17, 22, 23, 24, 29, 30, 31};

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8 - 1,
                StyleRowsCount = 6,
                TotalRowsCount = 3,
                FirstTotalColumn = 3,
                ExistHeaderNumColumn = true,
                Columns = columns,
                SumColumns = columns.Where(i => i >= firstSumColumn).ToList()
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
