using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.Documents.Excel;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0060_05
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtDimension = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2008;
        private int endYear = 2020;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // фильтр по годам
        private CustomParam filterYear;
        // Последний год
        private CustomParam lastYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 180);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            #region Инициализация параметров запроса

            if (filterYear == null)
            {
                filterYear = UserParams.CustomParam("filter_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "Удельный&nbsp;вес&nbsp;расходов&nbsp;на&nbsp;содержание&nbsp;органов&nbsp;гос.&nbsp;власти";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0060_06/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string dimension = "";
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                dtDimension = new DataTable();

                string query2 = DataProvider.GetQueryText("FO_0002_0060_05_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Месяц", dtDimension);
                DateTime maxDate = CRHelper.PeriodDayFoDate(dtDimension.Rows[0][1].ToString());

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, maxDate.Year));
                ComboYear.SetСheckedState(maxDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 140;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(maxDate.Month)), true);

            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            DateTime newDate = new DateTime(year, ComboMonth.SelectedIndex + 1, 1);
            dimension = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", newDate, 4);
            UserParams.PeriodDimension.Value = dimension;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            PageTitle.Text = "Удельный вес расходов на содержание органов местного самоуправления в расходах бюджетов муниципальных образований";
            if (ComboMonth.SelectedIndex + 2 == 13)
            { PageSubTitle.Text = string.Format("Данные на 01.{0:00}.{1} года.", 1, (Convert.ToInt16(UserParams.PeriodYear.Value) + 1).ToString()); }
            else
            { PageSubTitle.Text = string.Format("Данные на 01.{0:00}.{1} года.", ComboMonth.SelectedIndex + 2, UserParams.PeriodYear.Value); }
            Page.Title = PageTitle.Text;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0060_05_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dt);
             decimal itog = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString().ToLower().Contains("итого"))
                {
                    itog = Convert.ToDecimal(dt.Rows[i][1].ToString());
                }
            }
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (itog != 0)
                { dt.Rows[i][8] = Convert.ToDecimal(dt.Rows[i][2].ToString()) / itog; }
            }
            if (dt.Rows.Count > 0 && dt.Rows[0][1] != DBNull.Value)
            {
                UltraWebGrid.DataSource = dt;
            }

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(140);
            }
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(105);
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.HeaderStyleDefault.Wrap = true;

            string year = ComboYear.SelectedValue;
            string lastYear = Convert.ToString(Convert.ToInt16(ComboYear.SelectedValue) - 1);

            headerLayout.AddCell("Муниципальные образования");
            headerLayout.AddCell("Итого расходов, тыс. руб.", "Общая сумма расходов по ОМС");
            GridHeaderCell costs = headerLayout.AddCell("Расходы по содержанию органов местного самоуправления, тыс. руб.");
            costs.AddCell("Всего", "Общая сумма расходов по содержанию органов местного самоуправления");
            GridHeaderCell IncludingCosts = costs.AddCell("В том числе");
            IncludingCosts.AddCell("Оплата труда", "Расходы на оплату труда");
            IncludingCosts.AddCell("Материальные затраты", "Сумма материальных затрат");
            GridHeaderCell specificGravity = headerLayout.AddCell("Удельный вес расходов на содержание ОМС в общем объеме расходов местного бюджета");
            specificGravity.AddCell("Всего", "Удельный вес расходов на содержание ОМС в расходах местного бюджета");
            GridHeaderCell IncludingSpecificGravity = specificGravity.AddCell("В том числе");
            IncludingSpecificGravity.AddCell("Оплата труда", "Удельный вес расходов на оплату труда в общем объеме расходов местного бюджета");
            IncludingSpecificGravity.AddCell("Материальные затраты", "Удельный вес материальных затрат в общем объеме расходов местного бюджета");
            headerLayout.AddCell("Удельный вес расходов на содержание ОМСУ в общем объеме расходов местных бюджетов");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0 && i != 4)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("итого")))
                    {
                        cell.Style.Font.Bold = true;

                    }
                }
            }
        }

        #endregion

        #region Экспорт

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion
    }
}
