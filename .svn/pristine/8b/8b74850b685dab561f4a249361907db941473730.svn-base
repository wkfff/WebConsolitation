using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using System.Drawing;
using Infragistics.WebUI.UltraWebGauge;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class BarGaugeIndicator : UserControl
    {
        private LinearGauge Gauge
        {
            get { return (LinearGauge)GaugeControl.Gauges[0]; }
        }

        public LinearGaugeScale MinorScale
        {
            get { return Gauge.Scales[1]; }
        }

        private LinearGaugeScale MajorScale
        {
            get { return Gauge.Scales[2]; }
        }

        private LinearGaugeScale MarkerScale
        {
            get { return Gauge.Scales[1]; }
        }

        private LinearGaugeMarker Marker
        {
            get { return MarkerScale.Markers[0]; }
        }

        private GaugeRange MarkerRange
        {
            get { return MarkerScale.Ranges[0]; }
        }
        private GaugeRange MajorRange
        {
            get { return MajorScale.Ranges[0]; }
        }

        private Axis Axis
        {
            get { return MajorScale.Axes[0]; }
        }

        public void SetRange(double startValue, double endValue, double interval)
        {
            //            MarkerRange.StartValue = startValue;
            //            MarkerRange.EndValue = endValue;
            //
            //            MajorRange.StartValue = startValue;
            //            MajorRange.EndValue = endValue;
            //
            //            ((NumericAxis) Axis).StartValue = startValue;
            //            ((NumericAxis) Axis).EndValue = endValue;
            //            ((NumericAxis) Axis).TickmarkInterval = interval;

            MajorScale.Axis.SetStartValue(startValue);
            MajorScale.Axis.SetEndValue(endValue);

            MinorScale.Axis.SetStartValue(startValue);
            MinorScale.Axis.SetEndValue(endValue);

            MajorScale.Labels.FormatString = "";
        }

        public double IndicatorValue
        {
            get { return Convert.ToDouble(Marker.Value); }
            set { Marker.Value = value; }
        }

        public double MarkerPrecision
        {
            get { return Convert.ToDouble(Marker.Precision); }
            set { Marker.Precision = value; }
        }

        public string TitleText
        {
            get { return GaugeTitle.Text; }
            set { GaugeTitle.Text = value; }
        }

        public void SetImageUrl(int imageIndex)
        {
            GaugeControl.DeploymentScenario.ImageURL = String.Format("../../TemporaryImages/gaugeIndicator_{0}_#SEQNUM(500).png", imageIndex);
        }

        public double Width
        {
            get { return GaugeControl.Width.Value; }
            set { GaugeControl.Width = Unit.Pixel((int)value); }
        }

        public double Height
        {
            get { return GaugeControl.Height.Value; }
            set { GaugeControl.Height = Unit.Pixel((int)value); }
        }

        public HtmlTable GaugeContainer
        {
            get { return GaugeTable; }
        }

        public void SetMargins(int value)
        {
            GaugeControl.Style.Add("margin-top", value.ToString());
            GaugeControl.Style.Add("margin-bottom", (-value).ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GaugeControl.DeploymentScenario.FilePath = "../../TemporaryImages";
        }

        public void SaveAsImage(string path)
        {
            GaugeControl.SaveTo(path, GaugeImageType.Png, new Size((int)Width, (int)Height));
        }

        public  UltraGauge GetGaugeControl
        {
            get { return GaugeControl; }
        }
    }
}