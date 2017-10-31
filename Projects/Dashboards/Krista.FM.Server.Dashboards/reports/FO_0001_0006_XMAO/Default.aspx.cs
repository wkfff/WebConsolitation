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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0006_XMAO
{
    public partial class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        private int firstYear = 2008;
       
        private GridHeaderLayout headerLayout;

      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -10);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight /1.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            CrossLink1.Visible = true;
            CrossLink1.Text = "�����������&nbsp;����������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0003/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�����������&nbsp;�������&nbsp;��&nbsp;��������&nbsp;����������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0005_XMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "������&nbsp;��������������&nbsp;��������&nbsp;���&nbsp;�&nbsp;����";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0007_XMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "������&nbsp;��������&nbsp;��&nbsp;���&nbsp;�&nbsp;����";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0003_XMAO/Default.aspx";

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
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0006_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear; 

                if (dtDate.Rows[0][2].ToString() == "������� 4")
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
                else
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0])-1;
                }

                ComboYear.Title = "���";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.Set�heckedState(endYear.ToString(),true);
                
            }
            if (DebtKindButtonList.SelectedIndex == 0)
            {
                Page.Title =
                    "������������ ������� ����������� ��������������� ����������� �������� � ������� ��������� � ����� ���������� �����-����������� ����������� ������-����";
            }
            else
            {
                Page.Title =
                    "������������ ������� ����������� ����������, ���������� ��������� ������������� ������ �����-����������� ����������� ������-����";
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
                string query = DataProvider.GetQueryText("FO_0001_0006_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ���������������� ������",
                                                                                 dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                   UltraWebGrid1.DataSource = dtGrid;
                }

            }
            else
            {
                string query = DataProvider.GetQueryText("FO_0001_0006_Grid_MO");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �������������� �����������",
                                                                                 dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                    for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++)
                    {
                        dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("�����-����������� ����������� ������ � ����", "");

                    }

                    dtGrid.AcceptChanges();
                    UltraWebGrid1.DataSource = dtGrid;
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Width = CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.Padding.Right = 10;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(60);
            }

            string[] caption;
            if (DebtKindButtonList.SelectedIndex == 0)
            {
                headerLayout.AddCell("������������ ���������������� ������", "");
                GridHeaderCell cell0 = headerLayout.AddCell("����������� ����������, ���������� ��������� ��������������� ����������� ��������, �� ��� �� ����������");
                GridHeaderCell cell2_1 = cell0.AddCell("������������");
                cell2_1.AddCell("�����");
                GridHeaderCell cell3_1 = cell2_1.AddCell("� ��� ����� �� �������");
                cell3_1.AddCell("������");
                cell3_1.AddCell("�������");
                cell3_1.AddCell("�������");
                GridHeaderCell cell2_2  = cell0.AddCell("��������� (���������)");
                cell2_2.AddCell("�����");
                GridHeaderCell cell3_2 = cell2_2.AddCell("� ��� ����� �� �������");
                cell3_2.AddCell("������");
                cell3_2.AddCell("�������");
                cell3_2.AddCell("�������");
                GridHeaderCell cell2_3 = cell0.AddCell("�����������");
                cell2_3.AddCell("�����");
                GridHeaderCell cell3_3 = cell2_3.AddCell("� ��� ����� �� �������");
                cell3_3.AddCell("������");
                cell3_3.AddCell("�������");
                cell3_3.AddCell("�������");
                cell3_3.AddCell("�������");
                GridHeaderCell cell2_4 = cell0.AddCell("�������������� �����������");
                cell2_4.AddCell("�����");
                GridHeaderCell cell3_4 = cell2_4.AddCell("� ��� ����� �� �������");
                cell3_4.AddCell("�������");
                cell3_4.AddCell("�������");
                cell3_4.AddCell("�������");
                cell3_4.AddCell("�������");
                headerLayout.AddCell("������������ ������� �����������", "");
              
            }
            else
            {
                headerLayout.AddCell("������������ �������������� �����������", "");

                GridHeaderCell cell0 = headerLayout.AddCell("����������� ����������, ���������� ��������� ������������� ������, �� ��� �� ������� ����������");
                cell0.AddCell("������");
                cell0.AddCell("�������");
                cell0.AddCell("�������");
                cell0.AddCell("�������");
                cell0.AddCell("�������");
                headerLayout.AddCell("������������ ������� �����������", "");
                
               

            }
          
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
           
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


            for (int i = 1; i < e.Row.Cells.Count; i++)
            {

                if (e.Row.Cells[0].Value.ToString() == "�����")
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                }
                if (Convert.ToInt32(e.Row.Cells[i].Value) == 0)
                {
                    e.Row.Cells[i].Value = "";
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
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
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
            
            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 5; j < rowsCount + 7; j++)
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
           
            ReportPDFExporter1.HeaderCellHeight = 30;
            ReportPDFExporter1.Export(headerLayout, section1);
         

        }

        #endregion

      #endregion


       }
}