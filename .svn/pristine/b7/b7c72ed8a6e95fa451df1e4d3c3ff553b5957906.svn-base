using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Common.Converts;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Grid.Style;

namespace Krista.FM.Client.MDXExpert.Exporter
{
    abstract public class ElementExporter
    {
        private CustomReportElement _customElement;

        /// <summary>
        /// Область печати, берем ее за орентир ширины заголовка и комментария элемента
        /// </summary>
        const float sheetWidth = 623 * 0.75f;

        /// <summary>
        /// кол-во строк между экспортируемыми элементами
        /// </summary>
        private const int elementSpacing = 3;


        public ElementExporter(CustomReportElement customElement)
        {
            this.CustomElement = customElement;
        }

        //начальная строка экселя для экспорта
        public int GetStartRowIndex(Excel.Worksheet sheet)
        {
            //получаем последнюю строку предыдущего экспортированного элемента
            Excel.Name excelName = ExcelUtils.GetExcelName(sheet, Consts.elementLastRow);
            Excel.Range range = null;
            if (excelName != null)
            {
                range = excelName.RefersToRange;
            }

            int rowIndex = 1;

            if (range != null)
            {
                rowIndex = range.Row + elementSpacing;
            }
            return rowIndex; 
        }

        public float GetStartYCoordinate(Excel.Worksheet sheet, int rowIndex)
        {
            return (float)ExcelUtils.GetRowTop(sheet, rowIndex);
        }

        public void MapElementCaption(Excel.Worksheet sheet, bool isPrintVersion)
        {
            if ((sheet == null) ||!this.CustomElement.Caption.Visible)
                return;

            int startRowIndex = GetStartRowIndex(sheet);
            //Создадим форму с текстовым блоком
            Excel.Shape captionShape = sheet.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal,
                0, GetStartYCoordinate(sheet, startRowIndex), sheetWidth, ExcelUtils.DefaultRowHeight(sheet));
            //Получаем ссылку на сам блок
            Excel.TextBox captionTextBox = (Excel.TextBox)captionShape.DrawingObject;
            //Выставим высоту
            (sheet.Cells[startRowIndex, 1] as Excel.Range).RowHeight = this.CustomElement.Caption.Height * 0.75f;
            captionTextBox.Width = this.CustomElement.Caption.Width * 0.75f;
            captionTextBox.HorizontalAlignment = AlignConvertor.ToExcelHAlign(this.CustomElement.Caption.TextHAlign);
            captionTextBox.VerticalAlignment = AlignConvertor.ToExcelVAlign(this.CustomElement.Caption.TextVAlign);
            //Выставляем параметры шрифта
            UltraFontConvertor.SynchronizeFont(captionTextBox.Font, this.CustomElement.Caption.Font);
            ExcelUtils.SetTextToTextBox(captionTextBox, this.CustomElement.Caption.Text);

            if (this.CustomElement.Caption.BorderStyle == Infragistics.Win.UIElementBorderStyle.None)
            {                
                //Отключаем рамку
                captionTextBox.ShapeRange.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            }

            if (isPrintVersion)
                //Отключаем заливку
                captionTextBox.ShapeRange.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            else
                this.SetTextBoxStyle(captionTextBox, this.CustomElement.Caption.Style);
        }

        /// <summary>
        /// Нарисовать, и выставить размеры комментарию элемента.
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void MapElementComment(Excel.Worksheet sheet, float x, float y, float width, float height, bool isPrintVersion)
        {
            if ((sheet == null) || !this.CustomElement.Comment.Visible)
                return;
            //Создадим форму с текстовым блоком, так чтобы она ровно вписывалась в ячейку в которой распологается
            Excel.Shape commentShape = sheet.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal,
                0, 0, 48f, ExcelUtils.DefaultRowHeight(sheet));

            //Получаем ссылку на сам блок
            Excel.TextBox commentTextBox = (Excel.TextBox)commentShape.DrawingObject;

            switch (this.CustomElement.Comment.Place)
            {
                case CommentPlace.Top:
                case CommentPlace.Bottom:
                    {
                        commentTextBox.Top = y;
                        commentTextBox.Width = width;
                        //индекс строки в котрой размещен комментарий
                        int commentPlaceRowIndex = ExcelUtils.GetRowIndex(sheet, commentTextBox.Top + 1);
                        (sheet.Cells[commentPlaceRowIndex, 1] as Excel.Range).RowHeight = height;
                        break;
                    }
                case CommentPlace.Left:
                    {
                        commentTextBox.Top = y;
                        commentTextBox.Height = height;
                        (sheet.Cells[GetStartRowIndex(sheet), 1] as Excel.Range).ColumnWidth = width;
                        break;
                    }
                case CommentPlace.Right:
                    {
                        commentTextBox.Top = y;
                        commentTextBox.Left = x;
                        commentTextBox.Height = height;
                        //индекс колонки в которой размещен комментарий
                        int commentPlaceColumnIndex = ExcelUtils.GetColumnIndex(sheet, commentTextBox.Left + 1);
                        (sheet.Cells[GetStartRowIndex(sheet), commentPlaceColumnIndex] as Excel.Range).ColumnWidth = width;
                        break;
                    }
            }

            commentTextBox.HorizontalAlignment = AlignConvertor.ToExcelHAlign(this.CustomElement.Comment.TextHAlign);
            commentTextBox.VerticalAlignment = AlignConvertor.ToExcelVAlign(this.CustomElement.Comment.TextVAlign);

            UltraFontConvertor.SynchronizeFont(commentTextBox.Font, this.CustomElement.Comment.Font);
            ExcelUtils.SetTextToTextBox(commentTextBox, this.CustomElement.Comment.Text);

