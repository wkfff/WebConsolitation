using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class GaugeIndicator : UserControl
    {
        #region ��������

        public string Title
        {
            get { return gaugeTitle.Text; }
            set { gaugeTitle.Text = value; }
        }

        public string Name
        {
            get { return indNameLabel.Text; }
            set { indNameLabel.Text = value; }
        }

        public string Content
        {
            get { return indContentLabel.Text; }
            set { indContentLabel.Text = value; }
        }

        public string Formula
        {
            get { return indFormulaLabel.Text; }
            set { indFormulaLabel.Text = value; }
        }

        public string NormalValue
        {
            get { return indNormValueLabel.Text; }
            set { indNormValueLabel.Text = value; }
        }

        public string RankValue
        {
            get { return indRankValue.Text; }
            set { indRankValue.Text = value; }
        }

        public string FactValue
        {
            get { return indFactValue.Text; }
            set { indFactValue.Text = value; }
        }

        public string MinValue
        {
            get { return indMinValue.Text; }
            set { indMinValue.Text = value; }
        }

        public string MaxValue
        {
            get { return indMaxValue.Text; }
            set { indMaxValue.Text = value; }
        }

        /// <summary>
        /// �������� ����������
        /// </summary>
        public double MarkerValue
        {
            get { return (double)((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Markers[1].Value; }
            set { ((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Markers[1].Value = value; }
        }

        /// <summary>
        /// �������� ������ ����������
        /// </summary>
        public double LimitValue
        {
            get { return (double)((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Markers[0].Value; }
            set { ((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Markers[0].Value = value; }
        }

        /// <summary>
        /// ������� ��� ������
        /// </summary>
        public double AxisMin
        {
            get { return (double)((LinearGauge) ultraGauge.Gauges[0]).Scales[0].Axis.GetStartValue(); }
            set { ((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Axis.SetStartValue(value); }
        }

        /// <summary>
        /// �������� ��� ������
        /// </summary>
        public double AxisMax
        {
            get { return (double)((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Axis.GetEndValue(); }
            set { ((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Axis.SetEndValue(value); }
        }

        /// <summary>
        /// �������� ������� ��� ������
        /// </summary>
        public double AxisTickInterval
        {
            get { return (double) ((LinearGauge) ultraGauge.Gauges[0]).Scales[0].Axis.GetTickmarkInterval(); }
            set { ((LinearGauge) ultraGauge.Gauges[0]).Scales[0].Axis.SetTickmarkInterval(value); }
        }

        /// <summary>
        /// ��������� ���� �������� ���������� ������
        /// </summary>
        public Color BEStartColor
        {
            get
            {
                return ((SimpleGradientBrushElement)((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Markers[1].BrushElement).
                  StartColor;
            }
            set {
                ((SimpleGradientBrushElement) ((LinearGauge) ultraGauge.Gauges[0]).Scales[0].Markers[1].BrushElement).
                    StartColor = value; }
        }

        /// <summary>
        /// �������� ���� �������� ��������� ������
        /// </summary>
        public Color BEEndColor
        {
            get
            {
                return ((SimpleGradientBrushElement)((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Markers[1].BrushElement).
                  EndColor;
            }
            set
            {
                ((SimpleGradientBrushElement)((LinearGauge)ultraGauge.Gauges[0]).Scales[0].Markers[1].BrushElement).
                    EndColor = value;
            }
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        public string ToolTip
        {
            get { return ultraGauge.ToolTip; }
            set { ultraGauge.ToolTip = value; }
        }

        /// <summary>
        /// ������ 
        /// </summary>
        public string Width
        {
            get { return gaugeTable.Width; }
            set { gaugeTable.Width = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public string Height
        {
            get { return gaugeTable.Height; }
            set { gaugeTable.Height = value; }
        }

        #endregion

        public string GaugeImageURL
        {
            get { return ultraGauge.DeploymentScenario.ImageURL; }
            set { ultraGauge.DeploymentScenario.ImageURL = value; }
        }

        public void SetVisibleScales(bool visible)
        {
            ((LinearGauge) ultraGauge.Gauges[0]).Scales[0].Visible = visible;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ultraGauge.DeploymentScenario.FilePath = "../../TemporaryImages";
            ((SimpleGradientBrushElement) ((LinearGauge) ultraGauge.Gauges[0]).Scales[0].Markers[1].BrushElement).GradientStyle = Gradient.Horizontal;
        }
    }
}