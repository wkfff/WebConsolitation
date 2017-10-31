using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFNS.ReportFillers
{
    class ReportUFNS013Filler : CommonUFNSReportFiller
    {
        private const int YearLevel = 0;
        private const int FirstDataColumn = 3;

        private DataTable GetColumnsTable(IEnumerable<int> years, int halfsCount, DataTable dtMarks)
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
                dt.Rows.Add(DBNull.Value, i, YearLevel + 2, false, false, true);
            }

            foreach (var year in years)
            {
                dt.Rows.Add(String.Format("{0} год", year), FirstDataColumn, YearLevel, true, false, false);

                for (var j = 0; j < halfsCount; j++)
                {
                    var style = FirstDataColumn + j * 2;
                    dt.Rows.Add(DBNull.Value, style, YearLevel + 1, true, false, false);
                    foreach (DataRow row in dtMarks.Rows)
                    {
                        dt.Rows.Add(row[ReportDataServer.NAME], style, YearLevel + 2, false, row[ReportDataServer.IsRUB], true);
                    }
                }
            }

            return dt;
        }

        private void AddComments(HSSFSheet sheet, int rowIndex, DataTable dtMarks, int count)
        {
            var patr = sheet.CreateDrawingPatriarch();
            var row = sheet.GetRow(rowIndex);

            for (var i = 0; i < count; i++)
            {
                for (var n = 0; n < dtMarks.Rows.Count; n++)
                {
                    var cell = row.GetCell(FirstDataColumn + dtMarks.Rows.Count*i + n);
                    var note = Convert.ToString(dtMarks.Rows[n][ReportDataServer.NOTE]);
                    if (cell != null && note != String.Empty)
                    {
                        var height = note.Split(new[] {"\r\n"}, StringSplitOptions.None).Length + 1;
                        var anchor = new HSSFClientAnchor(0, 0, 0, 0, cell.ColumnIndex + 1, 0, cell.ColumnIndex + 10, height);
                        var comment = patr.CreateComment(anchor);
                        comment.String = new HSSFRichTextString(note);
                        cell.CellComment = comment;
                    }
                }
            }
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var paramYears = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEARS));
            var paramHalfYear = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.HALF_YEAR));
            var years = ReportDataServer.ConvertToIntList(paramYears);
            var halfYear = ReportDataServer.ConvertToIntList(paramHalfYear);
            var dtHeader = GetColumnsTable(years, halfYear.Count, tableList[1]);
            
            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 8 - 1,
                StyleRowsCount = 5,
                StyleColumnsCount = 7,
                HeaderRowsCount = 4,
                FirstTotalColumn = FirstDataColumn,
                TotalRowsCount = 3,
                ExistHeaderNumColumn = true,
                Columns = GetDataColumnsIndexies(dtHeader),
                SumColumns = GetSumColumnsIndexies(dtHeader)
            };

            AddColumns(wb, sheetParams, dtHeader);
            FillSheet(wb, tableList, sheetParams);
            var sheet = wb.GetSheetAt(sheetParams.SheetIndex);
            AddComments(sheet, sheetParams.FirstRow - 2, tableList[1], years.Count*halfYear.Count);
        }
    }
}
