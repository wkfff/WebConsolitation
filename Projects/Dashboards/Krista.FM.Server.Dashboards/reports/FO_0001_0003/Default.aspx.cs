using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;
using Orientation=Infragistics.Documents.Excel.Orientation;


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0003
{
    public partial class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        
        private GridHeaderLayout headerLayout;

        private int firstYear = 2008; 
      #endregion

        #region

        public CustomParam SelectYear;
        public CustomParam LastHalf;
        public CustomParam LastQuarter;
        public CustomParam SelectPost;
        public CustomParam Param;
        public CustomParam CurReport;
        

        public string curDate;
        public string lastDate;
        public int endYear;
        public string quart;
       
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -20 );
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.9 );
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2);

          /* 
            CrossLink1.Visible = true;
            CrossLink1.Text = "�����������&nbsp;�������&nbsp;��&nbsp;��������&nbsp;����������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0005_XMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�������&nbsp;�����������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0006_XMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "������&nbsp;��������������&nbsp;��������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0007_XMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "������&nbsp;��������&nbsp;��&nbsp;���&nbsp;�&nbsp;����";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0003_XMAO/Default.aspx";
          */
            #region ������������� ����������
             if (SelectYear ==null)
             {
                 SelectYear = UserParams.CustomParam("select_year");
             }

            if (SelectPost == null)
             {
                 SelectPost = UserParams.CustomParam("select_post");
             }
             if (LastHalf == null)
             {
                 LastHalf = UserParams.CustomParam("lastHalf");
             }

             if (LastQuarter == null)
             {
                 LastQuarter = UserParams.CustomParam("lastQuater");
             }

             if (Param == null)
             {
                 Param = UserParams.CustomParam("param");
             }

             if (CurReport == null)
             {
                 CurReport = UserParams.CustomParam("cur_report");
             }

            #endregion

             #region ��������� ���������
             System.Drawing.Font font = new System.Drawing.Font("Verdana", 10);
             UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Data.ZeroAligned = true;
             UltraChart1.Border.Thickness = 0;
             UltraChart1.Axis.X.Extent = 20;
             UltraChart1.Axis.Y.Extent = 40;
             UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
             UltraChart1.Axis.X.Labels.Visible = false;
             UltraChart1.Axis.X.Labels.SeriesLabels.Font = font;
             UltraChart1.Axis.Y.Labels.Font = font;
             UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            //UltraChart1.Axis.Y.TickmarkStyle = AxisTickStyle.DataInterval;
            //UltraChart1.Axis.Y.ti

             UltraChart1.Data.SwapRowsAndColumns = false;

             UltraChart1.TitleLeft.Visible = true;
             UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
             UltraChart1.TitleLeft.Text = "�������";


             UltraChart1.Legend.Visible = true;
             UltraChart1.Legend.Location = LegendLocation.Right;
             UltraChart1.Legend.SpanPercentage = 20;

             UltraChart1.Legend.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value / 1.5);
             UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n <DATA_VALUE:N0> ���. \n �� <SERIES_LABEL>");
             UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

             #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chart1ElementCaption.Text = string.Empty;

                chartsWebAsyncPanel.AddRefreshTarget(chart1ElementCaption);
                chartsWebAsyncPanel.AddRefreshTarget(UltraChart1);
                chartsWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid1);

                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0003_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quart = dtDate.Rows[0][2].ToString();

                ComboPeriod.Title = "������";
                ComboPeriod.Visible = true;
                ComboPeriod.Width = 120;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboPeriod.MultiSelect = false;
                ComboPeriod.Set�heckedState(endYear.ToString(),true);

                Collection <string> quarter = new Collection<string>();
                quarter.Add("������� 2");
                quarter.Add("������� 3");
                quarter.Add("������� 4");

                ComboQuarter.Title = "�������";
                ComboQuarter.Width = 150;
                ComboQuarter.FillValues(quarter);
                ComboQuarter.MultiSelect = false;
                ComboQuarter.Set�heckedState(quart, true);

                Dictionary<string, int> post = new Dictionary<string, int>();

                post.Add("����������� ����������, ���������� ���. ���������", 0);
                post.Add("����� ����������, ���������� ��������� ���. �����. ������", 0);
                post.Add("����������� ���������� ���. ������, ���������� ���������, �� ���������� ����������� ���. �����. ������", 0);
                post.Add("����������� ���������� ���. ������, ������ ����� ������� ������������ �� ������ ������ �������� �����", 0);
                post.Add("����� ����������� ���������� ���. ������", 0);

                ComboPost.Title = "���������";
                ComboPost.Visible = true;
                ComboPost.Width = 690;
                ComboPost.FillDictionaryValues(post);
                ComboPost.Set�heckedState("����� ����������� ���������� ���. ������", true);
                
                Dictionary<string, int> postMO = new Dictionary<string, int>();

                postMO.Add("����������� ����������, ���������� ���. ���������", 0);
                postMO.Add("����� ����������, ���������� ��������� ���. ������", 0);
                postMO.Add("����������� ���������� ����, ������������� ��������, ���������� ���������, �� ���������� ����������� ���. ������", 0);
                postMO.Add("����������� ���������� ���� � ������������� ��������, ������ ����� ������� ������������ �� ������ ������ �������� �����", 0);
                postMO.Add("����� ����������� ���������� ���� � ������������� ��������", 0);

                ComboPostMO.Title = "���������";
                ComboPostMO.Visible = false;
                ComboPostMO.Width = 770;
                ComboPostMO.FillDictionaryValues(postMO);
                ComboPostMO.Set�heckedState("����� ����������� ���������� ���� � ������������� ��������", true);
            }

            int year = Convert.ToInt32(ComboPeriod.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex + 2));
            UserParams.PeriodQuater.Value = ComboQuarter.SelectedValue;

            curDate = string.Empty;
            lastDate = string.Empty;
            lastDate = string.Format("01.01.{0}", ComboPeriod.SelectedValue);
            UserParams.PeriodLastYear.Value = (year -1).ToString();
            LastQuarter.Value = "������� 4";
            LastHalf.Value = "��������� 2";

            if (ComboQuarter.SelectedIndex == 0) // ������ ������ �������
            {
              curDate = string.Format("01.07.{0}", ComboPeriod.SelectedValue);
                
            }
            if (ComboQuarter.SelectedIndex == 1) // ������ ������ �������
            {
              curDate = string.Format("01.10.{0}", ComboPeriod.SelectedValue);
                
            }
            if (ComboQuarter.SelectedIndex == 2) // ������ ��������� �������
            {
               curDate = string.Format("01.01.{0}", Convert.ToInt32(ComboPeriod.SelectedValue) + 1);
            }

           PageTitle.Text = string.Empty;

            if (DebtKindButtonList.SelectedIndex == 0)
            {
                Page.Title = "����������� ���������� ������� ��������������� ������ �����-����������� ����������� ������-����";
                PageSubTitle.Text = string.Format("{1}, ���������� �� ��������� �� {0} �.", curDate, ComboPost.SelectedValue);
                ComboPost.Visible = true;
                ComboPostMO.Visible = false;
                Param.Value = "[�������������__����������].[�������������__����������].[��� ��������������].[���� �����-����������� ����������� ������ � ����]";
                CurReport.Value = "[������������__����].[������������__����].[���].[���������� ������� �������������� ������� �������]";
                chart1ElementCaption.Text = string.Format("�������� ����������� ���������� ������� ��������������� ������: {0}, ���� �����-����������� ����������� ������ � ����", ComboPost.SelectedValue);
             
                switch (ComboPost.SelectedValue)
                {
                    case "����������� ����������, ���������� ���. ���������":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ���������������� ������ ( ������ �������� ��������������)    (����� ����� 200+230+280+290+300)].[����������� ����������, ���������� ��������������� (�������������) ���������]";
                            break;
                        }
                    case "����� ����������, ���������� ��������� ���. �����. ������":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ���������������� ������ ( ������ �������� ��������������)    (����� ����� 200+230+280+290+300)].[�����  ����������, ���������� ��������� ��������������� ����������� ������ ��� ������������� ������      (����� ����� 210+220)]";
                            break;
                        }
                    case "����������� ���������� ���. ������, ���������� ���������, �� ���������� ����������� ���. �����. ������":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ���������������� ������ ( ������ �������� ��������������)    (����� ����� 200+230+280+290+300)].[����������� ���������� ���������������� ������ (������ �������� ��������������), ���������� ���������, �� ���������� ����������� ��������������� ����������� ������ ��� ������������� ������]";
                            break;
                        }
                    case "����������� ���������� ���. ������, ������ ����� ������� ������������ �� ������ ������ �������� �����":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ���������������� ������ ( ������ �������� ��������������)    (����� ����� 200+230+280+290+300)].[����������� ���������� ���������������� ������ (������ �������� ��������������), ������ ����� ������� ������������  �� ������ ������ �������� ����� �� ������ ����� ���������� ��������� �����  ***)]";
                            break;
                        }
                    case "����� ����������� ���������� ���. ������":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ���������������� ������ ( ������ �������� ��������������)    (����� ����� 200+230+280+290+300)]";
                            break;
                        }
                }

            }
            else
            {
                Page.Title = "����������� ���������� ������� �������� �������������� � ������������� �������� �� �����-����������� ����������� ������-����";
                PageSubTitle.Text = string.Format("{1}, ���������� �� ��������� �� {0} �.", curDate, ComboPostMO.SelectedValue);
                ComboPost.Visible = false;
                ComboPostMO.Visible = true;
                Param.Value = "[������__������������].[������__������������].[��� ������].[�.�����������]";
                CurReport.Value = "[������������__����].[������������__����].[���].[������ ����. ������ �������� � ������� �������, ������� � ��������� (5+6+7+8)]";
                chart1ElementCaption.Text = string.Format("�������� ����������� ���������� ������� �������� �������������� � ������������� ��������: {0}, �.�����������", ComboPostMO.SelectedValue);
                switch (ComboPostMO.SelectedValue)
                {
                    case "����������� ����������, ���������� ���. ���������":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ������ �������� ��������������, ������������� �������� �������������� �����������  (����� ����� 200+210+270+280)].[����������� ����������, ���������� ������������� ���������]";
                            break;
                        }
                    case "����� ����������, ���������� ��������� ���. ������":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ������ �������� ��������������, ������������� �������� �������������� �����������  (����� ����� 200+210+270+280)].[����������� ����������, ���������� ��������� ������������� ������  (����� ����� 220+230+240+250+260)]";
                            break;
                        }
                    case
                        "����������� ���������� ����, ������������� ��������, ���������� ���������, �� ���������� ����������� ���. ������"
                        :
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ������ �������� ��������������, ������������� �������� �������������� �����������  (����� ����� 200+210+270+280)].[����������� ���������� ������ �������� ��������������, ������������� �������� �������������� �����������, ���������� ���������, �� ���������� ����������� ������������� ������]";
                            break;
                        }
                    case
                        "����������� ���������� ���� � ������������� ��������, ������ ����� ������� ������������ �� ������ ������ �������� �����"
                        :
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ������ �������� ��������������, ������������� �������� �������������� �����������  (����� ����� 200+210+270+280)].[����������� ���������� ������ �������� ��������������, ������������� �������� �������������� �����������, ������ ����� ������� ������������  �� ������ ������ �������� ����� �� ������ ����� ���������� ��������� �����  *)]";
                            break;
                        }
                    case "����� ����������� ���������� ���� � ������������� ��������":
                        {
                            SelectPost.Value = "[���������__������������].[���������__������������].[���].[����� ����������� ����������  ������ �������� ��������������, ������������� �������� �������������� �����������  (����� ����� 200+210+270+280)]";
                            break;
                        }
                }
            }

            PageTitle.Text = Page.Title;
            
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
           
        }

        #region ���������� �����
        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            if (DebtKindButtonList.SelectedIndex == 0)
            {
                CurReport.Value = "[������������__����].[������������__����].[���].[���������� ������� �������������� ������� �������]";
                Param.Value = string.Format("[�������������__����������].[�������������__����������].[��� ��������������].[{0}]", subject);
                chart1ElementCaption.Text = string.Format("�������� ����������� ���������� ������� ��������������� ������: {0}, {1}", ComboPost.SelectedValue ,subject);
            }
            else
            {
                CurReport.Value = "[������������__����].[������������__����].[���].[������ ����. ������ �������� � ������� �������, ������� � ��������� (5+6+7+8)]";
                Param.Value = string.Format("[������__������������].[������__������������].[��� ������].[{0}]", subject);
                chart1ElementCaption.Text = string.Format("�������� ����������� ���������� ������� �������� �������������� � ������������� ��������: {0}, {1}", ComboPostMO.SelectedValue, subject);
            }

            UltraChart1.DataBind();

        }
       
        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            if (DebtKindButtonList.SelectedIndex == 0)
            {
                string query = DataProvider.GetQueryText("FO_0001_0003_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid);

                if (dtGrid.Rows.Count > 1)
                {
                   
                   UltraWebGrid1.DataSource = dtGrid;
                }
            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0001_0003_Grid_MO");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid);


                if (dtGrid.Rows.Count > 1)
                {
                   
                    UltraWebGrid1.DataSource = dtGrid;
                }
            }

        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (dtGrid.Rows.Count < 10)
            {
                UltraWebGrid1.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);

            if (DebtKindButtonList.SelectedIndex == 0)
            {
                headerLayout.AddCell("������������ ���������������� ������", "", 2);
            }
            else
            {
                headerLayout.AddCell("������������ �������������� �����������", "", 2);
            }

            if ((DebtKindButtonList.SelectedIndex ==0 && ComboPost.SelectedValue == "����� ����������� ���������� ���. ������" )|| (DebtKindButtonList.SelectedIndex==1 && ComboPostMO.SelectedValue == "����� ����������� ���������� ���� � ������������� ��������"))
                {
                    e.Layout.Bands[0].Columns[5].Hidden = true;
                    e.Layout.Bands[0].Columns[6].Hidden = true;
                    e.Layout.Bands[0].Columns[11].Hidden = true;
                    e.Layout.Bands[0].Columns[12].Hidden = true;
                    e.Layout.Bands[0].Columns[20].Hidden = true;
                    e.Layout.Bands[0].Columns[21].Hidden = true;

                    GridHeaderCell cell0 = headerLayout.AddCell("���������� ������� ������, ���.");
                    cell0.AddCell(string.Format("�� {0}", lastDate), string.Format("���������� ������� ������ �� ���������� � ������� ���������� �� {0} �.", lastDate));
                    cell0.AddCell(string.Format("�� {0}", curDate), string.Format("���������� ������� ������ �� ���������� � ������� ���������� �� {0} �.", curDate));
                    cell0.AddCell("����������", "���������� �� ����������� ��������� �������");
                    cell0.AddCell("�������/ ��������, %", "�������/�������� � �������� ��������� �������");

                    GridHeaderCell cell1 = headerLayout.AddCell("���������� �������� ������� ������, ���.");
                    cell1.AddCell(string.Format("�� {0}", lastDate), string.Format("���������� �������� ������� ������ �� {0} �.", lastDate));
                    cell1.AddCell(string.Format("�� {0}", curDate), string.Format("���������� �������� ������� ������ �� {0} �.", curDate));
                    cell1.AddCell("����������", "���������� �� ����������� ��������� �������");
                    cell1.AddCell("�������/ ��������, %", "�������/�������� � �������� ��������� �������");

                    GridHeaderCell cell3 = headerLayout.AddCell("% ������������ ���������");
                    cell3.AddCell(string.Format("�� {0}", lastDate), string.Format("������� ������������ ��������� �� {0} �.", lastDate));
                    cell3.AddCell(string.Format("�� {0}", curDate), string.Format("������� ������������ ��������� �� {0} �.", curDate));
                    cell3.AddCell("����������", "���������� �� ����������� ��������� �������");

                    GridHeaderCell cell2 = headerLayout.AddCell("��������������� �����������, ���.");
                    cell2.AddCell(string.Format("�� {0}", lastDate), string.Format("��������������� ����������� �� {0} �.", lastDate));
                    cell2.AddCell(string.Format("�� {0}", curDate), string.Format("��������������� ����������� �� {0} �.", curDate));
                    cell2.AddCell("����������", "���������� �� ����������� ��������� �������");
                    cell2.AddCell("�������/ ��������, %", "�������/�������� � �������� ��������� �������");
                }
                else
                {
                    GridHeaderCell cell0 = headerLayout.AddCell("���������� ������� ������, ���.");
                    cell0.AddCell(string.Format("�� {0}", lastDate), string.Format("���������� ������� ������ �� ���������� � ������� ���������� �� {0} �.", lastDate));
                    cell0.AddCell(string.Format("�� {0}", curDate), string.Format("���������� ������� ������ �� ���������� � ������� ���������� �� {0} �.", curDate));
                    cell0.AddCell("����������", "���������� �� ����������� ��������� �������");
                    cell0.AddCell("�������/ ��������, %", "�������/�������� � �������� ��������� �������");

                    GridHeaderCell cell4 = cell0.AddCell("���� � ����� �����������");
                    cell4.AddCell(string.Format("�� {0}, %", lastDate), "�������� ��� ����������� �� ��������� ��������� � ����� ����������� ����������");
                    cell4.AddCell(string.Format("�� {0}, %", curDate), "�������� ��� ����������� �� ��������� ��������� � ����� ����������� ����������");
                    
                    GridHeaderCell cell1 = headerLayout.AddCell("���������� �������� ������� ������, ���.");
                    cell1.AddCell(string.Format("�� {0}", lastDate), string.Format("���������� �������� ������� ������ �� {0} �.", lastDate));
                    cell1.AddCell(string.Format("�� {0}", curDate), string.Format("���������� �������� ������� ������ �� {0} �.", curDate));
                    cell1.AddCell("����������", "���������� �� ����������� ��������� �������");
                    cell1.AddCell("�������/ ��������, %", "�������/�������� � �������� ��������� �������");

                    GridHeaderCell cell5 = cell1.AddCell("���� � ����� �����������");
                    cell5.AddCell(string.Format("�� {0}, %", lastDate), "�������� ��� ����������� �� ��������� ��������� � ����� ����������� ����������");
                    cell5.AddCell(string.Format("�� {0}, %", curDate), "�������� ��� ����������� �� ��������� ��������� � ����� ����������� ����������");
                    
                    GridHeaderCell cell3 = headerLayout.AddCell("% ������������ ���������");
                    cell3.AddCell(string.Format("�� {0}", lastDate), string.Format("������� ������������ ��������� �� {0} �.", lastDate),2);
                    cell3.AddCell(string.Format("�� {0}", curDate), string.Format("������� ������������ ��������� �� {0} �.", curDate),2);
                    cell3.AddCell("����������", "���������� �� ����������� ��������� �������");
                    

                    GridHeaderCell cell2 = headerLayout.AddCell("��������������� �����������, ���.");
                    cell2.AddCell(string.Format("�� {0}", lastDate), string.Format("��������������� ����������� �� {0} �.", lastDate));
                    cell2.AddCell(string.Format("�� {0}", curDate), string.Format("��������������� ����������� �� {0} �.", curDate));
                    cell2.AddCell("����������", "���������� �� ����������� ��������� �������");
                    cell2.AddCell("�������/ ��������, %", "�������/�������� � �������� ��������� �������");

                    GridHeaderCell cell6 = cell2.AddCell("���� � ����� �����������");
                    cell6.AddCell(string.Format("�� {0}, %", lastDate), "�������� ��� ����������� �� ��������� ��������� � ����� ����������� ����������");
                    cell6.AddCell(string.Format("�� {0}, %", curDate), "�������� ��� ����������� �� ��������� ��������� � ����� ����������� ����������");
                    

                }
            
            headerLayout.ApplyHeaderInfo();
        
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80);
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[13], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[14], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[15], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[19], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[20], "P1");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[21], "P1");
         
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
           //e.Row.Cells[0].ToString().Replace("�����-����������� ����������� ������ � ����", " ");
           
           for (int i=1; i<e.Row.Cells.Count;i++)
              {
                
                  if (i == 4 || i == 10 || i == 19)
                  {
                      if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                              e.Row.Cells[i].Title = "������� � �������� ��������� �������";
                          }
                          else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                              e.Row.Cells[i].Title = "�������� � �������� ��������� �������";
                          }

                          e.Row.Cells[i].Style.CustomRules =
                              "background-repeat: no-repeat; background-position: left center; margin: 2px";
                      }
                   
                  }

                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                 {
                     decimal value;
                     if (decimal.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                     {
                         if (value < 0)
                         {
                             e.Row.Cells[i].Style.ForeColor = Color.Red;
                         }
                     }
                 }
               
              }
              if (e.Row.Cells[0].Value.ToString() == "�����")
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                }
           
          }
        
        #endregion 

        #region ����������� ��������

        protected void UltraChart1_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0003_Chart1");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���������1", dtChart1);

            DataTable dtChart11 = new DataTable();
            query = DataProvider.GetQueryText("FO_0001_0003_Chart1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtChart11);

            List<string> yearList = new List<string>();

            for (int j = 0; j < dtChart1.Rows.Count; j++)
            {
                DataRow row = dtChart1.Rows[j];

                if (row[0] != DBNull.Value && dtChart11.Rows[j][0] != DBNull.Value)
                {
                    if (row[0].ToString() == "������� 4")
                    {
                        row[0] = string.Format("{0}, �� 4", dtChart11.Rows[j][0]);
                        yearList.Add(dtChart11.Rows[j][0].ToString());
                    }

                    if (row[0].ToString() == "������� 3")
                    {
                        row[0] = string.Format("{0}, �� 3", dtChart11.Rows[j][0]);
                        yearList.Add(dtChart11.Rows[j][0].ToString());
                    }

                    if (row[0].ToString() == "������� 2")
                    {
                        row[0] = string.Format("{0}, �� 2", dtChart11.Rows[j][0]);
                        yearList.Add(dtChart11.Rows[j][0].ToString());
                    }
                }
            }
            UltraChart1.DataSource = dtChart1;
        }

        #endregion

        #region �������

        #region ������� � Excel

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesVertically = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 5; j < rowsCount + 5; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Font.Height = 200;
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.RowsAutoFitEnable = true;

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, sheet2, 3);
           
        }

    

        #endregion

        #region ������� � Pdf
      
        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
           
            ReportPDFExporter1.HeaderCellHeight = 50;
            UltraChart1.Width = 1000;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text ,section2);
         

        }


        #endregion

      #endregion


       }
}