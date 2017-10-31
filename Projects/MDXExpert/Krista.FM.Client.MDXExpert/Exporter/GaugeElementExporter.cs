using System;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.MDXExpert.Common;
using System.Windows.Forms;
using System.IO;

namespace Krista.FM.Client.MDXExpert.Exporter
{
    class GaugeElementExporter : ElementExporter
    {
        const string gaugeImageName = "tempGaugeImage.png";

        public GaugeElementExporter(CustomReportElement element)
            : base(element)
        {
        }

        public override Excel.Worksheet ToExcelWorksheet(Excel.Worksheet sheet, bool isPrintVersion, bool isSeparateProperties)
        {
            if (sheet == null)
                return null;

            sheet.Application.StatusBar = sheet.Application.StatusBar
                + "Заголовок экспортируемого индикатора \"" + this.Element.Title + "\". ";

            //string imagePath = Application.StartupPath + "\\" + chartImageName;
            string imagePath = Consts.UserAppDataPath + "\\" + gaugeImageName;

           // this.Element.Gauge Chart.DataBind();
            //Сначала сохраним изображение индикатора
            this.Element.GetBitmap().Save(imagePath, ImageFormat.Png); // Gauge.SaveTo(imagePath, GaugeImageType.Png, this.Element.Gauge.Size);
            //Теперь загрузим его в лист
            Excel.Shape gaugeShape = sheet.Shapes.AddPicture(imagePath,
                Microsoft.Office.Core.MsoTriState.msoFalse,
                Microsoft.Office.Core.MsoTriState.msoTrue,
                this.GetGaugeXLocation(), this.GetGaugeYLocation(sheet),
                this.Element.ElementPlace.Width, this.Element.ElementPlace.Height);
            //Приведем масштаб изображения диаграммы на листе, в значения по умолчанию
            gaugeShape.ScaleHeight(1f, Microsoft.Office.Core.MsoTriState.msoTrue, Type.Missing);
            gaugeShape.ScaleWidth(1f, Microsoft.Office.Core.MsoTriState.msoTrue, Type.Missing);
            
            //Для того чтобы была возможность выставить размеры комментарию, необходимо что 
            //бы край(правый или нижний, в зависимости от положения комментария) изображения
            //индикатора соприкасался с границей ячейки
            if (this.Element.Comment.Visible)
            {
                switch (this.Element.Comment.Place)
                {
                    case CommentPlace.Bottom:
                        {
                            int nextRowIndex = gaugeShape.BottomRightCell.Row + 1;
                            //подгоняем размер нижней ячейки под окончание диаграммы
                            gaugeShape.BottomRightCell.RowHeight =
                                ((double)gaugeShape.BottomRightCell.RowHeight / 0.75 -
                                ((float)(double)(sheet.Cells[nextRowIndex, 1] as Excel.Range).Top - 1
                                - (gaugeShape.Height + gaugeShape.Top))) * 0.75;
                            break;
                        }
                    case CommentPlace.Right:
                        {
                            Excel.Range nextColumn = (Excel.Range)sheet.Cells[1, gaugeShape.BottomRightCell.Column + 1];
                            float chartShapeRight = gaugeShape.Width + gaugeShape.Left;
                            while ((double)nextColumn.Left > chartShapeRight)
                            {
                                gaugeShape.BottomRightCell.ColumnWidth = (double)gaugeShape.BottomRightCell.ColumnWidth - 0.2f;
                            }
                            break;
                        }
                }
            }

            //Удалим временное изображение индикатора
            File.Delete(imagePath);

            base.MapElementCaption(sheet, isPrintVersion);
            base.MapChartComment(sheet, gaugeShape.Width, gaugeShape.Height, isPrintVersion);

            int elementLastRowIndex = gaugeShape.BottomRightCell.Row + 1;
            if (this.CustomElement.Comment.Visible && (this.CustomElement.Comment.Place == CommentPlace.Bottom))
            {
                elementLastRowIndex++;
            }
            ExcelUtils.MarkExcelName((sheet.Cells[elementLastRowIndex, 1] as Excel.Range),
                                     Consts.elementLastRow, false);

            //Параметры печати, устанавливаем так что-бы индикатор печатался на одном листе
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
                + "Заголовок экспортируемого индикатора \"" + this.Element.Title + "\". ";

            //Вставляем лист в который будем экспортировать
            Excel.Worksheet chartSheet = (Excel.Worksheet)workbook.Sheets.Add(workbook.Sheets[beforeSheetIndex], 
                Type.Missing, Type.Missing, Type.Missing);

            chartSheet.Name = ExcelUtils.GetSheetName(workbook, this.Element.Title);

            return this.ToExcelWorksheet(chartSheet, isPrintVersion, isSeparateProperties);
        }

        private float GetGaugeXLocation()
        {
            float x = 0;

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Left)
                x += 48f;

            return x;
        }

        private float GetGaugeYLocation(Excel.Worksheet sheet)
        {
            int startRowIndex = GetStartRowIndex(sheet);
            float y = GetStartYCoordinate(sheet, startRowIndex);

            if (this.Element.Caption.Visible)
                y += ExcelUtils.DefaultRowHeight(sheet);

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Top)
                y += ExcelUtils.DefaultRowHeight(sheet);

            return y;
        }

        
        public CustomReportElement Element
        {
            get { return base.CustomElement; }
        }
    }
}
