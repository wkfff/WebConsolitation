using System;
using System.Data;
using System.Drawing;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0041
{
    public partial class Default : CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            GridBrick.Height = CustomReportConst.minScreenHeight - 260;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;

            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboCalendar.WebCalendar.SelectedDate = CubeInfoHelper.BudgetIncomesFactInfo.LastDate.AddDays(-1);
            }

            currentDate = ComboCalendar.WebCalendar.SelectedDate.AddDays(1);

            Page.Title = "Исполнение областного бюджета по доходам";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные приводятся по состоянию на {0:dd.MM.yyyy}, тыс.руб.", currentDate);

            DateTime prevDay = currentDate.AddDays(-1);

            UserParams.PeriodYear.Value = prevDay.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(prevDay.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(prevDay.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(prevDay.Month);
            UserParams.PeriodDayFO.Value = prevDay.Day.ToString();

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0041_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            GridBrick.DataTable = gridDt;

            if (gridDt.Columns.Count > 1)
            {
                FontRowLevelRule rule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                rule.AddFontLevel("0", GridBrick.BoldFont8pt);
                GridBrick.AddIndicatorRule(rule);
            }
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(220);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;

                string formatString = GetColumnFormat(columnCaption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(145);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Показатели");
            
            GridHeaderCell prevYearCell = headerLayout.AddCell(String.Format("{0} год", currentDate.AddDays(-1).Year - 1));
            prevYearCell.AddCell(String.Format("Факт {0} год", currentDate.AddDays(-1).Year - 1),
                String.Format("Фактическое исполнение {0} год", currentDate.AddDays(-1).Year - 1));
            prevYearCell.AddCell(String.Format("Факт на {0:dd.MM.yyyy}", currentDate.AddYears(-1)),
                String.Format("Фактическое исполнение на {0:dd.MM.yyyy}", currentDate.AddYears(-1)));

            GridHeaderCell currYearCell = headerLayout.AddCell(String.Format("{0} год", currentDate.AddDays(-1).Year));
            currYearCell.AddCell(String.Format("План {0} года", currentDate.AddDays(-1).Year),
                String.Format("Годовые назначения на {0} год", currentDate.AddDays(-1).Year));
            currYearCell.AddCell(String.Format("Факт на {0:dd.MM.yyyy}", currentDate),
                String.Format("Фактическое исполнение на {0:dd.MM.yyyy}", currentDate));
            currYearCell.AddCell(String.Format("% исполнения к годовому плану {0} года", currentDate.AddDays(-1).Year),
                String.Format("Процент исполнения годового плана в {0} году", currentDate.AddDays(-1).Year));

            headerLayout.AddCell(String.Format("% исп-я к {0:dd.MM.yyyy}", currentDate.AddYears(-1)),
                String.Format("Процент поступлений в текущем году к аналогичному периоду предыдущего года"));

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("% исп-я"))
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