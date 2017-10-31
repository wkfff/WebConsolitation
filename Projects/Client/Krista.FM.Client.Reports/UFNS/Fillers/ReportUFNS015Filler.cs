using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS015Filler : CommonUFNSReportFiller
    {
        private const int MainColumnLevel = 0;
        private const int YearLevel = 1;
        private const int FirstDataColumn = 2;
        private const int YearsInPattern = 3;

        private DataTable GetColumnsTable(List<int> years, DataTable dtMainColumns)
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
                dt.Rows.Add(DBNull.Value, i, YearLevel, false, false, true);
            }

            foreach (DataRow row in dtMainColumns.Rows)
            {
                dt.Rows.Add(row[ReportDataServer.NAME], FirstDataColumn, MainColumnLevel, true, false, false);

                for (var i = 0; i < years.Count; i++)
                {
                    var yearIndex = i < YearsInPattern ? i : YearsInPattern - 1;
                    var style = yearIndex == 0 ? FirstDataColumn : FirstDataColumn + yearIndex * 2 - 1;
                    dt.Rows.Add(String.Format("{0} год", years[i]), style, YearLevel, false, true, true);
                    if (yearIndex > 0)
                    {
                        dt.Rows.Add(DBNull.Value, style + 1, YearLevel, false, false, false);
                    }
                }
            }

            return dt;
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            var sheet = wb.GetSheetAt(sheetIndex);
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var dtColumns = GetColumnsTable(years, tableList[1]);

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 7 - 1,
                StyleRowsCount = 4,
                StyleColumnsCount = 7,
                HeaderRowsCount = 3,
                FirstTotalColumn = FirstDataColumn,
                TotalRowsCount = 0,
                ExistHeaderNumColumn = true,
                RemoveUnusedColumns = false,
                Columns = GetDataColumnsIndexies(dtColumns),
                SumColumns = GetSumColumnsIndexies(dtColumns)
            };

            var styles = GetStyleRows(sheet, sheetParams);
            styles[2].SumColumns = new List<int>();
            styles[3].SumColumns = new List<int>();
            sheetParams.StyleRows = styles;
            AddColumns(wb, sheetParams, dtColumns);
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
