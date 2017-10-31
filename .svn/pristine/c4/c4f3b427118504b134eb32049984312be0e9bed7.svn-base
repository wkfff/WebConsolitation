using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK007Filler : CommonUFNSReportFiller
    {
        public virtual void FillUFKReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var year = Convert.ToInt32(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            SetParamValue(sheet, "YEAR-2", year - 2);
            SetParamValue(sheet, "YEAR-1", year - 1);
            SetParamValue(sheet, "YEAR-0", year - 0);

            var columns = new List<int>
                                 {
                                      0,
                                      1,  2,  4,  6,  7,  9,
                                     16, 17, 19, 24, 25, 27,
                                     40, 41, 43, 48, 49, 51
                                 };
            var sumColumns = new List<int>
                                 {
                                      1,  2,  3,  4,  6,  7,  8,  9, 11, 12, 13, 14,
                                     16, 17, 18, 19, 24, 25, 26, 27, 32, 33, 34, 35,
                                     40, 41, 42, 43, 48, 49, 50, 51, 56, 57, 58, 59
                                 };

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7,
                StyleRowsCount = 5,
                Columns = columns,
                SumColumns = sumColumns,
                FirstTotalColumn = 1,
                RemoveUnusedColumns = false
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
