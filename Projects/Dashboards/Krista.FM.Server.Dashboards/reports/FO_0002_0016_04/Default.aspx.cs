using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0016_04
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable moGridDt = new DataTable();
        private DateTime currentDate;
        private static MemberAttributesDigest adminDigest;

        #endregion

        #region ��������� �������

        // ��������� ������
        private CustomParam selectedPeriod;
        // ��������� ���������
        private CustomParam selectedAdmin;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� ����� � ��

            //MOGridBrick.BrowserSizeAdapting = true;
            //MOGridBrick.AutoSizeStyle = None;
            MOGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            MOGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);            
            MOGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(MoGrid_InitializeLayout);

            #endregion

            #region ������������� ���������� �������

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedAdmin = UserParams.CustomParam("selected_admin");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.MonthReportIncomesInfo.LastDate;

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2009, lastDate.Year));
                ComboYear.Set�heckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), true);

                ComboAdmin.Title = "���������";
                ComboAdmin.Width = 400;
                ComboAdmin.ParentSelect = true;
                ComboAdmin.MultiSelect = false;
                adminDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0016_04_adminDigest");
                ComboAdmin.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(adminDigest.UniqueNames, adminDigest.MemberLevels));
                ComboAdmin.Set�heckedState("��� ��������������", true);
            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            int month = ComboMonth.SelectedIndex + 2;
            if (month == 13)
            {
                year++;
                month = 1;
            }
            currentDate = new DateTime(year, month, 1);

            Page.Title = String.Format("���������� � ������������ ������������ ����������� �������� ������������� �����������");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("������������ ����������, ������������ �� ������� �������� � ������� ������������� �����������, � ������� ������������� ������������� �������� �� {0:dd.MM.yyyy} �. ({1}), ���. ���.", currentDate,
                ComboAdmin.SelectedValue);
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);
            selectedPeriod.Value = CRHelper.PeriodMemberUName("[������].[������]", currentDate, 4);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            selectedAdmin.Value = adminDigest.GetMemberUniqueName(ComboAdmin.SelectedValue);

            MOGridDataBind();
        }

        #region ����������� ����� � ��

        private void MOGridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0016_04_grid");
            moGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", moGridDt);

            if (moGridDt.Rows.Count > 0 && moGridDt.Columns.Count > 2)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(moGridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", MOGridBrick.BoldFont8pt);
                MOGridBrick.AddIndicatorRule(levelRule);

                MOGridBrick.DataTable = moGridDt;
            }
        }

        protected void MoGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(330);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            GridHeaderLayout headerLayout = MOGridBrick.GridHeaderLayout;
            int ColumnsWidth = 105;
            headerLayout.AddCell("������������ ������������� �����������");
            for (int i = 1; i < columnCount - 1; i = i + 3)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N2");
                //e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                //e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(120);
                //e.Layout.Bands[0].Columns[i + 2].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].Width = ColumnsWidth;
                e.Layout.Bands[0].Columns[i + 1].Width = ColumnsWidth + 15;
                e.Layout.Bands[0].Columns[i + 2].Width = ColumnsWidth;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i + 2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            double sumColumns = e.Layout.Bands[0].Columns[0].Width.Value;
            for (int i = 1; i < columnCount - 1; i = i + 1)
            {

                sumColumns = e.Layout.Bands[0].Columns[i].Width.Value + sumColumns;
                string[] headerParts = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                string groupName = headerParts[0].Trim();

                if (MOGridBrick.GridHeaderLayout.GetChildCellByCaption(groupName) == MOGridBrick.GridHeaderLayout)
                {
                    AddHeaderCellGroup(groupName);
                }
            }
            CRHelper.SaveToErrorLog("���������� �������: " + e.Layout.Bands[0].Columns.Count.ToString() + " ����� ������ �������: " + sumColumns);
            headerLayout.ApplyHeaderInfo();
            MOGridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private void AddHeaderCellGroup(string groupName)
        {
            GridHeaderCell groupCell = MOGridBrick.GridHeaderLayout.AddCell(groupName);
            groupCell.AddCell("���������� �� ���", "���� �� ���");
            groupCell.AddCell("����������������", "���� ����������� ������ � ������ ����");
            groupCell.AddCell("������� �������������� �� ����� �� ���", "������� �������������� �� ����� �� ���");
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
            ReportExcelExporter1.Export(MOGridBrick.GridHeaderLayout, sheet1, 3);
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
            ReportPDFExporter1.Export(MOGridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}