using System;
using Krista.FM.Client.MDXExpert.Common;
using System.Windows.Forms;
using System.IO;

namespace Krista.FM.Client.MDXExpert.Exporter
{
    class MapElementExporter : ElementExporter
    {
        const string mapImageName = "tempMapImage.ipg";

        public MapElementExporter(CustomReportElement element)
            : base(element)
        {
        }

        public override Excel.Worksheet ToExcelWorksheet(Excel.Worksheet sheet, bool isPrintVersion, bool isSeparatePropperties)
        {
            if (sheet == null)
                return null;

            sheet.Application.StatusBar = sheet.Application.StatusBar
                + "Заголовок экспортируемой карты \"" + this.Element.Title + "\". ";

            //string imagePath = Application.StartupPath + "\\" + mapImageName;
            string imagePath = Consts.UserAppDataPath + "\\" + mapImageName;

            //Сначала сохраним изображение карты
            this.Element.Map.SaveAsImage(imagePath, Dundas.Maps.WinControl.MapImageFormat.Jpeg);
            //Теперь загрузим его в лист
            Excel.Shape mapShape = sheet.Shapes.AddPicture(imagePath,
                Microsoft.Office.Core.MsoTriState.msoFalse,
                Microsoft.Office.Core.MsoTriState.msoTrue,
                this.GetMapXLocation(), this.GetMapYLocation(sheet),
                this.Element.Map.Width, this.Element.Map.Height); //Chart.Image.Width, this.Element.Chart.Image.Height);
            //Приведем масштаб изображения карты на листе, в значения по умолчанию
            mapShape.ScaleHeight(1f, Microsoft.Office.Core.MsoTriState.msoTrue, Type.Missing);
            mapShape.ScaleWidth(1f, Microsoft.Office.Core.MsoTriState.msoTrue, Type.Missing);

            //Для того чтобы была возможность выставить размеры комментарию, необходимо что 
            //бы край(правый или нижний, в зависимости от положения комментария) изображения
            //карты соприкасался с границей ячейки
            if (this.Element.Comment.Visible)
            {
                switch (this.Element.Comment.Place)
                {
                    case CommentPlace.Bottom:
                        {
                            int nextRowIndex = mapShape.BottomRightCell.Row + 1;
                            //подгоняем размер нижней ячейки под окончание карты
                            mapShape.BottomRightCell.RowHeight =
                                ((double)mapShape.BottomRightCell.RowHeight / 0.75 -
                                ((float)(double)(sheet.Cells[nextRowIndex, 1] as Excel.Range).Top - 1
                                - (mapShape.Height + mapShape.Top))) * 0.75;
                            break;
                        }
                    case CommentPlace.Right:
                        {
                            Excel.Range nextColumn = (Excel.Range)sheet.Cells[1, mapShape.BottomRightCell.Column + 1];
                            float mapShapeRight = mapShape.Width + mapShape.Left;
                            while ((double)nextColumn.Left > mapShapeRight)
                            {
                                mapShape.BottomRightCell.ColumnWidth = (double)mapShape.BottomRightCell.ColumnWidth - 0.2f;
                            }
                            break;
                        }
                }
            }

            //Удалим временное изображение карты
            File.Delete(imagePath);

            base.MapElementCaption(sheet, isPrintVersion);
            base.MapChartComment(sheet, mapShape.Width, mapShape.Height, isPrintVersion);

            int elementLastRowIndex = mapShape.BottomRightCell.Row + 1;
            if (this.CustomElement.Comment.Visible && (this.CustomElement.Comment.Place == CommentPlace.Bottom))
            {
                elementLastRowIndex++;
            }
            ExcelUtils.MarkExcelName((sheet.Cells[elementLastRowIndex, 1] as Excel.Range),
                                     Consts.elementLastRow, false);

            //Параметры печати, устанавливаем так что-бы карта печаталась на одном листе
            //  mapSheet.PageSetup.Zoom = false;
            //  mapSheet.PageSetup.FitToPagesWide = 1;
            //  mapSheet.PageSetup.FitToPagesTall = 1;

            sheet.Application.StatusBar = false;

            return sheet;
        }


        public override Excel.Worksheet ToExcel(Excel.Workbook workbook, bool isPrintVersion, bool isSeparateProperties, int beforeSheetIndex)
        {
            if (workbook == null)
                return null;

            workbook.Application.StatusBar = workbook.Application.StatusBar
                + "Заголовок экспортируемой карты \"" + this.Element.Title + "\". ";

            //Вставляем лист в который будем экспортировать
            Excel.Worksheet mapSheet = (Excel.Worksheet)workbook.Sheets.Add(workbook.Sheets[beforeSheetIndex], 
                Type.Missing, Type.Missing, Type.Missing);

            mapSheet.Name = ExcelUtils.GetSheetName(workbook, this.Element.Title);

            return this.ToExcelWorksheet(mapSheet, isPrintVersion, isSeparateProperties);
        }

        private float GetMapXLocation()
        {
            float x = 0;

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Left)
                x += 48f;

            return x;
        }

        private float GetMapYLocation(Excel.Worksheet sheet)
        {
            int startRowIndex = GetStartRowIndex(sheet);
            float y = GetStartYCoordinate(sheet, startRowIndex);

            if (this.Element.Caption.Visible)
                y += ExcelUtils.DefaultRowHeight(sheet);

            if (this.Element.Comment.Visible && this.Element.Comment.Place == CommentPlace.Top)
                y += ExcelUtils.DefaultRowHeight(sheet);

            return y;
        }

        public MapReportElement Element
        {
            get { return (MapReportElement)base.CustomElement; }
        }
    }
}
