using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class NumericAxisTypeConverter : EnumConverter
    {
        const string ntLinear = "Линейная";
        const string ntLogarithmic = "Логарифмическая";

        public NumericAxisTypeConverter(Type type)
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
                case ntLinear: return NumericAxisType.Linear;
                case ntLogarithmic: return NumericAxisType.Logarithmic;
            }
            return NumericAxisType.Linear;
        }

        public static string ToString(object value)
        {
            switch ((NumericAxisType)value)
            {
                case NumericAxisType.Linear: return ntLinear;
                case NumericAxisType.Logarithmic: return ntLogarithmic;
            }
            return string.Empty;
        }
    }
}