using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0013_02
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private int firstYear = 2000;
        private int endYear = 2011;

        private GridHeaderLayout headerLayout;

        private string chartQueryType;

        #endregion

        #region Параметры запроса

        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // доходы всего
        private CustomParam incomesAll;
        // расходы всего
        private CustomParam outcomesAll;

        // Уровень районов
        private CustomParam regionsLevel;

        // выбранный регион
        private CustomParam budgetLevel;

        // фильтр по годам
        private CustomParam filterYear;
        // Последний год
        private CustomParam periodYear;
        // Последний год
        private CustomParam periodLastYear;
        // Последний год
        private CustomParam periodMonth;
        private CustomParam chartFilter;

        #endregion

        private double rubMultiplier;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        private MemberAttributesDigest budgetDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.35 - 70);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.52 - 100);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.53 - 110);

            //  UltraChart1WebAsyncRefreshPanel.Width = CRHelper.GetChartWidth(2 * CustomReportConst.minScreenWidth / 4 - 25);
            // UltraChart1WebAsyncRefreshPanel.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.53 - 110);

            #region Инициализация параметров запроса

            if (regionsConsolidateBudget == null)
            {
                regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            }

            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }

            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }

            if (budgetLevel == null)
            {
                budgetLevel = UserParams.CustomParam("budget_level");
            }

            if (incomesAll == null)
            {
                incomesAll = UserParams.CustomParam("incomes_all");
            }
            if (outcomesAll == null)
            {
                outcomesAll = UserParams.CustomParam("outcomes_all");
            }


            if (filterYear == null)
            {
                filterYear = UserParams.CustomParam("filter_year");
            }
            if (periodLastYear == null)
            {
                periodLastYear = UserParams.CustomParam("period_last_year");
            }
            if (periodYear == null)
            {
                periodYear = UserParams.CustomParam("period_year");
            }
            if (periodMonth == null)
            {
                periodMonth = UserParams.CustomParam("period_month");
            }
            if (chartFilter == null)
            {
                chartFilter = UserParams.CustomParam("chart_filter");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", RubMultiplierCaption.ToLower());
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 160;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.SeriesLabels.FormatString = "<SERIES_LABEL>";
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 11;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) - UltraChart1.Axis.X.Extent;
            UltraChart1.TitleLeft.Text = RubMultiplierCaption;


            UltraChart2.Axis.X.Labels.ItemFormatString = String.Format("<ITEM_LABEL><SERIES_LABEL>\nфакт <DATA_VALUE:N2> {0}", RubMultiplierCaption.ToLower());

            // CRHelper.FillCustomColorModel(UltraChart1, 17, false);
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            UltraChart2.ChartType = ChartType.SplineChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Tooltips.FormatString = String.Format("<ITEM_LABEL> <SERIES_LABEL> г.\n<DATA_VALUE:N2> {0}", RubMultiplierCaption.ToLower());
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.2;
            UltraChart2.SplineChart.LineAppearances.Add(lineAppearance);

            EmptyAppearance emptyAppearance = new EmptyAppearance();
            emptyAppearance.EnablePoint = true;
            emptyAppearance.EnablePE = true;
            emptyAppearance.EnableLineStyle = true;
            emptyAppearance.PointStyle.Icon = SymbolIcon.Circle;
            emptyAppearance.PointStyle.IconSize = SymbolIconSize.Large;
            emptyAppearance.LineStyle.MidPointAnchors = true;
            UltraChart2.SplineChart.EmptyStyles.Add(emptyAppearance);

            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.SplineChart.NullHandling = NullHandling.DontPlot;

            UltraChart2.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart1.Width.Value) / 4;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 11;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart2.Height.Value) - UltraChart2.Axis.X.Extent;
            UltraChart2.TitleLeft.Text = RubMultiplierCaption;

            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #endregion

            UltraChart1WebAsyncRefreshPanel.AddLinkedRequestTrigger(UltraWebGrid);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(UltraChart1);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(UltraChart2);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(chartHeaderLabel);
            UltraChart1WebAsyncRefreshPanel.AddRefreshTarget(chart2HeaderLabel);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.RegionsLocalBudgetLevel;

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            #region Комбики
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0013_02_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboBudgetLevel.Title = "Уровень бюджета";
                ComboBudgetLevel.Width = 400;
                ComboBudgetLevel.MultiSelect = false;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboBudgetLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }
            #endregion
            string yearDescedants = string.Empty;

            string year = ComboYear.SelectedValue;
            int lastYear = Convert.ToInt32(year) - 1;

            yearDescedants += string.Format("Descendants ([Период].[Период].[Данные всех периодов].[{0}], [Период].[Период].[Месяц], SELF) +" +
                        "Descendants ([Период].[Период].[Данные всех периодов].[{1}], [Период].[Период].[Месяц], SELF)",
                        year, lastYear);

            filterYear.Value = yearDescedants.TrimEnd('+');

            periodLastYear.Value = lastYear.ToString();
            periodYear.Value = year;
            periodMonth.Value = MakePeriodMonth(ComboMonth.SelectedValue);


            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            incomesAll.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesAll.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            switch (ComboBudgetLevel.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        budgetLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
                        break;
                    }
                default:
                    {
                        budgetLevel.Value = budgetDigest.GetMemberUniqueName(ComboBudgetLevel.SelectedValue);
                        break;
                    }
            }

            Page.Title = string.Format("Динамика исполнения бюджета в разрезе отдельных показателей: {0}", ComboBudgetLevel.SelectedValue);
            PageTitle.Text = Page.Title;
            string stateYear = ComboMonth.SelectedIndex == 11
                                   ? (Convert.ToInt32(ComboYear.SelectedValue) + 1).ToString()
                                   : ComboYear.SelectedValue;
            PageSubTitle.Text = string.Format("По состоянию на 1 {0} {1} года.", CRHelper.RusMonthGenitive(ComboMonth.SelectedIndex + 2), stateYear);
            chartFilter.Value = "Доходы ";
            chartQueryType = "incomes";
            UltraWebGrid.Bands.Clear();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
            if (dtGrid.Rows.Count > 0)
            {
                if (!Page.IsPostBack)
                {
                    UltraWebGrid.Rows[0].Activate();
                }
                if (!UltraChart1WebAsyncRefreshPanel.IsAsyncPostBack)
                {
                    UltraChart1.DataBind();
                    UltraChart2.DataBind();
                    chartHeaderLabel.Text = String.Format("Динамика основных показателей исполнения местных бюджетов: {0} за {1} {2} {3} года.",
                              chartFilter.Value, ComboMonth.SelectedIndex + 1,
                              CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), ComboYear.SelectedValue);
                    chart2HeaderLabel.Text = String.Format("Помесячная динамика основных показателей исполнения: {0}, {1}.",
                                                           chartFilter.Value, ComboBudgetLevel.SelectedValue);
                }
            }
        }

        private string MakePeriodMonth(string month)
        {
            int monthNum = CRHelper.MonthNum(month);
            int halfYear = CRHelper.HalfYearNumByMonthNum(monthNum);
            int quater = CRHelper.QuarterNumByMonthNum(monthNum);
            return String.Format("[Полугодие {0}].[Квартал {1}].[{2}]", halfYear, quater, month);
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0013_02_grid_1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);

            // удаляем первую строку
            if (dt.Rows.Count > 0)
            {
                dt.Rows.RemoveAt(0);
            }

            dtGrid = dt.Copy();

            dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0013_02_grid_2");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);
            foreach (DataRow row in dt.Rows)
            {
                DataRow dtRow = dtGrid.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dtRow[i] = row[i];
                }
                dtGrid.Rows.Add(dtRow);
            }
            dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0013_02_grid_3");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);
            foreach (DataRow row in dt.Rows)
            {
                DataRow dtRow = dtGrid.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dtRow[i] = row[i];
                }
                dtGrid.Rows.Add(dtRow);
            }
            dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0013_02_grid_4");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dt);
            foreach (DataRow row in dt.Rows)
            {
                DataRow dtRow = dtGrid.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!(i == 3 || i == 6))
                    {
                        dtRow[i] = row[i];
                    }
                }
                dtGrid.Rows.Add(dtRow);
            }
            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (!(i == 3 || i == 6)
                        && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N2";
                int widthColumn = 100;

                int j = (i) % 3;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "P2";
                            widthColumn = 100;
                            break;
                        }
                    case 2:
                        {
                            formatString = "N2";
                            widthColumn = 100;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 290;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

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
            headerLayout.AddCell("Показатели");
            GridHeaderCell cell = headerLayout.AddCell(String.Format("За {0} {1} {2} года",
                                                                     ComboMonth.SelectedIndex + 1,
                                                                     CRHelper.RusManyMonthGenitive(
                                                                         ComboMonth.SelectedIndex + 1),
                                                                         (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString()));
            cell.AddCell(String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()),
                         String.Format("За {0} {1} {2} года",
                                       ComboMonth.SelectedIndex + 1,
                                       CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1),
                                       (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString()));
            cell.AddCell(String.Format("Исполнено прошлый год, {0}", RubMultiplierCaption.ToLower()),
                         "Исполнено за аналогичный период предыдущего года");
            cell.AddCell("Темп роста", "Темп роста к прошлому году");
            cell = headerLayout.AddCell(String.Format("За {0} {1} {2} года",
                                                         ComboMonth.SelectedIndex + 1,
                                                         CRHelper.RusManyMonthGenitive(
                                                             ComboMonth.SelectedIndex + 1),
                                                             ComboYear.SelectedValue));
            cell.AddCell(String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()),
                         String.Format("За {0} {1} {2} года",
                                       ComboMonth.SelectedIndex + 1,
                                       CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1),
                                       ComboYear.SelectedValue));
            cell.AddCell(String.Format("Исполнено прошлый год, {0}", RubMultiplierCaption.ToLower()),
                         "Исполнено за аналогичный период предыдущего года");
            cell.AddCell("Темп роста", "Темп роста к прошлому году");
            headerLayout.ApplyHeaderInfo();
            /*
            
            int multiHeaderPos = 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 2; i = i + 3)
            {
                string year = multiHeaderPos == 1 ? (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString() : ComboYear.SelectedValue;
                ColumnHeader ch = new ColumnHeader(false);
                ch.Caption = String.Format("За {0} {1} {2} года",
                                           ComboMonth.SelectedIndex + 1,
                                           CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), year);
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 3;
                ch.RowLayoutColumnInfo.SpanX = 3;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()), String.Format("За {0} {1} {2} года",
                                           ComboMonth.SelectedIndex + 1,
                                           CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, String.Format("Исполнено прошлый год, {0}", RubMultiplierCaption.ToLower()),
                    "Исполнено за аналогичный период предыдущего года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Темп роста", "Темп роста к прошлому году");
            }*/
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 3; i < e.Row.Band.Columns.Count; i = i + 3)
            {
                UltraGridCell cell = e.Row.Cells[i];
                SetCellIndicator(cell);
            }
            string kind = e.Row.Cells[0].Value.ToString();
            if (kind == "Доходы " ||
                kind == "Расходы " ||
                kind == "Дефицит/профицит ")
            {
                foreach (UltraGridCell item in e.Row.Cells)
                {
                    item.Style.Font.Bold = true;
                    item.Row.Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
                }
            }
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            UltraGridRow row = e.Row;
            chartFilter.Value = row.Cells[0].Value.ToString();

            if (e.Row.Index < 3)
            {
                chartQueryType = "incomes";
            }
            else if (e.Row.Index == 7)
            {
                chartQueryType = "deficite";
            }
            else if (e.Row.Index == 3)
            {
                chartQueryType = "outcomes_all";
            }
            else
            {
                chartQueryType = "outcomes";
            }
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            chartHeaderLabel.Text =
                    String.Format("Динамика основных показателей исполнения местных бюджетов: {0} за {1} {2} {3} года.",
                              chartFilter.Value, ComboMonth.SelectedIndex + 1,
                              CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), ComboYear.SelectedValue);
            chart2HeaderLabel.Text = String.Format("Помесячная динамика основных показателей исполнения: {0}, {1}.",
                                                   chartFilter.Value, ComboBudgetLevel.SelectedValue);
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
                //                switch(cell.Column.Index)
                //                {
                //                    case 1:
                //                    case 2:
                //                    case 4:
                //                    case 5:
                //                        {
                //                            cell.Value = Convert.ToDouble(cell.Value).ToString("N2");
                //                            break;
                //                        }
                //                    case 3:
                //                    case 6:
                //                        {
                //                            cell.Value = Convert.ToDouble(cell.Value).ToString("P2");
                //                            break;
                //                        }
                //                }

                if (curVal > 1)
                {
                    cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    cell.Title = String.Format("Прирост к прошлому году");
                }
                else if (curVal < 1)
                {
                    cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    cell.Title = String.Format("Падение к прошлому году");
                }
            }
            cell.Style.CustomRules =
                            "background-repeat: no-repeat; padding-left: 10px; background-position: 10px center;";
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string queryName = "FO_0002_0013_02_chart1_" + chartQueryType;
            string query = DataProvider.GetQueryText(queryName);
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            foreach (DataColumn col in dtChart1.Columns)
            {
                col.ColumnName = col.ColumnName.Replace("\"", "'");
                col.ColumnName = col.ColumnName.Replace("Городской округ", "ГО");
                col.ColumnName = col.ColumnName.Replace("городской округ", "ГО");
                col.ColumnName = col.ColumnName.Replace("муниципальное образование", "МО");
                col.ColumnName = col.ColumnName.Replace("муниципальный район", "МР");
                col.ColumnName = col.ColumnName.Replace("Муниципальный район", "МР");
                col.ColumnName = col.ColumnName.Replace("район", "р-н");
            }

            double maxValue = double.MinValue;

            foreach (DataRow row in dtChart1.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        double value = Convert.ToDouble(row[i]) / rubMultiplier;

                        // пока только для костромы, потом нужно вывести в настройки
                        if (value > maxValue && !dtChart1.Columns[i].ToString().ToLower().Contains("кострома"))
                        {
                            maxValue = value;
                        }

                        row[i] = value;
                    }
                }
            }

            if (maxValue != double.MinValue)
            {
                UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
                UltraChart1.Axis.Y.RangeMax = maxValue + maxValue / 10;
            }
            else
            {
                UltraChart1.Axis.Y.RangeType = AxisRangeType.Automatic;
            }

            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.DataSource = dtChart1;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string queryName = "FO_0002_0013_02_chart2_" + chartQueryType;
            string query = DataProvider.GetQueryText(queryName);
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);
            dtChart2 = new DataTable();
            dtChart2.Columns.Add(new DataColumn("Год", typeof(string)));
            for (int i = 1; i <= 12; i++)
            {
                dtChart2.Columns.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), typeof(double));
            }

            string year = dt.Rows[0][1].ToString();
            DataRow row = dtChart2.NewRow();
            row[0] = year;
            int rowNum = 0;
            while (dt.Rows[rowNum][1].ToString() == year)
            {
                if (dt.Rows[rowNum][2] != DBNull.Value)
                {
                    row[rowNum + 1] = Convert.ToDouble(dt.Rows[rowNum][2]) / rubMultiplier;
                }
                rowNum++;
            }
            dtChart2.Rows.Add(row);
            row = dtChart2.NewRow();
            row[0] = dt.Rows[rowNum][1].ToString();

            for (int i = 1; i < 13; i++)
            {
                if (dt.Rows[rowNum][2] != DBNull.Value)
                {
                    row[i] = Convert.ToDouble(dt.Rows[rowNum][2]) / rubMultiplier;
                }
                rowNum++;
            }
            dtChart2.Rows.Add(row);
            UltraChart2.DataSource = dtChart2;
        }

        void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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
                        if (box.DataPoint.Label != null)
                        {
                            string seriesYear = box.DataPoint.Label == "Исполнено"
                                                    ? periodYear.Value
                                                    : periodLastYear.Value;
                            box.DataPoint.Label = String.Format("{0}: исполнено за {1} {2} {3} года.",
                                box.Series.Label, ComboMonth.SelectedIndex + 1,
                                CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), seriesYear);
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            sheet1.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet2.Rows[0].Cells[0].Value = chartHeaderLabel.Text;
            sheet3.Rows[0].Cells[0].Value = chart2HeaderLabel.Text + " " + PageSubTitle.Text;

            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, sheet2, 4);
            ReportExcelExporter1.Export(UltraChart2, sheet3, 4);

        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;
            IText text = section1.AddText();
            text.AddContent(" ");
            ReportPDFExporter1.Export(headerLayout, PageSubTitle.Text, section1);

            ISection section2 = report.AddSection();
            //Первая диаграмма
            /*title = section2.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartHeaderLabel.Text);*/

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart1.Legend.Margins.Right = 5;
            ReportPDFExporter1.Export(UltraChart1,chartHeaderLabel.Text, section2);

            //Вторая диаграмма
            ISection section3 = report.AddSection();
            /*title = section3.AddText();
            font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chart2HeaderLabel.Text + " " + PageSubTitle.Text);*/

            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart2.Legend.Margins.Right = 5;
            ReportPDFExporter1.Export(UltraChart2,chart2HeaderLabel.Text + " " + PageSubTitle.Text, section3);
        }
        #endregion


    }
}
