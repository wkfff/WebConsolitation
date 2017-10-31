using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0011_01
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private int firstYear = 2008;
        private int endYear = 2011;
        private double rubMultiplier;
		private GridHeaderLayout headerLayout;
        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "тыс.руб." : "млн.руб."; }
        }

        private bool UseConsolidateRegionBudget
        {
            get { return useConsolidateRegionBudget.Checked; }
        }

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;
        // Тип документа
        private CustomParam documentType;
        // Консолидированный уровень
        private CustomParam consolidateLevel;
        // Уровень бюджета СКИФ
        private CustomParam budgetSKIFLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.47 - 120);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2 - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 110);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2 - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 110);

            CrossLink1.Visible = false;
            CrossLink1.Text = "Динамика&nbsp;фактического&nbsp;поступления&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_00011_02/Default.aspx";
            CrossLink2.Text = "Исполнение&nbsp;доходов";
            CrossLink2.NavigateUrl = "~/reports/FO_0002_0005/DefaultCompare.aspx";
            if (RegionSettingsHelper.Instance.Name == "Ярославская область")
            {
                CrossLink3.Visible = false;
                CrossLink3.Text = "Темп&nbsp;роста&nbsp;доходов";
                CrossLink3.NavigateUrl = "~/reports/FO_0002_0003/DefaultCompare.aspx";
            }
            else
            {
                CrossLink3.Visible = true;
                CrossLink3.Text = "Темп&nbsp;роста&nbsp;доходов";
                CrossLink3.NavigateUrl = "~/reports/FO_0002_0003/DefaultCompare.aspx";
            }

            #region Инициализация параметров запроса

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

            #endregion

            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.DoughnutChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart1.DoughnutChart.ShowConcentricLegend = false;
            UltraChart1.DoughnutChart.Concentric = true;

            UltraChart1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Left;
            UltraChart1.Legend.SpanPercentage = 40;
            UltraChart1.Legend.Margins.Top = 0;

            CRHelper.FillCustomColorModel(UltraChart1, 17, false);
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;

            CalloutAnnotation planAnnotation = new CalloutAnnotation();
            planAnnotation.Text = "План";
            planAnnotation.Width = 40;
            planAnnotation.Height = 20;
            planAnnotation.Location.Type = LocationType.Percentage;
            planAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;
            planAnnotation.Location.LocationX = UltraChart1.Legend.SpanPercentage + (100 - UltraChart1.Legend.SpanPercentage) / 2;
            planAnnotation.Location.LocationY = 71;

            CalloutAnnotation factAnnotation = new CalloutAnnotation();
            factAnnotation.Text = "Факт";
            factAnnotation.Width = 40;
            factAnnotation.Height = 20;
            factAnnotation.Location.Type = LocationType.Percentage;
            factAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;
            factAnnotation.Location.LocationX = UltraChart1.Legend.SpanPercentage + (100 - UltraChart1.Legend.SpanPercentage) / 2;
            factAnnotation.Location.LocationY = 14;

            UltraChart1.Annotations.Add(planAnnotation);
            UltraChart1.Annotations.Add(factAnnotation);
            UltraChart1.Annotations.Visible = true;

            UltraChart2.ChartType = ChartType.SplineChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Left;
            UltraChart2.Legend.SpanPercentage = 15;
            UltraChart2.Legend.Margins.Bottom = Convert.ToInt32(3 * UltraChart2.Height.Value / 4);
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart2.Axis.Y.Extent = 30;
            UltraChart2.Axis.X.Extent = 100;
            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.Data.ZeroAligned = true;

            UltraChart2.Axis.Y2.Visible = true;
            UltraChart2.Axis.Y2.LineColor = Color.Transparent;
            UltraChart2.Axis.Y2.Labels.Visible = false;
            UltraChart2.Axis.Y2.Extent = 10;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart2.SplineChart.LineAppearances.Add(lineAppearance);

            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0011_01_date");
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

                ComboBudgetLevel.Title = "Уровень бюджета";
                ComboBudgetLevel.Width = 400;
                ComboBudgetLevel.MultiSelect = false;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillLocalBudgets(RegionsNamingHelper.LocalBudgetTypes));
                ComboBudgetLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");

            bool regionSelected = false;
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(ComboBudgetLevel.SelectedValue))
            {
                regionSelected = RegionsNamingHelper.LocalBudgetTypes[ComboBudgetLevel.SelectedValue] == "МР";
            }
            useConsolidateRegionBudget.Visible = regionSelected;

            switch (ComboBudgetLevel.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Конс.бюджет субъекта]";
                        break;
                    }
                case "Местные бюджеты":
                    {
                        budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Конс.бюджет МО]";
                        break;
                    }
                default:
                    {
                        if (ComboBudgetLevel.SelectedValue == RegionSettingsHelper.Instance.OwnSubjectBudgetName)
                        {
                            budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Бюджет субъекта]";
                        }
                        else
                        {
                            selectedRegion.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboBudgetLevel.SelectedValue];
                            string regionType = RegionsNamingHelper.LocalBudgetTypes[ComboBudgetLevel.SelectedValue];
                            if (regionType.Contains("МР"))
                            {
                                budgetSKIFLevel.Value = UseConsolidateRegionBudget && regionSelected ?
                                    "[Уровни бюджета].[СКИФ].[Конс.бюдежт района ]" :
                                    RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetLevel");
                                documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
                            }
                            else if (regionType.Contains("ГО"))
                            {
                                budgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("DistrictBudgetLevel");
                                documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("DistrictDocumentSKIFType");
                            }
                        }
                        break;
                    }
            }

            Page.Title = string.Format("Информация об исполнении налоговых и неналоговых доходов: {0}", ComboBudgetLevel.SelectedValue);
            PageTitle.Text = Page.Title;
            chartHeaderLabel.Text = "Исполнение по основным видам доходных источников";

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            PageSubTitle.Text = string.Format("Оценка исполнения и структуры за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> <DATA_VALUE:N2> {0}\nдоля <PERCENT_VALUE:N2>%", RubMultiplierCaption);

            UltraWebGrid.Bands.Clear();
			headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_01_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i == 1 || i == 2 || i == 4 || i == 5 || i == 7)
                            && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString = "P2";
                int widthColumn = 80;

                if (i != e.Layout.Bands[0].Columns.Count - 2)
                {
                    // номер в тройке
                    int j = (i - 1) % 3;
                    // номер тройки
                    int k = (i - 1) / 3;
                    switch (j)
                    {
                        case 0:
                            {
                                formatString = "N2" ;
                                if (k == 3)
                                {
                                    widthColumn = 50;
                                }
                                else if (k == 2)
                                {
                                    widthColumn = 95;
                                }
                                else
                                {
                                    widthColumn = 125;
                                }
                                
                                break;
                            }
                        case 1:
                            {
                                formatString = k == 2 ? "P2" : "N2";
                                widthColumn = k == 2 ? 70 : 105;
                                break;
                            }
                        case 2:
                            {
                                formatString = "P2";
                                widthColumn = 70;
                                break;
                            }
                    }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 3;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(185);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

          /*  for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == e.Layout.Bands[0].Columns.Count - 2)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }*/
			headerLayout.AddCell("Наименование показателей");
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 3)
            {

                int j = (i - 1) / 3;

                if (j == 0 || j == 1 || j == 2)
                {
                    string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                    GridHeaderCell cell = headerLayout.AddCell(captions[0]);

                    int monthNum = ComboMonth.SelectedIndex + 1;
                    int yearNum = j == 0 ? Convert.ToInt32(ComboYear.SelectedValue) - 1 : Convert.ToInt32(ComboYear.SelectedValue);

                    if (j == 2)
                    {
						cell.AddCell("Отклонение<br/>(+,-)", string.Format("Отклонение исполнения {0} года от {1} года", yearNum, yearNum - 1));
						cell.AddCell("Темп роста", string.Format("Темп роста факта к аналогичному периоду {0} года", yearNum - 1));
						cell.AddCell("Рост (снижение)", string.Format("Рост (снижение) факта доходов к {0} году", yearNum - 1));
                    }
                    else
                    {
						cell.AddCell(String.Format("Уточненные годовые назначения, {0}", RubMultiplierCaption), 
                            String.Format("Уточненные годовые назначения на {0} год", yearNum));
						cell.AddCell(String.Format("Факт, {0}", RubMultiplierCaption), 
                            String.Format("Исполнено за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum));	
						cell.AddCell("% исполнен.", string.Format("% исполнения годовых назначений расходов за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum));
                    }
                }
                else
                {
				headerLayout.AddCell("Доля", string.Format("Удельный вес доходного источника в общем объеме собственных доходов"));
                }    
			}				
			headerLayout.ApplyHeaderInfo();	
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool increase = (i == 9);
                bool complete = (i == 6);
                int levelIndex = 11;

                if ((increase) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if ( Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Наблюдается рост доходов";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Наблюдается снижение доходов";
                        }
                    }
                }

//                if (complete)
//                {
//                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
//                    {
//                        double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
//
//                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
//                        {
//                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
//                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
//                        }
//                        else
//                        {
//                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
//                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
//                        }
//                        e.Row.Cells[i].Style.Padding.Right = 2;
//                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
//                    }
//                }

                if (e.Row.Cells[levelIndex] != null && e.Row.Cells[levelIndex].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[levelIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;

                    //string groupLevelName = RegionSettingsHelper.Instance.IncomesKDGroupLevel;

                    switch (level)
                    {
                        case "Группа":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Подгруппа":
                            {
                                fontSize = 9;
                                bold = false;
                                italic = true;
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
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
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

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_01_chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            foreach (DataRow row in dtChart1.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = DataDictionariesHelper.GetShortKDName(row[0].ToString().TrimEnd(' '));
                }

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1 || i == 2)
                         && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                    }
                }
            }

            UltraChart1.DataSource = dtChart1;
        }

        void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Wedge)
                {
                    Wedge wedge = (Wedge)primitive;
                    if (wedge.DataPoint != null)
                    {
                        string shortName = wedge.DataPoint.Label;
                        string fullName = DataDictionariesHelper.GetFullKDName(shortName);
                        string name = shortName == fullName ? fullName : string.Format("{0} ({1}) ", fullName, shortName);
                        wedge.DataPoint.Label = string.Format("{0}\n{1}", name, wedge.Column == 1 ? "план" : "факт");
                    }
                }
            }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0011_01_chart2");
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            if (dtChart2.Rows.Count > 0)
            {
                foreach (DataRow row in dtChart2.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = DataDictionariesHelper.GetShortKDName(row[0].ToString().TrimEnd(' '));
                    }
                }

                UltraChart2.Series.Clear();
                for (int i = 1; i < dtChart2.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
                    series.Label = dtChart2.Columns[i].ColumnName;
                    UltraChart2.Series.Add(series);
                }
            }
        }

        void UltraChart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 45;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                }
                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.DataPoint != null)
                        {
                            string shortName = point.DataPoint.Label;
                            string fullName = DataDictionariesHelper.GetFullKDName(shortName);
                            string name = shortName == fullName ? fullName : string.Format("{0} ({1}) ", fullName, shortName);

                            int monthNum = ComboMonth.SelectedIndex + 1;
                            string year = string.Empty;
                            if (point.Series != null && point.Series.Label.ToString() != string.Empty)
                            {
                                year = point.Series.Label.ToString().Split(' ')[0];
                            }

                            if (!point.DataPoint.Label.Contains("% исполнения"))
                            {
                                point.DataPoint.Label = string.Format("{0}\n % исполнения за {1} {2} {3} года: ", name, monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);
                            }
                        }
                    }
                }
            }
        }

        #endregion
