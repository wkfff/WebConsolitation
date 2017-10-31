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
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0001_Yamal
{
    public partial class DefaultRegions : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private int firstYear = 2000;
        private int endYear = 2011;
        private bool fns28nSplitting;
        private bool regionFilterEnable;
        private bool kdMeasureFilterEnable;
        private DataTable dtMonthReportGrid;

        private DateTime currentDate;

        #endregion
        
        #region Параметры запроса

        // уровень МР и ГО
        private CustomParam regionsLevel;
        // группа доходов
        private CustomParam fnsKDGroup;
        // доходы ВСЕГО
        private CustomParam incomesTotal;
        // множество МО для 1й диаграммы
        private CustomParam chart1RegionSet;
        // множество МО для 2й диаграммы
        private CustomParam chart2RegionSet;
        // множество МО для грида
        private CustomParam gridRegionSet;
        // выбранная мера
        private CustomParam selectedMeasure;

        // фильтр для мер элементом КД
        private CustomParam kdMeasureFilter;
        #endregion

        private bool Chart1DistrictExcluding
        {
            get { return Chart1DistrictExclude.Checked; }
        }

        private bool Chart2DistrictExcluding
        {
            get { return Chart2DistrictExclude.Checked; }
        }

        private bool AbsoluteMeasureSelected
        {
            get { return AbsobuteMeasure.Checked; }
        }

        private string GridRegionsSet
        {
            get
            {
                if (RegionsCheckBox.Checked && DistrictCheckBox.Checked)
                {
                    return "Районы и города";
                }
                else if (RegionsCheckBox.Checked)
                {
                    return "Районы с элементом Итого";
                }
                else
                {
                    return "Города с элементом Итого";
                }
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 18);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45 - 110);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75 - 90);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75 - 90);

            #region Настройка диаграммы 1

            UltraChart1.ChartType = ChartType.StackColumnChart;
