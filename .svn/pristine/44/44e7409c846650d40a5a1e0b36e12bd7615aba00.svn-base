using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class TextAlignmentConverter : EnumConverter
    {
        const string taBottom = "Снизу";
        const string taCenter = "По центру";
        const string taLeft = "Слева";
        const string taRight = "Справа";
        const string taTop = "Сверху";

        public TextAlignmentConverter(Type type)
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
                case taBottom: return TextAlignment.Bottom;
                case taCenter: return TextAlignment.Center;
                case taLeft: return TextAlignment.Left;
                case taRight: return TextAlignment.Right;
                case taTop: return TextAlignment.Top;
            }
            return TextAlignment.Bottom;
        }

        public static string ToString(object value)
        {
            switch ((TextAlignment)value)
            {
                case TextAlignment.Bottom: return taBottom;
                case TextAlignment.Center: return taCenter;
                case TextAlignment.Left: return taLeft;
                case TextAlignment.Right: return taRight;
                case TextAlignment.Top: return taTop;
            }
            return string.Empty;
        }
    }
}