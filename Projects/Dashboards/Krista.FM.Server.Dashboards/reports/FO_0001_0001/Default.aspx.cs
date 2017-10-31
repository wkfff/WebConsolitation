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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0001
{
    public partial class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        private DataTable dt;
        private DataTable dt1;
        private DataTable dt11;
        private DataTable dt12;
        private DataTable dt13;
        private DataTable dtChart1;
        private DataTable dtChart11;
        private DataTable dtChart2;
  

        private int firstYear = 2008;
        public string curDate;
        public string lastDate;

        public CustomParam LastHalf;
        public CustomParam LastQuater;
        public CustomParam RzPR;
        public CustomParam Admin;

        private GridHeaderLayout headerLayout;

      #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth/2);
            UltraChart2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);

            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2);
            UltraChart3.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2);

            CrossLink1.Visible = true;
            CrossLink1.Text = "�����������&nbsp;����������&nbsp;���.&nbsp;�������";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0002/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�����������&nbsp;�������&nbsp;��&nbsp;����������&nbsp;���.&nbsp;��������";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0005/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "�����������&nbsp;�������&nbsp;��&nbsp;����������&nbsp;���.&nbsp;��������";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0007/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "������������&nbsp;�������&nbsp;�����������&nbsp;���.&nbsp;��������";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0006/Default.aspx";

            #region ������������� ����������
             

             if (RzPR == null)
             {
                 RzPR = UserParams.CustomParam("RzPR");
             }

             if (Admin == null)
             {
                 Admin = UserParams.CustomParam("Admin");
             }

             if (LastHalf == null)
             {
                 LastHalf = UserParams.CustomParam("lastHalf");
             }

             if (LastQuater == null)
             {
                 LastQuater = UserParams.CustomParam("lastQuater");
             }
            #endregion

            #region  ��������� ��������

             System.Drawing.Font font = new System.Drawing.Font("Verdana", 10);
             System.Drawing.Font font1 = new System.Drawing.Font("Verdana", 9);

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 20;
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = font;
            UltraChart1.Axis.Y.Labels.Font = font;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
         
            UltraChart1.Data.SwapRowsAndColumns = false;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Text = "�������";

           
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Right;
            UltraChart1.Legend.SpanPercentage = 20;

            UltraChart1.Legend.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value / 1.5);

            /*ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = 1;
            appearance.Row = -2;
            appearance.Column = -2; 
            appearance.VerticalAlign = StringAlignment.Center;
            appearance.ItemFormatString = "<DATA_VALUE_ITEM:N0>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart1.ColumnChart.ChartText.Add(appearance);*/

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
 
            UltraChart2.ChartType = ChartType.DoughnutChart;
            UltraChart2.DoughnutChart.Labels.Font = font;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Legend.Visible = false;
           
            UltraChart2.DoughnutChart.ShowConcentricLegend = false;
            UltraChart2.DoughnutChart.Concentric = true;
            UltraChart2.DoughnutChart.OthersCategoryPercent = 0;

            UltraChart2.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleTop.Font = font;
           
            UltraChart2.DoughnutChart.RadiusFactor = 80;

            
            CalloutAnnotation planAnnotation1 = new CalloutAnnotation();
            planAnnotation1.Text = "���������� ��������\n ������� ������";
            planAnnotation1.TextStyle.Font = font1;
            planAnnotation1.Width = 148;
            planAnnotation1.Height = 37;
            planAnnotation1.Location.Type = LocationType.Percentage;
            planAnnotation1.Location.LocationX = 50;
            planAnnotation1.Location.LocationY = 71;

            CalloutAnnotation factAnnotation1 = new CalloutAnnotation();
            factAnnotation1.Text = "���������� \n ������� ������";
            factAnnotation1.TextStyle.Font = font1;
            factAnnotation1.Width = 130;
            factAnnotation1.Height = 37;
            factAnnotation1.Location.Type = LocationType.Percentage;
            factAnnotation1.Location.LocationX = 50;
            factAnnotation1.Location.LocationY = 21;

            UltraChart2.Annotations.Add(planAnnotation1);
            UltraChart2.Annotations.Add(factAnnotation1);
            UltraChart2.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart2_ChartDrawItem);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);


            UltraChart3.ChartType = ChartType.DoughnutChart;
            UltraChart3.Border.Thickness = 0;
            UltraChart3.DoughnutChart.Labels.Font = font;


            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.SpanPercentage = 35;
            UltraChart3.Legend.Margins.Bottom = 0;
            UltraChart3.Legend.Margins.Top = 0;
            UltraChart3.Legend.Margins.Left = 0;
            UltraChart3.Legend.Margins.Right = 0;
            UltraChart3.Legend.Location = LegendLocation.Right;

            UltraChart3.Legend.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value /1.5);
            UltraChart3.DoughnutChart.ShowConcentricLegend = false;
            UltraChart3.DoughnutChart.Concentric = true;
            UltraChart3.DoughnutChart.OthersCategoryPercent = 0;

            UltraChart3.TitleTop.HorizontalAlign = StringAlignment.Near;
            UltraChart3.TitleTop.Font = font;
            UltraChart3.TitleTop.Margins.Left = Convert.ToInt32((UltraChart3.Width.Value)) * UltraChart3.Legend.SpanPercentage / 100 - 65;
            UltraChart3.DoughnutChart.RadiusFactor = 80;

            CalloutAnnotation planAnnotation2 = new CalloutAnnotation();
            planAnnotation2.Text = "���������� ��������\n ������� ������";
            planAnnotation2.TextStyle.Font = font1;
            planAnnotation2.Width = 148;
            planAnnotation2.Height = 35;
            planAnnotation2.Location.Type = LocationType.Percentage;
            planAnnotation2.Location.LocationX = 33;
            planAnnotation2.Location.LocationY = 70;

            CalloutAnnotation factAnnotation2 = new CalloutAnnotation();
            factAnnotation2.Text = "���������� \n ������� ������";
            factAnnotation2.TextStyle.Font = font1;
            factAnnotation2.Width = 130;
            factAnnotation2.Height = 37;
            factAnnotation2.Location.Type = LocationType.Percentage;
            factAnnotation2.Location.LocationX = 33;
            factAnnotation2.Location.LocationY = 22;

            UltraChart3.Annotations.Add(planAnnotation2);
            UltraChart3.Annotations.Add(factAnnotation2);
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);


            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        void UltraChart2_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //������������� ������ ������ ������� 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart2.Legend.Location == LegendLocation.Top) || (UltraChart2.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart2.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChart2.Legend.SpanPercentage * (int)UltraChart2.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart2.Legend.Margins.Left + UltraChart2.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0001_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quart = dtDate.Rows[0][2].ToString();

                ComboPeriod.Title = "���";
                ComboPeriod.Visible = true;
                ComboPeriod.Width = 100;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboPeriod.MultiSelect = false;
                ComboPeriod.Set�heckedState(endYear.ToString(), true);


                Collection <string> quarter = new Collection<string>();
                quarter.Add("������� 2");
                quarter.Add("������� 3");
                quarter.Add("������� 4");

                ComboQuarter.Title = "�������";
                ComboQuarter.Width = 200;
                ComboQuarter.FillValues(quarter);
                ComboQuarter.MultiSelect = false;
                ComboQuarter.Set�heckedState(quart,true);
                
            }
            
            Page.Title = "����������� ��������������� ����������� �������� � ����������� ������� �� �� �������� ����������";
            PageTitle.Text = Page.Title;
            gridCaptionElement.Text = string.Empty;

            int year = Convert.ToInt32(ComboPeriod.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex + 2));
            UserParams.PeriodQuater.Value = ComboQuarter.SelectedValue;

            curDate = string.Empty;
            lastDate = string.Empty;

            if (ComboQuarter.SelectedIndex == 0) // ������ ������ �������
            {
                LastQuater.Value = "������� 4";
                LastHalf.Value = "��������� 2";
                UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboPeriod.SelectedValue) - 1).ToString();
                curDate = string.Format("01.07.{0}", ComboPeriod.SelectedValue);
                lastDate = string.Format("01.01.{0}", ComboPeriod.SelectedValue);
            }
            if (ComboQuarter.SelectedIndex == 1) // ������ ������ �������
            {
                LastQuater.Value = "������� 2";
                LastHalf.Value = "��������� 1";
                UserParams.PeriodLastYear.Value = ComboPeriod.SelectedValue;
                curDate = string.Format("01.10.{0}", ComboPeriod.SelectedValue);
                lastDate = string.Format("01.07.{0}", ComboPeriod.SelectedValue);
            }
            if (ComboQuarter.SelectedIndex == 2) // ������ ��������� �������
            {
                LastQuater.Value = "������� 3";
                LastHalf.Value = "��������� 2";
                UserParams.PeriodLastYear.Value = ComboPeriod.SelectedValue;
                curDate = string.Format("01.01.{0}", Convert.ToInt32(ComboPeriod.SelectedValue) + 1);
                lastDate = string.Format("01.10.{0}", ComboPeriod.SelectedValue);
            }

           
            UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n <DATA_VALUE:N0> ���. \n �� <SERIES_LABEL>");
            UltraChart3.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n <DATA_VALUE:N0> ���. - <PERCENT_VALUE:#0.00>%\n {0}", curDate);
            UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n <DATA_VALUE:N0> ���. - <PERCENT_VALUE:#0.00>%\n {0}", lastDate);
           
            chart1ElementCaption.Text = "�������� ����������� ����������� ��������, ���������� ������� ������������ �� ���� ����������� ������� ���������� �������";
            chart2ElementCaption.Text = "�������� ��� ����������� �������� (�� �������) � ����� �������� ������� ������";
            PageSubTitle.Text = string.Format("���������� �� ��������� �� {0} �.",curDate);
           
            RzPR.Value = "[����].[������������_�����14].[Row 1]";
            Admin.Value = "[�������������].[����������].[Row 1]";

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
           
        }

        #region ���������� �����

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            if (subject == "����������� ����������� �������� � ������� ��������������� ������, ���������� ������� ������������ �� ���� ����������� ������� ���������� �������")
            {
                RzPR.Value = "[����].[������������_�����14].[Row 1]";
                Admin.Value = "[�������������].[����������].[Row 1]";
                chart1ElementCaption.Text = "�������� ����������� ����������� ��������, ���������� ������� ������������ �� ���� ����������� ������� ���������� �������";
               
            }
            else if (subject == "����������� ����������� �������� � ������� ��������������� ������, ���������� ������� ������������ �� ���������� ������� �� ���� ��������� �� ������������ �������")
            {
                RzPR.Value = "[����].[������������_�����14].[Row 2]";
                Admin.Value = "[�������������].[����������].[Row 2]";
                chart1ElementCaption.Text = "�������� ����������� ����������� ��������, ���������� ������� ������������ �� ���� ��������� �� ������������ �������";
               
            }
            else if (subject == "������������� ��������")
            {
                RzPR.Value = "[����].[������������_�����14].[Row 3]";
                Admin.Value = " [�������������].[����������].[��� ��������������].[������������� �������� ����������� �������]";
                chart1ElementCaption.Text = "�������� ����������� ������ ������������� ��������";
                
            }
            else if (subject == "�����")
            {
                RzPR.Value = "[����].[������������_�����14].[Row 1]";
                Admin.Value = "[�������������].[����������].[Row 1]";
                chart1ElementCaption.Text = "�������� ����������� ����������� ��������, ���������� ������� ������������ �� ���� ����������� ������� ���������� �������";
               
            }
            
            UltraChart1.DataBind();
        }

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0001_grid_firstRow");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid);

            dtGrid.Rows[0][0] = "����������� ����������� �������� � ������� ��������������� ������, ���������� ������� ������������ �� ���� ����������� ������� ���������� �������";
            dtGrid.Rows[1][0] = "����������� ����������� �������� � ������� ��������������� ������, ���������� ������� ������������ �� ���������� ������� �� ���� ��������� �� ������������ �������";
            dtGrid.Rows[2][0] = "������������� ��������";
            dtGrid.Rows[3][0] = "�����";

            query = DataProvider.GetQueryText("FO_0001_0001_grid_secondRow");
            dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            query = DataProvider.GetQueryText("FO_0001_0001_grid_thirdRow");
            dt1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt1);

            query = DataProvider.GetQueryText("FO_0001_0001_grid_fourthColumn_firstRow");
            dt11 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt11);

            query = DataProvider.GetQueryText("FO_0001_0001_grid_fourthColumn_secondRow");
            dt12 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt12);

            query = DataProvider.GetQueryText("FO_0001_0001_grid_fourthColumn_thirdRow");
            dt13 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt13);

            // ��������� ������ � ������ � ������ ������
            for (int i = 2; i < dt.Columns.Count; i++)
            {
                if (dt.Rows.Count > 0)
                {   CRHelper.SaveToErrorLog(i.ToString());
                    CRHelper.SaveToErrorLog(dt.Rows[0][i].ToString());

                    dtGrid.Rows[1][i-1] = dt.Rows[0][i];
                   
                }
                if (dt1.Rows.Count>0)
                {
                    dtGrid.Rows[2][i-1] = dt1.Rows[0][i];
                }
            }
            
            // ��������� ������ ��� �������� "����������� �������"
            int j = 1;
            for (int i = 13; i < 17; i++ )
            {
                dtGrid.Rows[0][i] = Convert.ToDouble(dt11.Rows[0][j]);
                dtGrid.Rows[1][i] = Convert.ToDouble(dt12.Rows[0][j]);
                dtGrid.Rows[2][i] = Convert.ToDouble(dt13.Rows[0][j]);
                j++;
            }

            for (j = 13; j < 15; j++ )
            {
                dtGrid.Rows[0][j] = Convert.ToDouble(dtGrid.Rows[0][j]) /1000;
                dtGrid.Rows[1][j] = Convert.ToDouble(dtGrid.Rows[1][j]) / 1000;
                dtGrid.Rows[2][j] = Convert.ToDouble(dtGrid.Rows[2][j]) / 1000;
            }

            // ����� �� ��������
                for (int i = 1; i < dtGrid.Columns.Count; i += 4)
                {
                    // ������ �� ���������� �������� ������
                    if (dtGrid.Rows[0][i] != DBNull.Value || dtGrid.Rows[1][i] != DBNull.Value || dtGrid.Rows[2][i] != DBNull.Value)
                    {
                        dtGrid.Rows[3][i] = 0;
                        if (dtGrid.Rows[0][i] != DBNull.Value)
                        {
                            dtGrid.Rows[3][i] = Convert.ToInt32(dtGrid.Rows[3][i]) + Convert.ToInt32(dtGrid.Rows[0][i]);
                        }
                        if (dtGrid.Rows[1][i] != DBNull.Value)
                        {
                            dtGrid.Rows[3][i] = Convert.ToInt32(dtGrid.Rows[3][i]) + Convert.ToInt32(dtGrid.Rows[1][i]);
                        }
                        if (dtGrid.Rows[2][i] != DBNull.Value)
                        {
                            dtGrid.Rows[3][i] = Convert.ToInt32(dtGrid.Rows[3][i]) + Convert.ToInt32(dtGrid.Rows[2][i]);
                        }
                    }

                    // ������ �� ������� ����
                    if (dtGrid.Rows[0][i + 1] != DBNull.Value || dtGrid.Rows[1][i + 1] != DBNull.Value || dtGrid.Rows[2][i + 1] != DBNull.Value)
                    {
                        dtGrid.Rows[3][i + 1] = 0;
                        if (dtGrid.Rows[0][i + 1] != DBNull.Value)
                        {
                            dtGrid.Rows[3][i + 1] = Convert.ToInt32(dtGrid.Rows[3][i + 1]) + Convert.ToInt32(dtGrid.Rows[0][i + 1]);
                        }
                        if (dtGrid.Rows[1][i + 1] != DBNull.Value)
                        {
                            dtGrid.Rows[3][i + 1] = Convert.ToInt32(dtGrid.Rows[3][i + 1]) + Convert.ToInt32(dtGrid.Rows[1][i + 1]);
                        }
                        if (dtGrid.Rows[2][i + 1] != DBNull.Value)
                        {
                            dtGrid.Rows[3][i + 1] = Convert.ToInt32(dtGrid.Rows[3][i + 1]) + Convert.ToInt32(dtGrid.Rows[2][i + 1]);
                        }
                    }
                }
            //����� ���� �����
            for (int i = 3; i < dtGrid.Columns.Count; i+=4)
            {
                if (dtGrid.Rows[3][i - 1] != DBNull.Value && dtGrid.Rows[3][i - 2]!=DBNull.Value)
                {
                  dtGrid.Rows[3][i] = Convert.ToDouble(dtGrid.Rows[3][i-1]) / Convert.ToDouble(dtGrid.Rows[3][i-2]);
                }
            }
           // ���� 
            for (int i = 4; i < dtGrid.Columns.Count; i+=4)
            {
              for (j = 0; j <= 3; j++ )
              {
                  if (dtGrid.Rows[3][i - 2] != DBNull.Value && dtGrid.Rows[j][i - 2]!=DBNull.Value)
                {
                    double curdata = Convert.ToDouble(dtGrid.Rows[3][i - 2]);
                    dtGrid.Rows[j][i] = Convert.ToDouble(dtGrid.Rows[j][i - 2]) / curdata;
                }
              }
            }
           dtGrid.AcceptChanges();
           UltraWebGrid1.DataSource = dtGrid;
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(180);

            headerLayout.AddCell("����������", "", 2);

            GridHeaderCell cell0 = headerLayout.AddCell("���������� ������� ������, ���.");
            cell0.AddCell(string.Format("�� {0}", lastDate), string.Format("���������� ������� ������ �� ���������� � ������� ���������� �� {0} �.", lastDate));
            cell0.AddCell(string.Format("�� {0}", curDate), string.Format("���������� ������� ������ �� ���������� � ������� ���������� �� {0} �.", curDate));
            cell0.AddCell("���� �����", "���� ����� � �������� ��������� �������");
            cell0.AddCell("����", "�������� ��� ����������");

            GridHeaderCell cell1 = headerLayout.AddCell("���������� �������� ������� ������, ���.");
            cell1.AddCell(string.Format("�� {0}", lastDate), string.Format("���������� �������� ������� ������ �� {0} �.", lastDate));
            cell1.AddCell(string.Format("�� {0}", curDate), string.Format("���������� �������� ������� ������ �� {0} �.", curDate));
            cell1.AddCell("���� �����", "���� ����� � �������� ��������� �������");
            cell1.AddCell("����", "�������� ��� ����������");

            GridHeaderCell cell2 = headerLayout.AddCell("��������������� �����������, ���.");
            cell2.AddCell(string.Format("�� {0}", lastDate), string.Format("��������������� ����������� �� {0} �.", lastDate));
            cell2.AddCell(string.Format("�� {0}", curDate), string.Format("��������������� ����������� �� {0} �.", curDate));
            cell2.AddCell("���� �����", "���� ����� � �������� ��������� �������");
            cell2.AddCell("����", "�������� ��� ����������");

            GridHeaderCell cell3 = headerLayout.AddCell("����������� ������� �� �������� ���������� ����������� ��������, ���. ���.");
            cell3.AddCell("2008", "����������� ������� �� �������� ���������� ����������� �������� �� 2008 �.");
            cell3.AddCell("2009", "����������� ������� �� �������� ���������� ����������� �������� �� 2009 �.");
            cell3.AddCell("���� �����", "���� ����� � �������� ��������� �������");
            cell3.AddCell("����", "�������� ��� ����������");

            headerLayout.ApplyHeaderInfo();
         
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80);
                }
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
              for (int i=3; i<e.Row.Cells.Count;i+=4)
              {
                  if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                  {
                      if (100*Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                          e.Row.Cells[i].Title = "���� � �������� ��������� �������";
                      }
                      else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                          e.Row.Cells[i].Title = "�������� � �������� ��������� �������";
                      }

                      e.Row.Cells[i].Style.CustomRules =
                          "background-repeat: no-repeat; background-position: left center; margin: 2px";
                  }
              }
              for (int i = 1; i < e.Row.Cells.Count; i += 4)
              {
                  

                  if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                  {
                      e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("N0");
                  }
                  if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != String.Empty)
                  {
                      e.Row.Cells[i + 1].Value = Convert.ToDouble(e.Row.Cells[i + 1].Value).ToString("N0");
                  }

              }

              for (int i = 3; i < e.Row.Cells.Count; i += 4)
              {
                  if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                  {
                      e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("P2");
                  }
                  if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != String.Empty)
                  {
                      e.Row.Cells[i + 1].Value = Convert.ToDouble(e.Row.Cells[i + 1].Value).ToString("P2");
                  }


              }
              
          }

        #endregion 

        #region ����������� �������� 
         
          protected  void UltraChart1_DataBinding(Object sender, EventArgs e)
          {
              string query = DataProvider.GetQueryText("FO_0001_0001_chart1");
              dtChart1 = new DataTable();
              DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���������1", dtChart1);
              
              dtChart11 = new DataTable();
              query = DataProvider.GetQueryText("FO_0001_0001_chart1");
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

          protected void UltraChart3_DataBinding(Object sender, EventArgs e)
          {
              string query = DataProvider.GetQueryText("FO_0001_0001_chart2");
              dtChart2 = new DataTable();
              DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���������2", dtChart2);

              query = DataProvider.GetQueryText("FO_0001_0001_chart22");
              dt12 = new DataTable();
              DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Chart", dt12);

              dtChart2.Rows[1][1] = dt12.Rows[0][1];
              dtChart2.Rows[1][2] = dt12.Rows[0][2];

              query = DataProvider.GetQueryText("FO_0001_0001_chart23");
              dt13 = new DataTable();
              DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Chart", dt13);

              dtChart2.Rows[2][1] = dt13.Rows[0][1];
              dtChart2.Rows[2][2] = dt13.Rows[0][2];

              dtChart2.Rows[0][0] = "����������� �� (������. ��������)";
              dtChart2.Rows[1][0] = "����������� �� (��������� �� ��)";
              dtChart2.Rows[2][0] = "������������� ��������";

              dtChart2.AcceptChanges();
              UltraChart3.DataSource = dtChart2;
              
          }

        protected void UltraChart2_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0001_chart3");
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���������2", dtChart2);

            query = DataProvider.GetQueryText("FO_0001_0001_chart32");
            dt12 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Chart", dt12);

            dtChart2.Rows[1][1] = dt12.Rows[0][1];
            dtChart2.Rows[1][2] = dt12.Rows[0][2];

            query = DataProvider.GetQueryText("FO_0001_0001_chart33");
            dt13 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Chart", dt13);

            dtChart2.Rows[2][1] = dt13.Rows[0][1];
            dtChart2.Rows[2][2] = dt13.Rows[0][2];

            dtChart2.Rows[0][0] = "����������� �� (������. ��������)";
            dtChart2.Rows[1][0] = "����������� �� (��������� �� ��)";
            dtChart2.Rows[2][0] = "������������� ��������";

            dtChart2.AcceptChanges();
            UltraChart2.DataSource = dtChart2;

        }
        #endregion 

        #region �������

          #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            Worksheet sheet4 = workbook.Worksheets.Add("sheet4");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart2, chart2ElementCaption.Text, sheet3, 3);
            ReportExcelExporter1.Export(UltraChart3, chart2ElementCaption.Text, sheet4, 3);
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
            ISection section3 = report.AddSection();
            ISection section4 = report.AddSection();

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart3.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart2, chart2ElementCaption.Text, section3);
            ReportPDFExporter1.Export(UltraChart3, chart2ElementCaption.Text, section4);

        }


        #endregion

      #endregion


       }
}