using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK003Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            SetParamValue(sheet, "YEAR-2", years[0]);
            SetParamValue(sheet, "YEAR-1", years[1]);
            SetParamValue(sheet, "YEAR", years[2]);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 10 - 1,
                StyleRowsCount = 5,
                TotalRowsCount = 3,
                FirstTotalColumn = 3,
                RemoveUnusedColumns = false,
                Columns = new List<int> {0, 1, 2, 3, 4, 6},
                SumColumns = new List<int> {3, 4, 6}
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
