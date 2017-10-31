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

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0005_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedYear;

        private static Dictionary<string, string> indicatorNameList;
       
        private GridHeaderLayout headerLayout;

        #endregion


        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
    
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 25);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.57);
            UltraWebGrid.DisplayLayout.NoDataMessage = String.Empty;
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
      

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;МО";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0002_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Картограмма&nbsp;с&nbsp;результатами&nbsp;оценки";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0003_HMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отдельному&nbsp;показателю";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0004_HMAO/Default.aspx";

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            { 
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0005_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

            }

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedPeriod.Value = string.Format("{0}", selectedYear);

            Page.Title = String.Format("Нарушения БК");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("по итогам {0} года", selectedYear);
     
         
            IndicatorDescriptionDataBind();
          

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

        }

        #region Показатели

        private void IndicatorDescriptionDataBind()
        {
            indicatorNameList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0021_0005_HMAO_indicatorDescription");
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

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0021_0005_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Муниципальное образование";
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
 
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string indicatorCode = e.Layout.Bands[0].Columns[i].Header.Caption;
                int widthColumn = 210;
                string formatNumber = "N0";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatNumber);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell("Соблюдение установленных нормативов формирования расходов на содержание органов местного самоуправления за отчетный финансовый год");
            headerLayout.AddCell("Отношение объема заимствований муниципального образования в отчетном финансовом году к сумме, направляемой в отчетном финансовом году на финансирование дефицита бюджета и (или) погашение долговых обязательств местного бюджета");
            headerLayout.AddCell("Отношение объема расходов на обслуживание муниципального долга к объему расходов бюджета муниципального образования, за исключением объема расходов, которые осуществляются за счет субвенций, предоставляемых из бюджетов бюджетной системы Российской Федерации в отчетном финансовом году");
            headerLayout.AddCell("Отношение дефицита бюджета муниципального образования к общему годовому объему доходов бюджета без учета объема безвозмездных поступлений и поступлений налоговых доходов по дополнительным нормативам отчислений в отчетном финансовом году");
            headerLayout.AddCell("Отношение объема муниципального долга муниципального образования к общему годовому объему доходов бюджета муниципального образования без учета объема безвозмездных поступлений и поступлений налоговых доходов по дополнительным нормативам отчислений");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
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