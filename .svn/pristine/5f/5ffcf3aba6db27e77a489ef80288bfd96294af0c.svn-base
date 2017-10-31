using System;
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
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtSummaryGrid;
        private DataTable dtChart;
        private DataTable dtChartAVG;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedQuarterIndex;
        private int selectedYear;

        private double beginQualityLimit;
        private double endQualityLimit;
        private double avgValue;

        private int beginQualityIndex;
        private int endQualityIndex;

        #endregion

        private bool UseQualityDegree
        {
            get { return WithQualityDegree.Checked; }
        }

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный предыдущий период
        private CustomParam selectedPrevPeriod;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.75;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            SummaryGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            SummaryGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 220);
            SummaryGrid.DataBound += new EventHandler(SummaryGrid_DataBound);
            SummaryGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.63);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedPrevPeriod == null)
            {
                selectedPrevPeriod = UserParams.CustomParam("selected_prev_period");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 140;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 20;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = "Баллы";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart.ColorModel.ColorBegin = Color.Green;
            UltraChart.ColorModel.ColorEnd = Color.Red;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N2>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N2>";

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты оценки качества МР";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0002/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Сравн.характеристика&nbsp;мин.&nbsp;и&nbsp;макс.&nbsp;оценок";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0004/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Картограмма";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0005/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отд.показателю";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                WithoutQualityDegree.Attributes.Add("onclick", string.Format("uncheck('{0}')", WithQualityDegree.ClientID));
                WithQualityDegree.Attributes.Add("onclick", string.Format("uncheck('{0}')", WithoutQualityDegree.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(WithoutQualityDegree.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(WithQualityDegree.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Оценка качества";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;
            
            string currentDate = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);

            Page.Title = String.Format("Рейтинг муниципальных районов по результатам оценки качества");
            PageTitle.Text = Page.Title;
            chart1Label.Text =  String.Format("Результаты проведения оценки качества ОиОБП в МР Омской области {0}", currentDate);
            PageSubTitle.Text = currentDate;
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            string prevQuarter = String.Format("Квартал {0}", selectedQuarterIndex - 1);
            string prevHalfYear = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex - 1));

            if (IsYearCompare)
            {
                selectedPeriod.Value = String.Format("[{0}]", selectedYear);
                selectedPrevPeriod.Value = String.Format("[{0}]", selectedYear - 1); 
            }
            else
            {
                selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
                selectedPrevPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, prevHalfYear, prevQuarter);
            }
            
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                SummaryGrid.Bands.Clear();
                if (IsYearCompare)
                {
                    UltraWebGridTable.Visible = true;
                    SummaryGridTable.Visible = true;

                    UltraWebGrid.DataBind();
                    SummaryGrid.DataBind();
                }
                else
                {
                    UltraWebGridTable.Visible = false;
                    SummaryGridTable.Visible = false;
                }
            }
            
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
                        return "по состоянию на 01.04";
                    }
                case "Квартал 2":
                    {
                        return "по состоянию на 01.07";
                    }
                case "Квартал 3":
                    {
                        return "по состоянию на 01.10";
                    }
                case "Квартал 4":
                case "Данные года":
                    {
                        return "по итогам года";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private string GetDateQuarterText(int quarterIndex)
        {
            switch (quarterIndex)
            {
                case 0:
                    {
                        return "на 01.01";
                    }
                case 1:
                    {
                        return "на 01.04";
                    }
                case 2:
                    {
                        return "на 01.07";
                    }
                case 3:
                    {
                        return IsYearCompare ? String.Format("по итогам {0} года", selectedYear - 1) : "на 01.10";
                    }
                case 4:
                    {
                        return String.Format("по итогам {0} года", selectedYear);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0039_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("район", "р-н");
                    }
                }

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

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                int widthColumn = 100;

                switch(i)
                {
                    case 1:
                    case 2:
                    case 3:
                        {
                            formatString = "N0";
                            break;
                        }
                    case 4:
                    case 5:
                    case 7:
                        {
                            formatString = "N2";
                            break;
                        }
                    case 6:
                    case 8:
                    case 9:
                        {
                            formatString = "P2";
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, GetDateQuarterText(selectedQuarterIndex - 1), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, GetDateQuarterText(selectedQuarterIndex), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Изменение рейтинга", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, GetDateQuarterText(selectedQuarterIndex - 1), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, GetDateQuarterText(selectedQuarterIndex), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Темп роста", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Отклонение", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, GetDateQuarterText(selectedQuarterIndex - 1), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, GetDateQuarterText(selectedQuarterIndex), "");

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Занимаемое место по данным оценки", 1, 0, 3, 1);
            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Итоговая оценка уровня качества ОиОБП", 4, 0, 2, 1);
            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Динамика итоговой оценки", 6, 0, 2, 1);
            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Отношение фактического результата к максимально возможному", 8, 0, 2, 1);

        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool ratingChangeColumn = (i == 3);

                if (ratingChangeColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                {
                    double value = Convert.ToDouble(e.Row.Cells[i].Value);
                    if (value < -15)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                    }
                    else if (value > 15)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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

        #region Обработчики справочного грида

        protected void SummaryGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0039_0003_summaryGrid");
            dtSummaryGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtSummaryGrid);
            if (dtSummaryGrid.Rows.Count > 0 && dtGrid.Columns.Count > 1)
            {
                SummaryGrid.DataSource = dtSummaryGrid;
            }
            else
            {
                SummaryGrid.DataSource = null;
            }
        }

        protected void SummaryGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count < 1)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(350);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                int widthColumn = 100;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, GetDateQuarterText(selectedQuarterIndex - 1), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, GetDateQuarterText(selectedQuarterIndex), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Отклонение", "");
        }

        protected void SummaryGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int nameColumnIndex = 0;

                string indicatorName = String.Empty;
                if (e.Row.Cells[nameColumnIndex].Value != null)
                {
                    indicatorName = e.Row.Cells[nameColumnIndex].Value.ToString();
                }

                bool integerFormat = indicatorName == "Максимально возможная оценка качества, балл";

                if (i != 0 && e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = integerFormat
                                            ? Convert.ToDouble(e.Row.Cells[i].Value).ToString("N0")
                                            : Convert.ToDouble(e.Row.Cells[i].Value).ToString("N2");
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

        protected void SummaryGrid_DataBound(object sender, EventArgs e)
        {
            SummaryGrid.Height = Unit.Empty;
            SummaryGrid.Width = Unit.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0039_0003_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        row[i] = row[i].ToString().Replace("муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("Муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("\"", "'");
                        row[i] = row[i].ToString().Replace(" район", " р-н");
                    }
                }
            }

            //UltraChart.DataSource = dtChart;

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
            string query = DataProvider.GetQueryText("FO_0039_0003_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);

            avgValue = GetDoubleDTValue(dtChartAVG, "Средняя оценка уровня качества");
            beginQualityLimit = GetDoubleDTValue(dtChartAVG, "Начальная граница интервала");
            endQualityLimit = GetDoubleDTValue(dtChartAVG, "Конечная граница интервала");
        }

        private int GetQualityDegree(double value)
        {
            if (value > beginQualityLimit)
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
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
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
                        if (value > beginQualityLimit)
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
                e.SceneGraph.Add(line);

                double endLineAxisX = xAxis.Map(2 * endQualityIndex + 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)endLineAxisX, (int)yMin);
                line.p2 = new Point((int)endLineAxisX, (int)yMax - textHeight);
                e.SceneGraph.Add(line);
                
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

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;

            if (e.CurrentWorksheet.Name == "sheet3")
            {
                UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
                UltraChart.Legend.Margins.Right = 5;
                UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[2].Cells[0], UltraChart);
            }
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            if (e.CurrentWorksheet.Name == "sheet3")
            {
                return;
            }

            int columnCount = exportGrid.Columns.Count;

            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "";

                switch (i)
                {
                    case 1:
                    case 2:
                        {
                            formatString = (exportGrid == UltraWebGrid) ? "#0;[Red]-#0" : "#,##0.00;[Red]-#,##0.00"; ;
                            break;
                        }
                    case 3:
                        {
                            formatString = (exportGrid == UltraWebGrid) ? "#0;[Red]-#0" : "#,##0.00;[Red]-#,##0.00"; ;
                            break;
                        }
                    case 4:
                    case 5:
                    case 7:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            break;
                        }
                    case 6:
                    case 8:
                    case 9:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }

            e.CurrentWorksheet.Columns[0].Width = (exportGrid == UltraWebGrid) ? 150 * 37 : 700 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = (exportGrid == UltraWebGrid) ? 50 * 37 : 20*37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private UltraWebGrid exportGrid = new UltraWebGrid();

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            if (IsYearCompare)
            {
                exportGrid = UltraWebGrid;

                Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
                UltraGridExporter1.ExcelExporter.Export(exportGrid, sheet1);

                exportGrid = SummaryGrid;
                Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
                UltraGridExporter1.ExcelExporter.Export(SummaryGrid, sheet2);
            }
            
            exportGrid = emptyExportGrid;
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid, sheet3);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (exportGrid == UltraWebGrid)
            {
                e.HeaderText = exportGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            Report report = new Report();
            ISection section = report.AddSection();

            if (IsYearCompare)
            {
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section);
                section.AddPageBreak();
                UltraGridExporter1.PdfExporter.Export(SummaryGrid, section);
            }
            else
            {
                UltraGridExporter1.PdfExporter.Export(emptyExportGrid, section);
            }
        }
        
        private bool titleAdded = false;
        
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
            }

            if (!titleAdded)
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
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(chart1Label.Text);

                UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
                UltraChart.Legend.Margins.Right = 5;
                Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
                e.Section.AddImage(img);

                if (IsYearCompare)
                {
                    e.Section.AddPageBreak();
                }
            }

            titleAdded = true;
        }

        #endregion
    }
}
