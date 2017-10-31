using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK015Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var totalDt = tableList[1];
            var precision = GetPrecisionFormat(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.PRECISION));
            var paramStartDate = Convert.ToDateTime(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.STARTDATE));
            var paramEndDate = Convert.ToDateTime(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.ENDDATE));
            SetParamValue(sheet, "START_DATE_YEAR-1", paramStartDate.AddYears(-1).ToShortDateString());
            SetParamValue(sheet, "END_DATE_YEAR-1", paramEndDate.AddYears(-1).ToShortDateString());
            SetParamValue(sheet, "START_DATE_YEAR", paramStartDate.ToShortDateString());
            SetParamValue(sheet, "END_DATE_YEAR", paramEndDate.ToShortDateString());
            SetParamValue(sheet, "YEAR-1", paramEndDate.Year - 1);
            var paramSum = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.SUMM);
            if (Convert.ToDecimal(paramSum) == 0)
            {
                DeleteParamCell(sheet, ParamUFKHelper.SUMM);
            }

            var columns = new List<int> { 0, 1, 2, 3, 5, 6 };
            var sumColumns = columns.Where(i => i > 2).ToList();

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 11,
                StyleRowsCount = 1,
                TotalRowsCount = 0,
                RemoveUnusedColumns = false,
                Columns = columns,
                SumColumns = sumColumns
            };
  
            // заполняем итоговые строки
            var firstTotalRow = sheetParams.FirstRow - 3;
            var totalRows = new List<int>();
            for (var i = 0; i < totalDt.Rows.Count; i++)
            {
                var row = totalDt.Rows[i];
                var rowIndex = firstTotalRow + i;
                totalRows.Add(rowIndex);
                NPOIHelper.SetCellValue(sheet, rowIndex, 3, row[0]);
                NPOIHelper.SetCellValue(sheet, rowIndex, 5, row[1]);
                NPOIHelper.SetCellValue(sheet, rowIndex, 6, row[2]);
            }

            FillSheet(wb, tableList, sheetParams);
            // устанавливаем формат чисел для итоговых строк
            SetRangeDataFormat(wb, sheet, precision, totalRows, sumColumns);

            sheet.ForceFormulaRecalculation = true;
        }
    }
}
