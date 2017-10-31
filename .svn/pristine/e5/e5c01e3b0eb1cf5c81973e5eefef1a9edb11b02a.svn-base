using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Excel;
using System.IO;
using System.Drawing.Imaging;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0011_02
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1_1;
        private DataTable dtChart1_2;
        private DataTable dtChart2;
        private int firstYear = 2008;
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;
        private string chartYear;
        private string chartMonth;
        private Bitmap bm1, bm2, bm3;
        private string growRateType;

        #endregion

        #region Параметры запроса

        // мера Исполнено
        private CustomParam factMeasure;
        // мера Темп роста
        private CustomParam rateMeasure;

        // группа КД
        private CustomParam kdGroupName;
        // выбранный регион
        private CustomParam selectedRegion;
        // Тип документа
        private CustomParam documentType;
        // Консолидированный уровень
        private CustomParam consolidateLevel;
        // Уровень бюджета СКИФ
        private CustomParam budgetSKIFLevel;
        // Уровень районов
        private CustomParam regionsLevel;

        // фильтр по годам
        private CustomParam filterYear;
        // Последний год
        private CustomParam lastYear;
        // Последний год
        private CustomParam periodMonth;

        // Доходы-Всего
        private CustomParam incomesTotal;

        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // Множество периодов для диаграммы с накоплением
        private CustomParam periodSet;

        #endregion

        public bool UseStack
        {
            get { return useStack.Checked; }
        }

        public bool QuarterValuation
        {
            get { return quarterValuation.Checked; }
        }

        public bool IsPreviousMonthGrowRate
        {
            get { return growRateType == "PreviousMonth"; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.50 - 130);

            UltraChart1_1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 3 - 25);
            UltraChart1_1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.45 - 110);

            UltraChart1_2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 6 - 25);
            UltraChart1_2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.45 - 110);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2 - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.45 - 110);

            #region Инициализация параметров запроса

            if (factMeasure == null)
            {
                factMeasure = UserParams.CustomParam("fact_measure");
            }
            if (rateMeasure == null)
            {
                rateMeasure = UserParams.CustomParam("rate_measure");
            }

            if (kdGroupName == null)
            {
                kdGroupName = UserParams.CustomParam("kd_group_name");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (documentType == null)
            {
                documentType = UserParams.CustomParam("document_type");
            }
            if (consolidateLevel == null)
            {
                consolidateLevel = UserParams.CustomParam("consolidate_level");
            }
            if (budgetSKIFLevel == null)
            {
                budgetSKIFLevel = UserParams.CustomParam("budget_SKIF_level");
            }
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }

            if (filterYear == null)
            {
                filterYear = UserParams.CustomParam("filter_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (periodMonth == null)
            {
                periodMonth = UserParams.CustomParam("period_month");
            }

            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }

            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }

            periodSet = UserParams.CustomParam("period_set");

            #endregion

            #region Настройка диаграммы

            UltraChart1_1.ChartType = ChartType.PieChart;
            UltraChart1_1.Border.Thickness = 0;
            UltraChart1_1.PieChart.OthersCategoryPercent = 0;
            UltraChart1_1.PieChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            UltraChart1_1.Tooltips.FormatString = "<ITEM_LABEL>\nфакт <DATA_VALUE:N2> тыс.руб.\nдоля <PERCENT_VALUE:N2>%";
            UltraChart1_1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1_1.Legend.Visible = true;
            UltraChart1_1.Legend.Location = LegendLocation.Left;
            UltraChart1_1.Legend.SpanPercentage = 53;
            UltraChart1_1.Legend.Margins.Top = 0;
            UltraChart1_1.Legend.Margins.Left = 0;

            UltraChart1_1.TitleTop.Text = "Конс. бюджет субъекта";
            UltraChart1_1.TitleTop.Font = new Font("Verdana", 8);
            UltraChart1_1.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart1_1.TitleTop.Margins.Left = Convert.ToInt32((UltraChart1_1.Width.Value)) * UltraChart1_1.Legend.SpanPercentage / 100 + 5;
            UltraChart1_1.TitleTop.Visible = true;

            //UltraChart1_1.PieChart.RadiusFactor = 90;
            UltraChart1_1.PieChart.StartAngle = 270;

            UltraChart1_1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1_1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_1_FillSceneGraph);

            UltraChart1_2.ChartType = ChartType.PieChart;
            UltraChart1_2.Border.Thickness = 0;
            UltraChart1_2.PieChart.OthersCategoryPercent = 0;
            UltraChart1_1.PieChart.RadiusFactor = 67;
            UltraChart1_2.PieChart.RadiusFactor = 70;
            UltraChart1_2.PieChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            UltraChart1_2.Tooltips.FormatString = "<ITEM_LABEL>\nфакт <DATA_VALUE:N2> тыс.руб.\nдоля <PERCENT_VALUE:N2>%";
            UltraChart1_2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1_2.TitleTop.Text = "Конс. бюджет МО";
            UltraChart1_2.TitleTop.Font = new Font("Verdana", 8);
            UltraChart1_2.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart1_2.TitleTop.Visible = true;

            UltraChart1_2.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1_2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_2_FillSceneGraph);

            UltraChart2.ChartType = ChartType.StackColumnChart;
            UltraChart2.StackChart.StackStyle = StackStyle.Normal;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            UltraChart2.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.TitleLeft.Text = "тыс.руб.";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.Axis.X.Extent = 20;
            UltraChart2.Axis.Y.Extent = 50;
            UltraChart2.TitleTop.Visible = true;
            UltraChart2.TitleTop.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart2.TitleTop.VerticalAlign = System.Drawing.StringAlignment.Near;

            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #endregion

            UltraChart1WebAsyncRefreshPanel.AddLinkedRequestTrigger(UltraWebGrid);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(UltraChart1_1);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(UltraChart1_2);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(UltraChart2);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(chartHeaderLabel);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 !=
                                                          "null"
                                                              ? string.Format(",{2}.[{1}].[ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ].[{0}],",
                                                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                                                    RegionSettingsHelper.Instance.IncomesKDRootName,
                                                                    RegionSettingsHelper.Instance.IncomesKDAllLevel)
                                                              : ",";

            if (!Page.IsPostBack)
            {
                GridWebAsyncRefreshPanel.AddRefreshTarget(UltraWebGrid);
                GridWebAsyncRefreshPanel.AddRefreshTarget(chartHeaderLabel);
                GridWebAsyncRefreshPanel.AddRefreshTarget(UltraChart1_1);
                GridWebAsyncRefreshPanel.AddRefreshTarget(UltraChart1_2);
                GridWebAsyncRefreshPanel.AddRefreshTarget(UltraChart2);
                GridWebAsyncRefreshPanel.LinkedRefreshControlID = UltraChart1WebAsyncRefreshPanel.ClientID;
                //GridWebAsyncRefreshPanel.AddLinkedRequestTrigger(useStack.ClientID);
                GridWebAsyncRefreshPanel.AddLinkedRequestTrigger(quarterValuation.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0011_02_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.SetСheckedState((endYear - 1).ToString(), true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingList());
                ComboKD.SetСheckedState("Доходы бюджета - Итого ", true);
            }

            string yearDescedants = string.Empty;
            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string periodLevel = QuarterValuation ? "Квартал" : "Месяц";
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];

                    yearDescedants +=
                        string.Format(
                                "{{[Период].[Период].[Данные всех периодов].[{0}] }} + Descendants ([Период].[Период].[Данные всех периодов].[{0}], [Период].[Период].[{1}], SELF) +",
                                year, periodLevel);
                    if (i == selectedValues.Count - 1)
                    {
                        lastYear.Value = year;
                    }
                }
            }
            filterYear.Value = yearDescedants.TrimEnd('+');
            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            growRateType = RegionSettingsHelper.Instance.GetPropertyValue("GrowRateType");
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            PopupInformer1.HelpPageUrl = IsPreviousMonthGrowRate ? "Default.html" : "Default_PrevYearGrowRate.html";

            kdGroupName.Value = ComboKD.SelectedValue;

            consolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            Page.Title = String.Format("Структурная динамика фактического поступления доходов по уровням бюджета: {0}", ComboKD.SelectedValue);
            PageTitle.Text = Page.Title;
            chartHeaderLabel.Text = String.Format("Структура поступлений доходов в консолидированный бюджет субъекта ({0})", ComboKD.SelectedValue);

            PageSubTitle.Text = string.Format("Оценка исполнения бюджетов за {0} {1}.", CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','), ComboYear.SelectedValues.Count == 1 ? "год" : "годы");
            SelectedKdCaption.Text = ComboKD.SelectedValue;

            factMeasure.Value = (!UseStack) ? "Факт_за период" : "Факт";
            rateMeasure.Value = (!UseStack) ? "Темп роста к аналогичному периоду предыдущего года_За период" : "Темп роста к аналогичному периоду предыдущего года_Факт";
            periodSet.Value = UseStack ? "Года" : "Месяцы текущего года";

            if (!(UltraChart1WebAsyncRefreshPanel.IsAsyncPostBack))
            {
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.DataBind();
                if (dtGrid.Rows.Count > 0)
                {
                    if (!Page.IsPostBack)
                    {
                        UltraWebGrid.Rows[UltraWebGrid.Rows.Count - 1].Activate();
                        BindPeriodMonth(UltraWebGrid.Rows[UltraWebGrid.Rows.Count - 1]);
                        UserParams.DataCount.Value = (UltraWebGrid.Rows.Count - 1).ToString();
                    }
                    else
                    {
                        CRHelper.SaveToErrorLog(UserParams.DataCount.Value);
                        if ((UltraWebGrid.Rows.Count - 1) > Convert.ToInt32(UserParams.DataCount.Value))
                        {
                            UltraWebGrid.Rows[Convert.ToInt32(UserParams.DataCount.Value)].Activate();
                            BindPeriodMonth(UltraWebGrid.Rows[Convert.ToInt32(UserParams.DataCount.Value)]);
                        }
                        else
                        { 
                            UltraWebGrid.Rows[Convert.ToInt32(UltraWebGrid.Rows.Count - 1)].Activate();
                            BindPeriodMonth(UltraWebGrid.Rows[Convert.ToInt32(UltraWebGrid.Rows.Count - 1)]);
                        }

                    }

                    UltraChart1_1.DataBind();
                    UltraChart1_2.DataBind();
                    UltraChart2.DataBind();

                    MemoryStream imageStream = new MemoryStream();
                    UltraChart1_1.SaveTo(imageStream, ImageFormat.Png);
                    bm1 = new Bitmap(imageStream);//ваша маленькая картинка

                    imageStream = new MemoryStream();
                    UltraChart1_2.SaveTo(imageStream, ImageFormat.Png);
                    bm2 = new Bitmap(imageStream);

                    imageStream = new MemoryStream();
                    UltraChart2.SaveTo(imageStream, ImageFormat.Png);

                    bm3 = new Bitmap(imageStream);

                }
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_02_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                if (IsPreviousMonthGrowRate)
                {
                    for (int i = 2; i < dtGrid.Rows.Count; i++)
                    {
                        int year;
                        if (!Int32.TryParse(dtGrid.Rows[i][0].ToString(), out year))
                        {
                            CalculateGrowTemp(i, 1);
                            CalculateGrowTemp(i, 3);
                            CalculateGrowTemp(i, 6);
                            CalculateGrowTemp(i, 9);
                            CalculateGrowTemp(i, 12);
                        }
                    }

                    int lastYearValue = Convert.ToInt32(ComboYear.SelectedValues[0]) - 1;

                    filterYear.Value = String.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", lastYearValue);
                    query = DataProvider.GetQueryText("FO_0002_0011_02_grid");
                    DataTable dt = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dt);

                    CalculateGrowTempFirstMonth(1, dt);
                    CalculateGrowTempFirstMonth(3, dt);
                    CalculateGrowTempFirstMonth(6, dt);
                    CalculateGrowTempFirstMonth(9, dt);
                    CalculateGrowTempFirstMonth(12, dt);
                }

                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i == 1 || i == 3 || i == 6 || i == 9 || i == 12)
                            && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        private void CalculateGrowTemp(int rowNum, int colNum)
        {
            if (dtGrid.Rows[rowNum][colNum] != DBNull.Value &&
                dtGrid.Rows[rowNum - 1][colNum] != DBNull.Value)
            {
                int year;
                if (!Int32.TryParse(dtGrid.Rows[rowNum - 1][0].ToString(), out year))
                {
                    double value = GetDoubleDividing(dtGrid.Rows[rowNum][colNum], dtGrid.Rows[rowNum - 1][colNum]);
                    if (value != Double.MaxValue)
                    {
                        dtGrid.Rows[rowNum][colNum + 1] = value;
                    }
                }
                else
                {
                    double value = GetDoubleDividing(dtGrid.Rows[rowNum][colNum], dtGrid.Rows[rowNum - 2][colNum]);
                    if (value != Double.MaxValue)
                    {
                        dtGrid.Rows[rowNum][colNum + 1] = value;
                    }
                }
            }
        }

        private void CalculateGrowTempFirstMonth(int colNum, DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return;

            double value = GetDoubleDividing(dtGrid.Rows[1][colNum], dt.Rows[0][colNum]);
            if (value != Double.MaxValue)
            {
                dtGrid.Rows[1][colNum + 1] = value;
            }
        }

        private Double GetDoubleDividing(object val1, object val2)
        {
            double dValue1 = GetDoubleValue(val1);
            double dValue2 = GetDoubleValue(val2);

            if (dValue1 != Double.MaxValue && dValue2 != Double.MaxValue && dValue2 != 0)
            {
                return dValue1 / dValue2;
            }

            return Double.MaxValue;
        }

        private Double GetDoubleValue(object value)
        {
            if (value != null && value.ToString() != String.Empty)
            {
                return Convert.ToDouble(value);
            }
            return Double.MaxValue;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");
            e.Layout.Bands[0].Columns[1].Width = 90;
            e.Layout.Bands[0].Columns[2].Width = 90;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "P2";
                int widthColumn = 100;

                int j = (i) % 3;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "N2";
                            widthColumn = 90;
                            break;
                        }
                    case 1:
                        {
                            formatString = "P2";
                            widthColumn = 90;
                            break;
                        }
                    case 2:
                        {
                            formatString = "P2";
                            widthColumn = 75;
                            break;
                        }

                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 90;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout.AddCell("Период");

            GridHeaderCell cell = headerLayout.AddCell("Консолидированный бюджет");
            cell.AddCell("Факт, тыс.руб.", "Исполнено за период", 2);
            cell.AddCell("Темп роста", IsPreviousMonthGrowRate ? "Темп роста к прошлому месяцу" : "Темп роста к аналогичному периоду прошлого года", 2);

            GridHeaderCell cell2 = headerLayout.AddCell("В том числе:");
            GridHeaderCell cellBudget = cell2.AddCell("Бюджет субъекта");
            cellBudget.AddCell("Факт, тыс.руб.", "Исполнено за период");
            cellBudget.AddCell("Темп роста", IsPreviousMonthGrowRate ? "Темп роста к прошлому месяцу" : "Темп роста к аналогичному периоду прошлого года");
            cellBudget.AddCell("Доля", "Удельный вес в консолидированном бюджете");

            cellBudget = cell2.AddCell("Бюджет городских округов");
            cellBudget.AddCell("Факт, тыс.руб.", "Исполнено за период");
            cellBudget.AddCell("Темп роста", IsPreviousMonthGrowRate ? "Темп роста к прошлому месяцу" : "Темп роста к аналогичному периоду прошлого года");
            cellBudget.AddCell("Доля", "Удельный вес в консолидированном бюджете");

            cellBudget = cell2.AddCell("Бюджет районов");
            cellBudget.AddCell("Факт, тыс.руб.", "Исполнено за период");
            cellBudget.AddCell("Темп роста", IsPreviousMonthGrowRate ? "Темп роста к прошлому месяцу" : "Темп роста к аналогичному периоду прошлого года");
            cellBudget.AddCell("Доля", "Удельный вес в консолидированном бюджете");

            cellBudget = cell2.AddCell("Бюджет поселений");
            cellBudget.AddCell("Факт, тыс.руб.", "Исполнено за период");
            cellBudget.AddCell("Темп роста", IsPreviousMonthGrowRate ? "Темп роста к прошлому месяцу" : "Темп роста к аналогичному периоду прошлого года");
            cellBudget.AddCell("Доля", "Удельный вес в консолидированном бюджете");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            UltraGridCell cell;

            SetCellIndicator(e.Row.Cells[2]);

            if (e.Row.Cells[2].Value != null && e.Row.Cells[2].Value.ToString() != string.Empty)
            {
                double val = Convert.ToDouble(e.Row.Cells[2].Value);
                e.Row.Cells[2].Value = string.Format("{0:P2}", val);
            }

            for (int i = 4; i < e.Row.Band.Columns.Count; i = i + 3)
            {
                cell = e.Row.Cells[i];
                SetCellIndicator(cell);

                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double val = Convert.ToDouble(e.Row.Cells[i].Value);
                    e.Row.Cells[i].Value = string.Format("{0:P2}", val);
                }
            }
            cell = e.Row.Cells[0];
            int value;
            if (Int32.TryParse(cell.Value.ToString(), out value))
            {
                foreach (UltraGridCell item in e.Row.Cells)
                {
                    item.Style.Font.Bold = true;
                    item.Row.Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
                }
            }
            SetNegativeRedColor(e.Row);
        }

        private static void SetNegativeRedColor(UltraGridRow row)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    double value;
                    if (Double.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            UltraGridRow row = e.Row;
            UserParams.DataCount.Value = e.Row.Index.ToString();
            BindPeriodMonth(row);
            UltraChart1_1.DataBind();
            UltraChart1_2.DataBind();
            UltraChart2.DataBind();


        }

        private void BindPeriodMonth(UltraGridRow row)
        {
            chartMonth = row.Cells[0].Value.ToString();
            int value;
            if (Int32.TryParse(chartMonth, out value))
            {
                chartYear = chartMonth;
                chartMonth = String.Empty;
                periodMonth.Value =
                String.Format("[{0}]", chartYear);
                lastYear.Value = chartYear;
                chartHeaderLabel.Text = String.Format("Структура поступлений доходов в консолидированный бюджет субъекта за {0} год", chartYear);
            }
            else if (chartMonth.ToLower().Contains("квартал"))
            {
                chartYear = row.Cells[row.Band.Columns.Count - 1].Value.ToString();
                string[] strs = chartMonth.Split(' ');
                if (strs.Length > 1)
                {
                    int quarterNum = Convert.ToInt32(strs[1]);
                    int halfYear = CRHelper.HalfYearNumByQuarterNum(quarterNum);
                    periodMonth.Value = String.Format("[{0}].[Полугодие {1}].[{2}]", chartYear, halfYear, chartMonth);
                    lastYear.Value = chartYear;
                    chartHeaderLabel.Text = String.Format("Структура поступлений доходов в консолидированный бюджет субъекта за {0} квартал {1} года",
                                      quarterNum, chartYear);
                }
            }
            else
            {
                chartYear = row.Cells[row.Band.Columns.Count - 1].Value.ToString();
                int monthNum = CRHelper.MonthNum(chartMonth);
                int halfYear = CRHelper.HalfYearNumByMonthNum(monthNum);
                int quater = CRHelper.QuarterNumByMonthNum(monthNum);
                periodMonth.Value = String.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", chartYear, halfYear, quater, chartMonth);
                lastYear.Value = chartYear;
                chartHeaderLabel.Text = String.Format("Структура поступлений доходов в консолидированный бюджет субъекта за {0} {1} года",
                                  CRHelper.ToLowerFirstSymbol(chartMonth), chartYear);
            }
        }

        private static void SetCellIndicator(UltraGridCell cell)
        {
            if (cell == null)
            {
                return;
            }
            double curVal;
            if (cell.Value != null && double.TryParse(cell.Value.ToString(), out curVal))
            {
                if (curVal > 1)
                {
                    cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    cell.Title = String.Format("Наблюдается рост доходов");
                }
                else if (curVal < 1)
                {
                    cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    cell.Title = String.Format("Наблюдается снижение доходов");
                }
            }
            cell.Style.CustomRules =
                            "background-repeat: no-repeat; padding-left: 10px; background-position: 10px center;";
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_02_chart1_1");
            dtChart1_1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1_1);

            foreach (DataRow row in dtChart1_1.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

            UltraChart1_1.DataSource = dtChart1_1;
        }

        protected void UltraChart1_2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_02_chart1_2");
            dtChart1_2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1_2);

            foreach (DataRow row in dtChart1_2.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

            UltraChart1_2.DataSource = dtChart1_2;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_02_chart2");
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            // CRHelper.NormalizeDataTable(dtSource);

            DataTable dtSource = dtChart2.Copy();
            dtSource.Columns.RemoveAt(2);
            dtSource.Columns.RemoveAt(3);
            dtSource.Columns.RemoveAt(4);
            dtSource.Columns.RemoveAt(5);
            dtSource.Columns.RemoveAt(6);
            foreach (DataColumn col in dtSource.Columns)
            {
                col.Caption = col.Caption.Split(';')[0];
                col.ColumnName = col.ColumnName.Split(';')[0];
            }
            /*foreach (DataRow row in dtSource.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }
            dtSource.AcceptChanges();*/
            UltraChart2.Series.Clear();
            for (int i = 1; i < dtSource.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtSource);
                UltraChart2.Series.Add(series);
            }
            UltraChart2.Data.SwapRowsAndColumns = true;

            UltraChart2.TitleTop.Text = UseStack ? String.Empty : String.Format("{0} год", chartYear);
            UltraChart2.Tooltips.FormatString = UseStack ? "<ITEM_LABEL>" : "<SERIES_LABEL> <ITEM_LABEL>";
        }

        void UltraChart1_1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Wedge)
                {
                    Wedge box = (Wedge)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0} {1}\n{2}", chartMonth, chartYear, box.DataPoint.Label).Trim(' ');
                    }
                }
            }
        }

        void UltraChart1_2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Wedge)
                {
                    Wedge box = (Wedge)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0} {1}\n{2}", chartMonth, chartYear, box.DataPoint.Label).Trim(' ');
                    }
                }
            }
        }

        void UltraChart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label != null)
                        {
                            int columnIndex = (box.Column + 1) * 2;
                            int rowIndex = box.Row;

                            if (dtChart2 != null && dtChart2.Rows.Count != 0 &&
                                dtChart2.Rows[rowIndex][columnIndex] != DBNull.Value &&
                                dtChart2.Rows[rowIndex][0] != DBNull.Value)
                            {
                                double percent;
                                Double.TryParse(dtChart2.Rows[rowIndex][columnIndex].ToString(), out percent);
                                string year = UseStack ? dtChart2.Rows[rowIndex][0].ToString() : chartYear;

                                box.DataPoint.Label =
                                        string.Format("{1}\n {0}\nфакт {2:N2} тыс.руб.\n доля {3:P2}",
                                        box.DataPoint.Label, year, dtChart2.Rows[rowIndex][columnIndex - 1], percent);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private Bitmap GetImageChart()
        {
            UltraChart2.Width = CRHelper.GetChartWidth(UltraChart2.Width.Value - 100);
            int picWidth = (int)(UltraChart1_1.Width.Value + 20 + UltraChart1_2.Width.Value + 20 + UltraChart2.Width.Value);
            int picHeight = (int)(UltraChart1_1.Height.Value);
            Bitmap bmNewImg = new Bitmap(picWidth, picHeight);//ваша нвоая картинка

            //MemoryStream imageStream = new MemoryStream();
            //UltraChart1_1.SaveTo(imageStream, ImageFormat.Png);
            //Bitmap bm1 = new Bitmap(imageStream);//ваша маленькая картинка

            //imageStream = new MemoryStream();
            //UltraChart1_2.SaveTo(imageStream, ImageFormat.Png);
            //Bitmap bm2 = new Bitmap(imageStream);

            //imageStream = new MemoryStream();
            //UltraChart2.SaveTo(imageStream, ImageFormat.Png);

            //Bitmap bm3 = new Bitmap(imageStream);

            Graphics g = Graphics.FromImage(bmNewImg);
            g.DrawImage(bm1, 0, 0, (int)UltraChart1_1.Width.Value, (int)UltraChart1_1.Height.Value);//туту нужно изменить под ваши задачи размещения картинок, координаты, области и тд
            g.DrawImage(bm2, (int)UltraChart1_1.Width.Value, 0, (int)UltraChart1_2.Width.Value,
                        (int)UltraChart1_2.Height.Value);
            g.DrawImage(bm3, (int)UltraChart1_1.Width.Value + (int)UltraChart1_2.Width.Value, 0, (int)UltraChart2.Width.Value,
                        (int)UltraChart2.Height.Value);

            g.Dispose();
            return bmNewImg;
        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");

            sheet1.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet2.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet2.Rows[1].Cells[0].Value = chartHeaderLabel.Text;
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(GetImageChart(), sheet2, 4);
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, PageTitle.Text + " " + PageSubTitle.Text, section1);
            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageTitle.Text + " " + PageSubTitle.Text);


            ReportPDFExporter1.Export(GetImageChart(), chartHeaderLabel.Text, section2);
        }
        #endregion

    }
}
