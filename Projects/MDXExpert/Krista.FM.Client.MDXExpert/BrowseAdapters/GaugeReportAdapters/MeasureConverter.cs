using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Client.MDXExpert
{
    class MeasureConverter : EnumConverter
    {
        const string mPixels = "�������";
        const string mPercent = "��������";

        public MeasureConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch ((string)value)
            {
                case mPercent: return Measure.Percent;
                case mPixels: return Measure.Pixels;
            }
            return Measure.Pixels;
        }

        public static string ToString(object value)
        {
            switch ((Measure)value)
            {
                case Measure.Percent: return mPercent;
                case Measure.Pixels: return mPixels;
            }
            return string.Empty;
        }
    }
}