using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0002_Saratov
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtChartAVG;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedQuarterIndex;
        private int selectedYear;


        private double beginQualityLimit;
        private double endQualityLimit;
        private int beginQualityIndex;
        private int endQualityIndex;

        private double avgValue;

        private GridHeaderLayout headerLayout;

        #endregion

        private bool FirstYearSelected
        {
            get { return selectedYear == firstYear; }
        }

        private bool LastQuarterSelected
        {
            get { return selectedQuarterIndex == 4; }
        }

        private bool UseQualityDegree
        {
            get { return WithQualityDegree.Checked; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
    
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.75;
        
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.2);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale);
            UltraWebGrid.DisplayLayout.NoDataMessage = String.Empty;
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * scale);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            //UltraChart.Data.ZeroAligned = true;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 165;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 30;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
            //UltraChart.Axis.Y.TickmarkInterval = 5;

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = "Итоговая оценка, балл";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart.ColorModel.ColorBegin = Color.Green;
            UltraChart.ColorModel.ColorEnd = Color.Red;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N1>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>: <DATA_VALUE:N2>";

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0001_Saratov/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0003_Saratov/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Диаграмма&nbsp;динамики&nbsp;оценки&nbsp;качества";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0005_Saratov/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Картограмма&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0004_Saratov/Default.aspx";


        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                WithoutQualityDegree.Attributes.Add("onclick", string.Format("uncheck('{0}')", WithQualityDegree.ClientID));
                WithQualityDegree.Attributes.Add("onclick", string.Format("uncheck('{0}')", WithoutQualityDegree.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(WithoutQualityDegree.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(WithQualityDegree.ClientID);
                
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0002_Saratov_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillEvaluaitonQuarters(true));
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("Рейтинг муниципальных районов и городских округов Саратовской области по результатам оценки качества управления муниципальными финансами и платежеспособности муниципальных образований");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = GetQuarterText(selectedQuarterIndex);

            UserParams.PeriodYear.Value = selectedYear.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            
            
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            
            AVGChartDataBind();
            UltraChart.DataBind();
        }

        /// <summary>
        /// Получить элемент параметра по значению классификатора
        /// </summary>
        /// <param name="classQuarter">элемент классификатора</param>
        /// <returns>значение параметра</returns>
        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "Квартал 1":
                    {
                        return "Оценка качеcтва на 01.04";
                    }
                case "Квартал 2":
                    {
                        return "Оценка качеcтва на 01.07";
                    }
                case "Квартал 3":
                    {
                        return "Оценка качества на 01.10";
                    }
                case "Квартал 4":
                    {
                        return "Оценка качества по итогам года";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private string GetQuarterText(int selectedQuarter)
        {
            switch (selectedQuarter)
            {
                case 1:
                    {
                        return String.Format("Оценка качества по состоянию на 01.04.{0}", selectedYear);
                    }
                case 2:
                    {
                        return String.Format("Оценка качества по состоянию на 01.07.{0}", selectedYear);
                    }
                case 3:
                    {
                        return String.Format("Оценка качества по состоянию на 01.10.{0}", selectedYear);
                    }
                case 4:
                    {
                        return String.Format("Оценка качества по итогам {0} года", selectedYear);
                    }
            }
            return String.Empty;
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            if (FirstYearSelected && !LastQuarterSelected)
            {
                UltraWebGrid.DataSource = null;
            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0021_0002_Saratov_grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGrid.Rows)
                    {
                        if (row[0] != DBNull.Value)
                        {
                            row[0] = row[0].ToString().Replace("Муниципальное образование", "МО");
                            row[0] = row[0].ToString().Replace("муниципальное образование", "МО");
                            row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                        }
                    }

                    UltraWebGrid.DataSource = dtGrid;
                }
                else
                {
                    UltraWebGrid.DataSource = null;
                }
            }
        }
       

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(50);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Header.Caption);

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
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

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //UltraWebGrid.Width = Unit.Empty;
            //UltraWebGrid.Height = Unit.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            if (FirstYearSelected && !LastQuarterSelected)
            {
                UltraChart.DataSource = null;
            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0021_0002_Saratov_chart");
                dtChart = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

                foreach (DataRow row in dtChart.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = row[i].ToString().Replace("муниципальный район", "МР");
                            row[i] = row[i].ToString().Replace("муниципальное образование", "МО");
                            row[i] = row[i].ToString().Replace("\"", "'");
                            row[i] = row[i].ToString().Replace(" район", " р-н");
                        }
                    }
                }

                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }

                if (dtChart != null)
                {
                    int currentDegree = 0;
                    beginQualityIndex = -1;
                    endQualityIndex = -1;
                    for (int i = 0; i < dtChart.Rows.Count; i++)
                    {
                        if (dtChart.Rows[i][1] != DBNull.Value && dtChart.Rows[i][1].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(dtChart.Rows[i][1]);

                            if (i == 0 || currentDegree < GetQualityDegree(value))
                            {
                                currentDegree = GetQualityDegree(value);

                                if (i != 0)
                                {
                                    if (currentDegree == 2)
                                    {
                                        beginQualityIndex = i - 1;
                                    }
                                    else
                                    {
                                        endQualityIndex = i - 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void AVGChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0021_0002_Saratov_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);

            avgValue = GetDoubleDTValue(dtChartAVG, "Средняя оценка");

            string queryQuality = DataProvider.GetQueryText("FO_0021_0002_Saratov_quality_limit");
            
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryQuality, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                beginQualityLimit = GetDoubleDTValue(dtChart, "Первая граница");
                endQualityLimit = GetDoubleDTValue(dtChart, "Вторая граница");
                //beginQualityLimit = Convert.ToDouble(dtChart.Rows[1][0]);
                //endQualityLimit = Convert.ToDouble(dtChart.Rows[1][1]);
            }
         
        }

        private int GetQualityDegree(double value)
        {
            if (value >= beginQualityLimit)
            {
                return 1;
            }
            if (value <= endQualityLimit)
            {
                return 3;
            }
            return 2;
        }
        
        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }
        
        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Columns.Contains(columnName) && dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.bounds.Width = 30;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
                if (UseQualityDegree && primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        box.PE.ElementType = PaintElementType.Gradient;
                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;

                        double value = Convert.ToDouble(box.Value);
                        if (value >= beginQualityLimit)
                        {
                            box.PE.Fill = Color.LimeGreen;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else if (value <= endQualityLimit)
                        {
                            box.PE.Fill = Color.OrangeRed;
                            box.PE.FillStopColor = Color.Red;
                        }
                        else
                        {
                            box.PE.Fill = Color.SkyBlue;
                            box.PE.FillStopColor = Color.DeepSkyBlue;
                        }
                    }
                }
            }
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            double lineStart = xAxis.MapMinimum;
            double lineLength = xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new Point((int)lineStart + (int)lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)lineLength - textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя оценка: {0:N2}", avgValue));
            e.SceneGraph.Add(text);


            if (UseQualityDegree)
            {
                double xMin = xAxis.MapMinimum;
                double xMax = xAxis.MapMaximum;
                double yMin = yAxis.MapMinimum;
                double yMax = yAxis.MapMaximum;

                double axisStep = (xAxis.Map(1) - xAxis.Map(0)) / 2;

                double beginLineAxisX = xAxis.Map(2 * beginQualityIndex + 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)beginLineAxisX, (int)yMin);
                line.p2 = new Point((int)beginLineAxisX, (int)yMax - textHeight);
                if (beginLineAxisX >= xMin && beginLineAxisX <= xMax)
                {
                    e.SceneGraph.Add(line);
                }

                double endLineAxisX = xAxis.Map(2 * endQualityIndex + 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)endLineAxisX, (int)yMin);
                line.p2 = new Point((int)endLineAxisX, (int)yMax - textHeight);
                if (endLineAxisX >= xMin && endLineAxisX <= xMax)
                {
                    e.SceneGraph.Add(line);
                }

                LabelStyle labelStyle = new LabelStyle();
                labelStyle.HorizontalAlign = StringAlignment.Center;
                labelStyle.Font = new Font("Verdana", 8);
                labelStyle.FontColor = Color.Black;

                text = new Text();
                text.bounds = new Rectangle((int)xMin, (int)yMax - textHeight, (int)(beginLineAxisX - xMin), textHeight);
                text.SetTextString("I степень");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)beginLineAxisX, (int)yMax - textHeight, (int)(endLineAxisX - beginLineAxisX), textHeight);
                text.SetTextString("II степень");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)endLineAxisX, (int)yMax - textHeight, (int)(xMax - endLineAxisX), textHeight);
                text.SetTextString("III степень");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);
            }
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

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, section2);
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

            ReportExcelExporter1.GridColumnWidthScale = 1.5;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.75));
            ReportExcelExporter1.Export(UltraChart, sheet2, 3);
        }

        #endregion
    }
}