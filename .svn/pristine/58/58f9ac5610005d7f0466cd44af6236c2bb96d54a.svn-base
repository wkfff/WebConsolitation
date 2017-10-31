using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.MDXExpert;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MultiRadialGaugeLabelsBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private RadialGaugeScaleLabelsAppearance _labels;
        private GaugeFormatBrowseClass _format;
        private MultipleGaugeReportElement _gaugeElement;

        #endregion

        #region Свойства

        [Description("Одинаковый размер шрифта")]
        [DisplayName("Одинаковый размер шрифта")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool EqualFontScaling
        {
            get { return this._labels.EqualFontScaling ; }
            set
            {
                //this._labels.EqualFontScaling = value;
                SetEqualFontScaling(value);
            }
        }

        [Description("Смещение меток (в процентах)")]
        [DisplayName("Смещение")]
        [Browsable(true)]
        public double Extent
        {
            get { return this._labels.Extent; }
            set
            {
                //this._labels.Extent = value;
                SetExtent(value);
            }
        }

        [Description("Шрифт")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return this._labels.Font; }
            set
            {
                //this._labels.Font = value;
                SetFont(value);
            }
        }

        [Description("Ориентация")]
        [DisplayName("Ориентация")]
        [TypeConverter(typeof(RadialLabelOrientationConverter))]
        [Browsable(true)]
        public RadialLabelOrientation Orientation
        {
            get { return this._labels.Orientation; }
            set
            {
                //this._labels.Orientation = value;
                SetOrientation(value);
            }
        }

        [Description("Формат меток")]
        [DisplayName("Формат")]
        [Browsable(true)]
        public GaugeFormatBrowseClass Format
        {
            get { return this._format; }
            set { this._format = value; }
        }

        #endregion

        public MultiRadialGaugeLabelsBrowseClass(RadialGaugeScaleLabelsAppearance labels, MultipleGaugeReportElement gaugeElement)
        {
            this._labels = labels;
            this._format = new GaugeFormatBrowseClass(this._labels.FormatString);
            this._format.FormatChanged += new ValueFormatEventHandler(_format_FormatChanged);
            this._gaugeElement = gaugeElement;
        }

        void _format_FormatChanged()
        {
            ((RadialGauge)this._gaugeElement.MainGauge.Gauges[0]).Scales[0].Labels.FormatString = this.Format.ApplyFormatToText(this._labels.FormatString, this.Format.FormatString);
            foreach (ExpertGauge gauge in this._gaugeElement.Gauges)
            {
                ((RadialGauge)gauge.Gauges[0]).Scales[0].Labels.FormatString = this.Format.ApplyFormatToText(this._labels.FormatString, this.Format.FormatString);
            }
            this._gaugeElement.RefreshDigitalGauge();
        }

        private void SetOrientation(RadialLabelOrientation value)
        {
            ((RadialGauge)this._gaugeElement.MainGauge.Gauges[0]).Scales[0].Labels.Orientation = value;
            foreach (ExpertGauge gauge in this._gaugeElement.Gauges)
            {
                ((RadialGauge)gauge.Gauges[0]).Scales[0].Labels.Orientation = value;
            }
        }

        private void SetRotationAngle(double value)
        {
            ((RadialGauge)this._gaugeElement.MainGauge.Gauges[0]).Scales[0].Labels.RotationAngle = value;
            foreach (ExpertGauge gauge in this._gaugeElement.Gauges)
            {
                ((RadialGauge)gauge.Gauges[0]).Scales[0].Labels.RotationAngle = value;
            }
        }

        private void SetFont(Font value)
        {
            ((RadialGauge)this._gaugeElement.MainGauge.Gauges[0]).Scales[0].Labels.Font = value;
            foreach (ExpertGauge gauge in this._gaugeElement.Gauges)
            {
                ((RadialGauge)gauge.Gauges[0]).Scales[0].Labels.Font = value;
            }
        }

        private void SetExtent(double value)
        {
            ((RadialGauge)this._gaugeElement.MainGauge.Gauges[0]).Scales[0].Labels.Extent = value;
            foreach (ExpertGauge gauge in this._gaugeElement.Gauges)
            {
                ((RadialGauge)gauge.Gauges[0]).Scales[0].Labels.Extent = value;
            }
        }

        private void SetEqualFontScaling(bool value)
        {
            ((RadialGauge)this._gaugeElement.MainGauge.Gauges[0]).Scales[0].Labels.EqualFontScaling = value;
            foreach (ExpertGauge gauge in this._gaugeElement.Gauges)
            {
                ((RadialGauge)gauge.Gauges[0]).Scales[0].Labels.EqualFontScaling = value;
            }
        }

        public override string ToString()
        {
            return "";
        }

    }


}
