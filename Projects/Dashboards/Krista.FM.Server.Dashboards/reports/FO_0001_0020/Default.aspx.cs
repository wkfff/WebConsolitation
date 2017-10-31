using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0020
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2008;
        private int endYear = 2011;
        private DateTime curDate;

        private GridHeaderLayout headerLayout1;

        #endregion

        #region Параметры запроса

        private CustomParam lastYear;
        private CustomParam listQuarts;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            lastYear = UserParams.CustomParam("last_year");
            listQuarts = UserParams.CustomParam("list_quarts");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0020_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                Dictionary<string, int> quarter = new Dictionary<string, int>();
                quarter.Add("Квартал 1", 0);
                quarter.Add("Квартал 2", 0);
                quarter.Add("Квартал 3", 0);
                quarter.Add("Квартал 4", 0);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 150;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(quarter);
                ComboQuarter.SetСheckedState(dtDate.Rows[0][2].ToString(), true);
            }

            Page.Title = "Поквартальный анализ расходов на содержание и численности органов местного самоуправления";
            PageTitle.Text = Page.Title;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            PageSubTitle.Text = string.Format("за {0} {1} {2} года", ComboQuarter.SelectedIndex + 1,
                                              (ComboQuarter.SelectedIndex == 0) ? "квартал" : "квартала",
                                              ComboYear.SelectedValue);

            lastYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodYear.Value = yearNum.ToString();
         
            string list = string.Empty;
            for (int i = 1; i <= ComboQuarter.SelectedIndex + 1; i++)
            {
                list +=string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}],", ComboYear.SelectedValue, CRHelper.HalfYearNumByQuarterNum(i),i);
            }

            listQuarts.Value = list.TrimEnd(',');
            headerLayout1 = new GridHeaderLayout(UltraWebGrid);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
           
            string query = DataProvider.GetQueryText("FO_0001_0020_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
              UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);

            headerLayout1.AddCell("Главный распорядитель бюджетных средств");

            string caption = string.Empty;
            for (int i = 1; i < 16; i+=5 )
            {
              caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                GridHeaderCell cell = headerLayout1.AddCell(string.Format("{0} год ({1})", caption, caption == ComboYear.SelectedValue ? "план" : "факт"));
              cell.AddCell("Количество подведомственных учреждений (шт.)");
              cell.AddCell("Количество гос. гражд. служащих (чел.)");
              cell.AddCell("Количество работников(чел.)");
              cell.AddCell("Расходы на содержание (тыс. рублей)"); 
              cell.AddCell("в том числе: денежное содержание сотрудников (тыс. рублей)");
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+1], "N0");
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+2], "N0");
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+3], "N1");
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+4], "N1");
            }

            int j = 1;
            for (int i = 16; i < e.Layout.Bands[0].Columns.Count; i += 5)
            {
               // caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                GridHeaderCell cell = headerLayout1.AddCell(string.Format("{0} квартал {1} года", j, ComboYear.SelectedValue ));
                j++;
                cell.AddCell("Количество подведомственных учреждений (шт.)");
                cell.AddCell("Количество гос. гражд. служащих (чел.)");
                cell.AddCell("Количество работников (чел.)");
                cell.AddCell("Расходы на содержание (тыс. рублей)");
                cell.AddCell("Денежное содержание сотрудников (тыс. рублей)");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 3], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 4], "N1");
            }

           for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++ )
             {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
             }

            headerLayout1.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
               UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
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

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (UltraWebGrid.Rows.Count < 30)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout1, section1);


        }
      
        #endregion
    }
}
