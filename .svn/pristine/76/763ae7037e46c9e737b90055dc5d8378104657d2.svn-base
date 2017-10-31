using System;
using System.Data;
using System.Text;
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

namespace Krista.FM.Server.Dashboards.reports.STAT_0023_0003_01
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DataTable chartDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2010;
        private static MemberAttributesDigest orgDigest;
        private int columnWidth = 100;

        #endregion

        #region Параметры запроса

        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;
        // выбранная организация
        private CustomParam selectedOrganization;
        // выбранная организация
        private CustomParam selectedOrg;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 235);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);
            
            #endregion
            
            #region Настройка диаграммы динамики

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.57 - 100);
            
            DynamicChartBrick.YAxisLabelFormat = "N2";
            DynamicChartBrick.DataFormatString = "N2";
            DynamicChartBrick.Legend.Visible = false;
            DynamicChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            DynamicChartBrick.XAxisExtent = 60;
            DynamicChartBrick.YAxisExtent = 90;
            DynamicChartBrick.SwapRowAndColumns = true;
            DynamicChartBrick.IconSize = SymbolIconSize.Medium;
            DynamicChartBrick.PeriodFormat = "yyyy";

            #endregion

            #region Инициализация параметров запроса

            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");
            selectedOrganization = UserParams.CustomParam("selected_organization", true);
            selectedOrg = UserParams.CustomParam("selected_org", true);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = true;
            CrossLink.Text = "Реестр&nbsp;государственных&nbsp;предприятий";
            CrossLink.NavigateUrl = "~/reports/STAT_0023_0003_02/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(GridBrick.Grid.ClientID);

                DateTime lastDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0003_01_lastDate");
 
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboOrg.Title = "Предприятие";
                ComboOrg.Width = 500;
                ComboOrg.ParentSelect = true;
                ComboOrg.MultiSelect = false;
                orgDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0003_01_orgDigest");
                ComboOrg.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(orgDigest.UniqueNames, orgDigest.MemberLevels));
                ComboOrg.SetСheckedState(selectedOrganization.Value, true);

                selectedGridIndicator.Value = "Реализация товарной продукции";
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            selectedOrg.Value = orgDigest.GetMemberUniqueName(ComboOrg.SelectedValue);
            
            Page.Title = String.Format("Мониторинг финансово-хозяйственной деятельности государственных предприятий");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Анализ основных показателей, характеризующих работу государственных предприятий, Ханты-Мансийскийский автономный округ – Югра, за {0} год", 
                currentDate.Year);
            GridCaption.Text = selectedOrg.Value;

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0003_01_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                GridBrick.Width = CRHelper.GetGridWidth(340 + (gridDt.Columns.Count - 3) * columnWidth + 20);

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(340);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 3].Hidden = true;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Наименование");

            for (int i = 1; i < columnCount - 3; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
                headerLayout.AddCell(String.Format("{0} год", columnName));
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;

            int type = 0;
            if (e.Row.Cells[cellCount - 1].Value != null)
            {
                type = Convert.ToInt32(e.Row.Cells[cellCount - 1].Value.ToString());
            }

            for (int i = 1; i < cellCount - 3; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                cell.Style.Padding.Right = 3;

                string[] nameParts = e.Row.Band.Columns[i].Header.Caption.Split(' ');
                int prevYear = currentDate.Year;
                if (nameParts.Length > 0)
                {
                    prevYear = Convert.ToInt32(nameParts[0]) - 1;
                }

                switch (type)
                {
                    case 0:
                        {
                            if (cell.Value != null)
                            {
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                            }
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (cell.Value != null)
                            {
                                double rate = Convert.ToDouble(cell.Value.ToString());
                                cell.Value = rate.ToString("N2");

                                cell.Title = String.Format("Прирост к {0} году", prevYear);
                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 2:
                        {
                            if (cell.Value != null)
                            {
                                double growRate = Convert.ToDouble(cell.Value.ToString());
                                cell.Value = growRate.ToString("P2");

                                if (growRate > 0)
                                {
                                    cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                }
                                else if (growRate < 0)
                                {
                                    cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                }

                                cell.Title = String.Format("Темп прироста к {0} году", prevYear);
                                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center; margin: 2px";
                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        private void Grid_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(GridBrick.Grid, selectedGridIndicator.Value, 0, 0);
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
            if (row.Cells[row.Cells.Count - 3].Value != null)
            {
                indicatorName = row.Cells[row.Cells.Count - 3].Text;
            }

            string item = String.Empty;
            if (row.Cells[row.Cells.Count - 2].Value != null)
            {
                item = row.Cells[row.Cells.Count - 2].Text;
            }

            selectedGridIndicator.Value = indicatorName;

            DynamicChartCaption.Text = String.Format("Динамика показателя «{0}», {1}", indicatorName, item.ToLower());

            DynamicChartBrick.TooltipFormatString = String.Format("<b><SERIES_LABEL></b> год\n<b><DATA_VALUE_ITEM:N2></b>, {0}", item.ToLower());
            
            DynamicChartDataBind();
        }

        #endregion
        
        #region Обработчики диаграммы

        private void DynamicChartDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0003_01_chart");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                DynamicChartBrick.DataTable = chartDt;
                DynamicChartBrick.DataBind();
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            GridBrick.Grid.DisplayLayout.SelectedRows.Clear();

            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text + "\n" + GridCaption.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.7);
            DynamicChartBrick.Chart.Height = Convert.ToInt32(DynamicChartBrick.Chart.Height.Value * 0.7);
            ReportExcelExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void ExportGridSetup()
        {
            for (int i = 0; i < GridBrick.Grid.Rows.Count; i++)
            {
                UltraGridCell cell = GridBrick.Grid.Rows[i].Cells[0];

                int groupIndex = i % 3;

                switch (groupIndex)
                {
                    case 0:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 1:
                        {
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 2:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            break;
                        }
                }
            }
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ExportGridSetup();

            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text + "\n" + GridCaption.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, section1);

            ISection section2 = report.AddSection();
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, section2);
        }

        #endregion
    }
}