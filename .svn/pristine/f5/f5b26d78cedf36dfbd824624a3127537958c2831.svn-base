using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS002Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var codes = GetRowCodes(sheet, 8 - 1, 1);
            var columns = new List<int> { 0 };
            var dtColumns = tableList[1];
            columns.AddRange(from DataRow row in dtColumns.Rows
                             select Convert.ToString(row[0])
                             into code
                             select codes.Keys.Contains(code) ? codes[code] : UndefinedIndex);
            var sumColumns = columns.Where(column => column > 0).ToList();

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 10 - 1,
                StyleRowsCount = 2,
                TotalRowsCount = 0,
                HeaderRowsCount = 3,
                ExistHeaderNumColumn = true,
                ToSortColumns = false,
                Columns = columns,
                SumColumns = sumColumns
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
