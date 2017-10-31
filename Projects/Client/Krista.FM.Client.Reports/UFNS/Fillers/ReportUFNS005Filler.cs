using System.Collections.Generic;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS005Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var columnsDt = tableList[1];
            var sumColumns = new List<int> { 1 };
            var sumColumnsCount = (columnsDt.Rows.Count - 2) / 2;

            for (var i = 0; i < sumColumnsCount; i++)
            {
                sumColumns.Add(2 + i * 2);
            }

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7,
                StyleRowsCount = 1,
                StyleColumnsCount = 3,
                SumColumns = sumColumns,
                TotalRowsCount = 0,
                HeaderRowsCount = 3,
                ExistHeaderNumColumn = true
            };

            AddColumns(wb, sheetParams, columnsDt);
            if (sumColumnsCount > 0)
            {
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 1, columnsDt.Rows.Count - 1));
            }
            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
