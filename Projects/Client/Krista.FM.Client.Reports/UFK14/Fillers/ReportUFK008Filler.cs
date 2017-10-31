using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK008Filler : CommonUFNSReportFiller
    {
        private void RemoveNullSumParam(HSSFWorkbook wb, DataTable[] tableList)
        {
            var paramSum = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.SUMM);

            for (var i = 0; i < wb.NumberOfSheets; i++)
            {
                if (Convert.ToDecimal(paramSum) == 0)
                {
                    DeleteParamCell(wb.GetSheetAt(i), ParamUFKHelper.SUMM);
                }
            }
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            RemoveNullSumParam(wb, tableList);
            var paramColumns = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.COLUMNS));
            var levelColumns = ReportDataServer.ConvertToIntList(paramColumns);

            var sheetParams = new SheetParams
            {
                FirstRow = 10 - 1,
                FirstTotalColumn = 1,
                HeaderRowsCount = 2,
                ExistHeaderNumColumn = true
            };

            // Лист 1
            var columns = new List<int> { 0, 1 };
            columns.AddRange(levelColumns.Select(level => level + 2));
            sheetParams.SheetIndex = 0;
            sheetParams.StyleRowsCount = 1;
            sheetParams.Columns = columns;
            sheetParams.SumColumns = columns.Where(column => column >= 2).ToList();
            FillSheet(wb, tableList, sheetParams);

            // Лист 2
            columns = new List<int> { 0, 1, 2 };
            columns.AddRange(levelColumns.Select(level => level + 3));
            sheetParams.SheetIndex = 1;
            sheetParams.StyleRowsCount = 6;
            sheetParams.Columns = columns;
            sheetParams.SumColumns = columns.Where(column => column >= 3).ToList();
            FillSheet(wb, tableList, sheetParams);

            // Лист 3
            columns = new List<int> { 0 };
            columns.AddRange(levelColumns.Select(level => level + 1));
            sheetParams.SheetIndex = 2;
            sheetParams.StyleRowsCount = 2;
            sheetParams.Columns = columns;
            sheetParams.SumColumns = columns.Where(column => column >= 1).ToList();
            FillSheet(wb, tableList, sheetParams);

            // Лист 4
            columns = new List<int> { 0 };
            columns.AddRange(levelColumns.Select(level => level + 1));
            sheetParams.SheetIndex = 3;
            sheetParams.StyleRowsCount = 7;
            sheetParams.Columns = columns;
            sheetParams.SumColumns = columns.Where(column => column >= 1).ToList();
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
