using System;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.Components.ChartBricks
{
    public partial class PieChartBrick : UltraChartBrick
    {
        private double othersCategoryPercent = 0;
        private int startAngle = 0;

        public double OthersCategoryPercent
        {
            get { return othersCategoryPercent; }
            set { othersCategoryPercent = value; }
        }

        public int StartAngle
        {
            get { return startAngle; }
            set { startAngle = value; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            ChartControl.ChartDrawItem += new ChartDrawItemEventHandler(ChartControl_ChartDrawItem);
            ChartControl.PieChart.OthersCategoryText = "Прочие";
        }

        protected override void SetChartAppearance()
        {
            base.SetChartAppearance();

            ChartControl.PieChart.OthersCategoryPercent = othersCategoryPercent;
            ChartControl.PieChart.StartAngle = startAngle;
            ChartControl.ChartType = ChartType.PieChart;
        }

        protected void ChartControl_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((ChartControl.Legend.Location == LegendLocation.Top) || (ChartControl.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)ChartControl.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)ChartControl.Legend.SpanPercentage * (int)ChartControl.Width.Value / 100) - 20;
                }

                widthLegendLabel -= ChartControl.Legend.Margins.Left + ChartControl.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }
    }
}