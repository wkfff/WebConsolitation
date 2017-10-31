using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NPOI.HSSF.UserModel;


namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS004Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 11,
                StyleRowsCount = 3,
                FirstTotalColumn = 3,
                TotalRowsCount = 3,
                HeaderRowsCount = 4,
                ToSortColumns = false,
                ExistHeaderNumColumn = true
            };

            var codes = GetRowCodes(sheet, sheetParams.FirstRow - 2, 3);
            var columns = new List<int> { 0, 1, 2 };
            var dtColumns = tableList[1];
            columns.AddRange(from DataRow row in dtColumns.Rows
                             select Convert.ToString(row[0])
                             into code
                             select codes.Keys.Contains(code) ? codes[code] : UndefinedIndex);
            var sumColumns = columns.Where(column => column > 2).ToList();
            sheetParams.Columns = columns;
            sheetParams.SumColumns = sumColumns;
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
