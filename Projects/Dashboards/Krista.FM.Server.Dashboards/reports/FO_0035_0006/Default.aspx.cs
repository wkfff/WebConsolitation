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

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0006
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
        private int currentQuarter;
        
        private int beginAdminRowIndex = 0;
        private int endAdminRowIndex = 0;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // множество показателей
        private CustomParam indicatorSet;
        // выбранный квартал
        private CustomParam selectedQuarter;
        // первый месяц выбранного квартала
        private CustomParam selectedQuarterFirstMonth;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 200);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Инициализация параметров запроса

            indicatorSet = UserParams.CustomParam("indicator_set");
            selectedQuarter = UserParams.CustomParam("selected_quarter");
            selectedQuarterFirstMonth = UserParams.CustomParam("selected_quarter_first_month");

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
                string query = DataProvider.GetQueryText("FO_0035_0006_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 160;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters());
                ComboQuarter.SetСheckedState(quarter, true);
            }

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            dtDate = new DataTable();
            string lastMonthQuery = DataProvider.GetQueryText("FO_0035_0006_lastMonth");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(lastMonthQuery, dtDate);
            if (dtDate.Rows.Count > 0)
            {
                currentMonth = dtDate.Rows[0][3].ToString();
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(currentMonth), 1);
            nextMonthDate = currentDate.AddMonths(1);
            currentQuarter = ComboQuarter.SelectedIndex + 1;

            Page.Title = "Квартальный кассовый план исполнения областного бюджета Самарской области";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Кассовый план на {1} квартал {2} года, данные приводятся по состоянию на {0:dd.MM.yyyy}", 
                nextMonthDate, currentQuarter, currentDate.Year);
            
            selectedQuarter.Value = String.Format("Квартал {0}", currentQuarter);
            selectedQuarterFirstMonth.Value = String.Format("Месяц {0}", (currentQuarter - 1) * 3 + 1);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            indicatorSet.Value = "Показатели для первой таблицы";
            string query = DataProvider.GetQueryText("FO_0035_0006_grid_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid1);
            beginAdminRowIndex = dtGrid1.Rows.Count;

            query = DataProvider.GetQueryText("FO_0035_0006_grid_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid2);
            dtGrid1 = MergeDataTables(dtGrid1, dtGrid2);
            endAdminRowIndex = dtGrid1.Rows.Count;

            indicatorSet.Value = "Показатели для третьей таблицы";
            query = DataProvider.GetQueryText("FO_0035_0006_grid_incomes");
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

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString;
                int widthColumn;
                
                if (i == 3 || i == 5 || i == 7)
                {
                    formatString = "P2";
                    widthColumn = 85;
                }
                else
                {
                    formatString = "N2";
                    widthColumn = 140;
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(285);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            
            headerLayout.AddCell("Наименование показателя", "Показатели кассового плана исполнения областного бюджета");
            headerLayout.AddCell(String.Format("{0} квартал, тыс.руб.", GetRomanDigit(currentQuarter)), String.Format("Кассовый план на {0} квартал", currentQuarter));

            int monthIndex = 1;
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                headerLayout.AddCell(
                    String.Format("{0} месяц квартала, тыс.руб.", GetRomanDigit(monthIndex)),
                    String.Format("Кассовый план на {0}, тыс.руб.", CRHelper.RusMonth(GetFirstQuarterMonth(currentQuarter) + monthIndex - 1).ToLower()));
                headerLayout.AddCell("Удельный вес, %", String.Format("Удельный вес плана на {0} месяц квартала по отношению к плану на квартал", monthIndex));
                monthIndex++;
            }

            headerLayout.ApplyHeaderInfo();
        }

        private static int GetFirstQuarterMonth(int quarterIndex)
        {
            return (quarterIndex - 1)*3 + 1;
        }

        private static string GetRomanDigit(int arabDigit)
        {
            switch (arabDigit)
            {
                case 1:
                    {
                        return "I";
                    }
                case 2:
                    {
                        return "II";
                    }
                case 3:
                    {
                        return "III";
                    }
                case 4:
                    {
                        return "IV";
                    }
            }

            return String.Empty;
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