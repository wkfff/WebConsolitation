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
using Krista.FM.Server.Dashboards.Core.QueryGenerators;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0035_06
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dateDt = new DataTable();
        private DataTable factGridDt = new DataTable();
        private DataTable planGridDt = new DataTable();
        private int firstYear = 2011;

        private DateTime currentDate;
        private DateTime lastYearDay;
        private DateTime lastCubeDate;

        private KbkMemberGenerator kbkGenerator; 

        #endregion

        #region ��������� �������

        private CustomParam memberDeclaration;
        private CustomParam memberList;
        private CustomParam memberDetailList;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����

            GridBrick.Height = CustomReportConst.minScreenHeight - 255;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region ������������� ���������� �������

            memberDeclaration = UserParams.CustomParam("member_declaration");
            memberList = UserParams.CustomParam("member_list");
            memberDetailList = UserParams.CustomParam("member_detail_list");

            #endregion

            lastCubeDate = CubeInfoHelper.BudgetOutocmesFactInfo.LastDate;

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastCubeDate.Year));
                ComboYear.Set�heckedState(lastCubeDate.Year.ToString(), true);

            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            lastYearDay = GetLastYearDay();

            Page.Title = "������ ���������� �������� ���������� ������� �� ����������� �������� �� ������������ � ��������";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("������ �� {0} ���", lastYearDay.Year);

            GenerateQuery(currentDate.Year);

            GridDataBind();
        }

        private DateTime GetLastYearDay()
        {
            dateDt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0035_06_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dateDt);

            if (dateDt.Rows.Count > 0)
            {
                if (dateDt.Rows[0][1] != DBNull.Value && dateDt.Rows[0][1].ToString() != String.Empty)
                {
                    return CRHelper.PeriodDayFoDate(dateDt.Rows[0][1].ToString());
                }
            }

            return lastCubeDate;
        }
        
        private void GenerateQuery(int yearNum)
        {
            DescendantsGenerator descendantsGenerator = new DescendantsGenerator("[�����������__�� ������].[�����������__�� ������]",
                String.Format("��\\0001 �� ������ - ����� {0}", yearNum), "����������� ������� 3", "SELF_AND_BEFORE");

            kbkGenerator = new KbkMemberGenerator(DataProvidersFactory.PrimaryMASDataProvider, RegionSettingsHelper.GetReportConfigFullName(),
                descendantsGenerator, "Sum");

            kbkGenerator.GenerateQuery(yearNum);

            memberDeclaration.Value = kbkGenerator.MemberDeclarationListString;
            memberList.Value = kbkGenerator.MemberListString;
            memberDetailList.Value = kbkGenerator.MemberDetailListString;
        }

        #region ����������� �����
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0035_06_grid_plan");
            planGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", planGridDt);

            if (planGridDt.Columns.Count > 1 && planGridDt.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0035_06_grid_fact");
                factGridDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", factGridDt);

                if (factGridDt.Columns.Count > 1 && factGridDt.Rows.Count > 0)
                {
                    factGridDt.PrimaryKey = new DataColumn[] { factGridDt.Columns[0] };

                    foreach (DataRow planRow in planGridDt.Rows)
                    {
                        string rowName = planRow[0].ToString();
                        DataRow factRow = factGridDt.Rows.Find(rowName);
                        if (factRow != null)
                        {
                            for (int quarter = 1; quarter <= 4; quarter++)
                            {
                                CalculatePercents(factRow, planRow, quarter);
                            }
                        }
                    }
                }

                if (planGridDt.Columns.Count > 1)
                {
                    foreach (DataRow row in planGridDt.Rows)
                    {
                        if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                        {
                            string indicatorName = row[0].ToString().TrimEnd('_');
                            row[0] = indicatorName;
                            row[planGridDt.Columns.Count - 1] = kbkGenerator.GetIndicatorLevel(indicatorName);
                        }
                    }

                    FontRowLevelRule rule = new FontRowLevelRule(planGridDt.Columns.Count - 1);
                    rule.AddFontLevel("0", GridBrick.BoldFont10pt);
                    rule.AddFontLevel("1", GridBrick.BoldFont8pt);
                    GridBrick.AddIndicatorRule(rule);

                    for (int columnIndex = 1; columnIndex < planGridDt.Columns.Count; columnIndex++)
                    {
                        string columnName = planGridDt.Columns[columnIndex].ColumnName;

                        if (columnName.Contains("%"))
                        {
                            if (columnName.Contains("��������� �����"))
                            {
                                GrowRateRule growRateRule = new GrowRateRule(columnIndex, "���� ��������", "���� �� ��������");
                                growRateRule.IncreaseImg = "~/images/ballGreenBB.png";
                                growRateRule.DecreaseImg = "~/images/ballRedBB.png";
                                growRateRule.Limit = 0.99999999999;
                                GridBrick.AddIndicatorRule(growRateRule);
                            }
                            else
                            {
                                int quarterIndex = (columnIndex - 1) / 4 + 1;
                                PerformanceUniformityRule performanceRule = new PerformanceUniformityRule(columnIndex, CRHelper.QuarterLastMonth(quarterIndex));
                                GridBrick.AddIndicatorRule(performanceRule);
                            }
                        }
                    }
                }

                GridBrick.DataTable = planGridDt;
            }
        }

        const string cashPlanColumnName = "���������� �������� ���� �� �������";
        const string yearCashPlanColumnName = "���������� ������� ��������� ������� �� ���";
        const string cashFactColumnName = "�������� ���������� �� ����";
        const string cashPlanPercentColumnName = "% ���������� � ����������� ��������� �����";
        const string summaryBudgetPercentColumnName = "% ���������� � ���������� ������� ��������� �������";

        private static void CalculatePercents(DataRow factRow, DataRow planRow, int quaterIndex)
        {
            string quarterName = String.Format("������� {0}; ", quaterIndex);

            double cashFact = GetRowValue(factRow, quarterName + cashFactColumnName);
            double summaryBudget = GetRowValue(planRow, quarterName + summaryBudgetPercentColumnName);
            double cashPlan = quaterIndex == 4 ? GetRowValue(planRow, quarterName + yearCashPlanColumnName) : GetRowValue(planRow, quarterName + cashPlanColumnName);

            planRow[quarterName + cashFactColumnName] = factRow[quarterName + cashFactColumnName];

            if (cashFact != Double.MinValue)
            {
                if (summaryBudget != 0 && summaryBudget != Double.MinValue && planRow.Table.Columns.Contains(quarterName + summaryBudgetPercentColumnName))
                {
                    SetRowValue(planRow, quarterName + summaryBudgetPercentColumnName, cashFact / summaryBudget);
                }

                if (cashPlan != 0 && cashPlan != Double.MinValue)
                {
                    SetRowValue(planRow, quarterName + cashPlanPercentColumnName, cashFact / cashPlan);
                }
            }
            else
            {
                SetNullRowValue(planRow, quarterName + summaryBudgetPercentColumnName);
                SetNullRowValue(planRow, quarterName + cashPlanPercentColumnName);
            }
        }

        private static double GetRowValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return Convert.ToDouble(row[columnName]);
                }
            }

            return Double.MinValue;
        }

        private static void SetRowValue(DataRow row, string columnName, double value)
        {
            if (row.Table.Columns.Contains(columnName) && value != Double.MinValue)
            {
                row[columnName] = value;
            }
        }

        private static void SetNullRowValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                row[columnName] = DBNull.Value;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(360);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;

                string formatString = GetColumnFormat(columnCaption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(150);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("������������ ����������� � ��������");

            for (int quarter = 1; quarter <= 3; quarter++)
            {
                string quarterText = GetQuarterText(quarter, currentDate.Year);

                headerLayout.AddCell(String.Format("���������� �������� ���� {0}, ���.���.", quarterText), String.Format("���������� ���� {0}", quarterText));
                headerLayout.AddCell(String.Format("�������� ���������� {0}, ���.���.", quarterText), String.Format("�������� ���������� {0}", quarterText));
                headerLayout.AddCell(String.Format("% ���������� {0} � ����������� ��������� ����� {0}", quarterText),
                    "������� ���������� � ����������� ��������� �����");
                headerLayout.AddCell(String.Format("% ���������� {0} � ����������  ������� ��������� ������� �� {1} ���", quarterText, currentDate.Year), 
                    "������� ���������� � ���������� ������� ��������� �������");
            }

            headerLayout.AddCell(String.Format("���������� ������� ��������� ������� �� {0} ���, ���.���.", currentDate.Year),
                String.Format("���������� ������� ��������� ������� �� {0} ���", currentDate.Year));
            headerLayout.AddCell(String.Format("�������� ���������� �� {0} ���, ���.���.", currentDate.Year),
                String.Format("�������� ���������� �� {0} ���", currentDate.Year));
            headerLayout.AddCell(String.Format("% ���������� � ����������  ������� ��������� ������� �� {0} ���", currentDate.Year),
                "������� ���������� � ���������� ������� ��������� �������");

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private static string GetQuarterText(int quarterIndex, int year)
        {
            switch (quarterIndex)
            {
                case 1:
                    {
                        return String.Format("�� 1 ������� {0} ����", year);
                    }
                case 2:
                    {
                        return String.Format("�� 1 ��������� {0} ����", year);
                    }
                case 3:
                    {
                        return String.Format("�� 9 ������� {0} ����", year);
                    }
                default:
                    {
                        return String.Format("�� {0} ���", year);
                    }
            }
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("%"))
            {
                return "P2";
            }
            return "N2";
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