using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.Documents.Excel;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Core.MemberDigests;


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0061
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtDimension = new DataTable();
        private DataTable dtChart, dtChart2;
        private int firstYear = 2008;
        private int year = 0;
        private Font VerdanaFont = new Font("Verdana", 8);
        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // фильтр по годам
        private CustomParam filterYear;
        // Последний год
        private CustomParam lastYear;
        // выбранный район
        private CustomParam selectedRegion;
        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;
        private CustomParam selectedGridGroup;
        private CustomParam selectedGridLevel3;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;
        // уровень МО
        private CustomParam regionsLevel;
        #endregion
        private MemberAttributesDigest budgetDigest;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            #region настройка диаграммы
            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<DATA_VALUE:N3> {0}", RadioButtonUnits.SelectedValue.ToString());
            UltraChart.Axis.X.Extent = 90;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart.Axis.Y.Extent = 50;

            UltraChart.Data.ZeroAligned = true;
            UltraChart1.Data.ZeroAligned = true;

            UltraChart2.Legend.Visible = true;
            UltraChart2.BorderWidth = 0;
            UltraChart2.Legend.Font = VerdanaFont;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 100;

            UltraChart2.Tooltips.Display = TooltipDisplay.Never;

            UltraChart2.Axis.X.Visible = false;
            UltraChart2.Axis.Y.Visible = false;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Axis.X.Labels.SeriesLabels.Font = VerdanaFont;
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = VerdanaFont;
            UltraChart.TitleLeft.Font = VerdanaFont;

            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Text = RadioButtonUnits.SelectedValue;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.BorderWidth = 0;

            UltraChart.Legend.Visible = false;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 13;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart1.Visible = false;
            UltraChart1.Legend.Visible = false;
            #endregion
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 350);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            UltraWebGrid.DataBound += new EventHandler(Grid_DataBound);

            int currentWidth = (int)Session["width_size"] - 20;
            UltraChart.Width = currentWidth - 20;
            UltraChart2.Width = currentWidth - 20;
            int currentHeight = (int)Session["height_size"] - 192;
            UltraChart.Height = currentHeight / 2;
            UltraChart2.Height = 75;

            #region Инициализация параметров запроса

            if (filterYear == null)
            {
                filterYear = UserParams.CustomParam("filter_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (consolidateDocumentSKIFType == null)
            {
                consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            }
            if (regionDocumentSKIFType == null)
            {
                regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            selectedRegion = UserParams.CustomParam("selected_region");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");
            selectedGridGroup = UserParams.CustomParam("selected_grid_group");
            selectedGridLevel3 = UserParams.CustomParam("selected_level_3");
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;
            if (!Page.IsPostBack)
            {
                selectedGridGroup.Value = "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Подгруппа]";
                selectedGridIndicator.Value = "ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ";
                UserParams.Organization.Value = "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ]";

                chartWebAsyncPanel.AddRefreshTarget(UltraChart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart1.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid.ClientID);

                dtDate = new DataTable();
                dtDimension = new DataTable();

                string query2 = DataProvider.GetQueryText("FO_0002_0061_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Месяц", dtDimension);
                DateTime maxDate = CRHelper.PeriodDayFoDate(dtDimension.Rows[0][1].ToString());

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, maxDate.Year));
                ComboYear.SetСheckedState(maxDate.Year.ToString(), true);

                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboRegion.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 140;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(maxDate.Month)), true);

                year = Convert.ToInt32(ComboYear.SelectedValue);

                Label1.Text = "Уровень детализации данных:";
                Label2.Text = "Единицы измерения:";
            }
            year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodFirstYear.Value = (year - 1).ToString();
            UserParams.PeriodEndYear.Value = (year - 2).ToString();

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));


            PageSubTitle.Text = string.Format("{0}, данные за {1}-{2} гг., {3}", ComboRegion.SelectedValue, Convert.ToInt16(year - 2).ToString(), year, RadioButtonUnits.SelectedValue);
            PageTitle.Text = "Динамика источников финансирования бюджетов Новосибирской области.";
            Page.Title = PageTitle.Text;
            bool regionSelected = false;
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(ComboRegion.SelectedValue))
            {
                regionSelected = RegionsNamingHelper.LocalBudgetTypes[ComboRegion.SelectedValue] == "МР";
            }
            switch (ComboRegion.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        selectedRegion.Value = String.Format("{0}.[Консолидированный бюджет субъекта ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Муниципальные районы":
                    {
                        selectedRegion.Value = String.Format("{0}.[Муниципальные районы ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Городские округа":
                    {
                        selectedRegion.Value = String.Format("{0}.[Городские округа ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                default:
                    {
                        selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
                        break;
                    }
            }
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetLevel");

            switch (RadioButtonLevel.SelectedIndex)
            {
                case 0:
                    UserParams.FKRSectionLevel.Value = " Descendants ( " +
                    "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ]," +
                    "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Подгруппа], " +
                    "SELF_AND_BEFORE )";
                    break;
                case 1:
                    UserParams.FKRSectionLevel.Value = "{filter(Descendants(" +
                        "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ]," +
                        "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Подстатья] ," +
                        "SELF_AND_BEFORE" +
                    "), IIF(" +
                        "[КИФ__Сопоставимый].[КИФ__Сопоставимый].currentmember.level is  [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Подстатья]," +
                        "([КИФ__Сопоставимый].[КИФ__Сопоставимый].currentmember is [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Иные источники внутреннего финансирования дефицитов бюджетов].[Бюджетные кредиты, предоставленные внутри страны в валюте Российской Федерации].[Предоставление бюджетных кредитов внутри страны в валюте Российской Федерации] or [КИФ__Сопоставимый].[КИФ__Сопоставимый].currentmember is [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Иные источники внутреннего финансирования дефицитов бюджетов].[Бюджетные кредиты, предоставленные внутри страны в валюте Российской Федерации].[Возврат бюджетных кредитов, предоставленных внутри страны в валюте Российской Федерации])," +
                        "true))} ";
                    break;
                case 2:
                    UserParams.FKRSectionLevel.Value =
                        "Filter           " +
"                       (" +
"                            Descendants              " +
"                            (" +
"                                [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ]," +
"                                [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Элемент]," +
"                                SELF_AND_BEFORE              " +
"                            ), " +
"                    (" +
"                        IIF        " +
"                        (" +
"                            [КИФ__Сопоставимый].[КИФ__Сопоставимый].currentmember.level is  [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Подстатья]," +
"                            (" +
"                                [КИФ__Сопоставимый].[КИФ__Сопоставимый].currentmember is [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Иные источники внутреннего финансирования дефицитов бюджетов].[Бюджетные кредиты, предоставленные внутри страны в валюте Российской Федерации].[Предоставление бюджетных кредитов внутри страны в валюте Российской Федерации] or [КИФ__Сопоставимый].[КИФ__Сопоставимый].currentmember is [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Иные источники внутреннего финансирования дефицитов бюджетов].[Бюджетные кредиты, предоставленные внутри страны в валюте Российской Федерации].[Возврат бюджетных кредитов, предоставленных внутри страны в валюте Российской Федерации]           " +
"                            )," +
"                            true           " +
"                        )" +
"                    )" +
"				)";
                    /*                        " Descendants (" +
                        " [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ]," +
                        " [КИФ__Сопоставимый].[КИФ__Сопоставимый].[Элемент]," +
                        " SELF_AND_BEFORE)";*/
                    break;
                default:
                    UserParams.FKRSectionLevel.Value = " Descendants ( " +
                    "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ]," +
                    "[КИФ__Сопоставимый].[КИФ__Сопоставимый].[Подгруппа], " +
                    "SELF_AND_BEFORE )";
                    break;
            }

            switch (RadioButtonUnits.SelectedIndex)
            {
                case 0:
                    UserParams.KDLevel.Value = "1";
                    break;
                case 1:
                    UserParams.KDLevel.Value = "1000";
                    break;
                case 2:
                    UserParams.KDLevel.Value = "1000000";
                    break;
                default:
                    UserParams.KDLevel.Value = "1000";
                    break;
            }
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DisplayLayout.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        private void Grid_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, selectedGridIndicator.Value, 0, 0);
                ActivateGridRow(row);
            }
        }

        private void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }
            selectedGridIndicator.Value = row.Cells[0].Text;

            selectedGridLevel3.Value = row.Cells[17].Text + ".[" + selectedGridIndicator.Value + "]";

            switch (Convert.ToInt16(row.Cells[16].Text))
            {

                case 1:
                    UltraChart.Data.SwapRowsAndColumns = false;
                    UltraChart2.Data.SwapRowsAndColumns = false;
                    UltraChart1.Visible = false;
                    UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<span style='font-weight:bold'><DATA_VALUE:N3></span> {0}", RadioButtonUnits.SelectedValue.ToString().ToLower());
                    int currentWidth1 = (int)Session["width_size"] - 20;
                    UltraChart.Width = currentWidth1 - 20;
                    DynamicChartCaption.Text = selectedGridIndicator.Value;
                    int currentHeight1 = (int)Session["height_size"] - 192;
                    UltraChart.Height = currentHeight1 / 2;
                    dtChart = new DataTable();
                    string query = DataProvider.GetQueryText("FO_0002_0061_chart_IFDB");
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
                    break;
                case 2:
                    UltraChart.Data.SwapRowsAndColumns = false;
                    UltraChart2.Data.SwapRowsAndColumns = false;
                    DynamicChartCaption.Text = selectedGridIndicator.Value;
                    int currentWidth2 = (int)Session["width_size"] - 20;
                    UltraChart.Width = currentWidth2 - 20;

                    int currentHeight2 = (int)Session["height_size"] - 192;
                    UltraChart.Height = currentHeight2 / 2;
                    UltraChart1.Visible = false;
                    UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<span style='font-weight:bold'><DATA_VALUE:N3></span> {0}", RadioButtonUnits.SelectedValue.ToString().ToLower());
                    dtChart = new DataTable();
                    query = DataProvider.GetQueryText("FO_0002_0061_chart_IFDB");
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
                    break;
                case 3:
                    if (row.Cells[18].Text != "Иные источники внутреннего финансирования дефицитов бюджетов")
                    {
                        #region Настройка левой диаграммы
                        UltraChart.ChartType = ChartType.StackColumnChart;
                        UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<span style='font-weight:bold'><DATA_VALUE:N3></span> {0}", RadioButtonUnits.SelectedValue.ToString().ToLower());
                        UltraChart.Axis.X.Extent = 90;
                        UltraChart.Axis.X.Labels.SeriesLabels.WrapText = true;
                        UltraChart.Axis.Y.Extent = 50;
                        UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
                        UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
                        UltraChart.TitleLeft.Text = "Фактическое исполнение, " + RadioButtonUnits.SelectedValue;
                        UltraChart1.TitleLeft.Text = "Годовые назначения, " + RadioButtonUnits.SelectedValue;
                        UltraChart.TitleLeft.Visible = true;
                        UltraChart1.TitleLeft.Visible = true;
                        UltraChart.Data.SwapRowsAndColumns = true;
                        UltraChart.BorderWidth = 0;
                        UltraChart.Axis.X.Labels.SeriesLabels.Font = VerdanaFont;
                        UltraChart.TitleLeft.Font = VerdanaFont;
                        UltraChart1.TitleLeft.Font = VerdanaFont;
                        UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

                        ChartTextAppearance appearance = new ChartTextAppearance();
                        appearance.Column = 0;
                        appearance.Row = -2;
                        appearance.VerticalAlign = StringAlignment.Far;
                        appearance.ItemFormatString = "<DATA_VALUE_ITEM:N3>";
                        appearance.ChartTextFont = VerdanaFont;
                        appearance.Visible = true;
                        UltraChart.ColumnChart.ChartText.Add(appearance);

                        appearance = new ChartTextAppearance();
                        appearance.Column = 1;
                        appearance.Row = -2;
                        appearance.VerticalAlign = StringAlignment.Near;
                        appearance.ItemFormatString = "<DATA_VALUE_ITEM:N3>";
                        appearance.ChartTextFont = VerdanaFont;
                        appearance.Visible = true;
                        UltraChart.ColumnChart.ChartText.Add(appearance);
                        #endregion
                        #region Настройка правой диаграммы

                        int currentWidth = (int)Session["width_size"] - 20;
                        UltraChart1.Width = currentWidth / 2 - 20;

                        int currentHeight = (int)Session["height_size"] - 192;
                        UltraChart1.Height = currentHeight / 2;
                        UltraChart1.Axis.X.Labels.SeriesLabels.Font = VerdanaFont;
                        UltraChart.Width = currentWidth / 2 - 20;
                        UltraChart.Height = currentHeight / 2;
                        UltraChart1.ChartType = ChartType.StackColumnChart;
                        UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<span style='font-weight:bold'><DATA_VALUE:N3></span> {0}", RadioButtonUnits.SelectedValue.ToString());
                        UltraChart1.Axis.X.Extent = 90;
                        UltraChart1.Axis.X.Labels.SeriesLabels.WrapText = true;
                        UltraChart1.Axis.Y.Extent = 50;
                        UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                        UltraChart1.Data.SwapRowsAndColumns = true;
                        UltraChart2.Data.SwapRowsAndColumns = true;

                        UltraChart1.BorderWidth = 0;
                        DynamicChartCaption.Text = row.Cells[18].Text;//выввести имя предка!

                        ChartTextAppearance appearance1 = new ChartTextAppearance();
                        appearance1.Column = 0;
                        appearance1.Row = -2;
                        appearance1.VerticalAlign = StringAlignment.Far;
                        appearance1.ItemFormatString = "<DATA_VALUE_ITEM:N3>";
                        appearance1.ChartTextFont = VerdanaFont;
                        appearance1.Visible = true;
                        UltraChart1.ColumnChart.ChartText.Add(appearance1);

                        appearance1 = new ChartTextAppearance();
                        appearance1.Column = 1;
                        appearance1.Row = -2;
                        appearance1.VerticalAlign = StringAlignment.Near;
                        appearance1.ItemFormatString = "<DATA_VALUE_ITEM:N3>";
                        appearance1.ChartTextFont = VerdanaFont;
                        appearance1.Visible = true;
                        UltraChart1.ColumnChart.ChartText.Add(appearance1);
                        UltraChart1.Visible = true;

                        string query3 = DataProvider.GetQueryText("FO_0002_0061_chart2");
                        dtChart = new DataTable();
                        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query3, "Dummy", dtChart);

                        string query4 = DataProvider.GetQueryText("FO_0002_0061_chart3");
                        dtChart2 = new DataTable();
                        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query4, "Dummy", dtChart2);
                        UltraChart1.DataBind();
                        #endregion
                    }
                    else
                    {
                        UltraChart.Data.SwapRowsAndColumns = false;
                        UltraChart2.Data.SwapRowsAndColumns = false;
                        DynamicChartCaption.Text = selectedGridIndicator.Value;
                        int currentWidth3 = (int)Session["width_size"] - 20;
                        UltraChart.Width = currentWidth3 - 20;
                        int currentHeight3 = (int)Session["height_size"] - 192;
                        UltraChart.Height = currentHeight3 / 2;
                        UltraChart1.Visible = false;

                        UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
                        UltraChart.TitleLeft.Font = VerdanaFont;

                        UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<span style='font-weight:bold'><DATA_VALUE:N3></span> {0}", RadioButtonUnits.SelectedValue.ToString());
                        dtChart = new DataTable();
                        query = DataProvider.GetQueryText("FO_0002_0061_chart_IFDB");
                        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
                    }
                    break;
                case 4:
                    UltraChart.Data.SwapRowsAndColumns = false;
                    UltraChart2.Data.SwapRowsAndColumns = false;
                    DynamicChartCaption.Text = selectedGridIndicator.Value;
                    int currentWidth4 = (int)Session["width_size"] - 20;
                    UltraChart.Width = currentWidth4 - 20;
                    int currentHeight4 = (int)Session["height_size"] - 192;
                    UltraChart.Height = currentHeight4 / 2;
                    UltraChart1.Visible = false;

                    UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
                    UltraChart.TitleLeft.Font = VerdanaFont;

                    UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<span style='font-weight:bold'><DATA_VALUE:N3></span> {0}", RadioButtonUnits.SelectedValue.ToString());
                    dtChart = new DataTable();
                    query = DataProvider.GetQueryText("FO_0002_0061_chart_IFDB");
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
                    break;
                case 5:
                    UltraChart.Data.SwapRowsAndColumns = false;
                    UltraChart2.Data.SwapRowsAndColumns = false;
                    DynamicChartCaption.Text = selectedGridIndicator.Value;
                    int currentWidth5 = (int)Session["width_size"] - 20;
                    UltraChart.Width = currentWidth5 - 20;
                    int currentHeight5 = (int)Session["height_size"] - 192;
                    UltraChart.Height = currentHeight5 / 2;
                    UltraChart1.Visible = false;

                    UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
                    UltraChart.TitleLeft.Font = VerdanaFont;

                    UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<span style='font-weight:bold'><DATA_VALUE:N3></span> {0}", RadioButtonUnits.SelectedValue.ToString());
                    dtChart = new DataTable();
                    query = DataProvider.GetQueryText("FO_0002_0061_chart_IFDB");
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
                    break;
            }
            UltraChart.DataBind();
        }
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0061_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dt);
            decimal FinancingBudgetDeficit_1 = 0,
                    FinancingBudgetDeficit_2 = 0,
                    FinancingBudgetDeficit_3 = 0,
                    FinancingBudgetDeficit_4 = 0,
                    FinancingBudgetDeficit_5 = 0,
                    FinancingBudgetDeficit_6 = 0;
            //находим ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ 
            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                if (dt.Rows[i][0].ToString() == "ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ")
                {
                    FinancingBudgetDeficit_1 = Convert.ToDecimal(dt.Rows[i][1].ToString());
                    FinancingBudgetDeficit_2 = Convert.ToDecimal(dt.Rows[i][3].ToString());
                    FinancingBudgetDeficit_3 = Convert.ToDecimal(dt.Rows[i][5].ToString());
                    FinancingBudgetDeficit_4 = Convert.ToDecimal(dt.Rows[i][7].ToString());
                    FinancingBudgetDeficit_5 = Convert.ToDecimal(dt.Rows[i][9].ToString());
                    FinancingBudgetDeficit_6 = Convert.ToDecimal(dt.Rows[i][11].ToString());
                }
            }
            //теперь вычисляем поле "удельный вес"

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((dt.Rows[i][16].ToString() == "1") || (dt.Rows[i][16].ToString() == "2"))
                {
                    if (FinancingBudgetDeficit_1 != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][1].ToString()) >= 0 && FinancingBudgetDeficit_1 >= 0) || (Convert.ToDecimal(dt.Rows[i][1].ToString()) <= 0 && FinancingBudgetDeficit_1 <= 0))
                        {
                            dt.Rows[i][2] = Convert.ToDecimal(dt.Rows[i][1].ToString()) / FinancingBudgetDeficit_1;
                        }
                        else
                        { dt.Rows[i][2] = DBNull.Value; }
                    }
                    if (FinancingBudgetDeficit_2 != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][3].ToString()) >= 0 && FinancingBudgetDeficit_2 >= 0) || (Convert.ToDecimal(dt.Rows[i][3].ToString()) <= 0 && FinancingBudgetDeficit_2 <= 0))
                        {
                            dt.Rows[i][4] = Convert.ToDecimal(dt.Rows[i][3].ToString()) / FinancingBudgetDeficit_2;
                        }
                        else
                        { dt.Rows[i][4] = DBNull.Value; }
                    }
                    if (FinancingBudgetDeficit_3 != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][5].ToString()) >= 0 && FinancingBudgetDeficit_3 >= 0) || (Convert.ToDecimal(dt.Rows[i][5].ToString()) <= 0 && FinancingBudgetDeficit_3 <= 0))
                        {
                            dt.Rows[i][6] = Convert.ToDecimal(dt.Rows[i][5].ToString()) / FinancingBudgetDeficit_3;
                        }
                        else
                        { dt.Rows[i][6] = DBNull.Value; }
                    }
                    if (FinancingBudgetDeficit_4 != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][7].ToString()) >= 0 && FinancingBudgetDeficit_4 >= 0) || (Convert.ToDecimal(dt.Rows[i][7].ToString()) <= 0 && FinancingBudgetDeficit_4 <= 0))
                        {
                            dt.Rows[i][8] = Convert.ToDecimal(dt.Rows[i][7].ToString()) / FinancingBudgetDeficit_4;
                        }
                        else
                        { dt.Rows[i][8] = DBNull.Value; }
                    }
                    if (FinancingBudgetDeficit_5 != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][9].ToString()) >= 0 && FinancingBudgetDeficit_5 >= 0) || (Convert.ToDecimal(dt.Rows[i][9].ToString()) <= 0 && FinancingBudgetDeficit_5 <= 0))
                        {
                            dt.Rows[i][10] = Convert.ToDecimal(dt.Rows[i][9].ToString()) / FinancingBudgetDeficit_5;
                        }
                        else
                        { dt.Rows[i][10] = DBNull.Value; }
                    }
                    if (FinancingBudgetDeficit_6 != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][11].ToString()) >= 0 && FinancingBudgetDeficit_6 >= 0) || (Convert.ToDecimal(dt.Rows[i][11].ToString()) <= 0 && FinancingBudgetDeficit_6 <= 0))
                        {
                            dt.Rows[i][12] = Convert.ToDecimal(dt.Rows[i][11].ToString()) / FinancingBudgetDeficit_6;
                        }
                        else
                        { dt.Rows[i][12] = DBNull.Value; }
                    }
                    if (Convert.ToDecimal(dt.Rows[i][7].ToString()) != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][11].ToString()) >= 0 && Convert.ToDecimal(dt.Rows[i][7].ToString()) >= 0) || (Convert.ToDecimal(dt.Rows[i][11].ToString()) <= 0 && Convert.ToDecimal(dt.Rows[i][7].ToString()) <= 0))
                        {
                            dt.Rows[i][14] = Convert.ToDecimal(dt.Rows[i][11].ToString()) / Convert.ToDecimal(dt.Rows[i][7].ToString());
                            dt.Rows[i][15] = Convert.ToDecimal(dt.Rows[i][11].ToString()) / Convert.ToDecimal(dt.Rows[i][7].ToString()) - 1;
                        }
                        else
                        {
                            dt.Rows[i][14] = DBNull.Value;
                            dt.Rows[i][15] = DBNull.Value;
                        }
                    }
                }
                else
                {
                    dt.Rows[i][2] = DBNull.Value;
                    dt.Rows[i][4] = DBNull.Value;
                    dt.Rows[i][6] = DBNull.Value;
                    dt.Rows[i][8] = DBNull.Value;
                    dt.Rows[i][10] = DBNull.Value;
                    dt.Rows[i][12] = DBNull.Value;

                    if (Convert.ToDecimal(dt.Rows[i][7].ToString()) != 0)
                    {
                        if ((Convert.ToDecimal(dt.Rows[i][11].ToString()) >= 0 && Convert.ToDecimal(dt.Rows[i][7].ToString()) >= 0) || (Convert.ToDecimal(dt.Rows[i][11].ToString()) <= 0 && Convert.ToDecimal(dt.Rows[i][7].ToString()) <= 0))
                        {
                            dt.Rows[i][14] = Convert.ToDecimal(dt.Rows[i][11].ToString()) / Convert.ToDecimal(dt.Rows[i][7].ToString());
                            dt.Rows[i][15] = Convert.ToDecimal(dt.Rows[i][11].ToString()) / Convert.ToDecimal(dt.Rows[i][7].ToString()) - 1;
                        }
                        else
                        {
                            dt.Rows[i][14] = DBNull.Value;
                            dt.Rows[i][15] = DBNull.Value;
                        }
                    }
                }

            }

            if (dt.Rows.Count > 0 && dt.Rows[0][1] != DBNull.Value)
            {
                UltraWebGrid.DataSource = dt;
            }

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[13], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[14], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[15], "P2");

            e.Layout.Bands[0].Columns[16].Hidden = true;
            e.Layout.Bands[0].Columns[17].Hidden = true;
            e.Layout.Bands[0].Columns[18].Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
            }
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(105);
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.HeaderStyleDefault.Wrap = true;

            headerLayout.AddCell("Наименование ИФДБ");
            GridHeaderCell endYear = headerLayout.AddCell(string.Format("{0}", Convert.ToString(Convert.ToInt16(year) - 2)));
            GridHeaderCell PlanendYear = endYear.AddCell("план");
            PlanendYear.AddCell("Значение");
            PlanendYear.AddCell("Удельный вес");
            GridHeaderCell FactendYear = endYear.AddCell("факт");
            FactendYear.AddCell("Значение");
            FactendYear.AddCell("Удельный вес");

            GridHeaderCell MiddleYear = headerLayout.AddCell(string.Format("{0}", Convert.ToString(Convert.ToInt16(year) - 1)));
            GridHeaderCell PlanMiddleYear = MiddleYear.AddCell("план");
            PlanMiddleYear.AddCell("Значение");
            PlanMiddleYear.AddCell("Удельный вес");
            GridHeaderCell FactMiddleYear = MiddleYear.AddCell("факт");
            FactMiddleYear.AddCell("Значение");
            FactMiddleYear.AddCell("Удельный вес");

            GridHeaderCell FirstYear = headerLayout.AddCell(string.Format("{0}", year));
            GridHeaderCell PlanFirstYear = FirstYear.AddCell("план");
            PlanFirstYear.AddCell("Значение");
            PlanFirstYear.AddCell("Удельный вес");
            GridHeaderCell FactFirstYear = FirstYear.AddCell("факт");
            FactFirstYear.AddCell("Значение");
            FactFirstYear.AddCell("Удельный вес");

            FirstYear.AddCell("% исполнения плана");
            FirstYear.AddCell("темп роста");
            FirstYear.AddCell("темп прироста");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            switch (Convert.ToInt16(e.Row.Cells[16].Text))
            {

                case 1:
                    e.Row.Cells[0].Style.Font.Bold = true;
                    e.Row.Cells[0].Style.Font.Size = 10;
                    break;
                case 2:
                    e.Row.Cells[0].Style.Font.Bold = true;
                    e.Row.Cells[0].Style.Font.Size = 9;
                    break;
                case 3:
                    e.Row.Cells[0].Style.Font.Bold = false;
                    e.Row.Cells[0].Style.Font.Size = 9;
                    break;
                case 4:
                    e.Row.Cells[0].Style.Font.Bold = false;
                    e.Row.Cells[0].Style.Font.Size = 8;
                    break;
                case 5:
                    e.Row.Cells[0].Style.Font.Bold = false;
                    e.Row.Cells[0].Style.Font.Italic = true;
                    e.Row.Cells[0].Style.Font.Size = 8;
                    break;
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0 && i != 4)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("итого")))
                    {
                        cell.Style.Font.Bold = true;

                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            UltraChart.DataSource = dtChart;
            UltraChart2.DataSource = dtChart;
            UltraChart2.DataBind();
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {

        }
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.DataSource = dtChart2;
        }
        protected void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int j = 2;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    if (text.Row == -1)
                    {
                        text.bounds.Height = 40;
                        text.bounds = new Rectangle(text.bounds.Left, text.bounds.Y, text.bounds.Width, text.bounds.Height);
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;
                        text.labelStyle.VerticalAlign = StringAlignment.Far;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                        if (j >= 0)
                        {
                            text.SetTextString((Convert.ToInt16(ComboYear.SelectedValue) - j).ToString());
                            j--;
                        }
                    }
                }
            }
        }
        #endregion

        #region Экспорт

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
