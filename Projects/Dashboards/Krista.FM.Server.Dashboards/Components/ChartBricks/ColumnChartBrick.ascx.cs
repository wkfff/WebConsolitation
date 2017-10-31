using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components.ChartBricks
{
    public partial class ColumnChartBrick : UltraChartBrick
    {
        private string yAxisLabelFormat;
        private bool seriesLabelWrap;
        private bool xAxisLabelVisible = true;
        private int xAxisSeriesLabelWidth = 25;

        public string YAxisLabelFormat
        {
            get { return yAxisLabelFormat; }
            set { yAxisLabelFormat = value; }
        }

        public bool SeriesLabelWrap
        {
            get { return seriesLabelWrap; }
            set { seriesLabelWrap = value; }
        }

        public bool XAxisLabelVisible
        {
            get { return xAxisLabelVisible; }
            set { xAxisLabelVisible = value; }
        }

        public int XAxisSeriesLabelWidth
        {
            get { return xAxisSeriesLabelWidth; }
            set { xAxisSeriesLabelWidth = value; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected override void ChartControl_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text) primitive;
                    if (seriesLabelWrap && text.Row == -1)
                    {
                        text.bounds.Width = xAxisSeriesLabelWidth;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                        text.labelStyle.WrapText = true;
                    }
                }
            }
        }

        protected override void SetChartAppearance()
        {
            base.SetChartAppearance();

            ChartControl.ChartType = ChartType.ColumnChart;

            ChartControl.Axis.X.Labels.SeriesLabels.Visible = true;
            ChartControl.Axis.X.Labels.Visible = xAxisLabelVisible;

            ChartControl.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            ChartControl.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            
            if (seriesLabelWrap)
            {
                ChartControl.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;

                WrapTextAxisLabelLayoutBehavior wrapBehavior = new WrapTextAxisLabelLayoutBehavior();
                wrapBehavior.EnableRollback = false;
                ChartControl.Axis.X.Labels.SeriesLabels.Layout.BehaviorCollection.Add(wrapBehavior);
            }

            ChartControl.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            ChartControl.Axis.Y.Labels.ItemFormatString = String.Format("<DATA_VALUE:{0}>", YAxisLabelFormat);
        }
    }
}