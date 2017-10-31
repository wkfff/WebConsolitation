using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Server.Dashboards.Common;
using Image = System.Drawing.Image;
using Rectangle=System.Drawing.Rectangle;
using Infragistics.Documents.Excel;
using Krista.FM.Server.Dashboards.Components.Components;
using Infragistics.WebUI.UltraWebGauge;

namespace Krista.FM.Server.Dashboards.Components
{
    /// <summary>
    /// Экспортер отчета в Excel
    /// </summary>
    public partial class ReportExcelExporter : UserControl
    {
        #region Поля

        private int headerLevelCount;
        private int gridColumnCount;
        private int gridRowCount;
        private int startExportRow;
        private bool rowsAutoFitEnable;

        private bool headerTitleEnable;

        private string worksheetTitle;
        private string worksheetSubTitle;
        
        public const string ExelPercentFormat = "0.00%";
        public const string ExelNumericFormat = "#,##0.00;[Red]-#,##0.00";
        public const int ExcelCellSizeMultiplier = 37;
        
		private string downloadFileName = "report.xls";
    	public string DownloadFileName
    	{
    		set { downloadFileName = value; } 
			get { return downloadFileName; }
    	}

        private Font headerCellFont;
        private Font titleFont;
        private Font subTitleFont;
        private HorizontalCellAlignment titleAlignment;
        private int titleStartRow;

        private int headerCellHeight;
        private GridHeaderLayout headerLayout;
        private UltraChart chart;
        private MapControl map;
        private UltraGauge gauge;
        private Image image;
        private string elementCaption;
        
        private double gridColumnWidthScale;

        private int sheetColumnCount;

        Graphics graphics = Graphics.FromImage(new Bitmap(1000, 500));

        #endregion

        #region Свойства

        /// <summary>
        /// Экспортер в эксель
        /// </summary>
        public UltraWebGridExcelExporter ExcelExporter
        {
            get { return ultraWebGridExcelExporter; }
        }

        /// <summary>
        /// Кнопка экспорта
        /// </summary>
        public LinkButton ExcelExportButton
        {
            get { return excelExportButton; }
        }

        /// <summary>
        /// Заголовок листа экспорта
        /// </summary>
        public string WorksheetTitle
        {
            get { return worksheetTitle; }
            set { worksheetTitle = value; }
        }

        /// <summary>
        /// Подзаголовок листа экспорта
        /// </summary>
        public string WorksheetSubTitle
        {
            get { return worksheetSubTitle; }
            set { worksheetSubTitle = value; }
        }

        /// <summary>
        /// Высота ячейки хидера грида
        /// </summary>
        public int HeaderCellHeight
        {
            get { return headerCellHeight; }
            set { headerCellHeight = value; }
        }

        /// <summary>
        /// Автоподбор высоты ячеек
        /// </summary>
        public bool RowsAutoFitEnable
        {
            get { return rowsAutoFitEnable; }
            set { rowsAutoFitEnable = value; }
        }

        /// <summary>
        /// Шрифт содержимого ячеек грида
        /// </summary>
        public Font HeaderCellFont
        {
            get { return headerCellFont; }
            set { headerCellFont = value; }
        }
        
        /// <summary>
        /// Шрифт заголовка
        /// </summary>
        public Font TitleFont
        {
            get { return titleFont; }
            set { titleFont = value; }
        }

        /// <summary>
        /// Шрифт подзаголовка
        /// </summary>
        public Font SubTitleFont
        {
            get { return subTitleFont; }
            set { subTitleFont = value; }
        }
        
        /// <summary>
        /// Выравнивание заголовка и позаголовка
        /// </summary>
        public HorizontalCellAlignment TitleAlignment
        {
            get { return titleAlignment; }
            set { titleAlignment = value; }
        }

        /// <summary>
        /// Начальная строка заголовка и позаголовка
        /// </summary>
        public int TitleStartRow
        {
            get { return titleStartRow; }
            set { titleStartRow = value; }
        }

        /// <summary>
        /// Показывать хинты к ячейкам хидера грида
        /// </summary>
        public bool HeaderTitleEnable
        {
            get { return headerTitleEnable; }
            set { headerTitleEnable = value; }
        }

