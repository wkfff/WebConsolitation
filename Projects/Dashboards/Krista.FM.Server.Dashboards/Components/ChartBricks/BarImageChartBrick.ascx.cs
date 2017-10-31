using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components.Components.ChartBricks
{
    public partial class BarImageChartBrick : UltraChartBrick
    {
        public void SetBarRange(double startValue, double endValue)
        {
            ChartControl.Axis.X.RangeMin = startValue;
            ChartControl.Axis.X.RangeMax = endValue;
        }
        
        public void SetBarValue(double value)
        {
            ChartControl.Series.Clear();

            NumericSeries series = new NumericSeries();
            series.Label = "Label";
            series.Points.Add(new NumericDataPoint(value, "BarValue", false));
            ChartControl.Series.Add(series);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected override void SetChartAppearance()
        {
            base.SetChartAppearance();  

            ChartControl.ChartType = ChartType.BarChart;

            ChartControl.Axis.Y.Labels.Visible = false;
            ChartControl.Axis.Y.Labels.SeriesLabels.Visible = false;
            ChartControl.Axis.Y.Visible = false;

            ChartControl.Axis.X.Labels.Visible = false;
            ChartControl.Axis.X.Labels.SeriesLabels.Visible = false;
            ChartControl.Axis.X.Visible = false;

            ChartControl.Axis.X.RangeType = AxisRangeType.Custom;
            ChartControl.BackgroundImageFileName = Server.MapPath("~/images/workers.png");
            ChartControl.BackgroundImageStyle = ImageFitStyle.Tiled;

            ChartControl.ColorModel.ModelStyle = ColorModels.CustomSkin;
            ChartControl.ColorModel.Skin.PEs.Add(GetTransparentPE(235));
        }

        private PaintElement GetTransparentPE(byte opacity)
        {
            PaintElement pe = new PaintElement();
            pe.ElementType = PaintElementType.SolidFill;
            pe.Fill = Color.White;
            pe.FillStopColor = Color.White;
            pe.FillStopOpacity = opacity;
            pe.FillOpacity = opacity;
            pe.Stroke = Color.White;
            return pe;
        }
    }
}
