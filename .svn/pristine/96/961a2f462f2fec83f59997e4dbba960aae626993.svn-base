using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0032
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtStackColumnChart = new DataTable();
        private DataTable dtLineChart = new DataTable();
        private DataTable dtExecutePercentChart = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private GridHeaderLayout headerLayout;
        private DateTime currentDate;
        private DateTime nextMonthDate;
        private string rubMultiplierCaption;
        private string FilterParams;

        #region Параметры запроса

        // выбранный район
        private CustomParam selectedRegion;
        // доходы Итого
        private CustomParam incomesTotal;
        // расходы Итого
        private CustomParam outcomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для собственного бюджета
        private CustomParam ownBudgetDocumentSKIFType;

        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // список доходов для таблицы и даиграммы
        private CustomParam kdList;
        // множитель рублей
        private CustomParam rubMultiplier;

        // использовать куб [ФО_Первоначальный план доходов] для колонки уточненного плана
        private CustomParam originalPlanEnable;

        // фильтр по КОСГУ
        private CustomParam kosguFilter;

        #endregion

        private MemberAttributesDigest budgetDigest;

        private bool IsConsolidateBudgetSelected
        {
            get
            {
                return ComboRegion.SelectedValue == "Консолидированный бюджет субъекта" ||
                    ComboRegion.SelectedValue == "Бюджет субъекта" || ComboRegion.SelectedValue == "Собственный бюджет субъекта";
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedRegion = UserParams.CustomParam("selected_region");
            incomesTotal = UserParams.CustomParam("incomes_total");
            outcomesTotal = UserParams.CustomParam("outcomes_total");
            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            kdList = UserParams.CustomParam("kd_list");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            originalPlanEnable = UserParams.CustomParam("original_plan_enable");

            kosguFilter = UserParams.CustomParam("kosgu_filter");
            ownBudgetDocumentSKIFType = UserParams.CustomParam("own_budget_document_skif_type");

            #endregion

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 235);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(850 * 0.8);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = true;
            CrossLink.Text = "Сравнение&nbsp;плановых&nbsp;показателей<br/>по&nbsp;налоговым&nbsp;доходам";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0031/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            originalPlanEnable.Value = RegionSettingsHelper.Instance.GetPropertyValue("OriginalPlanEnable");

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0032_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboRegion.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);
            nextMonthDate = currentDate.AddMonths(1);

            Page.Title = "Сравнение плановых показателей по основным параметрам бюджетов";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{0} на {1} год по состоянию на 1 {2} {3} года, тыс.руб.",
                ComboRegion.SelectedValue, currentDate.Year, CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year);
            CommentTextLabel.Text = String.Empty;
            chartHeaderLabel.Text = String.Format("Основные параметры бюджета ({0})", ComboRegion.SelectedValue);

            kdList.Value = IsConsolidateBudgetSelected ? "[Список КД для конс.бюджета]" : "[Список КД для МО]";
            rubMultiplier.Value = IsConsolidateBudgetSelected ? "1000000000" : "1000000";
            rubMultiplierCaption = IsConsolidateBudgetSelected ? "млрд.руб." : "млн.руб.";

            switch (ComboRegion.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        selectedRegion.Value = String.Format("{0}.[Консолидированный бюджет субъекта]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Бюджет субъекта":
                case "Собственный бюджет субъекта":
                    {
                        selectedRegion.Value = String.Format("{0}.[Собственный бюджет субъекта]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Местные бюджеты":
                    {
                        selectedRegion.Value = String.Format("{0}.[Местные бюджеты]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                default:
                    {
                        selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
                        break;
                    }
            }

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            kosguFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("KosguFilter");
            ownBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("OwnBudgetDocumentSKIFType");

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart.DataBind();
            SetupChart();
            FilterParams = string.Format("за {0} {1} года, бюджет: {2}", ComboMonth.SelectedValue, ComboYear.SelectedValue, ComboRegion.SelectedValue);
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0032_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            headerLayout.AddCell("Показатель");
            headerLayout.AddCell("План на год (первоначальный)");

            string currentMonthName = String.Empty;
            GridHeaderCell monthCell = new GridHeaderCell();
            for (int i = 1; i < columnCount - 2; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;
                string headerHint = GetHeaderHint(columnCaption);

                string formatString = GetColumnFormat(columnCaption);
                int widthColumn = GetColumnWidth(columnCaption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                if (i > 1)
                {
                    string[] nameParts = columnCaption.Split(';');
                    string monthName = nameParts[0];
                    string measureName = nameParts[1];

                    if (currentMonthName == String.Empty || currentMonthName != monthName)
                    {
                        monthCell = headerLayout.AddCell(monthName);
                    }
                    currentMonthName = monthName;
                    monthCell.AddCell(measureName, headerHint);
                }
            }

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("факт") || columnName.ToLower().Contains("план") || columnName.ToLower().Contains("отклонение"))
            {
                return "N1";
            }
            return "P2";
        }

        private static int GetColumnWidth(string columnName)
        {
            if (columnName.ToLower().Contains("отклонение") || columnName.ToLower().Contains("первоначальный"))
            {
                return 120;
            }
            if (columnName.ToLower().Contains("факт") || columnName.ToLower().Contains("план"))
            {
                return 90;
            }
            return 100;
        }

        private string GetHeaderHint(string columnName)
        {
            if (columnName.ToLower().Contains("отклонение от первоначального плана"))
            {
                return "Отклонение от первоначального плана";
            }
            if (columnName.ToLower().Contains("факт") || columnName.ToLower().Contains("план"))
            {
                return GetDateString(columnName.Split(';')[0]);
            }
            if (columnName.ToLower().Contains("отклонение от предыдущего месяца"))
            {
                return "Отклонение от предыдущего месяца";
            }
            return String.Empty;
        }

        private string GetDateString(string monthName)
        {
            int monthNum = CRHelper.MonthNum(monthName.ToLower());
            DateTime date = new DateTime(currentDate.Year, monthNum, 1);
            DateTime nextMonth = date.AddMonths(1);

            return String.Format("на {0} год по состоянию на 1 {1} {2} года", nextMonth.Year, CRHelper.RusMonthGenitive(nextMonth.Month), nextMonth.Year);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int columnCount = e.Row.Cells.Count;
            int levelColumnIndex = columnCount - 2;
            int emptyPlanColumnIndex = columnCount - 1;

            string level = String.Empty;
            if (e.Row.Cells[levelColumnIndex].Value != null)
            {
                level = e.Row.Cells[levelColumnIndex].Value.ToString();
            }

            string emptyPlanHint = String.Empty;
            if (e.Row.Cells[emptyPlanColumnIndex].Value != null)
            {
                emptyPlanHint = e.Row.Cells[emptyPlanColumnIndex].Value.ToString();
            }

            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != String.Empty)
            {
                rowName = e.Row.Cells[0].Value.ToString();
                if (CommentTextLabel.Text == String.Empty)
                {
                    CommentTextLabel.Text = "* - в качестве первоначального плана используются уточненные годовые назначения января";
                }
            }

            e.Row.Cells[0].Value = String.Format("{0}{1}", rowName, emptyPlanHint);

            for (int i = 0; i < e.Row.Cells.Count - 2; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];

                if (level == "0")
                {
                    cell.Style.Font.Bold = true;
                }

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

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0032_stackColumnChart");
            dtStackColumnChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtStackColumnChart);
            if (dtStackColumnChart.Columns.Count > 0)
            {
                dtStackColumnChart.Columns.RemoveAt(0);
            }

            query = DataProvider.GetQueryText("FO_0002_0032_lineChart");
            dtLineChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtLineChart);
            if (dtLineChart.Columns.Count > 0)
            {
                dtLineChart.Columns.RemoveAt(0);
            }

            query = DataProvider.GetQueryText("FO_0002_0032_executePercentChart");
            dtExecutePercentChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtExecutePercentChart);
            if (dtExecutePercentChart.Columns.Count > 0)
            {
                dtExecutePercentChart.Columns.RemoveAt(0);
            }
        }

        private void SetupChart()
        {
            UltraChart.ChartType = ChartType.Composite;
            UltraChart.BorderWidth = 0;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = CRHelper.ToUpperFirstSymbol(rubMultiplierCaption);
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);

            UltraChart.Legend.MoreIndicatorText = " ";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LightGreen;
            Color color2 = Color.DodgerBlue;
            Color color3 = Color.Gold;
            Color color4 = Color.LawnGreen;
            Color color5 = Color.DarkViolet;

            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            UltraChart.ColorModel.Skin.ApplyRowWise = false;

            ChartArea area = new ChartArea();
            area.Border.Thickness = 0;
            UltraChart.CompositeChart.ChartAreas.Add(area);

            AxisItem axisX = new AxisItem();
            axisX.OrientationType = AxisNumber.X_Axis;
            axisX.SetLabelAxisType = SetLabelAxisType.GroupBySeries;
            axisX.DataType = AxisDataType.String;
            axisX.LineThickness = 1;
            axisX.Extent = 150;
            axisX.Key = "X";
            axisX.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            AxisItem axisX2 = new AxisItem();
            axisX2.OrientationType = AxisNumber.X_Axis;
            axisX2.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            axisX2.DataType = AxisDataType.String;
            axisX2.Visible = false;
            axisX2.Labels.Visible = false;
            axisX2.Key = "X2";

            AxisItem axisY = new AxisItem();
            axisY.OrientationType = AxisNumber.Y_Axis;
            axisY.DataType = AxisDataType.Numeric;
            axisY.LineThickness = 1;
            axisY.Extent = 60;
            axisY.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            axisY.Labels.HorizontalAlign = StringAlignment.Far;
            axisY.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            axisY.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            axisY.Labels.Layout.Padding = 5;
            axisY.TickmarkStyle = AxisTickStyle.Smart;

            AxisItem axisY2 = new AxisItem();
            axisY2.OrientationType = AxisNumber.Y2_Axis;
            axisY2.DataType = AxisDataType.Numeric;
            axisY2.LineThickness = 1;
            axisY2.Extent = 60;
            axisY2.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            axisY2.Labels.HorizontalAlign = StringAlignment.Far;
            axisY2.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            axisY2.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            axisY2.Labels.Layout.Padding = 5;
            axisY2.TickmarkStyle = AxisTickStyle.Percentage;
            axisY2.RangeType = AxisRangeType.Custom;
            axisY2.RangeMax = 100;
            axisY2.RangeMin = 0;

            AxisItem hiddenAxisX2 = new AxisItem();
            hiddenAxisX2.OrientationType = AxisNumber.X2_Axis;
            hiddenAxisX2.Extent = 20;
            hiddenAxisX2.Labels.Visible = false;
            hiddenAxisX2.LineThickness = 0;
            hiddenAxisX2.Margin.Near.Value = 10;
            hiddenAxisX2.Margin.Far.Value = 10;
            hiddenAxisX2.Visible = true;

            area.Axes.Add(axisX);
            area.Axes.Add(axisX2);
            area.Axes.Add(axisY);
            area.Axes.Add(axisY2);
            area.Axes.Add(hiddenAxisX2);

            ChartLayerAppearance layer1 = new ChartLayerAppearance();
            ChartLayerAppearance layer2 = new ChartLayerAppearance();
            ChartLayerAppearance layer3 = new ChartLayerAppearance();

            layer1.ChartType = ChartType.StackColumnChart;
            ((ColumnChartAppearance)layer1.ChartTypeAppearance).ColumnSpacing = 1;
            ((ColumnChartAppearance)layer1.ChartTypeAppearance).ChartText.Add(GetAllChartText());

            layer2.ChartType = ChartType.LineChart;
            LineAppearance emptylineAppearance = new LineAppearance();
            emptylineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            emptylineAppearance.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            emptylineAppearance.IconAppearance.PE.Fill = Color.Red;
            emptylineAppearance.Thickness = 0;
            ((LineChartAppearance)layer2.ChartTypeAppearance).LineAppearances.Add(emptylineAppearance);

            layer3.ChartType = ChartType.LineChart;
            LineAppearance incomesLineAppearance = new LineAppearance();
            incomesLineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            incomesLineAppearance.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            incomesLineAppearance.Thickness = 0;
            ((LineChartAppearance)layer3.ChartTypeAppearance).LineAppearances.Add(incomesLineAppearance);
            ((LineChartAppearance)layer3.ChartTypeAppearance).NullHandling = NullHandling.DontPlot;

            for (int i = 1; i < dtStackColumnChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtStackColumnChart);
                UltraChart.CompositeChart.Series.Add(series);
                //series.Label = "stack";
                layer1.Series.Add(series);
            }

            for (int i = 1; i < dtLineChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtLineChart);
                UltraChart.CompositeChart.Series.Add(series);
                layer2.Series.Add(series);
            }

            for (int i = 1; i < dtExecutePercentChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtExecutePercentChart);
                UltraChart.CompositeChart.Series.Add(series);
                layer3.Series.Add(series);
            }

            layer1.ChartArea = area;
            layer1.AxisX = axisX;
            layer1.AxisY = axisY;
            layer1.LegendItem = LegendItemType.Point;
            layer1.SwapRowsAndColumns = true;

            layer2.ChartArea = area;
            layer2.AxisX = axisX2;
            layer2.AxisY = axisY;
            layer2.LegendItem = LegendItemType.Series;

            layer3.ChartArea = area;
            layer3.AxisX = axisX2;
            layer3.AxisY = axisY2;
            layer3.LegendItem = LegendItemType.Series;

            UltraChart.CompositeChart.ChartLayers.Add(layer1);
            UltraChart.CompositeChart.ChartLayers.Add(layer2);
            UltraChart.CompositeChart.ChartLayers.Add(layer3);

            CompositeLegend compositeLegend = new CompositeLegend();
            compositeLegend.ChartLayers.Add(layer2);
            compositeLegend.ChartLayers.Add(layer3);
            compositeLegend.ChartLayers.Add(layer1);
            compositeLegend.PE.ElementType = PaintElementType.SolidFill;
            compositeLegend.PE.Fill = Color.FloralWhite;
            compositeLegend.BoundsMeasureType = MeasureType.Percentage;
            compositeLegend.Bounds = new Rectangle(2, 90, 49 , 9);//48 -width
            compositeLegend.LabelStyle.Font = new Font("Verdana", 10);
            UltraChart.CompositeChart.Legends.Add(compositeLegend);
        }

        private ChartTextAppearance GetAllChartText()
        {
            ChartTextAppearance chartText = new ChartTextAppearance();
            chartText.Column = -2;
            chartText.Row = -2;
            chartText.VerticalAlign = StringAlignment.Center;
            chartText.HorizontalAlign = StringAlignment.Center;
            chartText.ItemFormatString = "<DATA_VALUE_ITEM:N1>";
            chartText.ChartTextFont = new Font("Verdana", 8);
            chartText.Visible = true;

            return chartText;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)UltraChart.CompositeChart.ChartLayers[0].ChartLayer.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)UltraChart.CompositeChart.ChartLayers[0].ChartLayer.Grid["Y"];

            if (xAxis == null)
                return;

            double yMin = yAxis.MapMinimum;
            double xMin = xAxis.MapMinimum;
            double xMax = xAxis.MapMaximum;

            double axisStep = xAxis.Map(1) - xAxis.Map(0);
            double lineX = xMin + 3.5 * axisStep;
            double lineY = UltraChart.Height.Value - 80;

            double textX = xMin - 0.5 * axisStep;
            double textY = UltraChart.Height.Value - 100;

            Line line = new Line();
            line.p1 = new Point((int)xMin, (int)yMin);
            line.p2 = new Point((int)xMin, (int)lineY);
            line.PE.Fill = Color.Black;
            line.lineStyle.DrawStyle = LineDrawStyle.Solid;
            e.SceneGraph.Add(line);

            line = new Line();
            line.p1 = new Point((int)xMax, (int)yMin);
            line.p2 = new Point((int)xMax, (int)lineY);
            line.PE.Fill = Color.Black;
            line.lineStyle.DrawStyle = LineDrawStyle.Solid;
            e.SceneGraph.Add(line);

            int monthCount = dtStackColumnChart.Rows.Count / 2;

            for (int i = 0; i < monthCount; i++)
            {
                Text rankText = new Text();
                rankText.bounds = new Rectangle((int)textX, (int)textY, 4 * (int)axisStep, 20);
                rankText.SetTextString(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i + 1)));
                LabelStyle labelStyle = new LabelStyle();
                labelStyle.Font = new Font("Verdana", 8);
                labelStyle.HorizontalAlign = StringAlignment.Center;
                rankText.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(rankText);

                if (lineX <= xMax)
                {
                    line = new Line();
                    line.p1 = new Point((int)lineX, (int)yMin);
                    line.p2 = new Point((int)lineX, (int)lineY);
                    line.PE.Fill = Color.Black;
                    line.lineStyle.DrawStyle = LineDrawStyle.Solid;
                    e.SceneGraph.Add(line);
                }

                textX += 4 * axisStep;
                lineX += 4 * axisStep;
            }

            int legendItem = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline poly = (Polyline)primitive;

                    if (poly.Series != null &&
                        (poly.Series.Label == "Бюджет откорректированный" || poly.Series.Label == "Процент исполнения по доходам" || poly.Series.Label == "Процент исполнения по расходам"))
                    {
                        poly.PE.StrokeWidth = 0;
                        poly.PE.ElementType = PaintElementType.SolidFill;

                        double offsetX = axisStep / 2;
                        for (int j = 0; j < poly.points.Length; j++)
                        {
                            DataPoint point = poly.points[j];
                            point.point = new Point((int)xMin + (int)offsetX, point.point.Y);

                            e.SceneGraph.Add(GetSymbolIcon(point, poly.Series.Label, point.DataPoint.Label));

                            Text chartText = new Text();
                            chartText.labelStyle.Font = new Font("Verdana", 8);
                            chartText.labelStyle.FontColor = point.PolylineParent.PE.Fill;
                            chartText.labelStyle.HorizontalAlign = StringAlignment.Center;
                            chartText.labelStyle.FontColor = GetSeriesColor(poly.Series.Label, point.DataPoint.Label);
                            chartText.bounds = new Rectangle(point.point.X - 20, point.point.Y - 25, 50, 20);
                            chartText.SetTextString(Convert.ToDouble(point.Value).ToString("N1"));
                            e.SceneGraph.Add(chartText);

                            offsetX += 2 * axisStep;

                            point.DataPoint.Label = GetSeriesHint(point, poly.Series.Label);
                        }
                    }
                    else if (poly.Path != null && poly.Path.ToLower().Contains("legend"))
                    {
                        poly.Visible = false;

                        Ellipse icon = null;
                        switch (legendItem)
                        {
                            case 0:
                                {
                                    icon = GetCircleIcon(poly, Color.Red, 8);
                                    break;
                                }
                            case 1:
                                {
                                    icon = GetCircleIcon(poly, Color.DarkGreen, 5);
                                    break;
                                }
                            case 2:
                                {
                                    icon = GetCircleIcon(poly, Color.Blue, 5);
                                    break;
                                }
                        }
                        if (icon != null)
                        {
                            e.SceneGraph.Add(icon);
                        }

                        legendItem++;
                    }
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;

                    int row = box.Row;
                    int column = box.Column + 1;

                    if (box.DataPoint != null)
                    {
                        double value = Double.MinValue;
                        if (dtStackColumnChart.Rows[row][column] != DBNull.Value && dtStackColumnChart.Rows[row][column].ToString() != String.Empty)
                        {
                            value = Convert.ToDouble(dtStackColumnChart.Rows[row][column]);
                        }

                        if (box.Column < 2)
                        {
                            double nextValue = Double.MinValue;
                            if (dtStackColumnChart.Rows[row][column + 1] != DBNull.Value && dtStackColumnChart.Rows[row][column + 1].ToString() != String.Empty)
                            {
                                nextValue = Convert.ToDouble(dtStackColumnChart.Rows[row][column + 1]);
                            }

                            if (value != Double.MinValue && nextValue != Double.MinValue)
                            {
                                value = value + nextValue;
                            }
                        }

                        if (value != Double.MinValue)
                        {
                            string monthName = CRHelper.RusMonth(box.Row / 2 + 1);
                            box.DataPoint.Label = String.Format("{0}\n{1}\n{2:N1} {3}", box.DataPoint.Label, GetDateString(monthName), value, rubMultiplierCaption);
                        }
                    }
                }
            }
        }

        private static Symbol GetSymbolIcon(DataPoint point, string serieName, string label)
        {
            Symbol symbol = new Symbol();
            symbol.PE.Fill = GetSeriesColor(serieName, label);
            symbol.icon = SymbolIcon.Circle;
            symbol.iconSize = SymbolIconSize.Medium;
            symbol.PE.ElementType = PaintElementType.SolidFill;
            symbol.point = point.point;
            return symbol;
        }

        private string GetDataValue(DataPoint point, string serieName)
        {
            string item = String.Empty;
            string value = String.Empty;
            switch (serieName)
            {
                case "Бюджет откорректированный":
                    {
                        item = rubMultiplierCaption;
                        value = String.Format("{0:N1}", point.Value);
                        break;
                    }
                case "Процент исполнения по доходам":
                    {
                        value = String.Format("{0:N1}%", point.Value);
                        break;
                    }
            }

            return String.Format("{0} {1}", value, item);
        }

        private static Color GetSeriesColor(string serieName, string label)
        {
            switch (serieName)
            {
                case "Бюджет откорректированный":
                    {
                        return Color.Red;
                    }
                case "Процент исполнения по доходам":
                    {
                        return (label == "Доходы") ? Color.DarkGreen : Color.Blue;
                    }
            }

            return Color.Transparent;
        }

        private string GetSeriesHint(DataPoint point, string serieName)
        {
            bool isIncomes = (point.Column % 2) == 0;

            string monthName = CRHelper.RusMonth(point.Row / 2 + 1);
            string dataValueStr = GetDataValue(point, serieName);
            point.DataPoint.Label = String.Format("{0}\n{1}\n{2}", serieName, GetDateString(monthName), dataValueStr);

            string category = String.Empty;
            switch (serieName)
            {
                case "Бюджет откорректированный":
                    {
                        category = isIncomes ? "Бюджет откорректированный по доходам" : "Бюджет откорректированный по расходам";
                        break;
                    }
                case "Процент исполнения по доходам":
                    {
                        category = isIncomes ? "Процент исполнения годовых назначений по доходам" : "Процент исполнения годовых назначений по расходам";
                        break;
                    }
            }

            return String.Format("{0}\n{1}\n{2}", category, GetDateString(month), dataValueStr);
        }

        private static Ellipse GetCircleIcon(Polyline polyline, Color color, int radius)
        {
            Point center = new Point(polyline.points[0].point.X + (polyline.points[2].point.X - polyline.points[0].point.X) / 2, polyline.points[0].point.Y);
            Ellipse circle = new Ellipse(center, radius);
            circle.PE.ElementType = PaintElementType.SolidFill;
            circle.PE.Fill = color;

            return circle;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            ReportExcelExporter1.Export(UltraChart, chartHeaderLabel.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, section1);

            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);           
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            //section2 = report.AddSection();
            title = section2.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.85));
            //ltraChart.Legend.Margins.Right = 1;
            UltraChart.Legend.Margins.Bottom = 5;
            ReportPDFExporter1.Export(UltraChart, chartHeaderLabel.Text, section2);

        }

        #endregion
    }
}