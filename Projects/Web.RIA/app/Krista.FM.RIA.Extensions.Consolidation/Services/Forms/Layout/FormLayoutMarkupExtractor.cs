using System;
using System.Collections.Generic;
using System.Drawing;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Models;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms
{
    public class FormLayoutMarkupExtractor
    {
        private readonly HSSFWorkbook workbook;
        private readonly HSSFSheet sheet;
        private readonly LayoutMarkupViewModel model;
        private readonly List<CellRangeAddress> mergedRegions;

        public FormLayoutMarkupExtractor(HSSFWorkbook workbook, HSSFSheet sheet, LayoutMarkupViewModel model)
        {
            this.workbook = workbook;
            this.sheet = sheet;
            this.model = model;
            mergedRegions = new List<CellRangeAddress>();
        }

        public LayoutMarkupCellViewModel GetCell(int rowIndex, int colIndex)
        {
            int colspan = 1;
            int rowspan = 1;
            int groupCellWidth = -1;
            short groupCellHeight = -1;

            var row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                throw new InvalidOperationException("Строка с номером {0} не имеет данных.".FormatWith(rowIndex + 1));
            }

            var cell = row.GetCell(colIndex);

            var mergeRange = GetMergedAdress(sheet, cell);
            if (mergeRange != null)
            {
                if (!mergedRegions.Contains(mergeRange))
                {
                    mergedRegions.Add(mergeRange);

                    colspan = mergeRange.LastColumn - mergeRange.FirstColumn + 1;
                    rowspan = mergeRange.LastRow - mergeRange.FirstRow + 1;

                    // Определяем ширину и высоту сгруппированной ячейки
                    for (int i = mergeRange.FirstColumn; i <= mergeRange.LastColumn; i++)
                    {
                        groupCellWidth += sheet.GetColumnWidth(i);
                    }

                    for (int i = mergeRange.FirstRow; i <= mergeRange.LastRow; i++)
                    {
                        groupCellHeight += sheet.GetRow(i).Height;
                    }
                }
                else
                {
                    return null;
                }
            }

            LayoutMarkupCellViewModel layoutMarkupCell = new LayoutMarkupCellViewModel();

            // Размеры ячейки
            layoutMarkupCell.Colspan = colspan;
            layoutMarkupCell.Rowspan = rowspan;
            layoutMarkupCell.Width = groupCellWidth != -1 ? groupCellWidth : sheet.GetColumnWidth(colIndex);
            layoutMarkupCell.Height = groupCellHeight != -1 ? groupCellHeight : sheet.GetRow(rowIndex).Height;

            layoutMarkupCell.Width =
                Convert.ToInt32(Math.Round(layoutMarkupCell.Width / 256.0 * 7 /*38.75*/, MidpointRounding.AwayFromZero));
            layoutMarkupCell.Height = Convert.ToInt16(Math.Round(layoutMarkupCell.Height / 15.46));

            if (cell != null && cell.CellStyle.Indention != 0)
            {
                layoutMarkupCell.Width -= cell.CellStyle.Indention * 12;
            }

            // Получаем значение ячейки
            layoutMarkupCell.Value = layoutMarkupCell.Text = GetCellText(cell);

            // Формируем стиль ячейки
            AddCellStyle(workbook, layoutMarkupCell, cell, model);

            return layoutMarkupCell;
        }

        private string GetCellText(HSSFCell cell)
        {
            if (cell == null)
            {
                return String.Empty;
            }

            string cellText = String.Empty;
            if (cell.CellType == HSSFCell.CELL_TYPE_STRING)
            {
                cellText = cell.StringCellValue;
            }
            else if (cell.CellType == HSSFCell.CELL_TYPE_NUMERIC)
            {
                cellText = new HSSFDataFormatter().FormatCellValue(cell);
            }
            else if (cell.CellType == HSSFCell.CELL_TYPE_BOOLEAN)
            {
                cellText = Convert.ToString(cell.BooleanCellValue);
            }

            return cellText;
        }

        private void AddCellStyle(HSSFWorkbook wb, LayoutMarkupCellViewModel layoutMarkupCell, HSSFCell cell, LayoutMarkupViewModel model)
        {
            if (cell == null)
            {
                return;
            }

            layoutMarkupCell.Style = cell.CellStyle.Index;
            if (!model.Styles.ContainsKey(cell.CellStyle.Index))
            {
                Dictionary<string, string> style = GetStyle(cell, wb);

                // Если ячейка имеет вертикальное выравнивание по середине или по нижней границе,
                // то добавляем этот стить отдельно
                if (cell.CellStyle.VerticalAlignment != HSSFCellStyle.VERTICAL_TOP || cell.CellStyle.Indention != 0)
                {
                    var innerStyle = new Dictionary<string, string>();

                    if (cell.CellStyle.VerticalAlignment != HSSFCellStyle.VERTICAL_TOP)
                    {
                        innerStyle.Add("vertical-align", NPOIHelper.StyleAlignVertical[cell.CellStyle.VerticalAlignment]);
                        innerStyle.Add("display", "table-cell");
                    }

                    if (cell.CellStyle.Indention != 0)
                    {
                        innerStyle.Add("padding-left", "{0}px".FormatWith(cell.CellStyle.Indention * 12));
                    }

                    if (innerStyle.Count > 0)
                    {
                        model.StylesInnerCell.Add(cell.CellStyle.Index, innerStyle);
                    }
                }

                model.Styles.Add(cell.CellStyle.Index, style);
            }
        }

        private Dictionary<string, string> GetStyle(HSSFCell cell, HSSFWorkbook wb)
        {
            var cssStyle = new Dictionary<string, string>();

            cssStyle.Add("text-align", NPOIHelper.StyleAlign[cell.CellStyle.Alignment]);
            cssStyle.Add("white-space", cell.CellStyle.WrapText ? "pre-wrap" : "pre");

            HSSFFont hssfFont = cell.CellStyle.GetFont(wb);
            Font font = cell.Sheet.HSSFFont2Font(hssfFont);
            cssStyle.Add("font-family", font.FontFamily.Name);
            cssStyle.Add("font-size", "{0}pt".FormatWith(hssfFont.FontHeightInPoints));
            cssStyle.Add("font-weight", hssfFont.Boldweight == HSSFFont.BOLDWEIGHT_BOLD ? "bold" : "normal");

            cssStyle.Add("color", "black");

            cssStyle.Add("background-color", NPOIHelper.StyleColor[cell.CellStyle.FillForegroundColor]);

            cssStyle.Add("border-top-style", NPOIHelper.StyleBorder[cell.CellStyle.BorderTop]);
            cssStyle.Add("border-left-style", NPOIHelper.StyleBorder[cell.CellStyle.BorderLeft]);
            cssStyle.Add("border-right-style", NPOIHelper.StyleBorder[cell.CellStyle.BorderRight]);
            cssStyle.Add("border-bottom-style", NPOIHelper.StyleBorder[cell.CellStyle.BorderBottom]);

            cssStyle.Add("border-color", "Black");
            cssStyle.Add("border-width", "1px");

            return cssStyle;
        }

        /// <summary>
        /// Определяет входит ли ячейка в группу ячеек. Если входит, то возвращает 
        /// адрес (диапазон) сгруппированной ячейки.
        /// </summary>
        /// <param name="sheet">Лист Excel</param>
        /// <param name="cell">Ячейка листа.</param>
        private CellRangeAddress GetMergedAdress(HSSFSheet sheet, HSSFCell cell)
        {
            if (cell == null)
            {
                return null;
            }

            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                var range = sheet.GetMergedRegion(i);
                if (cell.ColumnIndex >= range.FirstColumn && cell.ColumnIndex <= range.LastColumn && cell.RowIndex >= range.FirstRow && cell.RowIndex <= range.LastRow)
                {
                    return range;
                }
            }

            return null;
        }
    }
}