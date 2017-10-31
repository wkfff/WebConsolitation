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

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0002_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private DataTable dtChart;
        private DataTable dtChartAVG;
        private DataTable dtChart2;
        private DataTable dtChart3;
        private DataTable dtChart4;
        private DataTable dtChart2AVG;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedYear;
        private int stepNumStr = 1;
        private int chart3Sum;
        private int chart4Sum;
        private static Dictionary<string, string> indicatorNameList;
        

        private double beginQualityLimit;
        private double endQualityLimit;
        private int beginQualityIndex;
        private int endQualityIndex;
        private double beginQualityLimit2;
        private double endQualityLimit2;
        private int beginQualityIndex2;
        private int endQualityIndex2;

        private double avgValue;
        private double avgValue2;

        private GridHeaderLayout headerLayout;

        #endregion


        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
    
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.7; 
            double scaleChart = 0.35;
        
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.57);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.73);
            UltraWebGrid.DisplayLayout.NoDataMessage = String.Empty;
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.4);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * scaleChart);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.4);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * scaleChart);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.475);
            UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart4.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.475);
            UltraChart4.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
            UltraChart4.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
        
            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 100;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = "Итоговая сводная оценка, балл";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart.ColorModel.ColorBegin = Color.Green;
            UltraChart.ColorModel.ColorEnd = Color.Red;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N2>";
            appearance.ChartTextFont = new Font("Verdana", 7);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>: <DATA_VALUE:N2>";

            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.ColumnChart.SeriesSpacing = 1;
            UltraChart2.ColumnChart.ColumnSpacing = 1;

            UltraChart2.Axis.X.Extent = 105;
            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart2.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart2.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart2.Axis.X.StripLines.Interval = 2;
            UltraChart2.Axis.X.StripLines.Visible = true;
            UltraChart2.Axis.Y.Extent = 50;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;

            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart2.TitleLeft.Text = "Итоговая сводная оценка, балл";
            UltraChart2.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart2.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart2.ColorModel.ColorBegin = Color.Green;
            UltraChart2.ColorModel.ColorEnd = Color.Red;

            ChartTextAppearance appearance2 = new ChartTextAppearance();
            appearance2.Column = -2;
            appearance2.Row = -2;
            appearance2.VerticalAlign = StringAlignment.Far;
            appearance2.ItemFormatString = "<DATA_VALUE:N2>";
            appearance2.ChartTextFont = new Font("Verdana", 7);
            appearance2.Visible = true;
            UltraChart2.ColumnChart.ChartText.Add(appearance2);
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>: <DATA_VALUE:N2>";

            UltraChart3.ChartType = ChartType.DoughnutChart;
            UltraChart3.DoughnutChart.RadiusFactor = 90;
            UltraChart3.DoughnutChart.InnerRadius = 75;
            UltraChart3.Border.Thickness = 0;
            UltraChart3.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart3.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            UltraChart3.Tooltips.FormatString = "<ITEM_LABEL>\n <DATA_VALUE:N2> тыс.руб.";
            CRHelper.FillCustomColorModel(UltraChart3, 6, true);

            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            UltraChart3.Legend.SpanPercentage = 15;
            UltraChart3.Legend.Margins.Top = 0;
            UltraChart3.Legend.Margins.Left = 0;
            UltraChart3.Legend.Margins.Bottom = 0;
        
            UltraChart3.TitleTop.Text = "";
            UltraChart3.TitleTop.Font = new Font("Verdana", 8);
            UltraChart3.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart3.TitleTop.Margins.Left = Convert.ToInt32((UltraChart3.Width.Value)) * UltraChart3.Legend.SpanPercentage / 100 + 5;
            UltraChart3.TitleTop.Visible = true;

            UltraChart4.ChartType = ChartType.DoughnutChart;
            UltraChart4.DoughnutChart.RadiusFactor = 90;
            UltraChart4.DoughnutChart.InnerRadius = 75;
            UltraChart4.Border.Thickness = 0;
            UltraChart4.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart4.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            UltraChart4.Tooltips.FormatString = "<ITEM_LABEL>\n <DATA_VALUE:N2> тыс.руб.";
            CRHelper.FillCustomColorModel(UltraChart4, 6, true);

            UltraChart4.Legend.Visible = true;
            UltraChart4.Legend.Location = LegendLocation.Bottom;
            UltraChart4.Legend.SpanPercentage = 15;
            UltraChart4.Legend.Margins.Top = 0;
            UltraChart4.Legend.Margins.Left = 0;
            UltraChart4.Legend.Margins.Bottom = 0;

            UltraChart4.TitleTop.Text = "";
            UltraChart4.TitleTop.Font = new Font("Verdana", 8);
            UltraChart4.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart4.TitleTop.Margins.Left = Convert.ToInt32((UltraChart4.Width.Value)) * UltraChart4.Legend.SpanPercentage / 100 + 5;
            UltraChart4.TitleTop.Visible = true;
            
            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Картограмма&nbsp;с&nbsp;результатами&nbsp;оценки";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0003_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отдельному&nbsp;показателю";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0004_HMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Мониторинг&nbsp;соблюдения&nbsp;бюджетного&nbsp;законодательства";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0005_HMAO/Default.aspx";

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedPeriod.Value = string.Format("{0}", selectedYear);

            dtChart3 = new DataTable();
            string queryChart = DataProvider.GetQueryText("FO_0021_0002_HMAO_chart3_sum");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(queryChart, dtChart3);
            chart3Sum = Convert.ToInt32(dtChart3.Rows[0][0]);

            BoxAnnotation annotation1 = new BoxAnnotation();
            annotation1.Text = string.Format("Общая сумма грантов\n {0:N0} тыс. руб.",chart3Sum);
            annotation1.TextStyle.Font = new Font("Verdana", 8);
            annotation1.Width = 130;
            annotation1.Height = 100;
            annotation1.Location.Type = LocationType.Percentage;
            annotation1.Location.LocationX = 50;
            annotation1.Location.LocationY = 43;
            annotation1.Border.Thickness = 0;
            UltraChart3.Annotations.Add(annotation1);

            dtChart4 = new DataTable();
            queryChart = DataProvider.GetQueryText("FO_0021_0002_HMAO_chart4_sum");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(queryChart, dtChart4);
            chart4Sum = Convert.ToInt32(dtChart4.Rows[0][0]);

            annotation1 = new BoxAnnotation();
            annotation1.Text = string.Format("Общая сумма грантов\n {0:N0} тыс. руб.", chart4Sum);
            annotation1.TextStyle.Font = new Font("Verdana", 8);
            annotation1.Width = 130;
            annotation1.Height = 100;
            annotation1.Location.Type = LocationType.Percentage;
            annotation1.Location.LocationX = 50;
            annotation1.Location.LocationY = 43;
            annotation1.Border.Thickness = 0;
            UltraChart4.Annotations.Add(annotation1);

            Page.Title = String.Format("Сводная оценка качества организации и осуществления бюджетного процесса в муниципальных образованиях автономного округа, рейтинг муниципальных образований автономного округа");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;
            PageSubTitle.Text = string.Format("По итогам {0} года", selectedYear);
            
            IndicatorDescriptionDataBind();

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            chartCaption.Text = "Рейтинг ГО";
            chart2Caption.Text = "Рейтинг МР";
            chart3Caption.Text = "Распределение грантов в городских округах";
            chart4Caption.Text = "Распределение грантов в муниципальных районах";
            AVGChartDataBind();
            UltraChart.DataBind();

            AVGChart2DataBind();
            UltraChart2.DataBind();

            UltraChart3.DataBind(); 
            UltraChart4.DataBind();
        }

        #region Показатели

        private void IndicatorDescriptionDataBind()
        {
            indicatorNameList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();

                indicatorNameList.Add(code, name);
            }
        }

        private static int GetIndicatorColumnWidth(string indicatorCode)
        {
            switch (indicatorCode)
            {
                case "ИСО":
                case "СОК":
                case "Рейтинг":
                    {
                        return 80;
                    }
                default:
                    {
                        return 130;
                    }
            }
        }

        private static string GetIndicatorColumnFormat(string indicatorCode)
        {
            switch (indicatorCode)
            {
                case "ИСО":
                case "СОК":
                    {
                        return "N2";
                    }
                default:
                    {
                        return "N0";
                    }
            }
        }

        #endregion

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_grid");
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
       

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count < 2)
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

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(120);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Муниципальное образование";
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Header.Caption);
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string indicatorCode = e.Layout.Bands[0].Columns[i].Header.Caption;
                int widthColumn = GetIndicatorColumnWidth(indicatorCode);
                string formatNumber = GetIndicatorColumnFormat(indicatorCode);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatNumber);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell("Высокодотационное муниципальное образование (в соответствии с ч. 4 ст. 136 БК РФ)");
            headerLayout.AddCell("Сводная оценка качества");
            headerLayout.AddCell("Количество нарушений бюджетного законодательства");
            headerLayout.AddCell("Итоговая сводная оценка");
            headerLayout.AddCell("Рейтинг по итоговой сводной оценке");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].ToString() == "Средняя сводная оценка качества по городским округам" || e.Row.Cells[1].ToString() == "Средняя сводная оценка качества по муниципальным районам")
            {
                stepNumStr--;
                e.Row.Cells[0].Value = null;
                for (int i = 0; i < e.Row.Cells.Count; i = i + 1)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            else
            {             
                e.Row.Cells[0].Value = Convert.ToInt32(e.Row.Index + stepNumStr).ToString("N0");             
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
        }

        #endregion

        #region Обработчики диаграммы для ГО 

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
          
                string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_chart");
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

        protected void AVGChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);

            avgValue = GetDoubleDTValue(dtChartAVG, "ИСО");

            string queryQuality = DataProvider.GetQueryText("FO_0021_0002_HMAO_quality_limit");
            
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryQuality, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                beginQualityLimit = GetDoubleDTValue(dtChart, "Первая граница");
                endQualityLimit = GetDoubleDTValue(dtChart, "Вторая граница");
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

        private int GetQualityDegree2(double value2)
        {
            if (value2 >= beginQualityLimit2)
            {
                return 1;
            }
            if (value2 <= endQualityLimit2)
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
               
            }
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 120;
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


            
        }

        #endregion

        #region Обработчики диаграммы для МР

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_chart2");
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            foreach (DataRow row in dtChart2.Rows)
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

            UltraChart2.Series.Clear();
            for (int i = 1; i < dtChart2.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
                series.Label = dtChart2.Columns[i].ColumnName;
                UltraChart2.Series.Add(series);
            }

            if (dtChart2 != null)
            {
                int currentDegree = 0;
                beginQualityIndex2 = -1;
                endQualityIndex2 = -1;
                for (int i = 0; i < dtChart2.Rows.Count; i++)
                {
                    if (dtChart2.Rows[i][1] != DBNull.Value && dtChart2.Rows[i][1].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(dtChart2.Rows[i][1]);

                        if (i == 0 || currentDegree < GetQualityDegree2(value))
                        {
                            currentDegree = GetQualityDegree2(value);

                            if (i != 0)
                            {
                                if (currentDegree == 2)
                                {
                                    beginQualityIndex2 = i - 1;
                                }
                                else
                                {
                                    endQualityIndex2 = i - 1;
                                }
                            }
                        }
                    }
                }
            }

        }

        protected void AVGChart2DataBind()
        {
            string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_avg_chart2");
            dtChart2AVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2AVG);

            avgValue2 = GetDoubleDTValue(dtChart2AVG, "ИСО");

            string queryQuality2 = DataProvider.GetQueryText("FO_0021_0002_HMAO_quality_limit2");

            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryQuality2, "Dummy", dtChart2);
            if (dtChart2.Rows.Count > 0)
            {
                beginQualityLimit2 = GetDoubleDTValue(dtChart2, "Первая граница");
                endQualityLimit2 = GetDoubleDTValue(dtChart2, "Вторая граница");
            }

        }

        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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
                
            }
            IAdvanceAxis xAxis2 = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis2 = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis2 == null || yAxis2 == null)
                return;

            int textWidht = 120;
            int textHeight = 12;
            double lineStart2 = xAxis2.MapMinimum;
            double lineLength2 = xAxis2.MapMaximum;

            Line line2 = new Line();
            line2.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line2.PE.Stroke = Color.DarkGray;
            line2.PE.StrokeWidth = 2;
            line2.p1 = new Point((int)lineStart2, (int)yAxis2.Map(avgValue2));
            line2.p2 = new Point((int)lineStart2 + (int)lineLength2, (int)yAxis2.Map(avgValue2));
            e.SceneGraph.Add(line2);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)lineLength2 - textWidht, ((int)yAxis2.Map(avgValue2)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя оценка: {0:N2}", avgValue2));
            e.SceneGraph.Add(text);


            
        }

        #endregion

        #region Обработчики диаграммы грантов для городских округов

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_chart3");
            dtChart3 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart3);
            UltraChart3.DataSource = dtChart3;
        }    

        #endregion

        #region Обработчики диаграммы грантов для муниципальных районов

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0021_0002_HMAO_chart4");
            dtChart4 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart4);
            UltraChart4.DataSource = dtChart4;
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
            ISection section3 = report.AddSection();
            ISection section4 = report.AddSection();
            ISection section5 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart3.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart3.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));
            UltraChart4.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart4.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, chartCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart2, chart2Caption.Text, section3);
            ReportPDFExporter1.Export(UltraChart3, chart3Caption.Text, section4);
            ReportPDFExporter1.Export(UltraChart4, chart4Caption.Text, section5);
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
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            Worksheet sheet4 = workbook.Worksheets.Add("sheet4");
            Worksheet sheet5 = workbook.Worksheets.Add("sheet5");

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.GridColumnWidthScale = 1.5;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart3.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart3.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));
            UltraChart4.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.45));
            UltraChart4.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));

            ReportExcelExporter1.Export(UltraChart, chartCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart2, chart2Caption.Text, sheet3, 3);
            ReportExcelExporter1.Export(UltraChart3, chart3Caption.Text, sheet4, 3); 
            ReportExcelExporter1.Export(UltraChart4, chart4Caption.Text, sheet5, 3);
        }

        #endregion
    }
}