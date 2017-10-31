using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FO_0003_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Faces;
        private CustomParam Units;
        private CustomParam Year;
        private CustomParam month;
        private CustomParam mul;
        private DataTable candleDT;
        private DataTable chart1DT;
        private Dictionary<DateTime, string> candleLabelsDictionary;
        private Dictionary<DateTime, string> candleLabelsDictionary1;
        private DateTime currDateTime;
        private DateTime lastDateTime;
        private CustomParam ufo;
        private CustomParam currYear;
        private CustomParam years;
        private CustomParam monthlab;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
        private int endYear = 2009;
        private string endmonth = "Декабрь";
        private int indexOfRow = 0; 
        /// <summary>
        /// Выбраны ли
        /// федеральные округа
        /// </summary>
        ///
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 850 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый прелоад {0}", Environment.TickCount - start));
            base.Page_PreLoad(sender, e);
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            start = Environment.TickCount;
            if (Faces == null)
            {
                Faces = UserParams.CustomParam("Faces");
            }
            if (monthlab == null)
            {
                monthlab = UserParams.CustomParam("monthlab");
            }
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }
            if (month == null)
            {
                month = UserParams.CustomParam("month");
            }
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (currYear == null)
            {
                currYear = UserParams.CustomParam("currYear");
            }
            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }
            if (years == null)
            {
                years = UserParams.CustomParam("years");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.65);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.CandleChart.SkipN = 0;
            UltraChart.CandleChart.HighLowVisible = true;
            UltraChart.CandleChart.ResetHighLowVisible();
            UltraChart.CandleChart.VolumeVisible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 20);
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            UltraChart.Tooltips.FormatString = "";
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 100;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 10);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
            UltraChart1.ChartType = ChartType.SplineChart;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph1);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.CandleChart.SkipN = 0;
            UltraChart1.CandleChart.HighLowVisible = true;
            UltraChart1.CandleChart.ResetHighLowVisible();
            UltraChart1.CandleChart.VolumeVisible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.SpanPercentage = 9;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            UltraChart1.Tooltips.FormatString = "";
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Extent = 110;
            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.SplineChart.NullHandling = NullHandling.DontPlot;
            UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE:P0>");
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraGridExporter1.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
            <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
            <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CRHelper.SaveToUserAgentLog(String.Format("Остальной прелоад {0}", Environment.TickCount - start));
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.HeaderChildCellHeight = 100;
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {

        }

        string meas = string.Empty;
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));
            start = Environment.TickCount;
            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
            }
            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddRefreshTarget(ChartCaption);
            }
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0003_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            endmonth = dtDate.Rows[0][3].ToString();
            int firstYear = 2002;
            if (!Page.IsPostBack)
            {
                ComboYear.Width = 150;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetAllСheckedState(true, true);
            }
            string faces = string.Empty;
            if (RadioList.SelectedIndex == 0)
            {
                mul.Value = Convert.ToString(1000);
                meas = "тыс.руб";
            }
            else
            {
                mul.Value = Convert.ToString(1000000);
                meas = "млн.руб";
            }
            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string gridDescendants = String.Empty;
                string chartDescendants = String.Empty;
                years.Value = String.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    string sign = string.Empty;
                    if (i != selectedValues.Count - 1)
                    {
                        sign = ",";
                    }
                    years.Value += string.Format("{1}.[Данные всех периодов].[{0}]{2}",
                    year, "[Период__Период].[Период__Период]", sign);
                }
            }
            ChartCaption.Text = string.Format("Годовая динамика поступлений собственных доходов за период, {0}", meas);
            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE:N2> {0}", meas);
            currDateTime = new DateTime(endYear, CRHelper.MonthNum(endmonth), 01);
            currDateTime = currDateTime.AddMonths(1);
            Page.Title = "Поступление собственных доходов в консолидированный бюджет субъекта";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("за {2} - {3} годы по состоянию на {0:dd.MM.yyyy} года, {1}", currDateTime, meas, selectedValues[0], selectedValues[selectedValues.Count - 1]);
            UserParams.PeriodYear.Value = "2008";
            int defaultRowIndex = (CRHelper.MonthNum(endmonth) - 1) * 2;
            UltraWebGrid.DataBind();
            UltraChart.DataBind();
            UltraChart1.DataBind();
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, monthlab.Value, 0, defaultRowIndex);
            ActiveGridRow(row);
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;
            monthlab.Value = dtGrid.Rows[row.Index][0].ToString();
            UltraChart.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            string query = DataProvider.GetQueryText("FO_0003_0001_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

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
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].AllowSorting = AllowSorting.No;
            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 1;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(1230/UltraWebGrid.Columns.Count);
                    }
                    string formatString = "N0";
                    if ((i % 2) == 0)
                    {
                        formatString = "N1";
                    }
                    e.Layout.Bands[0].Columns[i].Format = formatString;
                    if (i > 0)
                    {
                        if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Факт"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Substring(0, 4);
                            e.Layout.Bands[0].Columns[i].Header.Caption = String.Format("{0}{1}", e.Layout.Bands[0].Columns[i].Header.Caption, " г.");
                        }
                        else if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("роста"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = "% роста";
                        }
                    }
                }
                int count = e.Layout.Bands[0].Columns.Count;
                e.Layout.Bands[0].Columns[0].Hidden = false;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
                    e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                Collection<string> cellsCaption = new Collection<string>();
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {

        }

        #endregion

        #region Обработчики диаграмы
        DataTable dtChart = new DataTable();
        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Chart_query");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                UltraChart.DataSource = dtChart;
            }
        }
        DataTable dtChart1 = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = string.Empty;
            if (periodList.SelectedIndex == 0)
            {
                query = DataProvider.GetQueryText("Chart1_query_1");
            }
            else
            {
                query = DataProvider.GetQueryText("Chart1_query_2");
            }

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            UltraChart1.Series.Clear();
            for (int i = 1; i < dtChart1.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                series.Label = dtChart1.Columns[i].ColumnName;
                UltraChart1.Series.Add(series);
            }

        }


        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string grbsName = box.DataPoint.Label;
                        box.DataPoint.Label = DataDictionariesHelper.GetFullFMGRBSNames(grbsName);

                        box.PE.ElementType = PaintElementType.Gradient;
                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;

                        int boxRow = box.Column;

                        if (boxRow == 0)
                        {
                            box.PE.Fill = Color.LightGreen;
                            box.PE.FillStopColor = Color.Green;
                        }
                        else
                        {
                            if (Convert.ToInt32(dtChart.Rows[0][box.Column + 1]) > Convert.ToInt32(dtChart.Rows[0][box.Column]))
                            {
                                box.PE.Fill = Color.LightGreen;
                                box.PE.FillStopColor = Color.Green;
                                box.DataPoint.Label += "\nРост объема фактических поступлений по сравнению с аналогичным периодом прошлого года";
                            }
                            else if (Convert.ToInt32(dtChart.Rows[0][box.Column + 1]) < Convert.ToInt32(dtChart.Rows[0][box.Column]))
                            {
                                box.PE.Fill = Color.LightPink;
                                box.PE.FillStopColor = Color.Red;
                                box.DataPoint.Label += "\nСнижение объема фактических поступлений по сравнению с аналогичным периодом прошлого года";
                            }
                        }
                    }
                }
            }
        }


        void UltraChart_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Polyline)
                {
                    Polyline poly = (Polyline)primitive;

                    if (poly.Series != null)
                    {
                        for (int j = 0; j < poly.points.Length; j++)
                        {
                            DataPoint point = poly.points[j];
                            if ((dtChart1.Rows[point.Column][point.Row + 1] != DBNull.Value))
                            {
                                if (Convert.ToDouble(dtChart1.Rows[point.Column][point.Row + 1]) > 1)
                                {
                                    point.DataPoint.Label = String.Format("Рост поступлений по сравнению с аналогичным периодом прошлого года");
                                }
                                else if (Convert.ToDouble(dtChart1.Rows[point.Column][point.Row + 1]) < 1)
                                {
                                    point.DataPoint.Label = String.Format("Снижение поступлений по сравнению с аналогичным периодом прошлого года");
                                }
                            }
                        }
                    }
                }
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            int lineStart = (int)xAxis.MapMinimum;
            int lineLength = (int)xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = System.Drawing.Color.LightGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new System.Drawing.Point(lineStart, (int)yAxis.Map(1));
            line.p2 = new System.Drawing.Point(lineStart + lineLength, (int)yAxis.Map(1));
            e.SceneGraph.Add(line);
        }


        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        }
        #endregion

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = String.Format("{0} {1}",Page.Title, Label2.Text);

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 300;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            for (int i = 1; i < columnCount; i = i + 1)
            {
                e.CurrentWorksheet.Columns[i].Width = CRHelper.GetColumnWidth(1230 / UltraWebGrid.Columns.Count) * 40;
            }
            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                for (int j = 5; j < 20; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
            }
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                if (i % 2 == 0)
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,#0.0";
                }else
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#0";
                }
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
        }



        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Пост. доходов");
            Worksheet sheet2 = workbook.Worksheets.Add("Годовая динамика");
            Worksheet sheet3 = workbook.Worksheets.Add("Сравнение т.р.");
            sheet2.Rows[0].Cells[0].Value = ChartCaption.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[1].Cells[0], UltraChart);
            sheet3.Rows[0].Cells[0].Value = ChartCaption1.Text;
            UltraGridExporter.ChartExcelExport(sheet3.Rows[1].Cells[0], UltraChart1);
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
        }

        protected void RadioList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text.Replace("<br/>", ""));
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
            e.Section.AddPageBreak();
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);

            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

        }

        #endregion

    }
}