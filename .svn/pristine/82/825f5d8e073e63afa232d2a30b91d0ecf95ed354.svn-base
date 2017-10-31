using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK22.ReportFillers
{
    class ReportUFK22_027Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var endDate = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.ENDDATE));
            var date = endDate.AddDays(1);
            SetParamValue(sheet, "YEAR-2", endDate.Year - 2);
            SetParamValue(sheet, "YEAR-1", endDate.Year - 1);
            SetParamValue(sheet, "YEAR", endDate.Year);
            SetParamValue(sheet, "DAY.MONTH-2", ReportDataServer.GetDateDayMonth(date.AddYears(-2)));
            SetParamValue(sheet, "DAY.MONTH-1", ReportDataServer.GetDateDayMonth(date.AddYears(-1)));
            SetParamValue(sheet, "DAY.MONTH", ReportDataServer.GetDateDayMonth(date));
            var columns = new List<int> {0, 7, 1, 2, 4, 5, 8 }; // колонки не по порядку т.к. План года в таблице идёт сразу после наименований

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7 - 1,
                StyleRowsCount = 3,
                FirstTotalColumn = 1,
                RemoveUnusedColumns = false,
                ToSortColumns = false,
                Columns = columns,
                SumColumns = columns.Where(column => column > 0).ToList()
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
