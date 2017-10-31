using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0011
{
    public enum SliceType
    {
        OKVED,
        OKOPF,
        OKFS
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest kindDigest;
        private static MemberAttributesDigest measureDigest;
        private int firstYear = 2000;
        private CustomParam currentPeriod;
        private CustomParam lastPeriod;
        private CustomParam Finance;
        private CustomParam kind;
        private GridHeaderLayout headerLayout;
        private UltraGridRow gridrow;
        #endregion

        #region Параметры запроса

        // множество для среза данных
        private CustomParam sliceSet;
        private CustomParam period;
        private CustomParam lastquart;
        private CustomParam datasources;
        private CustomParam rows;
        private CustomParam prds;
        private CustomParam Kind;
        private CustomParam measure;
        private string queryName = string.Empty;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            #endregion

            #region Настройка диаграммы динамики

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.95));
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75);
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 30;
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart1.Axis.X.Labels.ItemFormatString = " ";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 12;
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 14);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 14);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N1>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart1.ColumnChart.ChartText.Add(appearance);

            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.Data.ZeroAligned = true;


            #endregion

            #region Инициализация параметров запроса

            currentPeriod = UserParams.CustomParam("current_period");
            lastPeriod = UserParams.CustomParam("last_period");
            Finance = UserParams.CustomParam("finance");
            kind = UserParams.CustomParam("kind");
            period = UserParams.CustomParam("period");
            lastquart = UserParams.CustomParam("lastquart");
            datasources = UserParams.CustomParam("data_sources");
            rows = UserParams.CustomParam("rows");
            kind = UserParams.CustomParam("kind");
            measure = UserParams.CustomParam("measure");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0011_lastDate");
                ComboYear.PanelHeaderTitle = "Выберите период";
                ComboYear.Title = "Выберите период";
                ComboYear.Width = 290;
                ComboYear.ParentSelect = true;
                ComboYear.ShowSelectedValue = true;
                ComboYear.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboYear.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0011_Date");
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboYear.SelectLastNode();

                ComboKind.PanelHeaderTitle = "Сфера";
                ComboKind.Title = "Сфера";
                ComboKind.Width = 260;
                ComboKind.ParentSelect = false;
                ComboKind.ShowSelectedValue = true;
                ComboKind.MultiSelect = false;
                kindDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0011_Kinds");
                ComboKind.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(kindDigest.UniqueNames, kindDigest.MemberLevels));
                ComboKind.SetСheckedState("Энергетика", true);
                ComboKind.RemoveTreeNodeByName("Транспорт");

                ComboMeasure.PanelHeaderTitle = "Показатель";
                ComboMeasure.Title = "Показатель";
                ComboMeasure.Width = 590;
                ComboMeasure.ParentSelect = true;
                ComboMeasure.ShowSelectedValue = true;
                ComboMeasure.MultiSelect = false;
                measureDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0011_Measures");
                ComboMeasure.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(measureDigest.UniqueNames, measureDigest.MemberLevels));
                ComboMeasure.SelectLastNode();
            }
            string periodUniqueName = string.Empty;
            int yearNum = firstYear;
            string ps = string.Empty;
            if (ComboYear.SelectedValue.Contains("квартал"))
            {
                ps = "";
            }
            else
            {
                ps = ".[Данные года]";
            }
            period.Value = string.Format("{0}{1}", StringToMDXDate(ComboYear.SelectedValue), ps);
            lastquart.Value = periodUniqueName;
            UserParams.PeriodYear.Value = yearNum.ToString();
            Page.Title = String.Format("Распределение предприятий по выбранной сфере деятельности");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Распределение предприятий по сфере деятельности \"{0}\" за {1}", ComboKind.SelectedValue, ComboYear.SelectedValue);

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            kind.Value = ComboKind.SelectedNode.Text;
            measure.Value = measureDigest.UniqueNames[ComboMeasure.SelectedNode.Text];
            UltraChart1.DataBind();
            string query = DataProvider.GetQueryText("STAT_0002_0011_item");
            DataTable dtItem = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtItem);
            string item = string.Empty;
            if (dtItem.Rows.Count > 0)
            {
                item = dtItem.Rows[0][1].ToString();
            }
            UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n за {0} \n<DATA_VALUE:N1>, {1}", ComboYear.SelectedValue, item.ToLower());
            ChartCaption1.Text = String.Format("Показатель \"{0}\", {1}", ComboMeasure.SelectedValue, item.ToLower());
            string patternValue = string.Empty;
            int defaultRowIndex = 0;
            patternValue = Finance.Value;

        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0011_chart");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            if (dtChart1.Rows.Count > 0)
            {
                foreach (DataColumn column in dtChart1.Columns)
                {

                    column.ColumnName = column.ColumnName.ToString().Replace("\"", "'");

                }
                if (dtChart1.Rows.Count > 4)
                {
                    UltraChart1.Legend.SpanPercentage = 32;
                }
                UltraChart1.DataSource = dtChart1;
            }

        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
            }
            combo.FillDictionaryValues(dictDate);
            combo.SelectLastNode();
        }

        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        public string StringToMDXDate(string str)
        {
            if (str.Contains("квартал"))
            {
                string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}]";
                string[] dateElements = str.Split(' ');
                int year = Convert.ToInt32(dateElements[2]);
                int quarter = Convert.ToInt32(dateElements[0]);
                int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
                return String.Format(template, year, halfYear, quarter);
            }
            else
            {
                return String.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}]", str.Split(' ')[0].ToString());
            }
        }

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Диаграмма1");
            ReportExcelExporter1.Export(UltraChart1, ChartCaption1.Text, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF



        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.HeaderCellHeight = 50;
            Report report = new Report();
            ISection section1 = report.AddSection();
            UltraChart1.Width = Convert.ToInt32(UltraChart1.Width.Value * 0.8);
            ReportPDFExporter1.HeaderCellHeight = 60;
            Infragistics.Documents.Reports.Report.Text.IText header1 = section1.AddText();
            header1.Style.Font.Name = "Verdana";
            header1.Style.Font.Size = 15;
            header1.Style.Font.Bold = true;
            header1.AddContent(Label1.Text);

            Infragistics.Documents.Reports.Report.Text.IText header2 = section1.AddText();
            header2.Style.Font.Name = "Verdana";
            header2.Style.Font.Size = 13;
            header2.AddContent(Label2.Text);
            ReportPDFExporter1.Export(UltraChart1, ChartCaption1.Text, section1);

        }

        #endregion
    }
}