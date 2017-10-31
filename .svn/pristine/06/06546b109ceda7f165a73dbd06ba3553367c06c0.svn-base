using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.Export;
using Color=System.Drawing.Color;
using Graphics=System.Drawing.Graphics;
using Image=System.Drawing.Image;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class UltraGridExporter : UserControl
    {
        private bool multiHeader = false;
        private float headerChildCellHeight = 30;
        private float headerCellHeight = 30;

        public const string ExelPercentFormat = "0.00%";
        public const string ExelNumericFormat = "#,##0.00;[Red]-#,##0.00";

        public const int ExelCellSizeMultiplier = 37;

        public string gridElementCaption = String.Empty;

        public UltraWebGridExcelExporter ExcelExporter
        {
            get { return ultraWebGridExcelExporter; }
        }

        public UltraWebGridDocumentExporter PdfExporter
        {
            get { return ultraWebGridDocumentExporter; }
        }

        public LinkButton ExcelExportButton
        {
            get { return excelExportButton; }
        }

        public LinkButton PdfExportButton
        {
            get { return pdfExportButton; }
        }

        public LinkButton WordExportButton
        {
            get { return wordExportButton; }
        }

        public bool MultiHeader
        {
            get { return multiHeader; }
            set { multiHeader = value; }
        }

        public float HeaderChildCellHeight
        {
            get { return headerChildCellHeight; }
            set { headerChildCellHeight = value; }
        }

        public float HeaderCellHeight
        {
            get { return headerCellHeight; }
            set { headerCellHeight = value; }
        }

        public string GridElementCaption
        {
            get { return gridElementCaption; }
            set { gridElementCaption = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PrintVersion"] != null && (bool)Session["PrintVersion"])
            {
                this.Visible = false;
            }

            // Нужно указать обработчику хттп режим экспорта.
            // Сессия не доступна, напишем прямо в респонс.
            if (Session["WordExportMode"] != null && (bool)Session["WordExportMode"])
            {
                Response.Write("WordExportMode");
            }

            ultraWebGridDocumentExporter.TargetPaperOrientation =
                PageOrientation.Landscape;
            if (MultiHeader)
            {
                ultraWebGridExcelExporter.BeginExport +=
                        new BeginExportEventHandler(ultraWebGridExcelExporter_BeginExport);
                ultraWebGridExcelExporter.RowExporting +=
                        new RowExportingEventHandler(ultraWebGridExcelExporter_RowExporting);
                ultraWebGridExcelExporter.EndExport += 
                        new EndExportEventHandler(ultraWebGridExcelExporter_EndExport);
                ultraWebGridDocumentExporter.HeaderCellExporting += 
                        new EventHandler<MarginCellExportingEventArgs>(ultraWebGridDocumentExporter_HeaderCellExporting);
            }
            ultraWebGridDocumentExporter.InitializeRow += new EventHandler<DocumentExportInitializeRowEventArgs>(ultraWebGridDocumentExporter_InitializeRow);
            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ultraWebGridDocumentExporter_BeginExport);
            ultraWebGridDocumentExporter.CellExporting += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.CellExportingEventArgs>(ultraWebGridDocumentExporter_CellExporting);
        }

        #region pdf экспорт
        private Collection<float> cellsWidth;
        private Collection<string> cellsCaption;

        /// <summary>
        /// Делает двухуровневый заголовок для ультрагрида.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraWebGridDocumentExporter_HeaderCellExporting(object sender, MarginCellExportingEventArgs e)
        {
            // сохраняем информацию о родных ячейках
            cellsWidth.Add(((FixedWidth)e.ReportCell.Width).Value);
            cellsCaption.Add(e.ExportValue.ToString());

            // и скрываем их
            e.ReportCell.Height = new FixedHeight(0);

            // Если это последняя колонка или все последующие скрыты
            if (!(LastColumn(e) || NextColumnsHidden(e)))
                return;

            // Вся информация собрана, начинаем формировать заголовок.

            // Добавляем строку
            e.ReportCell.Parent.Parent.Header.Repeat = true;

            // Добавляем ячейку под хидеры
            ITableCell c = e.ReportCell.Parent.Parent.Header.AddCell();
            
            // Добавляем таблицу
            ITable table = c.AddTable();
            // добавлем строку

            if (gridElementCaption != String.Empty)
            {
                table.AddRow().AddCell().AddQuickText(gridElementCaption);
            }

            ITableRow mainHeaderRow = table.AddRow();
            
            // Добавляем ячейку под селектор
            AddSelectorCell(e, mainHeaderRow);
            
            // Поехали добавлять
            int orignX = 0;
            int invisibleColumnsCount = 0;
            foreach (UltraGridColumn item in e.Column.Band.Columns)
            {
                if (item.Hidden || !item.Visible)
                {
                    invisibleColumnsCount++;
                }
            }
            int originForseChangeCount = 0;
            while (orignX < e.Column.Band.Columns.Count - invisibleColumnsCount)
            {
                bool originChanged = false;
                foreach (HeaderBase item in e.Column.Band.HeaderLayout)
                {
                    // Проверяем итемы с ориджин х 0
                    if (item.RowLayoutColumnInfo.OriginX == orignX &&
                        item.RowLayoutColumnInfo.OriginY == 0)
                    {
                        // Смотрим спаны по иксу 
                        // если спан == 1, то 
                        ITableCell headerCell;
                        if (item.RowLayoutColumnInfo.SpanX == 1)
                        {
                            // просто добавляем
                            headerCell = mainHeaderRow.AddCell();
                            SetCellStyle(e, headerCell);
                            headerCell.Width = new FixedWidth(cellsWidth[orignX]);
                            IText t = headerCell.AddText();
                            SetFontStyle(t);
                            t.AddContent(item.Caption);
                        }
                        else if (item.Caption == "Несопоставленные данные")
                        {
                            headerCell = mainHeaderRow.AddCell();
                            SetCellStyle(e, headerCell);
                            headerCell.Width = new FixedWidth(cellsWidth[orignX]);
                            IText t = headerCell.AddText();
                            SetFontStyle(t);
                            t.AddContent(cellsCaption[orignX]);
                        }
                        else
                        {
                            // Иначе добавляем новую таблицу
                            ITableCell mainHeaderCell = mainHeaderRow.AddCell();
                            table = mainHeaderCell.AddTable();
                            // Добавляем строку и ячейку, растянется сама
                            ITableRow headerRow = table.AddRow();
                            headerCell = headerRow.AddCell();
                            SetCellStyle(e, headerCell);
                            IText t = headerCell.AddText();
                            SetFontStyle(t);
                            t.AddContent(item.Caption);
                            headerCell.Height = new FixedHeight(HeaderCellHeight);
                            // Добавляем еще строку
                            headerRow = table.AddRow();
                            // добавляем ячейки в соответствии со спаном
                            float width = 0;
                            for (int i = 0; i < item.RowLayoutColumnInfo.SpanX; i++)
                            {
                                ITableCell headerChildCell = headerRow.AddCell();
                                SetCellStyle(e, headerChildCell);
                                if ((i + orignX) < cellsWidth.Count)
                                {
                                    headerChildCell.Width = new FixedWidth(cellsWidth[i + orignX - originForseChangeCount]);
                                    headerChildCell.Height = new FixedHeight(headerChildCellHeight);
                                    width += cellsWidth[i + orignX - originForseChangeCount];
                                    t = headerChildCell.AddText();
                                    SetFontStyle(t);
                                    t.AddContent(cellsCaption[i + orignX - originForseChangeCount]);
                                }
                            }
                            headerCell.Width = new FixedWidth(width);
                            mainHeaderCell.Width = new FixedWidth(width);
                        }
                        // Смотрим, какой получился ориджин х и идем дальше.
                        orignX += item.RowLayoutColumnInfo.SpanX;
                        originChanged = true;
                        break;
                    }
                }
                // Если перебрали все итемы и не нашли ориджин
                // значит он соответствует скрытой колонке.
                // попробуем поискать дальше
                if (!originChanged)
                {
                    orignX++;
                    originForseChangeCount++;
                }
            }
        }

        private static bool LastColumn(MarginCellExportingEventArgs e)
        {
            return e.Column.Index == e.Column.Band.Columns.Count - 1;
        }

        private static bool NextColumnsHidden(MarginCellExportingEventArgs e)
        {
            for (int i = e.Column.Index + 1; i < e.Column.Band.Columns.Count; i++)
            {
                if (!e.Column.Band.Columns[i].Hidden)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool LastColumnHidden(MarginCellExportingEventArgs e)
        {
            return e.Column.Band.Columns[e.Column.Band.Columns.Count - 1].Hidden;
        }

        private static bool PenultimateColumn(MarginCellExportingEventArgs e)
        {
            return e.Column.Index == e.Column.Band.Columns.Count - 2;
        }

        public static void SetCellStyle(MarginCellExportingEventArgs e, ITableCell headerCell)
        {
            headerCell.Alignment.Horizontal = Alignment.Center;
            headerCell.Alignment.Vertical = Alignment.Middle;
            headerCell.Borders = e.ReportCell.Borders;
            headerCell.Paddings.All = 2;
            headerCell.Background = e.ReportCell.Background;
        }

        public static void SetFontStyle(IText t)
        {
            Infragistics.Documents.Reports.Graphics.Font font =
                new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            t.Style.Font = font;
            t.Style.Font.Bold = true;
            t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;
        }
        
        public static void AddSelectorCell(MarginCellExportingEventArgs e, ITableRow row)
        {
            ITableCell c = row.AddCell();
            c.Borders = e.ReportCell.Borders;
            System.Drawing.Color col = System.Drawing.Color.FromArgb(210, 210, 210);

            SolidBrush sbr = new SolidBrush(col);
            Infragistics.Documents.Reports.Graphics.SolidColorBrush br =
                new Infragistics.Documents.Reports.Graphics.SolidColorBrush(sbr);
            Background b = new Background(br);
            c.Background = b;
            c.Width = new FixedWidth(16);
        }
        #endregion

        #region excel экспорт
        /// <summary>
        /// Сливает ячейки хидера по вертикали
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ultraWebGridExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int startRow = ultraWebGridExcelExporter.ExcelStartRow;

            Infragistics.Documents.Excel.WorksheetCellCollection cells =
                    e.CurrentWorksheet.Rows[startRow - 1].Cells;
            Infragistics.Documents.Excel.WorksheetCellCollection childCells =
                    e.CurrentWorksheet.Rows[startRow].Cells;
            int cellCounter = 0;
            Infragistics.Documents.Excel.WorksheetCell childCell = cells[cellCounter];
            Infragistics.Documents.Excel.WorksheetCell worksheetCell = childCells[cellCounter];
            while (!(childCell.Value == null && worksheetCell.Value == null))
            {
                childCell.CellFormat.Alignment = Infragistics.Documents.Excel.HorizontalCellAlignment.Center;
                childCell.CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center;

                if (childCell.Value != null && worksheetCell.Value != null &&
                       (childCell.Value.ToString() == worksheetCell.Value.ToString() ||
                       childCell.Value.ToString() == "Несопоставленные данные" ||
                       childCell.Value.ToString() == "'Значение не указано'" ||
                       String.IsNullOrEmpty(childCell.Value.ToString())))
                {
                    childCell.Value = worksheetCell.Value;
                    e.CurrentWorksheet.MergedCellsRegions.Add(
                            startRow - 1, cellCounter, startRow, cellCounter);
                }
                cellCounter++;
                childCell = cells[cellCounter];
                worksheetCell = childCells[cellCounter]; 
            }
        }

        /// <summary>
        /// Сливает ячейки хидера по горизонтали
        /// </summary>
        public void ultraWebGridExcelExporter_RowExporting(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.RowExportingEventArgs e)
        {
            if (e.CurrentRowIndex > this.ExcelExporter.ExcelStartRow)
                return;
            // сливаем по горизонтали
            Infragistics.Documents.Excel.WorksheetCellCollection cells =
                    e.CurrentWorksheet.Rows[e.CurrentRowIndex - 1].Cells;

            Infragistics.Documents.Excel.WorksheetCell worksheetCell = cells[0];
            Infragistics.Documents.Excel.WorksheetCell childCell = cells[1];
            int cellCounter = 1;
            while (childCell.Value != null && worksheetCell.Value != null)
            {
                int firstMergedColumn = cellCounter - 1;
                while (childCell.Value != null && worksheetCell.Value != null &&
                       childCell.Value.ToString() == worksheetCell.Value.ToString())
                {
                    cellCounter++;
                    childCell = cells[cellCounter];
                }
                if (firstMergedColumn != cellCounter - 1)
                {
                    e.CurrentWorksheet.MergedCellsRegions.Add(
                            e.CurrentRowIndex - 1, firstMergedColumn,
                            e.CurrentRowIndex - 1, cellCounter - 1);
                }
                worksheetCell = cells[cellCounter];
                cellCounter++;
                childCell = cells[cellCounter];
            }
        }

        /// <summary>
        /// Добавляет хидер второго уровня
        /// </summary>
        private void ultraWebGridExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            UltraGridRow newRow = new UltraGridRow();
            foreach (UltraGridColumn item in e.Layout.Bands[0].Columns)
            {
                UltraGridCell cell = new UltraGridCell(item.HeaderText);
                cell.Style.HorizontalAlign = HorizontalAlign.Left;
                newRow.Cells.Add(cell);
                newRow.Cells[newRow.Cells.Count - 1].Style.BackColor = System.Drawing.Color.FromArgb(211, 211, 211);
            }
            e.Rows.Insert(0, newRow, true);
        }

        /// <summary>
        /// Правит рендеринг буквы 'ы'в ячейках грида.
        /// </summary>
        private void ultraWebGridDocumentExporter_InitializeRow(object sender, DocumentExportInitializeRowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Font.Name = "Arial";
            }
        }

        private void ultraWebGridDocumentExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            if (e.Layout.Bands[0] != null)
            {
                e.Layout.Bands[0].HeaderStyle.Font.Name = "Arial";
            }
            cellsWidth = new Collection<float>();
            cellsCaption = new Collection<string>();
        }

        private void ultraWebGridDocumentExporter_CellExporting(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.CellExportingEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.GridCell.Style.BackgroundImage))
			{
				string imagePath = HttpContext.Current.Server.MapPath(e.GridCell.Style.BackgroundImage);

				Color backColor = Color.Transparent;
				if (e.ReportCell.Background.Brush.Type == BrushType.SolidColor)
				{
					Infragistics.Documents.Reports.Graphics.Color color = ((SolidColorBrush)e.ReportCell.Background.Brush).Color;
					backColor = Color.FromArgb(unchecked((int)(color.ToARGB())));
				}

                CellBackgroundDrawing drawing = new CellBackgroundDrawing(imagePath, backColor);
				drawing.Scale = Convert.ToSingle(1);
				drawing.ImageBorderSize = 2;

				e.ReportCell.Background = new Background(e.ReportCell.Background.Brush, drawing);
			}
        }

        #endregion

        # region перерисовка картинок
        private static Graphics GetGraphics(Color backColor, Bitmap bitmap, Image image)
        {
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(backColor);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawImageUnscaled(image, 2, 2);
            return graphics;
        }

        private static Infragistics.Documents.Reports.Graphics.Image GetImg(Bitmap bitmap)
        {
            MemoryStream imageStream = new MemoryStream();
            bitmap.Save(imageStream, ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);
            img.Preferences.Compressor = ImageCompressors.Flate;
            return img;
        }

        private static Bitmap GetBitmap(Color backColor, Image image)
        {
            Bitmap bitmap = new Bitmap(image.Width + 4, image.Height + 4);
            bitmap.SetResolution(72, 72);
                
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (bitmap.GetPixel(x, y) == Color.FromArgb(0, 0, 0, 0))
                    {
                        bitmap.SetPixel(x, y, backColor);
                    }
                }
            }
            return bitmap;
        }

        public static Infragistics.Documents.Reports.Graphics.Image GetImageFromChart(UltraChart chart)
        { 
            Bitmap bitmap = new Bitmap((int)chart.Width.Value, (int)chart.Height.Value);
            bitmap.SetResolution(94, 94);
            Infragistics.Documents.Reports.Graphics.Image img;
            if (chart.ChartType == ChartType.PieChart)
            {
                MemoryStream imageStream = new MemoryStream();
                chart.SaveTo(imageStream, ImageFormat.Png);
                img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);
                img.Preferences.Compressor = ImageCompressors.Flate;
            }
            else
            {
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
                chart.RenderPdfFriendlyGraphics(graphics);
                img = GetImg(bitmap);
            }
            return img;
        }
        
        public static Infragistics.Documents.Reports.Graphics.Image GetImageFromMap(MapControl map)
        {
            MemoryStream stream = new MemoryStream();
            map.ZoomPanel.Visible = false;
            map.NavigationPanel.Visible = false;
            map.SaveAsImage(stream);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            bitmap.SetResolution(90, 90);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.DrawImageUnscaled(image, 0, 0);
            return GetImg(bitmap);
        }

        public static Infragistics.Documents.Reports.Graphics.Image GetImageFromGauge(UltraGauge gauge)
        {
            MemoryStream imageStream = new MemoryStream();
            gauge.SaveTo(imageStream, GaugeImageType.Png, new System.Drawing.Size((int)gauge.Width.Value, (int)gauge.Height.Value));
            System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);
            
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            bitmap.SetResolution(90, 90);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.DrawImageUnscaled(image, 0, 0);
            return GetImg(bitmap);
        }

        public static Infragistics.Documents.Reports.Graphics.Image GetImage(string path)
        {
            string imagePath = HttpContext.Current.Server.MapPath(path);
            // Загружаем картинку
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
            // Рисуем картинку с рамкой
            Bitmap bitmap = GetBitmap(Color.White, image);
            GetGraphics(Color.White, bitmap, image);
            // Создаем из нее картинку для ячейки.
            return GetImg(bitmap);
        }

        public static Infragistics.Documents.Reports.Graphics.Image GetInfragisticsImage(System.Drawing.Image image)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            bitmap.SetResolution(90, 90);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawImageUnscaled(image, 0, 0);
            return GetImg(bitmap);
        }

        #endregion

        protected void wordExportButton_Click(object sender, EventArgs e)
        {
            // Если нажали на кнопку, значит хотим экспортировать в ворд. К.О.
            // Но обработчик сработает когда будет уже поздно включать принт версион
            // Тогда включим его и перегрузим страницу.
            // Но будет непонятно, то ли это просто принт версион, то ли экспорт.
            // Поэтому еще включим режим экспорта.
            // Больше обработчик не сработает, поэтому дальше работаем в пейдж лоад компонента
            Session["PrintVersion"] = true;
            Session["WordExportMode"] = true;
            Response.Redirect(Request.FilePath, true);
        }

        public static Image GetExcelImage(UltraChart chart)
        {
            MemoryStream imageStream = new MemoryStream();
            chart.SaveTo(imageStream, ImageFormat.Png);
            return Image.FromStream(imageStream);
        }

        public static Image GetExcelImage(MapControl map)
        {
            MemoryStream imageStream = new MemoryStream();
            map.ZoomPanel.Visible = false;
            map.NavigationPanel.Visible = false;
            map.SaveAsImage(imageStream);
            return Image.FromStream(imageStream);
        }

        public static void ChartExcelExport(WorksheetCell cell, UltraChart chart)
        {
            Image image = GetExcelImage(chart);
            ImageExcelExport(cell, image, (int)chart.Width.Value, (int)chart.Height.Value);
        }

        public static void MapExcelExport(WorksheetCell cell, MapControl map)
        {
            Image image = GetExcelImage(map);
            ImageExcelExport(cell, image, (int)map.Width.Value, (int)map.Height.Value);
        }

        public static void ImageExcelExport(WorksheetCell cell, Image image, int width, int height)
        {
            WorksheetImage excelImage = new WorksheetImage(image);
            excelImage.TopLeftCornerCell = cell;
            excelImage.BottomRightCornerCell = cell;
            cell.Worksheet.Shapes.Add(excelImage);
            excelImage.SetBoundsInTwips(cell.Worksheet,
                                        new System.Drawing.Rectangle(
                                            excelImage.TopLeftCornerCell.GetBoundsInTwips().Left,
                                            excelImage.TopLeftCornerCell.GetBoundsInTwips().Top,
                                            20 * width, 20 * height),
                                        true);
        }
    }

    /// <summary>
    /// Воспомогательный класс для отрисовки картинок в гриде.
    /// </summary>
    internal class CustomDrawing : IDrawing
    {
        private Infragistics.Documents.Reports.Graphics.Image image;

        public CustomDrawing(Infragistics.Documents.Reports.Graphics.Image image)
        {
            this.image = image;
        }

        public void OnDraw(IGraphics graphics, float x, float y, float width, float height)
        {
            Infragistics.Documents.Reports.Graphics.TextureBrush textureBrush = new Infragistics.Documents.Reports.Graphics.TextureBrush(image);
            textureBrush.ScaleX = 0.7f;
            textureBrush.ScaleY = 0.7f;
            graphics.Brush = textureBrush;
            float num = textureBrush.Image.Width * textureBrush.ScaleX;
            float num2 = textureBrush.Image.Height * textureBrush.ScaleY;

            graphics.FillRectangle(x, y, num, num2);
        }
    }

    /// <summary>
    /// Воспомогательный класс для отрисовки картинок в гриде
    /// </summary>
    public class CellBackgroundDrawing : IDrawing
    {
        // масштаб картинок, выполняется в любом случае
        private const float primaryScale = 0.75f;
        private string imgPath;

        private Infragistics.Documents.Reports.Graphics.Image Picture { set; get; }
        public float ImageBorderSize { set; get; }
        public float Scale { set; get; }

        public CellBackgroundDrawing(string imagePath, Color backColor)
        {
            imgPath = imagePath;

            // Загружаем картинку
            Image imageLoaded = Image.FromFile(imgPath);

            // Рисуем картинку с рамкой
            Bitmap bitmap = new Bitmap(imageLoaded.Width + 4, imageLoaded.Height + 4);
            bitmap.SetResolution(72, 72);

            // Создаем из нее картинку для ячейки.
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                SolidBrush brush = new SolidBrush(backColor);
                graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                graphics.DrawImageUnscaled(imageLoaded, 2, 2);
            }

            Picture = new Infragistics.Documents.Reports.Graphics.Image(bitmap);
            Picture.Preferences.Compressor = ImageCompressors.Flate;
        }

        public void OnDraw(IGraphics graphics, float x, float y, float width, float height)
        {
            float scale = primaryScale * Scale;
            float newWidth = Picture.Width * scale;
            float newHeight = Picture.Height * scale;
            float offsetX = CRHelper.PixelsToPoints(5);
            float offsetY = (height - CRHelper.PixelsToPoints(newHeight) - ImageBorderSize) / 2;
            
            Infragistics.Documents.Reports.Graphics.TextureBrush textureBrush = new Infragistics.Documents.Reports.Graphics.TextureBrush (Picture);
            textureBrush.ScaleX = scale;
            textureBrush.ScaleY = scale;
            graphics.Brush = textureBrush;
            if (imgPath.ToLower().Contains("corner"))
            {
                graphics.FillRectangle(x + width - newWidth, y, newWidth, newHeight);
            }
            else
            {
                graphics.FillRectangle(x + offsetX, y + offsetY, newWidth, newHeight);
            }
        }
    }
}