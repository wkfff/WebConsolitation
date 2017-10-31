using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.Components.ChartBricks
{
    public partial class BarChartBrick : UltraChartBrick
    {
        private string xAxisLabelFormat;
        private bool seriesLabelWrap;
        private bool x2AxisVisible = false;
        private bool y2AxisVisible = false;

        public string XAxisLabelFormat
        {
            get { return xAxisLabelFormat; }
            set { xAxisLabelFormat = value; }
        }

        public bool X2AxisVisible
        {
            get { return x2AxisVisible; }
            set { x2AxisVisible = value; }
        }

        public int X2AxisExtent
        {
            get { return ChartControl.Axis.X2.Extent; }
            set { ChartControl.Axis.X2.Extent = value; }
        }

        public bool Y2AxisVisible
        {
            get { return y2AxisVisible; }
            set { y2AxisVisible = value; }
        }

        public int Y2AxisExtent
        {
            get { return ChartControl.Axis.Y2.Extent; }
            set { ChartControl.Axis.Y2.Extent = value; }
        }

        public bool SeriesLabelWrap
        {
            get { return seriesLabelWrap; }
            set { seriesLabelWrap = value; }
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
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text)primitive;
                    if (seriesLabelWrap && text.Row == -1)
                    {
                        text.bounds.Height = 40;

                        text.bounds = new Rectangle(text.bounds.Left, text.bounds.Y - text.bounds.Height / 3, text.bounds.Width, text.bounds.Height);

                        text.labelStyle.HorizontalAlign = StringAlignment.Center;

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

            ChartControl.ChartType = ChartType.BarChart;

            ChartControl.Axis.Y.Labels.SeriesLabels.Visible = true;
            ChartControl.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            ChartControl.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
            
            if (seriesLabelWrap)
            {
                ChartControl.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;

                WrapTextAxisLabelLayoutBehavior wrapBehavior = new WrapTextAxisLabelLayoutBehavior();
                wrapBehavior.EnableRollback = false;
                ChartControl.Axis.Y.Labels.SeriesLabels.Layout.BehaviorCollection.Add(wrapBehavior);
            }

            ChartControl.Axis.Y.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            ChartControl.Axis.X.Labels.ItemFormatString = String.Format("<DATA_VALUE:{0}>", XAxisLabelFormat);
            ChartControl.Axis.X2.Labels.ItemFormatString = String.Format("<DATA_VALUE:{0}>", XAxisLabelFormat);
            
            ChartControl.Axis.X2.Visible = x2AxisVisible;
            ChartControl.Axis.X2.Labels.Visible = true;
            ChartControl.Axis.X2.TickmarkStyle = AxisTickStyle.Smart;
            ChartControl.Axis.X2.LineThickness = 1;

            ChartControl.Axis.Y2.Visible = y2AxisVisible;
        }
    }
}