using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0024.ReportFillers
{
    class ReportMOFO0024_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);

            // вставляем пустые строки
            var dt = tableList[0];
            dt.Rows.InsertAt(dt.NewRow(), 1);
            dt.Rows.InsertAt(dt.NewRow(), 4);
            dt.Rows.InsertAt(dt.NewRow(), 6);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8,
                StyleRowsCount = 0,
                TotalRowsCount = 9,
                FirstTotalColumn = 1,
                RemoveUnusedColumns = false,
                SumColumns = new List<int>{1, 2, 3}
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
