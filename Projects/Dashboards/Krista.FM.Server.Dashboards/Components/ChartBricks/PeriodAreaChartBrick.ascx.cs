using System;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.Components.ChartBricks
{
    public partial class PeriodAreaChartBrick : UltraChartBrick
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected override void SetChartAppearance()
        {
            base.SetChartAppearance();

            ChartControl.ChartType = ChartType.AreaChart;

            ChartControl.Axis.X.Labels.SeriesLabels.Visible = true;
            ChartControl.Axis.Y.Labels.ItemFormatString = String.Format("<DATA_VALUE:{0}>", DataFormatString);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            ChartControl.AreaChart.LineAppearances.Add(lineAppearance);

            ChartControl.TitleLeft.Visible = true;
       }
    }
}