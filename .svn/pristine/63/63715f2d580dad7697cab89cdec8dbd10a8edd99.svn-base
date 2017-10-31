using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0024.ReportFillers
{
    class ReportMOFO0024_002Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var totalDt = tableList[1];
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var sheet = wb.GetSheetAt(0);
            var sumColumns = new List<int> {4, 5};
            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 9,
                StyleRowsCount = 1,
                TotalRowsCount = 0,
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
                NPOIHelper.SetCellValue(sheet, rowIndex, 4, row[0]);
                NPOIHelper.SetCellValue(sheet, rowIndex, 5, row[1]);
            }
            // устанавливаем формат чисел для итоговых строк
            SetRangeDataFormat(wb, sheet, GetPrecisionFormat(paramHelper.GetParamValue(ParamUFKHelper.PRECISION)), totalRows, sumColumns);
            
            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
