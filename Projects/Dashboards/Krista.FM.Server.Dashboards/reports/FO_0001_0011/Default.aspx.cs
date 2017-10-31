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
using Infragistics.UltraChart.Shared.Events;
using System.Collections;
namespace Krista.FM.Server.Dashboards.reports.FO_0001_0011
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
        private CustomParam incomes;
        private CustomParam incomes2;
        private CustomParam outcomes;
        private CustomParam level;
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

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
           
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }

            if (regionChart == null)
            {
                regionChart = UserParams.CustomParam("regionChart");
            }
            if (selectedRegion == null)
            {
                selectedRegion =  UserParams.CustomParam("selectedRegion");
            }
            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }
            if (regType == null)
            {
                regType = UserParams.CustomParam("regType");
            }
            if (incomes == null)
            {
                incomes = UserParams.CustomParam("incomes");
            }
            if (incomes2 == null)
            {
                incomes2 = UserParams.CustomParam("incomes2");
            }
            if (outcomes == null)
            {
                outcomes = UserParams.CustomParam("outcomes");
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
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.60);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 1.12);
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
            UltraGridExporter1.HeaderChildCellHeight = 100;
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.97));
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.8);
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.ColumnChart.SeriesSpacing = 1;
            UltraChart1.ColumnChart.ColumnSpacing = 1;

            UltraChart1.Axis.X.Extent = 140;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.StripLines.PE.Fill = System.Drawing.Color.Gainsboro;
            UltraChart1.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart1.Axis.X.StripLines.PE.Stroke = System.Drawing.Color.DarkGray;
            UltraChart1.Axis.X.StripLines.Interval = 2;
            UltraChart1.Axis.X.StripLines.Visible = true;
            UltraChart1.Axis.Y.Extent = 20;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);

            UltraChart1.Axis.X.Labels.WrapText = true;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            UltraChart1.Axis.X.Labels.Visible = true;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) / 4;
            UltraChart1.TitleLeft.Text = "Удельный вес";
            UltraChart1.TitleLeft.Font = new System.Drawing.Font("Verdana", 8);

            UltraChart1.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart1.ColorModel.ColorBegin = System.Drawing.Color.Red;
            UltraChart1.ColorModel.ColorEnd = System.Drawing.Color.Green;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 25);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 25);
            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N1>%";
            appearance.ChartTextFont = new System.Drawing.Font("Verdana", 6);
            appearance.Visible = true;
            UltraChart1.ColumnChart.ChartText.Add(appearance);

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N2>";

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
        
        }


        int Q = 0;
        double avgValue = 0;
        string units = string.Empty;
        string queryName = string.Empty;
        string queryName2 = string.Empty;
        string queryChartName = string.Empty;
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
                string queryName = "FO_0001_0011_date";
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

                ComboLevel.Title = "Бюджет";
                ComboLevel.Width = 230;
                ComboLevel.MultiSelect = false;
                FillLevelCombo();
                ComboLevel.FillDictionaryValues(LevelsDictionary);
                
                ComboLevel.RenameTreeNodeByName("Конс.бюджет субъекта", "Бюджет МР");
                ComboLevel.RenameTreeNodeByName("Конс.бюджет МО", "Свод бюджетов поселений");
                ComboLevel.RemoveTreeNodeByName("Бюджет субъекта");
                ComboMonth.SetСheckedState("Бюджет субъекта", true);
                ComboLevel.Visible = true;
      
                ComboIncomes.Title = "Вариант доходов";
                ComboIncomes.Width = 240;
                ComboIncomes.MultiSelect = false;
                FillIncomesCombo();
                ComboIncomes.FillDictionaryValues(IncomesDictionary);
            }
           Year.Value = ComboYear.SelectedValue;
           Page.Title = "Удельный вес дотации на выравнивание уровня бюджетной обеспеченности в объеме доходов местных бюджетов";
           int month = ComboMonth.SelectedIndex + 2;
           nMonth.Value = Convert.ToString(month);
           int year = Convert.ToInt32(ComboYear.SelectedValue);
           UserParams.PeriodYear.Value = ComboYear.SelectedValue;
           UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
           UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
           UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
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

           UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<DATA_VALUE:N0> %", ComboYear.SelectedValue, units);
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
           if (RList.SelectedIndex == 0)
           {
               queryName = "FO_0001_0011_compare_Grid1";
               queryChartName = "FO_0001_0011_compare_Chart1";
               lab = "фактические данные";
               ComboIncomes.Visible = false;
               ComboMonth.Visible = true;
               ComboYear.Visible = true;
           }
           else if (RList.SelectedIndex == 1)
           {
               queryName = "FO_0001_0011_compare_Grid2";
               queryChartName = "FO_0001_0011_compare_Chart2";
               lab = "плановые данные";
               ComboIncomes.Visible = false;
               ComboMonth.Visible = true;
               ComboYear.Visible = true;
           }
           else
           {
               queryName = "FO_0001_0011_compare_Grid3";
               queryChartName = "FO_0001_0011_compare_Chart3";
               lab = "проектные данные";
               ComboIncomes.Visible = true;
               incomes.Value = ComboIncomes.SelectedValue;
               UserParams.PeriodYear.Value = ComboIncomes.SelectedNodeParent;
               ComboMonth.Visible = false;
               ComboYear.Visible = false;
           }
           level.Value = ComboLevel.SelectedValue;
           if (level.Value == "Бюджет муниципального района")
           {
               level.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Бюджет района]";
           }
           else if (level.Value == "Бюджет поселений в составе МР")
           {
               level.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Бюджет поселения]";
           }
           else
           {
               level.Value = "    [Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Конс.бюджет МО]";
           }
           Label1.Text = "Удельный вес дотации на выравнивание уровня бюджетной обеспеченности в объеме доходов местных бюджетов";
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
           UltraChart1.DataBind();
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

                string query = DataProvider.GetQueryText(queryName);
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Местные бюджеты", dtGrid);
                UltraWebGrid.DataSource = dtGrid; 
                
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

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.bounds.Width = 30;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new System.Drawing.Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
            }
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            double lineStart = xAxis.MapMinimum;
            double lineLength = xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = System.Drawing.Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new System.Drawing.Point((int)lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new System.Drawing.Point((int)lineStart + (int)lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = System.Drawing.Color.Black;
            text.bounds = new System.Drawing.Rectangle((int)lineLength - textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средний удельный вес: {0:N2}", avgValue));
            e.SceneGraph.Add(text);


        }


        DataTable Chart = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(queryChartName);
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {

                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("Муниципальный район", "МР");
                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("муниципальный район", "МР");
                
            }
            double Sum = 0;
            if (dtChart1.Columns.Count > 0)
            {
                for (int k = 1; k < dtChart1.Columns.Count; k++)
                {
                    if (dtChart1.Rows[0][k] != DBNull.Value)
                    {
                        Sum += Convert.ToDouble(dtChart1.Rows[0][k]);
                    }
                }
            }
            avgValue = Convert.ToDouble(Sum / (dtChart1.Columns.Count - 1));
            

                    UltraChart1.DataSource = dtChart1;
                
        }

        private static void FillIncomesCombo()
        {
            if (IncomesDictionary.Count > 0)
                return;
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0001_0011_compare_Grid_Kind");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtIncomes);

            DataTable dtIncomesChildren = new DataTable();
            query = DataProvider.GetQueryText("FO_0001_0011_compare_Grid_Kind_a");
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
            string query = DataProvider.GetQueryText("FO_0001_0011_compare_Grid_Kinds");
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
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(215);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 1;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[k].Width = 118;
                    string formatString = "N2";
                    if ((k == 3)||(k == 6))
                    {
                        formatString = "P2";
                    }
                    e.Layout.Bands[0].Columns[k].Format = formatString;
                    e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.SpanY = 1;
                }

                if (RList.SelectedIndex == 0)
                {
                    e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("Общий объем дотаций на выравнивание уровня бюджетной обеспеченности (исполнено), {0}", units);
                    e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("Всего доходов (исполнено), {0}", units);
                    e.Layout.Bands[0].Columns[3].Header.Caption += ", %"; 
                    e.Layout.Bands[0].Columns[1].Header.Title = "Фактическое поступление дотаций на выравнивание уровня бюджетной обеспеченности";
                    e.Layout.Bands[0].Columns[2].Header.Title = "Фактическое поступление доходов нарастающим итогом с начала года";
                    e.Layout.Bands[0].Columns[3].Header.Title = "Доля дотаций на выравнивание уровня бюджетной обеспеченности в общей сумме доходов бюджета";
                   
                }else
                    if (RList.SelectedIndex == 1)
                    {
                        e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("Общий объем дотаций на выравнивание уровня бюджетной обеспеченности (бюджетные назначения), {0}", units);
                        e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("Всего доходов (бюджетные назначения), {0}", units);
                        e.Layout.Bands[0].Columns[3].Header.Caption += ", %"; 
                        e.Layout.Bands[0].Columns[1].Header.Title = "Плановые назначения дотаций на выравнивание уровня бюджетной обеспеченности";
                        e.Layout.Bands[0].Columns[2].Header.Title = "Плановые назначения поступления доходов нарастающим итогом с начала года";
                        e.Layout.Bands[0].Columns[3].Header.Title = "Доля дотаций на выравнивание уровня бюджетной обеспеченности в общей сумме доходов бюджета";
                   
                    }else
                        if (RList.SelectedIndex == 2)
                        {
                            e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("Общий объем дотаций на выравнивание уровня бюджетной обеспеченности (проект бюджета), {0}", units);
                            e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("Всего доходов (проект бюджета), {0}", units);
                            e.Layout.Bands[0].Columns[3].Header.Caption += ", %"; 
                            e.Layout.Bands[0].Columns[1].Header.Title = "Проект дотаций на выравнивание уровня бюджетной обеспеченности";
                            e.Layout.Bands[0].Columns[2].Header.Title = "Проект поступления доходов";
                            e.Layout.Bands[0].Columns[3].Header.Title = "Доля дотаций на выравнивание уровня бюджетной обеспеченности в общей сумме доходов бюджета";
                        }
            
            }
        }

        

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int min = 0;
        }

        #endregion

        #region Обработчики диаграмы
        

        #endregion
        
        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
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
            if ((e.CurrentColumnIndex > 0)&&(e.CurrentColumnIndex < 4))
            {
                e.HeaderText = "Доходы бюджета";
            }else
                if ((e.CurrentColumnIndex > 0)&&(e.CurrentColumnIndex > 3))
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
            for (int i = 3; i <  5; i++)
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
            Worksheet sheet1 = workbook.Worksheets.Add("Доходы бюджетов");
            Worksheet sheet2 = workbook.Worksheets.Add("Удельный вес");
            sheet2.Rows[0].Cells[0].Value = ChartCaption1.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[1].Cells[0], UltraChart1);
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            ReportSection section1 = new ReportSection(report, false);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            IText title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = e.Section.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(label);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
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
