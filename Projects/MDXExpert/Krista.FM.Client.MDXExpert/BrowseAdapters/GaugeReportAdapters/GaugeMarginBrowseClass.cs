using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GaugeMarginBrowseClass
    {
        #region Поля

        private Margin margin;
        private MultipleGaugeReportElement gaugeElement;
        private GaugeReportElement singleGaugeElement;

        #endregion

        #region Свойства

        /// <summary>
        /// Левое поле
        /// </summary>
        [Description("Левое поле")]
        [DisplayName("Левое")]
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
        /// Правое поле
        /// </summary>
        [Description("Правое поле")]
        [DisplayName("Правое")]
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
        /// Верхнее поле
        /// </summary>
        [Description("Верхнее поле")]
        [DisplayName("Верхнее")]
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
        /// Нижнее поле
        /// </summary>
        [Description("Нижнее поле")]
        [DisplayName("Нижнее")]
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

        [Description("Тип измерения границ")]
        [DisplayName("Тип измерения границ")]
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