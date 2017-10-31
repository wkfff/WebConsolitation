using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class ColorScalingTypeConverter : EnumConverter
    {
        const string csNone = "Нет";
        const string csIncreasing = "Возрастающий";
        const string csDecreasing = "Убывающий";
        const string csOscillating = "Колеблющийся";
        const string csRandom = "Случайный";

        public ColorScalingTypeConverter(Type type)
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
                case csNone: return ColorScaling.None;
                case csIncreasing: return ColorScaling.Increasing;
                case csDecreasing: return ColorScaling.Decreasing;
                case csOscillating: return ColorScaling.Oscillating;
                case csRandom: return ColorScaling.Random;
            }
            return ColorScaling.None;
        }

        public static string ToString(object value)
        {
            switch ((ColorScaling)value)
            {
                case ColorScaling.None: return csNone;
                case ColorScaling.Increasing: return csIncreasing;
                case ColorScaling.Decreasing: return csDecreasing;
                case ColorScaling.Oscillating: return csOscillating;
                case ColorScaling.Random: return csRandom;
            }
            return string.Empty;
        }
    }
}