using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0009_Horizontal : CustomReportPage
    {
       

        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            CustomParams.MakeRegionParams("65", "id");

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0003_0009_Horizontal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            DateTime date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            lbDescription.Text = String.Format("��������� �������� ������������������ �������");

            SetChartAppearance(UltraChart1, 430, true);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            SetupCustomSkin(UltraChart1);
            DataTable dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0009_Horizontal_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            dtIncomes.Columns[1].ColumnName = String.Format("{0} ���", dtIncomes.Columns[1].ColumnName);
            dtIncomes.Columns[2].ColumnName = String.Format("{0} ���", dtIncomes.Columns[2].ColumnName);
            dtIncomes.Columns[3].ColumnName = String.Format("�� {0:dd.MM.yyyy}", date.AddMonths(1));

            dt1 = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0009_Horizontal_chart_summ");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt1);

            UltraChart1.Series.Clear();

            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtIncomes));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtIncomes));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(3, dtIncomes));
            UltraChart1.DataBind();

            SetChartAppearance(UltraChart2, 330, false);
            SetupCustomSkin(UltraChart2);
            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0009_Horizontal_chart2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            dtIncomes.Columns[1].ColumnName = String.Format("{0} ���", dtIncomes.Columns[1].ColumnName);
            dtIncomes.Columns[2].ColumnName = String.Format("{0} ���", dtIncomes.Columns[2].ColumnName);
            dtIncomes.Columns[3].ColumnName = String.Format("�� {0:dd.MM.yyyy}", date.AddMonths(1));

            dt2 = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0009_Horizontal_chart2_summ");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt2);

            UltraChart2.Series.Clear();

            UltraChart2.Series.Add(CRHelper.GetNumericSeries(1, dtIncomes));
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(2, dtIncomes));
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(3, dtIncomes));
            UltraChart2.DataBind();

            UltraChart3.Width = 760;
            UltraChart3.Height = 60;
            UltraChart3.Legend.Font = new Font("Verdana", 10);
            UltraChart3.Legend.FontColor = Color.Black;
            UltraChart3.BackColor = Color.FromArgb(0xa8a8a8);
            UltraChart3.Legend.SpanPercentage = 100;
            UltraChart3.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);
            SetupCustomSkin(UltraChart3);
            UltraChart3.Axis.X.Labels.Visible = false;
            UltraChart3.Axis.X.Labels.SeriesLabels.Visible = false;

            UltraChart3.Series.Clear();

            UltraChart3.Series.Add(CRHelper.GetNumericSeries(1, dtIncomes));
            UltraChart3.Series.Add(CRHelper.GetNumericSeries(2, dtIncomes));
            UltraChart3.Series.Add(CRHelper.GetNumericSeries(3, dtIncomes));
            UltraChart3.DataBind();
        }

        void UltraChart3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive.Path != null && 
                    primitive.Path.Contains("Legend"))
                {

                }
                else if (!(primitive.ToString().Contains("Text")))
                {
                    primitive.Visible = false;
                }
            }
        }

        void UltraChart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0} {2}<br/>�����&nbsp;<b>{1:N0}</b>&nbsp;���.���.", box.DataPoint.Label, dt2.Rows[box.Column][box.Row + 1], box.Series.Label);
                    }
                }
            }
        }

        void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0} {2}<br/>�����&nbsp;<b>{1:N0}</b>&nbsp;���.���.", box.DataPoint.Label, dt1.Rows[box.Column][box.Row + 1], box.Series.Label);
                    }
                }
            }
        }

        private void SetChartAppearance(UltraChart chart, int width, bool seriesLabelVisible)
        {
            chart.Width = width;
            chart.Height = 160;
            chart.ChartType = ChartType.StackBarChart;
            chart.StackChart.StackStyle = StackStyle.Complete;

            //chart.Data.SwapRowsAndColumns = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br />����&nbsp;<b><DATA_VALUE_ITEM:N0>%</b></span>";

            chart.Data.ZeroAligned = true;
            chart.Legend.SpanPercentage = 35;
            chart.Legend.Margins.Bottom = 80;

            chart.Legend.Visible = false;
            chart.BackColor = Color.FromArgb(0xa8a8a8);

            // chart.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            chart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            chart.Axis.Y.Labels.HorizontalAlign = StringAlignment.Far;
            chart.Axis.Y.Labels.VerticalAlign = StringAlignment.Far;

            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;

            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.Visible = seriesLabelVisible;
            chart.Axis.Y.Labels.SeriesLabels.FontColor = Color.Black;
            chart.Axis.X.Labels.FontColor = Color.Black;
            chart.Axis.Y.Extent = seriesLabelVisible ? 120 : 20;

            chart.Axis.X.Labels.Visible = true;
            chart.Border.Color = Color.FromArgb(0xa8a8a8);

            chart.Style.Add("margin-left", "-25px");
        }

        private void SetupCustomSkin(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = false;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 2; i <= 4; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {                
                case 2:
                    {
                        return Color.FromArgb(145, 10, 149);
                    }
                case 3:
                    {
                        return Color.FromArgb(143, 199, 219);
                    }
                case 4:
                    {
                        return Color.FromArgb(176, 217, 117);
                    }
            }
            return Color.White;
        }

        private static Color GetStopColor(int i)
        {
            switch (i)
            {               
                case 2:
                    {
                        return Color.FromArgb(145, 10, 149);
                    }
                case 3:
                    {
                        return Color.FromArgb(11, 100, 131);
                    }
                case 4:
                    {
                        return Color.FromArgb(102, 168, 9);
                    }
            }
            return Color.White;
        }
    }

}
