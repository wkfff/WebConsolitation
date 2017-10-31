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


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0060_01
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
        // ��������� ���
        private CustomParam periodMonth;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 150);
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
            if (periodMonth == null)
            {
                periodMonth = UserParams.CustomParam("period_month");
            }
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "�������&nbsp;��&nbsp;������&nbsp;�����&nbsp;�&nbsp;�������&nbsp;���������������&nbsp;������";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0060_02/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string dimension = "";
            if (!Page.IsPostBack)
            {
                dtDimension = new DataTable();

                string query2 = DataProvider.GetQueryText("FO_0002_0060_01_date");
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

            PageTitle.Text = "������� �� ������ ����� � ������� �������� ��������������";
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
            string query = DataProvider.GetQueryText("FO_0002_0060_01_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������� ������", dt);
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
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(140);
            }

            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.HeaderStyleDefault.Wrap = true;

            string year = ComboYear.SelectedValue;
            string lastYear = Convert.ToString(Convert.ToInt16(ComboYear.SelectedValue) - 1);
            int monthCount = ComboMonth.SelectedIndex + 2;
            string months = CRHelper.RusManyMonthGenitive(monthCount);
            if (monthCount == 13)
            {
                monthCount = 1;
                year = Convert.ToString(Convert.ToInt16(ComboYear.SelectedValue) + 1);
                lastYear = Convert.ToString(Convert.ToInt16(ComboYear.SelectedValue));
            }

            headerLayout.AddCell("������������� �����������");
            headerLayout.AddCell("������� ����������, ���.���.", "������� ���� �� �������� �� ���������� �����");
            headerLayout.AddCell(String.Format("��������� �� 01.{0:00}.{1} ����, ���.���.", monthCount, year), "����������� ���������� ����������� ������ � ������ ����");
            headerLayout.AddCell("������� ����������", "������� ���������� ����� �� �������� �� ���������� �����");
            headerLayout.AddCell("���������� �� ������� ����������, ���. ���.", "���������� �� ������� ����������");
            headerLayout.AddCell(String.Format("��������� �� 01.{0:00}.{1} ����, ���.���.", monthCount, lastYear), "����������� ���������� �� ����������� ������ �������� ����");
            headerLayout.AddCell("���� ����� � �������� ����", "���� ����� ������������ ���������� � ������������ ������� �������� ����");
            headerLayout.AddCell("���������� �� �������� ����, ���. ���.", "���������� �� ������������ ���������� ������������ ������� �������� ����");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            SetRankImg(e.Row, 3, ComboMonth.SelectedIndex + 1);
            SetTempImg(e.Row, 6);

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

        private static void SetTempImg(UltraGridRow row, int i)
        {
            string css = string.Empty;
            if (row.Cells[i].Value != null)
            {
                if ((Convert.ToDouble(row.Cells[i].Value) * 100) > 100)
                { css = "~/images/arrowRedUpBB.png"; }
                else
                {
                    css = "~/images/arrowGreenDownBB.png";
                }
            }
            if (css != string.Empty)
            {
                row.Cells[i].Style.BackgroundImage = css;
                row.Cells[i].Title = (css == "~/images/arrowRedUpBB.png")
                   ? "���� �� ��������� � ����������� ����"
                   : "���������� �� ��������� � ����������� ����";
            }
            row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 30% center; margin: 2px";
        }
        private static void SetRankImg(UltraGridRow row, int i, int monthcount)
        {
            string css = string.Empty;
            double conditionOfUneven = ((monthcount / 12.0) * 100);
            if (row.Cells[i].Value != null)
            {
                if (conditionOfUneven <= (Convert.ToDouble(row.Cells[i].Value) * 100))
                {
                    css = "~/images/ballGreenBB.png";
                }
                else
                {
                    css = "~/images/ballRedBB.png";
                }
            }
            if (css != string.Empty)
            {
                row.Cells[i].Style.BackgroundImage = css;
                row.Cells[i].Title = (css == "~/images/ballGreenBB.png")
                   ? string.Format("����������� ������� �������������, ({0:N2}%)", conditionOfUneven)
                   : string.Format("�� c���������� ������� �������������, ({0:N2}%)", conditionOfUneven);
            }
            row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 30% center; margin: 2px";
        }

        private static bool UglyRank(UltraGridRow row, int i)
        {
            return row.Cells[i] != null && row.Cells[i + 1] != null &&
                   Convert.ToInt32(row.Cells[i].Value) == Convert.ToInt32(row.Cells[i + 1].Value) &&
                   Convert.ToInt32(row.Cells[i].Value) != 0;
        }

        private string MakePeriodMonth(string year, string month)
        {
            int monthNum = CRHelper.MonthNum(month);
            int halfYear = CRHelper.HalfYearNumByMonthNum(monthNum);
            int quater = CRHelper.QuarterNumByMonthNum(monthNum);
            return String.Format("[{0}].[��������� {1}].[������� {2}].[{3}]", year, halfYear, quater, month);
        }

        #endregion

        #region �������

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion
    }
}
