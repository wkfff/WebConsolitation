using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK005Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var paramColumns = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.COLUMNS));
            var columns = ReportDataServer.ConvertToIntList(paramColumns);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 10 - 1,
                StyleRowsCount = 5,
                TotalRowsCount = 3,
                HeaderRowsCount = 2,
                FirstTotalColumn = 3,
                ExistHeaderNumColumn = true,
                Columns = columns,
                SumColumns = columns.Where(i => i >= 3).ToList()
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
