using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0051_0001
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable gridDt = new DataTable();
        private int firstYear = 2010;
        private DateTime currentDate;
        private static MemberAttributesDigest regionDigest;

        #endregion

        #region ��������� �������

        // ��������� ��
        private CustomParam selectedMO;
        // ������� ��
        private CustomParam regionLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����

            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region ������������� ���������� �������

            selectedMO = UserParams.CustomParam("selected_mo");
            regionLevel = UserParams.CustomParam("region_level");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            regionLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.MoFinPassportInfo.LastDate;
                
                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.Set�heckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 160;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), true);
                
                ComboRegion.Title = "��";
                ComboRegion.Width = 300;
                ComboRegion.ParentSelect = false;
                ComboRegion.MultiSelect = false;
                regionDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0051_0001_regionList");
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(regionDigest.UniqueNames, regionDigest.MemberLevels));
                ComboRegion.Set�heckedState("����������� ������������� �����", true);
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int monthNum = ComboMonth.SelectedIndex + 1;
            currentDate = new DateTime(yearNum, monthNum, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            Page.Title = String.Format("��������� ���������� ���������� �������: {0}", ComboRegion.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = String.Format("���������� ������� �� ��������� �� {0:dd.MM.yyyy} ����, ���.���.", currentDate.AddMonths(1));

            selectedMO.Value = regionDigest.GetMemberUniqueName(ComboRegion.SelectedValue);

            GridDataBind();
        }

        #region ����������� �����
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0051_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                gridDt.PrimaryKey = new DataColumn[] { gridDt.Columns[0]};

                DataRow deficitRow = gridDt.Rows.Find("�������� (+) / ������� (-) ");
                DataRow remainsRow = gridDt.Rows.Find("������� ������� ��������, ����� ");
                DataRow lackRow = gridDt.Rows.Find("���������� �������");

                foreach (DataColumn column in gridDt.Columns)
                {
                    double value;

                    double deficitValue = Double.MinValue;
                    if (deficitRow[column.ColumnName] != DBNull.Value && Double.TryParse(deficitRow[column.ColumnName].ToString(), out value))
                    {
                        deficitValue = value;
                    }

                    double remainsValue = Double.MinValue;
                    if (remainsRow[column.ColumnName] != DBNull.Value && Double.TryParse(remainsRow[column.ColumnName].ToString(), out value))
                    {
                        remainsValue = value;
                    }

                    if (remainsValue > 0)
                    {
                        if (deficitValue != Double.MinValue)
                        {
                            lackRow[column.ColumnName] = deficitValue + remainsValue;
                        }
                    }
                }
                
                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
                GridBrick.AddIndicatorRule(levelRule);

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            for (int i = 1; i < columnCount - 2; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption));
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("����������");

            currentDate = currentDate.AddMonths(1);

            headerLayout.AddCell(String.Format("������������ ������������� �� 01.01.{0}&nbsp;�.", currentDate.Year - 1));

            GridHeaderCell prevYearGroupCell = headerLayout.AddCell("�������� ���");
            prevYearGroupCell.AddCell(String.Format("���������� ���� �� {0}&nbsp;�.", currentDate.Year - 1));
            prevYearGroupCell.AddCell(String.Format("���������� �� {0}&nbsp;�.", currentDate.Year - 1));
            prevYearGroupCell.AddCell("% ���������� ");

            GridHeaderCell currYearGroupCell = headerLayout.AddCell("������� ���������� ���");
            currYearGroupCell.AddCell(String.Format("������������ ������������� �� 01.01.{0}&nbsp;�.", currentDate.Year));
            currYearGroupCell.AddCell(String.Format("������������ ������������� �� {0:dd.MM.yyyy}&nbsp;�.", currentDate));
            currYearGroupCell.AddCell(String.Format("������������ ������ (�� ��������� �� {0:dd.MM.yyyy}&nbsp;�.)", currentDate));
            currYearGroupCell.AddCell("������������ ������ � % � ����� �� �������� ���");
            currYearGroupCell.AddCell(String.Format("���������� �� {0:dd.MM.yyyy}&nbsp;�.", currentDate));
            currYearGroupCell.AddCell("���������� �� ������� ���� � % � ������������� ������� �� ���");
            currYearGroupCell.AddCell("��������� ���������� ������� �� ������� ���");
            currYearGroupCell.AddCell("��������� ���������� � % � ��������� ����");

            headerLayout.AddCell("����������");

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            return columnName.Contains("%") ? "P2" : "N2";
        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            bool inverse = false;
            if (e.Row.Cells[e.Row.Cells.Count - 2].Value != null)
            {
                inverse = Convert.ToBoolean(e.Row.Cells[e.Row.Cells.Count - 2].Value.ToString());
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];

                string columnCaption = e.Row.Band.Grid.Columns[i].Header.Caption;
                bool rateColumn = columnCaption.Contains("������������ ������ � %") || columnCaption.Contains("��������� ���������� � %");

                if (rateColumn )
                {
                    double growRate = GetDoubleDtValue(e.Row, i);
                    if (growRate != Double.MinValue)
                    {
                        if (growRate > 1)
                        {
                            cell.Style.BackgroundImage = inverse ? "~/images/arrowGreenUpBB.png" : "~/images/arrowRedUpBB.png";
                            cell.Title = "����������� ���� �������� ���������� ������������ ���������� �������� ����";
                        }
                        else if (growRate < 1)
                        {
                            cell.Style.BackgroundImage = inverse ? "~/images/arrowRedDownBB.png" : "~/images/arrowGreenDownBB.png";
                            cell.Title = "����������� �������� �������� ���������� ������������ ���������� �������� ����";
                        }
                        cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }
        }

        protected static double GetDoubleDtValue(UltraGridRow row, int cellIndex)
        {
            if (row.Cells.Count > cellIndex)
            {
                double value;
                if (row.Cells[cellIndex].Value != null && Double.TryParse(row.Cells[cellIndex].Value.ToString(), out value))
                {
                    return value;
                }
            }
            return Double.MinValue;
        }

        #endregion
        
        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
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
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}