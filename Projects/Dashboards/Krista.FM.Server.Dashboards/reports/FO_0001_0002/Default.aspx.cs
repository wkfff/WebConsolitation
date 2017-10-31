using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebChart;
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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0002
{
    public partial class Default: CustomReportPage
    {
      #region ����

        private DataTable dtGrid;
        private DataTable dtChart;

        private int firstYear = 2007;
        private int endYear;

        private GridHeaderLayout headerLayout;

        public CustomParam LastHalf;
        public CustomParam LastQuater;
        public CustomParam Measures;
        
        public string curDate;
        public string lastDate;

      #endregion

      protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/1.1);
            
            #region ������������� ����������
             if (LastHalf ==null)
             {
                 LastHalf = UserParams.CustomParam("lastHalf");
             }

             if (LastQuater == null)
             {
                 LastQuater = UserParams.CustomParam("lastQuater");
             }

             if (Measures == null)
             {
                Measures = UserParams.CustomParam("measures");
             }
            
            #endregion

            #region  ��������� ��������
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);

            UltraChart1.ChartType = ChartType.BarChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X2.Visible = true;
            UltraChart1.Axis.X2.Extent = 30;
            UltraChart1.Axis.X.Extent = 40 ;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X2.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
            UltraChart1.Axis.Y.Extent = 350;
         
            UltraChart1.Data.SwapRowsAndColumns = true;
            

            UltraChart1.TitleBottom.Visible = true;
            UltraChart1.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleBottom.Text = "�������";

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 5;
            UltraChart1.Legend.Font = font;

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
         
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                GradientStyle peStyle = GradientStyle.ForwardDiagonal;

                switch (i)
                {
                    case 2:
                        {
                            color = Color.Lime;
                            stopColor = Color.LimeGreen;
                            peType = PaintElementType.Gradient;
                            peStyle = GradientStyle.ForwardDiagonal;
                            pe.FillOpacity = 150;
                            pe.FillStopOpacity = 250;
                            break;
                        }
                    case 1:
                        {
                            color = Color.LimeGreen;
                            stopColor = Color.ForestGreen;
                            peType = PaintElementType.Gradient;
                            peStyle = GradientStyle.ForwardDiagonal;
                            pe.FillOpacity = 250;
                            pe.FillStopOpacity = 250;
                            break;
                        }
                   

                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = peStyle;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
         #endregion

            CrossLink1.Visible = true;
            CrossLink1.Text = "�����������&nbsp;���.&nbsp;��������";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�����������&nbsp;�������&nbsp;��&nbsp;����������&nbsp;���.&nbsp;��������";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0005/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "�����������&nbsp;�������&nbsp;��&nbsp;����������&nbsp;���.&nbsp;��������";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0007/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "������������&nbsp;�������&nbsp;�����������&nbsp;���.&nbsp;��������";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0006/Default.aspx";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0002_Data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string quart = dtDate.Rows[0][2].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                
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
                ComboQuarter.Set�heckedState(quart, true);
                
            }

            Page.Title = "����������� ���������� ��������������� ������� ";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;
            gridCaptionElement.Text = string.Empty;

            if (DebtKindButtonList2.SelectedIndex == 0)
             {
                 Measures.Value = "[���������� �� ����]";
                 chart1ElementCaption.Text = "������������ ����������� ���������� ��������������� ������� � ��������� � ������� �������� ��������";
                 UltraChart1.Tooltips.FormatString = "���������� ������� ������ \n <DATA_VALUE:N0> ���. \n  �� ��������� <ITEM_LABEL> �. ";
             }
            else
             {
                 Measures.Value = "[���������� �� ����]";
                 chart1ElementCaption.Text = "����������� ����������� ���������� ��������������� ������� � ��������� � ������� �������� ��������";
                 UltraChart1.Tooltips.FormatString = "���������� �������� ������� ������ \n <DATA_VALUE:N0> ���. \n  �� ��������� <ITEM_LABEL> �.";
             }

             int year = Convert.ToInt32(ComboPeriod.SelectedValue);
             UserParams.PeriodYear.Value = year.ToString();
             UserParams.PeriodHalfYear.Value = string.Format("��������� {0}",CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex + 2));
             UserParams.PeriodQuater.Value = ComboQuarter.SelectedValue;

             curDate = string.Empty;
             lastDate = string.Empty;

             if (ComboQuarter.SelectedIndex == 0 ) // ������ ������ �������
             {
                 LastQuater.Value = "������� 4";
                 LastHalf.Value = "��������� 2";
                 UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboPeriod.SelectedValue)-1).ToString();
                 curDate = string.Format("01.07.{0}",ComboPeriod.SelectedValue);
                 lastDate = string.Format("01.01.{0}",ComboPeriod.SelectedValue);
             }
             if (ComboQuarter.SelectedIndex == 1 ) // ������ ������ �������
             {
                 LastQuater.Value = "������� 2";
                 LastHalf.Value = "��������� 1";
                 UserParams.PeriodLastYear.Value = ComboPeriod.SelectedValue;
                 curDate = string.Format("01.10.{0}", ComboPeriod.SelectedValue);
                 lastDate = string.Format("01.07.{0}", ComboPeriod.SelectedValue);
             }
             if (ComboQuarter.SelectedIndex == 2 ) // ������ ��������� �������
             {
                 LastQuater.Value = "������� 3";
                 LastHalf.Value = "��������� 2";
                 UserParams.PeriodLastYear.Value = ComboPeriod.SelectedValue;
                 curDate = string.Format("01.01.{0}", Convert.ToInt32(ComboPeriod.SelectedValue)+1);
                 lastDate = string.Format("01.10.{0}", ComboPeriod.SelectedValue);
             }
           
            PageSubTitle.Text = string.Format("���������� �� ��������� �� {0} �.",curDate);
            
           
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
       }

        #region ���������� �����

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0002_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"����������",dtGrid);

            for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++)
            {
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("����������� �������", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("���������������_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("�����������_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("����_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("����_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("������������_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("���_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("�������_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("��������_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("���_", "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("������ �������������� �����_",
                                                                                   "");
                dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace(
                    "��� ������� ��������������� ������_", "");
                dtGrid.Rows[indRow][0] =
                    dtGrid.Rows[indRow][0].ToString().Replace("����� ���� ������������� ��������",
                                                              string.Format(
                                                                  "����� ���� <br/> ������������� ��������"));
            }

            UltraWebGrid1.DataSource = dtGrid;
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(190);


            headerLayout.AddCell("������������ ���������������� ������", "", 2);

            GridHeaderCell cell0 = headerLayout.AddCell(string.Format("���������� ������� ������ �� {0} �., ���.", lastDate));
            cell0.AddCell("�����", string.Format("������������ ����������� ���������� ���. ������ � ������� ���������� �� {0} �.", lastDate));
            cell0.AddCell("� ��� ����� ���. ����������� ������", string.Format("������������ ����������� ���������� ��������������� ����������� ������ � ������� ���������� �� {0} �.",lastDate));
            GridHeaderCell cell1 = headerLayout.AddCell(string.Format("���������� ������� ������ �� {0} �., ���.", curDate));
            cell1.AddCell("�����", string.Format("������������ ����������� ���������� ���. ������ � ������� ���������� �� {0} �.", curDate));
            cell1.AddCell("� ��� ����� ���. ����������� ������", string.Format("������������ ����������� ���������� ��������������� ����������� ������ � ������� ���������� �� {0} �.", curDate));
            GridHeaderCell cell2= headerLayout.AddCell("���� �����");
            cell2.AddCell("�����", "���� ����� ������������ ����������� ���������� ���. ������ � �������� ��������� ������� ");
            cell2.AddCell("� ��� ����� ���. ����������� ������", "���� ����� ������������ ����������� ���������� ��������������� ����������� ������ � �������� ��������� �������");

            GridHeaderCell cell3 = headerLayout.AddCell(string.Format("���������� �������� ������ �� {0} �., ���", lastDate));
            cell3.AddCell("�����", string.Format("����������� ����������� ���������� ���. ������ �� {0} �.", lastDate));
            cell3.AddCell("� ��� ����� ���. ����������� ������", string.Format("����������� ����������� ���������� ��������������� ����������� ������ �� {0} �.", lastDate));
            GridHeaderCell cell4 = headerLayout.AddCell(string.Format("���������� �������� ������ �� {0} �., ���", curDate));
            cell4.AddCell("�����", string.Format("����������� ����������� ���������� ���. ������ �� {0} �.", curDate));
            cell4.AddCell("� ��� ����� ���. ����������� ������", string.Format("����������� ����������� ���������� ��������������� ����������� ������ �� {0} �.", curDate));
            GridHeaderCell cell5 = headerLayout.AddCell("���� �����");
            cell5.AddCell("�����", "���� ����� ����������� ����������� ���������� ���. ������ � �������� ��������� ������� ");
            cell5.AddCell("� ��� ����� ���. ����������� ������", "���� ����� ����������� ����������� ���������� ��������������� ����������� ������ � �������� ��������� �������");

            headerLayout.ApplyHeaderInfo();
            

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                   e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                   e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80);
                }
       
            for (int i = 5; i < e.Layout.Bands[0].Columns.Count; i += 6)
                {
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                }
            
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
            e.Row.Cells[0].Style.Font.Size = 10;


            if (e.Row.Cells[0].Value.ToString() == "�� ���� ������� ���������� �������" || e.Row.Cells[0].Value.ToString() == "�� ���� ��������� �� ������������ �������")
            {
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[0].Style.Font.Italic = true;
                e.Row.Cells[0].Style.Padding.Right = 10;
            }
            if (e.Row.Cells[0].Value.ToString() == "����� ������� ���������� (��� ������� ��������������� ������), � ��� �����" || e.Row.Cells[0].Value.ToString() == "����� ������� ���������� (������� ������ �������������� �����), � ��� �����" || e.Row.Cells[0].Value.ToString() == "����� ������� ���������� (��� ������������� ��������), � ��� �����" || e.Row.Cells[0].Value.ToString() == "����� ������� ����������")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            
            for (int i=1; i<e.Row.Cells.Count;i++)
              {
                  if (i == 5 || i == 6 || i == 11 || i == 12)
                  {
                      if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
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
              }

             
              
          }

        #endregion 

        #region ����������� �������� 
          /// <summary>
          /// ����������� ���������� ����� � �������
          /// </summary>
          /// <param name="dt">������� �������</param>
          /// <returns>�������� �������</returns>
          private static DataTable ReverseRowsDataTable(DataTable dt)
          {
              DataTable resDt = new DataTable();

              for (int i = 0; i < dt.Columns.Count; i++)
              {
                  DataColumn column = new DataColumn(dt.Columns[i].Caption, dt.Columns[i].DataType);
                  resDt.Columns.Add(column);
              }

              for (int i = dt.Rows.Count; i > 0; i--)
              {
                  DataRow row = resDt.NewRow();
                  row.ItemArray = dt.Rows[i - 1].ItemArray;
                  resDt.Rows.Add(row);
              }

              return resDt;
          }

          protected  void UltraChart1_DataBinding(Object sender, EventArgs e)
          {
              string query = DataProvider.GetQueryText("FO_0001_0002_Chart");
              dtChart = new DataTable();
              DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���������1" , dtChart);

              for (int i = 0; i < dtChart.Rows.Count; i++)
              {
                  dtChart.Rows[i][0] = dtChart.Rows[i][0].ToString().Replace("����������� �������", "");
              }

              dtChart = ReverseRowsDataTable(dtChart);
              NumericSeries series1 = CRHelper.GetNumericSeries(2, dtChart);
              series1.Label = string.Format("�� {0}", curDate); 
              UltraChart1.Series.Add(series1);

              NumericSeries series2 = CRHelper.GetNumericSeries(1, dtChart);
              series2.Label = string.Format("�� {0}", lastDate); 
              UltraChart1.Series.Add(series2);
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
           
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 3);
            
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
            
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
           
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section2);
           

        }


        #endregion

      #endregion


       }
}