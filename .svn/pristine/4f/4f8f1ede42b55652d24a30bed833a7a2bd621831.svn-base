using System.Data;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS010Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8 - 1,
                StyleRowsCount = 3,
                StyleColumnsCount = 5,
                HeaderRowsCount = 4,
                FirstTotalColumn = 3,
                TotalRowsCount = 3,
                ToSortColumns = false,
                ExistHeaderNumColumn = true,
                Columns = GetDataColumnsIndexies(tableList[1]),
                SumColumns = GetSumColumnsIndexies(tableList[1])
            };

            AddColumns(wb, sheetParams, tableList[1]);
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
