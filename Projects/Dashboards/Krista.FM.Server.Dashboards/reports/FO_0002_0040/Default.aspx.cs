using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0040
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable monthReportDt = new DataTable();
        private DataTable yearReportDt = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;
        private DateTime currentDate;
        private DateTime nextMonthDate;

        private List<string> compareMonthList;

        #region Параметры запроса

        // список месяцев для сравнения
        private CustomParam prevYearCompareMonths;
        private CustomParam currYearCompareMonths;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            prevYearCompareMonths = UserParams.CustomParam("prev_year_compare_months");
            currYearCompareMonths = UserParams.CustomParam("curr_year_compare_months");

            #endregion

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 260);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0040_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();
                int monthNum = CRHelper.MonthNum(month);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

                ComboCompareMonth.Title = "Месяцы для сравнения";
                ComboCompareMonth.Width = 230;
                ComboCompareMonth.MultiSelect = true;
                ComboCompareMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboCompareMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(monthNum + 1)), true);
                ComboCompareMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(monthNum + 2)), true);
            }

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, ComboMonth.SelectedIndex + 1, 1);
            nextMonthDate = currentDate.AddMonths(1);

            prevYearCompareMonths.Value = String.Empty;
            currYearCompareMonths.Value = String.Empty;

            compareMonthList = new List<string>();
            foreach (Node node in ComboCompareMonth.SelectedNodes)
            {
                int monthNum = CRHelper.MonthNum(node.Text.Trim(' '));
                compareMonthList.Add(node.Text.Trim(' '));
                prevYearCompareMonths.Value += String.Format("[Период__Период].[Период__Период].[{0}].[Полугодие {1}].[Квартал {2}].[{3}],",
                    currentDate.Year - 1, CRHelper.HalfYearNumByMonthNum(monthNum), CRHelper.QuarterNumByMonthNum(monthNum), CRHelper.RusMonth(monthNum));
                currYearCompareMonths.Value += String.Format("[Период__Период].[Период__Период].[{0}].[Полугодие {1}].[Квартал {2}].[{3}],",
                    currentDate.Year, CRHelper.HalfYearNumByMonthNum(monthNum), CRHelper.QuarterNumByMonthNum(monthNum), CRHelper.RusMonth(monthNum));
            }
            prevYearCompareMonths.Value = prevYearCompareMonths.Value.TrimEnd(',');
            currYearCompareMonths.Value = currYearCompareMonths.Value.TrimEnd(',');

            Page.Title = "Паспорт исполнения консолидированного бюджета";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Показатели исполнения бюджетов Самарской области на {1} год по состоянию на {0:dd.MM.yyyy}, тыс.руб.", nextMonthDate, nextMonthDate.Year);

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0040_grid_monthReport");
            monthReportDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", monthReportDt);

            if (monthReportDt.Columns.Count > 1 && monthReportDt.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0040_grid_yearReport");
                yearReportDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", yearReportDt);

                if (yearReportDt.Columns.Count > 1 && yearReportDt.Rows.Count > 0)
                {
                    yearReportDt.PrimaryKey = new DataColumn[] { yearReportDt.Columns[0] };

                    foreach (DataRow monthRow in monthReportDt.Rows)
                    {
                        string rowName = monthRow[0].ToString();
                        DataRow yearRow = yearReportDt.Rows.Find(rowName);
                        if (yearRow != null)
                        {
                            foreach (DataColumn column in yearReportDt.Columns)
                            {
                                if (column.ColumnName != "Наименование показателей" && monthReportDt.Columns.Contains(column.ColumnName))
                                {
                                    monthRow[column.ColumnName] = yearRow[column.ColumnName];
                                }
                            }

                            // тут пересчитываем темпы роста плана из месОтч к факту из ГодОтч
                            double prevYearFact = double.MinValue;
                            string prevYearFactColumnName = String.Format("{0}; Исполнение конс.бюджета", currentDate.Year - 1);
                            if (yearReportDt.Columns.Contains(prevYearFactColumnName))
                            {
                                if (yearRow[prevYearFactColumnName] != DBNull.Value && yearRow[prevYearFactColumnName].ToString() != String.Empty)
                                {
                                    prevYearFact = Convert.ToDouble(yearRow[prevYearFactColumnName]);
                                }
                            }

                            double currYearPlan = double.MinValue;
                            string currYearPlanColumnName = String.Format("{0}; План конс.бюджета", CRHelper.RusMonth(currentDate.Month));
                            if (monthReportDt.Columns.Contains(prevYearFactColumnName))
                            {
                                if (monthRow[currYearPlanColumnName] != DBNull.Value && monthRow[currYearPlanColumnName].ToString() != String.Empty)
                                {
                                    currYearPlan = Convert.ToDouble(monthRow[currYearPlanColumnName]);
                                }
                            }

                            object rateValue = null;
                            if (prevYearFact != double.MinValue && currYearPlan != double.MinValue && prevYearFact != 0)
                            {
                                rateValue = currYearPlan / prevYearFact;
                            }
                            else
                            {
                                rateValue = DBNull.Value;
                            }

                            string currYearRateColumnName = String.Format("{0}; Темп роста план к факту", CRHelper.RusMonth(currentDate.Month));
                            if (monthReportDt.Columns.Contains(currYearRateColumnName))
                            {
                                monthRow[currYearRateColumnName] = rateValue;
                            }

                            string currYearRate2ColumnName = String.Format("{0}; Темп роста план к факту ожидаемый", CRHelper.RusMonth(currentDate.Month));
                            if (monthReportDt.Columns.Contains(currYearRate2ColumnName))
                            {
                                monthRow[currYearRate2ColumnName] = rateValue;
                            }
                        }
                    }
                }

                UltraWebGrid.DataSource = monthReportDt;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

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
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;

                string formatString = GetColumnFormat(columnCaption);
                int widthColumn = 110;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell("Показатели");
            
            GridHeaderCell prevPrevYearCell = headerLayout.AddCell(String.Format("Отчетный {0} год", currentDate.Year - 2));
            GridHeaderCell executePrevPrevYearCell = prevPrevYearCell.AddCell(String.Format("Исполнение {0} года", currentDate.Year - 2));
            AddBudgetCells(executePrevPrevYearCell);

            GridHeaderCell prevYearCell = headerLayout.AddCell(String.Format("Отчетный {0} год", currentDate.Year - 1));
            GridHeaderCell executePrevYearCell = prevYearCell.AddCell(String.Format("Исполнение {0} года", currentDate.Year - 1));
            AddBudgetCells(executePrevYearCell);
            prevYearCell.AddCell(String.Format("Темп роста к {0} году", currentDate.Year - 2));
            prevYearCell.AddCell(String.Format("Исполнение по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year - 1));

            foreach (string month in compareMonthList)
            {
                prevYearCell.AddCell(String.Format("Исполнение {0}", CRHelper.RusMonthGenitive(CRHelper.MonthNum(month))));
            }

            GridHeaderCell currYearCell = headerLayout.AddCell(String.Format("Отчетный {0} год", currentDate.Year));
            currYearCell.AddCell("План (первоначальный)");
            GridHeaderCell subjectPlanCell = currYearCell.AddCell("Уточненный план по данным субъекта РФ");
            AddBudgetCells(subjectPlanCell);
            currYearCell.AddCell(String.Format("Темп роста уточненного плана к исполнению {0} года", currentDate.Year - 1));
            currYearCell.AddCell(String.Format("Исполнение по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year));
            currYearCell.AddCell(String.Format("Темп роста к {0} году", currentDate.Year - 1));

            foreach (string month in compareMonthList)
            {
                currYearCell.AddCell(String.Format("Исполнение {0}", CRHelper.RusMonthGenitive(CRHelper.MonthNum(month))));
                currYearCell.AddCell(String.Format("Темп роста к {0} году", currentDate.Year - 1));
            }

            GridHeaderCell expectedSubjectPlanCell = currYearCell.AddCell(String.Format("Ожидаемое исполнение {0} года", currentDate.Year));
            AddBudgetCells(expectedSubjectPlanCell);
            currYearCell.AddCell(String.Format("Темп роста уточненного плана к исполнению {0} года", currentDate.Year - 1));

            currYearCell.AddCell(String.Format("Ожидаемое исполнение {0} года по данным Минфина России ({1} {0}г)", currentDate.Year,
                CRHelper.RusMonth(currentDate.Month).ToLower()));
            currYearCell.AddCell("Темп роста");

            GridHeaderCell deviationGroupCell = currYearCell.AddCell("Отклонение");
            deviationGroupCell.AddCell("(+,-)");
            deviationGroupCell.AddCell("%");

            headerLayout.ApplyHeaderInfo();
        }

        private static void AddBudgetCells(GridHeaderCell groupCell)
        {
            groupCell.AddCell("Конс.бюджет");
            groupCell.AddCell("Бюджет субъекта");
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("темп роста"))
            {
                return "P2";
            }
            return "N2";
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
            }

            string level = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex].Value != null)
            {
                level = e.Row.Cells[levelColumnIndex].Value.ToString();
            }

            bool isColSpanningRow = false;
            int colSpanColumnIndex = e.Row.Cells.Count - 2;
            if (e.Row.Cells[colSpanColumnIndex].Value != null)
            {
                isColSpanningRow = Convert.ToBoolean(e.Row.Cells[colSpanColumnIndex].Value.ToString());
            }

            bool isBkIndicatorRow = rowName.ToLower().Contains("размер дефицита бюджета") || rowName.ToLower().Contains("предельный объем") ||
                                rowName.ToLower().Contains("расходы на обслуживание государственного долга");

            for (int i = 0; i < e.Row.Cells.Count - 2; i++)
            {
                string columnName = e.Row.Band.Grid.Columns[i].Header.Caption;

                bool isConsBudget = columnName.ToLower().Contains("консолидир");
                bool rateColumn = columnName.ToLower().Contains("темп роста");

                if (isColSpanningRow && isConsBudget)
                {
                    e.Row.Cells[i].ColSpan = 2;
                    e.Row.Cells[i].Style.Padding.Right = Convert.ToInt32(e.Row.Band.Grid.Columns[i].Width.Value) + 10;
                }

                if (rateColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                    {
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue > 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Наблюдается рост исполнения относительно предыдущего года";
                        }
                        else if (currentValue < 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Наблюдается снижение исполнения относительно предыдущего года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (i > 0 && isBkIndicatorRow && !rateColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                    {
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = "Не соответствует";
                        }
                        else if (currentValue < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = "Соответствует";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                switch (level)
                {
                    case "0":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 10;
                            e.Row.Cells[i].ColSpan = e.Row.Cells.Count - 2;
                            break;
                        }
                    case "1":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 8;
                            break;
                        }
                }
                
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
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
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
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
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}