using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class GradientColoringStyleConverter : EnumConverter
    {
        const string csDarken = "Темная";
        const string csLighten = "Светлая";

        public GradientColoringStyleConverter(Type type)
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
                case csDarken: return GradientColoringStyle.Darken;
                case csLighten: return GradientColoringStyle.Lighten;
            }
            return GradientColoringStyle.Darken;
        }

        public static string ToString(object value)
        {
            switch ((GradientColoringStyle)value)
            {
                case GradientColoringStyle.Darken: return csDarken;
                case GradientColoringStyle.Lighten: return csLighten;
            }
            return string.Empty;
        }
    }
}