using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;

namespace Krista.FM.Server.Dashboards.reports.STAT_0023_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DataTable organizSelectDt = new DataTable();
        private DateTime currDate;
        private DateTime prevDate;

        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest orgDigest;

        #endregion

        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1000; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 800 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        #region Параметры запроса

        // текущий период
        private CustomParam currPeriod;
        // предыдущий период
        private CustomParam prevPeriod;
        // выбранная организация
        private CustomParam selectedOrganization;
        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Width = IsSmallResolution ? 720 : CRHelper.GetGridWidth(MinScreenWidth - 12);
            GridBrick.Height = Unit.Empty;
            //GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 235);
            //GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);

            #endregion

            #region Настройка диаграммы динамики

            //ChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            //ChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.55 - 100);
            ChartBrick.Width = CRHelper.GetChartWidth(MinScreenWidth - 25);
            ChartBrick.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.55 - 100);
            ChartBrick.YAxisLabelFormat = "N2";
            ChartBrick.DataFormatString = "N2";
            ChartBrick.Legend.Visible = false;
            ChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            ChartBrick.XAxisExtent = 100;
            ChartBrick.YAxisExtent = 90;
            ChartBrick.SwapRowAndColumns = true;
            ChartBrick.IconSize = SymbolIconSize.Medium;
            ChartBrick.AxisXAutoFormat = false;
            ChartBrick.Chart.Axis.X.Labels.ItemFormat = AxisItemLabelFormat.ItemLabel;
            ChartBrick.ZeroAligned = false;

            #endregion

            #region Инициализация параметров запроса

            currPeriod = UserParams.CustomParam("curr_period");
            prevPeriod = UserParams.CustomParam("prev_period");
            selectedOrganization = UserParams.CustomParam("selected_organization");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(ChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(ChartCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(GridBrick.Grid.ClientID);

                ComboPeriod.Title = "Период";
                ComboPeriod.Width = 250;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ParentSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0001_periodDigest");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();

                currPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);

                string query = DataProvider.GetQueryText("STAT_0023_0001_organizationSelected");
                organizSelectDt = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", organizSelectDt);
                string organizSelect = "";
                if (organizSelectDt.Rows[0][0] != DBNull.Value)
                {
                    organizSelect = organizSelectDt.Rows[0][0].ToString();
                    organizSelect = organizSelect.Remove(organizSelect.IndexOf(';'));
                }
    
                ComboOrg.Title = "Предприятие";
                ComboOrg.Width = 400;
                ComboOrg.ParentSelect = false;
                ComboOrg.MultiSelect = false;
                orgDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0001_organizationDigest");
                ComboOrg.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(orgDigest.UniqueNames, orgDigest.MemberLevels));
                if (organizSelect != "")
                {
                    ComboOrg.SetСheckedState(organizSelect, true);
                }
                selectedGridIndicator.Value = "Численность работников списочного состава на конец отчетного месяца";
            }

            currPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);

            currDate = CRHelper.PeriodDayFoDate(currPeriod.Value);
            prevDate = currDate.AddMonths(-1);

            prevPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", prevDate, 4);

            UserParams.PeriodYear.Value = currDate.Year.ToString();

            Page.Title = String.Format("Мониторинг финансово-экономического состояния системообразующих предприятий");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Ежемесячный анализ основных показателей, характеризующих работу системообразующих предприятий, Ханты-Мансийский автономный округ – Югра, за {0} {1} года", 
                CRHelper.RusMonth(currDate.Month), currDate.Year);

            ChartCaption.Text = String.Format("Структурная динамика числа учтенных организаций и предприятий на начало года");

            selectedOrganization.Value = ComboOrg.SelectedValue;
            OrganizationCaption.Text = ComboOrg.SelectedValue;

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                } 
                
                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(370);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            
            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 3].Hidden = true;

            for (int i = 1; i < columnCount - 3; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption.ToLower();

                string format = columnName.Contains("темп") ? "P2" : "N2";
                int width = columnName.Contains("темп") || columnName.Contains("отклонение") ? 90 : 130;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], format);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Основные показатели, характеризующие деятельность предприятия");
            headerLayout.AddCell(String.Format("{0} {1} года", CRHelper.RusMonth(currDate.Month).ToUpperFirstSymbol(), currDate.Year));
            headerLayout.AddCell(String.Format("{0} {1} года", CRHelper.RusMonth(prevDate.Month).ToUpperFirstSymbol(), prevDate.Year));
            headerLayout.AddCell(String.Format("Январь {0} года", currDate.Year));
            AddGroupCell("Динамика к предыдущему периоду", true);
            AddGroupCell("Динамика за период с начала года", false);

            headerLayout.ApplyHeaderInfo();
        }

        private void AddGroupCell(string groupName, bool rate)
        {
            GridHeaderCell groupCell = GridBrick.GridHeaderLayout.AddCell(groupName);
            groupCell.AddCell("Абсолютное отклонение",
                rate ? String.Format("Прирост к {0} {1} года", CRHelper.RusMonthDat(prevDate.Month), prevDate.Year) : String.Format("Прирост к началу {0} года", currDate.Year));
            groupCell.AddCell("Темп прироста, %",
                rate ? String.Format("Темп прироста к {0} {1} года", CRHelper.RusMonthDat(prevDate.Month), prevDate.Year) : String.Format("Темп прироста к началу {0} года", currDate.Year));
        }

        private void Grid_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(GridBrick.Grid, selectedGridIndicator.Value, GridBrick.Grid.Columns.Count - 2, 0);
                ActivateGridRow(row);
            }
        }

        private void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }

            string indicatorName = String.Empty;
            if (row.Cells[row.Cells.Count - 2].Value != null)
            {
                indicatorName = row.Cells[row.Cells.Count - 2].Text;
            }

            string item = String.Empty;
            if (row.Cells[row.Cells.Count - 1].Value != null)
            {
                item = row.Cells[row.Cells.Count - 1].Text;
            }

            selectedGridIndicator.Value = indicatorName;

            ChartCaption.Text = String.Format("Динамика показателя «{0}», {1}", indicatorName, item.ToLower());

            ChartBrick.TooltipFormatString = String.Format("<b><SERIES_LABEL></b> года\n<b><DATA_VALUE_ITEM:N2></b>, {0}", item.ToLower());

            ChartDataBind();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;
            int prevPeriodRateIndex = 5;
            int yearBeginRateIndex = 7;

            bool inverseRate = false;
            if (e.Row.Cells[e.Row.Cells.Count - 3].Value != null)
            {
                int code = Convert.ToInt32(e.Row.Cells[e.Row.Cells.Count - 3].Text);
                inverseRate = (code >= 6 && code <= 8) || code == 12 || code == 16 || code == 17;
            }
            
            for (int i = 1; i < cellCount - 3; i++)
            {
                if (i == prevPeriodRateIndex || i == yearBeginRateIndex)
                {
                    string prevMonthStr = String.Format("к {0} {1} года", CRHelper.RusMonthDat(prevDate.Month), prevDate.Year);

                    UltraGridCell cell = e.Row.Cells[i];
                    if (cell.Value != null)
                    {
                        double growRate = Convert.ToDouble(cell.Value.ToString());
                        if (growRate > 0)
                        {
                            cell.Style.BackgroundImage = inverseRate ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                            //cell.Title = i == prevPeriodRateIndex ? String.Format("Прирост {0}", prevMonthStr) : String.Format("Прирост к началу {0} года", currDate.Year);
                        }
                        else if (growRate < 0)
                        {
                            cell.Style.BackgroundImage = inverseRate ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                            //cell.Title = i == prevPeriodRateIndex ? String.Format("Снижение {0}", prevMonthStr) : String.Format("Снижение к началу {0} года", currDate.Year);
                        }
                        cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center left; margin: 2px";
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            string queryText = DataProvider.GetQueryText("STAT_0023_0001_chart");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                if (chartDt.Columns.Count > 1)
                {
                    chartDt.Columns.RemoveAt(0);
                }

                ChartBrick.DataTable = chartDt;
                ChartBrick.DataBind();
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;
  
            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
     
            sheet1.Rows[2].Cells[0].Value = OrganizationCaption.Text;
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
           
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            ChartBrick.Chart.Width = Convert.ToInt32(ChartBrick.Chart.Width.Value * 0.7);
            ChartBrick.Chart.Height = Convert.ToInt32(ChartBrick.Chart.Height.Value * 0.7);
            ReportExcelExporter1.Export(ChartBrick.Chart, ChartCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text + "\n" + OrganizationCaption.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;
            
            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            ChartBrick.Chart.Width = Convert.ToInt32(ChartBrick.Chart.Width.Value * 0.7);
            ChartBrick.Chart.Height = Convert.ToInt32(ChartBrick.Chart.Height.Value * 0.7);
            ReportPDFExporter1.Export(ChartBrick.Chart, ChartCaption.Text, section2);
        }

        #endregion
    }
}