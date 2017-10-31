using System;
using System.Data;
using System.IO;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0056_02
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable mainGridDt = new DataTable();
        private DataTable creditGridDt = new DataTable();
        private DataTable referenceGridDt = new DataTable();
        private DataTable populationDt = new DataTable();
        private DataTable nonEmptyQueryDt = new DataTable();
        
        private DateTime currentDate;
        private int firstYear = 2012;
        private static MemberAttributesDigest regionDigest;
        private static MemberAttributesDigest conditionDigest;
        private string rubMultiplierCaption;

        #endregion

        #region ��������� �������

        // ��������� ��
        private CustomParam selectedRegion;
        // ��� ���������� ��
        private CustomParam selectedRegionName;
        // ���������
        private CustomParam population;
        // ���������
        private CustomParam selectedCondition;
        // ��������� ��������� ������
        private CustomParam rubMultiplier;

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 1; }
        }

        private bool IsEmptyData
        {
            get
            {
                if (nonEmptyQueryDt != null)
                {
                    return nonEmptyQueryDt.Rows.Count == 0;
                }
                return false;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� ������

            MainGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            MainGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 250);
            MainGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
            MainGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(MainGrid_InitializeLayout);

            CreditGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            CreditGridBrick.Height = 270;
            CreditGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
            CreditGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(CreditGrid_InitializeLayout);

            ReferenceGridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            ReferenceGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight - 250);
            ReferenceGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
            ReferenceGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(ReferenceGrid_InitializeLayout);

            #endregion

            #region ������������� ���������� �������
            
            selectedRegion = UserParams.CustomParam("selected_region");
            population = UserParams.CustomParam("population");
            selectedCondition = UserParams.CustomParam("selected_condition");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            selectedRegionName = UserParams.CustomParam("selected_region_name");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            
            Session["condition"] = null;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            conditionDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0056_02_conditionDigest");

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.MonthReportOutcomesInfo.LastDate;

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.Set�heckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(CRHelper.RusMonth(lastDate.Month).ToUpperFirstSymbol(), true);

                ComboRegion.Title = "��";
                ComboRegion.Width = 300;
                ComboRegion.MultiSelect = false;
                regionDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0056_02_regionDigest");
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(regionDigest.UniqueNames, regionDigest.MemberLevels));
                ComboRegion.Set�heckedState("�.�����������", true);

                ComboCondition.Title = "��������� ������";
                ComboCondition.Width = 300;
                ComboCondition.MultiSelect = false;
                ComboCondition.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(conditionDigest.UniqueNames, conditionDigest.MemberLevels));
                ComboCondition.Set�heckedState("�� ������������ � ���", true);
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodLastDate.Value = currentDate.Month == 12
                                                  ? CRHelper.PeriodMemberUName("[������__������].[������__������]", currentDate, 4)
                                                  : CRHelper.PeriodMemberUName("[������__������].[������__������]", currentDate.AddMonths(1), 4);
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            rubMultiplierCaption = IsThsRubSelected ? "���.���." : "���.";
            rubMultiplier.Value =  IsThsRubSelected ? "1000" : "1 ";

            Page.Title = String.Format("��������� ���������� ���������� �������: {0}", ComboRegion.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = String.Format("�� ��������� �� {0:dd.MM.yyy}, {1}", currentDate.AddMonths(1), rubMultiplierCaption);

            selectedRegion.Value = regionDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
            selectedCondition.Value = conditionDigest.GetMemberUniqueName(ComboCondition.SelectedValue);
            selectedRegionName.Value = ComboRegion.SelectedValue;

            NonEmptyDataBind();

            if (IsEmptyData)
            {
                IsEmptyDataLabel.Text = "������ ��� ������������ ������ �� ������������� ����������� �� ��������� �� ��������� ������.<br/>";
                IsEmptyDataLabel.Visible = true;
                
                MainGridTable.Visible = false;
                CreditGridTable.Visible = false;
                ReferenceGridTable.Visible = false;
            }
            else
            {
                IsEmptyDataLabel.Visible = false;

                MainGridTable.Visible = true;
                CreditGridTable.Visible = true;
                ReferenceGridTable.Visible = true;

                PopulationDataBind();
                MainGridDataBind();
                CreditGridDataBind();
                ReferenceGridDataBind();
            }
        }

        #region ����������� �����

        private void PopulationDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0056_02_population");
            populationDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������ ��", populationDt);

            population.Value = "null";
            if (populationDt.Rows.Count > 0)
            {
                if (populationDt.Rows[0][0] != DBNull.Value)
                {
                    population.Value = populationDt.Rows[0][0].ToString().Replace(",", ".");
                }
            }
        }

        private void MainGridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0056_02_mainGrid");
            mainGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ��", mainGridDt);

            if (mainGridDt.Rows.Count > 0)
            {
                FontRowLevelRule fontRule = new FontRowLevelRule(mainGridDt.Columns.Count - 1);
                fontRule.AddFontLevel("1", MainGridBrick.BoldFont8pt);
                //fontRule.AddFontLevel("-1", MainGridBrick.ItalicFont8pt);
                MainGridBrick.AddIndicatorRule(fontRule);

                PaddingRule paddingRule = new PaddingRule(0, "�������", 10);
                MainGridBrick.AddIndicatorRule(paddingRule);

                GrowRateRule rateRule = new GrowRateRule("���� ����� � ������ �� ���������� ���, %");
                MainGridBrick.AddIndicatorRule(rateRule);

                MainGridBrick.DataTable = mainGridDt;
            }
        }

        private void CreditGridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0056_02_creditGrid");
            creditGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ��", creditGridDt);

            if (creditGridDt.Rows.Count > 0)
            {
                FontRowLevelRule fontRule = new FontRowLevelRule(creditGridDt.Columns.Count - 1);
                fontRule.AddFontLevel("0", MainGridBrick.BoldFont8pt);
                CreditGridBrick.AddIndicatorRule(fontRule);

                PaddingRule paddingRule = new PaddingRule(0, "�������", 10);
                CreditGridBrick.AddIndicatorRule(paddingRule);

                CreditGridBrick.DataTable = creditGridDt;
            }
        }

        private void ReferenceGridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0056_02_referenceGrid");
            referenceGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ��", referenceGridDt);

            if (referenceGridDt.Rows.Count > 0)
            {
                FontRowLevelRule fontRule = new FontRowLevelRule(referenceGridDt.Columns.Count - 1);
                fontRule.AddFontLevel("0", MainGridBrick.BoldFont8pt);
                ReferenceGridBrick.AddIndicatorRule(fontRule);

                PaddingRule paddingRule = new PaddingRule(0, "�������", 10);
                ReferenceGridBrick.AddIndicatorRule(paddingRule);

                ReferenceGridBrick.DataTable = referenceGridDt;
            }
        }

        protected void MainGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(350);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
            }

            GridHeaderLayout headerLayout = MainGridBrick.GridHeaderLayout;
            headerLayout.AddCell("����������");

            GridHeaderCell prevYearCell = headerLayout.AddCell(String.Format("{0} ���", currentDate.Year - 1));
            prevYearCell.AddCell("���� ������������� ������������");
            prevYearCell.AddCell("���������� ���� �� ��� (�� �������� ������)");

            GridHeaderCell prevYearMonthComleteCell = prevYearCell.AddCell("����������");
            prevYearMonthComleteCell.AddCell("��� (������� �����)");
            for (int month = 1; month < 13; month++)
            {
                prevYearMonthComleteCell.AddCell(CRHelper.RusMonth(month));
                if (month % 3 == 0 && month != 12)
                {
                    prevYearMonthComleteCell.AddCell(GetQuarterText(month));
                }
            }

            GridHeaderCell currYearCell = headerLayout.AddCell(String.Format("{0} ���", currentDate.Year));

            GridHeaderCell currBeginPlanCell = currYearCell.AddCell("���� �� ��� (������������� ������������)");
            currBeginPlanCell.AddCell("C����");
            currBeginPlanCell.AddCell("���� ����� � ������ �� ���������� ���, %");

            GridHeaderCell currCorrectPlanCell = currYearCell.AddCell("���������� ���� �� ���");
            currCorrectPlanCell.AddCell("C����");
            currCorrectPlanCell.AddCell("���������� ����������� ����� �� ������������� �������������");

            GridHeaderCell currYearMonthComleteCell = currYearCell.AddCell("����������");
            for (int month = 1; month <= currentDate.Month; month++)
            {
                currYearMonthComleteCell.AddCell(CRHelper.RusMonth(month));
                if (month % 3 == 0)
                {
                    currYearMonthComleteCell.AddCell(GetQuarterText(month));
                }
            }

            if (currentDate.Month == 12)
            {
                headerLayout.AddCell("���� ����� ���������� ���������� ���� � �������� ������ �� ���������� ���, %");
            }
            else
            {
                GridHeaderCell evaluationCell = currYearCell.AddCell("������ ���������� ����������");
                for (int month = currentDate.Month + 1; month < 13; month++)
                {
                    evaluationCell.AddCell(CRHelper.RusMonth(month));
                    if (month % 3 == 0)
                    {
                        evaluationCell.AddCell(GetQuarterText(month));
                    }
                }
                headerLayout.AddCell("���� ����� ��������� ������ ���� � �������� ������ �� ���������� ���, %");
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void CreditGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(350);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
            }

            GridHeaderLayout headerLayout = CreditGridBrick.GridHeaderLayout;
            headerLayout.AddCell("����������");
            
            GridHeaderCell prevYearCell = headerLayout.AddCell(String.Format("���������� {0} ���", currentDate.Year - 1));
            prevYearCell.AddCell(String.Format("�� 01.01.{0} �.", currentDate.Year - 1));
            for (int month = 1; month < 13; month++)
            {
                prevYearCell.AddCell(CRHelper.RusMonth(month));
                if (month % 3 == 0 && month != 12)
                {
                    prevYearCell.AddCell(GetQuarterText(month));
                }
            }

            GridHeaderCell currYearCell = headerLayout.AddCell(String.Format("���������� {0} ���", currentDate.Year));
            currYearCell.AddCell(String.Format("�� 01.01.{0} �.", currentDate.Year));
            for (int month = 1; month <= currentDate.Month; month++)
            {
                currYearCell.AddCell(CRHelper.RusMonth(month));
                if (month % 3 == 0 && month != 12)
                {
                    currYearCell.AddCell(GetQuarterText(month));
                }
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void ReferenceGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(350);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(""));
            }

            GridHeaderLayout headerLayout = ReferenceGridBrick.GridHeaderLayout;
            
            headerLayout.AddCell("����������");
            headerLayout.AddCell(String.Format("�� 01.01.{0} �.", currentDate.Year));
            headerLayout.AddCell(String.Format("�� {0:dd.MM.yyy} �.", currentDate.AddMonths(1)));

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetQuarterText(int monthIndex)
        {
            int quarterIndex = CRHelper.QuarterNumByMonthNum(monthIndex);

            switch (quarterIndex)
            {
                case 1:
                    {
                        return "1 �������";
                    }
                case 2:
                    {
                        return "���������";
                    }
                case 3:
                    {
                        return "9 �������";
                    }
                case 4:
                    {
                        return "���";
                    }
            }
            return String.Empty;
        }

        private static string GetColumnFormat(string columnName)
        {
            columnName = columnName.ToLower();
            if (columnName.Contains("���� �����"))
            {
                return "P2";
            }
            return "N2";
        }

        #endregion

        #region �������� �� �������

        private void NonEmptyDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0056_02_nonEmptyQuery");
            nonEmptyQueryDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ��", nonEmptyQueryDt);
        }

        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            MainGridBrick.Grid.DisplayLayout.SelectedRows.Clear(); 
            
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("������, �������, ���������");
            ReportExcelExporter1.Export(MainGridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("������������ �������������");
            ReportExcelExporter1.Export(CreditGridBrick.GridHeaderLayout, sheet2, 3);

            Worksheet sheet3 = workbook.Worksheets.Add("���������");
            ReportExcelExporter1.Export(ReferenceGridBrick.GridHeaderLayout, sheet3, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(MainGridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            ReportPDFExporter1.Export(CreditGridBrick.GridHeaderLayout, section2);

            ISection section3 = report.AddSection();
            ReportPDFExporter1.Export(ReferenceGridBrick.GridHeaderLayout, section3);
        }

        #endregion
    }
}