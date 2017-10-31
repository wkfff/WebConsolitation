using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    class ContentAlignmentConverter : EnumConverter
    {
        const string caBottomCenter = "Снизу по центру";
        const string caBottomLeft = "Снизу слева";
        const string caBottomRight = "Снизу справа";
        const string caMiddleCenter = "Посередине в центре";
        const string caMiddleLeft = "Посередине слева";
        const string caMiddleRight = "Посередине справа";
        const string caTopCenter = "Сверху по центру";
        const string caTopLeft = "Сверху слева";
        const string caTopRight = "Сверху справа";

        public ContentAlignmentConverter(Type type)
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
                case caBottomCenter: return ContentAlignment.BottomCenter;
                case caBottomLeft: return ContentAlignment.BottomLeft;
                case caBottomRight: return ContentAlignment.BottomRight;
                case caMiddleCenter: return ContentAlignment.MiddleCenter;
                case caMiddleLeft: return ContentAlignment.MiddleLeft;
                case caMiddleRight: return ContentAlignment.MiddleRight;
                case caTopCenter: return ContentAlignment.TopCenter;
                case caTopLeft: return ContentAlignment.TopLeft;
                case caTopRight: return ContentAlignment.TopRight;
            }
            return ContentAlignment.MiddleCenter;
        }

        public static string ToString(object value)
        {
            switch ((ContentAlignment)value)
            {
                case ContentAlignment.BottomCenter: return caBottomCenter;
                case ContentAlignment.BottomLeft: return caBottomLeft;
                case ContentAlignment.BottomRight: return caBottomRight;
                case ContentAlignment.MiddleCenter: return caMiddleCenter;
                case ContentAlignment.MiddleLeft: return caMiddleLeft;
                case ContentAlignment.MiddleRight: return caMiddleRight;
                case ContentAlignment.TopCenter: return caTopCenter;
                case ContentAlignment.TopLeft: return caTopLeft;
                case ContentAlignment.TopRight: return caTopRight;
            }
            return string.Empty;
        }
    }
}