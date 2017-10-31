using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    class Report028Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            var sheet = wb.GetSheetAt(sheetIndex);
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var year = Convert.ToInt32(paramHelper.GetParamValue(ParamUFKHelper.YEAR));
            var month = Convert.ToInt32(paramHelper.GetParamValue(ParamUFKHelper.MONTH));
            var date = month < 12 ? new DateTime(year, month + 1, 1) : new DateTime(year + 1, 1, 1);
            SetParamValue(sheet, "01.MM+1.YEAR-1", date.AddYears(-1).ToShortDateString());
            SetParamValue(sheet, "01.MM+1.YEAR", date.ToShortDateString());
            SetParamValue(sheet, "01.MM+1", ReportDataServer.GetDateDayMonth(date));
            SetParamValue(sheet, "YEAR-2", year - 2);
            SetParamValue(sheet, "YEAR-1", year - 1);
            SetParamValue(sheet, "YEAR", year);
            var newSheetName = wb.GetSheetName(sheetIndex).Replace("01.MM+1", ReportDataServer.GetDateDayMonth(date));
            wb.SetSheetName(sheetIndex, newSheetName);
            var columns = new List<int> {0, 1, 2, 3, 4, 5, 6};

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 8 - 1,
                StyleRowsCount = 3,
                FirstTotalColumn = 1,
                RemoveUnusedColumns = false,
                TotalRowsCount = 0,
                Columns = columns,
                SumColumns = columns.Where(column => column > 0).ToList()
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
