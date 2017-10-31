using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0006_0001
{
    public partial class Default : CustomReportPage
    {
        private int year = 2008;

        private GridHeaderLayout headerLayout;
        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid2.DataBinding += new EventHandler(UltraWebGrid2_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid2_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid2_InitializeRow);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid2_DataBound);

            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int startYear = 2007;
            int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            if (!Page.IsPostBack)
            {
                ComboFin.Width = 300;
                ComboFin.MultiSelect = false;
                ComboFin.FillDictionaryValues(GetPercentDictionary());
                ComboFin.Title = "����� ������ ���������������";
                ComboFin.Set�heckedState("13%", true);

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(startYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);
            }

            UserParams.Filter.Value = String.Format("[{0}]", GetFilterValue(ComboFin.SelectedIndex));

            UserParams.PeriodFirstYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) + 1).ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue)).ToString();
            UserParams.PeriodEndYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();

            year = Convert.ToInt32(ComboYear.SelectedValue);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid2.Bands.Clear();

            UltraWebGrid.Bands.Clear();

            UltraWebGrid.DataBind();
            UltraWebGrid1.DataBind();
            UltraWebGrid2.DataBind();

            Label1.Text = "������ ������ �� ������ ���������� ���";
            Label2.Text = String.Format("�� {0}-{1} ���� �� ������ {2}", UserParams.PeriodEndYear.Value, UserParams.PeriodLastYear.Value, ComboFin.SelectedValue);

            Label5.Text = String.Format("������� ������������ ������ �� ������ ���������� ��� {0}-{1} (�� ��������� ������ ���������� 1-��, 5-����)", UserParams.PeriodEndYear.Value, UserParams.PeriodLastYear.Value);
        }

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable physFaces = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_PhysFaces");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", physFaces);
            if (physFaces.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = physFaces;
            }
        }

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable taxDeduction = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_TaxDeduction");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", taxDeduction);

            if (taxDeduction.Rows.Count > 0)
            {
              UltraWebGrid1.DataSource = taxDeduction;
            }
            
        }

        void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            DataTable taxLevel = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_TaxLevel");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������ ������", taxLevel);

            for (int i = 0; i < taxLevel.Rows.Count; i++)
            {
                taxLevel.Rows[i][0] = TableRowName(i);
            }

            UltraWebGrid2.DataSource = taxLevel;
        }

        private void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout.AddCell("������������ �����������");
            headerLayout.AddCell("��� ������", "��� ������ � ����� 5-����");
            GridHeaderCell cell = headerLayout.AddCell("�������� �����������, ���");
            cell.AddCell((year - 1).ToString(), String.Format("�������� ���������� �� {0} ���", year - 1));
            cell.AddCell(year.ToString(), String.Format("�������� ���������� �� {0} ���", year));
            headerLayout.AddCell(String.Format("�������� {0} � {1}", year, year - 1), String.Format("��������� ���������� {0} ���� � ������������ ������� {1} ����", year, year - 1));

            headerLayout.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(80, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(90, 1280);

        }

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            bool direct = e.Row.Index < 5;
            SetConditionArrow(e, 4, direct, "����������", 1);
        }

        private void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == 2 || i == 5)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            headerLayout1.AddCell("������������ �����������");
            headerLayout1.AddCell("��� ������", "��� ������ � ����� 5-����");
            headerLayout1.AddCell(String.Format("���������� ������� �� ����� � 2-����<br/>{0}, ��", year));
            GridHeaderCell cell = headerLayout1.AddCell("�������� �����������, ���");
            cell.AddCell((year - 1).ToString(), String.Format("�������� ���������� �� {0} ���", year - 1));
            cell.AddCell(year.ToString(), String.Format("�������� ���������� �� {0} ���", year));
            headerLayout1.AddCell(String.Format("�������� {0} � {1}", year, year - 1), String.Format("��������� ���������� {0} ���� � ������������ ������� {1} ����", year, year - 1));

            headerLayout1.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(80, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(80, 1280);
        }

        private void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            //SetConditionArrow(e, 5, true);
            string level = string.Empty;
            if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() != string.Empty)
            {
                level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
            }

            if (e.Row.Cells[0] != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                if (level == "1")
                {
                    e.Row.Cells[0].Style.Padding.Left = 20;
                    
                }
                else
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                }
            }

                string number = e.Row.Cells[0].Value.ToString().Replace("�� ���� ", "").Replace("������ ", "");
                switch (number)
                {
                    case "101":
                        {
                            e.Row.Cells[0].Title = string.Format("600 ���. �� ������� ������� � �������� �� 18 ���, ����������� ����� �����  ��������,  ���������,  ����������,��������,  ��������  �  ��������  ��  24  ���  ���������, �������� ���������", e.Row.Cells[0].Value);
                            break;
                        }
                    case "102":
                        {
                            e.Row.Cells[0].Title = string.Format("1200 ���. �� ������� ������� � �������� �� 18 ���, �� ��������� ����� �����  ��������,  ���������,  ����������, ��������, �������� � �������� �� 24 ���  �����  (������), ���������  ��������,  �������  ���  ����������,  �������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "103":
                        {
                            e.Row.Cells[0].Title = string.Format("400 ���. �� �����������������, �� ������������ � ����������, ������������� � ��.1 -2  �. 1 ��.218 ���������� ������� ���������� ���������", e.Row.Cells[0].Value);
                            break;
                        }
                    case "104":
                        {
                            e.Row.Cells[0].Title = string.Format("500 ������ �� �����������������, ������������ � ����������, ������������� � ��. 2 �. 1 ��. 218 ���������� ������� ���������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "105":
                        {
                            e.Row.Cells[0].Title = string.Format("3000 ������ �� �����������������, ������������ � ����������, ������������� � ��. 1 �. 1 ��. 218 ���������� ������� ���������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "106":
                        {
                            e.Row.Cells[0].Title = string.Format("1200 ���. �� ������� �������-��������  �  �������� ��  18 ���,  ��  ���������  �����  �����  ��������,   ���������, ����������, ��������, ��������  �  ��������  ��  24  ���, �����������  ���������  I  ���  II   ������,   ���������, �������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "107":
                        {
                            e.Row.Cells[0].Title = string.Format("2400 ���. �� ������� �������-�������� �  ��������  ��  18 ���,  ��  ���������  �����  �����  ��������,   ���������, ����������, �������� � �������� ��  24  ���,  ����������� ��������� I ���  II  ������,  �����  (������),  ��������� ��������, ������� ��� ����������, �������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "108":
                        {
                            e.Row.Cells[0].Title = string.Format("1000 ���. �� ������� ������� � ��������  ��  18  ���,  �� ��������� ����� �����  ��������,  ���������,  ����������, ��������,    ��������    �    ��������    ��    24    ��� ������������������,  ��  �����������  �������   ��������� �������  (��������,  �������   ���������,   �������   ��� ����������,   ��������   ��������,    �������    �������� ���������) (������� � ������� 2009 ����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "109":
                        {
                            e.Row.Cells[0].Title = string.Format("2000 ���. �� ������� �������-�������� �  ��������  ��  18 ���,  ��  ���������  �����  �����  ��������,   ���������, ����������, �������� � �������� ��  24  ���,  ����������� ���������  I  ���  II  ������,   ������������������,   �� ����������� ������� ��������� ������� (��������,  ������� ���������, �������  ���  ����������,  ��������  ��������, ������� �������� ���������) (������� � ������� 2009 ����)", e.Row.Cells[0].Value);
                            break;
                        }
                    case "110":
                        {
                            e.Row.Cells[0].Title = string.Format("2000  ���.  ��  �������  �������  �������������  �������� (���������  ��������),  �������,  ����������  (�������  � ������� 2009 ����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "111":
                        {
                            e.Row.Cells[0].Title = string.Format("2000 ���. �� ������� ������� �������� (��������� ��������) ��� ������� ������ ������� �������� (��������� ��������) �� ��������� ������ � ��������� ����� ������� (������� � ������� 2009 ����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "112":
                        {
                            e.Row.Cells[0].Title = string.Format("4000 ���. �� ������� �������-�������� �  ��������  ��  18 ���,  ��  ���������  �����  �����  ��������,   ���������, ����������, �������� � �������� ��  24  ���,  ����������� ���������  I  ���  II  ������,   �������������   �������� (���������  ��������),  �������,  ����������  (�������  � ������� 2009 ����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "113":
                        {
                            e.Row.Cells[0].Title = string.Format("4000 ���. �� ������� �������-�������� �  ��������  ��  18 ���,  ��  ���������  �����  �����  ��������,   ���������, ����������, �������� � �������� ��  24  ���,  ����������� ��������� I ��� II ������, �������� (���������  ��������) ��� ������� ������ ������� �������� (���������  ��������) �� ��������� ������ � ��������� ����� ������� (�������  � ������� 2009 ����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "114":
                        {
                            e.Row.Cells[0].Title = "�� ������� ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ��������, ������� (�������) ��������, �������, ����������, ��������� ��������, ������� (�������) ��������� ��������, �� ����������� ������� ��������� �������";
                            break;
                        }
                    case "115":
                        {
                            e.Row.Cells[0].Title = "�� ������� ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ��������, ������� (�������) ��������, �������, ����������, ��������� ��������, ������� (�������) ��������� ��������, �� ����������� ������� ��������� �������";
                            break;
                        }
                    case "116":
                        {
                            e.Row.Cells[0].Title = "�� �������� � ������� ������������ ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ��������, ������� (�������) ��������, �������, ����������, ��������� ��������, ������� (�������) ��������� ��������, �� ����������� ������� ��������� �������";
                            break;
                        }
                    case "117":
                        {
                            e.Row.Cells[0].Title = "�� �������-�������� � �������� �� 18 ��� ��� ��������� ����� ����� ��������, ���������, ����������, �������� � �������� �� 24 ���, ����������� ��������� I ��� II ������ ��������, ������� (�������) ��������, �������, ����������, ��������� ��������, ������� (�������) ��������� ��������, �� ����������� ������� ��������� �������";
                            break;
                        }
                    case "118":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� ������� ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ������������� �������� (��������� ��������) �������, ����������";
                            break;
                        }
                    case "119":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� ������� ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ������������� �������� (��������� ��������) �������, ����������";
                            break;
                        }
                    case "120":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� �������� � ������� ������������ ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ������������� �������� (��������� ��������) �������, ����������";
                            break;
                        }
                    case "121":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� �������-�������� � �������� �� 18 ��� ��� ��������� ����� ����� ��������, ���������, ����������, �������� � �������� �� 24 ���, ����������� ��������� I ��� II ������ ������������� �������� (��������� ��������) �������, ����������";
                            break;
                        }
                    case "122":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� ������� ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ������ �� ��������� (�������� ���������) �� �� ������ �� ��������� ��������� �� ������ ������ �� ��������� (�������� ���������) �� ��������� ���������� ������";
                            break;
                        }
                    case "123":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� ������� ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ������ �� ��������� (�������� ���������) �� �� ������ �� ��������� ��������� �� ������ ������ �� ��������� (�������� ���������) �� ��������� ���������� ������";
                            break;
                        }
                    case "124":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� �������� � ������� ������������ ������� � �������� �� 18 ���, � ����� �� ������� ��������� ����� ����� ��������, ���������, ����������, ��������, �������� � �������� �� 24 ��� ������ �� ��������� (�������� ���������) �� �� ������ �� ��������� ��������� �� ������ ������ �� ��������� (�������� ���������) �� ��������� ���������� ������";
                            break;
                        }
                    case "125":
                        {
                            e.Row.Cells[0].Title = "� ������� ������� �� �������-�������� � �������� �� 18 ��� ��� ��������� ����� ����� ��������, ���������, ����������, �������� � �������� �� 24 ���, ����������� ��������� I ��� II ������, ������ �� ��������� (�������� ���������) �� �� ������ �� ��������� ��������� �� ������ ������ �� ��������� (�������� ���������) �� ��������� ���������� ������";
                            break;
                        }
                    case "201":
                        {
                            e.Row.Cells[0].Title = "������� �� ��������� � ������� ��������, ������������� �� �������������� ����� ������ �����";
                            break;
                        }
                    case "202":
                        {
                            e.Row.Cells[0].Title = "������� �� ��������� � ������� ��������, �� ������������� �� �������������� ����� ������ �����";
                            break;
                        }
                    case "203":
                        {
                            e.Row.Cells[0].Title = "������� �� ��������� � ������� ��������, �� ������������� �� �������������� ����� ������ �����, ������� �� ������ �� ������������ ���������� � ������ �������, ������������ �� �������������� ����� ������ �����";
                            break;
                        }
                    case "204":
                        {
                            e.Row.Cells[0].Title = "����� ������ �� ��������� � ������� ��������, �� ������������� �� �������������� ����� ������ �����, ������� �� ������ �� ������������ ���������� � ������ �������, ������������ �� �������������� ����� ������ �����, ����������� ��������� ���� �� ��������� � ������� ��������, ������������� �� �������������� ����� ������ �����";
                            break;
                        }
                    case "205":
                        {
                            e.Row.Cells[0].Title = "����� ������ �� ��������� � ������� ��������, ������������� �� �������������� ����� ������ �����, ����������� ��������� ���� �� ��������� � ����������� ������������� ������� ������ ������� ���������� �� �������������� ����� � �������� ������� ������� �������� ������ ������, �������� ������� ��� ���� ���������� ����������� ������� ������, �������� ������� ������� �������� ������ ������ ��� �������� �������";
                            break;
                        }
                    case "206":
                        {
                            e.Row.Cells[0].Title = "������� �� ��������� � ����������� ������������� ������� ������, ������� ���������� �� �������������� ����� � �������� ������� ������� �������� ������ ������, �������� ������� ��� ���� ���������� ����������� ������� ������, �������� ������� ������� �������� ������ ������ ��� �������� �������";
                            break;
                        }
                    case "207":
                        {
                            e.Row.Cells[0].Title = "������� �� ��������� � ����������� ������������� ������� ������, ������� ���������� �� �������������� ����� � �������� ������� ������� �� �������� ������ ������, �������� ������� ��� ���� ���������� ����������� ������� ������, �������� ������� ������� �������� ������ ������ ��� �������� �������";
                            break;
                        }
                    case "208":
                        {
                            e.Row.Cells[0].Title = "����� ������ �� ��������� � ����������� ������������� ������� ������, ������� ���������� �� �������������� ����� � �������� ������� ������� �������� ������ ������, �������� ������� ��� ���� ���������� ����������� ������� ������, �������� ������� ������� �������� ������ ������ ��� �������� �������, ����������� ��������� ���� �� ��������� � ������� ��������, ������������� �� �������������� ����� ������ �����";
                            break;
                        }
                    case "209":
                        {
                            e.Row.Cells[0].Title = "����� ������ �� ��������� � ����������� ������������� ������� ������ ������� ���������� �� �������������� ����� � �������� ������� ������� �� �������� ������ ������, �������� ������� ��� ���� ���������� ����������� ������� ������, �������� ������� ������� �������� ������ ������ ��� �������� �������, ����������� ��������� ���� �� ��������� � ����������� ������������� ������� ������ ������� ���������� �� �������������� �����";
                            break;
                        }
                    case "210":
                        {
                            e.Row.Cells[0].Title = "����� ������ �� ��������� � ����������� ������������� ������� ������, ������������� �� �������������� ����� ������ ����� � �������� ������� ������� �������� ������ ������, �������� ������� ��� ���� ���������� ����������� ������� ������, �������� ������� ������� �������� ������ ������ ��� �������� �������, ����������� ��������� ���� �� ��������� � ����������� ������������� ������� ������, ������� ���������� �� �������������� �����";
                            break;
                        }
                    case "211":
                        {
                            e.Row.Cells[0].Title = "�������, � ���� ��������� �� �����, ������������� �� ������������ �������� ����";
                            break;
                        }
                    case "212":
                        {
                            e.Row.Cells[0].Title = "����� ���������� �������� �� ��������� ���� ��� �������� �� ��������� ����, ����������� ��������� ���� �� ��������� � ������� ��������, ������������� �� �������������� ����� ������ �����, ������������ � ������������ � ����������, � ������ ��������� ������ �������� ������ 6 ������ 214.3 ���������� ������� ���������� ���������";
                            break;
                        }
                    case "213":
                        {
                            e.Row.Cells[0].Title = "������� �� ���������, ��������� � ��������� �������� �������, � �������, ��������� � ������������� � ����������� ������ �����, ���������� �������� �������� ����";
                            break;
                        }
                    case "214":
                        {
                            e.Row.Cells[0].Title = "������ �� ���������, ��������� � ��������� �������� �������, � �������, ��������� � ������������� � ����������� ������ �����, ���������� �������� �������� ����";
                            break;
                        }
                    case "215":
                        {
                            e.Row.Cells[0].Title = "������� � ���� ���������, ���������� � ��������� ������� �� ������������ ��������� �����";
                            break;
                        }
                    case "216":
                        {
                            e.Row.Cells[0].Title =
                                "����� ���������� �������� � ���� ���������, ���������� �� ������������ ��������� ����� ��� ��������, ����������� �� ������������ ��������� �����, ����������� ��������� ���� �� ��������� � ������� ��������, ������������� �� �������������� ����� ������ �����, ������������ � ������������ � ����������, � ������ ��������� ������ ������� ������ 5 ������ 214.4 ���������� ������� ���������� ���������";
                            break;
                        }
                    case "217":
                        {
                            e.Row.Cells[0].Title = "����� ���������� �������� � ���� ���������, ���������� �� ������������ ��������� ����� ��� ��������, ����������� �� ������������ ��������� �����, ����������� ��������� ���� �� ��������� � ������� ��������, �� ������������� �� �������������� ����� ������ �����, ������������ � ������������ � ����������, � ������ ��������� ������ ������� ������ 5 ������ 214.4 ���������� ������� ���������� ���������";
                            break;
                        }
                    case "301":
                        {
                            e.Row.Cells[0].Title = string.Format("����� ��������� ������ ������������������ ��������� ���������� �� �������, ����������� ��� ���� �����, ��������� ��� �����", e.Row.Cells[0].Value);
                            break;
                        }
                    case "305":
                        {
                            e.Row.Cells[0].Title = string.Format("����� ���������� ������������� � ������������� ��������������   ��������,   ���������   �   �����������, ����������� � � ������������ ������� ������ ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "306":
                        {
                            e.Row.Cells[0].Title = string.Format("����� ���������� ������������� � ������������� ��������������  ��������  ��  ������������,  ��������   � ���������� (���������) �������������� ���� ������ �������������� ������ ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "307":
                        {
                           e.Row.Cells[0].Title = string.Format("����� ���������� ������������� � ������������� �������������� ��������  ��  ������������,  ����������  � �������� ������  �����, ������������ �� �������������� ����� ������ �����, ������� �����, � ������� ��� �������� �  �������  �����  ���  ������������   ������   �����   � ������������� (� ��� ����� ��� ��������� �� ������������� ������ ��� � ��������� �������), � ����� ����� ���������, ����������   ��   �����������    ���������    ����������, ������������� ��� ���������� ������ �����-�������  ������ ����� ",e.Row.Cells[0].Title);
                            break;
                        }
                    case "308":
                        {
                           e.Row.Cells[0].Title = string.Format("����� ���������� ������������� � ������������� �������������� ��������  ��  ������������,  ����������  � �������� ������ �����, �� ������������ ��  �������������� ����� ������ �����, ������� �����, � ������� ��� �������� �  �������  �����  ���  ������������   ������   �����   � ������������� (� ��� ����� ��� ��������� �� ������������� ������ ��� � ��������� �������) ",e.Row.Cells[0].Title);
                            break;
                        }
                    case "309":
                        {
                           e.Row.Cells[0].Title = string.Format("����� ���������� ������������� � ������������� �������������� ��������  ��  ������������,  ����������  � �������� ������ �����, �� ������������ ��  �������������� ����� ������ �����, �������  ��  ������  ��  ������������ �������� �����������, ������������� � ������������ ������ �������, ������� �����, � ������� ��� �������� �  ������� ����� ��� ������������ ������ �����  �  �������������  (� ��� ����� ��� ��������� ��  �������������  ������  ���  � ��������� �������), � ����� ����� ������,  �����������  � ���������  �������  ��  ���������  �����-�������   ������ �����, ������������ �� �������������� ����� ������ �����, ����������� ��� ����������� ��������� ����  ��  ��������� �����-�������   ������   �����,   ��   ������������    �� �������������� ����� ������ �����, ������� ��  ������  �� ������������  ��������  �����������,  �������������   ��� ������ �����, ������������ �� �������������� ����� ������ ����� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "310":
                        {
                           e.Row.Cells[0].Title = string.Format("������, ���������� � ��������� ������� �� ��������� �����-������� ������ �����, ������������ �� �������������� ����� ������ �����, ����������� ��� ����������� ��������� ���� �� ��������� �����-������� ������ �����, ������������ �� �������������� ����� ������ �����, ������� �� ������ �� ������������ �������� �����������, ������������� ��� ������ �����, ������������ �� �������������� ����� ������ �����", e.Row.Cells[0].Value);
                            break;
                        }
                    case "316":
                        {
                           e.Row.Cells[0].Title = string.Format("�����, ���������� �� ������� ������ �����, ������������ � �������������  �����������������  �����  3  ���,  ��   �� ����������� 125  000  ���.  (��  �������,  ����������  �� 01.01.2007) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "317":
                        {
                           e.Row.Cells[0].Title = string.Format("�����, ���������� �� ������� ������ �����, ������������ � �������������  �����������������  3  ����  �  �����   (�� �������, ���������� �� 01.01.2007) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "311":
                        {
                           e.Row.Cells[0].Title = string.Format("�����,  ���������������   ������������������   ��   ����� ������������� ���� ������������ �� ����������  ���������� ��������� ������ ����, ��������, ������� ��� ���� (�����) �   ���,   ���������   ��������,   ���������������    ��� ���������������  ���������  �������������,  �   ��������� ��������,  ��  �������  �����������  �������������  ����� ����, ��� ���� (�����) � ��� (����� ����, ������������ �� ���������  ���������  ��  �������  ������  (��������)   � �������   ����������   �������������   �    ������������� �������������� �������� � �������� �������������� ������� ��������������  ����������   ������   �   ��������������� ��������� ������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "312":
                        {
                           e.Row.Cells[0].Title = string.Format("�����,  ������������ �� ���������  ���������  ��  ������� ������ (��������), ���������� ��  ����������  �����������  ���   ��������������   ����������������   �    ���������� ��������������� ������������������ �� ����� ������������� ���  ������������  ��  ����������  ����������   ��������� ������ ����, ��������, ������� ��� ����  (�����)  �  ���, ��������� ��������, ���������������  ���  ��������������� ��������� �������������, � ��������� ��������, �� ������� ����������� ������������� ����� ����, ��� ���� (�����)  � ��� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "318":
                        {
                           e.Row.Cells[0].Title = string.Format("�����, ������������ �� ��������� ��������� ��  ��������, ����������   ��   ������,   �����������   ��   ���������� ����������   ���������,    �    �����    ���������������� (����������������) �������� �� �����  �������������  ���� ������������ �� ����������  ����������  ���������  ������  ����, ��������, ������� ��� ���� (�����) � ���, ��������� ��������, ��������������� ���  ���������������  ��������� �������������,   �   ���������   ��������,   ��   ������� ����������� ������������� ����� ����, ��� ���� (�����)  � ��� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "319":
                        {
                            e.Row.Cells[0].Title = string.Format("�����  ���������� ������������������ � ���������  ������� ����������    �������     ��     ��������     (���������) ������������������ ����������� �����������,  ������������ (�����������)  ������������������   �   ����������������� ���������� ������ � ���� ������ � (���) � ������  ������� (� ��� ����� � ������ �����, ������),  ���������  (�  ��� �����  ������������),  �����-���������   (�   ���   ����� ������������, ����������� ��� ������ (���������������), � (���) � ����� ���������� ������������������  �  ��������� �������  ���������  �������   ��   ��������   (���������) �������������   �����������   �����������,   ������������ (�����������) �� ��������� ������������ � ����  ������  � (���) � ������  �������  (�  ���  �����  �����,  ������), ��������� (� ��� ����� ������������), �����-���������  (� ���   �����   ������������,   �����������   ���    ������ (���������������), - � �������  ����������  ������������� �������� � ������ �����������, ��������������  �������  2 ������ 219 ���������� ������� ���������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "313":
                        {
                            e.Row.Cells[0].Title = string.Format("�����,  ���������������   ������������������   ��   �����  ������������� ���� ������������ �� ����������  ���������� ��������� ������ ����, ��������, ������� ��� ���� (�����) � ��� (����� ����, ������������ �� ��������� ��������� �� ������� ������ (��������) � ���������� ��������������� �� �����  �������������  ���  ������������   ��   ���������� ���������� ��������� ������ ����, ��������,  �������  ��� ���� (�����) � ���), � ������� ���������� ������������� � ������������� �������������� �������� �  ��������  2  000 000 ���. (������� � ������� 2008 ����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "403":
                        {
                            e.Row.Cells[0].Title = string.Format("����� ���������� ������������� � ������������� ��������������  ��������,  ���������������  ���������   � �����������  �����   (���������   �����)   ��   ��������� ����������-��������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "404":
                        {
                            e.Row.Cells[0].Title = string.Format("����� ���������� ������������� � �������������  �������������� ��������, ��������� � ���������� ���������  �������������� ��� �������������� ��  ��������,  �������, ���������� ���  ����  �������������  ������������  �����, ���������� � ���������, �������������� �������  ��������, ����������� � ������������ �������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "405":
                        {
                            e.Row.Cells[0].Title = string.Format("�����  �  ��������   ����������   ������,   ���������   � ���������� ��������� �������������� �  ��������������  �� ��������, ���������� ��� ���� �������������  ������������ �����, ����������  �  ���������,  ��������������  ������� ��������,  �����������   �   ������������   ��������   (� ��������� � ����� ������������ ������)", e.Row.Cells[0].Value);
                            break;
                        }
                    case "501":
                        {
                            e.Row.Cells[0].Title = string.Format("����� �� ��������� ��������, ���������� �� �����������  � �������������� ���������������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "502":
                        {
                            e.Row.Cells[0].Title = string.Format("�����  ��  ���������  ������  �  ��������  �  ����������� ������,  ����������   ��   ���������   �   �������������, ����������  �  ������������  �  ���������   �������������  ���������� ���������, ���������������  (����������������) �������  ���������������  ������   ���   ���������������� ������� �������� �������������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "503":
                        {
                            e.Row.Cells[0].Title = string.Format("�����  �� ����� ������������ ������,   �����������  �������������� ����� ����������,  �  �����  ������  ����� ����������, ����������� � ����� � �������  ��  ������  �� ������������ ��� �� �������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "504":
                        {
                            e.Row.Cells[0].Title = string.Format("����� �� ����� ���������� (������)  ��������������  ����� ����������, �� ��������, ��������� � �����, ������  �����  ���������� (����������� �� ��������), �  �����  ��������� ���������  �������������  ���  (���  ���)   ������������, ����������� �� ������� ������ ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "505":
                        {
                            e.Row.Cells[0].Title = string.Format("����� �� ���������  ���������  �  ������,  ����������  �� ���������, ����� � ������ ������������  �  �����  �������  ������� (�����, �����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "506":
                        {
                            e.Row.Cells[0].Title = string.Format("����� �� ����� ������������ ������, ����������� ��������� ������������� ������������� ���������", e.Row.Cells[0].Value);
                            break;
                        }
                    case "507":
                        {
                            e.Row.Cells[0].Title = string.Format(" ����� �� ����� ������ (� �������� � �����������  ������), � ����� ��������� ��������, ���������� ���������� �������  �������������  �����,  ����������  �������  ������������� �����, ������� ��������������, �������� � ������ �����  � ����������, ������� ������������� �����, ����� � �������, ������� ������� ��������� ������� �������������  �����  � ������� �������� ���������� �����������, ����� � �����, � ����� ������� �������������������  ��������  �����������, ����� � ������ ���� ��������������� ����������, ��������� ��������� � �� ���������� � ������ ������ ������� ����� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "508":
                        {
                            e.Row.Cells[0].Title = string.Format(" �����  ��  �����  ��������������   ������������   ������, �����������   ��������������    ����������    (���������, ������������,   ��������)   ���   ��������   (����������� (����������)) ������� (������� � ������� 2008 ����) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "509":
                        {
                            e.Row.Cells[0].Title = string.Format(" �����   ��  �����  �������,  ����������   �����������   � ����������� ����� � �������� ������ ����� ��  ����������� - �������������������� ��������������������, ������������ �  ������������  �  �������  2  ������  346.2  ���������� ������� ���������� ���������,  ������������  (����������) ��������  �  ����   ��������������������   ���������   �� ������������  ������������   �   (���)   �����   (�����), �����������   (���������)    ������    �������������    � �������������  (�����������)  �����������   �   ��������� ���������,  �������������  ����,  ����������   ���������� ������������� � �������������  (�����������)  ����������� ��������� (������� � ������� 2009 ���� �  �����������  �� 01.01.2016 ����)", e.Row.Cells[0].Value);
                            break;
                        }
                    case "601":
                        {
                            e.Row.Cells[0].Title = string.Format(" �����, �����������  ���������  ����  ��  �������  �  ����  ���������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "602":
                        {
                            e.Row.Cells[0].Title = string.Format(" ����� ��������� ������������������ ��������� ������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "603":
                        {
                            e.Row.Cells[0].Title = string.Format(" �����  �  �����  ���������  ������������������  ��������� �������   ��    ���������    �������������    ����������� ����������� � ������,  ����  ����������������  ���������� �������, �������� ��������� ������� ��  �����  ����������  �����������������,       ��������������       ����������� ������������������ ����������� ���������� ������ ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "604":
                        {
                            e.Row.Cells[0].Title = string.Format(" ��������  ���������  ���������������  ���������  ��  ���� ����������  ��������  (��  ����  �����������   ���������� ������   -   ��    ��������    �����������    ����������� ���������������),  �����������  ��  �����  ����������  �� ����������� ����� ��������� ��������� ������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "605":
                        {
                            e.Row.Cells[0].Title = string.Format(" �������, ����������� ��� ���������� ������� (��������������) ��������� (�  ������,  ����  ������  ��  �������������), ���  ���������  �������  (��������������) �����  ���������  (� ������ ������������� �������), ����������� �� ����� ���������� �� ����������� ����� ��������� ��������� ������� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "607":
                        {
                            e.Row.Cells[0].Title = string.Format(" ����� � ����� ���������� ������������� ��������� ������� �� ��������� �  ������������  �  �����������  �������  \"� �������������� ��������� ������� ��  �������������  ����� �������� ������ � ��������������� ���������  ������������ ���������� ����������\", �� �� ����� 12000 ������ � ��� ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "606":
                        {
                            e.Row.Cells[0].Title = string.Format(" ����� � ����� ���������  ������������������   �������� (�������)  ��  ���������  ������������������  ����������� �����������,  �����������  �   ��������   ��������������� ��������   �����������   ������������������   ����������� �������,  �  ������,  ����  ����������������   ���������� �������, �������� ��������� ������� ��  �����  ���������� �����������������,  �������������� ����������� ������������������ ����������� ���������� ������ ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "620":
                        {
                            e.Row.Cells[0].Title = string.Format(" ���� �����, ����������� ��������� ���� �  ������������  � �����������  �����  23  ����������   �������   ���������� ��������� ", e.Row.Cells[0].Value);
                            break;
                        }
              }
        }
        
        void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout2.AddCell("������ ������");

            GridHeaderCell cell = headerLayout2.AddCell((year - 1).ToString());
            cell.AddCell("��������� (5-����), ���");
            cell.AddCell("��������� (1-��), ���");
            cell.AddCell("������������");

            cell = headerLayout2.AddCell((year).ToString());
            cell.AddCell("��������� (5-����), ���");
            cell.AddCell("��������� (1-��), ���");
            cell.AddCell("������������");

            cell = headerLayout2.AddCell((year + 1) + "<br/>(���������)");
            cell.AddCell("��������� (1-��), ���");

            headerLayout2.AddCell(String.Format("��������� ������������ {0} �� ������ {1}", year, year - 1));

            headerLayout2.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(120, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(120, 1280);
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(120, 1280);
            e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(110, 1280);

            e.Layout.Bands[0].Columns[9].Hidden = true;
            e.Layout.Bands[0].Columns[10].Hidden = true;
        }

        void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            SetRankImage(e, 9, 3);
            SetRankImage(e, 10, 6);
            SetConditionArrow(e, 8, true, "������ ������������", 1);
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.465);
            UltraWebGrid.Height = Unit.Empty;
        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.535);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.8 ); ;
        }

        void UltraWebGrid2_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid2.Height = Unit.Empty;
        }
        
        public static void SetConditionArrow(RowEventArgs e, int index, bool direct, string description, int borderValue)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                string title;
                if (direct)
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                        title = String.Format("���� {0} � ������������ ������� �������� ����", description);
                    }
                    else
                    {
                        img = "~/images/arrowRedDownBB.png";
                        title = String.Format("�������� {0} � ������������ ������� �������� ����", description);
                    }
                }
                else
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowRedUpBB.png";
                        title = String.Format("���� {0} � ������������ ������� �������� ����", description);
                    }
                    else
                    {
                        img = "~/images/arrowGreenDownBB.png";
                        title = String.Format("�������� {0} � ������������ ������� �������� ����", description);
                    }
                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
                e.Row.Cells[index].Title = title;
            }
        }

        private Dictionary<string, int> GetPercentDictionary()
        {
            Dictionary<string, int> kinds = new Dictionary<string, int>();
            kinds.Add("13%", 0);
            kinds.Add("30%", 0);
            kinds.Add("9%", 0);
            kinds.Add("35%", 0);
            kinds.Add("����", 0);
            return kinds;
        }

        private string GetFilterValue(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        return
                            "������ I. ��������� ����, ���������� ��������������� �� ������ 13%, ����� ����� ������������, ����������� � �������������� ������";
                    }
                case 1:
                    {
                        return
                            "������ II. ��������� ����, ���������� ��������������� �� ������ 30%, ����� ����� ������������, ����������� � �������������� ������";
                    }
                case 2:
                    {
                        return
                            "������ III. ��������� ����, ���������� ��������������� �� ������ 9%, ����� ����� ������������ ����������� � �������������� ������";
                    }
                case 3:
                    {
                        return
                            "������ IV. ��������� ����, ���������� ��������������� �� ������ 35%, ����� ����� ������������, ����������� � �������������� ������";
                    }
                case 4:
                    {
                        return
                            "������ V. ��������� ����, ���������� ��������������� �� ��������� �������, ������������� � ����������� �� ��������� �������� ���������������, ����� ����� ������������, ����������� � �������������� ������ (5%, 10%, 15%)";
                    }
            }
            return String.Empty;
        }

        private string TableRowName(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        return
                            "13%";
                    }
                case 1:
                    {
                        return
                            "30%";
                    }
                case 2:
                    {
                        return
                            "9%";
                    }
                case 3:
                    {
                        return
                            "35%";
                    }
                case 4:
                    {
                        return
                            "����";
                    }
            }
            return String.Empty;
        }

        public static void SetRankImage(RowEventArgs e, int rankCellIndex, int indicateCellIndex)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = 5;
                string img = String.Empty;
                string title = String.Empty;
                if (value == 1)
                {
                    img = "~/images/StarYellowBB.png";
                    title = "����� ������� ������� ������������ ������";
                }
                else if (value == worseRankValue)
                {
                    img = "~/images/StarGrayBB.png";
                    title = "����� ������ ������� ������������ ������";
                }

                e.Row.Cells[indicateCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[indicateCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 30px center; padding-left: 2px; padding-right: 10px";
                e.Row.Cells[indicateCellIndex].Title = title;
            }
        }


        #region  �������

        void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook book = new Workbook();
            ReportExcelExporter1.SheetColumnCount = 20;
            ReportExcelExporter1.WorksheetTitle = Label1.Text + " " + Label2.Text;

            Worksheet sheet = book.Worksheets.Add("������ ������� ���������� ���");
            // sheet.Rows[1].Cells[0].Value = "������ ������� ���������� ��� �� ������� ���������������";
            ReportExcelExporter1.Export(headerLayout, Label3.Text,sheet, 4);

            sheet = book.Worksheets.Add("������ ��������� �������");
            // sheet.Rows[1].Cells[0].Value = "������ ��������� �������";
            ReportExcelExporter1.Export(headerLayout1, Label4.Text ,sheet, 4);

            sheet = book.Worksheets.Add("������� ������������ ������");
            // sheet.Rows[1].Cells[0].Value = Label5.Text;
            ReportExcelExporter1.Export(headerLayout2,Label5.Text, sheet, 4);
        }

        void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = String.Empty;
            ReportPDFExporter1.PageSubTitle = String.Empty;

            Report report = new Report();
            ISection section = report.AddSection();

            IText title = section.AddText();
            title.Style.Font = new Font("Verdana", 18);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = section.AddText();
            title.Style.Font = new Font("Verdana", 16);
            title.AddContent(Label2.Text);

            IText text = section.AddText();
            text.AddContent("������ ������� ���������� ��� �� ������� ���������������");
            text.Style.Font = new Font("Verdana", 14);
            ReportPDFExporter1.Export(headerLayout, section);

            text = section.AddText();
            text.AddContent("������ ��������� �������");
            text.Style.Font = new Font("Verdana", 14);
            ReportPDFExporter1.Export(headerLayout1, section);

            text = section.AddText();
            text.AddContent(Label5.Text);
            text.Style.Font = new Font("Verdana", 14);
            ReportPDFExporter1.Export(headerLayout2, section);
        }
        #endregion



    }
}
