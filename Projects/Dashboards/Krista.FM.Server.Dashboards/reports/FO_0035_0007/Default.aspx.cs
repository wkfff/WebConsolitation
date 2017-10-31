using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0007
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid1 = new DataTable();
        private DataTable dtGrid2 = new DataTable();
        private DataTable dtGrid3 = new DataTable();
        private int firstYear = 2010;
        private int endYear = 2011;
        private DateTime currentDate;
        private DateTime nextMonthDate;
        private string currentMonth = "Январь";
        
        private int beginAdminRowIndex = 0;
        private int endAdminRowIndex = 0;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // множество показателей
        private CustomParam indicatorSet;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 205);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Инициализация параметров запроса

            indicatorSet = UserParams.CustomParam("indicator_set");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0035_0007_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                currentMonth = dtDate.Rows[0][3].ToString();
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 160;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(currentMonth, true);
            }

            currentMonth = ComboMonth.SelectedValue;
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(currentMonth), 1);
            nextMonthDate = currentDate.AddMonths(1);

            Page.Title = "Исполнение кассового плана";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Данные приводятся по состоянию на {0:dd.MM.yyyy}, тыс.руб.", nextMonthDate);

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
            indicatorSet.Value = "Показатели для первой таблицы";
            string query = DataProvider.GetQueryText("FO_0035_0007_grid_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid1);
            beginAdminRowIndex = dtGrid1.Rows.Count;

            query = DataProvider.GetQueryText("FO_0035_0007_grid_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid2);
            dtGrid1 = MergeDataTables(dtGrid1, dtGrid2);
            endAdminRowIndex = dtGrid1.Rows.Count;

            indicatorSet.Value = "Показатели для третьей таблицы";
            query = DataProvider.GetQueryText("FO_0035_0007_grid_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid3);
            dtGrid1 = MergeDataTables(dtGrid1, dtGrid3);

            if (dtGrid1.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid1;
            }
        }

        private static DataTable MergeDataTables(DataTable dt1, DataTable dt2)
        {
            DataTable newDT = dt1.Copy();
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = newDT.NewRow();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    newRow[i] = row[i];
                }
                newDT.Rows.Add(newRow);
            }
            newDT.AcceptChanges();

            return newDT;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = e.Layout.Bands[0].Columns[i].Header.Caption.Contains("%") ? "P2" : "N2";

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(165);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(285);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            
            headerLayout.AddCell("Наименование показателя", 
                "Показатели кассового плана исполнения областного бюджета");
            headerLayout.AddCell("Сводная бюджетная роспись", 
                "Годовые плановые назначения, утвержденные сводной бюджетной росписью");
            headerLayout.AddCell("Кассовый план", 
                "Кассовый план на год");
            headerLayout.AddCell("Фактическое исполнение", 
                String.Format("Фактическое исполнение по состоянию на {0:dd.MM.yyyy} нарастающим итогом с начала года", nextMonthDate));
            headerLayout.AddCell("% исполнения к годовой росписи",
                "Процент фактического исполнения к плановым назначениям, утвержденным сводной бюджетной росписью");
            headerLayout.AddCell("% исполнения к кассовому плану",
                "Процент фактического исполнения к годовому кассовому плана");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0] != null)
            {
                indicatorName = e.Row.Cells[0].ToString();
            }

            bool isRemains = indicatorName.Contains("Остатки");
            bool boldFont = indicatorName == "Кассовые поступления - всего" ||
                            indicatorName == "Кассовые выплаты - всего" ||
                            indicatorName == "Остатки на едином счете бюджета на начало квартала" ||
                            indicatorName == "Остатки на едином счете бюджета на конец года" ||
                            indicatorName == "Сальдо операций по поступлениям и выплатам";
            
            bool borderLineRow = (e.Row.Index == beginAdminRowIndex || e.Row.Index == endAdminRowIndex);

            if (boldFont)
            {
                e.Row.Style.Font.Bold = true;
            }

            if (isRemains && indicatorName.Contains("на начало квартала"))
            {
                e.Row.Cells[0].Value = indicatorName.Replace("на начало квартала", "на начало года");
            }

            if (isRemains && indicatorName.Contains("на конец квартала"))
            {
                e.Row.Cells[0].Value = indicatorName.Replace("на конец квартала", "на конец отчетного периода");
            }

            if (isRemains && indicatorName.Contains("на конец года"))
            {
                e.Row.Cells[0].Value = indicatorName.Replace("на конец года", "на конец отчетного периода");
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool emptyPercent = (i == 3 || i == 5 || i == 7 || i == 9);

                if (borderLineRow)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 2;
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.Gray;
                    e.Row.Cells[i].Style.BorderDetails.StyleTop = BorderStyle.Solid;
                }

                if (isRemains && emptyPercent)
                {
                    e.Row.Cells[i].Value = String.Empty;
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

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.HeaderCellHeight = 40;
            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}