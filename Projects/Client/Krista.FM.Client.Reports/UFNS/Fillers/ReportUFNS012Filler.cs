using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS012Filler : CommonUFNSReportFiller
    {
        private const int YearLevel = 0;
        private const int YearsInPattern = 3;
        private const int FirstDataColumn = 2;

        public void RemoveHalfYearColumns(HSSFWorkbook wb, HSSFSheet sheet, IList<int> halfYear)
        {
            for (var i = YearsInPattern - 1; i >= 0; i--)
            {
                var firstYearColumn = i == 0 ? FirstDataColumn : FirstDataColumn + i * 6 - 2;
                var halfs = new[] { 1, 0 };
                foreach (var half in halfs.Where(e => !halfYear.Contains(e)))
                {
                    var column = i == 0 ? firstYearColumn + half*2 : firstYearColumn + half * 3;
                    if (i > 0)
                    {
                        NPOIHelper.DeleteColumn(wb, sheet, column + 2);
                    }
                    NPOIHelper.DeleteColumn(wb, sheet, column + 1);
                    NPOIHelper.DeleteColumn(wb, sheet, column);
                }
            }
        }

        private DataTable GetColumnsTable(List<int> years, int halfsCount)
        {
            var dt = new DataTable();
            dt.Columns.Add(ReportDataServer.NAME, typeof(string));
            dt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            dt.Columns.Add(ReportDataServer.LEVEL, typeof(int));
            dt.Columns.Add(ReportDataServer.MERGED, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsRUB, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsDATA, typeof(bool));
            for (var i = 0; i < FirstDataColumn; i++)
            {
                dt.Rows.Add(DBNull.Value, i, YearLevel + 1, false, false, true);
            }

            for (var i = 0; i < years.Count; i++)
            {
                dt.Rows.Add(String.Format("{0} год", years[i]), FirstDataColumn, YearLevel, true, false, false);
                var yearIndex = i < YearsInPattern ? i : YearsInPattern - 1;
                var firstYearColumn = yearIndex == 0
                                          ? FirstDataColumn
                                          : FirstDataColumn + halfsCount*(yearIndex*3 - 1);

                for (var j = 0; j < halfsCount; j++)
                {
                    var style = yearIndex == 0 ? firstYearColumn + j*2 : firstYearColumn + j*3;
                    dt.Rows.Add(DBNull.Value, style++, YearLevel + 1, false, true, true);
                    if (yearIndex > 0)
                    {
                        dt.Rows.Add(DBNull.Value, style++, YearLevel + 1, false, false, false);
                    }
                    dt.Rows.Add(DBNull.Value, style, YearLevel + 1, false, false, true);
                }
            }

            return dt;
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            var sheet = wb.GetSheetAt(sheetIndex);
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var paramHalfYear = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.HALF_YEAR));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var halfYear = ReportDataServer.ConvertToIntList(paramHalfYear);
            RemoveHalfYearColumns(wb, sheet, halfYear);
            var dtColumns = GetColumnsTable(years, halfYear.Count);

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 7 - 1,
                StyleRowsCount = 6,
                StyleColumnsCount = 18,
                HeaderRowsCount = 3,
                FirstTotalColumn = FirstDataColumn,
                TotalRowsCount = 0,
                ExistHeaderNumColumn = true,
                RemoveUnusedColumns = false,
                Columns = GetDataColumnsIndexies(dtColumns),
                SumColumns = GetSumColumnsIndexies(dtColumns)
            };

            var styles = GetStyleRows(sheet, sheetParams);
            styles[3].SumColumns = new List<int>();
            styles[4].SumColumns = new List<int>();
            styles[5].SumColumns = new List<int>();
            sheetParams.StyleRows = styles;
            AddColumns(wb, sheetParams, dtColumns);
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
