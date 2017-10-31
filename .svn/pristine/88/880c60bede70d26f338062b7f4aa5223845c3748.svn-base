using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class TextOrientationTypeConverter : EnumConverter
    {
        const string lcHorizontal = "Горизонтальная";
        const string lcVerticalLeft = "Вертикальная по левому краю";
        const string lsVerticalRight = "Вертикальная по правому краю";
        const string lsCustom = "Пользовательская";

        public TextOrientationTypeConverter(Type type)
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
                case lcHorizontal: return TextOrientation.Horizontal;
                case lcVerticalLeft: return TextOrientation.VerticalLeftFacing;
                case lsVerticalRight: return TextOrientation.VerticalRightFacing;
                case lsCustom: return TextOrientation.Custom;
            }
            return TextOrientation.Horizontal;
        }

        public static string ToString(object value)
        {
            switch ((TextOrientation)value)
            {
                case TextOrientation.Horizontal: return lcHorizontal;
                case TextOrientation.VerticalLeftFacing: return lcVerticalLeft;
                case TextOrientation.VerticalRightFacing: return lsVerticalRight;
                case TextOrientation.Custom: return lsCustom;
            }
            return string.Empty;
            
        }
    }
}