            if (this.CustomElement.Comment.BorderStyle == Infragistics.Win.UIElementBorderStyle.None)
            {
                //Отключаем рамку
                commentTextBox.ShapeRange.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            }

            if (isPrintVersion)
                //Отключаем заливку
                commentTextBox.ShapeRange.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            else
                this.SetTextBoxStyle(commentTextBox, this.CustomElement.Comment.Style);
        }

        /// <summary>
        /// Установить екслевскому текстовому окну цвет фона и шрифта
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="style"></param>
        private void SetTextBoxStyle(Excel.TextBox textBox, CellStyle style)
        {
            if ((textBox != null) && (style != null))
            {
                textBox.Font.Color = ColorTranslator.ToWin32(style.ForeColor);
                textBox.Interior.Color = ColorTranslator.ToWin32(style.BackColorStart);
            }
        }

        /// <summary>
        /// Нарисовать комментарий диаграммы.(Все размеры контролов задаются в пикселах, а
        /// размеры строк и столбцов в Excel измеряются в каких то своих экселевских единицах, причем они 
        /// разные для строк и столбцов. Именно поэтому для задания размера, в коде очень много извращения)
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="chartWidth">ширина диаграммы</param>
        /// <param name="chartHeight">высота диаграммы</param>
        public void MapChartComment(Excel.Worksheet sheet, float chartWidth, float chartHeight, bool isPrintVersion)
        {
            if ((sheet == null) || !this.CustomElement.Comment.Visible)
                return;

            float captionHeight = 0;
            int startRowIndex = GetStartRowIndex(sheet);

            if (this.CustomElement.Caption.Visible)
                captionHeight = (float)(double)(sheet.Cells[startRowIndex, 1] as Excel.Range).RowHeight;

            float x = 0;
            float y = GetStartYCoordinate(sheet, startRowIndex);
            float width = sheetWidth;
            float height = 50;

            switch (this.CustomElement.Comment.Place)
            {
                case CommentPlace.Top:
                    {
                        y += captionHeight;
                        width = this.CustomElement.Comment.Width * 0.75f;
                        height = this.CustomElement.Comment.Height * 0.75f;
                        break;
                    }
                case CommentPlace.Bottom:
                    {
                        y += captionHeight + chartHeight;
                        width = this.CustomElement.Comment.Width * 0.75f;
                        height = this.CustomElement.Comment.Height * 0.75f;
                        break;
                    }
                case CommentPlace.Left:
                    {
                        y += captionHeight;
                        if (sheet.UsedRange != null)
                            height = chartHeight;
                        else
                            height = this.CustomElement.Comment.Height * 0.75f;
                        width = this.CustomElement.Comment.Width / 7;
                        break;
                    }
                case CommentPlace.Right:
                    {
                        y += captionHeight;
                        x = chartWidth;
                        height = chartHeight;
                        width = this.CustomElement.Comment.Width / 7;
                        break;
                    }
            }
            this.MapElementComment(sheet, x, y, width, height, isPrintVersion);
        }

        /// <summary>
        /// Нарисовать комментарий таблицы.(Все размеры контролов задаются в пикселах, а
        /// размеры строк и столбцов в Excel измеряются в каких то своих экселевских единицах, причем они 
        /// разные для строк и столбцов. Именно поэтому для задания размера, в коде очень много извращения)
        /// </summary>
        /// <param name="sheet"></param>
        public void MapTableComment(Excel.Worksheet sheet, bool isPrintVersion)
        {
            if ((sheet == null) || !this.CustomElement.Comment.Visible)
                return;

            float captionHeight = 0;
            int startRowIndex = GetStartRowIndex(sheet);


            if (this.CustomElement.Caption.Visible)
                captionHeight = (float)(double)(sheet.Cells[startRowIndex, 1] as Excel.Range).RowHeight;
            float usedRangeHeight = (sheet.UsedRange != null) ? (float)(double)sheet.UsedRange.Height : 0;

            float x = 0;
            float y = GetStartYCoordinate(sheet, startRowIndex);
            float width = sheetWidth;
            float height = 50;

            switch (this.CustomElement.Comment.Place)
            {
                case CommentPlace.Top:
                    {
                        y += captionHeight;
                        width = this.CustomElement.Comment.Width * 0.75f;
                        height = this.CustomElement.Comment.Height * 0.75f;
                        break;
                    }
                case CommentPlace.Bottom:
                    {
                        y = usedRangeHeight;
                        width = this.CustomElement.Comment.Width * 0.75f;
                        height = this.CustomElement.Comment.Height * 0.75f;
                        break;
                    }
                case CommentPlace.Left:
                    {
                        y += captionHeight;
                        if (sheet.UsedRange != null)
                            height = usedRangeHeight - captionHeight;
                        else
                            height = this.CustomElement.Comment.Height * 0.75f;
                        width = this.CustomElement.Comment.Width / 7;
                        break;
                    }
                case CommentPlace.Right:
                    {
                        y += captionHeight;

                        if (sheet.UsedRange != null)
                        {
                            x = (float)(double)sheet.UsedRange.Width;
                            height = usedRangeHeight - captionHeight;
                        }
                        width = this.CustomElement.Comment.Width / 7;
                        break;
                    }
            }
            this.MapElementComment(sheet, x, y, width, height, isPrintVersion);
        }

        abstract public Excel.Worksheet ToExcel(Excel.Workbook workbook, bool isPrintVersion, bool isSeparateProperties,
            int beforeSheetIndex);

        abstract public Excel.Worksheet ToExcelWorksheet(Excel.Worksheet sheet, bool isPrintVersion, bool isSeparateProperites);

        public CustomReportElement CustomElement
        {
            get { return _customElement; }
            set { _customElement = value; }
        }

    }
}
