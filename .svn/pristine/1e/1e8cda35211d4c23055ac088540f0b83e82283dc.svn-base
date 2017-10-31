using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Collections.ObjectModel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.FO_0003_0004
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid1 = new DataTable();
        private DataTable dtGrid2 = new DataTable();
        private int firstYear = 2010;
        private int endYear = 2011;
        private string descendantsLevel = string.Empty;
        private string incomesListKind = string.Empty;

        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;
        private int rubMultiplier;
        private MemberAttributesDigest budgetDigest;
        #endregion

        #region Свойства

        public bool UseStack
        {
            get { return useStack.Checked; }
        }

        public bool QuarterValuation
        {
            get { return quarterValuation.Checked; }
        }

        public bool IncomesFullList
        {
            get { return incomesListKind == "FullList"; }
        }

        private bool UseConsolidateRegionBudget
        {
            get { return useConsolidateRegionBudget.Checked; }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        #endregion

        #region Параметры запроса

        // мера Исполнено
        private CustomParam factMeasure;
        // мера Темп роста
        private CustomParam rateMeasure;
        // группа КД
        private CustomParam kdGroupName;
        // список выбранных годов для таблицы
        private CustomParam yearGridDescendants;
        // список выбранных годов для диаграммы
        private CustomParam yearChartDescendants;
        // выбранный район
        private CustomParam selectedRegion;
        // доходы Итого
        private CustomParam incomesTotal;

        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            factMeasure = UserParams.CustomParam("fact_measure");
            rateMeasure = UserParams.CustomParam("rate_measure");
            kdGroupName = UserParams.CustomParam("kd_group_name");
            yearGridDescendants = UserParams.CustomParam("year_grid_descendants");
            yearChartDescendants = UserParams.CustomParam("year_chart_descendants");
            selectedRegion = UserParams.CustomParam("selected_region");
            incomesTotal = UserParams.CustomParam("incomes_total");
            
            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0003_0004_Digest");
            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Axis.X.Extent = 50;
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Extent = IsThsRubSelected ? 80 : 50;
            
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y2.Extent = 50;
            
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            CRHelper.FillCustomColorModel(UltraChart, 10, false);
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 6;
            UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value / 2);

            UltraChart.TitleLeft.Text = RubMultiplierCaption;
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Extent = 30;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Visible = true;

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL> <SERIES_LABEL>г.\n<DATA_VALUE:N1> {0}", RubMiltiplierButtonList.SelectedValue);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

            EmptyAppearance emptyAppearance = new EmptyAppearance();
            emptyAppearance.EnablePoint = true;
            emptyAppearance.EnablePE = true;
            emptyAppearance.EnableLineStyle = true;
            emptyAppearance.PointStyle.Icon = SymbolIcon.Circle;
            emptyAppearance.PointStyle.IconSize = SymbolIconSize.Large;
            emptyAppearance.LineStyle.MidPointAnchors = true;
            UltraChart.SplineChart.EmptyStyles.Add(emptyAppearance);

            UltraChart.Data.ZeroAligned = true;
            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;
            
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 120);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            UltraWebGrid2.Visible = QuarterValuation;
            if (QuarterValuation)
            {
                UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid_QuarterDataBinding);
                UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";
                UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid_DataBound);
                UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
                UltraWebGrid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight*0.5 - 120);
                UltraWebGrid2.DataBinding += new EventHandler(UltraWebGrid2_QuarterDataBinding);
                UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            }
            else
            {
                UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid_MonthDataBinding);
            }

            int currentWidth = (int)Session["width_size"] - 20;
            UltraChart.Width = currentWidth - 20;

            int currentHeight = (int)Session["height_size"] - 192;
            UltraChart.Height = currentHeight / 2;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 != "null"
                    ? string.Format(",{2}.[{1}].[ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ].[{0}],",
                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                    RegionSettingsHelper.Instance.IncomesKDRootName,
                                    RegionSettingsHelper.Instance.IncomesKDAllLevel)
                    : ",";
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            firstYear = 2000;
            incomesListKind = RegionSettingsHelper.Instance.GetPropertyValue("IncomesListKind");
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;

            if (!Page.IsPostBack)
            {
                WebAsyncPanel.AddRefreshTarget(UltraWebGrid1);
                WebAsyncPanel.AddRefreshTarget(UltraWebGrid2);
                WebAsyncPanel.AddRefreshTarget(UltraChart);
                WebAsyncPanel.AddLinkedRequestTrigger(useStack.ClientID);
                WebAsyncPanel.AddLinkedRequestTrigger(quarterValuation.ClientID);
                WebAsyncPanel.AddLinkedRequestTrigger(useConsolidateRegionBudget.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0003_0004_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Width = 100;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2010, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.SetСheckedState((endYear - 1).ToString(), true);
                ComboYear.SetСheckedState((endYear - 2).ToString(), true);

                ComboKD.Width = 230;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(IncomesFullList
                    ? CustomMultiComboDataHelper.FillFullKDIncludingList()
                    : CustomMultiComboDataHelper.FillShortKDIncludingList());
                ComboKD.SetСheckedState("Доходы ВСЕГО ", true);
                ComboKD.SetСheckedState("Доходы бюджета - Итого ", true);
                ComboKD.RemoveTreeNodeByName("Налог на прибыль ");
                ComboKD.RemoveTreeNodeByName("Акцизы ");
                ComboKD.RemoveTreeNodeByName("НДПИ ");

                ComboRegion.Title = "Бюджет поселения";
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
            }

            string settlement = string.Empty;
            if (ComboRegion.SelectedNode.Level == 0)
            {
                settlement = string.Format("{0}, все поселения", ComboRegion.SelectedValue);
            }
            else
            {
                settlement = string.Format("{0}, {1}", ComboRegion.SelectedNodeParent, ComboRegion.SelectedValue);
            }
            Page.Title = String.Format("Анализ динамики доходов бюджета поселения");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Empty;

            bool regionSelected = false;
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(ComboRegion.SelectedValue))
            {
                regionSelected = RegionsNamingHelper.LocalBudgetTypes[ComboRegion.SelectedValue] == "МР";
            }
            useConsolidateRegionBudget.Visible = regionSelected;

            descendantsLevel = (QuarterValuation) ? "Квартал" : "Месяц";

            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string kd = ComboKD.SelectedValue;
                if (kd != "НДПИ " && kd != "НДФЛ ")
                {
                    kd = CRHelper.ToLowerFirstSymbol(kd);
                }

                string selectedBudget = UseConsolidateRegionBudget && regionSelected
                                            ? String.Format("{0} (Консолидированный бюджет МО)", ComboRegion.SelectedValue)
                                            : String.Format("{0}", ComboRegion.SelectedValue);

                PageSubTitle.Text = String.Format("{0}, {1} за {2} {3}, {4}",
                    settlement, kd, CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                    ComboYear.SelectedValues.Count == 1 ? "год" : "годы",
                    RubMiltiplierButtonList.SelectedValue); 

                string gridDescendants = String.Empty;
                string chartDescendants = String.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    // для таблицы всегда выбираем месяцы
                    if (!QuarterValuation)
                    {
                        gridDescendants += string.Format("Descendants ({1}.[Данные всех периодов].[{0}],{1}.[Месяц], SELF) + ",
                                          year, UserParams.PeriodDimension.Value);
                    }
                    else
                    {
                        gridDescendants += string.Format(@"Generate (Descendants({1}.[Данные всех периодов].[{0}], {1}.[Квартал], SELF),
                                Descendants 
                                (
                                    {1}.CurrentMember,
                                    {1}.[Месяц],
                                    SELF 
                                ) + {{{1}.CurrentMember}}
                            ) + ", year, UserParams.PeriodDimension.Value);
                    }
                    // для диаграммы в зависимости от чекбокса
                    chartDescendants += string.Format("Descendants ({1}.[Данные всех периодов].[{0}],{1}.[{2}], SELF) + ",
                        year, UserParams.PeriodDimension.Value, descendantsLevel);
                }
                gridDescendants = gridDescendants.Remove(gridDescendants.Length - 3, 2);
                yearGridDescendants.Value = string.Format("{1}{0}{2}", gridDescendants, '{', '}');

                chartDescendants = chartDescendants.Remove(chartDescendants.Length - 3, 2);
                yearChartDescendants.Value = string.Format("{1}{0}{2}", chartDescendants, '{', '}');
            }
            else
            {
                yearGridDescendants.Value = "{}";
                yearChartDescendants.Value = "{}";
            }

            selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
            
            kdGroupName.Value = ComboKD.SelectedValue;
            factMeasure.Value = (!UseStack) ? "Факт_за период" : "Факт";
            rateMeasure.Value = (!UseStack) ? "Темп роста к аналогичному периоду предыдущего года_За период" : "Темп роста к аналогичному периоду предыдущего года_Факт";
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            
            regionBudgetSKIFLevel.Value = UseConsolidateRegionBudget && regionSelected
                ? "Aggregate({[Уровни бюджета].[СКИФ].[Все].[Бюджет района], [Уровни бюджета].[СКИФ].[Все].[Бюджет поселения]})"
                : RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
      
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            UltraChart.DataBind();
        }

        #region Обработчики грида

        private static bool NullValueDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 2; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected void UltraWebGrid_MonthDataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0003_0004_grid");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid1);

            if (dtGrid1.Rows.Count > 0 && !NullValueDataTable(dtGrid1))
            {
                DataTable newDtGrid = new DataTable();
                DataColumn column = new DataColumn("Год", typeof(string));
                newDtGrid.Columns.Add(column);

                for (int i = 1; i < 13; i++)
                {
                    DataColumn monthCompleteColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Исполнено", typeof(decimal));
                    newDtGrid.Columns.Add(monthCompleteColumn);
                    DataColumn monthRateColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Темп роста, %", typeof(decimal));
                    newDtGrid.Columns.Add(monthRateColumn);
                }

                int year = 0;
                DataRow currRow = null;
                foreach (DataRow row in dtGrid1.Rows)
                {
                    int currYear = 0;
                    string period = string.Empty;
                    object completeValue = DBNull.Value;
                    object rateValue = DBNull.Value;
                    if (row[0] != DBNull.Value)
                    {
                        period = row[0].ToString();
                    }
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        currYear = Convert.ToInt32(row[1]);
                    }
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        completeValue = Convert.ToDouble(row[2]) / rubMultiplier;
                    }

                    if (row[3] != DBNull.Value && row[3].ToString() != string.Empty)
                    {
                        rateValue = Convert.ToDouble(row[3]) * 100;
                    }

                    // добавляем новый год
                    if (year != currYear)
                    {
                        year = currYear;
                        DataRow newRow = newDtGrid.NewRow();
                        newRow[0] = year;
                        newDtGrid.Rows.Add(newRow);

                        currRow = newRow;
                    }

                    if (currRow != null && newDtGrid.Columns.Contains(period + "; Исполнено"))
                    {
                        currRow[period + "; Исполнено"] = completeValue;
                    }

                    if (currRow != null && newDtGrid.Columns.Contains(period + "; Темп роста, %"))
                    {
                        currRow[period + "; Темп роста, %"] = rateValue;
                    }
                }

                UltraWebGrid1.DataSource = newDtGrid;
            }
        }

        protected void UltraWebGrid_QuarterDataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0003_0004_grid");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid1);

            if (dtGrid1.Rows.Count > 0 && !NullValueDataTable(dtGrid1))
            {
                DataTable newDtGrid = new DataTable();
                DataColumn column = new DataColumn("Год", typeof(string));
                newDtGrid.Columns.Add(column);

                dtGrid2 = new DataTable();
                column = new DataColumn("Год", typeof(string));
                dtGrid2.Columns.Add(column);

                DataTable currDataTable;
                
                for (int i = 1; i < 13; i++)
                {
                    if (i < 7)
                    {
                        currDataTable = newDtGrid;
                    }
                    else
                    {
                        currDataTable = dtGrid2;
                    }

                    DataColumn monthCompleteColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Исполнено", typeof(decimal));
                    currDataTable.Columns.Add(monthCompleteColumn);
                    DataColumn monthRateColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Темп роста, %", typeof(decimal));
                    currDataTable.Columns.Add(monthRateColumn);

                    if (i > 2 && i % 3 == 0)
                    {
                        monthCompleteColumn = new DataColumn(string.Format("Итого за {0} квартал; Исполнено", i / 3), typeof(decimal));
                        currDataTable.Columns.Add(monthCompleteColumn);
                        monthRateColumn = new DataColumn(string.Format("Итого за {0} квартал; Темп роста, %", i / 3), typeof(decimal));
                        currDataTable.Columns.Add(monthRateColumn);
                    }
                }

                int year = 0;
                DataRow currRow = null;
                foreach (DataRow row in dtGrid1.Rows)
                {
                    int currYear = 0;
                    string period = string.Empty;
                    object completeValue = DBNull.Value;
                    object rateValue = DBNull.Value;
                    if (row[0] != DBNull.Value)
                    {
                        period = row[0].ToString();
                    }

                    int gridNumber = GetGridNumber(period);
                    if (gridNumber == 1)
                    {
                        currDataTable = dtGrid2;
                    }
                    else
                    {
                        currDataTable = newDtGrid;
                    }

                    period = GetGridQuarterStr(period);

                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        currYear = Convert.ToInt32(row[1]);
                    }
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        completeValue = Convert.ToDouble(row[2]) / rubMultiplier;
                    }

                    if (row[3] != DBNull.Value && row[3].ToString() != string.Empty)
                    {
                        rateValue = Convert.ToDouble(row[3]) * 100;
                    }

                    // добавляем новый год
                    if (year != currYear || !RowContains(currDataTable, currYear.ToString()))
                    {
                        year = currYear;
                        DataRow newRow = currDataTable.NewRow();
                        newRow[0] = year;
                        currDataTable.Rows.Add(newRow);

                        currRow = newRow;
                    }

                    if (currRow != null && currDataTable.Columns.Contains(period + "; Исполнено"))
                    {
                        currRow[period + "; Исполнено"] = completeValue;
                    }

                    if (currRow != null && currDataTable.Columns.Contains(period + "; Темп роста, %"))
                    {
                        currRow[period + "; Темп роста, %"] = rateValue;
                    }
                }

                UltraWebGrid1.DataSource = newDtGrid;
            }
        }

        private static bool RowContains(DataTable dt, string rowName)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() == rowName)
                {
                    return true;
                }
            }
            return false;
        }

        private static int GetGridNumber(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return quarterNumber > 2 ? 1 : 0;
            }
            else
            {
                return CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(period)) > 2 ? 1 : 0;
            }
        }

        private static string GetGridQuarterStr(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return string.Format("Итого за {0} квартал", quarterNumber);
            }
            else
            {
                return period;
            }
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

        protected void UltraWebGrid2_QuarterDataBinding(object sender, EventArgs e)
        {
            if (dtGrid2.Rows.Count > 0)
            {
                UltraWebGrid2.DataSource = dtGrid2;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            bool isFirstGrid = e.Layout.Grid == UltraWebGrid1;
            GridHeaderLayout headerLayout = isFirstGrid ? headerLayout1 : headerLayout2;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N1";
                double columnWidth;
                if (IsThsRubSelected && !QuarterValuation)
                {
                    columnWidth = HttpContext.Current.Request.Browser.Browser == "IE" ? 63 : 65; 
                }
                else
                {
                    columnWidth = HttpContext.Current.Request.Browser.Browser == "IE" ? 45 : 47;
                }

                if (QuarterValuation)
                {
                    columnWidth = 1.5 * columnWidth;
                }

                if (i % 2 == 0)
                {
                    formatString = "N0";
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            int zeroColumnWidth = HttpContext.Current.Request.Browser.Browser == "IE" ? 26 : 28;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(zeroColumnWidth);
            
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            if (QuarterValuation)
            {
                GridHeaderCell topHeader = new GridHeaderCell();
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
                {
                    if (i == 1)
                    {
                        string headerCaption = isFirstGrid ? string.Format("Квартал 1") : string.Format("Квартал 3");
                        topHeader = headerLayout.AddCell(headerCaption);
                    }
                    else if ((i - 1) % 8 == 0)
                    {
                        string headerCaption = isFirstGrid ? string.Format("Квартал 2") : string.Format("Квартал 4");
                        topHeader = headerLayout.AddCell(headerCaption);
                    }
                    
                    string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                    string caption = captions[0].TrimEnd('_');

                    GridHeaderCell headerCell = topHeader.AddCell(caption);
                    headerCell.AddCell("Факт", String.Format("Фактические поступления {0}, {1}", (useStack.Checked) ? "с начала года" : "за месяц", RubMiltiplierButtonList.SelectedValue));
                    headerCell.AddCell("Темп роста, %", "Темп роста к прошлому году");
                }
            }
            else
            {
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
                {
                    string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                    string caption = captions[0].TrimEnd('_');

                    GridHeaderCell headerCell = headerLayout.AddCell(caption);
                    headerCell.AddCell("Факт", String.Format("Фактические поступления {0}, {1}", (useStack.Checked) ? "с начала года" : "за месяц", RubMiltiplierButtonList.SelectedValue));
                    headerCell.AddCell("Темп роста, %", "Темп роста к прошлому году",2);
                }
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i % 2 == 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 0;
                }
                else if (i % 2 != 0 && i != e.Row.Cells.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 0;
                }

                e.Row.Cells[i].Style.Padding.Left = 3;
                e.Row.Cells[i].Style.Padding.Right = 3;
                if ((i % 2 == 0 && i != 0) && e.Row.Cells[i].Value != null)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к аналогичному периоду прошлого года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к аналогичному периоду прошлого года";
                    }
                    if (e.Row.Cells[i].Column.Width.Value < 60)
                    {
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center;";
                    }
                    else
                    {
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; padding-left: 10px; background-position: 10px center;";
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
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

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid2.Height = Unit.Empty;

            if (!IsThsRubSelected || QuarterValuation)
            {
                UltraWebGrid1.Width = Unit.Empty;
                UltraWebGrid2.Width = Unit.Empty;
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0003_0004_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                DataTable newDtChart = new DataTable();
                DataColumn column = new DataColumn("Год", typeof(string));
                newDtChart.Columns.Add(column);

                if (QuarterValuation)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        DataColumn quarterColumn = new DataColumn(string.Format("{0} квартал", i), typeof(double));
                        newDtChart.Columns.Add(quarterColumn);
                    }
                }
                else
                {
                    for (int i = 1; i < 13; i++)
                    {
                        DataColumn monthColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), typeof(double));
                        newDtChart.Columns.Add(monthColumn);
                    }
                }

                int year = 0;
                DataRow currRow = null;
                foreach (DataRow row in dtChart.Rows)
                {
                    int currYear = 0;
                    string period = string.Empty;
                    double measureValue = double.MinValue;
                    if (row[0] != DBNull.Value)
                    {
                        period = row[0].ToString();
                        period = GetChartQuarterStr(period);
                    }
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        currYear = Convert.ToInt32(row[1]);
                    }
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        measureValue = Convert.ToDouble(row[2]) / rubMultiplier;
                    }

                    // добавляем новый год
                    if (year != currYear)
                    {
                        year = currYear;
                        DataRow newRow = newDtChart.NewRow();
                        newRow[0] = year;
                        newDtChart.Rows.Add(newRow);

                        currRow = newRow;
                    }

                    if (currRow != null && newDtChart.Columns.Contains(period) && measureValue != double.MinValue)
                    {
                        currRow[period] = measureValue;
                    }
                }

                UltraChart.DataSource = newDtChart;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.HeaderCellHeight = 20;

            Workbook workbook = new Workbook();
            
            if (QuarterValuation)
            {
                Worksheet sheet1 = workbook.Worksheets.Add("1 и 2 кварталы");
                Worksheet sheet2 = workbook.Worksheets.Add("3 и 4 кварталы");

                SetExportGridParams(headerLayout1.Grid, 2);
                SetExportGridParams(headerLayout2.Grid, 2);

                ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
                ReportExcelExporter1.Export(headerLayout2, sheet2, 3);
            }
            else
            {
                Worksheet sheet = workbook.Worksheets.Add("Таблица");

                SetExportGridParams(headerLayout1.Grid, 2);

                ReportExcelExporter1.Export(headerLayout1, sheet, 3);
            }

            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма");
            ReportExcelExporter1.Export(UltraChart, sheet3, 3);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private static void SetExportGridParams(UltraWebGrid grid, double widthMultiplier)
        {
            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * widthMultiplier);
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;

            ReportPDFExporter1.Export(UltraChart);

            SetExportGridParams(headerLayout1.Grid, 1.3);
            ReportPDFExporter1.Export(headerLayout1);

            if (QuarterValuation)
            {
                SetExportGridParams(headerLayout2.Grid, 1.3);
                ReportPDFExporter1.Export(headerLayout2);
            }
        }

        #endregion
    }
}