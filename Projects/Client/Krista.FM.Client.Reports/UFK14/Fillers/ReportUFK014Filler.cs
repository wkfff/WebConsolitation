using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK014Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = new List<int> { 3, 4, 5, 6, 7, 8 };
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

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8,
                StyleRowsCount = 1,
                FirstTotalColumn = 3,
                SumColumns = sumColumns
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
