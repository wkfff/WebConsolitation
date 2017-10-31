using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK22.ReportFillers
{
    class ReportUFK22_024Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var startDate = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.STARTDATE));
            var endDate = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.ENDDATE));
            var date = endDate.AddDays(1);
            SetParamValue(sheet, "YEAR-1", endDate.Year - 1);
            SetParamValue(sheet, "YEAR", endDate.Year);
            SetParamValue(sheet, "STARTDAY.STARTMONTH", ReportDataServer.GetDateDayMonth(startDate));
            SetParamValue(sheet, "ENDDAY.ENDMONTH", ReportDataServer.GetDateDayMonth(endDate));
            SetParamValue(sheet, "DAY.MONTH",  ReportDataServer.GetDateDayMonth(date));
            var columns = new List<int> {0, 1, 2, 4, 5};

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7 - 1,
                StyleRowsCount = 3,
                FirstTotalColumn = 1,
                RemoveUnusedColumns = false,
                Columns = columns,
                SumColumns = columns.Where(column => column > 0).ToList()
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
