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


namespace Krista.FM.Server.Dashboards.reports.UFK_0001_0001
{
    public partial class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        private int firstYear = 2008;
        private int endYear;
        private string month;
        private DateTime data;
      
        private GridHeaderLayout headerLayout;


      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            CrossLink1.Visible = true;
            CrossLink1.Text = "����������&nbsp;��������&nbsp;�����&nbsp;��������&nbsp;�������������&nbsp;�����������";
            CrossLink1.NavigateUrl = "~/reports/UFK_0007_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "����&nbsp;�����&nbsp;�������&nbsp;��������&nbsp;�������������&nbsp;�����������";
            CrossLink2.NavigateUrl = "~/reports/UFK_0007_0002/Default.aspx";
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #region ������������� ����������
          
            
           #endregion

            
        }
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("UFK_0001_0001_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
               
                if (dtDate.Rows.Count > 0)
                {
                   month = dtDate.Rows[0][3].ToString();
                   endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
                ComboYear.Title = "���";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.Set�heckedState(endYear.ToString(),true);

                ComboMonth.Title = "�����";
                ComboMonth.Visible = true;
                ComboMonth.Width = 150;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.MultiSelect = false;
                ComboMonth.Set�heckedState(month,true);
            }
            data =new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex+1, 1);
            data = data.AddMonths(1);
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex +1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex+1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            
            Page.Title = "���������� ���������� ������� �� ������������� ������������";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("���������� ������ �� ����������� ������� � ��������� ������ ����� ������ ���� ������������, ��������������� ��� ����� � ������������� �����������, � ������� ������������� ������� � ��������� �������, �� ���������� ������� ������������ �������� ��������, �� ��������� �� {0:dd.MM.yyyy} ����", data);
            
            headerLayout = new GridHeaderLayout(UltraWebGrid1);

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
        }

        #region ���������� �����

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
           
                string query = DataProvider.GetQueryText("UFK_0001_0001_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "������������� �����������",
                                                                                 dtGrid);
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);

            e.Layout.Bands[0].Columns[UltraWebGrid1.Columns.Count - 1].Hidden = true;

            headerLayout.AddCell("������������� �����������");

            for (int colNum = 1; colNum < UltraWebGrid1.Columns.Count-1; colNum +=5)
            {
                string[] caption = e.Layout.Bands[0].Columns[colNum].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout.AddCell(caption[0]);
                cell.AddCell("���������, ���. ���.", "����������� ���������� ����������� ������ � ������ ����");
                cell.AddCell("��������� �� ����������� ������ �������� ����, ���. ���.", "��������� �� ����������� ������ �������� ����");
                cell.AddCell("���� ����� � �������� ����, %", "���� ����� ���������� � ������������ ������� �������� ����");
                cell.AddCell("���� � ��������� �������, %", "���� ������� ���������� � ����� ������� ���������� �������");
                cell.AddCell("���� � ��������� ������� � ������� ����, %", "���� ������� ���������� � ����� ����� ����������� ������� ���������� ������� � ������� ����");
            }

            headerLayout.ApplyHeaderInfo();
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                }

          for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i += 5)
                {
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+1], "P2");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "P2");
                   
                }
         
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            string level = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex] != null)
            {
                level = e.Row.Cells[levelColumnIndex].ToString();
            }
          
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
               switch (level)
                {
                    case "1":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 9;
                            break;
                        }
                   
                }
            }

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
            }

            for (int i=3; i<e.Row.Cells.Count;i+=5)
             {  
                     if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                       {
                           if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                           {
                               e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                               e.Row.Cells[i].Title = "���� �� ��������� � �������� ����";
                           }
                           else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                           {
                               e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                               e.Row.Cells[i].Title = "���������� �� ��������� � �������� ����";
                           }

                           e.Row.Cells[i].Style.CustomRules =
                               "background-repeat: no-repeat; background-position: left center; margin: 2px";
                       }
                   
               }
          
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
            
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
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