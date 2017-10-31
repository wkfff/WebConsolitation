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

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0001_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtNameRegion;
        private int firstYear = 2010;
        private int endYear = 2012;
        private int selectedPeriod;
        private int columnCount;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedYear;
        // множество индикаторв
        private CustomParam indicatorSet;
        // выбранная мера
        private CustomParam selectedMeasure;

        #endregion

        private MeasureType MeasureType
        {
            get
            {
                switch (MeasureButtonList.SelectedIndex)
                {
                    
                    case 0:
                        {
                            return MeasureType.Evaluation;
                        }
                    default:
                        {
                            return MeasureType.Value;
                        }
                }
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.4);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            selectedYear = UserParams.CustomParam("selected_year");
            indicatorSet = UserParams.CustomParam("indicator_set");
            selectedMeasure = UserParams.CustomParam("selected_measure");
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Рейтинг&nbsp;МО";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0002_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Картограмма&nbsp;с&nbsp;результатами&nbsp;оценки";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0003_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отдельному&nbsp;показателю";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0004_HMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Мониторинг&nbsp;соблюдения&nbsp;бюджетного&nbsp;законодательства";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0005_HMAO/Default.aspx";

        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0001_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
     
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }
            selectedPeriod = Convert.ToInt32(ComboYear.SelectedValue);
            selectedYear.Value = selectedPeriod.ToString();

            Page.Title = String.Format("Сводная оценка качества организации и осуществления бюджетного процесса в муниципальных образованиях Ханты-Мансийского автономного округа - Югры, индикаторы для оценки");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("по итогам {0} года", selectedPeriod);

            selectedMeasure.Value = Convert.ToBoolean(MeasureButtonList.SelectedIndex) ? "Значение" : "Оценка индикатора";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0021_0001_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Индикатор", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("муниципальное образование", "МО");
                        row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                    }
                }
                dtGrid.Columns.RemoveAt(0);
                
                if (MeasureType == MeasureType.Value)
                {
                    ClearFirstLevelValues(dtGrid);
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            columnCount = e.Layout.Bands[0].Columns.Count;
            string formatNumber = (MeasureType == MeasureType.Value) ? "N2" : "N1";
            if (MeasureType == MeasureType.Value)
            {
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200);
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                for (int i = 2; i < columnCount; i = i + 1)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatNumber);
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                headerLayout.AddCell("Индикатор");
                headerLayout.AddCell("Формула расчета");
                headerLayout.AddCell("Нижнее пороговое значение");
                headerLayout.AddCell("Верхнее пороговое значение");
            }
            else
            {
                for (int i = 1; i < columnCount; i = i + 1)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatNumber);
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                headerLayout.AddCell("Индикатор");
                headerLayout.AddCell("Вес");
            }

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            string query = DataProvider.GetQueryText("FO_0021_0001_HMAO_regionDescription");
            dtNameRegion = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Регион", dtNameRegion);
            if (dtNameRegion.Rows.Count > 0)
            {
                for (int i = 0; i < dtNameRegion.Rows.Count; i++)
                {
                    headerLayout.AddCell(dtNameRegion.Rows[i][1].ToString());
                }
            }
            headerLayout.ApplyHeaderInfo();

        }



        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int level;
            level = Convert.ToInt32(dtGrid.Rows[e.Row.Index][columnCount - 1]);
            if (level == 1)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void ClearFirstLevelValues(DataTable dt)
        {
            for (int i = 0; i <= dtGrid.Rows.Count - 1; i++)
            {
                int level = Convert.ToInt32(dtGrid.Rows[i][dtGrid.Columns.Count - 1]);
                if (level == 1)
                {
                    for (int j = 4; j < dtGrid.Columns.Count - 1; j++)
                    {
                        dtGrid.Rows[i][j] = DBNull.Value;
                    }
                }
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

    public enum MeasureType
    {
        Value,
        Evaluation
    }
}