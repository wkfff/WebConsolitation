using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    class Report023Filler : CommonUFNSReportFiller
    {
        private DataTable MakeColumnsTable(int yearsCount)
        {
            var dt = new DataTable();
            dt.Columns.Add(ReportDataServer.NAME);
            dt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            dt.Rows.Add(DBNull.Value, 0);

            if (yearsCount == 0)
            {
                return dt;
            }

            if (yearsCount == 1)
            {
                dt.Rows.Add(DBNull.Value, 3);
            }
            else
            {
                dt.Rows.Add(DBNull.Value, 1);

                for (var i = 0; i < yearsCount - 2; i++)
                {
                    dt.Rows.Add(DBNull.Value, 2);
                }

                dt.Rows.Add(DBNull.Value, 4);
            }

            dt.Rows.Add(DBNull.Value, 5);
            dt.Rows.Add(DBNull.Value, 6);
            return dt;
        }

        private void SetRowsStyle(DataTable dt)
        {
            for (var i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var style = Convert.ToInt32(dt.Rows[i][ReportDataServer.STYLE]);
                if (style == 1) // уровень бюджета
                {
                    dt.Rows[i][ReportDataServer.STYLE] = style + Convert.ToInt32(dt.Rows[i][0]);
                    dt.Rows.InsertAt(dt.NewRow(), i + 1); // Коэффициенты роста
                    dt.Rows[i + 1][ReportDataServer.STYLE] = 4;
                    continue;
                }

                if (style == 2) // на дату
                {
                    dt.Rows[i][ReportDataServer.STYLE] = 5;
                    dt.Rows.InsertAt(dt.NewRow(), i + 1); // Удельный вес периода в году (%)
                    dt.Rows[i + 1][ReportDataServer.STYLE] = 6;
                    dt.Rows.InsertAt(dt.NewRow(), i + 2); // Коэффициенты роста
                    dt.Rows[i + 2][ReportDataServer.STYLE] = 7;
                }
            }
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            var sheet = wb.GetSheetAt(sheetIndex);
            var rowCaption = tableList[tableList.Length - 1].Rows[0];
            var paramHelper = new ParamUFKHelper(rowCaption);
            var paramYears = Convert.ToString(paramHelper.GetParamValue(ParamUFKHelper.YEARS));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var month = Convert.ToInt32(paramHelper.GetParamValue(ParamUFKHelper.MONTH));
            var columnsDt = MakeColumnsTable(years.Count);
            var columns = new List<int> { 0 };
            columns.AddRange(ReportDataServer.GetColumnsList(1, years.Count));
            var sumColumns = columns.Where(column => column > 0).ToList();
            if (years.Count > 0)
            {
                sumColumns.Add(years.Count + 1);
                sumColumns.Add(years.Count + 2);
            }

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 5 - 1,
                StyleRowsCount = 8,
                RemoveUnusedColumns = false,
                TotalRowsCount = 0,
                StyleColumnsCount = 7,
                HeaderRowsCount = 2,
                ExistHeaderNumColumn = true,
                Columns = columns,
                SumColumns = sumColumns
            };

            // Настраиваем колонки в шаблоне
            AddColumns(wb, sheetParams, columnsDt);

            const string captionParam = "YEAR-N";
            for (var i = 0; i < years.Count - 1; i++)
            {
                var rowIndex = sheetParams.FirstRow - sheetParams.HeaderRowsCount;
                var cellValue = NPOIHelper.GetCellStringValue(sheet, rowIndex, i + 1);
                if (cellValue.Contains(captionParam))
                {
                    var newVal = cellValue.Replace(captionParam, Convert.ToString(years[i]));
                    NPOIHelper.SetCellValue(sheet, rowIndex, i + 1, newVal);
                }
            }

            // Настраиваем области объединения в заголовке и на уровне группы КД
            NPOIHelper.RemoveCellMergedRegion(sheet, 0, 0);

            if (years.Count > 0)
            {
                sheet.AddMergedRegion(new Region(0, 0, 0, years.Count + 2));
                sheet.AddMergedRegion(new Region(sheetParams.FirstRow, 0, sheetParams.FirstRow, years.Count + 2));
            }

            // параметры
            var dateNa = month < 12
                 ? String.Format("01 {0}", ReportDataServer.GetMonthText2(month + 1))
                 : "конец года";
            SetParamValue(sheet, "01.MONTH+1", dateNa);
            var year = years.Count > 0 ? Convert.ToString(years[years.Count - 1]) : String.Empty;
            var yearN = years.Count > 0 ? Convert.ToString(years[0]) : String.Empty;
            SetParamValue(sheet, "YEAR-N", yearN);
            SetParamValue(sheet, "YEAR", year);

            // настраиваем стили
            var styleRows = GetStyleRows(sheet, sheetParams, 1);
            styleRows[0].FirstDataColumn = 0;
            styleRows[4].FirstDataColumn = years.Count + 3;
            styleRows[4].SumColumns = new List<int>();
            styleRows[6].FirstDataColumn = years.Count + 3;
            styleRows[6].SumColumns = new List<int>();
            styleRows[7].FirstDataColumn = years.Count + 3;
            styleRows[7].SumColumns = new List<int>();
            sheetParams.StyleRows = styleRows;

            SetRowsStyle(tableList[sheetIndex]);
            FillSheet(wb, tableList, sheetParams);
        }
    }
}
