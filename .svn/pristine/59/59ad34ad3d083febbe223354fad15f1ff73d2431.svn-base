using System;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK018Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = ReportDataServer.GetColumnsList(3, 12);
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var paramEndDate = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.ENDDATE));
            var paramStartDate = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.STARTDATE));
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

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 9,
                StyleRowsCount = 2,
                TotalRowsCount = 0,
                SumColumns = sumColumns
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
