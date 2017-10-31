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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0005_Novosib
{
    public partial class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        private int firstYear = 2010;
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;


      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -10);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0005_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
               
                string quarter = dtDate.Rows[0][2].ToString();
                if (quarter != "������� 4")
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0])- 1;
                }
                else
                {
                   endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
                ComboYear.Title = "���";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.Set�heckedState(endYear.ToString(),true);
                
            }

            if (DebtKindButtonList.SelectedIndex == 0) // ���
            {
                Page.Title = "�������������� ���������� ����� ��������������� ����������� �������� � ������� ��������������� ������";
            }
            else  // ����
            {
                Page.Title = "�������������� ���������� ����� ������������� �������� � ������� �������� ��������������";
            }

            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("������ �� {0} �.", ComboYear.SelectedValue);
            
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
          
           
        }

        #region ���������� �����

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            if (DebtKindButtonList.SelectedIndex == 0)
            {
                string query = DataProvider.GetQueryText("FO_0001_0005_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "������������ ���������������� ������",
                                                                                 dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                  dtGrid.AcceptChanges();
                  UltraWebGrid1.DataSource = dtGrid;
                }
            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0001_0005_Grid_MO");
                DataTable dtGrid1 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "������������ �������������� �����������",
                                                                              dtGrid1);
                if (dtGrid1.Rows.Count > 1)
                {
                  dtGrid1.AcceptChanges();
                  UltraWebGrid1.DataSource = dtGrid1;
                }
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
            GridHeaderCell cell0 = headerLayout.AddCell(DebtKindButtonList.SelectedIndex == 0 ? "����������� ������� �� �������� ���������� ����������� ��������, ���. ���." : "����������� ������� �� �������� ���������� ������������� ��������, ���. ���.", DebtKindButtonList.SelectedIndex == 0 ? string.Format("����������� ������� �� �������� ���������� ����������� �������� �� {0} ���", ComboYear.SelectedValue) : string.Format("����������� ������� �� �������� ���������� ������������� �������� �� {0} ���", ComboYear.SelectedValue));
            cell0.AddCell(string.Format("{0}",Convert.ToInt32(ComboYear.SelectedValue)-1));
            cell0.AddCell(string.Format("{0}",ComboYear.SelectedValue));
            cell0.AddCell("����������", "���������� ����������� ������ �� ����������� ��������� �������");
            cell0.AddCell("���� �����, %", "���� ����� ����������� ������ � �������� ��������� �������");

            GridHeaderCell cell1 = headerLayout.AddCell("��������������� �����������, ���.", string.Format("��������������� ����������� �� {0} ���",ComboYear.SelectedValue));
            cell1.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1));
            cell1.AddCell(string.Format("{0}", ComboYear.SelectedValue));
            cell1.AddCell("����������", "���������� ��������������� ����������� �� ����������� ��������� �������");
            cell1.AddCell("���� �����, %", "���� ����� ��������������� ����������� � �������� ��������� �������");

            GridHeaderCell cell2 = headerLayout.AddCell("�������������� �������, ���. ���.", "�������������� ������� �� �������� ���������� 1 ������� ��������������� �����������");
            cell2.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1));
            cell2.AddCell(string.Format("{0}", ComboYear.SelectedValue));
            cell2.AddCell("����������", "���������� �������������� ������ �� ����������� ��������� �������");
            cell2.AddCell("���� �����, %", "���� ����� �������������� ������ � ����������� ��������� �������");
            
            headerLayout.ApplyHeaderInfo();
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(79);
                }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "N2");
            
            for (int i = 4; i < e.Layout.Bands[0].Columns.Count; i += 4)
                {
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
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
            }
              for (int i=1; i<e.Row.Cells.Count;i++)

              {
                  e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                  
                  if ( e.Row.Cells[0].Value.ToString() == "��������������� ������ ��������������� ������" ||
                       e.Row.Cells[0].Value.ToString() == "�������������� ������ ��������������� ������" ||
                       e.Row.Cells[0].Value.ToString() == "����������-������� ������ ������������� �������" ||
                       e.Row.Cells[0].Value.ToString() == "������������� �������� ������������� �������" ||
                       e.Row.Cells[0].Value.ToString() == "�����")
                  {
                      e.Row.Cells[0].Style.Font.Bold = true;
                  }
                  if (i == 4)
                  {
                      if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                              e.Row.Cells[i].Title = "������� ����������� �� ��������� � ���������� ��������";
                          }
                          else if (100*Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                              e.Row.Cells[i].Title = "������� ����������� �� ��������� � ���������� ��������";
                          }

                          e.Row.Cells[i].Style.CustomRules =
                              "background-repeat: no-repeat; background-position: left center; margin: 2px";
                      }
                  }
                  if (i==8)
                  {
                      if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                              e.Row.Cells[i].Title = "��������������� ����������� ����������� �� ��������� � ���������� �������� ";
                          }
                          else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                              e.Row.Cells[i].Title = "��������������� ����������� ����������� �� ��������� � ���������� ��������";
                          }

                          e.Row.Cells[i].Style.CustomRules =
                              "background-repeat: no-repeat; background-position: left center; margin: 2px";
                      }
                  }
                  if (i==12)
                  {
                      if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                              e.Row.Cells[i].Title = "����������� ������� �� �������� ���������� 1 ������� ��������������� ����������� ����������� �� ��������� � ���������� ��������";
                          }
                          else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                              e.Row.Cells[i].Title = "����������� ������� �� �������� ���������� 1 ������� ��������������� ����������� ����������� �� ��������� � ���������� ��������";
                          }

                          e.Row.Cells[i].Style.CustomRules =
                              "background-repeat: no-repeat; background-position: left center; margin: 2px";
                      }
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