using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK012Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            var sheet = wb.GetSheetAt(sheetIndex);
            var paramSum = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.SUMM);
            if (Convert.ToDecimal(paramSum) == 0)
            {
                DeleteParamCell(sheet, ParamUFKHelper.SUMM);
            }

            var sumColumns = new List<int> { 1, 2, 3, 4 };

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 7 - 1,
                StyleRowsCount = 8,
                FirstTotalColumn = 1,
                SumColumns = sumColumns
            };
            
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
