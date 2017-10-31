using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0024
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2011;
        private int currentYear;
        private int currentQuarter;
        private GridHeaderLayout headerLayout;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.31);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            SetColumnChartAppearance(UltraChart1);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            SetColumnChartAppearance(UltraChart2);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0024_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 170;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.RemoveTreeNodeByName("Квартал 1");
                ComboQuarter.SetСheckedState(baseQuarter, true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            currentQuarter = ComboQuarter.SelectedIndex + 2;

            PageTitle.Text = "Анализ расходов на содержание и численности органов местного самоуправления";
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format("за {0} квартал {1} года", currentQuarter, currentYear);

            chart1ElementCaption.Text = String.Format("Анализ расходов на содержание органов местного самоуправления за {0} квартал {1} года", currentQuarter, currentYear);
            chart2ElementCaption.Text = String.Format("Анализ численности органов местного самоуправления за {0} квартал {1} года", currentQuarter, currentYear);

            UserParams.PeriodYear.Value = currentYear.ToString();
            UserParams.PeriodLastYear.Value = (currentYear - 1).ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(currentQuarter));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", currentQuarter);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0024_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 80;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            int year = currentYear - 3;
            string outcomesThsRubCaption = "Расходы, тыс.руб.";
            string outcomesCaption = "Расходы";
            string outcomesHint = "Всего расходов на содержание органов местного самоуправления";
            string avgPopulationHumCaption = "Среднеспис. численность, чел.";
            string avgPopulationCaption = "Среднеспис. численность";
            string avgPopulationHint = "Численность работников органов местного самоуправления";

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            GridHeaderCell headerCell = headerLayout.AddCell(String.Format("{0} год", year));
            headerCell.AddCell(outcomesThsRubCaption, outcomesHint);
            headerCell.AddCell(avgPopulationHumCaption, avgPopulationHint);

            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i = i + 4)
            {
                year++;

                headerCell = headerLayout.AddCell(String.Format("{0} год", year));
                headerCell.AddCell(outcomesThsRubCaption, outcomesHint);
                headerCell.AddCell(avgPopulationHumCaption, avgPopulationHint);

                headerCell = headerLayout.AddCell(String.Format("Изменение {0}/{1} в%, '+' - рост, '-' - снижение", year, year - 1));
                headerCell.AddCell(outcomesCaption, outcomesHint);
                headerCell.AddCell(avgPopulationCaption, avgPopulationHint);
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null && 
                    (e.Row.Cells[0].Value.ToString() == "Итого по МР" || e.Row.Cells[0].Value.ToString() == "Итого по ГО"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
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
                }
            }
        }

        #endregion

        #region Обработчики диаграммы
        
        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = sender == UltraChart1 ? DataProvider.GetQueryText("FO_0002_0024_chart1") : DataProvider.GetQueryText("FO_0002_0024_chart2");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                ((UltraChart)sender).DataSource = dtChart;
            }
        }

        private void SetColumnChartAppearance(UltraChart chart)
        {
            chart.ChartType = ChartType.ColumnChart;
            chart.BorderWidth = 0;

            string item = chart == UltraChart1 ? "тыс.руб." : "чел.";

            chart.Tooltips.FormatString = string.Format("<SERIES_LABEL>\n<ITEM_LABEL>: <DATA_VALUE:N0> {0}", item);

            chart.Axis.X.Extent = 130;
            chart.Axis.Y.Extent = 60;

            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = true;
            chart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.ColumnChart.SeriesSpacing = 1;

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Top;
            chart.Legend.SpanPercentage = 13;
            chart.Legend.Margins.Right = Convert.ToInt32(chart.Width.Value / 2);
            chart.Legend.Font = new Font("Verdana", 8);

            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Font = new Font("Verdana", 8);
            chart.TitleLeft.Text = item;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Margins.Bottom = chart.Axis.X.Extent;

            chart.Data.ZeroAligned = true;
        }

        protected static void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 35;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 5);
            ReportExcelExporter1.Export(UltraChart2, chart2ElementCaption.Text, sheet3, 5);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart2, chart1ElementCaption.Text, section3);
        }

        #endregion
    }
}