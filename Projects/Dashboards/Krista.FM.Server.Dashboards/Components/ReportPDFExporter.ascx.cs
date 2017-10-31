using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Common;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGauge;

namespace Krista.FM.Server.Dashboards.Components
{
    /// <summary>
    /// Экспортер отчета в PDF
    /// </summary>
    public partial class ReportPDFExporter : UserControl
    {
        #region Поля

        private GridHeaderLayout headerLayout;
        private UltraChart chart;
        private MapControl map;
        private UltraGauge gauge;
        private Image image;
        private string elementCaption;

        private Background headerBackground;
        private Borders headerBorders;
        private int headerCellHeight;

        private string pageTitle;
        private string pageSubTitle;

        private const string DownloadFileName = "report.pdf";
        private double PDFCellWidthMultiplier = 1.33333;

        private bool pageTitlesAdded;
        private ISection defaultExportSection;

        #endregion

        #region Свойства

        /// <summary>
        /// Экспортер в PDF
        /// </summary>
        public UltraWebGridDocumentExporter PdfExporter
        {
            get { return ultraWebGridDocumentExporter; }
        }

        /// <summary>
        /// Кнопка для экспорта
        /// </summary>
        public LinkButton PdfExportButton
        {
            get { return pdfExportButton; }
        }

        /// <summary>
        /// Заголовок страницы PDF
        /// </summary>
        public string PageTitle
        {
            get { return pageTitle; }
            set { pageTitle = value; }
        }

        /// <summary>
        /// Подзаголовок страницы PDF
        /// </summary>
        public string PageSubTitle
        {
            get { return pageSubTitle; }
            set { pageSubTitle = value; }
        }

