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
using Orientation = Infragistics.Documents.Excel.Orientation;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0009
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable gridDt1;
        private DataTable gridDt2;
        private int firstYear = 2005;
        private int endYear = 2011;
        private DateTime currentDate;

        private GridHeaderLayout headerLayout;

        #endregion

        private IndicatorType SelectedIndicatorType
        {
            get
            {
                if (GRBSCheckBox.Checked && KOSGUCheckBox.Checked)
                {
                    return IndicatorType.GRBS_KOSGU;
                }
                else if (GRBSCheckBox.Checked)
                {
                    return IndicatorType.GRBS;
                }
                else
                {
                    return IndicatorType.KOSGU;
                }
            }
        }

        #region Параметры запроса

        // множество индикаторов
        private CustomParam indicatorSet;
        // дата на начало текущего месяца
        private CustomParam monthBeginningPeriod;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 240);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.EnableViewState = false;

            #region Инициализация параметров запроса

            indicatorSet = UserParams.CustomParam("indicator_set");
            monthBeginningPeriod = UserParams.CustomParam("month_beginning_period");

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport +=new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            CustomCalendar.WebPanel.Expanded = false;

            if (!Page.IsPostBack)
            {
                GRBSCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", KOSGUCheckBox.ClientID));
                KOSGUCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", GRBSCheckBox.ClientID));

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0035_0009_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();
                string day = dtDate.Rows[0][4].ToString();
                currentDate = new DateTime(endYear, CRHelper.MonthNum(month), Convert.ToInt32(day));

                DateTime date = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                CustomCalendar.WebCalendar.SelectedDate = date;
            }

            currentDate = CustomCalendar.WebCalendar.SelectedDate;

            Page.Title = "Аналитическая таблица по расчету ожидаемого исполнения областного бюджета";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Данные приводятся по состоянию на {0:dd.MM.yyyy} года, тыс.руб.", currentDate);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            DateTime monthBeginningDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            monthBeginningPeriod.Value = CRHelper.PeriodMemberUName(String.Empty, monthBeginningDate, 5);
            
            switch (SelectedIndicatorType)
            {
                case IndicatorType.GRBS_KOSGU:
                    {
                        indicatorSet.Value = "Список ГРБС-КОСГУ";
                        break;
                    }
                case IndicatorType.GRBS:
                    {
                        indicatorSet.Value = "Список ГРБС";
                        break;
                    }
                case IndicatorType.KOSGU:
                    {
                        indicatorSet.Value = "Список КОСГУ";
                        break;
                    }
            }
            
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0009_grid1");
            gridDt1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", gridDt1);
            
            if (gridDt1.Columns.Count > 0 && gridDt1.Rows.Count > 0)
            {
                gridDt1.Rows.RemoveAt(0);

                query = DataProvider.GetQueryText("FO_0035_0009_grid2");
                gridDt2 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt2);
                
                if (gridDt2.Columns.Count > 1)
                {
                    gridDt2.Columns.RemoveAt(0);
                }

                gridDt1 = ConvertToThsValues(gridDt1);
                gridDt2 = ConvertToThsValues(gridDt2);

                gridDt1 = MergeDataTables(gridDt1, gridDt2);

                UltraWebGrid.DataSource = gridDt1;
            }
        }

        private static DataTable MergeDataTables(DataTable dt1, DataTable dt2)
        {
            DataTable newDT = dt1.Copy();
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = newDT.NewRow();

                newRow[0] = row[0];
                for (int i = 1; i < dt2.Columns.Count; i++)
                {
                    string columnName = dt2.Columns[i].ColumnName;
                    if (newDT.Columns.Contains(columnName))
                    {
                        newRow[columnName] = row[columnName];
                    }
                }
                newDT.Rows.Add(newRow);
            }
            newDT.AcceptChanges();

            return newDT;
        }

        private static DataTable ConvertToThsValues(DataTable dt)
        {
            DataTable newDt = dt.Clone();
            
            foreach (DataRow row in dt.Rows)
            {
                bool isNotEmptyRow = false;
                for (int i = 3; i < row.ItemArray.Length - 1; i++)
                {
                    if (row[i] != DBNull.Value && row[i].ToString() != String.Empty)
                    {
                        isNotEmptyRow = true;

                        double value = Convert.ToDouble(row[i]);

                        if (value == 0)
                        {
                            row[i] = DBNull.Value;
                        }
                        else
                        {

                            string columnName = dt.Columns[i].ColumnName;
                            if (!columnName.Contains("Процент исполнения"))
                            {
                                row[i] = value / 1000;
                            }
                        }
                    }
                }

                if (isNotEmptyRow)
                {
                    newDt.ImportRow(row);
                }
            }

            newDt.AcceptChanges();
            return newDt;
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(55);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            //e.Layout.Bands[0].Columns[1].Hidden = SelectedIndicatorType == IndicatorType.KOSGU;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "#000");

            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(55);
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            //e.Layout.Bands[0].Columns[2].Hidden = SelectedIndicatorType == IndicatorType.GRBS;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");

            for (int i = 3; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;
                string formatString = columnCaption.Contains("Процент исполнения") ? "P2" : "N2";
                int widthColumn = i < 12 ? 120 : 90;
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout.AddCell("Наименование показателя", "Наименование показателя");

            headerLayout.AddCell("КВСР", "КВСР");
            headerLayout.AddCell("КОСГУ", "КОСГУ");
            
            headerLayout.AddCell("Первоначальный кассовый план", "Первоначальный кассовый план на год");
            headerLayout.AddCell("Уточненный кассовый план", "Уточненный кассовый план на год");
            headerLayout.AddCell("Исполнение на начало месяца (с начала года)", "Исполнение на начало месяца нарастающим итогом с начала года");
            headerLayout.AddCell("Процент исполнения КП", "Процент исполнения уточненного годового кассового плана");

            GridHeaderCell cell = headerLayout.AddCell("График текущего месяца");
            cell.AddCell("Первоначальный", "План на текущий месяц: первоначальный");
            cell.AddCell("Уточненный", "План на текущий месяц");
            
            headerLayout.AddCell("Исполнение на дату (с начала месяца)", "Исполнено на дату нарастающим итогом с начала месяца");
            headerLayout.AddCell("Остаток графика", "Остаток графика текущего месяца");

//            if (e.Layout.Bands[0].Columns.Count > 12)
//            {
//                cell = headerLayout.AddCell("График по дням");
//            }
//
//            DateTime currentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
//            for (int i = 11; i < e.Layout.Bands[0].Columns.Count - 1; i++)
//            {
//                
//                string[] captionParts = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
//                if (captionParts.Length > 1)
//                {
//                    string[] dayParts = captionParts[1].TrimStart(' ').Split(' ');
//                    if (dayParts.Length > 1)
//                    {
//                        int day = Convert.ToInt32(dayParts[1]);
//                        if (day < CRHelper.MonthLastDay(currentMonth.Month))
//                        {
//                            DateTime date = new DateTime(currentDate.Year, currentDate.Month, day);
//                            cell.AddCell(String.Format("{0:dd.MM.yyyy}", date), String.Format("Остаток графика на {0:dd.MM.yyyy}", date));
//                        }
//                    }
//                }
//            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString().Replace("_", String.Empty);
                e.Row.Cells[0].Value = indicatorName;
            }

            bool isIncomesDeviationRow = indicatorName.Contains("Отклонение доходов от расходов");

            string level = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex] != null)
            {
                level = e.Row.Cells[levelColumnIndex].ToString();
            }

            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                string columnCaption = e.Row.Cells[i].Column.Header.Caption;
                bool isExecutePercentColumn = columnCaption.Contains("Процент исполнения");

                if (isExecutePercentColumn && isIncomesDeviationRow)
                {
                    e.Row.Cells[i].Value = null;
                }

                switch (level)
                {
                    case "Итоговые группы":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 9;
                            break;
                        }                    
                    case "ГРБС":
                        {
                            if (SelectedIndicatorType == IndicatorType.GRBS_KOSGU)
                            {
                                e.Row.Cells[i].Style.Font.Bold = true;
                                e.Row.Cells[i].Style.Font.Size = 8;
                            }
                            break;
                        }
                    case "Остальные":
                        {
                            e.Row.Cells[i].Style.Font.Bold = false;
                            e.Row.Cells[i].Style.Font.Italic = false;
                            e.Row.Cells[i].Style.Font.Size = 8;
                            break;
                        }
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

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Orientation.Landscape;
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

            ReportPDFExporter1.HeaderCellHeight = 30;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }

    public enum IndicatorType
    {
        GRBS,
        KOSGU,
        GRBS_KOSGU
    }
}

