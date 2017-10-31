using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class TextureApplicationStyleTypeConverter : EnumConverter
    {
        const string tsNormal = "Нормальная";
        const string tsInverted = "Обратная";

        public TextureApplicationStyleTypeConverter(Type type)
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
                case tsNormal: return TextureApplicationStyle.Normal;
                case tsInverted: return TextureApplicationStyle.Inverted;
            }
            return TextureApplicationStyle.Normal;
        }

        public static string ToString(object value)
        {
            switch ((TextureApplicationStyle)value)
            {
                case TextureApplicationStyle.Normal: return tsNormal;
                case TextureApplicationStyle.Inverted: return tsInverted;
            }
            return string.Empty;
        }
    }
}