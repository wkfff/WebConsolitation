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

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0008
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable tableDt;
        private int firstYear = 2005;
        private int endYear = 2011;
        private DateTime currentDate;

        private GridHeaderLayout headerLayout;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 220);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.EnableViewState = false;

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
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0035_0008_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();
                string day = dtDate.Rows[0][4].ToString();
                currentDate = new DateTime(endYear, CRHelper.MonthNum(month), Convert.ToInt32(day));

                DateTime date = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                CustomCalendar.WebCalendar.SelectedDate = date;
            }

            currentDate = CustomCalendar.WebCalendar.SelectedDate;

            Page.Title = "Аналитическая таблица по ведению графика финансирования кассовых выплат";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Данные приводятся по состоянию на {0:dd.MM.yyyy} года, тыс.руб.", currentDate);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0008_grid");
            tableDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", tableDt);

            if (tableDt.Columns.Count > 0 && tableDt.Rows.Count > 0)
            {
                tableDt.Columns.RemoveAt(0);

                foreach (DataRow row in tableDt.Rows)
                {
                    for (int i = 2; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value && row[i].ToString() != String.Empty)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }

                UltraWebGrid.DataSource = ThrowNullRows(tableDt);
            }
        }

        private static DataTable ThrowNullRows(DataTable dt)
        {
            DataTable newDt = dt.Clone();

            foreach (DataRow row in dt.Rows)
            {
                bool isNotEmptyRow = false;
                for (int i = 2; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && row[i].ToString() != String.Empty)
                    {
                        isNotEmptyRow = true;
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(100);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[0].CellStyle.Padding.Right = 3;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[0], "00 00 00 00 00 00");

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(280);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N2";
                int widthColumn = i < 6 ? 100 : 80;
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            headerLayout.AddCell("Код", "Код показателя");
            headerLayout.AddCell("Наименование показателя", "Наименование показателя");
            headerLayout.AddCell("СБР", "Сводная бюджетная роспись");
            headerLayout.AddCell("График на текущий месяц", "План на текущий месяц");
            headerLayout.AddCell("Исполнение на дату", "Исполнено на дату нарастающим итогом с начала месяца");
            headerLayout.AddCell("Остаток графика на дату", "Остаток графика текущего месяца");

            GridHeaderCell cell = null;
            if (e.Layout.Bands[0].Columns.Count > 6)
            {
                cell = headerLayout.AddCell("в том числе по дням");
            }

            for (int i = 6; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                string[] captionParts = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                if (captionParts.Length > 1)
                {
                    string[] dayParts = captionParts[1].TrimStart(' ').Split(' ');
                    if (dayParts.Length > 1 && cell != null)
                    {
                        DateTime date = new DateTime(currentDate.Year, currentDate.Month, Convert.ToInt32(dayParts[1]));
                        cell.AddCell(String.Format("{0:dd.MM.yyyy}", date), String.Format("Остаток графика на {0:dd.MM.yyyy}", date));
                    }
                }
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
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

            UltraWebGrid.Bands[0].Columns[0].Format = String.Empty;

            ReportExcelExporter1.HeaderCellHeight = 20;
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
}

