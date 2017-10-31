using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0004_Yar
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2009;
        private int endYear = 2011;

        private double avgEvaluation;
        private static MemberAttributesDigest grbsDigest;
        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный индикатор
        private CustomParam selectedIndicator;
        // выбранный период
        private CustomParam selectedYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.60);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.Data.ZeroAligned = true;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Extent = 20;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X2.Visible = true;
            UltraChart.Axis.X2.Labels.Visible = false;
            UltraChart.Axis.X2.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X2.LineColor = Color.Transparent;
            UltraChart.Axis.X2.Extent = 10;

            UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 8;
            UltraChart.Legend.FormatString = "Среднее";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Количество ГРБС";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleBottom.Margins.Left = UltraChart.Axis.Y.Extent;
            UltraChart.TitleBottom.Text = "Оценки";
            UltraChart.TitleBottom.Font = new Font("Verdana", 8);

            //CRHelper.FillCustomColorModel(UltraChart, 10, true);

            UltraChart.Legend.MoreIndicatorText = " ";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LimeGreen;
            Color color2 = Color.DodgerBlue;
            Color color3 = Color.Gold;
            Color color4 = Color.Red;
            Color color5 = Color.Orange;

            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
            appearance.ChartTextFont = new Font("Verdana", 9, FontStyle.Bold);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "Оценка, равная <SERIES_LABEL>, наблюдается у <DATA_VALUE:N0> ГРБС\n<ITEM_LABEL>";

            #endregion

            #region Инициализация параметров запроса
            
            selectedIndicator = UserParams.CustomParam("selected_indicator");
            selectedYear = UserParams.CustomParam("selected_year");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;бальной&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_Yar/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Сводная&nbsp;оценка&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0002_Yar/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Рейтинг&nbsp;ГРБС";
            CrossLink3.NavigateUrl = "~/reports/FO_0042_0003_Yar/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartCaption);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0004_Yar_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
 
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                hiddenIndicatorLabel.Text = "[Показатели__Оценка качества ФМ_Сопоставимый].[Показатели__Оценка качества ФМ_Сопоставимый].[Все показатели].[Оценка механизмов планирования расходов бюджета].[Охват бюджетных ассигнований показателями непосредственных результатов в обоснованиях бюджетных ассигнований на очередной финансовый год и плановый период]";
            }

            Page.Title = String.Format("Анализ оценки качества финансового менеджмента, осуществляемого главными распорядителями средств областного бюджета Ярославской области в разрезе показателей");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = String.Format("по итогам {0} года", ComboYear.SelectedValue);

            selectedYear.Value = ComboYear.SelectedValue;

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                string patternValue = selectedIndicator.Value;
                int defaultRowIndex = 1;

                if (UltraWebGrid.Columns.Count > 0 && UltraWebGrid.Rows.Count > 0)
                {
                    // ищем строку
                    UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                    // выделяем строку
                    ActiveGridRow(row);
                }
            }

           //UltraChart.DataBind();
        }

        #region Обработчики грида
        
        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string indicatorName = row.Cells[1].Text;

            avgEvaluation = Double.MinValue;
            UltraChart.Legend.Visible = false;
            if (row.Cells[2].Value != null && row.Cells[2].Value.ToString() != String.Empty)
            {
                avgEvaluation = Convert.ToDouble(row.Cells[2].Value);
                UltraChart.Legend.Visible = true;
                UltraChart.Legend.FormatString = String.Format("Среднее значение оценки: {0:N2}", avgEvaluation);
            }

            string code = String.Empty;
            if (row.Cells[0].Value != null)
            {
                code = String.Format("{0} ", row.Cells[0].Value);
            }

            hiddenIndicatorLabel.Text = row.Cells[row.Cells.Count - 1].Text;
            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            chartCaption.Text = String.Format("Показатель «{1}{0}»", indicatorName, code);

            bool emptyRow = false;
            int emptyRowColumn = row.Cells.Count - 2;
            if (row.Cells[emptyRowColumn].Value != null)
            {
                emptyRow = row.Cells[emptyRowColumn].Value.ToString() == "true";
            }

            chartTable.Visible = !emptyRow;

            if (!emptyRow)
            {
                UltraChart.DataBind();
            }
            else
            {
                UltraChart.DataSource = null;
            }
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0004_Yar_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("_", String.Empty);
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                int widthColumn = 250;
                bool textWrap = false;
                HorizontalAlign textAlignment = HorizontalAlign.Left;

                switch(i)
                {
                    case 1:
                        {
                            widthColumn = 270;
                            textWrap = true;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 100;
                            formatString = "N2";
                            textAlignment = HorizontalAlign.Right;
                            break;
                        }
                    case 3:
                    case 4:
                    case 5:
                        {
                            widthColumn = 233;
                            textWrap = true;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = textAlignment;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = textWrap;
            }

            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[7].Hidden = true;
            e.Layout.Bands[0].Columns[8].Hidden = true;

            headerLayout.AddCell("№ п/п");
            headerLayout.AddCell("Наименование направлений оценки, групп показателей, показателей");
            headerLayout.AddCell("Средняя оценка по показателю (SP)");
            headerLayout.AddCell("ГРБС, получившие неудовлетворительную оценку по показателю");
            headerLayout.AddCell("ГРБС, получившие лучшую оценку по показателю");
            headerLayout.AddCell("ГРБС, к которым показатель не применим");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int levelColumn = e.Row.Cells.Count - 3;
            int emptyRowColumn = e.Row.Cells.Count - 2;
            string level = String.Empty;
            if (e.Row.Cells[levelColumn].Value != null)
            {
                level = e.Row.Cells[levelColumn].Value.ToString();
            }

            bool emptyRow = false;
            if (e.Row.Cells[emptyRowColumn].Value != null)
            {
                emptyRow = e.Row.Cells[emptyRowColumn].Value.ToString() == "true";
            }

            for (int i = 0; i < e.Row.Cells.Count - 3; i++)
            {
                int fontSize = 8;
                bool bold = false;
                bool italic = false;
                switch (level)
                {
                    case "Показатели 1":
                        {
                            fontSize = 8;
                            bold = true;
                            break;
                        }
                    case "Показатели 2":
                        {
                            fontSize = 8;
                            italic = true;
                            break;
                        }
                    case "Показатели 3":
                        {
                            fontSize = 8;
                            break;
                        }
                }
                e.Row.Cells[i].Style.Font.Size = fontSize;
                e.Row.Cells[i].Style.Font.Bold = bold;
                e.Row.Cells[i].Style.Font.Italic = italic;

                if (emptyRow)
                {
                    if (i == 1)
                    {
                        e.Row.Cells[i].ColSpan = e.Row.Cells.Count - 5;
                    }
                    else if (i > 1)
                    {
                        e.Row.Cells[i].Value = String.Empty;
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0004_Yar_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Columns.Count > 0 && !IsZeroOrNullColumn(dtChart, 1))
            {
                DataTable dtChartCopy = dtChart.Copy();
                if (dtChartCopy.Columns.Count > 3)
                {
                    dtChartCopy.Columns.RemoveAt(2);
                    dtChartCopy.Columns.RemoveAt(2);
                }

                foreach (DataRow row in dtChartCopy.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().TrimStart('_');
                    }
                }

                UltraChart.DataSource = dtChartCopy;
            }
        }

        private static bool IsZeroOrNullColumn(DataTable dt, int columnIndex)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[columnIndex] != DBNull.Value && row[columnIndex].ToString() != String.Empty)
                {
                    double value = Convert.ToDouble(row[columnIndex]);
                    if (value != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (dtChart != null && dtChart.Rows.Count > 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text)primitive;
                        axisText.bounds.Width = 30;
                        axisText.labelStyle.HorizontalAlign = StringAlignment.Center;
                        axisText.labelStyle.FontSizeBestFit = false;
                        axisText.labelStyle.Font = new Font("Verdana", 8);
                        axisText.labelStyle.WrapText = false;
                    }
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null && box.Value != null)
                        {
                            int rowIndex = box.Row;
                            int columnIndex = box.Column + 2;

                            string indicatorList = String.Empty;
                            if (dtChart.Rows[rowIndex][columnIndex] != DBNull.Value &&
                                dtChart.Rows[rowIndex][columnIndex].ToString() != String.Empty)
                            {
                                string list = String.Format("({0})", dtChart.Rows[rowIndex][columnIndex].ToString().TrimEnd(','));
                                indicatorList = GetWrapText(list.Replace(";", ", "), 60);
                            }
                            
                            string share = String.Empty;
                            if (dtChart.Rows[rowIndex][columnIndex + 1] != DBNull.Value &&
                                dtChart.Rows[rowIndex][columnIndex + 1].ToString() != String.Empty)
                            {
                                double value = Convert.ToDouble(dtChart.Rows[rowIndex][columnIndex + 1]);
                                share = String.Format("Доля: {0:P2}", value);
                            }

                            box.DataPoint.Label = String.Format("{0}\n{1}",indicatorList, share);
                        }
                        else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                        {
                            box.PE.Fill = Color.MediumPurple;
                            box.PE.FillStopColor = Color.MediumOrchid;
                            box.rect = new Rectangle(box.rect.X, box.rect.Y + box.rect.Height / 3, box.rect.Width, box.rect.Height / 3);
                        }
                    }
                }

                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];
                
                if (xAxis == null || yAxis == null)
                    return;

                double yMin = yAxis.MapMinimum;
                double yMax = yAxis.MapMaximum;

                double xMin = xAxis.MapMinimum;

                Box grayBox = new Box(new Point((int)xMin, (int)yMax), (int)xAxis.Map(3) - (int)xMin, (int)yMin - (int)yMax);
                grayBox.PE.Fill = Color.Gainsboro;
                grayBox.PE.FillOpacity = 100;
                grayBox.PE.ElementType = PaintElementType.SolidFill;
                grayBox.PE.Stroke = Color.Transparent;
                e.SceneGraph.Insert(20, grayBox);
                //e.SceneGraph.Add(grayBox);
                
                if (avgEvaluation != Double.MinValue)
                {
                    double axisStep = (xAxis.Map(1) - xAxis.Map(0));
                    double scale = 0.01;
                    double colIndex = avgEvaluation * scale;
                    double lineX = xAxis.Map(colIndex) + avgEvaluation/axisStep;

                    Line line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                    line.p1 = new Point((int)lineX, (int)yMax);
                    line.PE.Stroke = Color.MediumOrchid;
                    line.PE.FillOpacity = 50;
                    line.PE.StrokeWidth = 5;
                    line.p2 = new Point((int)lineX, (int)yMin);
                    e.SceneGraph.Add(line);
                }
            }
        }

        private static string GetWrapText(string text, int charCount)
        {
            bool wrap = false;
            string wrapText = String.Empty;
            int rowLenght = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (!wrap)
                {
                    wrap = (rowLenght == charCount);
                }

                if (wrap && text[i] == ' ')
                {
                    wrapText += '\n';
                    wrap = false;
                    rowLenght = 0;
                }
                else
                {
                    rowLenght++;
                    wrapText += text[i];
                }
            }

            return wrapText;
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart.Width.Value / 3);

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, chartCaption.Text, section2);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.75));
            UltraChart.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart.Width.Value / 3);
            ReportExcelExporter1.Export(UltraChart, sheet2, 3);
        }

        #endregion
    }
}
