using System.ComponentModel;
using System.Drawing;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartSize
    {
        private UltraChart chart;

        [Description("Ширина")]
        [DisplayName("Ширина")]
        public int Width
        {
            get { return chart.Width; }
            set
            {
                chart.Size = new Size(value, chart.Height);
            }
        }

        [Description("Высота")]
        [DisplayName("Высота")]
        public int Height
        {
            get { return chart.Height; }
            set
            {
                chart.Size = new Size(chart.Width, value);
            }
        }

        public ChartSize(UltraChart chart)
        {
            this.chart = chart;
        }

        public override string ToString()
        {
            return Width + "; " + Height;
        }
    }
}