        /// <summary>
        /// Множитель ширины колонок грида
        /// </summary>
        public double GridColumnWidthScale
        {
            get { return gridColumnWidthScale; }
            set { gridColumnWidthScale = value; }
        }

        /// <summary>
        /// Число колонок на странице
        /// </summary>
        public int SheetColumnCount
        {
            get { return sheetColumnCount; }
            set { sheetColumnCount = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PrintVersion"] != null && (bool) Session["PrintVersion"])
            {
                Visible = false;
            }

            headerCellHeight = 30;
            rowsAutoFitEnable = true;

            worksheetTitle = String.Empty;
            worksheetSubTitle = String.Empty;

            headerCellFont = new Font("Verdana", 10, FontStyle.Bold);
            titleFont = new Font("Verdana", 10, FontStyle.Bold);
            subTitleFont = new Font("Verdana", 10);
            titleAlignment = HorizontalCellAlignment.Left;
            titleStartRow = 0;
            headerTitleEnable = false;

            sheetColumnCount = -1;
            gridColumnWidthScale = 1;

            ultraWebGridExcelExporter.DownloadName = DownloadFileName;
            ultraWebGridExcelExporter.HeaderRowExporting += new HeaderRowExportingEventHandler(ultraWebGridExcelExporter_HeaderRowExporting);
            ultraWebGridExcelExporter.BeginExport += new BeginExportEventHandler(WorksheetTitlesAdding);
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        /// <param name="gridHeaderLayout">слой хидера грида</param>
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(GridHeaderLayout gridHeaderLayout, int startRow)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets.Add("sheet1");

            Export(gridHeaderLayout, sheet, startRow);
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        /// <param name="gridHeaderLayout">слой хидера грида</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(GridHeaderLayout gridHeaderLayout, Worksheet sheet, int startRow)
        {
            Export(gridHeaderLayout, String.Empty, sheet, startRow);
        }

        /// <summary>
        /// Экспорт грида
        /// </summary>
        /// <param name="gridHeaderLayout">слой хидера грида</param>
        /// <param name="gridCaption">заголовок таблицы</param>        
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(GridHeaderLayout gridHeaderLayout, string gridCaption, Worksheet sheet, int startRow)
        {
            headerLayout = gridHeaderLayout;

            headerLevelCount = headerLayout.HeaderHeight;
            gridColumnCount = headerLayout.Grid.Columns.Count;
            gridRowCount = headerLayout.Grid.Rows.Count;
            elementCaption = gridCaption;

            if (sheetColumnCount == -1)
            {
                sheetColumnCount = gridColumnCount;
            }

            startExportRow = startRow;
            ultraWebGridExcelExporter.ExcelStartRow = startExportRow + headerLevelCount + 1;

            ultraWebGridExcelExporter.BeginExport += new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.BeginExport += new BeginExportEventHandler(HeaderCellsMerging);
            ultraWebGridExcelExporter.EndExport += new EndExportEventHandler(GridColumnSettings);

            ultraWebGridExcelExporter.Export(headerLayout.Grid, sheet);

            ultraWebGridExcelExporter.BeginExport -= new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.BeginExport -= new BeginExportEventHandler(HeaderCellsMerging);
            ultraWebGridExcelExporter.EndExport -= new EndExportEventHandler(GridColumnSettings);
        }

        /// <summary>
        /// Экспорт диаграммы
        /// </summary>
        /// <param name="ultraChart">диаграмма</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(UltraChart ultraChart, Worksheet sheet, int startRow)
        {
            Export(ultraChart, String.Empty, sheet, startRow);
        }

        /// <summary>
        /// Экспорт диаграммы
        /// </summary>
        /// <param name="ultraChart">диаграмма</param>
        /// <param name="chartCaption">заголовок диаграммы</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(UltraChart ultraChart, string chartCaption, int startRow)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets.Add("sheet1");

            Export(ultraChart, chartCaption, sheet, startRow);
        }

        /// <summary>
        /// Экспорт диаграммы
        /// </summary>
        /// <param name="ultraChart">диаграмма</param>
        /// <param name="chartCaption">заголовок диаграммы</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(UltraChart ultraChart, string chartCaption, Worksheet sheet, int startRow)
        {
            startExportRow = startRow;
            ultraWebGridExcelExporter.ExcelStartRow = startExportRow + headerLevelCount + 1;

            chart = ultraChart;
            elementCaption = chartCaption;

            ultraWebGridExcelExporter.BeginExport += new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport += new EndExportEventHandler(ChartAdding);

            ultraWebGridExcelExporter.Export(emptyExportGrid, sheet);

            ultraWebGridExcelExporter.BeginExport -= new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport -= new EndExportEventHandler(ChartAdding);
        }

        /// <summary>
        /// Экспорт гейджа
        /// </summary>
        /// <param name="ultraGauge">гейдж</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(UltraGauge ultraGauge, Worksheet sheet, int startRow)
        {
            Export(ultraGauge, string.Empty, sheet, startRow);
        }
        
        /// <summary>
        /// Экспорт гейджа
        /// </summary>
        /// <param name="ultraGauge">гейдж</param>
        /// <param name="gaugeCaption">заголовок гейджа</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(UltraGauge ultraGauge, string gaugeCaption, Worksheet sheet, int startRow)
        {
            startExportRow = startRow;
            ultraWebGridExcelExporter.ExcelStartRow = startExportRow + headerLevelCount + 1;

            gauge = ultraGauge;
            elementCaption = gaugeCaption;

            ultraWebGridExcelExporter.BeginExport += new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport += new EndExportEventHandler(GaugeAdding);

            ultraWebGridExcelExporter.Export(emptyExportGrid, sheet);

            ultraWebGridExcelExporter.BeginExport -= new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport -= new EndExportEventHandler(GaugeAdding);
        }

        /// <summary>
        /// Экспорт картинки
        /// </summary>
        /// <param name="img">картинка</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(Image img, Worksheet sheet, int startRow)
        {
            Export(img, String.Empty, sheet, startRow);
        }

        /// <summary>
        /// Экспорт картинки
        /// </summary>
        /// <param name="img">картинка</param>
        /// <param name="imageCaption">заголовок картинки</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(Image img, string imageCaption, Worksheet sheet, int startRow)
        {
            startExportRow = startRow;
            ultraWebGridExcelExporter.ExcelStartRow = startExportRow + headerLevelCount + 1;

            image = img;
            elementCaption = imageCaption;

            ultraWebGridExcelExporter.BeginExport += new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport += new EndExportEventHandler(ImageAdding);

            ultraWebGridExcelExporter.Export(emptyExportGrid, sheet);

            ultraWebGridExcelExporter.BeginExport -= new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport -= new EndExportEventHandler(ImageAdding);
        }

        /// <summary>
        /// Экспорт карты
        /// </summary>
        /// <param name="mapControl">карта</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(MapControl mapControl, Worksheet sheet, int startRow)
        {
            Export(mapControl, String.Empty, sheet, startRow);
        }

        /// <summary>
        /// Экспорт карты
        /// </summary>
        /// <param name="mapControl">карта</param>
        /// <param name="mapCaption">заголовок карты</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(MapControl mapControl, string mapCaption, int startRow)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets.Add("sheet1");

            Export(mapControl, mapCaption, sheet, startRow);
        }

