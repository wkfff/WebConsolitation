using System;
using System.Data;
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
namespace Krista.FM.Server.Dashboards.reports.FO_0035_0012
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable gridDt = new DataTable();
        private int firstYear = 2012;

        private DateTime currentDate;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����

            GridBrick.Height = CustomReportConst.minScreenHeight - 255;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                DateTime lastCubeDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "FO_0035_0012_lastDate");

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastCubeDate.Year));
                ComboYear.Set�heckedState(lastCubeDate.Year.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastCubeDate.Month)), true);
            }

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int monthNum = CRHelper.MonthNum(ComboMonth.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            Page.Title = "���������� �� �������� ������������ ������������� ������������� �������������";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("������ �� {0:dd.MM.yyyy} �., ���.���.", currentDate.AddMonths(1));
            
            GridDataBind();
        }

        #region ����������� �����
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0012_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", gridDt);

            FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
            levelRule.AddFontLevel("0", GridBrick.BoldFont10pt);
            GridBrick.AddIndicatorRule(levelRule);

            GridBrick.DataTable = gridDt;
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

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnFormat = i < 3 ? "0" : "N2";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], columnFormat);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("������������ ����������� � ��������", "������� ������������ �������������");

            headerLayout.AddCell("��� ������ �������������", "��� ������ �������������");
            headerLayout.AddCell("��� ������������ �����", "��� ������������ �����");

            headerLayout.AddCell(String.Format("������� ��������� � ����� 01.01.{0}", currentDate.Year), "������� ��������� � ����� �� ������ ����");
            headerLayout.AddCell(String.Format("����������� ����� �� 01.01.{0}", currentDate.Year), "����������� ����� �� ������ ����");
            headerLayout.AddCell(String.Format("������� ������� ��������� � ����� 01.01.{0} �. �� 01.01.{0}", currentDate.Year), "������� ������� ��������� � ����� �� ������ ����");
            headerLayout.AddCell(String.Format("������������ (-), ����������� (+) ������������� �� {0} ���", currentDate.Year), "������������ (-), ����������� (+) ������������� �� ������ ����");
            headerLayout.AddCell(String.Format("���������� ������� ������������� ������� �� {0} ��� ��������� ������", currentDate.Year), "����, ������������ ������� ��� ��� ��������� ������� �� ������� ���������� ���");
            headerLayout.AddCell(String.Format("����� ����������� ����� �� ������ �� {0:dd.MM.yyyy}", currentDate.AddMonths(1)), "����� ����������� �����");
            headerLayout.AddCell(String.Format("�������� ���������� �� {0:dd.MM.yyyy} �� ���� ������� ���������� �������", currentDate.AddMonths(1)), "�������� ���������� �� ���� ������� ���������� �������");
            headerLayout.AddCell(String.Format("������� ������� ������������ ���������� ������� {0} ����", currentDate.Year), "������� ������� ������������ ���������� ������� �� ������� ���������� ���");


            GridHeaderCell fedaralBudgetCell = headerLayout.AddCell(String.Format("����������� ������ {0} ���", currentDate.Year));
            fedaralBudgetCell.AddCell("�������� ����������", "��������� ������������, ���������� �� ������������ ������� �� ������� ���������� ���");
            fedaralBudgetCell.AddCell(String.Format("�������� ���������� �� {0:dd.MM.yyyy} �� ���� ������� ������������ �������", currentDate.AddMonths(1)), "�������� ���������� �� ���� ������� ������������ �������");

            GridHeaderCell localBudgetCell = headerLayout.AddCell(String.Format("������� ������ {0} ���", currentDate.Year));
            localBudgetCell.AddCell("�������� ����������", "��������� ������������, ���������� �� ������������ ������� �� ������� ���������� ���");
            localBudgetCell.AddCell(String.Format("�������� ���������� �� {0:dd.MM.yyyy} �� ���� ������� �������� �������", currentDate.AddMonths(1)), "�������� ���������� �� ���� ������� �������� �������");

            headerLayout.AddCell(String.Format("��������� ������� ������� ��������� �� 01.01.{0}", currentDate.Year + 1), "��������� ������� ������� ���������  �� ������ ���������� ����������� ���� ");

            GridHeaderCell approvedBudgetCell = headerLayout.AddCell(String.Format("���������� �� {0} ���", currentDate.Year + 1));
            approvedBudgetCell.AddCell("���������  ������", "���������� �� �������� ���������� ��� �� ���������� �������");
            approvedBudgetCell.AddCell("����������� ������", "���������� �� �������� ���������� ��� �� ������������ �������");
            approvedBudgetCell.AddCell("������� ������", "���������� �� �������� ������ �� �������� �������");

            approvedBudgetCell = headerLayout.AddCell(String.Format("���������� �� {0} ���", currentDate.Year + 2));
            approvedBudgetCell.AddCell("���������  ������", "���������� �� ��������� ���������� ��� �� ���������� �������");
            approvedBudgetCell.AddCell("����������� ������", "���������� �� ��������� ���������� ��� �� ������������ �������");
            approvedBudgetCell.AddCell("������� ������", "���������� �� ��������� ���������� ��� �� �������� �������");

            headerLayout.AddCell(String.Format("��������� ������� ������� ��������� �� 01.01.{0}", currentDate.Year + 3), "��������� ������� ������� ��������� �� �������� ������");

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
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