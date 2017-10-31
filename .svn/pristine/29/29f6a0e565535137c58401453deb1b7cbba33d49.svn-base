using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class SymbolIconTypeConverter : EnumConverter
    {
        const string siCharacter = "Символ";
        const string siRandom = "Случайный";
        const string siPlus = "Плюс";
        const string siX = "Х";
        const string siTriangle = "Треугольник";
        const string siSquare = "Квадрат";
        const string siCircle = "Круг";
        const string siDiamond = "Ромб";
        const string siUpsideDownTriangle = "Перевернутый треугольник";
        const string siNone = "Нет";

        public SymbolIconTypeConverter(Type type)
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
                case siCharacter: return SymbolIcon.Character;
                case siRandom: return SymbolIcon.Random;
                case siPlus: return SymbolIcon.Plus;
                case siX: return SymbolIcon.X;
                case siTriangle: return SymbolIcon.Triangle;
                case siSquare: return SymbolIcon.Square;
                case siCircle: return SymbolIcon.Circle;
                case siDiamond: return SymbolIcon.Diamond;
                case siUpsideDownTriangle: return SymbolIcon.UpsideDownTriangle;
                case siNone: return SymbolIcon.None;
            }
            return SymbolIcon.Random;
        }

        public static string ToString(object value)
        {
            switch ((SymbolIcon)value)
            {
                case SymbolIcon.Character: return siCharacter;
                case SymbolIcon.Random: return siRandom;
                case SymbolIcon.Plus: return siPlus;
                case SymbolIcon.X: return siX;
                case SymbolIcon.Triangle: return siTriangle;
                case SymbolIcon.Square: return siSquare;
                case SymbolIcon.Circle: return siCircle;
                case SymbolIcon.Diamond: return siDiamond;
                case SymbolIcon.UpsideDownTriangle: return siUpsideDownTriangle;
                case SymbolIcon.None: return siNone;
            }
            return string.Empty;
        }
    }
}