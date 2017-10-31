using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class MeasureTypeConverter : EnumConverter
    {
        const string mPixels = "Пиксели";
        const string mPercentage = "Проценты";

        public MeasureTypeConverter(Type type)
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
                case mPercentage: return MeasureType.Percentage;
                case mPixels: return MeasureType.Pixels;
            }
            return MeasureType.Pixels;
        }

        public static string ToString(object value)
        {
            switch ((MeasureType)value)
            {
                case MeasureType.Percentage: return mPercentage;
                case MeasureType.Pixels: return mPixels;
            }
            return string.Empty;
        }
    }
}