/*
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            //e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;

 
            
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int width = 140;
            int columnCount = UltraWebGrid.Columns.Count;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                if (e.CurrentWorksheet.Rows[4].Cells[i].Value != null)
                {
                    e.CurrentWorksheet.Rows[4].Cells[i].Value = e.CurrentWorksheet.Rows[4].Cells[i].Value.ToString().Replace("<br/>", " ");
                }
            }

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

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            sheet1.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet2.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet2.Rows[1].Cells[0].Value = chartHeaderLabel.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[3].Cells[0], UltraChart1);
            UltraGridExporter.ChartExcelExport(sheet2.Rows[3].Cells[12], UltraChart2);

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex];
            e.HeaderText = col.Header.Key.Split(';')[0];
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
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(125);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(125);
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            UltraGridExporter1.GridElementCaption = PageSubTitle.Text;
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartHeaderLabel.Text);

            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();
            Infragistics.Documents.Reports.Graphics.Image img1 = UltraGridExporter.GetImageFromChart(UltraChart1);
            cell.AddImage(img1);

            cell = row.AddCell();

            Infragistics.Documents.Reports.Graphics.Image img2 = UltraGridExporter.GetImageFromChart(UltraChart2);
            cell.AddImage(img2);
            
                        Infragistics.Documents.Reports.Graphics.Image img1 = UltraGridExporter.GetImageFromChart(UltraChart1);
                        e.Section.AddImage(img1);
                        Infragistics.Documents.Reports.Graphics.Image img2 = UltraGridExporter.GetImageFromChart(UltraChart2);
                        e.Section.AddImage(img2);
        }

        #endregion
    */
	        private Bitmap GetImageChart()
    {
            int picWidth = (int)(UltraChart1.Width.Value + UltraChart2.Width.Value);
            int picHeight = (int)(UltraChart1.Height.Value);
            Bitmap bmNewImg = new Bitmap(picWidth, picHeight);//ваша нвоая картинка

            MemoryStream imageStream = new MemoryStream();
            UltraChart1.SaveTo(imageStream, ImageFormat.Png);
            Bitmap bm1 = new Bitmap(imageStream);//ваша маленькая картинка

            imageStream = new MemoryStream();
            UltraChart2.SaveTo(imageStream, ImageFormat.Png);
            Bitmap bm2 = new Bitmap(imageStream);

            Graphics g = Graphics.FromImage(bmNewImg);
            g.DrawImage(bm1, 0, 0, (int)UltraChart1.Width.Value, (int)UltraChart1.Height.Value);//туту нужно изменить под ваши задачи размещения картинок, координаты, области и тд
            g.DrawImage(bm2, (int)UltraChart2.Width.Value, 0, (int)UltraChart2.Width.Value,
                        (int)UltraChart2.Height.Value);
            g.Dispose();
            return bmNewImg;           
    }
	    #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            
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
