using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.Documents.Excel;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0060_05
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtDimension = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2008;
        private int endYear = 2020;

        private GridHeaderLayout headerLayout;

        #endregion

        #region ��������� �������

        // ������ �� �����
        private CustomParam filterYear;
        // ��������� ���
        private CustomParam lastYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 180);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
            #region ������������� ���������� �������

            if (filterYear == null)
            {
                filterYear = UserParams.CustomParam("filter_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "��������&nbsp;���&nbsp;��������&nbsp;��&nbsp;����������&nbsp;�������&nbsp;���.&nbsp;������";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0060_06/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string dimension = "";
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                dtDimension = new DataTable();

                string query2 = DataProvider.GetQueryText("FO_0002_0060_05_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "�����", dtDimension);
                DateTime maxDate = CRHelper.PeriodDayFoDate(dtDimension.Rows[0][1].ToString());

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, maxDate.Year));
                ComboYear.Set�heckedState(maxDate.Year.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 140;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(maxDate.Month)), true);

            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            DateTime newDate = new DateTime(year, ComboMonth.SelectedIndex + 1, 1);
            dimension = CRHelper.PeriodMemberUName("[������__������].[������__������]", newDate, 4);
            UserParams.PeriodDimension.Value = dimension;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            PageTitle.Text = "�������� ��� �������� �� ���������� ������� �������� �������������� � �������� �������� ������������� �����������";
            if (ComboMonth.SelectedIndex + 2 == 13)
            { PageSubTitle.Text = string.Format("������ �� 01.{0:00}.{1} ����.", 1, (Convert.ToInt16(UserParams.PeriodYear.Value) + 1).ToString()); }
            else
            { PageSubTitle.Text = string.Format("������ �� 01.{0:00}.{1} ����.", ComboMonth.SelectedIndex + 2, UserParams.PeriodYear.Value); }
            Page.Title = PageTitle.Text;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0060_05_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ����", dt);
             decimal itog = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString().ToLower().Contains("�����"))
                {
                    itog = Convert.ToDecimal(dt.Rows[i][1].ToString());
                }
            }
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (itog != 0)
                { dt.Rows[i][8] = Convert.ToDecimal(dt.Rows[i][2].ToString()) / itog; }
            }
            if (dt.Rows.Count > 0 && dt.Rows[0][1] != DBNull.Value)
            {
                UltraWebGrid.DataSource = dt;
            }

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(140);
            }
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(105);
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.HeaderStyleDefault.Wrap = true;

            string year = ComboYear.SelectedValue;
            string lastYear = Convert.ToString(Convert.ToInt16(ComboYear.SelectedValue) - 1);

            headerLayout.AddCell("������������� �����������");
            headerLayout.AddCell("����� ��������, ���. ���.", "����� ����� �������� �� ���");
            GridHeaderCell costs = headerLayout.AddCell("������� �� ���������� ������� �������� ��������������, ���. ���.");
            costs.AddCell("�����", "����� ����� �������� �� ���������� ������� �������� ��������������");
            GridHeaderCell IncludingCosts = costs.AddCell("� ��� �����");
            IncludingCosts.AddCell("������ �����", "������� �� ������ �����");
            IncludingCosts.AddCell("������������ �������", "����� ������������ ������");
            GridHeaderCell specificGravity = headerLayout.AddCell("�������� ��� �������� �� ���������� ��� � ����� ������ �������� �������� �������");
            specificGravity.AddCell("�����", "�������� ��� �������� �� ���������� ��� � �������� �������� �������");
            GridHeaderCell IncludingSpecificGravity = specificGravity.AddCell("� ��� �����");
            IncludingSpecificGravity.AddCell("������ �����", "�������� ��� �������� �� ������ ����� � ����� ������ �������� �������� �������");
            IncludingSpecificGravity.AddCell("������������ �������", "�������� ��� ������������ ������ � ����� ������ �������� �������� �������");
            headerLayout.AddCell("�������� ��� �������� �� ���������� ���� � ����� ������ �������� ������� ��������");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0 && i != 4)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("�����")))
                    {
                        cell.Style.Font.Bold = true;

                    }
                }
            }
        }

        #endregion

        #region �������

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion
    }
}
