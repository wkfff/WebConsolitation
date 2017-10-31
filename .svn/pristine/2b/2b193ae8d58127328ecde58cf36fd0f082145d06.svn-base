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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0035_04
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dateDt = new DataTable();
        private DataTable factGridDt = new DataTable();
        private DataTable planGridDt = new DataTable();
        private int firstYear = 2011;

        private DateTime currentDate;
        private DateTime lastYearDay;
        private DateTime lastCubeDate;

        private KbkMemberGenerator kbkGenerator; 

        #endregion

        #region Параметры запроса

        private CustomParam memberDeclaration;
        private CustomParam memberList;
        private CustomParam memberDetailList;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = false;
            CrossLink.Text = "Сравнение&nbsp;плановых&nbsp;показателей по&nbsp;основным&nbsp;параметрам&nbsp;бюджетов";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0032/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Инициализация параметров запроса

            memberDeclaration = UserParams.CustomParam("member_declaration");
            memberList = UserParams.CustomParam("member_list");
            memberDetailList = UserParams.CustomParam("member_detail_list");

            #endregion

            lastCubeDate = CubeInfoHelper.BudgetOutocmesFactInfo.LastDate;

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastCubeDate.Year));
                ComboYear.SetСheckedState(lastCubeDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastCubeDate.Month)), true);
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int monthNum = CRHelper.MonthNum(ComboMonth.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            lastYearDay = GetLastYearDay();

            Page.Title = "Исполнение расходов областного бюджета на капитальные вложения по направлениям и объектам";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные по состоянию на {0:dd.MM.yyyy} года, тыс.руб.", lastYearDay.AddDays(-1));

            GenerateQuery(currentDate.Year);

            GridDataBind();
        }

        private DateTime GetLastYearDay()
        {
            dateDt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0035_04_date");
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
            DescendantsGenerator descendantsGenerator = new DescendantsGenerator("[Мероприятия__АС Бюджет].[Мероприятия__АС Бюджет]",
                String.Format("ФО\\0001 АС Бюджет - УФиНП {0}", yearNum), "Мероприятия уровень 3", "SELF_AND_BEFORE");

            kbkGenerator = new KbkMemberGenerator(DataProvidersFactory.PrimaryMASDataProvider, RegionSettingsHelper.GetReportConfigFullName(),
                descendantsGenerator, "Sum");

            kbkGenerator.GenerateQuery(yearNum);

            memberDeclaration.Value = kbkGenerator.MemberDeclarationListString;
            memberList.Value = kbkGenerator.MemberListString;
            memberDetailList.Value = kbkGenerator.MemberDetailListString;
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0035_04_grid_plan");
            planGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", planGridDt);

            const string summaryBudgetColumnName = "Общий фонд ; Уточненная сводная бюджетная роспись на год";
            const string cashPlanColumnName = "Общий фонд ; Уточненный кассовый план на квартал";
            const string cashFactColumnName = "Общий фонд ; Кассовое исполнение на дату";
            const string summaryBudgetPercentColumnName = "Общий фонд ; % исполнения к уточненной сводной бюджетной росписи";
            const string cashPlanPercentColumnName = "Общий фонд ; % исполнения к уточненному кассовому плану";

            const string foFinancingSummaryBudgetColumnName = "Фонд софинансирования ; Уточненная сводная бюджетная роспись на год";
            const string foFinancingCashPlanColumnName = "Фонд софинансирования ; Уточненный кассовый план на квартал";
            const string foFinancingCashFactColumnName = "Фонд софинансирования ; Кассовое исполнение на дату";
            const string foFinancingSummaryBudgetPercentColumnName = "Фонд софинансирования ; % исполнения к уточненной сводной бюджетной росписи";
            const string foFinancingCashPlanPercentColumnName = "Фонд софинансирования ; % исполнения к уточненному кассовому плану";

            if (planGridDt.Columns.Count > 1 && planGridDt.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0035_04_grid_fact");
                factGridDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", factGridDt);

                if (factGridDt.Columns.Count > 1 && factGridDt.Rows.Count > 0)
                {
                    factGridDt.PrimaryKey = new DataColumn[] { factGridDt.Columns[0] };

                    foreach (DataRow planRow in planGridDt.Rows)
                    {
                        string rowName = planRow[0].ToString();
                        DataRow factRow = factGridDt.Rows.Find(rowName);
                        if (factRow != null)
                        {
                            double cashFact = GetRowValue(factRow, cashFactColumnName);
                            double summaryBudget = GetRowValue(planRow, summaryBudgetColumnName);
                            double cashPlan = GetRowValue(planRow, cashPlanColumnName);

                            planRow[cashFactColumnName] = factRow[cashFactColumnName];

                            if (cashFact != Double.MinValue)
                            {
                                if (summaryBudget != 0 && summaryBudget != Double.MinValue)
                                {
                                    SetRowValue(planRow, summaryBudgetPercentColumnName, cashFact / summaryBudget);
                                }

                                if (cashPlan != 0 && cashPlan != Double.MinValue)
                                {
                                    SetRowValue(planRow, cashPlanPercentColumnName, cashFact / cashPlan);
                                }
                            }
                            else
                            {
                                SetNullRowValue(planRow, summaryBudgetPercentColumnName);
                                SetNullRowValue(planRow, cashPlanPercentColumnName);
                            }

                            double foFinancingCashFact = GetRowValue(factRow, foFinancingCashFactColumnName);
                            double foFinancingSummaryBudget = GetRowValue(planRow, foFinancingSummaryBudgetColumnName);
                            double foFinancingCashPlan = GetRowValue(planRow, foFinancingCashPlanColumnName);

                            planRow[foFinancingCashFactColumnName] = factRow[foFinancingCashFactColumnName];

                            if (foFinancingCashFact != Double.MinValue)
                            {
                                if (foFinancingSummaryBudget != 0 && foFinancingSummaryBudget != Double.MinValue)
                                {
                                    SetRowValue(planRow, foFinancingSummaryBudgetPercentColumnName, foFinancingCashFact / foFinancingSummaryBudget);
                                }

                                if (foFinancingCashPlan != 0 && foFinancingCashPlan != Double.MinValue)
                                {
                                    SetRowValue(planRow, foFinancingCashPlanPercentColumnName, foFinancingCashFact / foFinancingCashPlan);
                                }
                            }
                            else
                            {
                                SetNullRowValue(planRow, foFinancingSummaryBudgetPercentColumnName);
                                SetNullRowValue(planRow, foFinancingCashPlanPercentColumnName);
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
                }

                GridBrick.DataTable = planGridDt;
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

            headerLayout.AddCell("Наименование направлений и объектов");

            headerLayout.AddCell("План, утвержденный законодательно", "План, утвержденный Законом НСО «Об областном бюджете НСО» на год");
            headerLayout.AddCell("Уточненная сводная бюджетная роспись на год", "Уточненный план на год, включая все уведомления по текущий квартал");
            headerLayout.AddCell("Уточненный кассовый план на квартал", "Уточненный кассовый план по текущий квартал");
            headerLayout.AddCell(String.Format("Кассовое исполнение на {0:dd.MM.yyyy} года", lastYearDay.AddDays(-1)), "Кассовое исполнение нарастающим итогом с начала года");
            headerLayout.AddCell("% исполнения к уточненной сводной бюджетной росписи", "Процент исполнения к уточненной сводной бюджетной росписи");
            headerLayout.AddCell("% исполнения к уточненному кассовому плану", "Процент исполнения к уточненному кассовому плану");

            headerLayout.AddCell("План, утвержденный законодательно (фонд софинансирования расходов Новосибирской области)", "План, утвержденный Законом НСО «Об областном бюджете НСО» на год");
            headerLayout.AddCell("Уточненная сводная бюджетная роспись на год (фонд софинансирования расходов Новосибирской области)", "Уточненный план на год, включая все уведомления по текущий квартал");
            headerLayout.AddCell("Уточненный кассовый план на квартал (фонд софинансирования расходов Новосибирской области)", "Уточненный кассовый план по текущий квартал");
            headerLayout.AddCell(String.Format("Кассовое исполнение на {0:dd.MM.yyyy} года (фонд софинансирования расходов Новосибирской области)", lastYearDay.AddDays(-1)), "Кассовое исполнение нарастающим итогом с начала года");
            headerLayout.AddCell("% исполнения к уточненной сводной бюджетной росписи (фонд софинансирования расходов Новосибирской области)", "Процент исполнения к уточненной сводной бюджетной росписи");
            headerLayout.AddCell("% исполнения к уточненному кассовому плану (фонд софинансирования расходов Новосибирской области)", "Процент исполнения к уточненному кассовому плану");

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
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
        
        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

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