using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class NullHandlingTypeConverter : EnumConverter
    {
        const string nhZero = "Нулевые";
        const string nhDontPlot = "Не выводить";
        const string nhInterpolateSimple = "Простая интерполяция";
        const string nhInterpolateCustom = "Выборочная интерполяция";

        public NullHandlingTypeConverter(Type type)
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
                case nhZero: return NullHandling.Zero;
                case nhDontPlot: return NullHandling.DontPlot;
                case nhInterpolateSimple: return NullHandling.InterpolateSimple;
                case nhInterpolateCustom: return NullHandling.InterpolateCustom;
            }
            return NullHandling.Zero;
        }

        public static string ToString(object value)
        {
            switch ((NullHandling)value)
            {
                case NullHandling.Zero: return nhZero;
                case NullHandling.DontPlot: return nhDontPlot;
                case NullHandling.InterpolateSimple: return nhInterpolateSimple;
                case NullHandling.InterpolateCustom: return nhInterpolateCustom;
            }
            return string.Empty;
        }
    }
}