using System;
using Krista.FM.Client.MDXExpert.Common;
using System.Windows.Forms;
using System.IO;

namespace Krista.FM.Client.MDXExpert.Exporter
{
    class ChartElementExporter : ElementExporter
    {
        const string chartImageName = "tempChartImage.ipg";

        public ChartElementExporter(CustomReportElement element)
            : base(element)
        {
        }

        public override Excel.Worksheet ToExcelWorksheet(Excel.Worksheet sheet, bool isPrintVersion, bool isSeparateProperties)
        {
            if (sheet == null)
                return null;

            sheet.Application.StatusBar = sheet.Application.StatusBar
                + "Заголовок экспортируемой диаграммы \"" + this.Element.Title + "\". ";

            //string imagePath = Application.StartupPath + "\\" + chartImageName;
            string imagePath = Consts.UserAppDataPath + "\\" + chartImageName;

            //При экмпорте будем принудительно строить диаграмму заново, т.к. диаграммы
            //которые пользователь не смотрел со времени откытия отчета возвращают 
            //пустое изображение
            this.Element.Chart.DataBind();
            //Сначала сохраним изображение диаграммы
            this.Element.Chart.Image.Save(imagePath);
            //Теперь загрузим его в лист
            Excel.Shape chartShape = sheet.Shapes.AddPicture(imagePath,
                Microsoft.Office.Core.MsoTriState.msoFalse,
                Microsoft.Office.Core.MsoTriState.msoTrue,
                this.GetChartXLocation(), this.GetChartYLocation(sheet),
                this.Element.Chart.Image.Width, this.Element.Chart.Image.Height);
            //Приведем масштаб изображения диаграммы на листе, в значения по умолчанию
            chartShape.ScaleHeight(1f, Microsoft.Office.Core.MsoTriState.msoTrue, Type.Missing);
            chartShape.ScaleWidth(1f, Microsoft.Office.Core.MsoTriState.msoTrue, Type.Missing);
            
            //Для того чтобы была возможность выставить размеры комментарию, необходимо что 
            //бы край(правый или нижний, в зависимости от положения комментария) изображения
            //диаграммы соприкасался с границей ячейки
            if (this.Element.Comment.Visible)
            {
                switch (this.Element.Comment.Place)
                {
                    case CommentPlace.Bottom:
                        {
                            int nextRowIndex = chartShape.BottomRightCell.Row + 1;
                            //подгоняем размер нижней ячейки под окончание диаграммы
                            chartShape.BottomRightCell.RowHeight =
                                ((double)chartShape.BottomRightCell.RowHeight / 0.75 -
                                ((float)(double)(sheet.Cells[nextRowIndex, 1] as Excel.Range).Top - 1
                                - (chartShape.Height + chartShape.Top))) * 0.75;
                            break;
                        }
                    case CommentPlace.Right:
                        {
                            Excel.Range nextColumn = (Excel.Range)sheet.Cells[1, chartShape.BottomRightCell.Column + 1];
                            float chartShapeRight = chartShape.Width + chartShape.Left;
                            while ((double)nextColumn.Left > chartShapeRight)
                            {
                                chartShape.BottomRightCell.ColumnWidth = (double)chartShape.BottomRightCell.ColumnWidth - 0.2f;
                            }
                            break;
                        }
                }
            }

            //Удалим временное изображение диаграммы
            File.Delete(imagePath);

            base.MapElementCaption(sheet, isPrintVersion);
            base.MapChartComment(sheet, chartShape.Width, chartShape.Height, isPrintVersion);

            int elementLastRowIndex = chartShape.BottomRightCell.Row + 1;
            if (this.CustomElement.Comment.Visible && (this.CustomElement.Comment.Place == CommentPlace.Bottom))
            {
                elementLastRowIndex++;
            }
            ExcelUtils.MarkExcelName((sheet.Cells[elementLastRowIndex, 1] as Excel.Range),
                                     Consts.elementLastRow, false);

            //Параметры печати, устанавливаем так что-бы диаграмма печаталась на одном листе
            sheet.PageSetup.Zoom = false;
            sheet.PageSetup.FitToPagesWide = 1;
            sheet.PageSetup.FitToPagesTall = 1;

            sheet.Application.StatusBar = false;

            return sheet;

        }


        public override Excel.Worksheet ToExcel(Excel.Workbook workbook, bool isPrintVersion, bool isSeparateProperties, int beforeSheetIndex)
        {
            if (workbook == null)
                return null;

            workbook.Application.StatusBar = workbook.Application.StatusBar
                + "Заголовок экспортируемой диаграммы \"" + this.Element.Title + "\". ";

            //Вставляем лист в который будем экспортировать
            Excel.Worksheet chartSheet = (Excel.Worksheet)workbook.Sheets.Add(workbook.Sheets[beforeSheetIndex], 
                Type.Missing, Type.Missing, Type.Missing);

            chartSheet.Name = ExcelUtils.GetSheetName(workbook, this.Element.Title);

            return this.ToExcelWorksheet(chartSheet, isPrintVersion, isSeparateProperties);
        }

        private float GetChartXLocation()
        {
            float x = 0;

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Left)
                x += 48f;

            return x;
        }

        private float GetChartYLocation(Excel.Worksheet sheet)
        {
            int startRowIndex = GetStartRowIndex(sheet);
            float y = GetStartYCoordinate(sheet, startRowIndex);

            if (this.Element.Caption.Visible)
                y += ExcelUtils.DefaultRowHeight(sheet);

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Top)
                y += ExcelUtils.DefaultRowHeight(sheet);

            return y;
        }

        public ChartReportElement Element
        {
            get { return (ChartReportElement)base.CustomElement; }
        }
    }
}
