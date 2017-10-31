using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK016Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);

            var columns = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 16, 17, 18};
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
                StyleRowsCount = 1,
                TotalRowsCount = 0,
                Columns = columns,
                SumColumns = columns.Where(column => column > 2).ToList(),
                RemoveUnusedColumns = false
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
