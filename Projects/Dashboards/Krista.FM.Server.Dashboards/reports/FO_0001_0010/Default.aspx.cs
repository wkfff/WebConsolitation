using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Band;
using System.IO;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Infragistics.Documents.Reports.Report.List;
using System.Drawing;
using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Components;
using System.Globalization;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.List;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.TOC;
using System.Collections;
namespace Krista.FM.Server.Dashboards.reports.FO_0001_0010
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Year;
        private CustomParam regionChart;
        private CustomParam mul;
        private CustomParam dec;
        private CustomParam nMonth;
        private CustomParam incomesTotal;
        private CustomParam selectedPeriod;
        private CustomParam regType;
        private CustomParam measStr;
        private CustomParam incomes1;
        private CustomParam incomes2;
        private CustomParam outcomes1;
        private CustomParam outcomes2;
        private CustomParam level;
        private CustomParam predyear;
        private CustomParam typeOfDocumentSKIF;
        private static Dictionary<string, int> LevelsDictionary = new Dictionary<string, int>();
        #region Параметры запроса

        // уровень бюджета
        private CustomParam budgetLevel;
        // группа доходов
        private CustomParam fnsKDGroup;
        private CustomParam selectedRegion;
        Boolean hideRang = false;
        #endregion

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 850 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }
        private int SelectedBudgetIndex
        {
            get { return MeasureButtonList.SelectedIndex; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
            if (typeOfDocumentSKIF == null)
            {
                typeOfDocumentSKIF = UserParams.CustomParam("typeOfDocumentSKIF");
            }
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }

            if (regionChart == null)
            {
                regionChart = UserParams.CustomParam("regionChart");
            }
            if (level == null)
            {
                level = UserParams.CustomParam("level");
            }
            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }
            if (regType == null)
            {
                regType = UserParams.CustomParam("regType");
            }
            if (incomes1 == null)
            {
                incomes1 = UserParams.CustomParam("incomes1");
            }
            if (incomes2 == null)
            {
                incomes2 = UserParams.CustomParam("incomes2");
            }
            if (outcomes1 == null)
            {
                outcomes1 = UserParams.CustomParam("outcomes1");
            }
            if (outcomes2 == null)
            {
                outcomes2 = UserParams.CustomParam("outcomes2");
            }
            if (dec == null)
            {
                dec = UserParams.CustomParam("dec");
            }
            if (budgetLevel == null)
            {
                budgetLevel = UserParams.CustomParam("budget_level");
            }
            if (fnsKDGroup == null)
            {
                fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            }
            if (nMonth == null)
            {
                nMonth = UserParams.CustomParam("nMonth");
            }
            if (measStr == null)
            {
                measStr = UserParams.CustomParam("measStr");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selectedPeriod");
            }
            if (level == null)
            {
                level = UserParams.CustomParam("level");
            }
            if (predyear == null)
            {
                predyear = UserParams.CustomParam("period_pred_year");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.52);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.25);
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.50);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.25);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraGridExporter1.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            CRHelper.SaveToUserAgentLog(String.Format("Остальной прелоад {0}", Environment.TickCount - start));
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.HeaderChildCellHeight = 190;
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.50));
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.65);
            UltraChart1.ChartType = ChartType.BarChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n за {0} год\n<DATA_VALUE:N0> млн.руб.", ComboYear.SelectedValue);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.Y.Extent = 150;
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 10;
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 35);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 15);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 12);
            UltraChart1.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.Data.ZeroAligned = true;
            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = - 2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE_ITEM:N2>";
            appearance.ChartTextFont = new System.Drawing.Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart1.BarChart.ChartText.Add(appearance);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.47));
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.65);
            UltraChart2.ChartType = ChartType.BarChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<DATA_VALUE:N0> млн.руб.", ComboYear.SelectedValue);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.Axis.Y.Extent = 250;
            UltraChart2.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = UltraChart2.Axis.X.Extent;
            UltraChart2.Axis.Y.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 10;
            UltraChart2.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 35);
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 15);
            UltraChart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 35);
            UltraChart2.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 15);
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart2.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart1.ColumnChart.NullHandling = NullHandling.Zero;
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Near;
            appearance.ItemFormatString = "<DATA_VALUE_ITEM:N2>";
            appearance.ChartTextFont = new System.Drawing.Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart2.BarChart.ChartText.Add(appearance);
            UltraChart1.Axis.X.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Center;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {

        }


        int Q = 0;
        string units = string.Empty;
        string queryName1 = string.Empty;
        string queryName2 = string.Empty;
        string queryChart1Name = string.Empty;
        string queryChart2Name = string.Empty;
        private static Dictionary<string, int> IncomesDictionary = new Dictionary<string, int>();
        private static Dictionary<string, int> OutcomesDictionary = new Dictionary<string, int>();
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));

            start = Environment.TickCount;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.FNSOKVEDGovernment.Value = RegionSettingsHelper.Instance.FNSOKVEDGovernment;
            UserParams.FNSOKVEDHousehold.Value = RegionSettingsHelper.Instance.FNSOKVEDHousehold;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string queryName = "FO_0001_0010_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string endmonth = dtDate.Rows[0][3].ToString();

                int firstYear = 2006;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(Convert.ToString((Convert.ToInt32(endYear.ToString()))), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endmonth, true);

                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 340;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillLocalBudgets(RegionsNamingHelper.LocalBudgetTypes));
                ComboRegion.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboIncomes1.Title = "Вариант доходов тек. год";
                ComboIncomes1.Width = 200;
                ComboIncomes1.MultiSelect = false;
                FillIncomesCombo();
                ComboIncomes1.FillDictionaryValues(IncomesDictionary);

                ComboIncomes2.Title = "Вариант доходов пред. год";
                ComboIncomes2.Width = 200;
                ComboIncomes2.MultiSelect = false;
                FillIncomesCombo();
                ComboIncomes2.FillDictionaryValues(IncomesDictionary);

                ComboOutcomes1.Title = "Вариант расходов тек. год";
                ComboOutcomes1.Width = 200;
                ComboOutcomes1.MultiSelect = false;
                FillOutcomesCombo();
                ComboOutcomes1.FillDictionaryValues(OutcomesDictionary);

                ComboOutcomes2.Title = "Вариант расходов пред. год";
                ComboOutcomes2.Width = 200;
                ComboOutcomes2.MultiSelect = false;
                FillOutcomesCombo();
                ComboOutcomes2.FillDictionaryValues(OutcomesDictionary);


            }

            Year.Value = ComboYear.SelectedValue;
            Page.Title = "Динамика доходов и расходов бюджетов";
            int month = ComboMonth.SelectedIndex + 1;
            nMonth.Value = Convert.ToString(month);
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            predyear.Value = Convert.ToString(year - 1);
            selectedPeriod.Value = String.Empty;
            if (RadioList.SelectedIndex == 0)
            {
                mul.Value = Convert.ToString(1000);
                units = "тыс.руб.";
            }
            else
            {
                mul.Value = Convert.ToString(1000000);
                units = "млн.руб.";
            }
            UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N0> {1}", ComboYear.SelectedValue, units);
            UltraChart2.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N0> {1}", ComboYear.SelectedValue, units);
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            string titlemonth = string.Empty;
            if (ComboMonth.SelectedValue != "Январь")
            {
                titlemonth = String.Format("за январь - {0}", ComboMonth.SelectedValue.ToLower());
            }
            else
            {
                titlemonth = "за январь";
            }
            Label2.Text = "";
            string lab = "";
            string labe = "";
            string memb = "";
            if (RList.SelectedIndex == 0)
            {
                queryName1 = "FO_0001_0010_compare_Grid1_1";
                queryName2 = "FO_0001_0010_compare_Grid2_1";
                queryChart1Name = "FO_0001_0010_compare_Chart1_1";
                queryChart2Name = "FO_0001_0010_compare_Chart2_1";
                lab = "фактические данные";
                ComboIncomes1.Visible = false;
                ComboIncomes2.Visible = false;
                ComboOutcomes1.Visible = false;
                ComboOutcomes2.Visible = false;
                ComboMonth.Visible = true;
                ComboYear.Visible = true;
                memb = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Консолидированный бюджет Саратовской области],[Уровни бюджета__СКИФ].[Все]";
            }
            else if (RList.SelectedIndex == 1)
            {
                queryName1 = "FO_0001_0010_compare_Grid1_2";
                queryName2 = "FO_0001_0010_compare_Grid2_2";
                queryChart1Name = "FO_0001_0010_compare_Chart1_2";
                queryChart2Name = "FO_0001_0010_compare_Chart2_2";
                lab = "плановые данные";
                ComboIncomes1.Visible = false;
                ComboIncomes2.Visible = false;
                ComboOutcomes1.Visible = false;
                ComboOutcomes2.Visible = false;
                ComboMonth.Visible = true;
                ComboYear.Visible = true;
                memb = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Консолидированный бюджет Саратовской области],[Уровни бюджета__СКИФ].[Все]";
            }
            else
            {
                queryName1 = "FO_0001_0010_compare_Grid1_3";
                queryName2 = "FO_0001_0010_compare_Grid2_3";
                queryChart1Name = "FO_0001_0010_compare_Chart1_3";
                queryChart2Name = "FO_0001_0010_compare_Chart2_3";
                lab = "проектные данные";
                ComboIncomes1.Visible = true;
                ComboIncomes2.Visible = true;
                ComboOutcomes1.Visible = true;
                ComboOutcomes2.Visible = true;
                incomes1.Value = ComboIncomes1.SelectedValue;
                incomes2.Value = ComboIncomes2.SelectedValue;
                outcomes1.Value = ComboOutcomes1.SelectedValue;
                outcomes2.Value = ComboOutcomes2.SelectedValue;
                UserParams.PeriodYear.Value = ComboIncomes1.SelectedNodeParent;
                predyear.Value = ComboIncomes2.SelectedNodeParent;
                ComboMonth.Visible = false;
                ComboYear.Visible = false;
                memb = "[Уровни бюджетов].[Все].[Все уровни]";
            }
            bool regionSelected = false;
            switch (ComboRegion.SelectedValue)
            {
                case "Местные бюджеты":
                    {
                        level.Value = memb + ".[Конс.бюджет МО]";
                        typeOfDocumentSKIF.Value = "[ТипДокумента__СКИФ].[Все].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";
                        break;
                    }
                case "Консолидированный бюджет субъекта":
                    {
                        level.Value = memb + ".[Конс.бюджет субъекта]";
                        typeOfDocumentSKIF.Value = "[ТипДокумента__СКИФ].[Все].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";
                        break;
                    }
                case "Собственный бюджет субъекта":
                    {
                        level.Value = memb + ".[Бюджет субъекта]";
                        typeOfDocumentSKIF.Value = "[ТипДокумента__СКИФ].[Все].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";
                        break;
                    }
                default:
                    {
                        level.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue];
                        if (ComboRegion.SelectedNodeParent == "Муниципальные районы")
                        {
                            typeOfDocumentSKIF.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Все].[Консолидированный отчет муниципального района]";
                            regionSelected = true;
                            switch (SelectedBudgetIndex)
                            {
                                case 0:
                                    {
                                        level.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue] + ",[Уровни бюджета].[СКИФ].[Все].[Конс.бюджет субъекта]";
                                        break;
                                    }
                                case 1:
                                    {
                                        level.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue] + ",[Уровни бюджета].[СКИФ].[Все].[Бюджет района]";
                                        break;
                                    }
                                case 2:
                                    {
                                        level.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue] + ",[Уровни бюджета].[СКИФ].[Все].[Бюджет поселения]";
                                        break;
                                    }
                            }
                        }
                        else typeOfDocumentSKIF.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Все].[Собственный отчет городского округа]";
                        break;
                    }
            }
            MeasureButtonList.Visible = regionSelected;
            Label1.Text = "Динамика доходов и расходов бюджетов";
            string monthnum = Convert.ToString(ComboMonth.SelectedIndex + 2);
            if ((ComboMonth.SelectedIndex + 1) < 10)
                monthnum = "0" + monthnum;
            if (lab == "проектные данные")
            {
                Label2.Text = string.Format("<br/>Проект бюджета на {0} год", ComboYear.SelectedValue);

            }
            else
                if (ComboMonth.SelectedValue == "Декабрь")
                {
                    if (lab == "плановые данные")
                    {

                        Label2.Text = string.Format("<br/>Бюджетные назначения за {0} год", ComboYear.SelectedValue);
                    }
                    else
                        Label2.Text = string.Format("<br/>Исполнено за {0} год", ComboYear.SelectedValue);

                }
                else
                    if (lab == "плановые данные")
                    {

                        Label2.Text = string.Format("<br/>Бюджетные назначения по состоянию на 01.{0}.{1}", monthnum, ComboYear.SelectedValue);
                    }
                    else
                        Label2.Text = string.Format("<br/>Исполнено на 01.{0}.{1}", monthnum, ComboYear.SelectedValue);
            string patternValue = UserParams.StateArea.Value;
            int defaultRowIndex = 1;
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            CRHelper.SaveToUserAgentLog(String.Format("Остльной лоад {0}", Environment.TickCount - start));
        }

        #region Обработчики грида
        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
        }
        int minf = 0;
        int minu = 0;
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText(queryName1);
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование доходов", dtGrid);

            UltraWebGrid.DataSource = dtGrid;

        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText(queryName2);
            DataTable dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование расходов", dtGrid1);

            UltraWebGrid1.DataSource = dtGrid1;

        }

        private static string GetChartQuarterStr(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return string.Format("{0} квартал", quarterNumber);
            }
            else
            {
                return period;
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.bounds.Width = 50;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new System.Drawing.Font("Verdana", 8);
                    axisText.labelStyle.WrapText = true;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.HorizontalAlign = StringAlignment.Center;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        double percent = 0;
                        string columnname = string.Empty;
                        {
                            //box. = "";
                                //String.Format("Доля в общей сумме начислений {0:N2}%<br />{1} за {2} год", percent, box.DataPoint.Label, ComboYear.SelectedValue);
                        }
                    }
                }
            }

        }


        DataTable Chart = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(queryChart1Name);
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {

                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("Муниципальный район", "МР");
                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("муниципальный район", "МР");

            }
            UltraChart1.Series.Clear();
            for (int i = 1; i < dtChart1.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                series.Label = dtChart1.Columns[i].ColumnName;
                UltraChart2.Series.Add(series);
            }

            UltraChart1.DataSource = dtChart1;

        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(queryChart2Name);
            DataTable dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);
            for (int i = 0; i < dtChart2.Rows.Count; i++)
            {

                dtChart2.Rows[i][0] = dtChart2.Rows[i][0].ToString().Replace("Муниципальный район", "МР");
                dtChart2.Rows[i][0] = dtChart2.Rows[i][0].ToString().Replace("муниципальный район", "МР");

            }
            UltraChart2.Series.Clear();
            for (int i = 1; i < dtChart2.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
                series.Label = dtChart2.Columns[i].ColumnName;
                UltraChart2.Series.Add(series);
            }


            UltraChart2.DataSource = dtChart2;
        }

        private static void FillIncomesCombo()
        {
            if (IncomesDictionary.Count > 0)
                return;
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0001_0010_compare_Grid_Kind");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtIncomes);

            DataTable dtIncomesChildren = new DataTable();
            query = DataProvider.GetQueryText("FO_0001_0010_compare_Grid_Kind_a");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtIncomesChildren);
            int IncomesCount = 0;
            foreach (DataRow row in dtIncomes.Rows)
            {
                string income = Convert.ToString(dtIncomes.Rows[IncomesCount][1].ToString());
                IncomesDictionary.Add(income, 0);
                int IncomesChildrenCount = 0;
                foreach (DataRow rowchildren in dtIncomesChildren.Rows)
                {
                    string element = Convert.ToString(dtIncomesChildren.Rows[IncomesChildrenCount][1].ToString());
                    if (dtIncomesChildren.Rows[IncomesChildrenCount][3].ToString().Contains("[" + income + "]"))
                    {
                        IncomesDictionary.Add(element, 1);
                    }
                    IncomesChildrenCount++;
                }
                IncomesCount++;
            }
        }

        private static void FillOutcomesCombo()
        {
            if (OutcomesDictionary.Count > 0)
                return;
            DataTable dtOutcomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0001_0010_compare_Grid_Kinds");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtOutcomes);
            int OutcomesCount = 0;
            foreach (DataRow row in dtOutcomes.Rows)
            {
                string outcome = Convert.ToString(dtOutcomes.Rows[OutcomesCount][1].ToString());
                OutcomesDictionary.Add(outcome, 0);
                OutcomesCount++;
            }
        }
        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {

        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count > 0)
            {
                e.Layout.GroupByBox.Hidden = true;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(245);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 3;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[k].Width = 105;
                    string formatString = "N2";
                    if ((k == 3) || (k == 6))
                    {
                        formatString = "P2";
                    }
                    e.Layout.Bands[0].Columns[k].Format = formatString;
                    e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.SpanY = 2;
                }

                if (RList.SelectedIndex == 0)
                {
                    e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Исполнено {0}", units);
                    e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Исполнено (прошлый год) {0}", units);
                    e.Layout.Bands[0].Columns[2].Header.Title = "Фактическое поступление доходов нарастающим итогом с начала года";
                    e.Layout.Bands[0].Columns[1].Header.Title = "Фактическое поступление доходов за аналогичный период прошлого года";
                    e.Layout.Bands[0].Columns[3].Header.Title = "Темп роста  фактического поступления доходов к аналогичному периоду предыдущего года";

                }
                else
                    if (RList.SelectedIndex == 1)
                    {
                        e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Бюджетные назначения {0}", units);
                        e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Бюджетные назначения (прошлый год) {0}", units);
                        e.Layout.Bands[0].Columns[2].Header.Title = "Плановые назначения на текущий год";
                        e.Layout.Bands[0].Columns[1].Header.Title = "Плановые назначения на прошлый год";
                        e.Layout.Bands[0].Columns[3].Header.Title = "Темп роста  плановых расходов к  аналогичному периоду предыдущего года";

                    }
                    else
                        if (RList.SelectedIndex == 2)
                        {
                            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Проект {0}", units);
                            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Проект (прошлый год) {0}", units);
                            e.Layout.Bands[0].Columns[2].Header.Title = "Проект доходов на текущий год";
                            e.Layout.Bands[0].Columns[1].Header.Title = "Проект доходов на прошлый год";
                            e.Layout.Bands[0].Columns[3].Header.Title = "Темп роста доходов к  аналогичному периоду предыдущего года";
                        }

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = "Доходы";
                ch.Title = "";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 1;
                ch.RowLayoutColumnInfo.SpanX = 3;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                ch = new ColumnHeader(true);
                ch.Caption = "Расходы бюджета";
                ch.Title = "";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 4;
                ch.RowLayoutColumnInfo.SpanX = 3;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

            }
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count > 0)
            {
                e.Layout.GroupByBox.Hidden = true;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(245);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 3;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[k].Width = 100;
                    string formatString = "N2";
                    if ((k == 3) || (k == 6))
                    {
                        formatString = "P2";
                    }
                    e.Layout.Bands[0].Columns[k].Format = formatString;
                    e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.SpanY = 2;
                }

                if (RList.SelectedIndex == 0)
                {
                    e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Исполнено {0}", units);
                    e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Исполнено (прошлый год) {0}", units);
                    e.Layout.Bands[0].Columns[2].Header.Title = "Кассовый расход нарастающим итогом с начала года";
                    e.Layout.Bands[0].Columns[1].Header.Title = "Кассовый расход за аналогичный период предыдущего года";
                    e.Layout.Bands[0].Columns[3].Header.Title = "Темп роста  кассового расхода к  аналогичному периоду предыдущего года";
                    
                }
                else
                    if (RList.SelectedIndex == 1)
                    {
                        e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Бюджетные назначения {0}", units);
                        e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Бюджетные назначения (прошлый год) {0}", units);
                        e.Layout.Bands[0].Columns[2].Header.Title = "Плановые назначения на текущий год";
                        e.Layout.Bands[0].Columns[1].Header.Title = "Плановые назначения на прошлый год";
                        e.Layout.Bands[0].Columns[3].Header.Title = "Темп роста  плановых расходов к  аналогичному периоду предыдущего года";
                    
                    }
                    else
                        if (RList.SelectedIndex == 2)
                        {
                            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Проект {0}", units);
                            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("Проект (прошлый год) {0}", units);
                            e.Layout.Bands[0].Columns[2].Header.Title = "Проект расходов на текущий год";
                            e.Layout.Bands[0].Columns[1].Header.Title = "Проект расходов на прошлый год";
                            e.Layout.Bands[0].Columns[3].Header.Title = "Темп роста  расходов к  аналогичному периоду предыдущего года";
                        }

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = "Расходы";
                ch.Title = "";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 1;
                ch.RowLayoutColumnInfo.SpanX = 3;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                ch = new ColumnHeader(true);
                ch.Caption = "Расходы бюджета";
                ch.Title = "";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 4;
                ch.RowLayoutColumnInfo.SpanX = 3;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int min = 0;

            for (int i = 3; i < e.Row.Cells.Count; i += 3)
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение доходов";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) > 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост доходов";
                    }
                    string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    e.Row.Cells[i].Style.CustomRules = style;
                }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[1].Value != null && (e.Row.Index == 0) &&
                       ((e.Row.Cells[1].Value.ToString().Contains("Все территории")) || (e.Row.Cells[1].Value.ToString().Contains("район"))))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            if ((e.Row.Cells[0].Value.ToString().Contains("доходы бюджетов бюджетной системы Российской") || e.Row.Cells[0].Value.ToString().Contains("возврат остатков субсидий и субвенций прошлых")) && (Convert.ToInt32(ComboYear.SelectedValue.ToString()) > 2010))
            {
                e.Row.Hidden = true;
            }
            if ((e.Row.Index < 5) || (e.Row.Index == 8) || (e.Row.Index == 14) || (e.Row.Index == 25) || (e.Row.Index == 26))
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            int min = 0;

            for (int i = 3; i < e.Row.Cells.Count; i += 3)
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение расходов";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) > 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост расходов";
                    }
                    string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    e.Row.Cells[i].Style.CustomRules = style;
                }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[1].Value != null && (e.Row.Index == 0) &&
                       ((e.Row.Cells[1].Value.ToString().Contains("Все территории")) || (e.Row.Cells[1].Value.ToString().Contains("район"))))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            if ((e.Row.Index < 6) || (e.Row.Index > 11))
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        #endregion

        #region Обработчики диаграмы

        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        }
        #endregion

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            ActiveGridRow(UltraWebGrid.Rows[0]);
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 65;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = label;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (e.CurrentWorksheet.Index == 0)
            {
                e.HeaderText = "Доходы бюджета";
            }
            else
                {
                    e.HeaderText = "Расходы бюджета";
                }
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 30;
            e.CurrentWorksheet.Columns[1].Width = width * 30;
            e.CurrentWorksheet.Columns[2].Width = width * 30;
            e.CurrentWorksheet.Columns[3].Width = width * 30;
            e.CurrentWorksheet.Columns[4].Width = width * 30;
            e.CurrentWorksheet.Columns[5].Width = width * 30;
            e.CurrentWorksheet.Columns[6].Width = width * 30;
            e.CurrentWorksheet.Columns[7].Width = width * 30;
            e.CurrentWorksheet.Columns[8].Width = width * 30;
            e.CurrentWorksheet.Columns[9].Width = width * 30;
            e.CurrentWorksheet.Columns[10].Width = width * 30;
            e.CurrentWorksheet.Columns[11].Width = width * 30;
            e.CurrentWorksheet.Columns[12].Width = width * 30;
            e.CurrentWorksheet.Columns[13].Width = width * 30;
            e.CurrentWorksheet.Columns[14].Width = width * 30;

            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                e.CurrentWorksheet.Rows[6].Cells[i].CellFormat.FillPattern = FillPatternStyle.None;
                for (int j = 5; j < 20; j++)
                {

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
            }
            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                if ((i == 3) || (i == 6))
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00%;[Red]-#,##0.00%";
                }
                else
                    if ((i == 5) || (i == 8) || (i == 9))
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0;[Red]-#,##0";
                    }
                    else
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                    }
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            for (int i = 3; i < 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 20 * 35;
            }
            e.CurrentWorksheet.Rows[5].Height = 20 * 15;
            for (int k = 1; k < columnCounttt; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private static void FillLevelCombo()
        {
            if (LevelsDictionary.Count > 0)
                return;
            LevelsDictionary.Add("Бюджет субъекта", 0);
            LevelsDictionary.Add("Конс.бюджет субъекта", 0);
            LevelsDictionary.Add("Конс.бюджет МО", 0);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Доходы");
            Worksheet sheet2 = workbook.Worksheets.Add("Расходы");
            Worksheet sheet3 = workbook.Worksheets.Add("Дин. доходов");
            Worksheet sheet4 = workbook.Worksheets.Add("Дин. расходов");
            sheet3.Rows[0].Cells[0].Value = ChartCaption1.Text;
            UltraGridExporter.ChartExcelExport(sheet3.Rows[1].Cells[0], UltraChart1);
            sheet4.Rows[0].Cells[0].Value = ChartCaption2.Text;
            UltraGridExporter.ChartExcelExport(sheet4.Rows[1].Cells[0], UltraChart2);
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet2);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

        }

        #endregion

        #region Экспорт в PDF
        ReportSection section1 = null;
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            section1 = new ReportSection(report, false);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section1);
        }
        bool IsExported = false;
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            if (IsExported) return;
            IsExported = true;
            IText title = section1.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = section1.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(label);

            title = section1.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(ChartCaption1.Text);

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            section1.AddImage(img);

            title = section1.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(ChartCaption2.Text);

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            img = UltraGridExporter.GetImageFromChart(UltraChart2);
            section1.AddImage(img);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {



        }

        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IBand AddBand()
        {
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public Infragistics.Documents.Reports.Report.List.IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(2560, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }



        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }
        
        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }
        
        #endregion
    }

}
