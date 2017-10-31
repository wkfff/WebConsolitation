using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;

namespace Krista.FM.Server.Dashboards.Components.Components.Gauges
{
    public partial class RainbowGaugeIndicator : UserControl
    {
        private string gaugeURLPrefix = "../../";

        private RadialGauge Gauge
        {
            get { return (RadialGauge)GaugeControl.Gauges[0]; }
        }

        private DigitalGauge Label
        {
            get { return (DigitalGauge)GaugeControl.Gauges[1]; }
        }

        private RadialGaugeScale MarkerScale
        {
            get { return Gauge.Scales[0]; }
        }

        private RadialGaugeMarker Marker
        {
            get { return MarkerScale.Markers[0]; }
        }

        public double IndicatorValue
        {
            get { return Convert.ToDouble(Marker.Value); }
            set { Marker.Value = value; }
        }

        public string LabelText
        {
            get { return Label.Text.ToString(); }
            set { Label.Text = value; }
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

        public UltraGauge GetGaugeControl
        {
            get { return GaugeControl; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GaugeControl.DeploymentScenario.ImageURL = string.Format("{0}TemporaryImages/rainbowGaugeIndicator#SEQNUM(500).png", gaugeURLPrefix);
            GaugeControl.DeploymentScenario.FilePath = string.Format("{0}TemporaryImages", gaugeURLPrefix);
        }
    }
}