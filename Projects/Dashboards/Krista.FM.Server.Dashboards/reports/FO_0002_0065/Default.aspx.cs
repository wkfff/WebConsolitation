using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0065
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endDate;
        private GridHeaderLayout headerLayout;
        #endregion

        #region ��������� �������


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
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";

            #region ������������� ���������� �������



            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            // �������� ��������� ����
            string query = DataProvider.GetQueryText("FO_0002_0065_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(
                query, dtDate);
            if (!Page.IsPostBack)
            {
                CustomCalendar1.Visible = true;

                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                       Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                // �������������� ���������
                CustomCalendar1.WebCalendar.SelectedDate = date;
            }

            Page.Title = string.Format("������� ���������� �� ���������� ������� �������� �� ���������� 261-��, �� ��������� �� {0:dd.MM.yyyy}, {1}", CustomCalendar1.WebCalendar.SelectedDate, RubMiltiplierButtonList.SelectedValue);
            PageTitle.Text = Page.Title;
            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            DateTime datefirstYear = new DateTime(Convert.ToInt32(dtDate.Rows[0][0].ToString()),1,1);
            UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", datefirstYear, 5);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", CustomCalendar1.WebCalendar.SelectedDate, 5);
            UserParams.PeriodEndYear.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", CustomCalendar1.WebCalendar.SelectedDate, 1);
            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", CustomCalendar1.WebCalendar.SelectedDate, 4);
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("[������__���� ��������].[������__���� ��������].[������ ���� ��������]", CustomCalendar1.WebCalendar.SelectedDate, 1);
            UserParams.KDLevel.Value = rubMultiplier.ToString();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0065_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                dtGrid.Columns.RemoveAt(0);
                dtGrid.Rows[dtGrid.Rows.Count - 1][1] = "�����";
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(45);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(245);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[0], "000");
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i != 6 && i != 8)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                }
                else
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                }
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
            }
            headerLayout.AddCell("����");
            headerLayout.AddCell("�������������");
            headerLayout.AddCell("���� �� ���");
            headerLayout.AddCell("��������������");
            headerLayout.AddCell("�������� ������");
            headerLayout.AddCell("������� �� ����� ����������");
            headerLayout.AddCell("% �������� ������������� �������");
            headerLayout.AddCell("������� �������� ����������");
            headerLayout.AddCell("������� �������� ����������, %");
            headerLayout.ApplyHeaderInfo();

        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[1].Value != null && e.Row.Cells[1].Value.ToString().Contains("�����"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }
        #endregion

        #region ������� � Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}
