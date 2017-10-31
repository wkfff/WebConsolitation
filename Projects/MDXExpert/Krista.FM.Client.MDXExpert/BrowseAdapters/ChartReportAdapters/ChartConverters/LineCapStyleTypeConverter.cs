using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class LineCapStyleTypeConverter : EnumConverter
    {
        const string lcNone = "Нет";
        const string lcArrow = "Стрелка";
        const string lcDiamond = "Ромб";
        const string lcFlat = "Плоский";
        const string lcRound = "Округлый";
        const string lcRoundAnchor = "Круг";
        const string lcSquare = "Квадратный";
        const string lcSquareAnchor = "Квадрат";
        const string lcTriangle = "Треугольник";

        public LineCapStyleTypeConverter(Type type)
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
                case lcNone: return LineCapStyle.NoAnchor;
                case lcArrow: return LineCapStyle.ArrowAnchor;
                case lcDiamond: return LineCapStyle.DiamondAnchor;
                case lcFlat: return LineCapStyle.Flat;
                case lcRound: return LineCapStyle.Round;
                case lcRoundAnchor: return LineCapStyle.RoundAnchor;
                case lcSquare: return LineCapStyle.Square;
                case lcSquareAnchor: return LineCapStyle.SquareAnchor;
                case lcTriangle: return LineCapStyle.Triangle;
            }
            return LineCapStyle.ArrowAnchor;
        }

        public static string ToString(object value)
        {
            switch ((LineCapStyle)value)
            {
                case LineCapStyle.NoAnchor: return lcNone;
                case LineCapStyle.ArrowAnchor: return lcArrow;
                case LineCapStyle.DiamondAnchor: return lcDiamond;
                case LineCapStyle.Flat: return lcFlat;
                case LineCapStyle.Round: return lcRound;
                case LineCapStyle.RoundAnchor: return lcRoundAnchor;
                case LineCapStyle.Square: return lcSquare;
                case LineCapStyle.SquareAnchor: return lcSquareAnchor;
                case LineCapStyle.Triangle: return lcTriangle;
            }
            return string.Empty;
        }
    }
}