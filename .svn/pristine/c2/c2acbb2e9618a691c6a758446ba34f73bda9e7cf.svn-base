using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Core.Primitives;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Shared.Events;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0016_0002: CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DataTable gridDate = new DataTable();
        private DataTable gridMax = new DataTable();
        private DataTable gridMin = new DataTable();
        private DataTable gridAvg = new DataTable();
        private DataTable chartDt = new DataTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Настройки таблиц и диаграмм

            UltraWebGrid1.Grid.EnableAppStyling = DefaultableBoolean.False;
            UltraWebGrid1.BrowserSizeAdapting = false;
            UltraWebGrid1.Height = 90;
            UltraWebGrid1.AutoSizeStyle = Components.GridAutoSizeStyle.AutoWidth;
            UltraWebGrid1.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.Grid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            #region Настройки диаграммы

            UltraChart.Width = 750;
            UltraChart.Height = 1200;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;
            UltraChart.BarChart.BarSpacing = 1; 
            UltraChart.ChartType = ChartType.BarChart;
            UltraChart.Axis.Y.Extent = 200;
            UltraChart.Axis.Y.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 12);
            UltraChart.Axis.Y.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Visible = true;
            UltraChart.Axis.X.Extent = 40;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 10);
            UltraChart.Axis.X.Margin.Far.Value = 5;
            UltraChart.Axis.X.Margin.Far.MarginType = LocationType.Percentage;
            UltraChart.Legend.Visible = false;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart.Tooltips.Font.Size = 12;
            UltraChart.Tooltips.Font.Name = "Verdana";
            UltraChart.Tooltips.Display = TooltipDisplay.MouseMove;
            UltraChart.BarChart.ChartText.Clear();
            UltraChart.BarChart.ChartText.Add(
                new ChartTextAppearance
                {
                    Column = -2,
                    Row = -2,
                    ItemFormatString = String.Format("<DATA_VALUE:{0}>", "N0"),
                    ChartTextFont = new Font("Verdana", 12),
                    Visible = true,
                    HorizontalAlign = StringAlignment.Far,
                    FontColor = Color.White
                });

            #endregion

            string query = DataProvider.GetQueryText("FO_0016_0002_date");
            gridDate = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, gridDate);
            UserParams.PeriodYear.Value = gridDate.Rows[0][0].ToString();
            UserParams.PeriodHalfYear.Value = gridDate.Rows[0][1].ToString();
            UserParams.PeriodQuater.Value = gridDate.Rows[0][2].ToString();

            UltraChartDataBind();
            GridDataBind();
        }

        private void GridDataBind()
        {

            string query2 = DataProvider.GetQueryText("FO_0016_0002_gridAvg");
            gridAvg = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Тип МО", gridAvg);

            if (gridAvg.Rows.Count > 0)
            {
                for (int i = 0; i < gridAvg.Rows.Count; i++)
                {

                    gridAvg.Rows[i][0] = String.Format(
                             "<a href='webcommand?showPopoverReport=FO_0016_0005_motype={0}&width=690&height=350&fitByHorizontal=true'>{1}</a>",
                             i, gridAvg.Rows[i][0]);

                }

                UltraWebGrid1.DataTable = gridAvg;
            }
        }
        

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
        
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(170);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            //e.Layout.Bands[0].Columns[0].Hidden = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 2; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }

            for (int i = e.Layout.Bands[0].Columns.Count - 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(150);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }


            GridHeaderLayout headerLayout = UltraWebGrid1.GridHeaderLayout;
            headerLayout.AddCell("Тип МО");
            headerLayout.AddCell("Всего");
            headerLayout.AddCell("Среднее");
            headerLayout.AddCell("Максимальное");
            headerLayout.AddCell("Минимальное");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Font.Size = 12;
            }
        }

        private void UltraChartDataBind()
        {
            string query3 = DataProvider.GetQueryText("FO_0016_0002_chart");
            chartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query3, "Наименование МР (ГО)", chartDt);
            chartDt.Columns.RemoveAt(0);
            for (int i = chartDt.Rows.Count - 1; i >= 0; i--)
            {
                chartDt.ImportRow(chartDt.Rows[i]);
                chartDt.Rows.RemoveAt(i);
            }
            for (int i = 1; i < chartDt.Columns.Count - 1; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, chartDt);
                series.Label = chartDt.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }
            UltraChart.DataBind();
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (chartDt.Rows.Count > 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];

                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null && box.Value != null)
                        {
                            int rowIndex = box.Column;
                            int columnIndex = 2;

                            string indicatorList = String.Empty;
                            if (chartDt != null && chartDt.Rows[rowIndex][columnIndex] != DBNull.Value &&
                                chartDt.Rows[rowIndex][columnIndex].ToString() != String.Empty)
                            {
                                indicatorList = String.Format("{0}", chartDt.Rows[rowIndex][columnIndex].ToString().TrimEnd(','));
                                indicatorList = BreakCollocator(indicatorList, ',', 3);
                                //indicatorList = list.Replace(",", ", ");
                            }

                            box.DataPoint.Label = string.Format("{0}",indicatorList);
                        }

                    }
                }
            }
        }

        private static string BreakCollocator(string source, char breakChar, int charIndex)
        {
            string breakedStr = String.Empty;
            int countBreak = 0;
            int charCount = 0;
            foreach (char ch in source)
            {
                breakedStr += ch;
                if (ch == breakChar)
                {
                    countBreak++;
                    charCount++;
                    if (charCount == charIndex)
                    {
                        breakedStr += "\n";
                        charCount = 0;
                    }
                }
            }
            if (countBreak == 26)
            {
                breakedStr += "...";
            }
            return breakedStr;
        }
    
    }
}