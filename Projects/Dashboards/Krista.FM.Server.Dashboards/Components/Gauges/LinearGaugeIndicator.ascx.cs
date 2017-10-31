using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class LinearGaugeIndicator : UserControl
    {
        private LinearGauge Gauge
        {
            get { return (LinearGauge) GaugeControl.Gauges[0]; }
        }

        private LinearGaugeScale MinorScale
        {
            get { return Gauge.Scales[0]; }
        }

        private LinearGaugeScale MajorScale
        {
            get { return Gauge.Scales[1]; }
        }

        private LinearGaugeScale MarkerScale
        {
            get { return Gauge.Scales[2]; }
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

        public BoxAnnotation MarkerAnnotation
        {
            get { return (BoxAnnotation)Gauge.GaugeComponent.Annotations[0]; }
        }

        public void SetRange(double startValue, double endValue, double interval)
        {
            //MarkerRange.StartValue = startValue;
            //MarkerRange.EndValue = endValue;

            //MajorRange.StartValue = startValue;
            //MajorRange.EndValue = endValue;

            //((NumericAxis)Axis).StartValue = startValue;
            //((NumericAxis)Axis).EndValue = endValue;
            //((NumericAxis)Axis).TickmarkInterval = interval;
           
            MajorScale.Axis.SetStartValue(startValue);
            MajorScale.Axis.SetEndValue(endValue);

            MarkerScale.Axis.SetStartValue(startValue);
            MarkerScale.Axis.SetEndValue(endValue);

            MajorRange.StartValue = startValue;
            MajorRange.EndValue = endValue;
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

    	public BrushElement BrushElement
    	{
			get { return Gauge.BrushElement; }
			set { Gauge.BrushElement = value; }
    	}

        public HtmlTable GaugeContainer
        {
            get { return GaugeTable; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GaugeControl.DeploymentScenario.FilePath = "../../TemporaryImages";
        }

        public string Tooltip
        {
            get { return GaugeControl.ToolTip; }
            set { GaugeControl.ToolTip = value; }
        }

        public void SetMarkerAnnotation(double value)
        {
            MarkerAnnotation.Bounds = new Rectangle(Convert.ToInt32(value * 16), MarkerAnnotation.Bounds.Y, MarkerAnnotation.Bounds.Width, MarkerAnnotation.Bounds.Height);
            MarkerAnnotation.Label.FormatString = String.Format("{0:N1}", value);
        }

        public void SaveAsImage(string path)
        {
            GaugeControl.SaveTo(path,GaugeImageType.Png,new Size((int)Width, (int)Height)); 
        }
    }
}