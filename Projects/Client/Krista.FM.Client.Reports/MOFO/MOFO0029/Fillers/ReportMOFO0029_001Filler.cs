using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0029.ReportFillers
{
    class ReportMOFO0029_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var year = Convert.ToInt32(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEAR));
            SetParamValue(sheet, "YEAR-1", year - 1);
            SetParamValue(sheet, "YEAR", year);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 10 - 1,
                StyleRowsCount = 5,
                TotalRowsCount = 3,
                FirstTotalColumn = 3,
                RemoveUnusedColumns = false,
                Columns = ReportDataServer.GetColumnsList(0, 12),
                SumColumns = new List<int> { 5, 8, 11, 12 }
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
