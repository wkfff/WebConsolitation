using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class RulerGenreTypeConverter : EnumConverter
    {
        const string rgContinuous = "Непрерывная";
        const string rgDiscrete = "Дискретная";

        public RulerGenreTypeConverter(Type type)
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
                case rgContinuous: return RulerGenre.Continuous;
                case rgDiscrete: return RulerGenre.Discrete;
            }
            return RulerGenre.Continuous;
        }

        public static string ToString(object value)
        {
            switch ((RulerGenre)value)
            {
                case RulerGenre.Continuous: return rgContinuous;
                case RulerGenre.Discrete: return rgDiscrete;
            }
            return string.Empty;
        }
    }
}