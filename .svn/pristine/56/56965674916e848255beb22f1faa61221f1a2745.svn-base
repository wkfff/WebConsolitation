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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0051
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable grbsGridDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2009;
        private static MemberAttributesDigest budgetDigest;
        private static MemberAttributesDigest facilityDigest;

        #endregion

        #region Параметры запроса

        // выбранный бюджет
        private CustomParam selectedBudget;
        // выбранный тип средств
        private CustomParam selectedFacility;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GRBSGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GRBSGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight - 235);
            GRBSGridBrick.Width =  Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            GRBSGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);

            #endregion

            #region Инициализация параметров запроса

            selectedBudget = UserParams.CustomParam("selected_budget");
            selectedFacility = UserParams.CustomParam("selected_facility");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0051_budgetDigest");
            facilityDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0051_facilityDigest");

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.FoMonthReportDebtInfo.LastDate;
 
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboBudget.Title = "Бюджет";
                ComboBudget.Width = 600;
                ComboBudget.MultiSelect = false;
                ComboBudget.ParentSelect = true;
                ComboBudget.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboBudget.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboFacility.Title = "Вид деятельности";
                ComboFacility.Width = 300;
                ComboFacility.MultiSelect = false;
                ComboFacility.ParentSelect = true;
                ComboFacility.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(facilityDigest.UniqueNames, facilityDigest.MemberLevels));
                ComboFacility.SetСheckedState("Все виды деятельности", true);
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            Page.Title = String.Format("Анализ кредиторской задолженности");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{1} ({2}), данные за {0} год", currentDate.Year, ComboBudget.SelectedValue, ComboFacility.SelectedValue);

            selectedBudget.Value = budgetDigest.GetMemberUniqueName(ComboBudget.SelectedValue);
            selectedFacility.Value = facilityDigest.GetMemberUniqueName(ComboFacility.SelectedValue);

            GridDataBind();
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0051_grid");
            grbsGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", grbsGridDt);

            if (grbsGridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(grbsGridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", GRBSGridBrick.BoldFont8pt);
                GRBSGridBrick.AddIndicatorRule(levelRule);

                GRBSGridBrick.DataTable = grbsGridDt;
            }
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(430);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(330);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GRBSGridBrick.GridHeaderLayout;

            headerLayout.AddCell("Наименование", "Счет бюджетного учета");

            headerLayout.AddCell("Сумма задолженности, тыс. руб.", "Общая сумма задолженности");
            headerLayout.AddCell("в том числе просроченная (нереальная к взысканию), тыс. руб.", "Сумма просроченной (нереальной к взысканию) задолженности");

            headerLayout.ApplyHeaderInfo();
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
            ReportExcelExporter1.Export(GRBSGridBrick.GridHeaderLayout, sheet1, 3);
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
            ReportPDFExporter1.Export(GRBSGridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}