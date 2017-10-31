using System;
using System.Drawing;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.Components.ChartBricks
{
    public partial class LegendChartBrick : UltraChartBrick
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            ChartControl.InvalidDataReceived +=new ChartDataInvalidEventHandler(ChartControl_InvalidDataReceived);
        }

        protected override void SetChartAppearance()
        {
            base.SetChartAppearance();

            ChartControl.Legend.Visible = true;
            ChartControl.Legend.Location = LegendLocation.Bottom;
            ChartControl.Legend.SpanPercentage = 100;

            ChartControl.Tooltips.Display = TooltipDisplay.Never;

            ChartControl.Axis.X.Visible = false;
            ChartControl.Axis.Y.Visible = false;
        }

        public static void ChartControl_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = String.Empty;
        }
    }
}