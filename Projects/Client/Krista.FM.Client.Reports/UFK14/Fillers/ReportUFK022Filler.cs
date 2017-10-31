using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK022Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var totalDt = tableList[1];
            var sumColumns = new List<int> { 2, 4, 5, 7, 10 };
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var precision = paramHelper.GetParamValue(ParamUFKHelper.PRECISION);
            var date = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.ENDDATE));
            
            SetParamValue(sheet, "YEAR-1", date.Year - 1);
            SetParamValue(sheet, "YEAR", date.Year);
            SetParamValue(sheet, "DAY.MONTH", ReportDataServer.GetDateDayMonth(date));

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 18 - 1,
                StyleRowsCount = 1,
                TotalRowsCount = 0,
                SumColumns = sumColumns
            };

            // заполняем итоговые строки
            var firstTotalRow = sheetParams.FirstRow - 10;
            var totalRows = new []{0, 2, 5, 8};
            totalRows = totalRows.Select(i => i + firstTotalRow).ToArray();

            for (var i = 0; i < Math.Min(totalDt.Rows.Count, totalRows.Length); i++)
            {
                for (var j = 0; j < sumColumns.Count; j++)
                {
                    var row = totalDt.Rows[i];
                    NPOIHelper.SetCellValue(sheet, totalRows[i], sumColumns[j], row[j]);
                }
            }

            FillSheet(wb, tableList, sheetParams);
            // устанавливаем формат чисел для итоговых строк
            SetRangeDataFormat(wb, sheet, GetPrecisionFormat(precision), totalRows, sumColumns);

            sheet.ForceFormulaRecalculation = true;
        }
    }
}
