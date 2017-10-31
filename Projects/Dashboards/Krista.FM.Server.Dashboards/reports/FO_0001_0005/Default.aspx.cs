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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0005
{
    public partial class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        private int firstYear = 2007;
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;


      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -10);
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";

            CrossLink1.Visible = true;
            CrossLink1.Text = "�����������&nbsp;���.&nbsp;��������;";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�����������&nbsp;����������&nbsp;���.&nbsp;�������";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0002/Default.aspx";

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

            if (DebtKindButtonList.SelectedIndex == 0)
            {
                Page.Title =
                    "����������� ������� �� �������� ���������� ���, ���������� ��������� ��������������� ����������� ������ � ������� ��������������� ������";
            }
            else
            {
                Page.Title =
                    "����������� ������� �� �������� ���������� ���, ���������� ��������� � ������� �������� ��������������, ������������� �������� ������������� �����������";
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

                int colLastYear = 9;
                int colCurYear = 10;
                
                for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++)
                {
                    if (dtGrid.Rows[indRow][2].ToString() != string.Empty && dtGrid.Rows[indRow][2] != DBNull.Value &&
                        dtGrid.Rows[indRow][6].ToString() != string.Empty && dtGrid.Rows[indRow][6] != DBNull.Value)
                    {
                        dtGrid.Rows[indRow][colCurYear] = Convert.ToDouble(dtGrid.Rows[indRow][2]) /
                                                          Convert.ToDouble(dtGrid.Rows[indRow][6]);

                    }

                    if (dtGrid.Rows[indRow][1].ToString() != string.Empty && dtGrid.Rows[indRow][1] != DBNull.Value &&
                        dtGrid.Rows[indRow][5].ToString() != string.Empty && dtGrid.Rows[indRow][5] != DBNull.Value)
                    {
                        dtGrid.Rows[indRow][colLastYear] = Convert.ToDouble(dtGrid.Rows[indRow][1]) /
                                                           Convert.ToDouble(dtGrid.Rows[indRow][5]);

                    }

                    if (dtGrid.Rows[indRow][colCurYear].ToString() != string.Empty && dtGrid.Rows[indRow][colCurYear] != DBNull.Value)
                    {

                        dtGrid.Rows[indRow][colCurYear + 1] = Convert.ToDouble(dtGrid.Rows[indRow][colCurYear]);
                    }
                    if (dtGrid.Rows[indRow][colLastYear].ToString() != string.Empty && dtGrid.Rows[indRow][colLastYear] != DBNull.Value)
                    {
                        dtGrid.Rows[indRow][colCurYear + 1] = Convert.ToDouble(dtGrid.Rows[indRow][colLastYear]);
                    }

                    if (dtGrid.Rows[indRow][colCurYear].ToString() != string.Empty && dtGrid.Rows[indRow][colCurYear] != DBNull.Value
                       && dtGrid.Rows[indRow][colLastYear].ToString() != string.Empty && dtGrid.Rows[indRow][colLastYear] != DBNull.Value)
                    {
                        dtGrid.Rows[indRow][colCurYear + 1] = Convert.ToDouble(dtGrid.Rows[indRow][colCurYear]) -
                                                              Convert.ToDouble(dtGrid.Rows[indRow][colLastYear]);

                        // dtGrid.Rows[indRow][colCurYear + 2] = Convert.ToDouble(dtGrid.Rows[indRow][colCurYear])/Convert.ToDouble(dtGrid.Rows[indRow][colLastYear]);

                    }


                }

                // ���������� �����
                dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear] = 0;
                if (dtGrid.Rows[dtGrid.Rows.Count - 2][colCurYear].ToString() != string.Empty &&
                    dtGrid.Rows[dtGrid.Rows.Count - 2][colCurYear] != DBNull.Value)
                {

                    dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear] = Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 2][colCurYear]) +
                                                                     Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear]);
                }
                if (dtGrid.Rows[dtGrid.Rows.Count - 5][colCurYear].ToString() != string.Empty &&
                    dtGrid.Rows[dtGrid.Rows.Count - 5][colCurYear] != DBNull.Value)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear] = Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 5][colCurYear]) +
                                                                     Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear]);
                }
                if (Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear]) == 0)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear] = DBNull.Value;
                }

                dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear] = 0;
                if (dtGrid.Rows[dtGrid.Rows.Count - 2][colLastYear].ToString() != string.Empty &&
                    dtGrid.Rows[dtGrid.Rows.Count - 2][colLastYear] != DBNull.Value)
                {

                    dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear] = Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear]) +
                                                                      Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 2][colLastYear]);
                }

                if (dtGrid.Rows[dtGrid.Rows.Count - 5][colLastYear].ToString() != string.Empty &&
                   dtGrid.Rows[dtGrid.Rows.Count - 5][colLastYear] != DBNull.Value)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear] = Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear]) +
                                                                      Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 5][colLastYear]);
                }

                if (Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear]) == 0)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear] = DBNull.Value;
                }


                // ���������� �����_����������

                if (dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear].ToString() != string.Empty && dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear] != DBNull.Value)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear + 1] =
                      Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear]);
                }
                if (dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear].ToString() != string.Empty && dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear] != DBNull.Value)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear + 1] =
                        Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear]);
                }
                if (dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear].ToString() != string.Empty && dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear] != DBNull.Value && dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear].ToString() != string.Empty && dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear] != DBNull.Value)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear + 1] = Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colCurYear]) - Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][colLastYear]);
                }
                // ���� �����
                for (int indCol = 4; indCol < dtGrid.Columns.Count; indCol += 4)
                {
                    for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++)
                    {

                        if (dtGrid.Rows[indRow][indCol - 2].ToString() != string.Empty &&
                            dtGrid.Rows[indRow][indCol - 2] != DBNull.Value &&
                            dtGrid.Rows[indRow][indCol - 3].ToString() != string.Empty &&
                            dtGrid.Rows[indRow][indCol - 3] != DBNull.Value)
                        {
                            dtGrid.Rows[indRow][indCol] = Convert.ToDouble(dtGrid.Rows[indRow][indCol - 2]) /
                                                          Convert.ToDouble(dtGrid.Rows[indRow][indCol - 3]);
                        }

                    }
                }

                dtGrid.AcceptChanges();
                UltraWebGrid1.DataSource = dtGrid;
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
                    for (int indRow = 0; indRow < dtGrid1.Rows.Count; indRow++)
                    {
                        dtGrid1.Rows[indRow][0] = dtGrid1.Rows[indRow][0].ToString().Replace("������������� �����", "��");
                        dtGrid1.Rows[indRow][0] = dtGrid1.Rows[indRow][0].ToString().Replace("������������� �����", "��");
                    }
                    int colNumLast = 9;
                    int colNumCur = 10;
                    double sumLast = 0;
                    double sumCur = 0;
                    for (int i = 0; i < dtGrid1.Rows.Count-1; i++)
                    {
                        if (dtGrid1.Rows[i][colNumLast] != DBNull.Value && dtGrid1.Rows[i][colNumLast].ToString() != string.Empty)
                        {
                            sumLast = Convert.ToDouble(sumLast) + Convert.ToDouble(dtGrid1.Rows[i][colNumLast]);
                        }
                        if (dtGrid1.Rows[i][colNumCur] != DBNull.Value && dtGrid1.Rows[i][colNumCur].ToString() != string.Empty)
                        {
                            sumCur = Convert.ToDouble(sumCur) + Convert.ToDouble(dtGrid1.Rows[i][colNumCur]);
                        }

                    }

                    dtGrid1.Rows[dtGrid1.Rows.Count - 1][colNumLast] = sumLast;
                    dtGrid1.Rows[dtGrid1.Rows.Count - 1][colNumCur] = sumCur;
                    dtGrid1.Rows[dtGrid1.Rows.Count - 1][colNumCur + 1] = sumCur - sumLast;
                    dtGrid1.Rows[dtGrid1.Rows.Count - 1][colNumCur + 2] = sumCur / sumLast;

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
            GridHeaderCell cell0 = headerLayout.AddCell("����������� ������� �� �������� ���������� ����������� ��������, ���. ���.", string.Format("����������� ������� �� �������� ���������� ����������� �������� �� {0} ���",ComboYear.SelectedValue));
            cell0.AddCell(string.Format("{0}",Convert.ToInt32(ComboYear.SelectedValue)-1));
            cell0.AddCell(string.Format("{0}",ComboYear.SelectedValue));
            cell0.AddCell("����������", "���������� ����������� ������ �� ����������� ��������� �������");
            cell0.AddCell("���� �����, %", "���� ����� ����������� ������ � �������� ��������� �������");

            GridHeaderCell cell1 = headerLayout.AddCell("��������������� �����������, ���.", string.Format("��������������� ����������� �� {0} ���",ComboYear.SelectedValue));
            cell1.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1));
            cell1.AddCell(string.Format("{0}", ComboYear.SelectedValue));
            cell1.AddCell("����������", "���������� ��������������� ����������� �� ����������� ��������� �������");
            cell1.AddCell("���� �����, %", "���� ����� ��������������� ����������� � �������� ��������� �������");

            GridHeaderCell cell2 = headerLayout.AddCell("����������� �������, ���. ���.", "����������� ������� �� �������� ���������� 1 ������� ��������������� �����������");
            cell2.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1));
            cell2.AddCell(string.Format("{0}", ComboYear.SelectedValue));
            cell2.AddCell("����������", "���������� ����������� ������ �� ����������� ��������� �������");
            cell2.AddCell("���� �����, %", "���� ����� ����������� ������ � ����������� ��������� �������");
            

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
                  e.Row.Cells[0].Style.Font.Size = 10;

                  
                  if (e.Row.Cells[0].Value.ToString() == "�� ���� ������� ���������� �������" || e.Row.Cells[0].Value.ToString() == "�� ���� ��������� �� ������������ �������")
                  {
                      e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
                      e.Row.Cells[0].Style.Font.Italic = true;
                  }
                  if (e.Row.Cells[0].Value.ToString() == "����� ������� ���������� (��� ������� ��������������� ������), � ��� �����" || e.Row.Cells[0].Value.ToString() == "����� ������� ���������� (������� ������ �������������� �����), � ��� �����" || e.Row.Cells[0].Value.ToString() == "����� ������� ���������� (��� ������������� ��������), � ��� �����" || e.Row.Cells[0].Value.ToString() == "����� ������� ����������")
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