using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;

namespace Krista.FM.Server.Dashboards.Components.Components.Gauges
{
    public partial class RadialGaugeIndicator : UserControl
    {
        private RadialGauge Gauge
        {
            get { return (RadialGauge)GaugeControl.Gauges[0]; }
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
		
    	public double AxisMin
    	{
    		set { ((NumericAxis) (MarkerScale.Axes[0])).StartValue = value; }
			get { return ((NumericAxis) (MarkerScale.Axes[0])).StartValue; }
    	}

		public double AxisMax
		{
			set { ((NumericAxis)(MarkerScale.Axes[0])).EndValue = value; }
			get { return ((NumericAxis)(MarkerScale.Axes[0])).EndValue; }
		}

		public double AxisTickmarkInterval
		{
			set { ((NumericAxis)(MarkerScale.Axes[0])).TickmarkInterval = value; }
			get { return ((NumericAxis)(MarkerScale.Axes[0])).TickmarkInterval; }
		}

		private string temporaryUrlPrefix;
		public string TemporaryUrlPrefix
		{
			set
			{
				temporaryUrlPrefix = value;
				GaugeControl.DeploymentScenario.FilePath = String.Format("{0}/TemporaryImages", temporaryUrlPrefix);
				GaugeControl.DeploymentScenario.ImageURL = String.Format("{0}/{1}_{2}.png", GaugeControl.DeploymentScenario.FilePath, ID, Guid.NewGuid());
			}
			get { return temporaryUrlPrefix; }
		}

		protected void Page_PreLoad(object sender, EventArgs e)
		{
			TemporaryUrlPrefix = "../..";
		}

    	protected void Page_Load(object sender, EventArgs e)
        {
            // empty now
        }

		public void SaveAsImage(string path)
		{
			GaugeControl.SaveTo(path, GaugeImageType.Png, new Size((int)Width, (int)Height));
		}
    }
}