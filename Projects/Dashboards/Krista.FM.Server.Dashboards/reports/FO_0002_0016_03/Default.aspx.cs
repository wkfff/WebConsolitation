using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0016_03
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable moGridDt = new DataTable();
        private DataTable kdGridDt = new DataTable();
        private DateTime currentDate;
        private Boolean flag = false;
        #endregion

        #region ��������� �������

        // ��������� ������
        private CustomParam selectedPeriod;
        // ��������� ���������� �������
        private CustomParam selectedGridIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� ����� � ��

            MOGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            MOGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            MOGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            MOGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(MoGrid_InitializeLayout);
            MOGridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(MoGridBrick_ActiveRowChange);
            MOGridBrick.Grid.DataBound += new EventHandler(MoGridBrick_DataBound);

            #endregion

            #region ��������� ����� � ��

            KdGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            KdGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            KdGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            KdGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(KdGrid_InitializeLayout);

            #endregion

            #region ������������� ���������� �������

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(KdGridBrick.Grid.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(KdGridCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MOGridBrick.Grid.ClientID);

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

                selectedGridIndicator.Value = "[����������].[������� �������].[��� ����������].[1000_������� ������� ������������ ������� �� ���]";
                hiddenIndicatorLabel.Text = "���� ��������������� � ������������� ���������� � ������� ������";
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
            Label2.Text = String.Format("������������ ����������, ������������ �� ������� �������� � ������� ������������� ����������� �� ��������� �� {0:dd.MM.yyyy} �., ���. ���.", currentDate);
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);
            selectedPeriod.Value = CRHelper.PeriodMemberUName("[������].[������]", currentDate, 4);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            MOGridDataBind();
        }

        #region ����������� ����� � ��

        private void MOGridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0016_03_moGrid");
            moGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", moGridDt);

            if (moGridDt.Rows.Count > 0)
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

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = MOGridBrick.GridHeaderLayout;

            headerLayout.AddCell("������������ ������������� �����������");

            AddHeaderCellGroup("�����");
            AddHeaderCellGroup("�������");
            AddHeaderCellGroup("���������");
            AddHeaderCellGroup("��������");
            AddHeaderCellGroup("���� ������������ ����������");

            headerLayout.ApplyHeaderInfo();
        }

        private void AddHeaderCellGroup(string groupName)
        {
            GridHeaderCell groupCell = MOGridBrick.GridHeaderLayout.AddCell(groupName);

            groupCell.AddCell("���������� �� ���", "���� �� ���");
            groupCell.AddCell("����������������", "���� ����������� ������ � ������ ����");
            groupCell.AddCell("������� �������������� �� ����� �� ���", "������� �������������� �� ����� �� ���");
        }

        private void MoGridBrick_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(MOGridBrick.Grid, selectedGridIndicator.Value, 0, 0);
                ActivateGridRow(row);
            }
        }

        private void MoGridBrick_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }

            string indicatorName = row.Cells[0].Text;

            hiddenIndicatorLabel.Text = indicatorName;
            selectedGridIndicator.Value = hiddenIndicatorLabel.Text;

            KdGridCaption.Text = String.Format("������������ ���������� ({0})", indicatorName);
            KdGridDataBind();
        }

        #endregion

        #region ����������� ����� � ��

        private void KdGridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0016_03_kdGrid");
            kdGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", kdGridDt);

            if (kdGridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(kdGridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", KdGridBrick.BoldFont10pt);
                levelRule.AddFontLevel("1", KdGridBrick.BoldFont8pt);
                levelRule.AddFontLevel("1", KdGridBrick.ItalicFont8pt);
                KdGridBrick.AddIndicatorRule(levelRule);

                KdGridBrick.DataTable = kdGridDt;
                KdGridBrick.Grid.Bands.Clear();
                KdGridBrick.DataBind();
            }
        }

        protected void KdGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(430);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            CRHelper.SaveToErrorLog(columnCount.ToString());
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            if (!flag)
            {

                GridHeaderLayout headerLayout = KdGridBrick.GridHeaderLayout;

                headerLayout.AddCell("��� ������������ �����������");

                headerLayout.AddCell("���������� �� ���", "���� �� ���");
                headerLayout.AddCell("����������������", "���� ����������� ������ � ������ ����");
                headerLayout.AddCell("������� �������������� �� ����� �� ���", "������� �������������� �� ����� �� ���");

                headerLayout.ApplyHeaderInfo();
                flag = true;
            }
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

            Worksheet sheet1 = workbook.Worksheets.Add("�������1");
            ReportExcelExporter1.Export(MOGridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("�������2");
            ReportExcelExporter1.Export(KdGridBrick.GridHeaderLayout, KdGridCaption.Text, sheet2, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            //ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(MOGridBrick.GridHeaderLayout, Label2.Text, section1);

            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(KdGridCaption.Text);
            ReportPDFExporter1.Export(KdGridBrick.GridHeaderLayout, Label2.Text, section2);
        }

        #endregion
    }
}