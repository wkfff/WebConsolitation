using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using System.IO;
using System.Drawing.Imaging;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0010
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2000;
        private int endYear = 2011;

        #endregion

        private GridHeaderLayout gridHeaderLayout;
        private DateTime currentDate;

        private bool ComparePrevYear
        {
            get { return comparePrevYear.Checked; }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        private bool ChartDistrictExcluding
        {
            get { return ChartDistrictExclude.Checked; }
        }

        private double rubMultiplier;

        #region Параметры запроса

        // Уровень районов
        private CustomParam regionsLevel;
        // Вид задолженности
        private CustomParam debtsKind;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для собственного бюджета субъекта
        private CustomParam ownBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;
        // собственный бюджет субъекта
        private CustomParam regionsOwnBudget;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // выбранная мера Факт для сравнения
        private CustomParam selectedFactMeasure;

        // выбранная мера Темп роста для сравнения
        private CustomParam selectedRateMeasure;

        // множество МО для диаграммы
        private CustomParam chartRegionSet;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45 - 120);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = 820;//CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 1 - 100);//0.52

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (debtsKind == null)
            {
                debtsKind = UserParams.CustomParam("debts_kind");
            }
            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (ownBudgetDocumentSKIFType == null)
            {
                ownBudgetDocumentSKIFType = UserParams.CustomParam("own_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }
            if (regionsConsolidateBudget == null)
            {
                regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            }
            if (regionsOwnBudget == null)
            {
                regionsOwnBudget = UserParams.CustomParam("regions_own_budget");
            }

            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            if (selectedFactMeasure == null)
            {
                selectedFactMeasure = UserParams.CustomParam("selected_fact_measure");
            }
            if (selectedRateMeasure == null)
            {
                selectedRateMeasure = UserParams.CustomParam("selected_rate_measure");
            }

            chartRegionSet = UserParams.CustomParam("chart_region_set");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Axis.X.Extent = 160;
            UltraChart.Axis.Y.Extent = 80;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.RangeType = AxisRangeType.Automatic;
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);

            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart.Width.Value) / 3;
            //UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) - 200;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 5;
            UltraChart.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Border.Thickness = 0;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = RubMultiplierCaption;
            UltraChart.TitleLeft.Font = new System.Drawing.Font("Verdana", 8);

            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(ChartDistrictExclude.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0010_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboDebts.Title = "Вид задолженности";
                ComboDebts.Width = 500;
                ComboDebts.MultiSelect = false;
                ComboDebts.ParentSelect = true;
                ComboDebts.FillDictionaryValues(CustomMultiComboDataHelper.FillDebtsList());
                ComboDebts.SetСheckedState("Просроченная кредиторская задолженность, всего ", true);
            }

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            debtsKind.Value = ComboDebts.SelectedValue;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            ownBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("OwnBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            regionsOwnBudget.Value = RegionSettingsHelper.Instance.RegionsOwnBudgetLevel;

            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            ChartDistrictExclude.Visible = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("DistrictExtrudeEnable"));

            int month = ComboMonth.SelectedIndex + 1;
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(year, month, 1);
            DateTime nextMonthDate = currentDate.AddMonths(1);

            Page.Title = "Динамика просроченной кредиторской задолженности";
            PageTitle.Text = Page.Title;
            string compareType = ComparePrevYear ? "с аналогичным периодом прошлого года" : "с началом года";
            chartHeaderLabel.Text = String.Format("Сравнение задолженности на 01.{0:00}.{1} года {3}: {2}",
                nextMonthDate.Month, nextMonthDate.Year, ComboDebts.SelectedValue, compareType);

            PageSubTitle.Text = String.Format("{3} за {0} {1} {2} года", currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month),
                currentDate.Year, ComboDebts.SelectedValue);

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            selectedFactMeasure.Value = ComparePrevYear
                ? String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value)
                : "[Полугодие 2].[Квартал 4].[Декабрь]";
            selectedRateMeasure.Value = ComparePrevYear ? "Темп роста к прошлому году" : "Темп роста к началу года";

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {

                gridHeaderLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
            }

            UltraChart.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", RubMultiplierCaption.ToLower());

            chartRegionSet.Value = ChartDistrictExcluding ? "МО без городов" : "МО с городами";
            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0010_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                SetColumnParams(e.Layout, 0, i, "N2", 101, false);
            }
            SetColumnParams(e.Layout, 0, e.Layout.Bands[0].Columns.Count - 1, "P2", 80, false);

            gridHeaderLayout.AddCell("Бюджет");
            GridHeaderCell groupCell = gridHeaderLayout.AddCell(String.Format("Просроченная кредиторская задолженность, {0}", RubMultiplierCaption.ToLower()));

            int year = currentDate.Year;
            int month = currentDate.Month;
            for (int i = 1; i < columnCount - 1; i++)
            {
                int m = i;
                int y = year;
                if (i > 12)
                {
                    m = 1;
                    y++;
                }

                string caption = String.Format("на 01.{0:00}.{1} г.", m, y);
                string hint = (i == 1) ? "Сумма задолженности на начало года" : String.Format("Сумма задолженности за {0} {1} года", CRHelper.RusMonth(i - 1), year);

                groupCell.AddCell(caption, hint);
            }
            string growRateHint = ComparePrevYear
                ? String.Format("Темп роста задолженности за {0} {1} к прошлому году", CRHelper.RusMonth(month), year)
                : String.Format("Темп роста задолженности за {0} к началу года", CRHelper.RusMonth(month));

            groupCell.AddCell(selectedRateMeasure.Value, growRateHint);

            gridHeaderLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rate = (i == e.Row.Cells.Count - 1);

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = ComparePrevYear
                                ? "Задолженность сократилась по сравнению с прошлым годом"
                                : "Задолженность сократилась по сравнению с началом года";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = ComparePrevYear
                                ? "Задолженность увеличилась по сравнению с прошлым годом"
                                : "Задолженность увеличилась по сравнению с началом года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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

                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") ||
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("городские округа") ||
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("муниципальные районы")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0010_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count == 0)
            {
                return;
            }

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("городской округ", "ГО");
                    row[0] = row[0].ToString().Replace("Городской округ", "ГО");
                    row[0] = row[0].ToString().Replace("муниципальное образование", "МО");
                    row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                    row[0] = row[0].ToString().Replace("Муниципальный район", "МР");
                    row[0] = row[0].ToString().Replace("\"", "'");
                    row[0] = row[0].ToString().Replace("район", "р-н");
                }

                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / rubMultiplier;
                }

                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) / rubMultiplier;
                }
            }

            int month = ComboMonth.SelectedIndex + 1;
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            if (month == 12)
            {
                month = 1;
                year++;
            }
            else
            {
                month++;
            }

            if (dtChart.Columns.Count > 2)
            {
                dtChart.Columns[1].ColumnName = ComparePrevYear
                        ? String.Format("на 01.{0:00}.{1} г.", month, year - 1)
                        : String.Format("на 01.01.{0} г. ", ComboYear.SelectedValue);
                dtChart.Columns[2].ColumnName = String.Format("на 01.{0:00}.{1} г.", month, year);
            }

            UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 30;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new System.Drawing.Font("Verdana", 8);
                    text.labelStyle.WrapText = true;
                }
            }
        }

        #endregion

        #region Экспорт

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Section.AddPageBreak();
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();
            IText title = cell.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartHeaderLabel.Text);

            MemoryStream imageStream = new MemoryStream();
            UltraChart.SaveTo(imageStream, ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image image = (new Bitmap(imageStream)).ScaleImageIg(1.1);
            switch (currentDate.Month)
            {
                case 9:
                    image = (new Bitmap(imageStream)).ScaleImageIg(1.5);
                    break;
                case 10:
                    image = (new Bitmap(imageStream)).ScaleImageIg(1.58);
                    break;
                case 11:
                    image = (new Bitmap(imageStream)).ScaleImageIg(1.8);
                    break;
                case 12:
                    image = (new Bitmap(imageStream)).ScaleImageIg(1.85);
                    break;
                default: image = (new Bitmap(imageStream)).ScaleImageIg(1.1);
                    break;
            }
            cell.AddImage(image);
            //cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart));
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.Export(gridHeaderLayout);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            //ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            // ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            sheet1.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet1.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet2.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet2.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet2.Rows[2].Cells[0].Value = chartHeaderLabel.Text;
            ReportExcelExporter1.Export(gridHeaderLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, sheet2, 4);
        }

        #endregion
    }
}
