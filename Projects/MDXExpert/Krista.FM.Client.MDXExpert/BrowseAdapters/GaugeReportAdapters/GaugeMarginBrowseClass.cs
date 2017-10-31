using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GaugeMarginBrowseClass
    {
        #region ����

        private Margin margin;
        private MultipleGaugeReportElement gaugeElement;
        private GaugeReportElement singleGaugeElement;

        #endregion

        #region ��������

        /// <summary>
        /// ����� ����
        /// </summary>
        [Description("����� ����")]
        [DisplayName("�����")]
        [DefaultValue(5)]
        [Browsable(true)]
        public double Left
        {
            get { return margin.Left; }
            set
            {
                margin.Left = value;
                if (this.gaugeElement != null)
                    this.gaugeElement.Margins = margin;
                if (this.singleGaugeElement != null)
                    this.singleGaugeElement.Margin = margin;
            }
        }

        /// <summary>
        /// ������ ����
        /// </summary>
        [Description("������ ����")]
        [DisplayName("������")]
        [DefaultValue(5)]
        [Browsable(true)]
        public double Right
        {
            get { return margin.Right; }
            set
            {
                margin.Right = value;
                if (this.gaugeElement != null)
                    this.gaugeElement.Margins = margin;
                if (this.singleGaugeElement != null)
                    this.singleGaugeElement.Margin = margin;

            }
        }

        /// <summary>
        /// ������� ����
        /// </summary>
        [Description("������� ����")]
        [DisplayName("�������")]
        [DefaultValue(5)]
        [Browsable(true)]
        public double Top
        {
            get { return margin.Top; }
            set
            {
                margin.Top = value;
                if (this.gaugeElement != null)
                    this.gaugeElement.Margins = margin;
                if (this.singleGaugeElement != null)
                    this.singleGaugeElement.Margin = margin;


            }
        }

        /// <summary>
        /// ������ ����
        /// </summary>
        [Description("������ ����")]
        [DisplayName("������")]
        [DefaultValue(5)]
        [Browsable(true)]
        public double Bottom
        {
            get { return margin.Bottom; }
            set
            {
                margin.Bottom = value;
                if (this.gaugeElement != null)
                    this.gaugeElement.Margins = margin;
                if (this.singleGaugeElement != null)
                    this.singleGaugeElement.Margin = margin;


            }
        }

        [Description("��� ��������� ������")]
        [DisplayName("��� ��������� ������")]
        [TypeConverter(typeof(MeasureConverter))]
        [DefaultValue(5)]
        [Browsable(true)]
        public Measure Measure
        {
            get { return margin.Measure; }
            set
            {
                margin.Measure = value;
                if (this.gaugeElement != null)
                    this.gaugeElement.Margins = margin;
                if (this.singleGaugeElement != null)
                    this.singleGaugeElement.Margin = margin;

            }
        }

        #endregion

        public GaugeMarginBrowseClass(GaugeReportElement gaugeElement)
        {
            this.singleGaugeElement = gaugeElement;
            this.margin = gaugeElement.Margin;
        }

        public GaugeMarginBrowseClass(MultipleGaugeReportElement gaugeElement)
        {
            this.gaugeElement = gaugeElement;
            this.margin = gaugeElement.Margins;
        }


        public override string ToString()
        {
            return Top + "; " + Left + "; " + Bottom + "; " + Right;
        }
    }
}