//            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart1.Axis.X.Extent = 175;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 3;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 9;
            UltraChart1.Legend.Font = new Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) / 3;
            UltraChart1.TitleLeft.Text = "Тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N2> тыс.руб.";

            UltraChart1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph1);

            #endregion

            #region Настройка диаграммы 2

            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Axis.X.Extent = 170;
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
            UltraChart2.Legend.SpanPercentage = 8;
            UltraChart2.Legend.Font = new Font("Verdana", 8);

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart2.Height.Value) / 4;
            UltraChart2.TitleLeft.Text = AbsoluteMeasureSelected ? "Тыс.руб." : " ";
            UltraChart2.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph2);

            #endregion

            #region Инициализация параметров запроса

            regionsLevel = UserParams.CustomParam("regions_level");
            fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            incomesTotal = UserParams.CustomParam("incomes_total");
            chart1RegionSet = UserParams.CustomParam("chart1_region_set");
            chart2RegionSet = UserParams.CustomParam("chart2_region_set");
            selectedMeasure = UserParams.CustomParam("selected_measure");
            gridRegionSet = UserParams.CustomParam("grid_region_set");
            kdMeasureFilter = UserParams.CustomParam("kd_measure_filter");
            #endregion

            KDLink.Visible = true;
            KDLink.Text = "См.&nbsp;также&nbsp;по&nbsp;доходным&nbsp;источникам";
            KDLink.NavigateUrl = "~/reports/FNS_0001_0001_Yamal/DefaultKD.aspx";

            OKVDLink.Visible = true;
            OKVDLink.Text = "По&nbsp;ОКВЭД";
            OKVDLink.NavigateUrl = "~/reports/FNS_0001_0001/DefaultOKVD.aspx";

            AllocationLink.Visible = true;
            AllocationLink.Text = "Диаграмма&nbsp;распределения";
            AllocationLink.NavigateUrl = "~/reports/FNS_0001_0001/DefaultAllocation.aspx";

            SettlementLink.Visible = true;
            SettlementLink.Text = "По&nbsp;поселениям";
            SettlementLink.NavigateUrl = "~/reports/FNS_0001_0001/DefaultSettlement.aspx";

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

            fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.FNS28nSplitting);
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            regionFilterEnable = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("RegionFilterEnable"));
            kdMeasureFilterEnable = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("KdMeasureFilterEnable"));
            //RegionCenterExclude.Text = RegionSettingsHelper.Instance.GetPropertyValue("RegionCenterCaption");
            
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel1.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(Chart2DistrictExclude.ClientID); 
                
                AbsobuteMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", RelativeMeasure.ClientID));
                RelativeMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", AbsobuteMeasure.ClientID));

                RegionsCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", DistrictCheckBox.ClientID));
                DistrictCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", RegionsCheckBox.ClientID));

                chartWebAsyncPanel2.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(AbsobuteMeasure.ClientID);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(RelativeMeasure.ClientID);
                chartWebAsyncPanel1.AddLinkedRequestTrigger(Chart1DistrictExclude.ClientID);
      
                currentDate = fns28nSplitting ? CubeInfoHelper.Fns28nSplitInfo.LastDate : CubeInfoHelper.Fns28nNonSplitInfo.LastDate;
                endYear = currentDate.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(currentDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month)), true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид налога";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullFNSKDIncludingList());
                ComboKD.SetСheckedState("Налоговые доходы ", true);
           }

            Page.Title = "Прирост недоимки по муниципальным районам и городским округам";
            PageTitle.Text = Page.Title;

            RegionsCheckBox.Visible = regionFilterEnable;
            DistrictCheckBox.Visible = regionFilterEnable;

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", currentDate.Year - 1);

            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", currentDate.Year, UserParams.PeriodHalfYear.Value,
                UserParams.PeriodQuater.Value, CRHelper.RusMonth(currentDate.Month));
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            fnsKDGroup.Value = ComboKD.SelectedValue;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            UserParams.PeriodLastDate.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", currentDate.Year - 1,
                                        UserParams.PeriodHalfYear.Value,
                                        UserParams.PeriodQuater.Value, CRHelper.RusMonth(currentDate.Month));

            chart1Label1.Text = string.Format("Прирост/снижение недоимки по местным бюджетам с начала {0} года", ComboYear.SelectedValue);
            chart1Label2.Text = "Распределение территорий по росту/снижению недоимки по местным бюджетам в сравнении с прошлым годом";
            gridCommentLabel.Text = "* поступления по выбранному в параметре доходному источнику приводятся по данным ежемесячной отчетности об исполнении бюджетов";

            PageSubTitle.Text = string.Format("{3} за {0} {1} {2} года", currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year, ComboKD.SelectedValue.TrimEnd(' '));

            gridRegionSet.Value = GridRegionsSet;

            kdMeasureFilter.Value = kdMeasureFilterEnable
                ? String.Format(",  {0}.[{1}]", RegionSettingsHelper.Instance.IncomesKDDimension, fnsKDGroup.Value) 
                : " ";

            if (!chartWebAsyncPanel1.IsAsyncPostBack && !chartWebAsyncPanel2.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
            }

            if (!chartWebAsyncPanel2.IsAsyncPostBack)
            {
                chart1RegionSet.Value = Chart1DistrictExcluding ? "МО без городов" : "МО с городами";
                UltraChart1.DataBind();
            }

            if (!chartWebAsyncPanel1.IsAsyncPostBack)
            {
                chart2RegionSet.Value = Chart2DistrictExcluding ? "МО без городов" : "МО с городами";
                selectedMeasure.Value = AbsoluteMeasureSelected ? "Прирост/снижение в тыс.руб." : "Прирост/снижение в %";
                UltraChart2.DataBind();
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting
                                   ? "FNS_0001_0001_Yamal_regions_grid_split"
                                   : "FNS_0001_0001_Yamal_regions_grid";
            string query = DataProvider.GetQueryText(queryName);
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Регион", dtGrid);

            const string allIncomesColumnName = "Общий объем поступлений налоговых доходов";
            const string monthArrearsColumnName = "Недоимка на текущий месяц";
            const string densityColumnName = "Удельный вес недоимки в общем объеме поступлений налоговых";
            const string densityRankColumnName = "Ранг по удельному весу недоимки";
            const string densityWorseRankColumnName = "Худший ранг по удельному весу недоимки";

            RankCalculator densityRank = new RankCalculator(RankDirection.Asc);

            if (dtGrid.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FNS_0001_0001_Yamal_regions_grid_monthReport");
                dtMonthReportGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Регион", dtMonthReportGrid);

                FillNullValues(dtGrid, allIncomesColumnName);
                FillNullValues(dtGrid, densityColumnName);
                FillNullValues(dtGrid, densityRankColumnName);
                FillNullValues(dtGrid, densityWorseRankColumnName);

                if (dtMonthReportGrid.Columns.Count > 1 && dtMonthReportGrid.Rows.Count > 0)
                {
                    dtGrid.PrimaryKey = new DataColumn[] {dtGrid.Columns[0]};

                    foreach (DataRow monthReportRow in dtMonthReportGrid.Rows)
                    {
                        if (monthReportRow[0] != DBNull.Value)
                        {
                            string rowName = monthReportRow[0].ToString();
                            if (monthReportRow[allIncomesColumnName] != DBNull.Value &&
                                monthReportRow[allIncomesColumnName].ToString() != String.Empty)
                            {
                                double allIncomes = Convert.ToDouble(monthReportRow[allIncomesColumnName]);

                                DataRow dtRow = dtGrid.Rows.Find(rowName);
                                if (dtRow != null)
                                {
                                    dtRow[allIncomesColumnName] = allIncomes;

                                    if (allIncomes != 0 && dtRow[monthArrearsColumnName] != DBNull.Value &&
                                        dtRow[monthArrearsColumnName].ToString() != String.Empty)
                                    {
                                        double monthArrears = Convert.ToDouble(dtRow[monthArrearsColumnName]);
                                        double density = monthArrears/allIncomes;
                                        dtRow[densityColumnName] = density;
                                        if (IsRankingRow(rowName))
                                        {
                                            densityRank.AddItem(density);
                                        }

                                    }
                                }
                            }
                        }
                    }

                    double densityWorseRank = densityRank.GetWorseRank();

                    // пробегаем еще раз, чтобы расставить ранги
                    foreach (DataRow row in dtGrid.Rows)
                    {
                        string rowName = row[0].ToString();
                        if (IsRankingRow(rowName) && row[densityColumnName] != DBNull.Value &&
                            row[densityColumnName].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[densityColumnName]);
                            int rank = densityRank.GetRank(value);
                            if (rank != 0)
                            {
                                row[densityRankColumnName] = rank;
                                row[densityWorseRankColumnName] = densityWorseRank;
                            }
                        }
                        else
                        {
                            row[densityRankColumnName] = DBNull.Value;
                        }
                    }
                }
                
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i == 0 && row[i] != DBNull.Value)
                        {
                            string value = row[i].ToString();
                            value = value.Replace("Муниципальный район", "МР");
                            value = value.Replace("муниципальный район", "МР");
                            value = value.Replace("\"", "'");
                            row[i] = value.Replace(" район", " р-н");
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
        }

        private static bool IsRankingRow(string rowName)
        {
            return rowName != "Консолидированный бюджет субъекта" && rowName != "Местные бюджеты" &&
                rowName != RegionSettingsHelper.Instance.OwnSubjectBudgetName && rowName != "Итого по ГО" && rowName != "Итого по МР";
        }

        private static void FillNullValues(DataTable dt, string columnName)
        {
            foreach (DataRow row in dt.Rows)
            {
                row[columnName] = DBNull.Value;
            }
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

            if (e.Layout.Bands[0].Columns.Count > 14)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

                int width = 95;
                int percentWidth = 70;
                int rankWidth = 78;

                SetColumnParams(e.Layout, 0, 1, "N2", width, false);
                SetColumnParams(e.Layout, 0, 2, "N2", width, false);
                SetColumnParams(e.Layout, 0, 3, "N2", width, false);
                SetColumnParams(e.Layout, 0, 4, "N2", width, false);
                SetColumnParams(e.Layout, 0, 5, "P2", percentWidth, false);
                SetColumnParams(e.Layout, 0, 6, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 7, "N0", width, true);
                SetColumnParams(e.Layout, 0, 8, "P2", rankWidth, false);
                SetColumnParams(e.Layout, 0, 9, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 10, "N0", width, true);
                SetColumnParams(e.Layout, 0, 11, "N2", width, false);
                SetColumnParams(e.Layout, 0, 12, "P2", width, false);
                SetColumnParams(e.Layout, 0, 13, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 14, "N0", width, true);

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 4 || i == 5)
                    //if (i > 3)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
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

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Территория", "Территория поступления доходов");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1,
                    string.Format("Недоимка на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear - 1),
                    string.Format("Недоимка за {0} {1} {2} года.", month, CRHelper.RusManyMonthGenitive(month), year - 1));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2,
                    string.Format("Недоимка на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear),
                    string.Format("Недоимка за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3,
                    string.Format("Недоимка на начало {0} года, тыс.руб", year),
                    "Недоимка на начало года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4,
                    string.Format("тыс.руб."),
                    "Прирост недоимки с начала года в тыс.руб.");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5,
                    string.Format("%"),
                    "Прирост недоимки с начала года в %");

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                    string.Format("Ранг по приросту недоимки с начала {0} года в %", year),
                    "Ранг по приросту недоимки с начала года в %");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8,
                    string.Format("Темп роста недоимки к {0} году, %", year - 1),
                    "Темп роста недоимки по отношению к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9,
                    string.Format("Ранг по темпу роста недоимки к {0} году", year - 1),
                    "Ранг по темпу роста недоимки по отношению к аналогичному периоду прошлого года");
                // на самом деле это скрытая колонка с худшим рангом по темпу роста недоимки
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10,
                    string.Format("Общий объем поступлений налоговых доходов на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear),
                    string.Format("Общий объем поступлений налоговых доходов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11,
                    String.Format("Объем поступлений* ({2}) на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear, ComboKD.SelectedValue.TrimEnd(' ')),
                    String.Format("Объем поступлений* ({3}) за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year, ComboKD.SelectedValue.TrimEnd(' ')));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12,
                    string.Format("Удельный вес недоимки в общем объеме поступлений налоговых доходов, %"),
                    string.Format("Удельный вес недоимки в общем объеме налоговых доходов по данным ежемесячного отчета об исполнении бюджетов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 13,
                    string.Format("Ранг по удельному весу"),
                   "Ранг по удельному весу недоимки в общем объеме налоговых доходов");

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
                bool growRate = (i == 8);
                bool rank = (i == 6 || i == 9 || i == 13);
                bool redValue = (i == 4 || i == 5);

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].ToString() == "Все территории")
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

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string indicatorName = string.Empty;
                        switch(i)
                        {
                            case 6:
                                {
                                    indicatorName = "прирост недоимки";
                                    break;
                                }
                            case 9:
                                {
                                    indicatorName = "темп роста недоимки";
                                    break;
                                }
                            case 13:
                                {
                                    indicatorName = "удельный вес";
                                    break;
                                }
                        }
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Наименьший {0}", indicatorName);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Наибольший {0}", indicatorName);
                        }

                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") ||
                    e.Row.Cells[0].Value.ToString().ToLower().Contains("итого по")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding1(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0001_0001_Yamal_regions_chart1_split" : "FNS_0001_0001_Yamal_regions_chart1";
            string query = DataProvider.GetQueryText(queryName);
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            foreach (DataRow row in dtChart1.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1 || i == 2 || i == 3)
                         && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }

                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
                        value = value.Replace("городской округ", "ГО");
                        value = value.Replace("Городской округ", "ГО");
                        value = value.Replace("муниципальное образование", "МО");
                        value = value.Replace("Муниципальный район", "МР");
                        value = value.Replace("муниципальный район", "МР");
                        value = value.Replace("\"", "'");
                        row[i] = value.Replace(" район", " р-н");
                    }
                }
            }

            UltraChart1.DataSource = dtChart1;
        }

        void UltraChart_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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
            string queryName = fns28nSplitting ? "FNS_0001_0001_Yamal_regions_chart2_split" : "FNS_0001_0001_Yamal_regions_chart2";
            string query = DataProvider.GetQueryText(queryName);
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            foreach (DataRow row in dtChart2.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
                        value = value.Replace("городской округ", "ГО");
                        value = value.Replace("Городской округ", "ГО");
                        value = value.Replace("муниципальное образование", "МО");
                        value = value.Replace("Муниципальный район", "МР");
                        value = value.Replace("муниципальный район", "МР");
                        value = value.Replace("\"", "'");
                        row[i] = value.Replace(" район", " р-н");
                    }
                }
            }

            UltraChart2.DataSource = dtChart2;
        }

        void UltraChart_FillSceneGraph2(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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
                        if (value > 0)
                        {
                            box.DataPoint.Label = string.Format("{0}\nРост недоимки к {2} году\n{1}",
                                box.Series.Label,
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
                                box.Series.Label,
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
            int width = 107 * 37;

            e.CurrentWorksheet.Columns[1].Width = width;
            e.CurrentWorksheet.Columns[2].Width = width;
            e.CurrentWorksheet.Columns[3].Width = width;
            e.CurrentWorksheet.Columns[4].Width = width;
            e.CurrentWorksheet.Columns[5].Width = width;
            e.CurrentWorksheet.Columns[6].Width = width;
            e.CurrentWorksheet.Columns[7].Width = width;
            e.CurrentWorksheet.Columns[8].Width = width;
            e.CurrentWorksheet.Columns[9].Width = width;
            e.CurrentWorksheet.Columns[10].Width = width;
            e.CurrentWorksheet.Columns[11].Width = width;
            e.CurrentWorksheet.Columns[12].Width = width;
            e.CurrentWorksheet.Columns[13].Width = width;
            e.CurrentWorksheet.Columns[14].Width = width;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "[Red]#,##0.00%;-#,##0.00%";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = "#0";

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

        private static void SetGridParams(UltraWebGrid grid)
        {
            int offset = 0;
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                UltraGridColumn column = grid.Columns[i];
                if (column.Hidden)
                {
                    offset++;
                }

                if (i + offset < grid.Columns.Count)
                {
                    column.Header.Caption = grid.Columns[i + offset].Header.Caption;
                }
            }
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            SetGridParams(UltraWebGrid);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
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
            title.AddContent(chart1Label2.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chart1Label1.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
