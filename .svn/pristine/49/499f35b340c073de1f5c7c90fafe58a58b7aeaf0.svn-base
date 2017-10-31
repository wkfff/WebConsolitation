using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    class Report030Filler : CommonUFNSReportFiller
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
            SetParamValue(sheet, "YEAR-1", year - 1);
            SetParamValue(sheet, "YEAR", year);
            var columns = new List<int> { 0, 1, 2, 4, 5, 7, 10 };
            var sumColumns = new List<int> { 1, 2, 4, 5, 6, 7, 10, 13, 14 };

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 11 - 1,
                StyleRowsCount = 3,
                RemoveUnusedColumns = false,
                TotalRowsCount = 0,
                Columns = columns,
                SumColumns = sumColumns
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
