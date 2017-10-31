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

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0001_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtCount;
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
  
        private int firstYear = 2011;
        private int endYear = 2012;
        private int selectedQuarterIndex;
        private int range;
        private int indexCol_BK2;
        private int indexCol_BK4;
        private static Dictionary<string, string> indicatorNameList;
       
        #endregion


        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный год
        private CustomParam selectedYear;
        // выбранные районы
        private CustomParam selectedRegion;
    
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            GridBrick.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            GridBrick.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 1.8);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoWidth;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (selectedYear == null)
            {
                selectedYear = UserParams.CustomParam("selected_year");
            }
            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Картограмма&nbsp;с&nbsp;результатами&nbsp;мониторинга";
            CrossLink1.NavigateUrl = "~/reports/FO_0016_0002_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;изменения&nbsp;значений&nbsp;показателей&nbsp;мониторинга";
            CrossLink2.NavigateUrl = "~/reports/FO_0016_0003_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Расходы&nbsp;на&nbsp;содержание&nbsp;ОМСУ&nbsp;в&nbsp;разрезе&nbsp;поселений";
            CrossLink3.NavigateUrl = "~/reports/FO_0016_0004_HMAO/Default.aspx";

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0001_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillMonitoringQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
           
            }

            selectedYear.Value = string.Format("{0}", ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("Результаты мониторинга соблюдения муниципальными районами и городскими округами Ханты-Мансийского автономного округа - Югры требований БК РФ");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("По итогам {0} квартала {1} года", selectedQuarterIndex, ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = selectedYear.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            IndicatorDescriptionDataBind();
            GridDataBind();
        }

        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "Квартал 1":
                    {
                        return "мониторинг за 1 квартал";
                    }
                case "Квартал 2":
                    {
                        return "мониторинг за 2 квартал";
                    }
                case "Квартал 3":
                    {
                        return "мониторинг за 3 квартал";
                    }
                case "Квартал 4":
                    {
                        return "мониторинг за 4 квартал";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region Показатели

        private void IndicatorDescriptionDataBind()
        {
            indicatorNameList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0016_0001_HMAO_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();

                indicatorNameList.Add(code, name);
            }
        }

        #endregion

        

        #region Обработчики грида

        protected void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0016_0001_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if ((dtGrid.Rows.Count > 0) && (dtGrid.Columns.Count > 4))
            {
                dtGrid.Columns.RemoveAt(0);
                GridBrick.DataTable = dtGrid;
                string query2 = DataProvider.GetQueryText("FO_0016_0001_HMAO_indicatorCount");
                dtCount = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Количество", dtCount);
                range = Convert.ToInt32(dtCount.Rows[0][0]);
            }
        }


        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowHeightDefault = 50;
            if (e.Layout.Bands[0].Columns.Count <= 2)
            {
                return;
            }

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i+2)
            {
                if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("БК 2"))
                {
                    indexCol_BK2 = i;
                }
                if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("БК 4"))
                {
                    indexCol_BK4 = i;
                } 

            }
            
            e.Layout.Bands[0].Columns[0].Width = 100;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = false;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Муниципальное образование";

            e.Layout.Bands[0].Columns[1].Width = 60;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].MergeCells = false;

            int widthColumn = 75;
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count-1; i = i+2)
            {
                e.Layout.Bands[0].Columns[i].MergeCells = false;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                e.Layout.Bands[0].Columns[i + 1].MergeCells = false;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                e.Layout.Bands[0].Columns[i + 1].Width = widthColumn;
                e.Layout.Bands[0].Columns[i + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
 
            headerLayout.AddCell("Группа МО");

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count-1; i = i + 2)
            {
                string indicatorCode = e.Layout.Bands[0].Columns[i].Header.Caption;
                indicatorCode = indicatorCode.Remove(4);
                string indicatorName = indicatorCode;
                if (indicatorNameList.ContainsKey(indicatorCode))
                {
                    indicatorName = indicatorNameList[indicatorCode];
                    GridHeaderCell newCell = headerLayout.AddCell(indicatorCode + ' ' + indicatorName);
                    newCell.AddCell("Значение");
                    newCell.AddCell("Количество нарушений");
                }
            }
            //колонка с гейджем
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].MergeCells = false;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Width = 250;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            headerLayout.AddCell("Общее количество нарушений");

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }


        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[1].Value = null;
                for (int i = 2; i < e.Row.Cells.Count; i = i + 2)
                {
                    e.Row.Cells[i].ColSpan = 2;
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[i].Style.Font.Bold = true;
                    if (i == indexCol_BK2)
                    {
                        e.Row.Cells[i].Text += " (0,50)";
                    }
                    if (i == indexCol_BK4)
                    {
                        e.Row.Cells[i].Text += " (0,05)";
                    }
                }
            }
            else
            {
                InitializeGaugeColumn(e);
            }
        }
      
        #endregion

        protected void InitializeGaugeColumn(RowEventArgs e)
        {
            
            int value = Convert.ToInt32(e.Row.Cells[e.Row.Cells.Count - 1].Value);
            
            LinearGaugeIndicator gauge = GaugeDataBind(value, range);
            string namefile = string.Format("LinearGauge_{0}.png", value);
            string path = Server.MapPath("~/TemporaryImages/" + namefile);
            gauge.SaveAsImage(path);
            e.Row.Cells[e.Row.Cells.Count - 1].Style.BackgroundImage = "~/TemporaryImages/" + namefile;
            e.Row.Cells[e.Row.Cells.Count - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: center center; margin: 2px";
            e.Row.Cells[e.Row.Cells.Count - 1].Value = null;
            e.Row.Cells[e.Row.Cells.Count - 1].Title = string.Format("Общее количество нарушений в МО {0} - {1}",e.Row.Cells[0].Text, value);
        }
        
       
        private LinearGaugeIndicator GaugeDataBind(int value, int range)
        {
            LinearGaugeIndicator gauge = (LinearGaugeIndicator)Page.LoadControl("../../Components/Gauges/LinearGaugeIndicator.ascx");

            gauge.Width = 200;
            gauge.Height = 50;
            gauge.SetRange(0, range, 1);
            gauge.MarkerPrecision = 0.01;
            gauge.IndicatorValue = value;
            gauge.TitleText = "";
            gauge.SetImageUrl(0);
            gauge.GaugeContainer.Width = "200px";
            gauge.GaugeContainer.Height = "50px";
            gauge.Tooltip = "Общее количество нарушений";
            gauge.MarkerAnnotation.Visible = false;

            return gauge;
        }
       
        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            for (int i = 0; i < GridBrick.Grid.Columns.Count-3; i = i + 2)
            {
                GridBrick.Grid.Rows[0].Cells[2+i].Style.BorderDetails.WidthRight = 0;
                GridBrick.Grid.Rows[0].Cells[2+i+1].Style.BorderDetails.WidthLeft = 0;
                GridBrick.Grid.Rows[0].Cells[2 + i + 1].Value = GridBrick.Grid.Rows[0].Cells[2 + i].Value;
                GridBrick.Grid.Rows[0].Cells[2 + i].Value = DBNull.Value;
                GridBrick.Grid.Rows[0].Cells[2 + i + 1].Style.HorizontalAlign = HorizontalAlign.Left;
            }
       
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
            ReportExcelExporter1.SheetColumnCount = 20;
          
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.RowsAutoFitEnable = true;
            for (int i = 0; i < dtGrid.Columns.Count - 3; i = i + 2)
            {
                WorksheetMergedCellsRegion mergedRegion = sheet1.MergedCellsRegions.Add(5, 2+i, 5, 3+i);
            }
            sheet1.Columns[GridBrick.Grid.Columns.Count - 1].Hidden = true;
            GridBrick.Grid.Columns.RemoveAt(GridBrick.Grid.Columns.Count - 1);

            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);          
        }

        #endregion
    }
}