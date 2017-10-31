using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
namespace Krista.FM.Server.Dashboards.reports.FO_0008_0002
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtTable = new DataTable();
        private DataTable dtDate = new DataTable();
        private DataTable dtNewTable;
        private int endYear = 2012;
        private string month;
        private int day;
        private int year;
        private int nCol = 1;
        private string query;

        private DateTime currentDate;
        private DateTime date;
        #endregion

        #region ��������� �������

        private CustomParam param;
        // ��������� ������
        private CustomParam selectedPeriod;
        // 
        private CustomParam dates;
        private CustomParam rubMultiplier;
        private CustomParam rest;
        private CustomParam currentYear;
        #endregion

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

            #region ��������� �����

            GridBrick.Height = CustomReportConst.minScreenHeight / 2;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow +=new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region ������������� ����������

            param = UserParams.CustomParam("param");
            selectedPeriod = UserParams.CustomParam("selected_period");
            dates = UserParams.CustomParam("dates");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            rest = UserParams.CustomParam("rest");
            currentYear = UserParams.CustomParam("cur_year");
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                CustomCalendar1.Visible = true;
                // �������� ��������� ����
                query = DataProvider.GetQueryText("FO_0008_0002_endYear");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                query = DataProvider.GetQueryText("FO_0008_0002_endDate");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                month = dtDate.Rows[0][3].ToString();
                day = Convert.ToInt32(dtDate.Rows[0][4]);
                date = new DateTime(endYear, CRHelper.MonthNum(month), day);
                // �������������� ���������
                CustomCalendar1.WebCalendar.SelectedDate = date;
                
            }
            
            year = CustomCalendar1.WebCalendar.SelectedDate.Year;
            month = CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month);
            day = CustomCalendar1.WebCalendar.SelectedDate.Day;

            date = new DateTime(year, CRHelper.MonthNum(month), day);

            currentYear.Value = date.Year.ToString();

            string dateList = string.Empty;

            for (int i=1; i<=day; i++)
            {
                dateList += string.Format("[������__������].[������__������].[������ ���� ��������].[{4}].[��������� {3}].[������� {2}].[{1}].[{0}],", i, month, CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)), CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)), year);
            }
            dates.Value = dateList.TrimEnd(',');

            string par = string.Empty;
            if (CheckBox1.Checked)
            {
                par += " Measures.[��� ������� �������],";
                nCol++;
            }
            if (CheckBox2.Checked)
            {
                par += " Measures.[������� ������],";
                nCol++;
            }
            if (CheckBox3.Checked)
            {
                par += "Measures.[��������],";
                nCol++;
            }
            if (CheckBox4.Checked)
            {
                par += " Measures.[���������� ��������� �������],";
                nCol++;
            }
             if (CheckBox5.Checked)
            {
                par += "Measures.[������� ���������������� ���./�������],";
                nCol++;
            }
            param.Value = par;

            rest.Value = string.Empty;
            if (CheckBox6.Checked)
            {
              rest.Value = "Measures.[������� �� �����],";
            }
            rubMultiplier.Value = IsThsRubSelected ? "1000": "1000000";
            Page.Title = string.Format("�������������� �� ���� ������� ������������ ������� ������ � �������� ����������� ������� �������� � {0} ����, �� ��������� �� {1:dd.MM.yyyy}, {2}", year, CustomCalendar1.WebCalendar.SelectedDate.Date, RubMultiplierCaption);
            Label1.Text = Page.Title;

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[������].[������]", date, 5);

            GridDataBind();
        }

        #region ����������� �����
        
        private void GridDataBind()
        {
            
             string query = DataProvider.GetQueryText("FO_0008_0002_grid");
             dtTable = new DataTable();
             DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������",
                                                                                 dtTable);
             if (dtTable.Rows.Count > 0)
             {
                 query = DataProvider.GetQueryText("FO_0008_0002_grid_detail");
                 dtNewTable = new DataTable();
                 DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������",
                                                                         dtNewTable);
                 string name = string.Empty;
                 double value = 0;
                 double res = 0;
                 for (int rowNum = 0; rowNum < dtTable.Rows.Count; rowNum++)
                 {
                     name = dtTable.Rows[rowNum][0].ToString();
                     
                     if (name == "3. ����������� � �������� ���������")
                     {
                         if (dtTable.Rows[rowNum][nCol + 2] != DBNull.Value && dtTable.Rows[rowNum][nCol + 2].ToString() != string.Empty)
                         {
                           value = Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]);
                           if (dtTable.Rows[rowNum][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum][nCol + 1].ToString() != string.Empty && dtTable.Rows[rowNum + 1][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum + 1][nCol + 1].ToString() != string.Empty)
                           {
                             res = (value*Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1])/ (Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1]) + Convert.ToDouble(dtTable.Rows[rowNum+1][nCol + 1])));
                           }
                           else if (dtTable.Rows[rowNum][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum][nCol + 1].ToString() != string.Empty)
                           {
                               res = (value * Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1]) / (Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1]) ));
                           }
                          
                           dtTable.Rows[rowNum][nCol + 2] = res;
                         }

                         if (dtNewTable.Rows.Count > 0)
                         {
                             if (CheckBox3.Checked)
                             {
                                 dtTable.Rows[rowNum]["��������"] = dtNewTable.Rows[0][1];
                             }
                             dtTable.Rows[rowNum][nCol + 3] = dtNewTable.Rows[0][2];
                             dtTable.Rows[rowNum][nCol + 4] = dtNewTable.Rows[0][3];
                         }

                         dtTable.AcceptChanges();
                         if (CheckBox6.Checked)
                         {
                             if (dtTable.Rows[rowNum][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum][nCol + 1].ToString() != string.Empty)
                             {
                                 // ������� �� �����
                                 if ( dtTable.Rows[rowNum][nCol + 4] != DBNull.Value && dtTable.Rows[rowNum][nCol + 4].ToString() != string.Empty)
                                 {
                                     dtTable.Rows[rowNum][nCol+5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1]) - Convert.ToDouble(dtTable.Rows[rowNum][nCol + 4]);
                                 }
                                 else
                                 {
                                     dtTable.Rows[rowNum][nCol + 5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1]) ;
                                 }
                               
                             }
                             // ������� �������
                             if (dtTable.Rows[rowNum][nCol] != DBNull.Value && dtTable.Rows[rowNum][nCol].ToString() != string.Empty && dtTable.Rows[rowNum][nCol + 2] != DBNull.Value && dtTable.Rows[rowNum][nCol + 2].ToString() != string.Empty)
                             {
                                 if (dtTable.Rows[rowNum][nCol + 4] != DBNull.Value && dtTable.Rows[rowNum][nCol + 4].ToString() != string.Empty)
                                 {
                                     dtTable.Rows[rowNum][nCol+6] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]) - Convert.ToDouble(dtTable.Rows[rowNum][nCol + 4]);
                                 }
                                 else
                                 {
                                     dtTable.Rows[rowNum][nCol + 6] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]);
                                 }
                         
                             }
                         }
                         else
                         {
                             // ������� �������
                             if (dtTable.Rows[rowNum][nCol] != DBNull.Value && dtTable.Rows[rowNum][nCol].ToString() != string.Empty && dtTable.Rows[rowNum][nCol + 2] != DBNull.Value && dtTable.Rows[rowNum][nCol + 2].ToString() != string.Empty)
                             {
                                 if (dtTable.Rows[rowNum][nCol + 4] != DBNull.Value && dtTable.Rows[rowNum][nCol + 4].ToString() != string.Empty)
                                 {
                                     dtTable.Rows[rowNum][nCol +5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]) - Convert.ToDouble(dtTable.Rows[rowNum][nCol + 4]);
                                 }
                                 else
                                 {
                                     dtTable.Rows[rowNum][nCol + 5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]);
                                 }
                              
                             }
                         }
                     }

                     if (name == "4. ������������� � �������� ���������")
                     {
                         if (dtTable.Rows[rowNum][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum][nCol + 1].ToString() != string.Empty && dtTable.Rows[rowNum - 1][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum - 1][nCol + 1].ToString() != string.Empty && value != 0)
                         {
                             res = (value*Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1])/
                                    (Convert.ToDouble(dtTable.Rows[rowNum - 1][nCol + 1]) +
                                     Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1])));

                             dtTable.Rows[rowNum][nCol + 2] = res;
                         }
                         else if (dtTable.Rows[rowNum - 1][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum - 1][nCol + 1].ToString() != string.Empty && value != 0)
                         {
                             if (Convert.ToDouble(dtTable.Rows[rowNum - 1][nCol + 1]) != 0)
                             {
                                 res = (value) / (Convert.ToDouble(dtTable.Rows[rowNum - 1][nCol + 1]));
                                 dtTable.Rows[rowNum][nCol + 2] = res;
                             }
                         }
                         
                         if (dtNewTable.Rows.Count > 1)
                             {
                                 if (CheckBox2.Checked)
                                 {
                                     dtTable.Rows[rowNum]["������� ������"] = dtTable.Rows[rowNum-1]["������� ������"];
                                 }

                                 if (CheckBox3.Checked)
                                 {
                                     dtTable.Rows[rowNum]["��������"] = dtNewTable.Rows[1][1];
                                 }
                                 dtTable.Rows[rowNum][nCol + 3] = dtNewTable.Rows[1][2];
                                 dtTable.Rows[rowNum][nCol + 4] = dtNewTable.Rows[1][3];
                             }
                         
                         dtTable.AcceptChanges();
                         if (CheckBox6.Checked)
                         {
                             if (dtTable.Rows[rowNum][nCol + 1] != DBNull.Value && dtTable.Rows[rowNum][nCol + 1].ToString() != string.Empty)
                             {
                                 // ������� �� �����
                                 if (dtTable.Rows[rowNum][nCol + 4] != DBNull.Value && dtTable.Rows[rowNum][nCol + 4].ToString() != string.Empty)
                                 {
                                     dtTable.Rows[rowNum][nCol + 5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1]) - Convert.ToDouble(dtTable.Rows[rowNum][nCol + 4]);
                                 }
                                 else
                                 {
                                     dtTable.Rows[rowNum][nCol + 5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol + 1]);
                                 }
                             
                             }
                             // ������� �������
                             if (dtTable.Rows[rowNum][nCol] != DBNull.Value && dtTable.Rows[rowNum][nCol].ToString() != string.Empty && dtTable.Rows[rowNum][nCol + 2] != DBNull.Value && dtTable.Rows[rowNum][nCol + 2].ToString() != string.Empty)
                             {
                                 if (dtTable.Rows[rowNum][nCol + 4] != DBNull.Value && dtTable.Rows[rowNum][nCol + 4].ToString() != string.Empty)
                                 {
                                     dtTable.Rows[rowNum][nCol + 6] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]) - Convert.ToDouble(dtTable.Rows[rowNum][nCol + 4]);
                                 }
                                 else
                                 {
                                     dtTable.Rows[rowNum][nCol + 6] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]);
                                 }
                             
                             }
                         }
                         else
                         {
                             // ������� �������
                             if (dtTable.Rows[rowNum][nCol] != DBNull.Value && dtTable.Rows[rowNum][nCol].ToString() != string.Empty && dtTable.Rows[rowNum][nCol + 2] != DBNull.Value && dtTable.Rows[rowNum][nCol + 2].ToString() != string.Empty)
                             {
                                 if (dtTable.Rows[rowNum][nCol + 4] != DBNull.Value && dtTable.Rows[rowNum][nCol + 4].ToString() != string.Empty)
                                 {
                                     dtTable.Rows[rowNum][nCol + 5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]) - Convert.ToDouble(dtTable.Rows[rowNum][nCol + 4]);
                                 }
                                 else
                                 {
                                     dtTable.Rows[rowNum][nCol + 5] = Convert.ToDouble(dtTable.Rows[rowNum][nCol]) + Convert.ToDouble(dtTable.Rows[rowNum][nCol + 2]);
                                 }
                                
                             }
                         }
                     }

                 }
                 
                 dtTable.AcceptChanges();
                 FontRowLevelRule levelRule = new FontRowLevelRule(dtTable.Columns.Count - 1);
                 levelRule.AddFontLevel("1",new Font(GridBrick.Grid.DisplayLayout.RowStyleDefault.Font.Name, 10,FontStyle.Bold));
                 levelRule.AddFontLevel("2",new Font(GridBrick.Grid.DisplayLayout.RowStyleDefault.Font.Name, 10,FontStyle.Bold));
                 GridBrick.AddIndicatorRule(levelRule);

                 GridBrick.DataTable = dtTable;
             }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count -1].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            string columnFormat = "";

            for (int i = 1; i < columnCount ; i++)
            {
               if (e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("��������") || e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("������� ������") )
                {
                  columnFormat = "000 00 00"; 
                }
               else if (e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("��� ������� �������"))
                {
                  columnFormat = "0 00 000 000"; 
                }
                else
                {
                  columnFormat = "N2";
                }

               if (e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("���������� ��������� �������"))
                {
                   e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                   e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(140);
                }
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], columnFormat);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("������������ ��������, ����������� � ��������");

            if (CheckBox1.Checked)
            {
              headerLayout.AddCell("��� ������� �������");
            }
            if (CheckBox2.Checked)
            {
                headerLayout.AddCell("������� ������");
            }
            if (CheckBox3.Checked)
            {
                headerLayout.AddCell("��������");
            }
            if (CheckBox4.Checked)
            {
                headerLayout.AddCell("���������� ��������� �������");
            }
            if (CheckBox5.Checked)
            {
                headerLayout.AddCell("������� ����������������");
            }
            
            headerLayout.AddCell(string.Format("������� ������� 01.01.{0} ���", date.Year));
            headerLayout.AddCell(string.Format("���� � �����.� ����.������.�� {0}", date.Year));
            headerLayout.AddCell(string.Format("��������� � {0} ����", date.Year));
            headerLayout.AddCell(string.Format("�������������� �� {1} �� {0:dd.MM.yyyy} ��� ", date, month));
            headerLayout.AddCell(string.Format("�������������� ����� �� {0:dd.MM.yyyy} ��� ", date));
            
            if (CheckBox6.Checked)
            {
                headerLayout.AddCell(string.Format("������� �� ����� �� {0:dd.MM.yyyy} ��� ", date));
            }

            headerLayout.AddCell(string.Format("������� ������� �� {0:dd.MM.yyyy} ��� ", date));
            
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }
        
        protected  void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0] != null)
            {
                e.Row.Cells[0].Value =
                    e.Row.Cells[0].Value.ToString().Replace(
                        "�������������� �� ���� ������� ���.������� ������ � �������� ���.������� ��������, �����", "�����");
            }


            string level = string.Empty;
            int numCol = 0;
            if (e.Row.Cells[e.Row.Cells.Count-1].Value != null && e.Row.Cells[e.Row.Cells.Count-1].ToString() != string.Empty)
            {
                level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();

                switch (level)
                {
                    case "3":
                        {
                            e.Row.Cells[numCol].Style.Padding.Left = 10;
                            break;
                        }
                    case "4":
                        {
                            e.Row.Cells[numCol].Style.Padding.Left = 15;
                            break;
                        }
                    case "5":
                        {
                            e.Row.Cells[numCol].Style.Padding.Left = 20;
                            break;
                        }
                    case "6":
                        {
                            e.Row.Cells[numCol].Style.Padding.Left = 25;
                            break;
                        }
                }
            }

           
        }

        #endregion
        
        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}