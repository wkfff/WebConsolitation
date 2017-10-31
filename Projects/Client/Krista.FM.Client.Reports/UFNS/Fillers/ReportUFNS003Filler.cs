﻿using System.Collections.Generic;
using System.Data;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS003Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var sumColumns = new List<int> {1, 2, 3};

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6,
                StyleRowsCount = 1,
                SumColumns = sumColumns,
                TotalRowsCount = 0
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
