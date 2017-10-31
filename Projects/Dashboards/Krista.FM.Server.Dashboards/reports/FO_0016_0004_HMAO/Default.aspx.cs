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
using Krista.FM.Server.Dashboards.Components;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.UltraGauge.Resources;
using System.Web;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0004_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
  
        private int firstYear = 2011;
        private int endYear = 2012;
        private int selectedQuarterIndex;
        private static Dictionary<string, int> valuesDictionary;

       
        #endregion


        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный год
        private CustomParam selectedYear;
    
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            //GridBrick.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.53);
            //GridBrick.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 2.77);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoWidth;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedYear == null)
            {
                selectedYear = UserParams.CustomParam("selected_year");
            }
            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;мониторинга";
            CrossLink1.NavigateUrl = "~/reports/FO_0016_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Картограмма&nbsp;с&nbsp;результатами&nbsp;мониторинга";
            CrossLink2.NavigateUrl = "~/reports/FO_0016_0002_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Динамика&nbsp;изменения&nbsp;значений&nbsp;показателей&nbsp;мониторинга";
            CrossLink3.NavigateUrl = "~/reports/FO_0016_0003_HMAO/Default.aspx";

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0004_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                valuesDictionary = new Dictionary<string, int>();
                valuesDictionary.Add("Квартал 3", 0);
                valuesDictionary.Add("Квартал 4", 0);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 170;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(valuesDictionary);
                ComboQuarter.SetСheckedState(quarter, true);
           
            }

            selectedYear.Value = string.Format("{0}", ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 3;

            Page.Title = String.Format("Результаты мониторинга соблюдения требований БК РФ в части индикатора \"Отклонение расходов на содержание органов местного самоуправления от установленного норматива формирования данных расходов\" в разрезе поселений");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("По итогам {0} квартала {1} года", selectedQuarterIndex, ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = selectedYear.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            GridDataBind();
        }
 
        #region Обработчики грида

        protected void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0016_0004_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if ((dtGrid.Rows.Count > 0) && (dtGrid.Columns.Count > 3))
            {
                dtGrid.Columns.RemoveAt(0);
                GridBrick.DataTable = dtGrid;
            }
        }


        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            if (e.Layout.Bands[0].Columns.Count <= 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = 200;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = false;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Муниципальное образование";

            e.Layout.Bands[0].Columns[1].Width = 90;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].MergeCells = false;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Тип территории";
        
            e.Layout.Bands[0].Columns[2].MergeCells = false;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            e.Layout.Bands[0].Columns[2].Width = 220;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[2].Header.Caption = "Отклонение расходов на содержание органов местного самоуправления от установленного норматива формирования данных расходов (тыс. руб.)";

            e.Layout.Bands[0].Columns[3].MergeCells = false;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            e.Layout.Bands[0].Columns[3].Width = 100;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[3].Header.Caption = "Количество нарушений";

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Header.Caption);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[2].Header.Caption);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[3].Header.Caption);

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }


        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].ToString() == "МР")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                
            }
            else if (e.Row.Cells[1].ToString() != "ГО")
            {
                e.Row.Cells[0].Text = "   " + e.Row.Cells[0].Text;
            }
        }
      
        #endregion
       
        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            //for (int i = 0; i < GridBrick.Grid.Columns.Count-3; i = i + 2)
            //{
            //    GridBrick.Grid.Rows[0].Cells[2+i].Style.BorderDetails.WidthRight = 0;
            //    GridBrick.Grid.Rows[0].Cells[2+i+1].Style.BorderDetails.WidthLeft = 0;
            //    GridBrick.Grid.Rows[0].Cells[2 + i + 1].Value = GridBrick.Grid.Rows[0].Cells[2 + i].Value;
            //    GridBrick.Grid.Rows[0].Cells[2 + i].Value = DBNull.Value;
            //    GridBrick.Grid.Rows[0].Cells[2 + i + 1].Style.HorizontalAlign = HorizontalAlign.Left;
            //}
       
            ReportPDFExporter1.HeaderCellHeight = 130;
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
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
            ReportExcelExporter1.SheetColumnCount = 4;
          
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.RowsAutoFitEnable = true;
            //for (int i = 0; i < dtGrid.Columns.Count - 3; i = i + 2)
            //{
            //    WorksheetMergedCellsRegion mergedRegion = sheet1.MergedCellsRegions.Add(5, 2+i, 5, 3+i);
            //}
            //sheet1.Columns[GridBrick.Grid.Columns.Count - 1].Hidden = true;
            //GridBrick.Grid.Columns.RemoveAt(GridBrick.Grid.Columns.Count - 1);

            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);          
        }

        #endregion
    }
}