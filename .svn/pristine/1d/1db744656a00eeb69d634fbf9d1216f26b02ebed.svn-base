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
namespace Krista.FM.Server.Dashboards.reports.FO_0035_0012
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private int firstYear = 2012;

        private DateTime currentDate;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 255;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
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
                DateTime lastCubeDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "FO_0035_0012_lastDate");

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

            Page.Title = "Информация по объектам капитального строительства муниципальной собственности";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные на {0:dd.MM.yyyy} г., тыс.руб.", currentDate.AddMonths(1));
            
            GridDataBind();
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0012_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
            levelRule.AddFontLevel("0", GridBrick.BoldFont10pt);
            GridBrick.AddIndicatorRule(levelRule);

            GridBrick.DataTable = gridDt;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnFormat = i < 3 ? "0" : "N2";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], columnFormat);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Наименование направлений и объектов", "Объекты капитального строительства");

            headerLayout.AddCell("Год начала строительства", "Год начала строительства");
            headerLayout.AddCell("Год планируемого ввода", "Год планируемого ввода");

            headerLayout.AddCell(String.Format("Сметная стоимость в ценах 01.01.{0}", currentDate.Year), "Сметная стоимость в ценах на начало года");
            headerLayout.AddCell(String.Format("Выполненный объём на 01.01.{0}", currentDate.Year), "Выполненный объем на начало года");
            headerLayout.AddCell(String.Format("Остатки сметной стоимости в ценах 01.01.{0} г. на 01.01.{0}", currentDate.Year), "Остатки сметной стоимости в ценах на начало года");
            headerLayout.AddCell(String.Format("Кредиторская (-), дебиторская (+) задолженность на {0} год", currentDate.Year), "Кредиторская (-), дебиторская (+) задолженность на начало года");
            headerLayout.AddCell(String.Format("Утверждено Законом Новосибирской области на {0} год областной бюджет", currentDate.Year), "План, утвержденный Законом НСО «Об областном бюджете» на текущий финансовый год");
            headerLayout.AddCell(String.Format("Объём выполненных работ по отчёту на {0:dd.MM.yyyy}", currentDate.AddMonths(1)), "Объем выполненных работ");
            headerLayout.AddCell(String.Format("Кассовое исполнение на {0:dd.MM.yyyy} за счёт средств областного бюджета", currentDate.AddMonths(1)), "Кассовое исполнение за счет средств областного бюджета");
            headerLayout.AddCell(String.Format("Остаток годовых ассигнований областного бюджета {0} года", currentDate.Year), "Остаток годовых ассигнований областного бюджета на текущий финансовый год");


            GridHeaderCell fedaralBudgetCell = headerLayout.AddCell(String.Format("Федеральный бюджет {0} год", currentDate.Year));
            fedaralBudgetCell.AddCell("Плановые назначения", "Бюджетные ассигнования, выделяемые из федерального бюджета на текущий финансовый год");
            fedaralBudgetCell.AddCell(String.Format("Кассовое исполнение на {0:dd.MM.yyyy} за счёт средств федерального бюджета", currentDate.AddMonths(1)), "Кассовое исполнение за счет средств федерального бюджета");

            GridHeaderCell localBudgetCell = headerLayout.AddCell(String.Format("Местный бюджет {0} год", currentDate.Year));
            localBudgetCell.AddCell("Плановые назначения", "Бюджетные ассигнования, выделяемые из федерального бюджета на текущий финансовый год");
            localBudgetCell.AddCell(String.Format("Кассовое исполнение на {0:dd.MM.yyyy} за счёт средств местного бюджета", currentDate.AddMonths(1)), "Кассовое исполнение за счёт средств местного бюджета");

            headerLayout.AddCell(String.Format("Ожидаемый остаток сметной стоимости на 01.01.{0}", currentDate.Year + 1), "Ожидаемый остаток сметной стоимости  на начало очередного финансового года ");

            GridHeaderCell approvedBudgetCell = headerLayout.AddCell(String.Format("Утверждено на {0} год", currentDate.Year + 1));
            approvedBudgetCell.AddCell("Областной  бюджет", "Утверждено на плановый финансовый год по областному бюджету");
            approvedBudgetCell.AddCell("Федеральный бюджет", "Утверждено на плановый финансовый год по федеральному бюджету");
            approvedBudgetCell.AddCell("Местный бюджет", "Утверждено на плановый период по местному бюджету");

            approvedBudgetCell = headerLayout.AddCell(String.Format("Утверждено на {0} год", currentDate.Year + 2));
            approvedBudgetCell.AddCell("Областной  бюджет", "Утверждено на очередной финансовый год по областному бюджету");
            approvedBudgetCell.AddCell("Федеральный бюджет", "Утверждено на очередной финансовый год по федеральному бюджету");
            approvedBudgetCell.AddCell("Местный бюджет", "Утверждено на очередной финансовый год по местному бюджету");

            headerLayout.AddCell(String.Format("Ожидаемый остаток сметной стоимости на 01.01.{0}", currentDate.Year + 3), "Ожидаемый остаток сметной стоимости на плановый период");

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
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