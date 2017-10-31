using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0039
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private DateTime curDate;

        private GridHeaderLayout headerLayout1;

        #endregion

        #region ��������� �������

        private CustomParam measuresTempRosta;
        private CustomParam multiplierRub;
        private CustomParam measures;

        #endregion

        private double rubMultiplier;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "���.���." : "���.���."; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.6);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";

            #region ������������� ���������� �������

            measuresTempRosta = UserParams.CustomParam("temp_rosta");
            multiplierRub = UserParams.CustomParam("rub_multiplier");
            measures = UserParams.CustomParam("measures");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0039_date");
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
            }

            Page.Title = "������� �� ���������� �������� ������� � �������";
            PageTitle.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            curDate = ComboMonth.SelectedValue == "�������" ? new DateTime(yearNum + 1, 1, 1) : new DateTime(yearNum , monthNum + 1, 1);

            PageSubTitle.Text = string.Format("������ �� ��������� �� {1:dd.MM.yyyy} �., {0}", IsThsRubSelected ? "���.���." : "���. ���.", curDate);

            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString(); 
            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            multiplierRub.Value = rubMultiplier.ToString();

            if (RadioButtonList1.SelectedIndex != 0)
            {
                Check1.Visible = false;
                Check2.Visible = false;
            }
            else
            {
                Check1.Visible = true;
                Check2.Visible = true;
            }

            measuresTempRosta.Value = Check2.Checked ? ",Measures.[���� ����� ]" : string.Empty;
            measures.Value = Check1.Checked ? "Measures.[���.�� ������],  Measures.[���.��] , Measures.[���� ] , Measures.[% ����������], Measures.[% ��� ���]" : "Measures.[���.��], Measures.[���� ], Measures.[% ����������] ";
            headerLayout1 = new GridHeaderLayout(UltraWebGrid);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            int num = RadioButtonList1.SelectedIndex;

            if (num == 0)
            {
                string query = DataProvider.GetQueryText("FO_0002_0039_grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������", dtGrid);
            }
            if (num == 1)
            {
                string query = DataProvider.GetQueryText("FO_0002_0039_grid_Dohod");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������", dtGrid);
            }
            
            if (num == 2)
            {
                string query = DataProvider.GetQueryText("FO_0002_0039_grid_Rashod");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������", dtGrid);
            }
            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0].ToString().Contains("������ ���������� ����"))
                    {
                        row[0] = row[0].ToString().Replace("������ ���������� ����", "����� �� �������");
                    }

                    if (row[0].ToString().Contains("��������� ������ ���������� ����"))
                    {
                        row[0] = row[0].ToString().Replace("��������� ������ ���������� ����", "����� �� �������");
                    }

                    if (row[0].ToString().Contains("��������� ����"))
                    {
                        row[0] = row[0].ToString().Replace("��������� ����", "����� �� ������� � �������");
                    }
                }
                
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);

              int num = RadioButtonList1.SelectedIndex;
              string caption;
              headerLayout1.AddCell("����������");
              
              if (num == 0) // ����� ���
              {
                  if (!Check1.Checked && !Check2.Checked)  // ��� ������� �����
                  {
                      for (int i = 1; i < UltraWebGrid.Columns.Count-8; i += 3)
                      {
                          caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                          GridHeaderCell cell1 = headerLayout1.AddCell(string.Format("{0}", caption));
                          cell1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                          cell1.AddCell("����", "");
                          cell1.AddCell("% ����������",
                                        "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      }
                      GridHeaderCell cell2 = headerLayout1.AddCell(string.Format("��. ����. �����������"));
                      cell2.AddCell("����", "");

                      GridHeaderCell cell3 = headerLayout1.AddCell(string.Format("������� �������� ����.� ����.������� ���"));
                      cell3.AddCell("����", "");

                      GridHeaderCell cell4 = headerLayout1.AddCell(string.Format("������ �� ����.����� ����"));
                      cell4.AddCell("����", "");

                      GridHeaderCell cell5 = headerLayout1.AddCell(string.Format("�������"));
                      cell5.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell5.AddCell("����", "");
                      cell5.AddCell("% ����������","������� ���������� ������� ������ � ��������� � ����������� �����������");

                      GridHeaderCell cell6 = headerLayout1.AddCell(string.Format("�������/��������"));
                      cell6.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell6.AddCell("����", "");
                      
                      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i ++)
                      {
                          e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                          if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("%"))
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                          }
                         else
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                          }
                      }
                  }
                  else if (!Check1.Checked && Check2.Checked)  // ����������� ���� �����
                  {
                      for (int i = 1; i < UltraWebGrid.Columns.Count - 9; i += 4)
                      {
                          caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                          GridHeaderCell cell1 = headerLayout1.AddCell(string.Format("{0}", caption));
                          cell1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                          cell1.AddCell("����", "");
                          cell1.AddCell("% ����������",
                                        "������� ���������� ������� ������ � ��������� � ����������� �����������");
                          cell1.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");
                      }
                      GridHeaderCell cell2 = headerLayout1.AddCell(string.Format("��. ����. �����������"));
                      cell2.AddCell("����", "");

                      GridHeaderCell cell3 = headerLayout1.AddCell(string.Format("������� �������� ����.� ����.������� ���"));
                      cell3.AddCell("����", "");

                      GridHeaderCell cell4 = headerLayout1.AddCell(string.Format("������ �� ����.����� ����"));
                      cell4.AddCell("����", "");

                      GridHeaderCell cell5 = headerLayout1.AddCell(string.Format("�������"));
                      cell5.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell5.AddCell("����", "");
                      cell5.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell5.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");

                      GridHeaderCell cell6 = headerLayout1.AddCell(string.Format("�������/��������"));
                      cell6.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell6.AddCell("����", "");

                      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                      {
                          e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                          if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("%"))
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                          }
                          else if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("���� �����"))
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                          }
                          else
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                          }
                      }
                  }
                  else if (Check1.Checked && !Check2.Checked) // ����������� ������ ��� ��
                  {
                      GridHeaderCell cell0_1 = headerLayout1.AddCell("������ �����");
                      cell0_1.AddCell("���.��.(���������)", "������� ���������� ������ ����������� ������ ��������");
                      cell0_1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell0_1.AddCell("����", "");
                      cell0_1.AddCell("% ����������","������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell0_1.AddCell("% ���������� (���������)", "������� ���������� ���������� ������ � ��������� � ����������� �����������");

                      GridHeaderCell cell0_2 = headerLayout1.AddCell("��������� � ����������� ������");
                      cell0_2.AddCell("���.��.(���������)", "������� ���������� ������ ����������� ������ ��������");
                      cell0_2.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell0_2.AddCell("����", "");
                      cell0_2.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell0_2.AddCell("% ���������� (���������)", "������� ���������� ���������� ������ � ��������� � ����������� �����������");

                      for (int i = 11; i < UltraWebGrid.Columns.Count - 10 ; i += 3)
                      {
                          caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                          GridHeaderCell cell1 = headerLayout1.AddCell(string.Format("{0}", caption));
                          cell1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                          cell1.AddCell("����", "");
                          cell1.AddCell("% ����������",
                                        "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      }

                      GridHeaderCell cell2 = headerLayout1.AddCell(string.Format("��. ����. �����������"));
                      cell2.AddCell("����", "");

                      GridHeaderCell cell3 = headerLayout1.AddCell(string.Format("������� �������� ����.� ����.������� ���"));
                      cell3.AddCell("����", "");

                      GridHeaderCell cell4 = headerLayout1.AddCell(string.Format("������ �� ����.����� ����"));
                      cell4.AddCell("����", "");

                      GridHeaderCell cell5 = headerLayout1.AddCell(string.Format("�������"));
                      cell5.AddCell("���.��.(���������)", "������� ���������� ������ ����������� ������ ��������");
                      cell5.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell5.AddCell("����", "");
                      cell5.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell5.AddCell("% ���������� (���������)", "������� ���������� ���������� ������ � ��������� � ����������� �����������");
                      
                      GridHeaderCell cell6 = headerLayout1.AddCell(string.Format("�������/��������"));
                      cell6.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell6.AddCell("����", "");

                      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i ++)
                      {
                          e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                          if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("%"))
                          {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                          }
                          else if ( e.Layout.Bands[0].Columns[i].Header.Caption.Contains("���� �����"))
                          {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                          }
                          else
                          {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                          }
                      }
                  }
                  else if (Check1.Checked && Check2.Checked) // �������� ��� �������
                  {
                      GridHeaderCell cell0_1 = headerLayout1.AddCell("������ �����");
                      cell0_1.AddCell("���.��.(���������)", "������� ���������� ������ ����������� ������ ��������");
                      cell0_1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell0_1.AddCell("����", "");
                      cell0_1.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell0_1.AddCell("% ���������� (���������)", "������� ���������� ���������� ������ � ��������� � ����������� �����������");
                      cell0_1.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");

                      GridHeaderCell cell0_2 = headerLayout1.AddCell("��������� � ����������� ������");
                      cell0_2.AddCell("���.��.(���������)", "������� ���������� ������ ����������� ������ ��������");
                      cell0_2.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell0_2.AddCell("����", "");
                      cell0_2.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell0_2.AddCell("% ���������� (���������)", "������� ���������� ���������� ������ � ��������� � ����������� �����������");
                      cell0_2.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");

                      for (int i = 13; i < UltraWebGrid.Columns.Count - 11; i += 4)
                      {
                          caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                          GridHeaderCell cell1 = headerLayout1.AddCell(string.Format("{0}", caption));
                          cell1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                          cell1.AddCell("����", "");
                          cell1.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                          cell1.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");
                      }

                      GridHeaderCell cell2 = headerLayout1.AddCell(string.Format("��. ����. �����������"));
                      cell2.AddCell("����", "");

                      GridHeaderCell cell3 = headerLayout1.AddCell(string.Format("������� �������� ����.� ����.������� ���"));
                      cell3.AddCell("����", "");

                      GridHeaderCell cell4 = headerLayout1.AddCell(string.Format("������ �� ����.����� ����"));
                      cell4.AddCell("����", "");

                      GridHeaderCell cell5 = headerLayout1.AddCell(string.Format("�������"));
                      cell5.AddCell("���.��.(���������)", "������� ���������� ������ ����������� ������ ��������");
                      cell5.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell5.AddCell("����", "");
                      cell5.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell5.AddCell("% ���������� (���������)", "������� ���������� ���������� ������ � ��������� � ����������� �����������");
                      cell5.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");

                      GridHeaderCell cell6 = headerLayout1.AddCell(string.Format("�������/��������"));
                      cell6.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell6.AddCell("����", "");
                      
                      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                      {
                          e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                          if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("%"))
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                          }
                          else if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("���� �����"))
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                          }
                          else
                          {
                              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                          }
                      }
                  }
              }
              
              if (num == 1) //����������� ������ ������
              {
                 

                  for (int i = 1; i < UltraWebGrid.Columns.Count; i += 4)
                  {
                      caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                      GridHeaderCell cell1 = headerLayout1.AddCell(string.Format("{0}", caption));
                      cell1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                      cell1.AddCell("����", "");
                      cell1.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                      cell1.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");
                      
                  }
                  
                  for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i += 4)
                  {
                      e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                      CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                      CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                      CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N1");
                      CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 3], "N2");
                  }
              }

            if (num == 2)  // ����������� ������ �������
              {
              
               GridHeaderCell cell0 = headerLayout1.AddCell(string.Format("�������"));
               cell0.AddCell("���.��.", "������� ������ ����������� ������ ��������");
               cell0.AddCell("����", "");
               cell0.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
               cell0.AddCell("���� �����","���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");
               
               
               for (int i=5; i<UltraWebGrid.Columns.Count; i+=5)
               {
                   caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                   GridHeaderCell cell1 = headerLayout1.AddCell(string.Format("{0}", caption));
                   cell1.AddCell("���.��.", "������� ������ ����������� ������ ��������");
                   cell1.AddCell("����","");
                   cell1.AddCell("% ����������", "������� ���������� ������� ������ � ��������� � ����������� �����������");
                   cell1.AddCell("���� �����", "���� ����� ������������ ���������� �� ������� ���� � ������������ ������� �������� ����");
                   cell1.AddCell("�������� ���", "�������� ��� � ����� ����� ��������");
               }

               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
               
               for (int i = 5; i < e.Layout.Bands[0].Columns.Count; i+=5)
               {
                   e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+1], "N0");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N1");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 3], "N2");
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 4], "N1");
                   
               }
            }

             for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++ )
             {
               e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
             }

            headerLayout1.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
               UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }

                    
                }
            }

            if (e.Row.Cells[0].Value != null &&
                          (e.Row.Cells[0].Value.ToString().Contains("����� �� �������") ||
                           e.Row.Cells[0].Value.ToString().Contains("����� �� �������") ||
                           e.Row.Cells[0].Value.ToString().Contains("����� �� ������� � �������")))
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (UltraWebGrid.Rows.Count < 30)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout1, section1);


        }
      
        #endregion
    }
}
