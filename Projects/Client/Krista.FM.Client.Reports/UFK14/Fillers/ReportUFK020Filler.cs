using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK020Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var year = Convert.ToInt32(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var month = Convert.ToInt32(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.MONTH));
            SetParamValue(sheet, ParamUFKHelper.MONTH, ReportDataServer.GetMonthRusNames()[month - 1]);
            SetParamValue(sheet, String.Format("{0}_1", ParamUFKHelper.YEARS), year - 1);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 11 - 1,
                StyleRowsCount = month == 12 ? 1 : 2, // если декабрь, то строка с ЗО будет выводиться как итоговая, в др. месяцы её нет
                Columns = new List<int> { 0, 1, 2, 3, 4, 6, 7 },
                SumColumns = new List<int> { 1, 2, 4, 6, 7, 10, 11, 12, 14, 16 },
                FirstTotalColumn = 1,
                TotalRowsCount = month == 12 ? 2 : 1,
                RemoveUnusedColumns = false
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
