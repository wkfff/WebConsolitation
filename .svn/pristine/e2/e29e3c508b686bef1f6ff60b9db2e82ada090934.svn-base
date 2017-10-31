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

namespace Krista.FM.Server.Dashboards.reports.FNS_0003_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private int firstYear = 2005;
        private int endYear = 2011;

        private int currentYear;
        private int currentMonth;

        private Font chartFont = new Font("Verdana", 8);

        #endregion
        
        #region Параметры запроса

        // группа доходов
        private CustomParam fnsKDGroup;
        // областной центр
        private CustomParam regionCenter;
        // выбранная мера
        private CustomParam selectedMeasure;
        // выбранный субъект
        private CustomParam selectedSubjeсt;
        // множество субъектов для диаграммы
        private CustomParam chartSubjectSet;
        // множество субъектов для таблицы
        private CustomParam gridSubjectSet;
        // выбран ли отдельный район
        private CustomParam isRegionSelected;
        // уровень выводимых потомков выбранного региона
        private CustomParam childRegionLevel;

        #endregion

        private bool RegionCenterExcluding
        {
            get { return RegionCenterExclude.Checked; }
        }

        private bool AbsoluteMeasureSelected
        {
            get { return AbsobuteMeasure.Checked; }
        }

        private bool IsUrFO
        {
            get { return ComboRegion.SelectedValue == "Уральский федеральный округ"; }
        }

        private bool IsRegionSelected
        {
            get { return ComboRegion.SelectedNode.Level == 1; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 18);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45 - 110);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 90);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 110);

            #region Настройка диаграммы 1

            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.Y.Extent = 65;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Font = chartFont;

            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 11;
            UltraChart1.Legend.Font = chartFont;
            UltraChart1.Border.Thickness = 0;
            
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.TitleLeft.Font = chartFont;
            UltraChart1.TitleLeft.Text = "Тыс.руб.";

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

            UltraChart2.Axis.Y.Extent = 65;
            UltraChart2.Axis.Y.Labels.ItemFormatString = AbsoluteMeasureSelected ? "<DATA_VALUE:N0>" : "<DATA_VALUE:P2>";
            UltraChart2.Axis.Y.Labels.Font = chartFont;

            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart2.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart2.Width.Value) / 4;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 9;
            UltraChart2.Legend.Font = chartFont;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = UltraChart2.Axis.X.Extent;
            UltraChart2.TitleLeft.Text = AbsoluteMeasureSelected ? "Тыс.руб." : " ";
            UltraChart2.TitleLeft.Font = chartFont;

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph2);

            #endregion

            #region Инициализация параметров запроса

            fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            regionCenter = UserParams.CustomParam("region_center");
            selectedMeasure = UserParams.CustomParam("selected_measure");
            selectedSubjeсt = UserParams.CustomParam("selected_subject");
            chartSubjectSet = UserParams.CustomParam("chart_subject_set");
            isRegionSelected = UserParams.CustomParam("is_region_selected");
            childRegionLevel = UserParams.CustomParam("child_region_level");
            gridSubjectSet = UserParams.CustomParam("grid_subject_set");

            #endregion

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

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel1.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel1.AddLinkedRequestTrigger(RegionCenterExclude.ClientID);

                AbsobuteMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", RelativeMeasure.ClientID));
                RelativeMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", AbsobuteMeasure.ClientID));

                chartWebAsyncPanel2.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(AbsobuteMeasure.ClientID);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(RelativeMeasure.ClientID);

                dtDate = new DataTable();
                string queryName = "FNS_0003_0001_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
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

                ComboRegion.Width = 300;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues((CustomMultiComboDataHelper.FillUrFORegionList(DataDictionariesHelper.UrfoRegionUniqueName, DataDictionariesHelper.UrfoRegionLevels)));
                ComboRegion.Title = "Территория";
                ComboRegion.SetСheckedState("Уральский федеральный округ", true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид налога";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullFNSKDIncludingList());
                ComboKD.SetСheckedState("Налоговые доходы ", true);
           }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            currentMonth = ComboMonth.SelectedIndex + 1;

            Page.Title = String.Format("Прирост недоимки по муниципальным районам и городским округам ({0})", RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("{3} за {0} {1} {2} года", currentMonth, CRHelper.RusManyMonthGenitive(currentMonth), currentYear, ComboKD.SelectedValue.TrimEnd(' '));

            chart1Label1.Text = String.Format("Прирост/снижение недоимки по территориям с начала {0} года", ComboYear.SelectedValue);
            chart1Label2.Text = "Распределение территорий по росту/снижению недоимки в сравнении с прошлым годом";

            RegionCenterExclude.Visible = !IsUrFO && !IsRegionSelected;
            RegionCenterExclude.Text = GetRegionCenterCaption(ComboRegion.SelectedValue);

            if (!chartWebAsyncPanel1.IsAsyncPostBack && !chartWebAsyncPanel2.IsAsyncPostBack)
            {
                UserParams.PeriodYear.Value = currentYear.ToString();
                UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentMonth));
                UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentMonth));
                UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentMonth);

                fnsKDGroup.Value = ComboKD.SelectedValue;
                selectedSubjeсt.Value = IsUrFO
                    ? "[Районы].[Сопоставимый].[Уральский федеральный округ ]"
                    : DataDictionariesHelper.UrfoRegionUniqueName[ComboRegion.SelectedValue];
                gridSubjectSet.Value = IsUrFO
                    ? "Территории для УрФО"
                    : IsRegionSelected
                      ? "Территории для выбранного района"
                      : "Территории для выбранного субъекта";

                chartSubjectSet.Value = IsUrFO
                  ? "[Субъекты]"
                  : "[МО выбранного субъекта]";

                isRegionSelected.Value = IsRegionSelected ? "true" : "false";
                childRegionLevel.Value = IsRegionSelected ? "[Районы].[Сопоставимый].[Уровень 06]" : "[Районы].[Сопоставимый].[Уровень 04]";

                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
            }

            if (!chartWebAsyncPanel2.IsAsyncPostBack)
            {
                regionCenter.Value = RegionCenterExcluding ? String.Format(" {0}", GetRegionCenter(ComboRegion.SelectedValue)) : " ";
                UltraChart1.DataBind();
            }

            if (!chartWebAsyncPanel1.IsAsyncPostBack)
            {
                selectedMeasure.Value = AbsoluteMeasureSelected ? "Прирост/снижение в тыс.руб." : "Прирост/снижение в %";
                UltraChart2.DataBind();
            }
        }

        private static string GetRegionCenter(string regionName)
        {
            switch (regionName)
            {
                case "Курганская область":
                    {
                        return "[Районы].[Сопоставимый].[Все районы].[Курганская область].[Города областного подчинения Курганской области].[Курган]";
                    }
                case "Свердловская область":
                    {
                        return "[Районы].[Сопоставимый].[Все районы].[Свердловская область].[Города областного подчинения Свердловская область].[Екатеринбург]";
                    }
                case "Тюменская область":
                    {
                        return "[Районы].[Сопоставимый].[Все районы].[Тюменская область].[Города областного подчинения Тюменской области].[Тюмень]";
                    }
                case "Челябинская область":
                    {
                        return "[Районы].[Сопоставимый].[Все районы].[Челябинская область].[Города областного подчинения ЧБ].[Челябинск]";
                    }
                case "Ханты-Мансийский автономный округ":
                    {
                        return "[Районы].[Сопоставимый].[Все районы].[Ямало-Ненецкий автономный округ].[Города окружного подчинения Ямало-Ненецкого автономного округа].[г.Салехард]";
                    }
                case "Ямало-Ненецкий автономный округ":
                    {
                        return "[Районы].[Сопоставимый].[Все районы].[Ямало-Ненецкий автономный округ].[Города окружного подчинения Ямало-Ненецкого автономного округа].[г.Салехард]";
                    }
                default:
                    {
                        return String.Empty;
                    }
            }
        }

        private static string GetRegionCenterCaption(string regionName)
        {
            switch (regionName)
            {
                case "Курганская область":
                case "Свердловская область":
                case "Тюменская область":
                case "Челябинская область":
                    {
                        return "Без областного центра";
                    }
                case "Ханты-Мансийский автономный округ":
                case "Ямало-Ненецкий автономный округ":
                    {
                        return "Без окружного центра";
                    }
                default:
                    {
                        return String.Empty;
                    }
            }
        }

        private static string GetShortRegionName(string regionName)
        {
            switch (regionName)
            {
                case "Курганская область":
                    {
                        return "КургО";
                    }
                case "Свердловская область":
                    {
                        return "СвердО";
                    }
                case "Тюменская область":
                    {
                        return "ТюменО";
                    }
                case "Челябинская область":
                    {
                        return "ЧелябО";
                    }
                case "Ханты-Мансийский автономный округ":
                    {
                        return "ХМАО";
                    }
                case "Ямало-Ненецкий автономный округ":
                    {
                        return "ЯНАО";
                    }
                default:
                    {
                        return String.Empty;
                    }
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string queryName = "FNS_0003_0001_regions_grid";
            string query = DataProvider.GetQueryText(queryName);
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Регион", dtGrid);

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
                    if (i == 1 && row[i] != DBNull.Value)
                    {


                        if (row[i].ToString().Contains("Ямало-Ненецкий автономный округ"))
                        {
                            row[i] = row[i].ToString().Replace("Ямало-Ненецкий автономный округ", "ЯНАО");
                        }

                        foreach (string key in RegionsNamingHelper.ShortRegionsNames.Keys)
                        {
                            if (row[i].ToString().Contains(key))
                            {
                                row[i] = row[i].ToString().Replace(key, RegionsNamingHelper.ShortName(key));
                            }
                        }
                    }
                }
            }

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

            if (e.Layout.Bands[0].Columns.Count > 0)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(155);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

                int width = 88;
                int percentWidth = 65;
                int rankWidth = 78;

                SetColumnParams(e.Layout, 0, 1, " ", 55, false);
                SetColumnParams(e.Layout, 0, 2, "N2", width, false);
                SetColumnParams(e.Layout, 0, 3, "N2", width, false);
                SetColumnParams(e.Layout, 0, 4, "N2", width, false);
                SetColumnParams(e.Layout, 0, 5, "N2", width, false);
                SetColumnParams(e.Layout, 0, 6, "P2", percentWidth, false);
                SetColumnParams(e.Layout, 0, 7, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 8, "N0", width, true);
                SetColumnParams(e.Layout, 0, 9, "P2", rankWidth, false);
                SetColumnParams(e.Layout, 0, 10, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 11, "N0", width, true);
                SetColumnParams(e.Layout, 0, 12, "N2", width, false);
                SetColumnParams(e.Layout, 0, 13, "P2", width, false);
                SetColumnParams(e.Layout, 0, 14, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 15, "N0", width, true);

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 5 || i == 6)
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
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, IsRegionSelected ? "Тип МО" : "Субъект", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2,
                    string.Format("Недоимка на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear - 1),
                    string.Format("Недоимка за {0} {1} {2} года.", month, CRHelper.RusManyMonthGenitive(month), year - 1));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3,
                    string.Format("Недоимка на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear),
                    string.Format("Недоимка за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4,
                    string.Format("Недоимка на начало {0} года, тыс.руб", year),
                    "Недоимка на начало года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5,
                    string.Format("тыс.руб."),
                    "Прирост недоимки с начала года в тыс.руб.");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                    string.Format("%"),
                    "Прирост недоимки с начала года в %");

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7,
                    string.Format("Ранг по приросту недоимки с начала {0} года в %", year),
                    "Ранг по приросту недоимки с начала года в %");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9,
                    string.Format("Темп роста недоимки к {0} году, %", year - 1),
                    "Темп роста недоимки по отношению к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10,
                    string.Format("Ранг по темпу роста недоимки к {0} году", year - 1),
                    "Ранг по темпу роста недоимки по отношению к аналогичному периоду прошлого года");
                // на самом деле это скрытая колонка с худшим рангом по темпу роста недоимки
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11,
                    string.Format("Общий объем поступлений налоговых доходов на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear),
                    string.Format("Общий объем поступлений налоговых доходов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12,
                    string.Format("Общий объем поступлений налоговых доходов на 01.{0:00}.{1}, тыс.руб.", nextMonth, nextYear),
                    string.Format("Общий объем поступлений налоговых доходов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 13,
                    string.Format("Удельный вес недоимки в общем объеме поступлений налоговых доходов, %"),
                    string.Format("Удельный вес недоимки в общем объеме налоговых доходов по данным ежемесячного отчета об исполнении бюджетов за {0} {1} {2} года", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 14,
                    string.Format("Ранг по удельному весу"),
                   "Ранг по удельному весу недоимки в общем объеме налоговых доходов");

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = string.Format("Прирост недоимки с начала {0} года", year);
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 5;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string regionName = String.Empty;
            if (e.Row.Cells[0] != null)
            {
                regionName = e.Row.Cells[0].Value.ToString().TrimEnd(' ');
            }

            string subjectName = String.Empty;
            if (e.Row.Cells[1].Value != null)
            {
                subjectName = e.Row.Cells[1].Value.ToString();
            }

            e.Row.Style.Padding.Right = 5;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool grow = (i == 6);
                bool growRate = (i == 9);
                bool rank = (i == 7 || i == 10 || i == 14);
                bool redValue = (i == 5 || i == 6);

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
                            case 7:
                                {
                                    indicatorName = "прирост недоимки";
                                    break;
                                }
                            case 10:
                                {
                                    indicatorName = "темп роста недоимки";
                                    break;
                                }
                            case 14:
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

                if (RegionsNamingHelper.IsFO(regionName) || GetRegionCenter(regionName) != String.Empty || subjectName.Contains("(МР)"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding1(object sender, EventArgs e)
        {
            string queryName = "FNS_0003_0001_regions_chart1";
            string query = DataProvider.GetQueryText(queryName);
            dtChart1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

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

        void UltraChart_FillSceneGraph1(object sender, FillSceneGraphEventArgs e)
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
                    text.labelStyle.Font = chartFont;
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
            string queryName = "FNS_0003_0001_regions_chart2";
            string query = DataProvider.GetQueryText(queryName);
            dtChart2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            foreach (DataRow row in dtChart2.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
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
                    text.labelStyle.Font = chartFont;
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

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = " ";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "[Red]#,##0.00%;-#,##0.00%";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "#0";

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
            if (e.CurrentColumnIndex == 5 || e.CurrentColumnIndex == 6)
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
