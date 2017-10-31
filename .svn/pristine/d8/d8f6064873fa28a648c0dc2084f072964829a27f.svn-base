using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0010_02
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

        #region Параметры запроса

        // Уровень районов
        private CustomParam regionsLevel;

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

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45 - 120);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.52 - 100);

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
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

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Axis.X.Extent = 160;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.RangeType = AxisRangeType.Automatic;

            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 11;
            UltraChart.Border.Thickness = 0;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) - UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Тыс.руб.";

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> тыс.руб.";
            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0010_02_date");
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
            }

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            ownBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("OwnBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            regionsOwnBudget.Value = RegionSettingsHelper.Instance.RegionsOwnBudgetLevel;
            
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            
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

            Page.Title = "Динамика просроченной кредиторской задолженности";
            PageTitle.Text = Page.Title;
            chartHeaderLabel.Text = string.Format("Сравнение задолженности на 01.{0:00}.{1} года с началом года", month, year);

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            PageSubTitle.Text = string.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString(); 
            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            UltraWebGrid.DataBind();
            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0010_02_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length - 1; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
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

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                SetColumnParams(e.Layout, 0, i, "N2", 101, false);
            }
            SetColumnParams(e.Layout, 0, e.Layout.Bands[0].Columns.Count - 1, "P2", 90, false);
            
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Просроченная кредиторская задолженность, тыс.руб.";
            
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            int month = ComboMonth.SelectedIndex + 1;
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Наименование", "");
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                int m = i;
                int y = year;
                if (i > 12)
                {
                    m = 1;
                    y++;
                }

                string caption = string.Format("на 01.{0:00}.{1} г.", m, y);
                string hint = (i == 1) ? "Сумма задолженности на начало года" : string.Format("Сумма задолженности за {0} {1} года", CRHelper.RusMonth(i - 1), year);

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, caption, hint);
            }
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, e.Layout.Bands[0].Columns.Count - 1,
                "Темп роста к началу года", string.Format("Темп роста задолженности за {0} к началу года", CRHelper.RusMonth(month)));

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = e.Layout.Bands[0].Columns.Count - 1;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rate = (i == e.Row.Cells.Count - 1);

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Задолженность сократилась по сравнению с началом года";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Задолженность увеличилась по сравнению с началом года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
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
            string query = DataProvider.GetQueryText("FO_0002_0010_02_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count == 0)
            {
                return;
            }

            double maxValue = 100000;

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                    row[0] = row[0].ToString().Replace("Муниципальный район", "МР");
                    row[0] = row[0].ToString().Replace("район", "р-н");
                }

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        double value = Convert.ToDouble(row[i]) / 1000;
                        
                        if (value > maxValue)
                        {
                            maxValue = value;
                        }

                        row[i] = value;
                    }
                }
            }

            if (maxValue > 100000)
            {
                UltraChart.Axis.Y.RangeType = AxisRangeType.Custom;
                UltraChart.Axis.Y.RangeMax = 100000;
            }
            else
            {
                UltraChart.Axis.Y.RangeType = AxisRangeType.Automatic;
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
                dtChart.Columns[1].ColumnName = string.Format("на 01.01.{0} г. ", ComboYear.SelectedValue);
                dtChart.Columns[2].ColumnName = string.Format("на 01.{0:00}.{1} г.", month, year);
            }

            UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
//            for (int i = 0; i < e.SceneGraph.Count; i++)
//            {
//                Primitive primitive = e.SceneGraph[i];
//                if (primitive is Box)
//                {
//                    Box box = (Box) primitive;
//                    if (box.DataPoint != null)
//                    {
//                        int columnIndex = box.Column == 1 ? 4 : 3;
//                        if (dtChart != null && dtChart.Rows.Count != 0 && dtChart.Rows[box.Row][columnIndex] != DBNull.Value)
//                        {
//                            double percent = Convert.ToDouble(dtChart.Rows[box.Row][columnIndex]);
//                            box.DataPoint.Label = string.Format("{0} {1:N3} тыс.руб.\n доля {2:P2}", box.DataPoint.Label, box.Value, percent);
//                        }
//                    }
//                }
//            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            int width = 101;
            for (int i = 1; i < columnCount; i++)
            {
                 e.CurrentWorksheet.Columns[i].Width = width * 37;
                 e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00";
            }
            e.CurrentWorksheet.Columns[columnCount - 1].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (e.CurrentColumnIndex > 0)
            {
                e.HeaderText = "Просроченная кредиторская задолженность, тыс.руб.";
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartHeaderLabel.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 0;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