        /// <summary>
        /// Высота ячейки хидера
        /// </summary>
        public int HeaderCellHeight
        {
            get { return headerCellHeight; }
            set { headerCellHeight = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PrintVersion"] != null && (bool)Session["PrintVersion"])
            {
                Visible = false;
            }

            pageTitlesAdded = false;
            headerCellHeight = 30;
            ultraWebGridDocumentExporter.DownloadName = DownloadFileName;

            Report report = new Report();
            defaultExportSection = report.AddSection();
            
            ultraWebGridDocumentExporter.TargetPaperOrientation = PageOrientation.Landscape;
            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PageTitlesAdding);
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        /// <param name="gridHeaderLayout">слой хидера грида</param>
        public void Export(GridHeaderLayout gridHeaderLayout)
        {
            Export(gridHeaderLayout, defaultExportSection);
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        /// <param name="gridHeaderLayout">слой хидера грида</param>
        /// <param name="section">секция</param>
        public void Export(GridHeaderLayout gridHeaderLayout, ISection section)
        {
            Export(gridHeaderLayout, String.Empty, section);
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        /// <param name="gridCaption">заголовок таблицы</param>
        /// <param name="gridHeaderLayout">слой хидера грида</param>
        public void Export(GridHeaderLayout gridHeaderLayout, string gridCaption)
        {
            Export(gridHeaderLayout, gridCaption, defaultExportSection);
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        /// <param name="gridHeaderLayout">слой хидера грида</param>
        /// <param name="gridCaption">заголовок таблицы</param>
        /// <param name="section">секция</param>
        public void Export(GridHeaderLayout gridHeaderLayout, string gridCaption, ISection section)
        {
            headerLayout = gridHeaderLayout;
            elementCaption = gridCaption;

            //ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(HeaderCellExporting);
            ultraWebGridDocumentExporter.InitializeRow += new EventHandler<DocumentExportInitializeRowEventArgs>(InitializeRow);
            ultraWebGridDocumentExporter.CellExporting += new EventHandler<CellExportingEventArgs>(GridCellExporting);

            ultraWebGridDocumentExporter.Export(headerLayout.Grid, section);

            //ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.HeaderCellExporting -= new EventHandler<MarginCellExportingEventArgs>(HeaderCellExporting);
            ultraWebGridDocumentExporter.InitializeRow -= new EventHandler<DocumentExportInitializeRowEventArgs>(InitializeRow);
            ultraWebGridDocumentExporter.CellExporting -= new EventHandler<CellExportingEventArgs>(GridCellExporting);
        }

        /// <summary>
        /// Экспорт диаграммы
        /// </summary>
        /// <param name="ultraChart">диаграмма</param>
        public void Export(UltraChart ultraChart)
        {
            Export(ultraChart, String.Empty, defaultExportSection);
        }

        /// <summary>
        /// Экспорт диаграммы
        /// </summary>
        /// <param name="ultraChart">диаграмма</param>
        /// <param name="section">секция</param>
        public void Export(UltraChart ultraChart, ISection section)
        {
            Export(ultraChart, String.Empty, section);
        }

        /// <summary>
        /// Экспорт диаграммы
        /// </summary>
        /// <param name="ultraChart">диаграмма</param>
        /// <param name="chartCaption">заголовок диаграммы</param>
        public void Export(UltraChart ultraChart, string chartCaption)
        {
            Export(ultraChart, chartCaption, defaultExportSection);
        }

        /// <summary>
        /// Экспорт диаграммы
        /// </summary>
        /// <param name="ultraChart">диаграмма</param>
        /// <param name="chartCaption">заголовок диаграммы</param>
        /// <param name="section">секция</param>
        public void Export(UltraChart ultraChart, string chartCaption, ISection section)
        {
            chart = ultraChart;
            elementCaption = chartCaption;

            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ChartAdding);
            
            ultraWebGridDocumentExporter.Export(new UltraWebGrid(), section);
            
            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(ChartAdding);
        }

        /// <summary>
        /// Экспорт карты
        /// </summary>
        /// <param name="mapControl">карта</param>
        public void Export(MapControl mapControl)
        {
            Export(mapControl, String.Empty, defaultExportSection);
        }

        /// <summary>
        /// Экспорт карты
        /// </summary>
        /// <param name="mapControl">карта</param>
        /// <param name="section">секция</param>
        public void Export(MapControl mapControl, ISection section)
        {
            Export(mapControl, String.Empty, section);
        }

        /// <summary>
        /// Экспорт карты
        /// </summary>
        /// <param name="mapControl">карта</param>
        /// <param name="mapCaption">заголовок карты</param>
        public void Export(MapControl mapControl, string mapCaption)
        {
            Export(mapControl, mapCaption, defaultExportSection);
        }

        /// <summary>
        /// Экспорт карты
        /// </summary>
        /// <param name="mapControl">диаграмма</param>
        /// <param name="mapCaption">заголовок карты</param>
        /// <param name="section">секция</param>
        public void Export(MapControl mapControl, string mapCaption, ISection section)
        {
            map = mapControl;
            elementCaption = mapCaption;

            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(MapAdding);

            ultraWebGridDocumentExporter.Export(new UltraWebGrid(), section);

            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(MapAdding);
        }

        /// <summary>
        /// Экспорт гейджа
        /// </summary>
        /// <param name="ultraChart">гейдж</param>
        /// <param name="section">секция</param>
        public void Export(UltraGauge ultraGauge, ISection section)
        {
            Export(ultraGauge, String.Empty, section);
        }

        /// <summary>
        /// Экспорт гейджа
        /// </summary>
        /// <param name="ultraChart">гейдж</param>
        /// <param name="chartCaption">заголовок гейджа</param>
        public void Export(UltraGauge ultraGauge, string gaugeCaption)
        {
            Export(ultraGauge, gaugeCaption, defaultExportSection);
        }

        /// <summary>
        /// Экспорт гейджа
        /// </summary>
        /// <param name="ultraChart">гейдж</param>
        /// <param name="chartCaption">заголовок гейджа</param>
        /// <param name="section">секция</param>
        public void Export(UltraGauge ultraGauge, string gaugeCaption, ISection section)
        {
            gauge = ultraGauge;
            elementCaption = gaugeCaption;

            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(GaugeAdding);

            ultraWebGridDocumentExporter.Export(new UltraWebGrid(), section);

            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(GaugeAdding);
        }

        /// <summary>
        /// Экспорт картинки
        /// </summary>
        /// <param name="img">картинка</param>
        /// <param name="section">секция</param>
        public void Export(Image img, ISection section)
        {
            Export(img, String.Empty, section);
        }

        /// <summary>
        /// Экспорт картинки
        /// </summary>
        /// <param name="img">картинка</param>
        /// <param name="imgCaption">заголовок картинки</param>
        public void Export(Image img, string imgCaption)
        {
            Export(img, imgCaption, defaultExportSection);
        }

        /// <summary>
        /// Экспорт картинки
        /// </summary>
        /// <param name="img">картинка</param>
        /// <param name="imgCaption">заголовок картинки</param>
        /// <param name="section">секция</param>
        public void Export(Image img, string imgCaption, ISection section)
        {
            image = img;
            elementCaption = imgCaption;

            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(ImageAdding);

            ultraWebGridDocumentExporter.Export(new UltraWebGrid(), section);

            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(ElementCaptionAdding);
            ultraWebGridDocumentExporter.BeginExport -= new EventHandler<DocumentExportEventArgs>(ImageAdding);
        }

        #region Экспорт грида

        /// <summary>
        /// Слияние ячеек хидера
        /// </summary>
        /// <param name="reportCell">ячейка таблицы PDF</param>
        private void MergeHeaderCells(ITableCell reportCell)
        {
            ITable table = reportCell.Parent.Parent;

            if (elementCaption != String.Empty)
            {
                table.AddRow().AddCell().AddQuickText(elementCaption);
            }

            ITableRow tableRow = table.AddRow();

            AddSelectorCell(reportCell, tableRow);

            int columnIndex = 0;
            foreach (GridHeaderCell cell in headerLayout.childCells)
            {
                if (cell.ChildCount == 0)
                {
                    AddTableCell(tableRow, cell.Caption, GetGridColumnWidth(columnIndex), cell.SpanY * HeaderCellHeight);
                    columnIndex++;
                }
                else
                {
                    MergeChildCells(tableRow, cell, columnIndex);
                    columnIndex += cell.ChildCount;
                }
            }
        }

        /// <summary>
        /// Слияние потомков ячейки хидера
        /// </summary>
        /// <param name="tableRow">строка таблицы PDF</param>
        /// <param name="headerCell">ячейка хидера</param>
        /// <param name="columnIndex">индекс колонки</param>
        /// <returns>ширина полученной области в point</returns>
        private float MergeChildCells(ITableRow tableRow, GridHeaderCell headerCell, int columnIndex)
        {
            ITableRow row = AddInnerTableRow(tableRow);
            ITableRow childRow = row.Parent.AddRow();

            int beginChildIndex = columnIndex;
            float childColumnWidth = 0;
            for (int i = 0; i < headerCell.ChildCount; i++)
            {
                GridHeaderCell cell = headerCell.childCells[i];
                if (cell.ChildCount == 0)
                {
                    AddTableCell(childRow, cell.Caption, GetGridColumnWidth(beginChildIndex), cell.SpanY * HeaderCellHeight);
                    childColumnWidth += GetGridColumnWidth(beginChildIndex);
                    beginChildIndex++;
                }
                else
                {
                    childColumnWidth += MergeChildCells(childRow, cell, beginChildIndex);
                    beginChildIndex += cell.ChildCount;
                }
            }

            ((ITableCell)row.Parent.Parent).Width = new FixedWidth(childColumnWidth);
            AddTableCell(row, headerCell.Caption, childColumnWidth, headerCell.SpanY * HeaderCellHeight);

            return childColumnWidth;
        }
        
        /// <summary>
        /// Получение ширины колонки грида
        /// </summary>
        /// <param name="columnIndex">индекс колонки грида</param>
        /// <returns>ширина в point</returns>
        private float GetGridColumnWidth(int columnIndex)
        {
            UltraGridColumn column = headerLayout.GetNonHiddenColumn(columnIndex);
            if (column != null)
            {
                return (float)(column.Width.Value / PDFCellWidthMultiplier);
            }
            return 0;
        }

        /// <summary>
        /// Добавление ячейки таблицы PDF
        /// </summary>
        /// <param name="row">строка</param>
        /// <param name="cellText">текст в ячейке</param>
        /// <param name="width">ширина</param>
        /// <param name="height">высота</param>
        /// <returns>ячейка таблицы PDF</returns>
        private ITableCell AddTableCell(ITableRow row, string cellText, float width, int height)
        {
            ITableCell tableCell = AddTableCell(row, cellText, width);
            tableCell.Height = new FixedHeight(height);
            return tableCell;
        }

        /// <summary>
        /// Добавление ячейки таблицы PDF
        /// </summary>
        /// <param name="row">строка</param>
        /// <param name="cellText">текст в ячейке</param>
        /// <param name="width">ширина</param>
        /// <returns>ячейка таблицы PDF</returns>
        private ITableCell AddTableCell(ITableRow row, string cellText, float width)
        {
            ITableCell tableCell = AddTableCell(row, cellText);
            tableCell.Width = new FixedWidth(width);
            return tableCell;
        }

        /// <summary>
        /// Добавление ячейки таблицы PDF
        /// </summary>
        /// <param name="row">строка</param>
        /// <param name="cellText">текст в ячейке</param>
        /// <returns>ячейка таблицы PDF</returns>
        private ITableCell AddTableCell(ITableRow row, string cellText)
        {
            ITableCell tableCell = row.AddCell();

            SetCellStyle(tableCell);
            tableCell.Height = new FixedHeight(headerCellHeight);
            IText text = tableCell.AddText();
            SetFontStyle(text);

			cellText = cellText.Replace("&nbsp;", " ");
			cellText = Regex.Replace(cellText, "<br[^>]*?>", Environment.NewLine);
			cellText = Regex.Replace(cellText, "<[^>]*?>", String.Empty);
			
            text.AddContent(cellText);

            return tableCell;
        }

        /// <summary>
        /// Добавление вложенной в таблицу строки
        /// </summary>
        /// <param name="row">родительская строка</param>
        /// <returns>вложенная строка</returns>
        private static ITableRow AddInnerTableRow(ITableRow row)
        {
            ITableCell tableCell = row.AddCell();
            ITable table = tableCell.AddTable();
            return table.AddRow();
        }

        /// <summary>
        /// Добавление ячейки селектора
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <param name="r">строка</param>
        public static void AddSelectorCell(ITableCell cell, ITableRow r)
        {
            ITableCell c = r.AddCell();
            c.Borders = cell.Borders;
            Color col = Color.FromArgb(178, 178, 178);

            SolidBrush sbr = new SolidBrush(col);
            SolidColorBrush br = new SolidColorBrush(sbr);
            Background b = new Background(br);
            c.Background = b;
            c.Width = new FixedWidth(16);
        }

        /// <summary>
        /// Добавление текста
        /// </summary>
        /// <param name="section">секция</param>
        /// <param name="text">текст</param>
        /// <param name="fontHeight">высота шрифта</param>
        /// <param name="bold">жирность шрифта</param>
        public static void AddTextContent(ISection section, string text, int fontHeight, bool bold)
        {
			if (!String.IsNullOrEmpty(text))
			{
				IText title = section.AddText();
				System.Drawing.Font font = new System.Drawing.Font("Verdana", fontHeight);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
				title.Style.Font.Bold = bold;

				Regex.Replace(text, "<[\\s\\S]*?>", String.Empty);
				text = text.Replace("&nbsp;", " ");
				text = Regex.Replace(text, "<br[^>]*?>", Environment.NewLine);
				text = Regex.Replace(text, "<[^>]*?>", String.Empty);

				title.AddContent(text);
			}
        }

        /// <summary>
        /// Установка стиля ячейки
        /// </summary>
        /// <param name="headerCell">ячейка</param>
        public void SetCellStyle(ITableCell headerCell)
        {
            headerCell.Alignment.Horizontal = Alignment.Center;
            headerCell.Alignment.Vertical = Alignment.Middle;
            headerCell.Borders = headerBorders;
            headerCell.Paddings.All = 2;
            headerCell.Background = headerBackground;
        }

        /// <summary>
        /// Установка стиля элемента текста
        /// </summary>
        /// <param name="t">элемент текста</param>
        public static void SetFontStyle(IText t)
        {
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            t.Style.Font = font;
            t.Style.Font.Bold = true;
            t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;
        }

        #endregion

        # region Перерисовка картинок
        private static Graphics GetGraphics(Color backColor, Bitmap bitmap, Image image)
        {
            SolidBrush brush = new SolidBrush(backColor);
            Graphics graphics = Graphics.FromImage(bitmap);
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
                Graphics graphics = Graphics.FromImage(bitmap);
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
            Image image = Image.FromStream(stream);

            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            bitmap.SetResolution(90, 90);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawImageUnscaled(image, 0, 0);
            return GetImg(bitmap);
        }

        public static Infragistics.Documents.Reports.Graphics.Image GetImage(string path)
        {
            string imagePath = HttpContext.Current.Server.MapPath(path);
            // Загружаем картинку
            Image image = Image.FromFile(imagePath);
            // Рисуем картинку с рамкой
            Bitmap bitmap = GetBitmap(Color.White, image);
            GetGraphics(Color.White, bitmap, image);
            // Создаем из нее картинку для ячейки.
            return GetImg(bitmap);
        }
        #endregion

        #region Обработчики

        private void HeaderCellExporting(object sender, MarginCellExportingEventArgs e)
        {
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;

            e.ReportCell.Height = new FixedHeight(0);
            
            e.ReportCell.Parent.Parent.Header.Repeat = true;

            if (e.Column.Index == 0)
            {
//                PDFCellWidthMultiplier = GetGridColumnWidth(0) / ((FixedWidth)e.ReportCell.Width).Value;
//                CRHelper.SaveToErrorLog(String.Format("Множитель: {0:N5}", PDFCellWidthMultiplier));
                MergeHeaderCells(e.ReportCell.Parent.Parent.Header.AddCell().AddTable().AddRow().AddCell());
            }
        }

        private static void InitializeRow(object sender, DocumentExportInitializeRowEventArgs e)
        {
            // правит рендеринг буквы 'ы'в ячейках грида.
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Font.Name = "Arial";
            }
        }

        private static void GridCellExporting(object sender, CellExportingEventArgs e)
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

        private void PageTitlesAdding(object sender, DocumentExportEventArgs e)
        {
            if (!pageTitlesAdded)
            {
                pageTitlesAdded = true;

                AddTextContent(e.Section, PageTitle, 16, true);
                AddTextContent(e.Section, PageSubTitle, 14, false);
            }

            if (e.Layout.Bands[0] != null)
            {
                e.Layout.Bands[0].HeaderStyle.Font.Name = "Arial";
            }
        }

        private void ElementCaptionAdding(object sender, DocumentExportEventArgs e)
        {
            if (elementCaption != String.Empty)
            {
                AddTextContent(e.Section, elementCaption, 14, false);
            }
        }

        private void ChartAdding(object sender, DocumentExportEventArgs e)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(chart);
            e.Section.AddImage(img);
        }

        private void MapAdding(object sender, DocumentExportEventArgs e)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(map);
            e.Section.AddImage(img);
        }

        private void GaugeAdding(object sender, DocumentExportEventArgs e)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromGauge(gauge);
            e.Section.AddImage(img);
        }

        private void ImageAdding(object sender, DocumentExportEventArgs e)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetInfragisticsImage(image);
            e.Section.AddImage(img);
        }

        #endregion
    }
}