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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0007_XMAO
{
    public partial class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        private int firstYear = 2008;
        private int endYear;
        private string endQuarter;
        private string curDate;
        private string lastDate;

        private CustomParam LastHalf;
        private CustomParam LastQuarter;
        private CustomParam numMonth;

        private GridHeaderLayout headerLayout;


      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            CrossLink1.Visible = true;
            CrossLink1.Text = "�����������&nbsp;����������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0003/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�����������&nbsp;�������&nbsp;��&nbsp;��������&nbsp;����������&nbsp;���&nbsp;�&nbsp;���� ";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0005_XMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "�������&nbsp;�����������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0006_XMAO/Default.aspx";
            
            CrossLink4.Visible = true;
            CrossLink4.Text = "������&nbsp;��������&nbsp;��&nbsp;���&nbsp;�&nbsp;���� ";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0003_XMAO/Default.aspx";
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #region ������������� ����������
          
             if (LastHalf == null)
              {
                LastHalf = UserParams.CustomParam("lastHalf");
              }

             if (LastQuarter == null)
              {
                LastQuarter = UserParams.CustomParam("lastQuater");
              }
            
              numMonth = UserParams.CustomParam("num_month");

            #endregion
        }
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0007_XMAO_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
               
                if (dtDate.Rows.Count > 0)
                {
                   endQuarter = dtDate.Rows[0][2].ToString();
                   endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
                ComboYear.Title = "���";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.Set�heckedState(endYear.ToString(),true);

                Dictionary <string,int> quarter = new Dictionary<string, int>();
                quarter.Add("������� 2",0);
                quarter.Add("������� 3", 0);
                quarter.Add("������� 4", 0);

                ComboQuarter.Title = "�������";
                ComboQuarter.Visible = true;
                ComboQuarter.Width = 180;
                ComboQuarter.FillDictionaryValues(quarter);
                ComboQuarter.MultiSelect = false;
                ComboQuarter.Set�heckedState(endQuarter,true);
                
            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex + 2));
            UserParams.PeriodQuater.Value = ComboQuarter.SelectedValue;
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();

            curDate = string.Empty;
            lastDate = string.Empty;
            numMonth.Value = string.Empty;
            int number = 6;

            if (ComboQuarter.SelectedIndex == 0) // ������ ������ �������
            {
                
                curDate = string.Format(" �� 01.07.{0}", ComboYear.SelectedValue);
                lastDate = string.Format(" �� 01.07.{0}", Convert.ToInt32(ComboYear.SelectedValue)-1);
                number = 6;
            }

            if (ComboQuarter.SelectedIndex == 1) // ������ ������ �������
            {
               
                curDate = string.Format("�� 01.10.{0}", ComboYear.SelectedValue);
                lastDate = string.Format("�� 01.10.{0}", Convert.ToInt32(ComboYear.SelectedValue)-1);
                number = 9;
            }

            if (ComboQuarter.SelectedIndex == 2) // ������ ��������� �������
            {
                curDate = string.Format("�� {0}", ComboYear.SelectedValue);
                lastDate = string.Format("�� {0}", Convert.ToInt32(ComboYear.SelectedValue) - 1);
                number = 12;
            }

            Page.Title = "������ �������������� ���������� ����� ����������� � ������������� �������� ������� ��������������� ������, ������� �������� �������������� � ������������� �������� �� �����-����������� ����������� ������-����";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format(" {0} �� ��������� {1} �.", DebtKindButtonList.SelectedValue, curDate);

            CRHelper.SaveToErrorLog(number.ToString());

            numMonth.Value = number.ToString();

            CRHelper.SaveToErrorLog(numMonth.Value);

            headerLayout = new GridHeaderLayout(UltraWebGrid1);

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
        }

        #region ���������� �����

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            if (DebtKindButtonList.SelectedIndex == 0)
            {
                string query = DataProvider.GetQueryText("FO_0001_0007_XMAO_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "������������ ���������������� ������",
                                                                                 dtGrid);
                dtGrid.AcceptChanges();
                UltraWebGrid1.DataSource = dtGrid;
            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0001_0007_XMAO_GridMO");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "������������ �������������� �����������",
                                                                                 dtGrid);
                dtGrid.AcceptChanges();
                UltraWebGrid1.DataSource = dtGrid;
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);

            if (DebtKindButtonList.SelectedIndex == 0)
            {
                headerLayout.AddCell("������������ ���������������� ������", "", 2);
            }
            else
            {
                headerLayout.AddCell("������������ �������������� �����������", "", 2);
            }

            GridHeaderCell cell0 = headerLayout.AddCell("�������������� ���������� �����, ���.���.");
            if (DebtKindButtonList.SelectedIndex == 0)
            {
                GridHeaderCell cell1 = cell0.AddCell("�����");
                cell1.AddCell(string.Format("{0} �.", lastDate),string.Format("�������������� ���������� ����� �� ���� ���������� {0} �.", lastDate));
                cell1.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� �� ���� ���������� {0} �.", curDate));
                cell1.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell2 = cell0.AddCell("��������������� ���������");
                cell2.AddCell(string.Format("{0} �.",lastDate), string.Format("�������������� ���������� ����� ��������������� �������� {0} �.", lastDate));
                cell2.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� ��������������� �������� {0} �.",curDate));
                cell2.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell3 = cell0.AddCell("��������������� ����������� ��������");
                cell3.AddCell(string.Format("{0} �.", lastDate),string.Format("�������������� ���������� ����� ��������������� ����������� �������� {0} �.",lastDate));
                cell3.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� ��������������� ����������� �������� {0} �.",curDate));
                cell3.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell4 = cell0.AddCell("����������� ��������");
                cell4.AddCell(string.Format("{0} �.", lastDate),string.Format("�������������� ���������� ����� ������������ ��������� {0} �.", lastDate));
                cell4.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� ������������ ��������� {0} �.", curDate));
                cell4.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell5 = cell0.AddCell("�������");
                cell5.AddCell(string.Format("{0} �.", lastDate),string.Format("�������������� ���������� ����� ������� {0} �.",lastDate));
                cell5.AddCell(string.Format("{0} �.", curDate), string.Format("�������������� ���������� ����� ������� {0} �.", curDate));
                cell5.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");
            }
            else
            {
                GridHeaderCell cell1 = cell0.AddCell("�����");
                cell1.AddCell(string.Format("{0} �.", lastDate),string.Format("�������������� ���������� ����� �� ���� ���������� {0} �.",lastDate));
                cell1.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� �� ���� ���������� {0} �.",curDate));
                cell1.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell2 = cell0.AddCell("������������� ���������");
                cell2.AddCell(string.Format("{0} �.", lastDate), string.Format("�������������� ���������� ����� ������������� ��������  � {0} ����", lastDate));
                cell2.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� ������������� ��������  � {0} ����", curDate));
                cell2.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell3 = cell0.AddCell("��������� ������������� ������");
                cell3.AddCell(string.Format("{0} �.", lastDate),string.Format("�������������� ���������� ����� ���������� ������������� ������ {0} �.",lastDate));
                cell3.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� ���������� ������������� ������ {0} �.",curDate));
                cell3.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell4 = cell0.AddCell("����������� ��������");
                cell4.AddCell(string.Format("{0} �.", lastDate), string.Format("�������������� ���������� ����� ������������ ��������� {0} �.", lastDate));
                cell4.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� ������������ ��������� {0} �.",ComboYear.SelectedValue));
                cell4.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");

                GridHeaderCell cell5 = cell0.AddCell("�������");
                cell5.AddCell(string.Format("{0} �.", lastDate), string.Format("�������������� ���������� ����� ������� {0} �.",lastDate));
                cell5.AddCell(string.Format("{0} �.", curDate),string.Format("�������������� ���������� ����� ������� {0} �.",curDate));
                cell5.AddCell("� % � �����.������� ����.����", "�������/�������� ������������ �������� ��������� ����");
                
            }
            headerLayout.ApplyHeaderInfo();
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(70);
                }

           for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i += 3)
                {
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P3");
                   e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
                }
           
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
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

                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;

                if (e.Row.Cells[0].Value.ToString() == "�����" || e.Row.Cells[0].Value.ToString() == "������� ��������" || e.Row.Cells[0].Value.ToString() == "C������ ��������")
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                }
            }
            for (int i=3; i<e.Row.Cells.Count;i+=3)
             {  
                     if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                       {
                           if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                           {
                               e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyDownBB.png";
                               e.Row.Cells[i].Title = "�������������� ������ ����� ����������� � �������� ����";
                           }
                           else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                           {
                               e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyUpBB.png";
                               e.Row.Cells[i].Title = "�������������� ������ ����� ����������� � �������� ����";
                           }

                           e.Row.Cells[i].Style.CustomRules =
                               "background-repeat: no-repeat; background-position: left center; margin: 2px";
                       }
                   
               }
        }

        #endregion 
  

        #region �������

      
        #region ������� � Excel

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 50;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }
       
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 5; j < rowsCount+6; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Font.Height = 200;
                }
            }
        }


        #endregion

        #region ������� � Pdf

        
        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
           
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
         

        }

        #endregion

      #endregion


       }
}