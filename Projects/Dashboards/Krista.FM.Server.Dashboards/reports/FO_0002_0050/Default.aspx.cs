using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0050
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable grbsGridDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2009;
        private static MemberAttributesDigest budgetDigest;

        #endregion

        #region Параметры запроса

        // выбранный бюджет
        private CustomParam selectedBudget;
        // выбранный показатель
        private CustomParam selectedIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GRBSGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.6 - 235);
            GRBSGridBrick.Width =  Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            GRBSGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);

            #endregion

            #region Настройка диаграммы динамики

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight- 100);

            DynamicChartBrick.XAxisLabelFormat = "N0";
            DynamicChartBrick.DataFormatString = "N0";
            DynamicChartBrick.TooltipFormatString = "<SERIES_LABEL>\n<ITEM_LABEL> г.\n<DATA_VALUE:N0>";
            DynamicChartBrick.Legend.Visible = false;
            DynamicChartBrick.ColorModel = ChartColorModel.DefaultFixedColors;
            DynamicChartBrick.TitleTop = "";
            DynamicChartBrick.YAxisExtent = 300;
            DynamicChartBrick.ZeroAligned = true;
            DynamicChartBrick.SeriesLabelWrap = true;

            #endregion

            #region Инициализация параметров запроса

            selectedBudget = UserParams.CustomParam("selected_budget");
            selectedIndicator = UserParams.CustomParam("selected_indicator");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = true;
            CrossLink.Text = "Анализ&nbsp;данных&nbsp;по&nbsp;сетям&nbsp;и&nbsp;штатам";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0049/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0050_budgetDigest");

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.FoYearReportStatesInfo.LastDate;
 
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
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            Page.Title = String.Format("Анализ данных по контингентам");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{1}, данные за {0} год", currentDate.Year, ComboBudget.SelectedValue);
            DynamicChartCaption.Text = "Динамика контингента государственных и муниципальных учреждений и органов власти";

            selectedBudget.Value = budgetDigest.GetMemberUniqueName(ComboBudget.SelectedValue);

            GridDataBind();

            ChartDataBind();
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0050_grbsGrid");
            grbsGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", grbsGridDt);

            if (grbsGridDt.Rows.Count > 0)
            {
               if (grbsGridDt.Columns.Count > 0)
               {
                   grbsGridDt.Columns.RemoveAt(0);
               }

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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 2; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(165);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GRBSGridBrick.GridHeaderLayout;

            headerLayout.AddCell("Вид учреждения");
            headerLayout.AddCell("Контингент");

            headerLayout.AddCell((currentDate.Year - 2).ToString());
            headerLayout.AddCell((currentDate.Year - 1).ToString());
            headerLayout.AddCell((currentDate.Year).ToString());

            headerLayout.ApplyHeaderInfo();
        }

        #endregion


        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0050_dynamicChart");
            dynamicChartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dynamicChartDt);

            if (dynamicChartDt.Rows.Count > 0)
            {
                DynamicChartBrick.DataTable = dynamicChartDt;
                DynamicChartBrick.DataBind();
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
            ReportExcelExporter1.Export(GRBSGridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Динамика");
            ReportExcelExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, sheet2, 3);
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

            ISection section2 = report.AddSection();
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, section2);
        }

        #endregion
    }
}