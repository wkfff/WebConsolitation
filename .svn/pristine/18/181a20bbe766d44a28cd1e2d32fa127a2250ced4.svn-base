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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0055
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2005;
        
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight - 250);
            GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
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
                DateTime lastDate = CubeInfoHelper.MonthReportOutcomesInfo.LastDate;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), true);
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            Page.Title = String.Format("Динамика доходов областного бюджета");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на {0:dd.MM.yyy}, тыс.руб.", currentDate.AddMonths(1));

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0055_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 0)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                FontRowLevelRule fontRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                fontRule.AddFontLevel("0", GridBrick.BoldFont8pt);
                fontRule.AddFontLevel("1", GridBrick.ItalicFont8pt);
                GridBrick.AddIndicatorRule(fontRule);

                PaddingRule paddingRule = new PaddingRule(0, "Уровень", 10);
                GridBrick.AddIndicatorRule(paddingRule);

                PerformanceUniformityRule performanceRule = new PerformanceUniformityRule("% исполнения утвержденного плана на год", currentDate.Month);
                GridBrick.AddIndicatorRule(performanceRule);
                performanceRule = new PerformanceUniformityRule("% исполнения кассового плана на год", currentDate.Month);
                GridBrick.AddIndicatorRule(performanceRule);

                GrowRateRule rateRule = new GrowRateRule("Темп роста к аналогичному периоду прошлого года");
                GridBrick.AddIndicatorRule(rateRule);
                
                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
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

            int currentQuarter = CRHelper.QuarterNumByMonthNum(currentDate.Month);

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Доходы (в разрезе групп, подгрупп)");

            GridHeaderCell prevYearCell = headerLayout.AddCell(String.Format("{0} год", currentDate.Year - 1));
            prevYearCell.AddCell("Факт за аналогичный период, тыс.руб.", "Фактическое исполнение за аналогичный период предыдущего года");
            prevYearCell.AddCell("Удельный вес в общей сумме доходов, %", "Удельный вес в общей сумме доходов по фактическому исполнению за аналогичный период предыдущего года");
            prevYearCell.AddCell("Удельный вес в собственных доходах, %", "Удельный вес в собственных доходах по фактическому исполнению за аналогичный период предыдущего года");
            prevYearCell.AddCell("Удельный вес отдельно в налоговых, неналоговых доходах, %", "Удельный вес отдельно в налоговых, неналоговых доходах по фактическому исполнению за аналогичный период предыдущего года");

            GridHeaderCell currYearCell = headerLayout.AddCell(String.Format("{0} год", currentDate.Year));
            currYearCell.AddCell("Утверждено на год, тыс.руб.", "Утвержденный план на год");
            currYearCell.AddCell("Удельный вес в общей сумме доходов, %", "Удельный вес в общей сумме доходов по утвержденному плану на год");
            currYearCell.AddCell("Удельный вес в собственных доходах, %", "Удельный вес в собственных доходах по утвержденному плану на год");
            currYearCell.AddCell("Удельный вес отдельно в налоговых, неналоговых доходах, %", "Удельный вес отдельно в налоговых, неналоговых доходах по утвержденному плану на год");

            currYearCell.AddCell(String.Format("Кассовый план {0}, тыс.руб.", GetQuarterText(currentQuarter)), String.Format("Кассовый план {0}", GetQuarterText(currentQuarter)));
            
            currYearCell.AddCell("Исполнено по отчету за период, тыс.руб.", "Фактическое исполнение нарастающим итогом с начала года");
            currYearCell.AddCell("Удельный вес в общей сумме доходов, %", "Удельный вес в общей сумме доходов по фактическому исполнению за текущий период");
            currYearCell.AddCell("Удельный вес в собственых доходах, %", "Удельный вес в собственных доходах по фактическому исполнению за текущий период");
            currYearCell.AddCell("Удельный вес отдельно в налоговых, неналоговых доходах, %", "Удельный вес отдельно в налоговых, неналоговых доходах по фактическому исполнению за текущий период");
            
            currYearCell.AddCell("Абсолютное отклонение от утвержденного плана на год, тыс.руб.", "Отклонение фактического исполнения от утвержденного плана на год");
            currYearCell.AddCell("% исполнения утвержденного плана на год", "Процент исполнения утвержденного плана на год");
            currYearCell.AddCell("Абсолютное отклонение от кассового плана на год, тыс.руб.", "Отклонение фактического исполнения от кассового плана");
            currYearCell.AddCell("% исполнения кассового плана на год", "Процент исполнения кассового плана");
            currYearCell.AddCell("Абсолютное отклонение от факта за аналогичный период прошлого года, тыс.руб.", "Отклонение фактического исполнения за текущий период от аналогичного периода предыдущего года");
            currYearCell.AddCell("Темп роста к аналогичному периоду прошлого года, %", "Темп роста фактического исполнения за текущий период к аналогичному периоду предыдущего года");

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetQuarterText(int quarterIndex)
        {
            switch (quarterIndex)
            {
                case 1:
                    {
                        return "на 1 квартал";
                    }
                case 2:
                    {
                        return "на полугодие";
                    }
                case 3:
                    {
                        return "на 9 месяцев";
                    }
                case 4:
                    {
                        return "на год";
                    }
            }
            return String.Empty;
        }

        private static string GetColumnFormat(string columnName)
        {
            columnName = columnName.ToLower();
            if (columnName.Contains("удельный вес") || columnName.Contains("темп роста") || columnName.Contains("% исполнения"))
            {
                return "P1";
            }
            return "N1";
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            GridBrick.Grid.DisplayLayout.SelectedRows.Clear(); 
            
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
            ReportPDFExporter1.HeaderCellHeight = 80;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}