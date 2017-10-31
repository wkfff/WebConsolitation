using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
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
using System.IO;
using System.Drawing.Imaging;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0001
{
    public partial class DefaultOKVD : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private int firstYear = 2000;
        private int endYear = 2011;
        private bool fns28nSplitting;

        private DateTime currentDate;

        #endregion

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;
        // уровень бюджета
        private CustomParam budgetLevel;
        // выбранная мера
        private CustomParam selectedMeasure;
        // группа доходов
        private CustomParam fnsKDGroup;
        // доходы ВСЕГО
        private CustomParam incomesTotal;
        // элемент "Нет кода доходов" для диаграммы 1
        private CustomParam noCodeOKVDItem1;
        // элемент "Нет кода доходов" для диаграммы 2
        private CustomParam noCodeOKVDItem2;
        // элемент "Нет кода доходов" для таблицы
        private CustomParam gridNoCodeOKVDItem;
        // уровень МР и ГО
        private CustomParam regionsLevel;

        #endregion

        private bool AbsoluteMeasureSelected
        {
            get { return AbsobuteMeasure.Checked; }
        }

        private bool HideNoCodeOKVD1Selected
        {
            get { return HideNoCodeOKVD1.Checked; }
        }

        private bool HideNoCodeOKVD2Selected
        {
            get { return HideNoCodeOKVD2.Checked; }
        }

        private bool HideGridNoCodeOKVDSelected
        {
            get { return HideGridNoCodeOKVD.Checked; }
        }

        private bool UseConsolidateRegionBudget
        {
            get { return useConsolidateRegionBudget.Checked; }
        }

        private bool regionSelected = false;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 25);
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
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 3;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 11;
            UltraChart1.Legend.Font = new Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.TitleLeft.Text = "Тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N2> тыс.руб.";
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph1);

            #endregion

            #region Настройка диаграммы 2

            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Axis.X.Extent = 160;
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart2.Axis.Y.Extent = 70;
            UltraChart2.Axis.Y.Labels.ItemFormatString = AbsoluteMeasureSelected ? "<DATA_VALUE:N0>" : "<DATA_VALUE:P2>";
            UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart2.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart2.Width.Value) / 4;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 9;
            UltraChart2.Legend.Font = new Font("Verdana", 8);

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = UltraChart2.Axis.X.Extent;
            UltraChart2.TitleLeft.Text = AbsoluteMeasureSelected ? "Тыс.руб." : " ";
            UltraChart2.TitleLeft.Font = new Font("Verdana", 8);

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
            if (fnsKDGroup == null)
            {
                fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (noCodeOKVDItem1 == null)
            {
                noCodeOKVDItem1 = UserParams.CustomParam("no_code_okvd_item1");
            }
            if (noCodeOKVDItem2 == null)
            {
                noCodeOKVDItem2 = UserParams.CustomParam("no_code_okvd_item2");
            }
            if (gridNoCodeOKVDItem == null)
            {
                gridNoCodeOKVDItem = UserParams.CustomParam("grid_no_code_okvd_item");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            RegionsLink.Visible = true;
            RegionsLink.Text = "См.&nbsp;также&nbsp;муницип.районам&nbsp;и&nbsp;гор.округам";
            RegionsLink.NavigateUrl = GetReportFullName("ФНС_0001_0001");

            OKVDLink.Visible = true;
            OKVDLink.Text = "По&nbsp;доходным источникам";
            OKVDLink.NavigateUrl = GetReportFullName("ФНС_0001_0001_KD");

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

            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;

            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.FNSOKVEDGovernment.Value = RegionSettingsHelper.Instance.FNSOKVEDGovernment;
            UserParams.FNSOKVEDHousehold.Value = RegionSettingsHelper.Instance.FNSOKVEDHousehold;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                AbsobuteMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", RelativeMeasure.ClientID));
                RelativeMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", AbsobuteMeasure.ClientID));

                gridtWebAsyncPanel.AddRefreshTarget(UltraWebGrid);
                gridtWebAsyncPanel.AddLinkedRequestTrigger(HideGridNoCodeOKVD.ClientID);

                chartWebAsyncPanel1.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel2.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(AbsobuteMeasure.ClientID);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(RelativeMeasure.ClientID);
                chartWebAsyncPanel1.AddLinkedRequestTrigger(HideNoCodeOKVD1.ClientID);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(HideNoCodeOKVD2.ClientID);

                currentDate = fns28nSplitting ? CubeInfoHelper.Fns28nSplitInfo.LastDate : CubeInfoHelper.Fns28nNonSplitInfo.LastDate;
                endYear = currentDate.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 90;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(currentDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 120;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month)), true);

                ComboKD.Width = 280;
                ComboKD.Title = "Вид налога";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullFNSKDIncludingList());
                ComboKD.SetСheckedState("Налоговые доходы ", true);

                if (fns28nSplitting)
                {
                    ComboBudgetLevel.Visible = true;
                    ComboBudgetLevel.Title = "Бюджет";
                    ComboBudgetLevel.Width = 400;
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

            Page.Title = "Прирост недоимки по видам деятельности";
            PageTitle.Text = Page.Title;

            Chart1BudgetLevelCaption.Text = ComboBudgetLevel.SelectedValue;
            Chart2BudgetLevelCaption.Text = ComboBudgetLevel.SelectedValue;

            regionSelected = false;

            if (fns28nSplitting)
            {
                selectedRegion.Value = DataDictionariesHelper.FullBudgetRegionUniqNames[ComboBudgetLevel.SelectedValue];
                string level = DataDictionariesHelper.FullBudgetLevelUniqNames[ComboBudgetLevel.SelectedValue];
                if (level.Contains("Бюджет района"))
                {
                    regionSelected = true;
                    if (UseConsolidateRegionBudget)
                    {
                        // для районов берем Конс.бюджет МО
                        level = String.Format("{0}.Parent.Parent", level);
                    }
                }

                // чтобы для 2005 версии не заменяло еще раз
                budgetLevel.Value = level.Replace("[Уровни бюджетов].[Уровни бюджетов]", "[Уровни бюджетов]");
            }
            else
            {
                if (ComboBudgetLevel.SelectedValue == "Все территории")
                {
                    selectedRegion.Value = string.Format("{0}.[Все районы]", RegionSettingsHelper.Instance.RegionDimension);
                }
                else
                {
                    selectedRegion.Value = RegionsNamingHelper.LocalSettlementUniqueNames[ComboBudgetLevel.SelectedValue];
                }
            }

            fnsKDGroup.Value = ComboKD.SelectedValue;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            chartLabel1.Text = string.Format("Прирост/снижение недоимки по видам деятельности с начала {0} года", ComboYear.SelectedValue);
            chartLabel2.Text = "Оценка роста/снижения недоимки по видам деятельности в сравнении с прошлым годом";

            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            string selectedBudget = UseConsolidateRegionBudget && regionSelected
                       ? String.Format("{0} (Консолидированный бюджет МО)", ComboBudgetLevel.SelectedValue)
                       : String.Format("{0}", ComboBudgetLevel.SelectedValue);


            PageSubTitle.Text = string.Format("{3} ({4}) за {0} {1} {2} года",
                                              currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year,
                                              selectedBudget,
                                              ComboKD.SelectedValue.TrimEnd(' '));

            checkboxTR.Visible = regionSelected;
            useConsolidateRegionBudget.Visible = regionSelected;

            selectedMeasure.Value = AbsoluteMeasureSelected ? "Прирост/снижение в тыс.руб." : "Прирост/снижение в %";
            noCodeOKVDItem1.Value = HideNoCodeOKVD1Selected ? "[ОКВЭД].[Сопоставимый].[Нет кода ОКВЭД]" : " ";
            noCodeOKVDItem2.Value = HideNoCodeOKVD2Selected ? "[ОКВЭД].[Сопоставимый].[Нет кода ОКВЭД]" : " ";
            gridNoCodeOKVDItem.Value = HideGridNoCodeOKVDSelected ? "[ОКВЭД].[Сопоставимый].[Нет кода ОКВЭД]" : " ";

            if (!chartWebAsyncPanel1.IsAsyncPostBack && !chartWebAsyncPanel2.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
            }

            if (!gridtWebAsyncPanel.IsAsyncPostBack)
            {
                UltraChart1.DataBind();
                UltraChart2.DataBind();
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0001_0001_gridOKVD_split" : "FNS_0001_0001_gridOKVD";
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

            if (e.Layout.Bands[0].Columns.Count > 13)
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
                SetColumnParams(e.Layout, 0, 8, "N2", width, false);
                SetColumnParams(e.Layout, 0, 9, "N2", width, false);

                SetColumnParams(e.Layout, 0, 10, "P2", width, false);
                SetColumnParams(e.Layout, 0, 11, "P2", width, false);
                SetColumnParams(e.Layout, 0, 12, "P2", width, false);

                SetColumnParams(e.Layout, 0, 13, "", width, true);

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i != 4 && i != 5 && i < 7)
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
                int lastYear = year - 1;
                int nextMonth = month;
                int nextYear = year;
                if (nextMonth == 12)
                {
                    nextMonth = 1;
                    nextYear++;
                    lastYear++;
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
                    string.Format(" тыс.руб."),
                    "Прирост недоимки с начала года в тыс.руб.");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5,
                    string.Format("%"),
                    "Прирост недоимки с начала года в %");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                    string.Format("Темп роста недоимки к {0} году, %", year - 1),
                    "Темп роста недоимки по отношению к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7,
                    string.Format("на 01.{0:00}.{1}", nextMonth, lastYear),
                    string.Format("Общий объем поступлений налоговых доходов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), lastYear));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8,
                    string.Format("на 01.{0:00}.{1}", nextMonth, nextYear),
                    string.Format("Общий объем поступлений налоговых доходов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "отклонение", "отклонение общего объема поступлений налоговых доходов");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10,
                    string.Format("на 01.{0:00}.{1}", nextMonth, lastYear),
                    string.Format("Удельный вес недоимки в общем объеме налоговых доходов по данным ежемесячного отчета об исполнении бюджетов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), lastYear));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11,
                    string.Format("на 01.{0:00}.{1}", nextMonth, nextYear),
                    string.Format("Удельный вес недоимки в общем объеме налоговых доходов по данным ежемесячного отчета об исполнении бюджетов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12, "отклонение", "отклонение удельного веса недоимки в общем объеме налоговых доходов");

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = string.Format("Прирост недоимки с начала {0} года", year);
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 4;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                ch = new ColumnHeader(true);
                ch.Caption = "Общий объем поступлений налоговых доходов, тыс.руб.";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 7;
                ch.RowLayoutColumnInfo.SpanX = 3;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                ch = new ColumnHeader(true);
                ch.Caption = "Удельный вес недоимки в общем объеме поступлений налоговых доходов, %";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 10;
                ch.RowLayoutColumnInfo.SpanX = 3;
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
                bool positiveRedValue = (i == 4 || i == 5 || i == 12);
                bool negativeRedValue = (i == 9);
                int levelColumnIndex = 13;

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].ToString() == "Всего налоговые доходы")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                if (positiveRedValue && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.ForeColor = Color.Red;
                    }
                }

                if (negativeRedValue && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
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

                if (e.Row.Cells[levelColumnIndex].Value != null && e.Row.Cells[levelColumnIndex].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[levelColumnIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "(All)":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Раздел":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Подраздел":
                            {
                                fontSize = 10;
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
            string queryName = fns28nSplitting ? "FNS_0001_0001_chartOKVD1_split" : "FNS_0001_0001_chartOKVD1";
            string query = DataProvider.GetQueryText(queryName);
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            foreach (DataRow row in dtChart1.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string fullKDName = row[i].ToString();
                        fullKDName = fullKDName.TrimEnd(' ');
                        string kdName = fullKDName.Replace("г, Д", "г,\nД");
                        kdName = kdName.Replace("и предметов", "и\nпредметов");
                        if (DataDictionariesHelper.ShortOKVDNames.ContainsKey(fullKDName))
                        {
                            kdName = string.Format("{0} ({1})", kdName, DataDictionariesHelper.GetShortOKVDName(fullKDName));
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
                    text.labelStyle.WrapText = fns28nSplitting ? true : false;

                    string kdName = text.GetTextString();
                    string[] strs = kdName.Split('(');
                    kdName = strs[0].TrimEnd(' ');
                    kdName = kdName.Replace("\n", " ");
                    text.SetTextString(DataDictionariesHelper.GetShortOKVDName(kdName));
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
            string queryName = fns28nSplitting ? "FNS_0001_0001_chartOKVD2_split" : "FNS_0001_0001_chartOKVD2";
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
                        row[i] = DataDictionariesHelper.GetShortOKVDName(kdName.TrimEnd(' '));
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
                    text.labelStyle.WrapText = fns28nSplitting ? true : false;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        double value = Convert.ToDouble(box.Value);

                        string kdName = box.Series.Label;
                        if (DataDictionariesHelper.ShortOKVDNames.ContainsValue(kdName))
                        {
                            string fullName = DataDictionariesHelper.GetFullOKVDName(kdName);
                            fullName = fullName.Replace("г, Д", "г, \nД");
                            fullName = fullName.Replace("и предметов", "и\nпредметов");
                            kdName = string.Format("{0} ({1})", fullName, kdName);
                        }

                        if (value > 0)
                        {
                            box.DataPoint.Label = string.Format("{0}\nРост недоимки в сравнении с аналогичным периодом прошлого года\n{1}",
                                kdName,
                                AbsoluteMeasureSelected ? string.Format("{0:N2} тыс.руб.", value) : string.Format("{0:P2}", value));
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Maroon;
                        }
                        else
                        {
                            box.DataPoint.Label = string.Format("{0}\nСнижение недоимки в сравнении с аналогичным периодом прошлого года\n{1}",
                                kdName,
                                AbsoluteMeasureSelected ? string.Format("{0:N2} тыс.руб.", value) : string.Format("{0:P2}", value));
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
            //    e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            //   e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
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
            e.CurrentWorksheet.Columns[9].Width = width * 37;
            e.CurrentWorksheet.Columns[10].Width = width * 37;
            e.CurrentWorksheet.Columns[11].Width = width * 37;
            e.CurrentWorksheet.Columns[12].Width = width * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "[Red]#,##0.00;-#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "[Red]#,##0.00%;-#,##0.00%";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "[Red]#,##0.00%;-#,##0.00%";

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
            if (e.CurrentColumnIndex >= 4 && e.CurrentColumnIndex != 6)
            {
                int year = Convert.ToInt32(ComboYear.SelectedValue);
                if (e.CurrentColumnIndex >= 4 && e.CurrentColumnIndex <= 5)
                {
                    e.HeaderText = string.Format("Прирост недоимки с начала {0} года", year);
                }
                else if (e.CurrentColumnIndex >= 7 && e.CurrentColumnIndex <= 9)
                {
                    e.HeaderText = "Общий объем поступлений налоговых доходов, тыс.руб.";
                }
                else if (e.CurrentColumnIndex >= 10 && e.CurrentColumnIndex <= 12)
                {
                    e.HeaderText = "Удельный вес недоимки в общем объеме поступлений налоговых доходов, %";
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            sheet1.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet1.Rows[1].Cells[0].Value = PageSubTitle.Text;

            sheet2.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet2.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet2.Rows[2].Cells[0].Value = chartLabel1.Text;

            sheet3.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet3.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet3.Rows[2].Cells[0].Value = chartLabel2.Text;

            UltraGridExporter.ChartExcelExport(sheet2.Rows[3].Cells[0], UltraChart1);
            UltraGridExporter.ChartExcelExport(sheet3.Rows[3].Cells[0], UltraChart2);

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1, 3, 0);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 3;
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

            UltraGridExporter1.GridElementCaption = PageSubTitle.Text;
            //title = e.Section.AddText();
            //font = new Font("Verdana", 14);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartLabel2.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            MemoryStream imageStream = new MemoryStream();
            UltraChart2.SaveTo(imageStream, ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image image = (new Bitmap(imageStream)).ScaleImageIg(2);
            e.Section.AddImage(image);

            e.Section.AddPageBreak();

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartLabel1.Text);

            title = e.Section.AddText();

            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
            imageStream = new MemoryStream();
            UltraChart1.SaveTo(imageStream, ImageFormat.Png);
            image = (new Bitmap(imageStream)).ScaleImageIg(2);
            e.Section.AddImage(image);
        }

        #endregion
    }
}