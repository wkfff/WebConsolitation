using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2008;
        private int endYear = 2011;

        private int currentYear;
        private double avgEvaluation;

        private static MemberAttributesDigest grbsDigest;

        #endregion

        public bool UseComparabledDimension
        {
            get { return currentYear > 2009; }
        }

        #region Параметры запроса

        // выбранный индикатор
        private CustomParam selectedIndicator;

        // измерение [Показатели].[Оценка качества ФМ]
        private CustomParam indicatorDimension;
        // корневой элемент [Показатели].[Оценка качества ФМ]
        private CustomParam indicatorAllLevel;

        // измерение [Администратор].[Анализ]
        private CustomParam administratorDimension;
        // корневой элемент [Администратор].[Анализ]
        private CustomParam administratorAllLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
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
            UltraChart.Legend.SpanPercentage = 9;
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

            UltraChart.Tooltips.FormatString = "Оценка в интервале <SERIES_LABEL> наблюдается у <DATA_VALUE:N0> ГРБС\n<ITEM_LABEL>";

            #endregion

            #region Инициализация параметров запроса

            selectedIndicator = UserParams.CustomParam("selected_indicator");

            indicatorDimension = UserParams.CustomParam("indicator_dimension");
            indicatorAllLevel = UserParams.CustomParam("indicator_all_level");
            administratorDimension = UserParams.CustomParam("administrator_dimension");
            administratorAllLevel = UserParams.CustomParam("administrator_all_level");

            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;ГРБС";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0002/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0003_grbsDigest");

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartCaption);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                hiddenIndicatorLabel.Text = "[Показатели].[Оценка качества ФМ].[Данные всех источников].[ФО\0042 Оценка качества ФМ - 2009 квартал 0].[Качество ведения реестра расходных обязательств Омской области]";
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = String.Format("Анализ оценки качества финансового менеджмента, осуществляемого главными распорядителями средств областного бюджета в разрезе показателей");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = String.Format("по итогам {0} года", currentYear);
            
            UserParams.PeriodYear.Value = currentYear.ToString();

            indicatorDimension.Value = UseComparabledDimension
                   ? "[Показатели].[Оценка качества ФМ_Сопоставимый]"
                   : "[Показатели].[Оценка качества ФМ]";
            indicatorAllLevel.Value = UseComparabledDimension
                                           ? "[Показатели].[Оценка качества ФМ_Сопоставимый].[Все показатели]"
                                           : String.Format("[Показатели].[Оценка качества ФМ].[Данные всех источников].[ФО\\0042 Оценка качества ФМ - {0} квартал 0]", currentYear);
            administratorDimension.Value = UseComparabledDimension
                                           ? "[Администратор].[Сопоставим]"
                                           : "[Администратор].[Анализ]";
            administratorAllLevel.Value = UseComparabledDimension
                                           ? "[Администратор].[Сопоставим].[Все администраторы]"
                                           : String.Format("[Администратор].[Анализ].[Данные всех источников].[ФО Анализ данных - {0}]", currentYear);

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                string patternValue = selectedIndicator.Value;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

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

            string indicatorCode = row.Cells[1].Text;
            string indicatorName = row.Cells[2].Text;

            avgEvaluation = Double.MinValue;
            UltraChart.Legend.Visible = false;
            if (row.Cells[3].Value != null && row.Cells[3].Value.ToString() != String.Empty)
            {
                avgEvaluation = Convert.ToDouble(row.Cells[3].Value);
                UltraChart.Legend.Visible = true;
                UltraChart.Legend.FormatString = String.Format("Среднее значение оценки: {0:N2}", avgEvaluation);
            }

            hiddenIndicatorLabel.Text = row.Cells[row.Cells.Count - 1].Text;
            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            chartCaption.Text = String.Format("Показатель «{0}» ({1})", indicatorName, indicatorCode);

            UltraChart.DataBind();
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 4; i <= 6; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = GetShortGRBSNames(row[i].ToString());
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private string GetShortGRBSNames(string fullNames)
        {
            string shortNames = String.Empty;

            string[] names = fullNames.Split(';');
            foreach (string s in names)
            {
                if (s != String.Empty)
                {
                    shortNames += String.Format("{0}, ",  UseComparabledDimension ? grbsDigest.GetShortName(s) : DataDictionariesHelper.GetShortFMGRBSNames(s));
                }
            }

            return shortNames.TrimEnd(' ').TrimEnd(',');
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[0];
            numberColumn.Header.Caption = "№ п/п";
            numberColumn.Width = CRHelper.GetColumnWidth(30);
            numberColumn.CellStyle.Padding.Right = 5;
            numberColumn.CellStyle.BackColor = numberColumn.Header.Style.BackColor;
            numberColumn.CellStyle.Font.Bold = true;
            numberColumn.SortingAlgorithm = SortingAlgorithm.NotSet;
            numberColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right;

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
                            widthColumn = 50;
                            textAlignment = HorizontalAlign.Left;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 230;
                            textWrap = true;
                            break;
                        }
                    case 3:
                        {
                            widthColumn = 100;
                            formatString = "N2";
                            textAlignment = HorizontalAlign.Right;
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
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

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Обозн.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Наименование показателя", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Средняя оценка показателя исходя из его применимости к ГРБС", "Ед.изм.: балл");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "ГРБС, получивший минимальную оценку", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "ГРБС, получивший максимальную оценку", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "ГРБС, по которому показатель не применим", "");

            e.Layout.Bands[0].Columns[7].Hidden = true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Value = Convert.ToInt32(e.Row.Index + 1).ToString("N0");

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
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
            string query = currentYear == 2008 
                ? DataProvider.GetQueryText("FO_0042_0003_chart_fourth")
                : DataProvider.GetQueryText("FO_0042_0003_chart_tenth");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("_", "]");
                }

                if (row[2] != DBNull.Value)
                {
                    row[2] = GetShortGRBSNames(row[2].ToString());
                }
            }

            DataTable dtChartCopy = dtChart.Copy();
            if (dtChartCopy.Columns.Count > 2)
            {
                dtChartCopy.Columns.RemoveAt(2);
            }

            UltraChart.DataSource = dtChartCopy;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (dtChart.Rows.Count > 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text)primitive;
                        axisText.bounds.Width = 50;
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
                            if (dtChart != null && dtChart.Rows[rowIndex][columnIndex] != DBNull.Value &&
                                dtChart.Rows[rowIndex][columnIndex].ToString() != String.Empty)
                            {
                                string list = String.Format("({0})", dtChart.Rows[rowIndex][columnIndex].ToString().TrimEnd(','));
                                list = BreakCollocator(list, ',', 5);
                                indicatorList = list.Replace(",", ", ");
                            }

                            box.DataPoint.Label = indicatorList;
                        }
                        else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                        {
                            box.PE.Fill = Color.MediumPurple;
                            box.PE.FillStopColor = Color.MediumOrchid;
                            box.rect = new Rectangle(box.rect.X, box.rect.Y + box.rect.Height / 3, box.rect.Width, box.rect.Height / 3);
                        }
                    }
                }

                if (avgEvaluation != Double.MinValue)
                {
                    IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                    IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                    if (xAxis == null || yAxis == null)
                        return;

                    double yMin = yAxis.MapMinimum;
                    double yMax = yAxis.MapMaximum;

                    double axisStep = (xAxis.Map(1) - xAxis.Map(0));
                    int scale = currentYear == 2008 ? 1 : 10;
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
                    //e.SceneGraph.Insert(10, line);
                }
            }
        }

        private static string BreakCollocator(string source, char breakChar, int charIndex)
        {
            string breakedStr = String.Empty;

            int charCount = 0;
            foreach (char ch in source)
            {
                breakedStr += ch;
                if (ch == breakChar)
                {
                    charCount++;
                    if (charCount == charIndex)
                    {
                        breakedStr += "\n";
                        charCount = 0;
                    }
                }
            }

            return breakedStr;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count - 1;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.CurrentWorksheet.Columns[0].Width = 90 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Height = 20 * 37;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                string formatString = "";
                int widthColumn = 250;
                ExcelDefaultableBoolean textWrap = ExcelDefaultableBoolean.False;
                HorizontalCellAlignment textAlignment = HorizontalCellAlignment.Left;

                switch (i)
                {
                    case 1:    
                        {
                            widthColumn = 50;
                            formatString = "0";
                            textAlignment = HorizontalCellAlignment.Right;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 240;
                            textWrap = ExcelDefaultableBoolean.True;
                            break;
                        }
                    case 3:
                        {
                            widthColumn = 100;
                            formatString = "#,###0.00;[Red]-#,###0.00";
                            textAlignment = HorizontalCellAlignment.Right;
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
                        {
                            widthColumn = 400;
                            textWrap = ExcelDefaultableBoolean.True;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = textAlignment;
                e.CurrentWorksheet.Columns[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Columns[i].CellFormat.WrapText = textWrap;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }

            // расставляем стили у начальных колонок
            for (int i = beginExportIndex; i < rowsCount + beginExportIndex; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 50 * 37;
            }

            int chartRowIndex = beginExportIndex + rowsCount + 2;
            e.CurrentWorksheet.Rows[chartRowIndex - 1].Cells[0].Value = chartCaption.Text;

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart.Width.Value) / 3;

            UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[chartRowIndex].Cells[0], UltraChart);
        }

        private int beginExportIndex = 4;

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = beginExportIndex;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart.Width.Value) / 3;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
