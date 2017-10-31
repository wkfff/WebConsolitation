using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components.Components;



namespace Krista.FM.Server.Dashboards.reports.FO_0002_0005_Samara
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DateTime currentDate;
        private DateTime prevDay;
        private GridHeaderLayout headerLayout;

        #endregion


        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedDay;
        private CustomParam selectedYear;
    
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        
            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DisplayLayout.NoDataMessage = String.Empty;
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Инициализация параметров запроса

            if (selectedDay == null)
            {
                selectedDay = UserParams.CustomParam("selected_day");
            }
            if (selectedYear == null)
            {
                selectedYear = UserParams.CustomParam("selected_year");
            }

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                string query = DataProvider.GetQueryText("FO_0002_0005_Samara_date");
                dtDate = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDate);
                
                ComboCalendar.WebCalendar.SelectedDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3).AddDays(0); 
            }
            currentDate = ComboCalendar.WebCalendar.SelectedDate.AddDays(1);
            prevDay = currentDate.AddDays(-1);

            UserParams.PeriodYear.Value = prevDay.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(prevDay.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(prevDay.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(prevDay.Month);
            UserParams.PeriodDayFO.Value = prevDay.Day.ToString();

            selectedYear.Value = string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}]", UserParams.PeriodYear.Value);
            selectedDay.Value = string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}].[{4}]",
                UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value,
                UserParams.PeriodMonth.Value, UserParams.PeriodDayFO.Value);
            Page.Title = String.Format("Безвозмездные поступления и бюджетные кредиты от других бюджетов бюджетной системы РФ в бюджет субъекта");
            PageTitle.Text = Page.Title;
            string day = currentDate.Day.ToString();
            string month = currentDate.Month.ToString();
            if (currentDate.Day < 10)
            {
                day = string.Format("0{0}", currentDate.Day);
            }
            if (currentDate.Month < 10)
            {
                month = string.Format("0{0}", currentDate.Month);
            }
            PageSubTitle.Text = string.Format("На {0}.{1}.{2}, тыс. руб.", day, month, currentDate.Year);


            UltraWebGrid.DataBind();

        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0005_Samara_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                double sum = 0;
                if (dtGrid.Rows[3][2] != DBNull.Value)
                {
                    sum += Convert.ToDouble(dtGrid.Rows[3][2]);
                }
                if (dtGrid.Rows[4][2] != DBNull.Value)
                {
                    sum += Convert.ToDouble(dtGrid.Rows[4][2]);
                }
                if (dtGrid.Rows[5][2] != DBNull.Value)
                {
                    sum += Convert.ToDouble(dtGrid.Rows[5][2]);
                }
                if (dtGrid.Rows[6][2] != DBNull.Value)
                {
                    sum += Convert.ToDouble(dtGrid.Rows[6][2]);
                }
                if (dtGrid.Rows[7][2] != DBNull.Value)
                {
                    sum += Convert.ToDouble(dtGrid.Rows[7][2]);
                }
                if (sum != 0)
                {
                    dtGrid.Rows[2][2] = sum;
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
           
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(700);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Наименование источника";
        
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                e.Layout.Bands[0].Columns[i].Width = 150;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            headerLayout.AddCell(string.Format("Плановые назначения на {0} год", prevDay.Year), string.Format("Годовые назначения на {0} год", currentDate.Year));
            string day = currentDate.Day.ToString();
            string month = currentDate.Month.ToString();
            if (currentDate.Day < 10)
            {
                day = string.Format("0{0}", currentDate.Day);
            }
            if (currentDate.Month < 10)
            {
                month = string.Format("0{0}", currentDate.Month);
            }
            headerLayout.AddCell(string.Format("Факт поступлений на {0}.{1}.{2}", day, month, currentDate.Year), string.Format("Фактическое исполнение на {0}.{1}.{2} года", currentDate.Day, currentDate.Month, currentDate.Year));
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if ((e.Row.Index < 3) || (e.Row.Index > 7))
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            if (e.Row.Cells[0].Text == "Сумма")
            {
                e.Row.Cells[0].Text = "Безвозмездные поступления из федерального бюджета и федеральных фондов, в том числе";
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
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
           
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.SheetColumnCount = 20;
             
            ReportExcelExporter1.GridColumnWidthScale = 1.0;
            ReportExcelExporter1.RowsAutoFitEnable = true;
      
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion
    }
}