        /// <summary>
        /// Экспорт карты
        /// </summary>
        /// <param name="mapControl">карта</param>
        /// <param name="mapCaption">заголовок карты</param>
        /// <param name="sheet">лист</param>        
        /// <param name="startRow">стартовый номер строки</param>
        public void Export(MapControl mapControl, string mapCaption, Worksheet sheet, int startRow)
        {
            startExportRow = startRow;
            ultraWebGridExcelExporter.ExcelStartRow = startExportRow + headerLevelCount + 1;

            map = mapControl;
            elementCaption = mapCaption;

            ultraWebGridExcelExporter.BeginExport += new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport += new EndExportEventHandler(MapAdding);

            ultraWebGridExcelExporter.Export(emptyExportGrid, sheet);

            ultraWebGridExcelExporter.BeginExport -= new BeginExportEventHandler(ElementCaptionAdding);
            ultraWebGridExcelExporter.EndExport -= new EndExportEventHandler(MapAdding);
        }

        #region Экспорт грида

        /// <summary>
        /// Слияние ячеек хидера
        /// </summary>
        /// <param name="worksheet">лист</param>
        private void MergeHeaderCells(Worksheet worksheet)
        {
            for (int level = 0; level < headerLevelCount; level++)
            {
                worksheet.Rows[startExportRow + level].Height = headerCellHeight * ExcelCellSizeMultiplier;
            }

            int columnIndex = 0;
            foreach (GridHeaderCell cell in headerLayout.childCells)
            {
                if (cell.ChildCount == 0)
                {
                    AddHeaderMergeCell(worksheet, cell.Caption, cell.Hint, startExportRow, columnIndex, startExportRow + headerLevelCount - 1, columnIndex);
                    columnIndex++;
                }
                else
                {
                    columnIndex += MergeChildCells(worksheet, cell, columnIndex, 0);
                }
            }
        }

