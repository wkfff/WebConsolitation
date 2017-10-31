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


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0060_03
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
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 150);
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

            CrossLink1.Text = "Численность&nbsp;и&nbsp;расходы&nbsp;на&nbsp;содержание&nbsp;органов&nbsp;государственной&nbsp;власти";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0060_04/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string dimension = "";
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                dtDimension = new DataTable();

                string query = DataProvider.GetQueryText("FO_0002_0060_03_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                string query2 = DataProvider.GetQueryText("FO_0002_0060_03_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Год", dtDimension);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);



            }



            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            DateTime newDate = new DateTime(year, 1, 1);
            dimension = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", newDate, 1);
            UserParams.PeriodDimension.Value = dimension;
            PageTitle.Text = "Численность и расходы на содержание органов местного самоуправления";
            PageSubTitle.Text = string.Format("Данные за {0} год.", (UserParams.PeriodYear.Value.ToString()));

            Page.Title = PageTitle.Text;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0060_03_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальные образования", dt);

           /* decimal sum1 = 0, sum2 = 0;

            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                object val1 = dt.Rows[i]["Плановые расходы на одного работника органа местного самоуправления "];
                object val2 = dt.Rows[i]["Фактические расходы на одного работника органа местного самоуправления "];
                if (val1 != DBNull.Value)
                {
                    sum1 += (decimal)val1;
                }

                if (val2 != DBNull.Value)
                {
                    sum2 += (decimal)val2;
                }
            }

            int indexLastRow = dt.Rows.Count - 1;
            dt.Rows[indexLastRow]["Плановые расходы на одного работника органа местного самоуправления "] = sum1;
            dt.Rows[indexLastRow]["Фактические расходы на одного работника органа местного самоуправления "] = sum2;
            */
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
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N2");

            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(125);
            }
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(100);

            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.HeaderStyleDefault.Wrap = true;

            string year = ComboYear.SelectedValue;
            string lastYear = Convert.ToString(Convert.ToInt16(ComboYear.SelectedValue) - 1);

            headerLayout.AddCell("Муниципальные образования");
            headerLayout.AddCell("Плановые расходы на содержание органов местного самоуправления, тыс. руб.", "Плановые расходы на содержание ОМС");
            headerLayout.AddCell("Фактические расходы на содержание органов местного самоуправления, тыс. руб.", "Фактические расходы на содержание ОМС");
            headerLayout.AddCell("Процент исполнения плановых назначений", "Процент исполнения плана по расходам на содержание ОМС");
            headerLayout.AddCell("Отклонение от плановых назначений, тыс. руб.", "Отклонение от плановых назначений по расходам на содержание ОМС");
            headerLayout.AddCell("Плановая численность, чел.", "Плановая среднегодовая численность ОМС");
            headerLayout.AddCell("Фактическая численность, чел.", "Фактическая среднегодовая численность ОМС");
            headerLayout.AddCell("Плановые расходы на одного работника органа местного самоуправления, тыс. руб.", "Плановые расходы на одного работника ОМС");
            headerLayout.AddCell("Фактические расходы на одного работника органа местного самоуправления, тыс. руб.", "Фактические расходы на одного работника ОМС");
            headerLayout.ApplyHeaderInfo();
            
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            SetRankImg(e.Row, 3);
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

        private static void SetRankImg(UltraGridRow row, int i)
        {
            string css = string.Empty;
            if (row.Cells[i].Value != null)
            {
                if (((Convert.ToDouble(row.Cells[i].Value)) * 100) >= 100)
                {
                    css = "~/images/ballGreenBB.png";
                }
                else
                {
                    css = "~/images/ballRedBB.png";
                }
            }
            if (css != string.Empty)
            {
                row.Cells[i].Style.BackgroundImage = css;
                row.Cells[i].Title = (css == "~/images/ballRedBB.png")
                   ? "Плановые назначения не исполнены, (100%)"
                   : "Плановые назначения исполнены, (100%)";
            }
            row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 30% center; margin: 2px";
        }

        private static bool UglyRank(UltraGridRow row, int i)
        {
            return row.Cells[i] != null && row.Cells[i + 1] != null &&
                   Convert.ToInt32(row.Cells[i].Value) == Convert.ToInt32(row.Cells[i + 1].Value) &&
                   Convert.ToInt32(row.Cells[i].Value) != 0;
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
