using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class BubbleShapeTypeConverter : EnumConverter
    {
        const string bsTriangle = "Треугольник";
        const string bsCircle = "Круг";
        const string bsSquare = "Квадрат";
        const string bsInvertedTriangle = "Перевернутый треугольник";
        const string bsCustom = "Выборочный";

        public BubbleShapeTypeConverter(Type type)
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
                case bsTriangle: return BubbleShape.Triangle;
                case bsCircle: return BubbleShape.Circle;
                case bsSquare: return BubbleShape.Square;
                case bsInvertedTriangle: return BubbleShape.InvertedTriangle;
                case bsCustom: return BubbleShape.Custom;
            }
            return BubbleShape.Circle;
        }

        public static string ToString(object value)
        {
            switch ((BubbleShape)value)
            {
                case BubbleShape.Triangle: return bsTriangle;
                case BubbleShape.Circle: return bsCircle;
                case BubbleShape.Square: return bsSquare;
                case BubbleShape.InvertedTriangle: return bsInvertedTriangle;
                case BubbleShape.Custom: return bsCustom;
            }
            return string.Empty;
        }
    }
}