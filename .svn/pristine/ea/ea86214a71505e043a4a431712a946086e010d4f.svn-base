using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK017Filler : CommonUFNSReportFiller
    {
        const int ColumnsInMainColumn = 5;
        const int FirstSumColumn = 1;

        private void FillHeader(HSSFSheet sheet, DateTime startDate, DateTime endDate)
        {
            const int yearsCount = 4;

            for (var i = yearsCount - 1; i >= 0; i--)
            {
                var paramName = i > 0 ? String.Format("-{0}", i) : String.Empty;
                SetParamValue(sheet, String.Format("START_DATE_YEAR{0}", paramName), startDate.AddYears(-i).ToShortDateString());
                SetParamValue(sheet, String.Format("END_DATE_YEAR{0}", paramName), endDate.AddYears(-i).ToShortDateString());
                SetParamValue(sheet, String.Format("YEAR{0}", paramName), endDate.Year - i);
            }
        }

        private List<int> GetNotDeletedColumns(IEnumerable<int> levels, int mainColumnsCount)
        {
            var columns = ReportDataServer.GetColumnsList(0, FirstSumColumn);

            for (var i = 0; i < mainColumnsCount; i++)
            {
                var firstColumn = FirstSumColumn + i * ColumnsInMainColumn;
                if (levels.Count() > 1)
                {
                    columns.Add(firstColumn);
                }

                columns.AddRange(levels.Select(level => firstColumn + 1 + level));
            }

            return columns;
        }

        public void DeleteColumns(HSSFWorkbook wb, HSSFSheet sheet, IEnumerable<int> levels, int mainColumnsCount)
        {
            const int measureRow = 1;
            var columnsInPattern = FirstSumColumn + mainColumnsCount * ColumnsInMainColumn;
            var columns = GetNotDeletedColumns(levels, mainColumnsCount);

            for (var i = columnsInPattern - 1; i >= 0; i--)
            {
                if (columns.Contains(i))
                {
                    continue;
                }

                if (i > columns.Max())
                {
                    var cell = NPOIHelper.GetCellByXY(sheet, measureRow, i);
                    MoveMeasureCell(cell, -1);
                }

                NPOIHelper.DeleteColumn(wb, sheet, i);
            }
        }
        
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var paramLevels = Convert.ToString(paramHelper.GetParamValue(ParamUFKHelper.BDGT_LEVEL));
            var levels = ReportDataServer.ConvertToIntList(paramLevels);
            var paramStartDate = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.STARTDATE));
            var paramEndDate = Convert.ToDateTime(paramHelper.GetParamValue(ParamUFKHelper.ENDDATE));
            var mainColumnsInList = new List<int> {12, 3 };

            for (var i = 0; i < mainColumnsInList.Count; i++)
            {
                var sheet = wb.GetSheetAt(i);
                FillHeader(sheet, paramStartDate, paramEndDate);
                DeleteColumns(wb, sheet, levels, mainColumnsInList[i]);
                var sumColumnsCount = tableList[i].Columns.Count - FirstSumColumn - 1;

                var sheetParams = new SheetParams
                {
                    SheetIndex = i,
                    FirstRow = 6 - 1,
                    StyleRowsCount = 2,
                    TotalRowsCount = 0,
                    RemoveUnusedColumns = false,
                    ExistHeaderNumColumn = true,
                    SumColumns = ReportDataServer.GetColumnsList(FirstSumColumn, sumColumnsCount)
                };

                FillSheet(wb, tableList, sheetParams);
            }
        }
    }
}
