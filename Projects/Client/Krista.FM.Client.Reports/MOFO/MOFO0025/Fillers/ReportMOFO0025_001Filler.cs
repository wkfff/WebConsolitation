using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0025.ReportFillers
{
    class ReportMOFO0025_001Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var year = Convert.ToInt32(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEAR));
            SetParamValue(sheet, "YEAR+2", year + 2);
            SetParamValue(sheet, "YEAR+1", year + 1);
            SetParamValue(sheet, "YEAR", year);
            var columns = ReportDataServer.GetColumnsList(0, 10);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8 - 1,
                StyleRowsCount = 5,
                TotalRowsCount = 3,
                FirstTotalColumn = 3,
                Columns = columns,
                SumColumns = columns.Where(i => i >= 4).ToList()
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
