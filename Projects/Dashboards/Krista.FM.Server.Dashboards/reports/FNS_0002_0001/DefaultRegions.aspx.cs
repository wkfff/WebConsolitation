using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FNS_0002_0001
{
    public partial class DefaultRegions : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private int firstYear = 2000;
        private int endYear = 2011;
        private bool fns28nSplitting;

        #endregion
        
        #region ��������� �������

        // ������� �� � ��
        private CustomParam regionsLevel;
        // ������ �������
        private CustomParam fnsKDGroup;
        // ������ �����
        private CustomParam incomesTotal;
        // ��������� �����
        private CustomParam regionCenter;
        // ��������� ����
        private CustomParam selectedMeasure;

        #endregion

        private bool RegionCenterExcluding
        {
            get { return RegionCenterExclude.Checked; }
        }

        private bool AbsoluteMeasureSelected
        {
            get { return AbsobuteMeasure.Checked; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 18);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45 - 110);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 90);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 90);

            #region ��������� ��������� 1

            UltraChart1.ChartType = ChartType.StackColumnChart;
//            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 11;
            UltraChart1.Border.Thickness = 0;
            
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) / 3;
            UltraChart1.TitleLeft.Text = "���.���.";

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N2> ���.���.";

            UltraChart1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph1);

            #endregion

            #region ��������� ��������� 2

            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Axis.X.Extent = 160;
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
//            UltraChart2.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
//            UltraChart2.Axis.X.StripLines.PE.FillOpacity = 150;
//            UltraChart2.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
//            UltraChart2.Axis.X.StripLines.Interval = 2;
//            UltraChart2.Axis.X.StripLines.Visible = true;
            UltraChart2.Axis.Y.Extent = 65;
            UltraChart2.Axis.Y.Labels.ItemFormatString = AbsoluteMeasureSelected ? "<DATA_VALUE:N0>" : "<DATA_VALUE:P2>";

            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart2.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart2.Width.Value) / 4;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 9;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart2.Height.Value) / 4;
            UltraChart2.TitleLeft.Text = AbsoluteMeasureSelected ? "���.���." : " ";

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph2);

            #endregion

            #region ������������� ���������� �������

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (fnsKDGroup == null)
            {
                fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionCenter == null)
            {
                regionCenter = UserParams.CustomParam("region_center");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }

            #endregion

            KDLink.Visible = true;
            KDLink.Text = "��.&nbsp;�����&nbsp;��&nbsp;��������&nbsp;����������";
            KDLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultKD.aspx";

            OKVDLink.Visible = true;
            OKVDLink.Text = "��&nbsp;�����";
            OKVDLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultOKVD.aspx";

            AllocationLink.Visible = true;
            AllocationLink.Text = "���������&nbsp;�������������";
            AllocationLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultAllocation.aspx";

            SettlementLink.Visible = true;
            SettlementLink.Text = "��&nbsp;����������";
            SettlementLink.NavigateUrl = "~/reports/FNS_0002_0001/DefaultSettlement.aspx";

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("FNS28nSplitting"));

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel1.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel1.AddLinkedRequestTrigger(RegionCenterExclude.ClientID);

                AbsobuteMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", RelativeMeasure.ClientID));
                RelativeMeasure.Attributes.Add("onclick", string.Format("uncheck('{0}')", AbsobuteMeasure.ClientID));

                chartWebAsyncPanel2.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(AbsobuteMeasure.ClientID);
                chartWebAsyncPanel2.AddLinkedRequestTrigger(RelativeMeasure.ClientID);

                dtDate = new DataTable();
                string queryName = fns28nSplitting ? "FNS_0002_0001_date_split" : "FNS_0002_0001_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(dtDate.Rows[0][3].ToString(), true);

                ComboKD.Width = 420;
                ComboKD.Title = "��� ������";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullFNSKDIncludingList());
                ComboKD.Set�heckedState("��������� ������ ", true);
           }

            Page.Title = "������� �������� �� ������������� ������� � ��������� �������";
            PageTitle.Text = Page.Title;

            if (!chartWebAsyncPanel1.IsAsyncPostBack && !chartWebAsyncPanel2.IsAsyncPostBack)
            {
                int year = Convert.ToInt32(ComboYear.SelectedValue);

                UserParams.PeriodLastYear.Value = string.Format("[{0}].[��������� 2].[������� 4].[�������]", year - 1);

                int month = ComboMonth.SelectedIndex + 1;

                UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
                UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
                UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));
                regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
                fnsKDGroup.Value = ComboKD.SelectedValue;
                incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

                chart1Label1.Text = string.Format("�������/�������� �������� �� ������� �������� � ������ {0} ����", ComboYear.SelectedValue);
                chart1Label2.Text = "������������� ���������� �� �����/�������� �������� �� ������� �������� � ��������� � ������� �����";

                PageSubTitle.Text = string.Format("{3} �� {0} {1} {2} ����", month, CRHelper.RusManyMonthGenitive(month), year, ComboKD.SelectedValue);

                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
            }

            if (!chartWebAsyncPanel2.IsAsyncPostBack)
            {
                regionCenter.Value = RegionCenterExcluding ? string.Format(" {0}", RegionSettingsHelper.Instance.GetPropertyValue("RegionCenter")) : " ";
                UltraChart1.DataBind();
            }

            if (!chartWebAsyncPanel1.IsAsyncPostBack)
            {
                selectedMeasure.Value = AbsoluteMeasureSelected ? "�������/�������� � ���.���." : "�������/�������� � %";
                UltraChart2.DataBind();
            }
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0002_0001_regions_grid_split" : "FNS_0002_0001_regions_grid";
            string query = DataProvider.GetQueryText(queryName);
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
                        value = value.Replace("������������� �����", "��");
                        value = value.Replace("������������� �����", "��");
                        value = value.Replace("\"", "'");
                        row[i] = value.Replace(" �����", " �-�");
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count > 14)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

                int width = 95;
                int percentWidth = 70;
                int rankWidth = 78;

                SetColumnParams(e.Layout, 0, 1, "N2", width, false);
                SetColumnParams(e.Layout, 0, 2, "N2", width, false);
                SetColumnParams(e.Layout, 0, 3, "N2", width, false);
                SetColumnParams(e.Layout, 0, 4, "N2", width, false);
                SetColumnParams(e.Layout, 0, 5, "P2", percentWidth, false);
                SetColumnParams(e.Layout, 0, 6, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 7, "N0", width, true);
                SetColumnParams(e.Layout, 0, 8, "P2", rankWidth, false);
                SetColumnParams(e.Layout, 0, 9, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 10, "N0", width, true);
                SetColumnParams(e.Layout, 0, 11, "N2", width, false);
                SetColumnParams(e.Layout, 0, 12, "P2", width, false);
                SetColumnParams(e.Layout, 0, 13, "N0", rankWidth, false);
                SetColumnParams(e.Layout, 0, 14, "N0", width, true);

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 4 || i == 5)
                    //if (i > 3)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                }

                int month = ComboMonth.SelectedIndex + 1;
                int year = Convert.ToInt32(ComboYear.SelectedValue);

                int nextMonth = month;
                int nextYear = year;
                if (nextMonth == 12)
                {
                    nextMonth = 1;
                    nextYear++;
                }
                else
                {
                    nextMonth++;
                }

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "����������", "���������� ����������� �������");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1,
                    string.Format("�������� �� 01.{0:00}.{1}, ���.���.", nextMonth, nextYear - 1),
                    string.Format("�������� �� {0} {1} {2} ����.", month, CRHelper.RusManyMonthGenitive(month), year - 1));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2,
                    string.Format("�������� �� 01.{0:00}.{1}, ���.���.", nextMonth, nextYear),
                    string.Format("�������� �� {0} {1} {2} ����", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3,
                    string.Format("�������� �� ������ {0} ����, ���.���", year),
                    "�������� �� ������ ����");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4,
                    string.Format("���.���."),
                    "������� �������� � ������ ���� � ���.���.");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5,
                    string.Format("%"),
                    "������� �������� � ������ ���� � %");

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                    string.Format("���� �� �������� �������� � ������ {0} ���� � %", year),
                    "���� �� �������� �������� � ������ ���� � %");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8,
                    string.Format("���� ����� �������� � {0} ����, %", year - 1),
                    "���� ����� �������� �� ��������� � ������������ ������� �������� ����");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9,
                    string.Format("���� �� ����� ����� �������� � {0} ����", year),
                    "���� �� ����� ����� �������� �� ��������� � ������������ ������� �������� ����");
                // �� ����� ���� ��� ������� ������� � ������ ������ �� ����� ����� ��������
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10,
                    string.Format("����� ����� ����������� ��������� ������� �� 01.{0:00}.{1}, ���.���.", nextMonth, nextYear),
                    string.Format("����� ����� ����������� ��������� ������� �� {0} {1} {2} ����", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11,
                    string.Format("����� ����� ����������� ��������� ������� �� 01.{0:00}.{1}, ���.���.", nextMonth, nextYear),
                    string.Format("����� ����� ����������� ��������� ������� �� {0} {1} {2} ����", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12,
                    string.Format("�������� ��� �������� � ����� ������ ����������� ��������� �������, %"),
                    string.Format("�������� ��� �������� � ����� ������ ��������� ������� �� ������ ������������ ������ �� ���������� �������� �� {0} {1} {2} ����", month, CRHelper.RusManyMonthGenitive(month), year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 13,
                    string.Format("���� �� ��������� ����"),
                   "���� �� ��������� ���� �������� � ����� ������ ��������� �������");

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = string.Format("������� �������� � ������ {0} ����", year);
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 4;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool grow = (i == 5);
                bool growRate = (i == 8);
                bool rank = (i == 6 || i == 9 || i == 13);
                bool redValue = (i == 4 || i == 5);

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].ToString() == "��� ����������")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                if (redValue && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.ForeColor = Color.Red;
                    }
                }

                if (grow && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {

                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i].Title = "�������� ��������";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        e.Row.Cells[i].Title = "������� ��������";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (growRate && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i].Title = "���������� �������� �� ��������� � ����������� �������� �������� ����";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        e.Row.Cells[i].Title = "���������� �������� �� ��������� � ����������� �������� �������� ����";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string indicatorName = string.Empty;
                        switch(i)
                        {
                            case 6:
                                {
                                    indicatorName = "������� ��������";
                                    break;
                                }
                            case 9:
                                {
                                    indicatorName = "���� ����� ��������";
                                    break;
                                }
                            case 13:
                                {
                                    indicatorName = "�������� ���";
                                    break;
                                }
                        }
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("���������� {0}", indicatorName);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("���������� {0}", indicatorName);
                        }

                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && (e.Row.Cells[0].Value.ToString().ToLower().Contains("������")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }

        #endregion

        #region ����������� ���������

        protected void UltraChart_DataBinding1(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0002_0001_regions_chart1_split" : "FNS_0002_0001_regions_chart1";
            string query = DataProvider.GetQueryText(queryName);
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            foreach (DataRow row in dtChart1.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1 || i == 2 || i == 3)
                         && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }

                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
                        value = value.Replace("������������� �����", "��");
                        value = value.Replace("������������� �����", "��");
                        value = value.Replace("\"", "'");
                        row[i] = value.Replace(" �����", " �-�");
                    }
                }
            }

            UltraChart1.DataSource = dtChart1;
        }

        void UltraChart_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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
                        switch (box.DataPoint.Label)
                        {
                            case "�������� �� ������ ����":
                                {
                                    box.PE.ElementType = PaintElementType.Gradient;
                                    box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                    box.PE.Fill = Color.Yellow;
                                    box.PE.FillStopColor = Color.Goldenrod;
                                    box.PE.FillOpacity = 250;
                                    break;
                                }
                            case "������� ��������":
                                {
                                    box.PE.ElementType = PaintElementType.Gradient;
                                    box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                    box.PE.Fill = Color.Red;
                                    box.PE.FillStopColor = Color.DarkRed;
                                    box.PE.FillStopOpacity = 250;
                                    break;
                                }
                            case "�������� ��������":
                                {
                                    box.PE.ElementType = PaintElementType.Gradient;
                                    box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                    box.PE.Fill = Color.LimeGreen;
                                    box.PE.FillStopColor = Color.ForestGreen;
                                    box.PE.FillOpacity = 250;
                                    break;
                                }
                        }
                    }
                    else if (box.Path.Contains("Legend") && i != 0)
                    {
                        Primitive postPrimitive = e.SceneGraph[i + 1];
                        if (postPrimitive is Text)
                        {
                            Text text = (Text)postPrimitive;
                            switch (text.GetTextString())
                            {
                                case "�������� �� ������ ����":
                                    {
                                        box.PE.ElementType = PaintElementType.Gradient;
                                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                        box.PE.Fill = Color.Yellow;
                                        box.PE.FillStopColor = Color.Goldenrod;
                                        box.PE.FillOpacity = 250;
                                        break;
                                    }
                                case "������� ��������":
                                    {
                                        box.PE.ElementType = PaintElementType.Gradient;
                                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                        box.PE.Fill = Color.DarkRed;
                                        box.PE.FillStopColor = Color.DarkRed;
                                        box.PE.FillStopOpacity = 250;
                                        break;
                                    }
                                case "�������� ��������":
                                    {
                                        box.PE.ElementType = PaintElementType.Gradient;
                                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                        box.PE.Fill = Color.LimeGreen;
                                        box.PE.FillStopColor = Color.ForestGreen;
                                        box.PE.FillOpacity = 250;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        }

        protected void UltraChart_DataBinding2(object sender, EventArgs e)
        {
            string queryName = fns28nSplitting ? "FNS_0002_0001_regions_chart2_split" : "FNS_0002_0001_regions_chart2";
            string query = DataProvider.GetQueryText(queryName);
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            foreach (DataRow row in dtChart2.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
                        value = value.Replace("������������� �����", "��");
                        value = value.Replace("������������� �����", "��");
                        value = value.Replace("\"", "'");
                        row[i] = value.Replace(" �����", " �-�");
                    }
                }
            }

            UltraChart2.DataSource = dtChart2;
        }

        void UltraChart_FillSceneGraph2(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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
                    int year = Convert.ToInt32(ComboYear.SelectedValue);
                    if (box.DataPoint != null && box.Value != null)
                    {
                        double value = Convert.ToDouble(box.Value);
                        if (value > 0)
                        {
                            box.DataPoint.Label = string.Format("{0}\n���� �������� � {2} ����\n{1}",
                                box.Series.Label,
                                AbsoluteMeasureSelected ? string.Format("{0:N2} ���.���.", value) : string.Format("{0:P2}", value),
                                year - 1);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Maroon;
                        }
                        else
                        {
                            box.DataPoint.Label = string.Format("{0}\n�������� �������� � {2} ����\n{1}",
                                box.Series.Label,
                                AbsoluteMeasureSelected ? string.Format("{0:N2} ���.���.", value) : string.Format("{0:P2}", value),
                                year - 1);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                    }
                    else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                    {
                        box.PE.ElementType = PaintElementType.CustomBrush;
                        LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Red, Color.Green, 45, false);
                        box.PE.CustomBrush = brush;
                    }
                }
            }
        }

        #endregion

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int width = 107 * 37;

            e.CurrentWorksheet.Columns[1].Width = width;
            e.CurrentWorksheet.Columns[2].Width = width;
            e.CurrentWorksheet.Columns[3].Width = width;
            e.CurrentWorksheet.Columns[4].Width = width;
            e.CurrentWorksheet.Columns[5].Width = width;
            e.CurrentWorksheet.Columns[6].Width = width;
            e.CurrentWorksheet.Columns[7].Width = width;
            e.CurrentWorksheet.Columns[8].Width = width;
            e.CurrentWorksheet.Columns[9].Width = width;
            e.CurrentWorksheet.Columns[10].Width = width;
            e.CurrentWorksheet.Columns[11].Width = width;
            e.CurrentWorksheet.Columns[12].Width = width;
            e.CurrentWorksheet.Columns[13].Width = width;
            e.CurrentWorksheet.Columns[14].Width = width;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "[Red]#,##0.00%;-#,##0.00%";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = "#0";

            // ����������� ����� � ����� ������
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 20 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (e.CurrentColumnIndex == 4 || e.CurrentColumnIndex == 5)
            {
                int year = Convert.ToInt32(ComboYear.SelectedValue);
                e.HeaderText = string.Format("������� �������� � ������ {0} ����", year);
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region ������� � PDF

        private static void SetGridParams(UltraWebGrid grid)
        {
            int offset = 0;
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                UltraGridColumn column = grid.Columns[i];
                if (column.Hidden)
                {
                    offset++;
                }

                if (i + offset < grid.Columns.Count)
                {
                    column.Header.Caption = grid.Columns[i + offset].Header.Caption;
                }
            }
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            SetGridParams(UltraWebGrid);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chart1Label2.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chart1Label1.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
