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
    public class LinearGaugeLabelsBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private LinearGaugeScaleLabelsAppearance _labels;
        private GaugeFormatBrowseClass _format;
        private GaugeReportElement _gaugeElement;

        #endregion

        #region Свойства

        [Description("Одинаковый размер шрифта")]
        [DisplayName("Одинаковый размер шрифта")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool EqualFontScaling
        {
            get { return this._labels.EqualFontScaling ; }
            set { this._labels.EqualFontScaling = value; }
        }

        [Description("Смещение меток (в процентах)")]
        [DisplayName("Смещение")]
        [Browsable(true)]
        public double Extent
        {
            get { return this._labels.Extent; }
            set { this._labels.Extent = value; }
        }

        [Description("Шрифт")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return this._labels.Font; }
            set { this._labels.Font = value; }
        }

        [Description("Угол поворота")]
        [DisplayName("Угол поворота")]
        [Browsable(true)]
        public double RotationAngle
        {
            get { return this._labels.RotationAngle; }
            set { this._labels.RotationAngle = value; }
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

        public LinearGaugeLabelsBrowseClass(LinearGaugeScaleLabelsAppearance labels, GaugeReportElement gaugeElement)
        {
            this._labels = labels;
            this._format = new GaugeFormatBrowseClass(this._labels.FormatString);
            this._format.FormatChanged += new ValueFormatEventHandler(_format_FormatChanged);
            this._gaugeElement = gaugeElement;
        }

        void _format_FormatChanged()
        {
            this._labels.FormatString = this.Format.ApplyFormatToText(this._labels.FormatString, this.Format.FormatString);
            this._gaugeElement.RefreshDigitalGauge();
        }


        public override string ToString()
        {
            return "";
        }

    }


}
