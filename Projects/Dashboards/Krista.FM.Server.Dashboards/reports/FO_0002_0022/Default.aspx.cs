using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0022
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

        private string exportFontName = "Times New Roman";
        private GridHeaderLayout headerLayout;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.31);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.3);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.BorderWidth = 0;
            
            UltraChart.Tooltips.FormatString = string.Format("<SERIES_LABEL>\n<ITEM_LABEL>: <DATA_VALUE:P0>");

            UltraChart.Axis.X.Extent = 130;
            UltraChart.Axis.Y.Extent = 40;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.ColumnChart.SeriesSpacing = 1;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 16;
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value / 3);
            UltraChart.Legend.Font = new Font("Verdana", 8);

            UltraChart.Data.ZeroAligned = true;
            
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0022_date");
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

            PageTitle.Text = "Расчет неэффективных расходов";
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = string.Format("за {0} квартал {1} года", currentQuarter, currentYear);
            chartElementCaption.Text = "Доля неэффективных расходов в общем объеме расходов в сравнении с аналогичным периодом предыдущего года";

            UserParams.PeriodYear.Value = currentYear.ToString();
            UserParams.PeriodLastYear.Value = (currentYear - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(currentQuarter));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", currentQuarter);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0022_grid");
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
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int groupIndex = (i - 1) % 7;

                string formatString = "N0";
                int widthColumn = 150;

                switch (groupIndex)
                {
                    case 2:
                        {
                            widthColumn = 100;
                            break;
                        }
                }
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 7)
            {
                int year = currentYear - (2 - (i - 1) / 7);
                GridHeaderCell header = headerLayout.AddCell(String.Format("Расчет неэффективных расходов за {0} год", year));

                header.AddCell(
                    String.Format("Общий размер расходов консолидированного бюджета за {0} год, руб.", year),
                    "Фактический объем расходов консолидированного бюджета");
                header.AddCell(
                    String.Format("Размер расходов на содержание органов местного самоуправления в консолидированном бюджете за {0} год, руб.", year),
                    "Фактический объем расходов на содержание органов местного самоуправления");
                header.AddCell(
                    "Размер неэффективных расходов на управление, руб.",
                    "Объем неэффективных расходов в сфере организации муниципального управления");
                header.AddCell(
                    "Доля неэффективных расходов (консолидированных бюджетов) на управление в общем размере расходов, %",
                    "Доля неэффективных расходов в общем объёме расходов на организацию муниципального управления");

                e.Layout.Bands[0].Columns[i + 4].Hidden = true;
                e.Layout.Bands[0].Columns[i + 5].Hidden = true;
                e.Layout.Bands[0].Columns[i + 6].Hidden = true;
                
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int groupIndex = (i - 1) % 7;

                int prevYearRateColumnOffset = 2;
                bool rate = (groupIndex == 2);

                int rankIndicateColumnOffset = -2;
                bool rank = (groupIndex == 5);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        int badRank = Convert.ToInt32(e.Row.Cells[i + 1].Value);
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == badRank)
                        {
                            e.Row.Cells[i + rankIndicateColumnOffset].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i + rankIndicateColumnOffset].Title = "Минимальная доля неэффективных расходов в общем объеме расходов"; 
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i + rankIndicateColumnOffset].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i + rankIndicateColumnOffset].Title = "Максимальная доля неэффективных расходов в общем объеме расходов";
                        }
                    }
                    e.Row.Cells[i + rankIndicateColumnOffset].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + prevYearRateColumnOffset].Value != null && e.Row.Cells[i + prevYearRateColumnOffset].Value.ToString() != string.Empty)
                    {
                        double currValue = Convert.ToDouble(e.Row.Cells[i].Value);
                        double prevValue = Convert.ToDouble(e.Row.Cells[i + prevYearRateColumnOffset].Value);
                        if (currValue > prevValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Рост размера неэффективных расходов по отношению к аналогичному периоду прошлого года";
                        }
                        else if (currValue < prevValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение размера неэффективных расходов по отношению к аналогичному периоду прошлого года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

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
            string query = DataProvider.GetQueryText("FO_0002_0022_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 2)
            {
                dtChart.Columns[1].ColumnName = String.Format("Доля неэффективных расходов за {0} квартал {1} года", currentQuarter, currentYear - 1);
                dtChart.Columns[2].ColumnName = String.Format("Доля неэффективных расходов за {0} квартал {1} года", currentQuarter, currentYear);

                UltraChart.DataSource = dtChart;
            }
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

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.HeaderCellHeight = 60;
            ReportExcelExporter1.HeaderCellFont = new Font(exportFontName, 12, FontStyle.Bold);
            ReportExcelExporter1.TitleFont = new Font(exportFontName, 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font(exportFontName, 11);
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, chartElementCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в Pdf

        void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartElementCaption.Text);

            UltraChart.Legend.Margins.Right = 5;
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section = report.AddSection();
            section.PageSize = new PageSize(2000, 1200);

            ReportPDFExporter1.HeaderCellHeight = 60;
            ReportPDFExporter1.Export(headerLayout, section);
            //
            //            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            //            ReportPDFExporter1.Export(UltraChart, chartElementCaption.Text, section);
        }

        #endregion
    }
}