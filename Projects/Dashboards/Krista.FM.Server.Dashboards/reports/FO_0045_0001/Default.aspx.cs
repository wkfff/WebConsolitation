using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0045_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable populationDt;
        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtChartLimit;
        private int firstYear = 2009;
        private int endYear = 2012;
        private int selectedMonthIndex;
        private int selectedYear;
        private DateTime currentDateTime;
        private double chartLimit;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // численность текущего года
        private CustomParam currentYearPopulation;
        // численность прошлого года
        private CustomParam lastYearPopulation;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.9;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.58);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #region Инициализация параметров запроса

            currentYearPopulation = UserParams.CustomParam("current_year_population");
            lastYearPopulation = UserParams.CustomParam("last_year_population");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X2.Extent = 10;
            UltraChart.Axis.X2.Visible = true;
            UltraChart.Axis.X2.LineThickness = 0;
            UltraChart.Axis.X2.Labels.Visible = false;
            UltraChart.Axis.X2.Labels.SeriesLabels.Visible = false;

            UltraChart.Axis.Y.Extent = 70;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            //UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 5;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 8;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Тыс.руб.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Муниципальный&nbsp;долг";
            CrossLink1.NavigateUrl = "~/reports/FO_0045_0002/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Долговая&nbsp;нагрузка&nbsp;";
            CrossLink2.NavigateUrl = "~/reports/FO_0045_0003/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0045_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedMonthIndex = ComboMonth.SelectedIndex + 1;
            currentDateTime = new DateTime(selectedYear, selectedMonthIndex, 1);
            DateTime nextMonthDateTime = currentDateTime.AddMonths(1);

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<SERIES_LABEL> {0} года\n<DATA_VALUE:N1> тыс.руб.", currentDateTime.Year);

            Page.Title = String.Format("Государственный долг");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDateTime.Month), nextMonthDateTime.Year);
            chartCaption.Text = "Структура государственного долга субъекта в помесячной динамике";

            CommentTextLabel.Text = String.Format("* Предельный объем государственного долга субъекта Российской Федерации не должен превышать утвержденный общий годовой объем доходов бюджета субъекта Российской Федерации без учета утвержденного объема безвозмездных поступлений.");

            UserParams.PeriodYear.Value = currentDateTime.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDateTime.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDateTime.Month));
            UserParams.PeriodMonth.Value = String.Format(CRHelper.RusMonth(currentDateTime.Month));

            PopulationDataBind();

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            ChartLimitDataBind();
            UltraChart.DataBind();
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0045_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Вид долговых обязательств", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }
        
        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0 )
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(260);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
                string formatString = columnName.ToLower().Contains("темп роста") || columnName.ToLower().Contains("удельный вес") ||
                    columnName.ToLower().Contains("уровень государственного долга") ? "P1" : "N1";
                int columnWidth = 110;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = columnWidth;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            string currYearCaption = (String.Format("На {0:dd.MM.yyyy}", currentDateTime.AddMonths(1)));
            string lastYearCaption = (String.Format("На {0:dd.MM.yyyy}", currentDateTime.AddYears(-1).AddMonths(1)));

            headerLayout.AddCell("Вид долговых обязательств");
            GridHeaderCell cell = headerLayout.AddCell("Сумма государственного долга, тыс.руб.");
            cell.AddCell(lastYearCaption, String.Format("Сумма государственного долга {0}", lastYearCaption.ToLower()));
            cell.AddCell(currYearCaption, String.Format("Сумма государственного долга {0}", currYearCaption.ToLower()));
            cell.AddCell("Отклонение", "Прирост /снижение государственного долга по сравнению с прошлым отчётным периодом, абсолютное значение");
            cell.AddCell("Темп роста, %", "Темп роста/снижения государственного долга относительно прошлого отчётного периода");
            cell.AddCell("Удельный вес, %", "Доля данного вида долга к общей сумме государственного долга");

            cell = headerLayout.AddCell("Сумма государственного долга на душу населения, тыс.руб./чел.");
            cell.AddCell(lastYearCaption, String.Format("Сумма государственного долга на душу населения {0}", lastYearCaption.ToLower()));
            cell.AddCell(currYearCaption, String.Format("Сумма государственного долга на душу населения {0}", currYearCaption.ToLower()));
            cell.AddCell("Отклонение", "Прирост /снижение государственного долга на душу населения по сравнению с прошлым отчётным периодом, абсолютное значение");

            cell = headerLayout.AddCell("Уровень государственного долга к утвержденному годовому объему собственных доходов, %");
            cell.AddCell(lastYearCaption, String.Format("Уровень государственного долга к утвержденному годовому объему собственных доходов {0}", lastYearCaption.ToLower()));
            cell.AddCell(currYearCaption, String.Format("Уровень государственного долга к утвержденному годовому объему собственных доходов {0}", currYearCaption.ToLower()));
            cell.AddCell("Отклонение", "Отклонение относительно аналогичного периода прошлого годае");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
            }

            bool totalRow = rowName.ToLower().Contains("итого");

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnName = e.Row.Band.Grid.Columns[i].Header.Caption.ToLower();

                bool rateColumn = columnName.Contains("темп роста");
                bool deviationColumn = columnName.Contains("отклонение");

                if (totalRow)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                if (rateColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }


                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (deviationColumn && value > 0 || rateColumn && value > 1)
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
            string query = DataProvider.GetQueryText("FO_0045_0001_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Columns.Count > 0)
            {
                foreach (DataColumn column in dtChart.Columns)
                {
                    //column.ColumnName = column.ColumnName.Replace("автономного округа", String.Empty);
                    column.ColumnName = CRHelper.ToUpperFirstSymbol(column.ColumnName);
                }

                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }
            }
        }

        private void ChartLimitDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0045_0001_chartLimitLine");
            dtChartLimit = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartLimit);

            chartLimit = double.MinValue;
            if (dtChartLimit.Rows.Count > 0)
            {
                if (dtChartLimit.Rows[0][0] != DBNull.Value && dtChartLimit.Rows[0][0].ToString() != String.Empty)
                {
                     chartLimit = Convert.ToDouble(dtChartLimit.Rows[0][0]);
                     CRHelper.SaveToErrorLog(chartLimit.ToString());
                }
            }
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int seriesNum = 0;
            if (dtChart != null && dtChart.Rows.Count > 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box && primitive.Path == "Legend")
                    {
                        Box box = (Box) primitive;
                        box.rect.X += 20 * seriesNum;
                        seriesNum++;
                    }
                    else if (primitive is Text && String.IsNullOrEmpty(primitive.Path))
                    {
                        Text legeng = (Text) primitive;
                        legeng.bounds.X += 20 * seriesNum;
                        
                    }
                }
            }

            if (chartLimit != Double.MinValue)
            {
                IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

                yAxis.Maximum = Math.Max(chartLimit, (double)yAxis.Maximum) * 1.01;

                int textWidht = 550;
                int textHeight = 16;
                int lineStart = (int) xAxis.MapMinimum;
                int lineLength = (int) xAxis.MapMaximum;

                Line line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.Red;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(lineStart, (int) yAxis.Map(chartLimit));
                line.p2 = new Point(lineStart + lineLength, (int) yAxis.Map(chartLimit));
                e.SceneGraph.Add(line);

                Text text = new Text();
                text.PE.Fill = Color.Black;
                text.labelStyle.Font = new Font("Verdana", 10);
                text.bounds = new Rectangle(lineLength - textWidht, ((int) yAxis.Map(chartLimit)) - textHeight, textWidht, textHeight);
                text.SetTextString(string.Format("Предельный уровень государственного долга: {0:N2} тыс.руб.*", chartLimit));
                e.SceneGraph.Add(text);
            }
        }
        
        #endregion

        #region Численность

        protected void PopulationDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0045_0001_population");
            populationDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", populationDt);

            currentYearPopulation.Value = GetStringDTValue(populationDt, "Численность постоянного населения в текущем году");
            lastYearPopulation.Value = GetStringDTValue(populationDt, "Численность постоянного населения в прошлом году");
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName]).ToString().Replace(",", ".");
            }
            return "0";
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
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.85));
            ReportExcelExporter1.Export(UltraChart, chartCaption.Text, sheet2, 3);
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

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, chartCaption.Text, section2);
        }

        #endregion
    }
}