        /// <summary>
        /// Слияние потомков ячейки хидера
        /// </summary>
        /// <param name="worksheet">лист</param>
        /// <param name="headerCell">ячейка хидера</param>
        /// <param name="columnIndex">индекс колонки</param>
        /// <param name="level">этаж хидера</param>
        private int MergeChildCells(Worksheet worksheet, GridHeaderCell headerCell, int columnIndex, int level)
        {
            int beginChildIndex = columnIndex;
            int childCount = 0;
            
            int currentRow = startExportRow + level;

            for (int i = 0; i < headerCell.ChildCount; i++)
            {
                GridHeaderCell cell = headerCell.childCells[i];
                if (cell.ChildCount == 0)
                {
                    AddHeaderMergeCell(worksheet, cell.Caption, cell.Hint, currentRow + headerCell.SpanY, beginChildIndex, startExportRow + headerLevelCount - 1, beginChildIndex);
                    beginChildIndex++;
                    childCount++;
                }
                else
                {
                    int count = MergeChildCells(worksheet, cell, beginChildIndex, level + headerCell.SpanY);
                    beginChildIndex += count;
                    childCount += count;
                }
            }

            AddHeaderMergeCell(worksheet, headerCell.Caption, headerCell.Hint, currentRow, columnIndex, currentRow + headerCell.SpanY - 1, columnIndex + childCount - 1);

            return childCount;
        }

