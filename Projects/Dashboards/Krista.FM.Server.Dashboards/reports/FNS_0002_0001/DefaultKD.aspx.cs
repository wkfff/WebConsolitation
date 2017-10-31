using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0002_0001
{
    public partial class DefaultKD : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private int firstYear = 2000;
        private int endYear = 2011;
        private bool fns28nSplitting;

        #endregion

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;
        // уровень бюджета
        private CustomParam budgetLevel;
        // выбранная мера
        private CustomParam selectedMeasure;

        #endregion

        private bool AbsoluteMeasureSelected
        {
            get { return AbsobuteMeasure.Checked; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45 - 100);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.52 - 95);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 90);

            #region Настройка диаграммы 1

            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 11;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) / 3;
            UltraChart1.TitleLeft.Text = "Тыс.руб.";

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N2> тыс.руб.";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph1);

            #endregion

            #region Настройка диаграммы 2

            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Axis.X.Extent = 160;
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
//            UltraChart2.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
//            UltraChart2.Axis.X.StripLines.PE.FillOpacity = 150;
//            UltraChart2.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
//            UltraChart2.Axis.X.StripLines.Interval = 2;
//            UltraChart2.Axis.X.StripLines.Visible = true;
            UltraChart2.Axis.Y.Extent = 65;
            UltraChart2.Axis.Y.Labels.ItemFormatString = AbsoluteMeasureSelected ? "<DATA_VALUE:N0>" : "<DATA_VALUE:P2>";

            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart2.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart2.Width.Value) / 4;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 9;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart2.Height.Value) / 4;
            UltraChart2.TitleLeft.Text = AbsoluteMeasureSelected ? "Тыс.руб." : " ";

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph2);

            #endregion

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (budgetLevel == null)
            {
                budgetLevel = UserParams.CustomParam("budget_level");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }

            #endregion

            RegionsLink.Visible = true;
            RegionsLink.Text = "См.&nbsp;также&nbsp;муницип.районам&nbsp;и&nbsp;гор.округам";
            RegionsLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultRegions.aspx";

            OKVDLink.Visible = true;
            OKVDLink.Text = "По&nbsp;ОКВЭД";
            OKVDLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultOKVD.aspx";

            AllocationLink.Visible = true;
            AllocationLink.Text = "Диаграмма&nbsp;распределения";
            AllocationLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultAllocation.aspx";

            SettlementLink.Visible = true;
            SettlementLink.Text = "По&nbsp;поселениям";
            SettlementLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultSettlement.aspx";

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("FNS28nSplitting"));

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                AbsobuteMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", RelativeMeasure.ClientID));
                RelativeMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", AbsobuteMeasure.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel.AddLinkedRequestTrigger(AbsobuteMeasure.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(RelativeMeasure.ClientID);

                dtDate = new DataTable();
                string queryName = fns28nSplitting ? "FNS_0002_0001_date_split" : "FNS_0002_0001_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                if (fns28nSplitting)
                {
                    ComboBudgetLevel.Visible = true;
                    ComboBudgetLevel.Title = "Бюджет";
                    ComboBudgetLevel.Width = 500;
                    ComboBudgetLevel.MultiSelect = false;
                    ComboBudgetLevel.ParentSelect = true;
                    ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillFullBudgetLevels(
                        DataDictionariesHelper.FullBudgetLevelNumbers, 
                        DataDictionariesHelper.FullBudgetLevelUniqNames, 
                        DataDictionariesHelper.FullBudgetRegionUniqNames));
                    ComboBudgetLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
                }
                else
                {
                    ComboBudgetLevel.Visible = true;
                    ComboBudgetLevel.Title = "Территории";
                    ComboBudgetLevel.Width = 280;
                    ComboBudgetLevel.MultiSelect = false;
                    ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSettlements(RegionsNamingHelper.LocalSettlementTypes, true));
                    ComboBudgetLevel.SetСheckedState("Все территории", true);
                }
            }

            Page.Title = "Прирост недоимки по доходным источникам";
            PageTitle.Text = Page.Title;

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                if (fns28nSplitting)
                {
                    selectedRegion.Value = DataDictionariesHelper.FullBudgetRegionUniqNames[ComboBudgetLevel.SelectedValue];
                    budgetLevel.Value = DataDictionariesHelper.FullBudgetLevelUniqNames[ComboBudgetLevel.SelectedValue];
                }
                else
                {
                    if (ComboBudgetLevel.SelectedValue == "Все территории")
                    {
                        selectedRegion.Value = string.Format("[Районы].[Сопоставимый].[Все районы]");
                    }
                    else
                    {
                        selectedRegion.Value = RegionsNamingHelper.LocalSettlementUniqueNames[ComboBudgetLevel.SelectedValue];
                    }
                }

                int year = Convert.ToInt32(ComboYear.SelectedValue);
                UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);

                chartLabel1.Text = string.Format("Прирост/снижение недоимки по видам налогов с начала {0} года", ComboYear.SelectedValue);
                chartLabel2.Text = "Распределение территорий по росту/снижению недоимки по видам налогов в сравнении с прошлым годом";

                int month = ComboMonth.SelectedIndex + 1;
                UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
                UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
                UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                                                            UserParams.PeriodHalfYear.Value,
                                                            UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

                PageSubTitle.Text = string.Format("{3} за {0} {1} {2} года",
                                                  month, CRHelper.RusManyMonthGenitive(month), year,
                                                  ComboBudgetLevel.SelectedValue);

                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                UltraChart1.DataBind();
            }

            selectedMeasure.Value = AbsoluteMeasureSelected ? "Прирост/снижение в тыс.руб." : "Прирост/снижение в %";
            UltraChart2.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0002_0001_gridKD_split" : "FNS_0002_0001_gridKD";
            string query = DataProvider.GetQueryText(queryName);
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Регион", dtGrid);

            UltraWebGrid.DataSource = dtGrid;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count > 9)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(320);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

                int width = 104;
                int rankWidth = 80;

                SetColumnParams(e.Layout, 0, 1, "N2", width, false);
                SetColumnParams(e.Layout, 0, 2, "N2", width, false);
                SetColumnParams(e.Layout, 0, 3, "N2", width, false);
                SetColumnParams(e.Layout, 0, 4, "N2", width, false);
                SetColumnParams(e.Layout, 0, 5, "P2", rankWidth, false);
                SetColumnParams(e.Layout, 0, 6, "P2", rankWidth, false);
                SetColumnParams(e.Layout, 0, 7, "N2", width, false);
                SetColumnParams(e.Layout, 0, 8, "P2", width, false);
                SetColumnParams(e.Layout, 0, 9, "", width, true);
                
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i != 4 && i != 5)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }
                
                int month = ComboMonth.SelectedIndex + 1;
                int year = Convert.ToInt32(ComboYear.SelectedValue);
                int nextMonth = month;
                int nextYear = year;
                if (nextMonth == 12)
                {
                    nextMonth = 1;
                    nextYear++;
                }
                else
                {
                    nextMonth++;
                }

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Доходы", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, 
                    string.Format("Недоимка на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear - 1),
                    string.Format("Недоимка за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year - 1));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2,
                    string.Format("Недоимка на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear),
                    string.Format("Недоимка за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3,
                    string.Format("Недоимка на начало {0} года, тыс.руб.", year),
                    "Недоимка на начало года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4,
                    string.Format("тыс.руб."),
                    "Прирост недоимки с начала года в тыс.руб.");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5,
                    string.Format("%"),
                    "Прирост недоимки с начала года в %");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                    string.Format("Темп роста недоимки к {0} году, %", year - 1),
                    "Темп роста недоимки по отношению к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7,
                    string.Format("Общий объем поступлений налоговых доходов на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear),
                    string.Format("Общий объем поступлений налоговых доходов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8,
                    string.Format("Удельный вес недоимки в общем объеме поступлений налоговых доходов, %"),
                    string.Format("Удельный вес недоимки в общем объеме налоговых доходов по данным ежемесячного отчета об исполнении бюджетов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = string.Format("Прирост недоимки с начала {0} года", year);
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 4;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool grow = (i == 5);
                bool growRate = (i == 6);
                bool redValue = (i == 4 || i == 5);
                int levelColumnIndex = 9;

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].ToString() == "Всего налоговые доходы")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                if (redValue && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.ForeColor = Color.Red;
                    }

                }

                if (grow && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {

                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i].Title = "Снижение недоимки";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        e.Row.Cells[i].Title = "Прирост недоимки";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (growRate && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {

                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i].Title = "Уменьшение недоимки по сравнению с аналогичным периодом прошлого года";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        e.Row.Cells[i].Title = "Увеличение недоимки по сравнению с аналогичным периодом прошлого года";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[levelColumnIndex] != null && e.Row.Cells[levelColumnIndex].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[levelColumnIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "(All)":
                            {
                                fontSize = 9;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Подгруппа":
                            {
                                fontSize = 8;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Статья":
                            {
                                fontSize = 8;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding1(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0002_0001_chartKD1_split" : "FNS_0002_0001_chartKD1";
            string query = DataProvider.GetQueryText(queryName);
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            foreach (DataRow row in dtChart1.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string kdName = row[i].ToString();
                        kdName = kdName.TrimEnd(' ');
                        if (DataDictionariesHelper.ShortKDNames.ContainsKey(kdName))
                        {
                            kdName = string.Format("{0} ({1})", kdName, DataDictionariesHelper.GetShortKDName(kdName));
                        }
                        row[i] = kdName;
                    }
                }
            }

//            UltraChart1.Series.Clear();
//            for (int i = 1; i < dtChart1.Columns.Count; i++)
//            {
//                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
//                series.Label = dtChart1.Columns[i].ColumnName;
//                UltraChart1.Series.Add(series);
//            }

            UltraChart1.DataSource = dtChart1;
        }

        protected void UltraChart_FillSceneGraph1(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 30;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;

                    string kdName = text.GetTextString();
                    string[] strs = kdName.Split('(');
                    kdName = strs[0].TrimEnd(' ');
                    text.SetTextString(DataDictionariesHelper.GetShortKDName(kdName));
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        switch (box.DataPoint.Label)
                        {
                            case "Недоимка на начало года":
                                {
                                    box.PE.ElementType = PaintElementType.Gradient;
                                    box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                    box.PE.Fill = Color.Yellow;
                                    box.PE.FillStopColor = Color.Goldenrod;
                                    box.PE.FillOpacity = 250;
                                    break;
                                }
                            case "Прирост недоимки":
                                {
                                    box.PE.ElementType = PaintElementType.Gradient;
                                    box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                    box.PE.Fill = Color.Red;
                                    box.PE.FillStopColor = Color.DarkRed;
                                    box.PE.FillStopOpacity = 250;
                                    break;
                                }
                            case "Снижение недоимки":
                                {
                                    box.PE.ElementType = PaintElementType.Gradient;
                                    box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                    box.PE.Fill = Color.LimeGreen;
                                    box.PE.FillStopColor = Color.ForestGreen;
                                    box.PE.FillOpacity = 250;
                                    break;
                                }
                        }
                    }
                    else if (box.Path.Contains("Legend") && i != 0)
                    {
                        Primitive postPrimitive = e.SceneGraph[i + 1];
                        if (postPrimitive is Text)
                        {
                            Text text = (Text)postPrimitive;
                            switch (text.GetTextString())
                            {
                                case "Недоимка на начало года":
                                    {
                                        box.PE.ElementType = PaintElementType.Gradient;
                                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                        box.PE.Fill = Color.Yellow;
                                        box.PE.FillStopColor = Color.Goldenrod;
                                        box.PE.FillOpacity = 250;
                                        break;
                                    }
                                case "Прирост недоимки":
                                    {
                                        box.PE.ElementType = PaintElementType.Gradient;
                                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                        box.PE.Fill = Color.DarkRed;
                                        box.PE.FillStopColor = Color.DarkRed;
                                        box.PE.FillStopOpacity = 250;
                                        break;
                                    }
                                case "Снижение недоимки":
                                    {
                                        box.PE.ElementType = PaintElementType.Gradient;
                                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                        box.PE.Fill = Color.LimeGreen;
                                        box.PE.FillStopColor = Color.ForestGreen;
                                        box.PE.FillOpacity = 250;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        }

        protected void UltraChart_DataBinding2(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0002_0001_chartKD2_split" : "FNS_0002_0001_chartKD2";
            string query = DataProvider.GetQueryText(queryName);
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            foreach (DataRow row in dtChart2.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string kdName = row[i].ToString();
                        row[i] = DataDictionariesHelper.GetShortKDName(kdName.TrimEnd(' '));
                    }
                }
            }

            UltraChart2.DataSource = dtChart2;
        }

        void UltraChart_FillSceneGraph2(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 30;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    int year = Convert.ToInt32(ComboYear.SelectedValue);
                    if (box.DataPoint != null && box.Value != null)
                    {
                        double value = Convert.ToDouble(box.Value);

                        string kdName = box.Series.Label;
                        if (DataDictionariesHelper.ShortKDNames.ContainsValue(kdName))
                        {
                            kdName = string.Format("{0} ({1})", DataDictionariesHelper.GetFullKDName(kdName), kdName);
                        }

                        if (value > 0)
                        {
                            box.DataPoint.Label = string.Format("{0}\nРост недоимки к {2} году\n{1}",
                                kdName,
                                AbsoluteMeasureSelected ? string.Format("{0:N2} тыс.руб.", value) : string.Format("{0:P2}", value),
                                year - 1);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Maroon;
                        }
                        else
                        {
                            box.DataPoint.Label = string.Format("{0}\nСнижение недоимки к {2} году\n{1}",
                                kdName,
                                AbsoluteMeasureSelected ? string.Format("{0:N2} тыс.руб.", value) : string.Format("{0:P2}", value),
                                year - 1);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                    }
                    else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                    {
                        box.PE.ElementType = PaintElementType.CustomBrush;
                        LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Red, Color.Green, 45, false);
                        box.PE.CustomBrush = brush;
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int width = 107;

            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            e.CurrentWorksheet.Columns[1].Width = width * 37;
            e.CurrentWorksheet.Columns[2].Width = width * 37;
            e.CurrentWorksheet.Columns[3].Width = width * 37;
            e.CurrentWorksheet.Columns[4].Width = width * 37;
            e.CurrentWorksheet.Columns[5].Width = width * 37;
            e.CurrentWorksheet.Columns[6].Width = width * 37;
            e.CurrentWorksheet.Columns[7].Width = width * 37;
            e.CurrentWorksheet.Columns[8].Width = width * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;

            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 20 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (e.CurrentColumnIndex == 4 || e.CurrentColumnIndex == 5)
            {
                int year = Convert.ToInt32(ComboYear.SelectedValue);
                e.HeaderText = string.Format("Прирост недоимки с начала {0} года", year);
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
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
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartLabel2.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartLabel1.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
        }

        #endregion
    }
}