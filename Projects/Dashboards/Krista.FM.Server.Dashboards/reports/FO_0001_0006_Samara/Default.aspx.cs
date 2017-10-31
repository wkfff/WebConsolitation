using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0006_Samara
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;

        private int currYear;
        private string currMonth;
        private int currMonthNum;
        private int currDay;
        private DateTime currentDate;
        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранная дата
        private CustomParam selectedPeriod;


        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            //UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.99);
            //UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.4);
            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
     
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0006_Samara_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                currYear = Convert.ToInt32(dtDate.Rows[0][0]);
                currMonth = dtDate.Rows[0][3].ToString();
                currMonthNum = CRHelper.MonthNum(currMonth);
                currDay = Convert.ToInt32(dtDate.Rows[0][4]);

                currentDate = new DateTime(currYear, currMonthNum, currDay);
                ComboCalendar.WebCalendar.SelectedDate = currentDate;
            }
            
            currentDate = ComboCalendar.WebCalendar.SelectedDate;
            //prevDay = currentDate.AddDays(-1);
            currYear = currentDate.Year;
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month).ToUpperFirstSymbol();
            UserParams.PeriodDayFO.Value = currentDate.Day.ToString();

            currDay = currentDate.Day;
            currMonthNum = currentDate.Month;
            selectedPeriod.Value = string.Format("[{0}].[{1}].[{2}].[{3}].[{4}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value,
                UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value, UserParams.PeriodDayFO.Value);

            Page.Title = String.Format("Освоение бюджетных ассигнований, выделенных за счет средств бюджета субъекта");
            PageTitle.Text = Page.Title;

            string day = string.Format("{0}", currDay);
            string month = string.Format("{0}", currMonthNum);
            if (currDay < 10)
            {
                day = string.Format("0{0}", currDay);
            }

            if (currMonthNum < 10)
            {
                month = string.Format("0{0}", currMonthNum);
            }
            PageSubTitle.Text = string.Format("по состоянию на {0}.{1}.{2} года, рублей", day, month, currYear);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0001_0006_Samara_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование источника", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                double prevColSum = 0; // "итого" для предпоследней колонки
                double lastColSum = 0; // "итого" для последней колонки
                int countColumn = dtGrid.Columns.Count;
                for (int i = 0; i < dtGrid.Rows.Count - 1; i++)
                {
                    if (dtGrid.Rows[i][countColumn - 2] != DBNull.Value)
                    {
                        prevColSum += Convert.ToDouble(dtGrid.Rows[i][countColumn - 2]);
                    }
                }
                if (prevColSum != 0)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 1][countColumn - 2] = prevColSum;
                    lastColSum = Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][countColumn - 2]) / Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][1]);
                    dtGrid.Rows[dtGrid.Rows.Count - 1][countColumn - 1] = lastColSum;
                }
                UltraWebGrid.DataSource = dtGrid;  
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(160);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[columnCount - 1], "P1");
            e.Layout.Bands[0].Columns[columnCount - 1].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[columnCount - 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            string day = string.Format("{0}", currDay);
            string month = string.Format("{0}", currMonthNum);
            if (currDay < 10)
            {
                day = string.Format("0{0}", currDay);
            }

            if (currMonthNum < 10)
            {
                month = string.Format("0{0}", currMonthNum);
            }
          
            headerLayout.AddCell("Наименование главных распорядителей средств областного бюджета");
            GridHeaderCell groupCel = headerLayout.AddCell(string.Format("нa {0}.{1}.{2}", day, month, currYear));
            groupCel.AddCell(string.Format("Сводная бюджетная роспись на {0} год", currYear));
            groupCel.AddCell(string.Format("Лимиты бюджетных обязательств на {0} год, доведенные до ГРБС", currYear));
            groupCel.AddCell(string.Format("Кассовое исполнение на {0}.{1}.{2}", day, month, currYear));
            groupCel.AddCell("Процент кассового исполнения к годовой росписи");
           
            headerLayout.ApplyHeaderInfo();

        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
    
            ReportPDFExporter1.HeaderCellHeight = 100;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            

            ReportExcelExporter1.HeaderCellHeight = 100;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion
    }

}