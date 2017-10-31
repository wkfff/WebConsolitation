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
using System.Collections;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0013
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
        private CustomParam outcomes;
        private CustomParam predyear;
        private CustomParam groupyear;
        #region ��������� �������

        // ������� �������
        private CustomParam budgetLevel;
        // ������ �������
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
                selectedRegion = UserParams.CustomParam("selectedRegion");
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
            if (outcomes == null)
            {
                outcomes = UserParams.CustomParam("outcomes");
            }
            if (groupyear == null)
            {
                groupyear = UserParams.CustomParam("groupyear");
            }

            if (dec == null)
            {
                dec = UserParams.CustomParam("dec");
            }
            if (predyear == null)
            {
                predyear = UserParams.CustomParam("period_pred_year");
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
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 70);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.28);
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
            CRHelper.SaveToUserAgentLog(String.Format("��������� ������� {0}", Environment.TickCount - start));
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.HeaderChildCellHeight = 100;
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.906));
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.65);
            UltraChart1.ChartType = ChartType.StackBarChart;
            UltraChart1.ColumnChart.NullHandling = NullHandling.Zero;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL> ", units);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 0;
            UltraChart1.Axis.Y.Extent = 290;
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.ItemFormatString = " ";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 12;
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 65);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 12);
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.Axis.X.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Center;
            UltraChart1.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Near;
            appearance.ItemFormatString = "<DATA_VALUE_ITEM:N2>%";
            appearance.ChartTextFont = new System.Drawing.Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart1.BarChart.ChartText.Add(appearance);
            UltraChart1.ColumnChart.ColumnSpacing = 1;
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {

        }


        int Q = 0;
        string units = string.Empty;
        string chartlab = string.Empty;
        string queryName = string.Empty;
        string queryChartName = string.Empty;
        private static Dictionary<string, int> IncomesDictionary = new Dictionary<string, int>();
        private static Dictionary<string, int> OutcomesDictionary = new Dictionary<string, int>();
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("������� ���� {0}", Environment.TickCount - start));

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
                string queryName = "FO_0001_0013_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string endmonth = dtDate.Rows[0][3].ToString();

                int firstYear = 2006;
                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(Convert.ToString(Convert.ToInt32(endYear.ToString())), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(endmonth, true);


                ComboOutcomes.Title = "������� ��������";
                ComboOutcomes.Width = 210;
                ComboOutcomes.MultiSelect = false;
                FillOutcomesCombo();
                ComboOutcomes.FillDictionaryValues(OutcomesDictionary);


            }

            Year.Value = ComboYear.SelectedValue;
            Page.Title = "������������� ��������� �������� ����� ��������� � �������� ���������";
            int month = ComboMonth.SelectedIndex + 2;
            nMonth.Value = Convert.ToString(month);
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            predyear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1);
            selectedPeriod.Value = String.Empty;
            if (RadioList.SelectedIndex == 0)
            {
                mul.Value = Convert.ToString(1000);
                units = "���.���.";
            }
            else
            {
                mul.Value = Convert.ToString(1000000);
                units = "���.���.";
            }
            //UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n <ITEM_LABEL>\n<DATA_LABEL>", units);
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            string titlemonth = string.Empty;
            if (ComboMonth.SelectedValue != "������")
            {
                titlemonth = String.Format("�� ������ - {0}", ComboMonth.SelectedValue.ToLower());
            }
            else
            {
                titlemonth = "�� ������";
            }
            Label2.Text = "";
            string lab = "";
            string labe = "";
            if (RList.SelectedIndex == 0)
            {
                if (Convert.ToInt32(ComboYear.SelectedValue) < 2011)
                {
                    queryName = "FO_0001_0013_compare_Grid";
                    queryChartName = "FO_0001_0013_compare_Chart1";
                }
                else
                {
                    queryName = "FO_0001_0013_compare_Grid_2011";
                    queryChartName = "FO_0001_0013_compare_Chart1_2011";
                }
                lab = "����������� ������";
                ComboOutcomes.Visible = false;
                ComboMonth.Visible = true;
                ComboYear.Visible = true;
                chartlab = "�����������";
            }
            else if (RList.SelectedIndex == 1)
            {
                if (Convert.ToInt32(ComboYear.SelectedValue) < 2011)
                {
                    queryName = "FO_0001_0013_compare_Grid1";
                    queryChartName = "FO_0001_0013_compare_Chart2";
                }
                else
                {
                    queryName = "FO_0001_0013_compare_Grid1_2011";
                    queryChartName = "FO_0001_0013_compare_Chart2_2011";
                }
                lab = "�������� ������";
                ComboOutcomes.Visible = false;
                ComboMonth.Visible = true;
                ComboYear.Visible = true;
                chartlab = "��������";
            }
            else
            {
                queryName = "FO_0001_0013_compare_Grid2";
                queryChartName = "FO_0001_0013_compare_Chart3";
                lab = "��������� ������";
                ComboOutcomes.Visible = true;
                outcomes.Value = ComboOutcomes.SelectedValue;
                ComboMonth.Visible = false;
                chartlab = "���������";
            }
            Label1.Text = "������������� ��������� �������� ����� ��������� � �������� ���������";
            string monthnum = Convert.ToString(ComboMonth.SelectedIndex + 2);
            if ((ComboMonth.SelectedIndex + 2) < 10)
                monthnum = "0" + monthnum;
            if (lab == "��������� ������")
            {
                Label2.Text = string.Format("<br/>������ ������� �� {0} ���", ComboYear.SelectedValue);

            }
            else
                if (ComboMonth.SelectedValue == "�������")
                {
                    if (lab == "�������� ������")
                    {

                        Label2.Text = string.Format("<br/>��������� ���������� �� {0} ���", ComboYear.SelectedValue);
                    }
                    else
                        Label2.Text = string.Format("<br/>��������� �� {0} ���", ComboYear.SelectedValue);

                }
                else
                    if (lab == "�������� ������")
                    {

                        Label2.Text = string.Format("<br/>��������� ���������� �� ��������� �� 01.{0}.{1}", monthnum, ComboYear.SelectedValue);
                    }
                    else
                        Label2.Text = string.Format("<br/>��������� �� 01.{0}.{1}", monthnum, ComboYear.SelectedValue);
          
            string patternValue = UserParams.StateArea.Value;
            int defaultRowIndex = 1;
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart1.DataBind();
            CRHelper.SaveToUserAgentLog(String.Format("�������� ���� {0}", Environment.TickCount - start));
        }

        #region ����������� �����
        /// <summary>
        /// ��������� ������ �����
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
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ��������", dtGrid);
            UltraWebGrid.DataSource = dtGrid;
        }

        private static string GetChartQuarterStr(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return string.Format("{0} �������", quarterNumber);
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
                if (primitive is Box)
                {
                    Box box = (Box)primitive;

                    if (box.DataPoint != null)
                    {
                        string percent = string.Format("{0:N2}",dtChart1.Rows[box.Column][box.Row + 1]);
                        string type = dtChart1.Rows[box.Column][0].ToString();
                        string succes = string.Empty;
                        if (Convert.ToInt32(ComboYear.SelectedValue) < 2011)
                        {
                            succes = string.Format("{0:N2}", dtGrid.Rows[box.Row][(box.Column + 1) * 2]);
                        }
                        else
                        {
                            succes = string.Format("{0:N2}", dtGrid.Rows[box.Row][(box.Column + 1) * 2 + 1]);
                        }
                        string lab = string.Empty;
                        if (Label2.Text.Contains("����������"))
                        {
                            lab = "����� ��������� ����������";
                        }
                        else
                        {
                            lab = "����� ������������ ����������"; 
                        }
                        box.DataPoint.Label = string.Format("{0} {1}% {2} {3} {4}", type, percent, lab, succes, units);
           
                    }
                }
            }

        }
        //dtChart1.Rows[box.Column][box.Row + 1].ToString(); ��������
        //dtChart1.Rows[box.Column][0].ToString(); ��� �������
        DataTable Chart = new DataTable();
        DataTable dtChart1 = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(queryChartName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {

                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("������������� �����", "��");
                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("������������� �����", "��");

            }

            UltraChart1.Series.Clear();
            for (int i = 1; i < dtChart1.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                series.Label = dtChart1.Columns[i].ColumnName;
                UltraChart1.Series.Add(series);
            }


            //UltraChart1.DataSource = dtChart1; ��������
            
        }

        private static void FillIncomesCombo()
        {
            if (IncomesDictionary.Count > 0)
                return;
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText("FO_0001_0013_compare_Grid_Kind");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtIncomes);

            DataTable dtIncomesChildren = new DataTable();
            query = DataProvider.GetQueryText("FO_0001_0013_compare_Grid_Kind_a");
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
            string query = DataProvider.GetQueryText("FO_0001_0013_compare_Grid_Kinds");
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
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(275);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 3;
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Header.RowLayoutColumnInfo.SpanY = 3;
                if ((Convert.ToInt32(ComboYear.SelectedValue) < 2011)||(RList.SelectedIndex == 2))
                {
                    for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                    {
                        e.Layout.Bands[0].Columns[k].Width = 135;
                        string formatString = "N2";
                        if ((k == 3) || (k == 5))
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
                        e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("���������, {0}", units);
                        e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("���������, {0}", units);
                        e.Layout.Bands[0].Columns[3].Header.Caption = "����";
                        e.Layout.Bands[0].Columns[4].Header.Caption = string.Format("���������, {0}", units);
                        e.Layout.Bands[0].Columns[5].Header.Caption = "����";
                        e.Layout.Bands[0].Columns[1].Header.Title = "�������� ������ ����������� ������ � ������ ����";
                        e.Layout.Bands[0].Columns[2].Header.Title = "�������� ������ ����������� ������ � ������ ����";
                        e.Layout.Bands[0].Columns[3].Header.Title = "���� ������� � ����� ����� �������� �������";
                        e.Layout.Bands[0].Columns[4].Header.Title = "�������� ������ ����������� ������ � ������ ����";
                        e.Layout.Bands[0].Columns[5].Header.Title = "���� ������� � ����� ����� �������� �������";

                    }
                    else
                        if (RList.SelectedIndex == 1)
                        {
                            e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("��������� ����������, {0}", units);
                            e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("��������� ����������, {0}", units);
                            e.Layout.Bands[0].Columns[3].Header.Caption = "����";
                            e.Layout.Bands[0].Columns[4].Header.Caption = string.Format("��������� ����������, {0}", units);
                            e.Layout.Bands[0].Columns[5].Header.Caption = "����";
                            e.Layout.Bands[0].Columns[1].Header.Title = "�������� ���������� �� ������� ���";
                            e.Layout.Bands[0].Columns[2].Header.Title = "�������� ���������� �� ������� ���";
                            e.Layout.Bands[0].Columns[3].Header.Title = "���� ������� � ����� ����� �������� �������";
                            e.Layout.Bands[0].Columns[4].Header.Title = "�������� ���������� �� ������� ���";
                            e.Layout.Bands[0].Columns[5].Header.Title = "���� ������� � ����� ����� �������� �������";

                        }
                        else
                            if (RList.SelectedIndex == 2)
                            {
                                e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("������, {0}", units);
                                e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("������, {0}", units);
                                e.Layout.Bands[0].Columns[3].Header.Caption = "����";
                                e.Layout.Bands[0].Columns[4].Header.Caption = string.Format("������, {0}", units);
                                e.Layout.Bands[0].Columns[5].Header.Caption = "����";
                                e.Layout.Bands[0].Columns[1].Header.Title = "������ ��������";
                                e.Layout.Bands[0].Columns[2].Header.Title = "������ ��������";
                                e.Layout.Bands[0].Columns[3].Header.Title = "���� ������� � ����� ����� �������� �������";
                                e.Layout.Bands[0].Columns[4].Header.Title = "������ ��������";
                                e.Layout.Bands[0].Columns[5].Header.Title = "���� ������� � ����� ����� �������� �������";

                            }

                    ColumnHeader ch = new ColumnHeader(true);
                    ch.Caption = "����������������� ������";
                    ch.Title = "";
                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = 1;
                    ch.RowLayoutColumnInfo.SpanX = 1;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                    ch = new ColumnHeader(true);
                    ch.Caption = "��������� ������";
                    ch.Title = "";
                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = 2;
                    ch.RowLayoutColumnInfo.SpanX = 2;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                    ch = new ColumnHeader(true);
                    ch.Caption = "������� �������";
                    ch.Title = "";
                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = 4;
                    ch.RowLayoutColumnInfo.SpanX = 2;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                }
                else
                {
                  for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                    {
                        e.Layout.Bands[0].Columns[k].Width = 135;
                        string formatString = "N2";
                        if ((k == 4) || (k == 6))
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
                        e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("���������, {0}", units);
                        e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("���������, {0}", units);
                        e.Layout.Bands[0].Columns[3].Header.Caption = string.Format("���������, {0}", units);
                        e.Layout.Bands[0].Columns[4].Header.Caption = "����";
                        e.Layout.Bands[0].Columns[5].Header.Caption = string.Format("���������, {0}", units);
                        e.Layout.Bands[0].Columns[6].Header.Caption = "����";
                        e.Layout.Bands[0].Columns[1].Header.Title = "�������� ������ ����������� ������ � ������ ����";
                        e.Layout.Bands[0].Columns[2].Header.Title = "�������� ������ ����������� ������ � ������ ����";
                        e.Layout.Bands[0].Columns[3].Header.Title = "�������� ������ ����������� ������ � ������ ����";
                        e.Layout.Bands[0].Columns[4].Header.Title = "���� ������� � ����� ����� �������� �������";
                        e.Layout.Bands[0].Columns[5].Header.Title = "�������� ������ ����������� ������ � ������ ����";
                        e.Layout.Bands[0].Columns[6].Header.Title = "���� ������� � ����� ����� �������� �������";

                    }
                    else
                        if (RList.SelectedIndex == 1)
                        {
                            e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("��������� ����������, {0}", units);
                            e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("��������� ����������, {0}", units);
                            e.Layout.Bands[0].Columns[3].Header.Caption = string.Format("��������� ����������, {0}", units);
                            e.Layout.Bands[0].Columns[4].Header.Caption = "����";
                            e.Layout.Bands[0].Columns[5].Header.Caption = string.Format("��������� ����������, {0}", units);
                            e.Layout.Bands[0].Columns[6].Header.Caption = "����";
                            e.Layout.Bands[0].Columns[1].Header.Title = "�������� ���������� �� ������� ���";
                            e.Layout.Bands[0].Columns[2].Header.Title = "�������� ���������� �� ������� ���";
                            e.Layout.Bands[0].Columns[3].Header.Title = "�������� ���������� �� ������� ���";
                            e.Layout.Bands[0].Columns[4].Header.Title = "���� ������� � ����� ����� �������� �������";
                            e.Layout.Bands[0].Columns[5].Header.Title = "�������� ���������� �� ������� ���";
                            e.Layout.Bands[0].Columns[6].Header.Title = "���� ������� � ����� ����� �������� �������";

                        }


                    ColumnHeader ch = new ColumnHeader(true);
                    ch.Caption = "����������������� ������";
                    ch.Title = "";
                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = 1;
                    ch.RowLayoutColumnInfo.SpanX = 1;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                    ch = new ColumnHeader(true);
                    ch.Caption = "����������� �����";
                    ch.Title = "";
                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = 2;
                    ch.RowLayoutColumnInfo.SpanX = 1;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                    ch = new ColumnHeader(true);
                    ch.Caption = "��������� ������";
                    ch.Title = "";
                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = 3;
                    ch.RowLayoutColumnInfo.SpanX = 2;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                    ch = new ColumnHeader(true);
                    ch.Caption = "������� �������";
                    ch.Title = "";
                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = 5;
                    ch.RowLayoutColumnInfo.SpanX = 2;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);
                }
            }
        }
       
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int min = 0;


            string styles = "background-repeat: no-repeat; background-position: center; margin: 2px";
        }

        #endregion

        #region ����������� ��������

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
        #region ������� � Excel

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
            if ((Convert.ToInt32(ComboYear.SelectedValue) < 2011) || (RList.SelectedIndex == 2))
            {

                if ((e.CurrentColumnIndex > 0) && (e.CurrentColumnIndex < 2))
                {
                    e.HeaderText = "���������������� ������";
                }
                else
                    if ((e.CurrentColumnIndex > 1) && (e.CurrentColumnIndex < 4))
                    {
                        e.HeaderText = "��������� ������";
                    }
                    else
                        if ((e.CurrentColumnIndex > 3))
                        {
                            e.HeaderText = "������� �������";
                        }
            }
            else
            {

                if ((e.CurrentColumnIndex > 0) && (e.CurrentColumnIndex < 2))
                {
                    e.HeaderText = "���������������� ������";
                }
                else
                    if ((e.CurrentColumnIndex > 1) && (e.CurrentColumnIndex < 3))
                    {
                        e.HeaderText = "����������� �����";
                    }
                    else
                        if ((e.CurrentColumnIndex > 2) && (e.CurrentColumnIndex < 5))
                            {
                                e.HeaderText = "��������� ������";
                            }
                             else
                                if ((e.CurrentColumnIndex > 4))
                                    {
                                        e.HeaderText = "������� �������";
                                    }
            }
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;


            e.CurrentWorksheet.Columns[0].Width = width * 30 * 3;
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
            if ((Convert.ToInt32(ComboYear.SelectedValue) < 2011) || (RList.SelectedIndex == 2))
            {
                e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
                int columnCounttt = UltraWebGrid.Columns.Count;
                for (int i = 1; i < columnCounttt; i = i + 1)
                {
                    if ((i == 3) || (i == 5))
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00%;[Red]-#,##0.00%";
                    }
                    else
                        if ((i == 6) || (i == 8) || (i == 9))
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0;[Red]-#,##0";
                        }
                        else
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                        }
                    e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
            }
            else
            {
                e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
                int columnCounttt = UltraWebGrid.Columns.Count;
                for (int i = 1; i < columnCounttt; i = i + 1)
                {
                    if ((i == 4) || (i == 6))
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00%;[Red]-#,##0.00%";
                    }
                    else
                        if ((i == 7) || (i == 8) || (i == 9))
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0;[Red]-#,##0";
                        }
                        else
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                        }
                    e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
            }
            for (int i = 3; i < 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 20 * 35;
            }
            e.CurrentWorksheet.Rows[5].Height = 20 * 15;
            for (int k = 1; k < UltraWebGrid.Columns.Count; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("�����. ��������");
            Worksheet sheet2 = workbook.Worksheets.Add("�������");
            sheet2.Rows[0].Cells[0].Value = ChartCaption1.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[1].Cells[0], UltraChart1);
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
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
            e.Section.AddPageBreak();
            title.AddContent(ChartCaption1.Text);
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
            e.Section.AddPageBreak();
            title = e.Section.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font.Bold = true;

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

        public IMetafile AddMetafile(Infragistics.Documents.Reports.Graphics.Metafile metafile)
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
