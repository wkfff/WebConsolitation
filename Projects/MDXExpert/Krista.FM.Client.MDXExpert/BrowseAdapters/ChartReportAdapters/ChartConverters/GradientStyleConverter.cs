using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class GradientStyleConverter : EnumConverter
    {
        const string gsBackwardDiagonal = "Обратнодиагональный";
        const string gsCircular = "Круговой";
        const string gsDefault = "По умолчанию";
        const string gsElliptical = "Эллиптический";
        const string gsForwardDiagonal = "Диагональный";
        const string gsHorizontal = "Горизонтальный";
        const string gsHorizontalBump = "Выпуклый горизонтальный";
        const string gsNone = "Нет";
        const string gsRaise = "Возрастающий";
        const string gsRectangular = "Прямоугольный";
        const string gsVertical = "Вертикальный";
        const string gsVerticalBump = "Выпуклый вертикальный";

        public GradientStyleConverter(Type type)
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
                case gsBackwardDiagonal: return GradientStyle.BackwardDiagonal;
                case gsCircular: return GradientStyle.Circular;
                case gsDefault: return GradientStyle.Default;
                case gsElliptical: return GradientStyle.Elliptical;
                case gsForwardDiagonal: return GradientStyle.ForwardDiagonal;
                case gsHorizontal: return GradientStyle.Horizontal;
                case gsHorizontalBump: return GradientStyle.HorizontalBump;
                case gsNone: return GradientStyle.None;
                case gsRaise: return GradientStyle.Raise;
                case gsRectangular: return GradientStyle.Rectangular;
                case gsVertical: return GradientStyle.Vertical;
                case gsVerticalBump: return GradientStyle.VerticalBump;
            }
            return GradientStyle.Default;
        }

        public static string ToString(object value)
        {
            switch ((GradientStyle)value)
            {
                case GradientStyle.BackwardDiagonal: return gsBackwardDiagonal;
                case GradientStyle.Circular: return gsCircular;
                case GradientStyle.Default: return gsDefault;
                case GradientStyle.Elliptical: return gsElliptical;
                case GradientStyle.ForwardDiagonal: return gsForwardDiagonal;
                case GradientStyle.Horizontal: return gsHorizontal;
                case GradientStyle.HorizontalBump: return gsHorizontalBump;
                case GradientStyle.None: return gsNone;
                case GradientStyle.Raise: return gsRaise;
                case GradientStyle.Rectangular: return gsRectangular;
                case GradientStyle.Vertical: return gsVertical;
                case GradientStyle.VerticalBump: return gsVerticalBump;
            }
            return string.Empty;
        }
    }
}