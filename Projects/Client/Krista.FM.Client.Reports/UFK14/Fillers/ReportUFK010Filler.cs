using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK010Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var paramSum = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.SUMM);
            if (Convert.ToDecimal(paramSum) == 0)
            {
                DeleteParamCell(sheet, ParamUFKHelper.SUMM);
            }

            var sumColumnsDt = tableList[1];
            var sumColumns = sumColumnsDt.Rows.Cast<object>().Select((t, i) => 1 + i).ToList();

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7,
                StyleRowsCount = 6,
                StyleColumnsCount = 3,
                SumColumns = sumColumns,
                HeaderRowsCount = 2,
                FirstTotalColumn = 1,
                ExistHeaderNumColumn = true
            };

            AddColumns(wb, sheetParams, sumColumnsDt);
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
