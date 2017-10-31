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
    public partial class StackColumnChartBrick : UltraChartBrick
    {
        private string yAxisLabelFormat;
        private bool seriesLabelWrap;

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
                    Text text = (Text)primitive;
                    if (seriesLabelWrap && text.Row == -1)
                    {
                        FontStyle style = text.labelStyle.Font.Style; 
                        
                        text.bounds.Width = 25;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8, style);
                        text.labelStyle.WrapText = true;
                    }
                }
            }
        }

        protected override void SetChartAppearance()
        {
            base.SetChartAppearance();

            ChartControl.ChartType = ChartType.StackColumnChart;

            ChartControl.Axis.X.Labels.SeriesLabels.Visible = true;
            ChartControl.Axis.X.Labels.Visible = true;

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