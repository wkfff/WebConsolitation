using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK006Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var tblCaptions = tableList[tableList.Length - 1];
            var rowCaption = tblCaptions.Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var paramColumns = Convert.ToString(paramHelper.GetParamValue(ParamUFKHelper.COLUMNS));
            var columns = ReportDataServer.ConvertToIntList(paramColumns);

            // Лист 0
            var firstListColumns = new List<int> { 0, 1 };
            firstListColumns.AddRange(columns.Select(column => column + 2));

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6 - 1,
                StyleRowsCount = 7,
                FirstTotalColumn = 2,
                ExistHeaderNumColumn = true,
                HeaderRowsCount = 2,
                Columns = firstListColumns,
                SumColumns = firstListColumns.Where(column => column >= 2).ToList()
            };

            FillSheet(wb, tableList, sheetParams);
            
            // Лист 1
            var secondListColumns = new List<int> { 0, 1, 2 };
            secondListColumns.AddRange(columns.Select(column => column + 3));

            sheetParams = new SheetParams
            {
                SheetIndex = 1,
                FirstRow = 6 - 1,
                StyleRowsCount = 7,
                FirstTotalColumn = 3,
                ExistHeaderNumColumn = true,
                HeaderRowsCount = 2,
                Columns = secondListColumns,
                SumColumns = secondListColumns.Where(column => column >= 3).ToList()
            };

            FillSheet(wb, tableList, sheetParams);

            wb.ActiveSheetIndex = 0;
        }
    }
}
