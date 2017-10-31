using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
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

namespace Krista.FM.Server.Dashboards.reports.FST_0002_0001_HMAO
{
    public partial class Default : CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private DateTime compareDate;

        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest periodCompareDigest;
        private static MemberAttributesDigest subjectDigest;
        private static MemberAttributesDigest serviceDigest;

        private bool IsIncrease 
        {
            get { return CompareButtonList.SelectedIndex == 0; }
        }

        #region Параметры запроса

        // текущий период
        private CustomParam currPeriod;
        // период для сравнения
        private CustomParam comparePeriod;
        // выбранная организация
        private CustomParam selectedService;
        // выбранный показатель таблицы
        private CustomParam selectedSubject;
        // рост/снижение
        private CustomParam compareOperator;
        // тип сортировки
        private CustomParam orderType;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            currPeriod = UserParams.CustomParam("period_cur_date");
            comparePeriod = UserParams.CustomParam("period_last_date");
            selectedService = UserParams.CustomParam("selected_service");
            selectedSubject = UserParams.CustomParam("selected_subject");
            compareOperator = UserParams.CustomParam("compare_operator");
            orderType = UserParams.CustomParam("order_type");

            #endregion

            GridBrick.Height = CustomReportConst.minScreenHeight - 210;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 10;
            GridBrick.RedNegativeColoring = false;
            GridBrick.Grid.DisplayLayout.NoDataMessage = String.Empty;

            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected void Grid_DataBound(object sender, EventArgs e)
        {
            double width = 0;
            foreach (UltraGridColumn column in ((UltraWebGrid)sender).Columns)
            {
                width += column.Width.Value;
            }

            ((UltraWebGrid)sender).Width = Convert.ToInt32(width) + 50;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboPeriod.Title = "Период";
                ComboPeriod.Width = 250;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ParentSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "FST_0002_0001_HMAO_periodDigest");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();

                ComboComparePeriod.Title = "Период для сравнения";
                ComboComparePeriod.Width = 320;
                ComboComparePeriod.MultiSelect = false;
                ComboComparePeriod.ParentSelect = false;
                periodCompareDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "FST_0002_0001_HMAO_periodDigest");
                ComboComparePeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodCompareDigest.UniqueNames, periodCompareDigest.MemberLevels));
                ComboComparePeriod.SetСheckedState("Декабрь 2011 года", true);
  
                ComboService.Title = "Услуга";
                ComboService.Width = 250;
                ComboService.MultiSelect = false;
                serviceDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "FST_0002_0001_HMAO_serviceDigest");
                ComboService.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(serviceDigest.UniqueNames, serviceDigest.MemberLevels));
            }

            currPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            comparePeriod.Value = periodCompareDigest.GetMemberUniqueName(ComboComparePeriod.SelectedValue);
            
            currentDate = CRHelper.PeriodDayFoDate(currPeriod.Value);
            compareDate = CRHelper.PeriodDayFoDate(comparePeriod.Value);

            compareOperator.Value = IsIncrease ? ">" : "<";
            orderType.Value = IsIncrease ? "BDESC" : "BASC";
            selectedSubject.Value = "[Территории__РФ].[Территории__РФ].[Ханты-Мансийский автономный округ - Югра]";
            selectedService.Value = serviceDigest.GetMemberUniqueName(ComboService.SelectedValue);

            Page.Title = "Тарифы на коммунальные услуги ХМАО";
            Label1.Text = Page.Title;

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string deviationText = IsIncrease ? "Рост" : "Снижение";
            string deviationGenText = IsIncrease ? "Роста" : "Снижения";
            
            string query = DataProvider.GetQueryText("FST_0002_0001_HMAO_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", gridDt);
            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 0)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                foreach (DataRow row in gridDt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        string[] splitParts = row[0].ToString().Replace("\"", "'").Split(';');
                        row[0] = String.Format("<b>{0}</b><br/>{1}<br/><i>{2}&nbsp;{3}</i>",
                            splitParts[0].Replace("муниципальный район", "МР"), splitParts[1], splitParts[2], splitParts[3]);
                    }
                }

                GrowRateRule growRateRule = new GrowRateRule("Темп прироста, %");
                growRateRule.IncreaseImg = "~/images/ArrowRedUpBB.png";
                growRateRule.DecreaseImg = "~/images/ArrowGreenDownBB.png";
                growRateRule.IncreaseText = "Рост к периоду для сравнения";
                growRateRule.DecreaseText = "Снижение к периоду для сравнения";
                growRateRule.Limit = 0;
                growRateRule.LeftPadding = 5;
                GridBrick.AddIndicatorRule(growRateRule);

                GridBrick.DataTable = gridDt;

                GridBrick.Visible = true;
                CommentTextLabel.Text = String.Format("<b>{1} тарифа</b> на <b>{0}</b> за период <b>{2} {3}</b> г. - <b>{4} {5}</b> г. наблюдался в следующих муниципальных образованиях и организациях",
                    ComboService.SelectedValue.ToLower(), deviationText, 
                    CRHelper.RusMonth(compareDate.Month), compareDate.Year, CRHelper.RusMonth(currentDate.Month), currentDate.Year);
            }
            else
            {
                GridBrick.Visible = false;
                CommentTextLabel.Text = String.Format("<b>{1} тарифа</b> на на <b>{0}</b> за период <b>{2} {3}</b> г. - <b>{4} {5}</b> г. не наблюдалось",
                    ComboService.SelectedValue.ToLower(), deviationGenText,
                    CRHelper.RusMonth(compareDate.Month), compareDate.Year, CRHelper.RusMonth(currentDate.Month), currentDate.Year);
            }
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(450);

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 1; i < columnCount; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption.ToLower();

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(160);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout1 = GridBrick.GridHeaderLayout;
            headerLayout1.AddCell("Организации");
            headerLayout1.AddCell(String.Format("{0}&nbsp;{1}&nbsp;г., руб.", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month)), currentDate.Year));
            headerLayout1.AddCell(String.Format("{0}&nbsp;{1}&nbsp;г., руб.", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(compareDate.Month)), compareDate.Year));
            headerLayout1.AddCell("Абс.откл., руб.");
            headerLayout1.AddCell("Темп прироста, %");
            headerLayout1.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("темп"))
            {
                return "P2";
            }
            return "N2";
        }

        #endregion

        #region Экспорт в Excel

        private void SetExportGrid()
        {
            foreach (UltraGridRow row in GridBrick.Grid.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    string value = row.Cells[0].Value.ToString();
                    value = Regex.Replace(value, "<br[^>]*?>", Environment.NewLine);
                    value = Regex.Replace(value, "<[^>]*?>", String.Empty);
                    row.Cells[0].Value = value.Replace("&nbsp;", " ");
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = CommentTextLabel.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            SetExportGrid();
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
       }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = CommentTextLabel.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            SetExportGrid();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}