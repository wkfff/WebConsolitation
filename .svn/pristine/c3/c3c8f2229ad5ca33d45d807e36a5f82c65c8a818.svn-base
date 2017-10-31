using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class GradientTypeConverter : EnumConverter
    {
        const string gBottomTop = "Снизу вверх";
        const string gCenter = "Из центра";
        const string gDiagonalLeft = "По диагонали слева";
        const string gDiagonalRight = "По диагонали справа";
        const string gHorizontalCenter = "По горизонтали из центра";
        const string gLeftRight = "Слева направо";
        const string gNone = "Нет";
        const string gReversedCenter = "Обратный из центра";
        const string gReversedDiagonalLeft = "Обратный по диагонали слева";
        const string gReversedDiagonalRight = "Обратный по диагонали справа";
        const string gReversedHorizontalCenter = "Обратный по горизонтали из центра";
        const string gReversedVerticalCenter = "Обратный по вертикали из центра";
        const string gRightLeft = "Справа налево";
        const string gTopBottom = "Сверху вниз";
        const string gVerticalCenter = "По вертикали из центра";

        public GradientTypeConverter(Type type)
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
                case gBottomTop: return GradientType.BottomTop;
                case gCenter: return GradientType.Center;
                case gDiagonalLeft: return GradientType.DiagonalLeft;
                case gDiagonalRight: return GradientType.DiagonalRight;
                case gHorizontalCenter: return GradientType.HorizontalCenter;
                case gLeftRight: return GradientType.LeftRight;
                case gNone: return GradientType.None;
                case gReversedCenter: return GradientType.ReversedCenter;
                case gReversedDiagonalLeft: return GradientType.ReversedDiagonalLeft;
                case gReversedDiagonalRight: return GradientType.ReversedDiagonalRight;
                case gReversedHorizontalCenter: return GradientType.ReversedHorizontalCenter;
                case gReversedVerticalCenter: return GradientType.ReversedVerticalCenter;
                case gRightLeft: return GradientType.RightLeft;
                case gTopBottom: return GradientType.TopBottom;
                case gVerticalCenter: return GradientType.VerticalCenter;
            }
            return GradientType.None;
        }

        public static string ToString(object value)
        {
            switch ((GradientType)value)
            {
                case GradientType.BottomTop: return gBottomTop;
                case GradientType.Center: return gCenter;
                case GradientType.DiagonalLeft: return gDiagonalLeft;
                case GradientType.DiagonalRight: return gDiagonalRight;
                case GradientType.HorizontalCenter: return gHorizontalCenter;
                case GradientType.LeftRight: return gLeftRight;
                case GradientType.None: return gNone;
                case GradientType.ReversedCenter: return gReversedCenter;
                case GradientType.ReversedDiagonalLeft: return gReversedDiagonalLeft;
                case GradientType.ReversedDiagonalRight: return gReversedDiagonalRight;
                case GradientType.ReversedHorizontalCenter: return gReversedHorizontalCenter;
                case GradientType.ReversedVerticalCenter: return gReversedVerticalCenter;
                case GradientType.RightLeft: return gRightLeft;
                case GradientType.TopBottom: return gTopBottom;
                case GradientType.VerticalCenter: return gVerticalCenter;
            }
            return string.Empty;
        }
    }
}