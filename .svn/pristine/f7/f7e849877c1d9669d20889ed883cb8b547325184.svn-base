using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK019Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var paramSum = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.SUMM);
            if (Convert.ToDecimal(paramSum) == 0)
            {
                DeleteParamCell(sheet, ParamUFKHelper.SUMM);
            }

            var columnsCount = tableList[0].Columns.Count - 1; // без колонки Style
            var columns = ReportDataServer.GetColumnsList(0, columnsCount);
            if (columnsCount > 2)
            {
                columns[columns.Count - 1] = 13;
            }

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8,
                StyleRowsCount = 6,
                Columns = columns,
                SumColumns = columns.Where(column => column > 0).ToList(),
                FirstTotalColumn = 1,
                ExistHeaderNumColumn = true
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