        /// <summary>
        /// Настройка параметров колонок
        /// </summary>
        /// <param name="worksheet">лист</param>
        private void SetColumnAppearance(Worksheet worksheet)
        {
            for (int i = 0; i < gridColumnCount; i++)
            {
                UltraGridColumn column = headerLayout.GetNonHiddenColumn(i);
                if (column != null)
                {
                    worksheet.Columns[i].Width = (int)(column.Width.Value * ExcelCellSizeMultiplier * gridColumnWidthScale);
                    
                    string format = GetExcelFormatString(column.Format);
                    if (format != String.Empty)
                    {
                        worksheet.Columns[i].CellFormat.FormatString = format;
                    }

                    //worksheet.Columns[i].CellFormat.WrapText = column.CellStyle.Wrap ? ExcelDefaultableBoolean.True : ExcelDefaultableBoolean.False;
                    // почему-то на IG 2011.1 перестало считываться свойство e.Layout.HeaderStyleDefault.Wrap
                    worksheet.Columns[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                }
            }
        }

        /// <summary>
        /// Автоподбор выcоты строк по содержимому ячеек
        /// </summary>
        /// <param name="worksheet">лист</param>
        private void SetRowsAutoFit(Worksheet worksheet)
        {
            int startRow = startExportRow;
            for (int i = startRow; i < startRow + headerLevelCount + gridRowCount; i++)
            {
                WorksheetRow row = worksheet.Rows[i];
                double maxHeight = 20;
                foreach (WorksheetCell cell in row.Cells)
                {
                    decimal value;
                    if (cell.Value != null && cell.Value.ToString() != String.Empty && !Decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        double currentWidth;
                        if (cell.AssociatedMergedCellsRegion != null)
                        {
                            currentWidth = GetMergeCellsRegionWidth(cell.AssociatedMergedCellsRegion);
                        }
                        else
                        {
                            UltraGridColumn column = headerLayout.GetNonHiddenColumn(cell.ColumnIndex);
                            currentWidth = column.Width.Value;
                        }

                        int currentHeight = GetStringHeight(cell.Value.ToString().TrimEnd(' '), GetSystemFont(cell.CellFormat.Font), (int)(currentWidth * gridColumnWidthScale));
                        maxHeight = Math.Max(currentHeight, maxHeight);
                    }
                }
                row.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

                //CRHelper.SaveToErrorLog(String.Format("{0}, {1}, {2}", row.Cells[0].Value, (int)headerLayout.Grid.Columns[0].Width.Value, maxHeight));

                row.Height = (int)maxHeight * 20;
            }
        }

        /// <summary>
        /// Получение ширины региона слитых ячеек
        /// </summary>
        /// <param name="mergedCellsRegion">регион слитых ячеек</param>
        /// <returns>ширина региона</returns>
        private int GetMergeCellsRegionWidth(WorksheetMergedCellsRegion mergedCellsRegion)
        {
            double regionWidth = 0;
            for (int columnIndex = mergedCellsRegion.FirstColumn; columnIndex <= mergedCellsRegion.LastColumn; columnIndex++)
            {
                UltraGridColumn column = headerLayout.GetNonHiddenColumn(columnIndex);
                if (column != null)
                {
                    regionWidth += column.Width.Value;
                }
            }

            return (int)regionWidth;
        }

        /// <summary>
        /// Получение формата экселя по формату колонки
        /// </summary>
        /// <param name="gridFormat">формат колонки</param>
        /// <returns>формат экселя</returns>
        private static string GetExcelFormatString(string gridFormat)
        {
            if (!String.IsNullOrEmpty(gridFormat))
            {
                if (gridFormat.Length > 1)
                {
                    string excelFormat = "#,##0";

                    char formatType = gridFormat[0];

                    if (formatType == '#' || formatType == '0')
                    {
                        excelFormat = gridFormat;
                    }
                    else
                    {
                        int precision = Convert.ToInt32(gridFormat.Substring(1));

                        for (int i = 0; i < precision; i++)
                        {
                            if (i == 0)
                            {
                                excelFormat += ".";
                            }
                            excelFormat += "0";
                        }

                        if (formatType == 'P')
                        {
                            excelFormat += "%";
                        }
                    }

                    return excelFormat;
                }
            }
            return String.Empty;
        }

        private void UnSelectGridRows()
        {
            headerLayout.Grid.DisplayLayout.SelectedRows.Clear();
            headerLayout.Grid.DisplayLayout.ActiveRow = null;
            foreach (UltraGridRow row in headerLayout.Grid.Rows)
            {
                row.Selected = false;
                row.Activated = false;
            }
        }

        /// <summary>
        /// Добавление слитой ячейки хидера
        /// </summary>
        /// <param name="worksheet">лист</param>
        /// <param name="caption">заголовок</param>
        /// <param name="startRow">начальный индекс слияния по строкам</param>
        /// <param name="startColumn">начальный индекс слияния по колонкам</param>
        /// <param name="endRow">конечный индекс слияния по строкам</param>
        /// <param name="endColumn">конечный индекс слияния по колонкам</param>
        private WorksheetMergedCellsRegion AddHeaderMergeCell(Worksheet worksheet, string caption, int startRow, int startColumn, int endRow, int endColumn)
        {
            WorksheetMergedCellsRegion mergedRegion = AddMergeCellsText(worksheet, caption, startRow, startColumn, endRow, endColumn, headerCellFont, HorizontalCellAlignment.Center);
            mergedRegion.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            mergedRegion.CellFormat.FillPatternForegroundColor = Color.LightGray;
            mergedRegion.CellFormat.FillPatternBackgroundColor = Color.LightGray;
            mergedRegion.CellFormat.TopBorderColor = Color.Gray;
            mergedRegion.CellFormat.BottomBorderColor = Color.Gray;
            mergedRegion.CellFormat.LeftBorderColor = Color.Gray;
            mergedRegion.CellFormat.RightBorderColor = Color.Gray;
            mergedRegion.CellFormat.WrapText = ExcelDefaultableBoolean.True;
            return mergedRegion;
        }

        /// <summary>
        /// Добавление слитой ячейки хидера
        /// </summary>
        /// <param name="worksheet">лист</param>
        /// <param name="caption">заголовок</param>
        /// <param name="comment">комментарий</param>
        /// <param name="startRow">начальный индекс слияния по строкам</param>
        /// <param name="startColumn">начальный индекс слияния по колонкам</param>
        /// <param name="endRow">конечный индекс слияния по строкам</param>
        /// <param name="endColumn">конечный индекс слияния по колонкам</param>
        private WorksheetMergedCellsRegion AddHeaderMergeCell(Worksheet worksheet, string caption, string comment, int startRow, int startColumn, int endRow, int endColumn)
        {
            WorksheetMergedCellsRegion mergedRegion = AddHeaderMergeCell(worksheet, caption, startRow, startColumn, endRow, endColumn);

            if (HeaderTitleEnable && comment != String.Empty)
            {
                WorksheetCellComment cellComment = new WorksheetCellComment();
                cellComment.Text = new FormattedString(comment);
                mergedRegion.Comment = cellComment;
            }

            return mergedRegion;
        }

        #endregion

        #region Экспорт ячейки с текстом

        /// <summary>
        /// Добавление слитой ячейки с текстом
        /// </summary>
        /// <param name="worksheet">лист</param>
        /// <param name="text">текст</param>
        /// <param name="startRow">начальный индекс слияния по строкам</param>
        /// <param name="startColumn">начальный индекс слияния по колонкам</param>
        /// <param name="endRow">конечный индекс слияния по строкам</param>
        /// <param name="endColumn">конечный индекс слияния по колонкам</param>
        /// <returns>слитая ячейка</returns>
        private static WorksheetMergedCellsRegion AddMergeCellsText(Worksheet worksheet, string text, int startRow, int startColumn, int endRow, int endColumn)
        {
            WorksheetMergedCellsRegion mergedRegion = worksheet.MergedCellsRegions.Add(startRow, startColumn, endRow, endColumn);
            text = text.Replace("&nbsp;", " ");
            text = Regex.Replace(text, "<br[^>]*?>", Environment.NewLine);
            text = Regex.Replace(text, "<[^>]*?>", String.Empty);
            mergedRegion.Value = text;
            return mergedRegion;
        }

        /// <summary>
        /// Добавление слитой ячейки с текстом
        /// </summary>
        /// <param name="worksheet">лист</param>
        /// <param name="text">текст</param>
        /// <param name="startRow">начальный индекс слияния по строкам</param>
        /// <param name="startColumn">начальный индекс слияния по колонкам</param>
        /// <param name="endRow">конечный индекс слияния по строкам</param>
        /// <param name="endColumn">конечный индекс слияния по колонкам</param>
        /// <param name="font">шрифт</param>
        /// <param name="cellAlignment">горизонтальное выравнивание</param>
        /// <returns>слитая ячейка</returns>
        private static WorksheetMergedCellsRegion AddMergeCellsText(Worksheet worksheet, string text, int startRow, int startColumn, int endRow, int endColumn, Font font, HorizontalCellAlignment cellAlignment)
        {
            WorksheetMergedCellsRegion mergedRegion = AddMergeCellsText(worksheet, text, startRow, startColumn, endRow, endColumn);
            mergedRegion.CellFormat.Alignment = cellAlignment;
            mergedRegion.CellFormat.Font.Name = font.Name;
            mergedRegion.CellFormat.Font.Height = 20 * (int)font.Size;
            mergedRegion.CellFormat.Font.Bold = font.Bold ? ExcelDefaultableBoolean.True : ExcelDefaultableBoolean.False;
            mergedRegion.CellFormat.Font.Italic = font.Italic ? ExcelDefaultableBoolean.True : ExcelDefaultableBoolean.False;
            mergedRegion.CellFormat.Font.Strikeout = font.Strikeout ? ExcelDefaultableBoolean.True : ExcelDefaultableBoolean.False;
            return mergedRegion;
        }

        /// <summary>
        /// Добавление секции подтверждения
        /// </summary>
        /// <param name="sheet">лист</param>
        /// <param name="rowIndex">начальная строка</param>
        /// <param name="cellIndex">начальная колонка</param>
        /// <param name="spanCells">число сливаемых ячеек для области</param>
        /// <param name="action">действие</param>
        /// <param name="person">субъект</param>
        /// <param name="date">дата</param>
        public void SetApprovedSection(Worksheet sheet, int rowIndex, int cellIndex, int spanCells, string action, string person, string date)
        {
            AddMergeCellsText(sheet, action, rowIndex, cellIndex, rowIndex, cellIndex + spanCells - 1, TitleFont, HorizontalCellAlignment.Center);
            sheet.Rows[rowIndex].Cells[cellIndex].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            sheet.Rows[rowIndex].Cells[cellIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            sheet.Rows[rowIndex].Cells[cellIndex].CellFormat.Font.Height = 280;

            AddMergeCellsText(sheet, person, rowIndex + 1, cellIndex, rowIndex + 1, cellIndex + spanCells - 1, SubTitleFont, HorizontalCellAlignment.Center);
            sheet.Rows[rowIndex + 1].Cells[cellIndex].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet.Rows[rowIndex + 1].Cells[cellIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
            sheet.Rows[rowIndex + 1].Cells[cellIndex].CellFormat.Font.Height = 220;

            AddMergeCellsText(sheet, date, rowIndex + 2, cellIndex, rowIndex + 2, cellIndex + spanCells - 1, TitleFont, HorizontalCellAlignment.Center);
            sheet.Rows[rowIndex + 2].Cells[cellIndex].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet.Rows[rowIndex + 2].Cells[cellIndex].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            sheet.Rows[rowIndex + 2].Cells[cellIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            sheet.Rows[rowIndex + 2].Cells[cellIndex].CellFormat.Font.Height = 240;
        }

        #endregion

        #region Расчет высоты ячейки по содержимому

        /// <summary>
        /// Получение высоты текста в области заданной ширины
        /// </summary>
        /// <param name="measuredString">текст</param>
        /// <param name="font">шрифт текста</param>
        /// <param name="rectangleWidth">ширина области</param>
        /// <returns>высота</returns>
        private int GetStringHeight(string measuredString, Font font, int rectangleWidth)
        {
            SizeF sizeF = graphics.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);

            return rect.Height;
        }

        /// <summary>
        /// Получение системного шрифта из шрифта экселя
        /// </summary>
        /// <param name="excelFont">шрифт экселя</param>
        /// <returns>системный шрифт</returns>
        private static Font GetSystemFont(IWorkbookFont excelFont)
        {
            Font font = null;
            if (excelFont != null && excelFont.Height != -1)
            {
                FontStyle fontStyle = FontStyle.Regular;
                if (excelFont.Bold == ExcelDefaultableBoolean.True)
                {
                    fontStyle |= FontStyle.Bold;
                }
                if (excelFont.Italic == ExcelDefaultableBoolean.True)
                {
                    fontStyle |= FontStyle.Italic;
                }
                if (excelFont.Strikeout == ExcelDefaultableBoolean.True)
                {
                    fontStyle |= FontStyle.Strikeout;
                }
                // для times new roman немного уменьшим шрифт, чтобы корректней определялась высота
                float fontHeight = excelFont.Name == "Times New Roman" ? excelFont.Height / 20 - 1 : excelFont.Height / 20;
                font = new Font(excelFont.Name, fontHeight, fontStyle);
            }
            return font;
        }

        #endregion

        #region Экспорт диаграмм и карт

        private static Image GetExcelImage(UltraChart chart)
        {
            MemoryStream imageStream = new MemoryStream();
            chart.SaveTo(imageStream, ImageFormat.Png);
            return Image.FromStream(imageStream);
        }

        private static Image GetExcelImage(UltraGauge gauge)
        {
            MemoryStream imageStream = new MemoryStream();
            gauge.SaveTo(imageStream, GaugeImageType.Png, new Size((int)gauge.Width.Value, (int)gauge.Height.Value));
            return Image.FromStream(imageStream);
        }

        private static Image GetExcelImage(MapControl map)
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

        public static void GaugeExcelExport(WorksheetCell cell, UltraGauge gauge)
        {
            Image image = GetExcelImage(gauge);
            ImageExcelExport(cell, image, (int)gauge.Width.Value, (int)gauge.Height.Value);
        }

        public static void MapExcelExport(WorksheetCell cell, MapControl map)
        {
            Image image = GetExcelImage(map);
            ImageExcelExport(cell, image, (int)map.Width.Value, (int)map.Height.Value);
        }

        public static void MapExcelExport(WorksheetCell cell, MapControl map, double scale)
        {
            Image image = GetExcelImage(map);
            ImageExcelExport(cell, image, (int)(map.Width.Value * scale), (int)(map.Height.Value * scale));
        }

        private static void ImageExcelExport(WorksheetCell cell, Image image, int width, int height)
        {
            WorksheetImage excelImage = new WorksheetImage(image);
            excelImage.TopLeftCornerCell = cell;
            excelImage.BottomRightCornerCell = cell;
            cell.Worksheet.Shapes.Add(excelImage);
            excelImage.SetBoundsInTwips(cell.Worksheet,
                                        new Rectangle(
                                            excelImage.TopLeftCornerCell.GetBoundsInTwips().Left,
                                            excelImage.TopLeftCornerCell.GetBoundsInTwips().Top,
                                            20 * width, 20 * height),
                                        true);
        }

        #endregion

        #region Обработчики

        private static void ultraWebGridExcelExporter_HeaderRowExporting(object sender, HeaderRowExportingEventArgs e)
        {
            e.Cancel = true;
        }

        private void WorksheetTitlesAdding(object sender, BeginExportEventArgs e)
        {
            if (WorksheetTitle != String.Empty)
            {
                AddMergeCellsText(e.CurrentWorksheet, WorksheetTitle, titleStartRow, 0, titleStartRow, sheetColumnCount - 1, TitleFont, TitleAlignment);
            }
            if (WorksheetSubTitle != String.Empty)
            {
                AddMergeCellsText(e.CurrentWorksheet, WorksheetSubTitle, titleStartRow + 1, 0, titleStartRow + 1, sheetColumnCount - 1, SubTitleFont, TitleAlignment);
            }
        }

        private void HeaderCellsMerging(object sender, BeginExportEventArgs e)
        {
            MergeHeaderCells(e.CurrentWorksheet);
        }

        private void GridColumnSettings(object sender, EndExportEventArgs e)
        {
            //UnSelectGridRows();
            if (rowsAutoFitEnable)
            {
                SetRowsAutoFit(e.CurrentWorksheet);
            }
            SetColumnAppearance(e.CurrentWorksheet);
        }

        private void ElementCaptionAdding(object sender, BeginExportEventArgs e)
        {
            if (elementCaption != String.Empty)
            {
                AddMergeCellsText(e.CurrentWorksheet, elementCaption, startExportRow - 1, 0, startExportRow - 1, sheetColumnCount - 1, subTitleFont, HorizontalCellAlignment.Left);
            }
        }

        private void ChartAdding(object sender, EndExportEventArgs e)
        {
            int startRow = startExportRow - 1;
            if (elementCaption != String.Empty)
            {
                startRow++;
            }

            ChartExcelExport(e.CurrentWorksheet.Rows[startRow].Cells[0], chart);
        }

        private void GaugeAdding(object sender, EndExportEventArgs e)
        {
            int startRow = startExportRow - 1;
            if (elementCaption != String.Empty)
            {
                startRow++;
            }

            GaugeExcelExport(e.CurrentWorksheet.Rows[startRow].Cells[0], gauge);
        }

        private void ImageAdding(object sender, EndExportEventArgs e)
        {
            int startRow = startExportRow - 1;
            if (elementCaption != String.Empty)
            {
                startRow++;
            }

            ImageExcelExport(e.CurrentWorksheet.Rows[startRow].Cells[0], image, image.Width, image.Height);
        }

        private void MapAdding(object sender, EndExportEventArgs e)
        {
            int startRow = startExportRow - 1;
            if (elementCaption != String.Empty)
            {
                startRow++;
            }

            MapExcelExport(e.CurrentWorksheet.Rows[startRow].Cells[0], map);
        }

        #endregion
    }
}
