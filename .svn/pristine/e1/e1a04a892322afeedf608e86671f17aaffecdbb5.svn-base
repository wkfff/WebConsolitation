using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class AngleUnitTypeConverter : EnumConverter
    {
        const string auDegrees = "Градусы";
        const string auRadians = "Радианы";
        const string auCustom = "Выборочно";

        public AngleUnitTypeConverter(Type type)
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
                case auDegrees: return AngleUnit.Degrees;
                case auRadians: return AngleUnit.Radians;
                case auCustom: return AngleUnit.Custom;
            }
            return AngleUnit.Degrees;
        }

        public static string ToString(object value)
        {
            switch ((AngleUnit)value)
            {
                case AngleUnit.Degrees: return auDegrees;
                case AngleUnit.Radians: return auRadians;
                case AngleUnit.Custom: return auCustom;
            }
            return string.Empty;
        }
    }
}