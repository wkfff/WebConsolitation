using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class MapHatchStyleConverter : EnumConverter
    {
        
        const string hBackwardDiagonal = "Диагональный 1";
        const string hCross = "Сетка";
        const string hDarkDownwardDiagonal = "Темный диагональный 1";
        const string hDarkHorizontal = "Темный горизонтальный";
        const string hDarkUpwardDiagonal = "Темный диагональный 2";
        const string hDarkVertical = "Темный вертикальный";
        const string hDashedDownwardDiagonal = "Штриховой диагональный 1";
        const string hDashedHorizontal = "Штриховой горизонтальный";
        const string hDashedUpwardDiagonal = "Штриховой диагональный 2";
        const string hDashedVertical = "Штриховой вертикальный";
        const string hDiagonalBrick = "Диагональный кирпич";
        const string hDiagonalCross = "Диагональная сетка";
        const string hDivot = "Уголки";
        const string hDottedDiamond = "Точечные ромбики";
        const string hDottedGrid = "Точечная сетка";
        const string hForwardDiagonal = "Диагональный 2";
        const string hHorizontal = "Горизонтальный";
        const string hHorizontalBrick = "Горизонтальный кирпич";
        const string hLargeCheckerBoard = "Крупная клетка";
        const string hLargeConfetti = "Крупное конфетти";
        const string hLargeGrid = "Крупная сетка";
        const string hLightDownwardDiagonal = "Светлый диагональный 1";
        const string hLightHorizontal = "Светлый горизонтальный";
        const string hLightUpwardDiagonal = "Светлый диагональный 2";
        const string hLightVertical = "Светлый вертикальный";
        const string hNarrowHorizontal = "Частый горизонтальный";
        const string hNarrowVertical = "Частый вертикальный";
        const string hNone = "Нет";
        const string hOutlinedDiamond = "Контурные ромбики";
        const string hPercent05 = "5%";
        const string hPercent10 = "10%";
        const string hPercent20 = "20%";
        const string hPercent25 = "25%";
        const string hPercent30 = "30%";
        const string hPercent40 = "40%";
        const string hPercent50 = "50%";
        const string hPercent60 = "60%";
        const string hPercent70 = "70%";
        const string hPercent75 = "75%";
        const string hPercent80 = "80%";
        const string hPercent90 = "90%";
        const string hPlaid = "Шотландка";
        const string hShingle = "Дранка";
        const string hSmallCheckerBoard = "Мелкая клетка";
        const string hSmallConfetti = "Мелкое конфетти";
        const string hSmallGrid = "Мелкая сетка";
        const string hSolidDiamond = "Ромбики";
        const string hSphere = "Сферы";
        const string hTrellis = "Решетка";
        const string hVertical = "Вертикальный";
        const string hWave = "Волны";
        const string hWeave = "Плетенка";
        const string hWideDownwardDiagonal = "Широкий дагональный 1";
        const string hWideUpwardDiagonal = "Широкий диагональный 2";
        const string hZigZag = "Зигзаг";

        public MapHatchStyleConverter(Type type)
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
                case hBackwardDiagonal: return MapHatchStyle.BackwardDiagonal;
                case hCross: return MapHatchStyle.Cross;
                case hDarkDownwardDiagonal: return MapHatchStyle.DarkDownwardDiagonal;
                case hDarkHorizontal: return MapHatchStyle.DarkHorizontal;
                case hDarkUpwardDiagonal: return MapHatchStyle.DarkUpwardDiagonal;
                case hDarkVertical: return MapHatchStyle.DarkVertical;
                case hDashedDownwardDiagonal: return MapHatchStyle.DashedDownwardDiagonal;
                case hDashedHorizontal: return MapHatchStyle.DashedHorizontal;
                case hDashedUpwardDiagonal: return MapHatchStyle.DashedUpwardDiagonal;
                case hDashedVertical: return MapHatchStyle.DashedVertical;
                case hDiagonalBrick: return MapHatchStyle.DiagonalBrick;
                case hDiagonalCross: return MapHatchStyle.DiagonalCross;
                case hDivot: return MapHatchStyle.Divot;
                case hDottedDiamond: return MapHatchStyle.DottedDiamond;
                case hDottedGrid: return MapHatchStyle.DottedGrid;
                case hForwardDiagonal: return MapHatchStyle.ForwardDiagonal;
                case hHorizontal: return MapHatchStyle.Horizontal;
                case hHorizontalBrick: return MapHatchStyle.HorizontalBrick;
                case hLargeCheckerBoard: return MapHatchStyle.LargeCheckerBoard;
                case hLargeConfetti: return MapHatchStyle.LargeConfetti;
                case hLargeGrid: return MapHatchStyle.LargeGrid;
                case hLightDownwardDiagonal: return MapHatchStyle.LightDownwardDiagonal;
                case hLightHorizontal: return MapHatchStyle.LightHorizontal;
                case hLightUpwardDiagonal: return MapHatchStyle.LightUpwardDiagonal;
                case hLightVertical: return MapHatchStyle.LightVertical;
                case hNarrowHorizontal: return MapHatchStyle.NarrowHorizontal;
                case hNarrowVertical: return MapHatchStyle.NarrowVertical;
                case hNone: return MapHatchStyle.None;
                case hOutlinedDiamond: return MapHatchStyle.OutlinedDiamond;
                case hPercent05: return MapHatchStyle.Percent05;
                case hPercent10: return MapHatchStyle.Percent10;
                case hPercent20: return MapHatchStyle.Percent20;
                case hPercent25: return MapHatchStyle.Percent25;
                case hPercent30: return MapHatchStyle.Percent30;
                case hPercent40: return MapHatchStyle.Percent40;
                case hPercent50: return MapHatchStyle.Percent50;
                case hPercent60: return MapHatchStyle.Percent60;
                case hPercent70: return MapHatchStyle.Percent70;
                case hPercent75: return MapHatchStyle.Percent75;
                case hPercent80: return MapHatchStyle.Percent80;
                case hPercent90: return MapHatchStyle.Percent90;
                case hPlaid: return MapHatchStyle.Plaid;
                case hShingle: return MapHatchStyle.Shingle;
                case hSmallCheckerBoard: return MapHatchStyle.SmallCheckerBoard;
                case hSmallConfetti: return MapHatchStyle.SmallConfetti;
                case hSmallGrid: return MapHatchStyle.SmallGrid;
                case hSolidDiamond: return MapHatchStyle.SolidDiamond;
                case hSphere: return MapHatchStyle.Sphere;
                case hTrellis: return MapHatchStyle.Trellis;
                case hVertical: return MapHatchStyle.Vertical;
                case hWave: return MapHatchStyle.Wave;
                case hWeave: return MapHatchStyle.Weave;
                case hWideDownwardDiagonal: return MapHatchStyle.WideDownwardDiagonal;
                case hWideUpwardDiagonal: return MapHatchStyle.WideUpwardDiagonal;
                case hZigZag: return MapHatchStyle.ZigZag;
            }
            return MapHatchStyle.None;
        }

        public static string ToString(object value)
        {
            switch ((MapHatchStyle)value)
            {
                case MapHatchStyle.BackwardDiagonal: return hBackwardDiagonal;
                case MapHatchStyle.Cross: return hCross;
                case MapHatchStyle.DarkDownwardDiagonal: return hDarkDownwardDiagonal;
                case MapHatchStyle.DarkHorizontal: return hDarkHorizontal;
                case MapHatchStyle.DarkUpwardDiagonal: return hDarkUpwardDiagonal;
                case MapHatchStyle.DarkVertical: return hDarkVertical;
                case MapHatchStyle.DashedDownwardDiagonal: return hDashedDownwardDiagonal;
                case MapHatchStyle.DashedHorizontal: return hDashedHorizontal;
                case MapHatchStyle.DashedUpwardDiagonal: return hDashedUpwardDiagonal;
                case MapHatchStyle.DashedVertical: return hDashedVertical;
                case MapHatchStyle.DiagonalBrick: return hDiagonalBrick;
                case MapHatchStyle.DiagonalCross: return hDiagonalCross;
                case MapHatchStyle.Divot: return hDivot;
                case MapHatchStyle.DottedDiamond: return hDottedDiamond;
                case MapHatchStyle.DottedGrid: return hDottedGrid;
                case MapHatchStyle.ForwardDiagonal: return hForwardDiagonal;
                case MapHatchStyle.Horizontal: return hHorizontal;
                case MapHatchStyle.HorizontalBrick: return hHorizontalBrick;
                case MapHatchStyle.LargeCheckerBoard: return hLargeCheckerBoard;
                case MapHatchStyle.LargeConfetti: return hLargeConfetti;
                case MapHatchStyle.LargeGrid: return hLargeGrid;
                case MapHatchStyle.LightDownwardDiagonal: return hLightDownwardDiagonal;
                case MapHatchStyle.LightHorizontal: return hLightHorizontal;
                case MapHatchStyle.LightUpwardDiagonal: return hLightUpwardDiagonal;
                case MapHatchStyle.LightVertical: return hLightVertical;
                case MapHatchStyle.NarrowHorizontal: return hNarrowHorizontal;
                case MapHatchStyle.NarrowVertical: return hNarrowVertical;
                case MapHatchStyle.None: return hNone;
                case MapHatchStyle.OutlinedDiamond: return hOutlinedDiamond;
                case MapHatchStyle.Percent05: return hPercent05;
                case MapHatchStyle.Percent10: return hPercent10;
                case MapHatchStyle.Percent20: return hPercent20;
                case MapHatchStyle.Percent25: return hPercent25;
                case MapHatchStyle.Percent30: return hPercent30;
                case MapHatchStyle.Percent40: return hPercent40;
                case MapHatchStyle.Percent50: return hPercent50;
                case MapHatchStyle.Percent60: return hPercent60;
                case MapHatchStyle.Percent70: return hPercent70;
                case MapHatchStyle.Percent75: return hPercent75;
                case MapHatchStyle.Percent80: return hPercent80;
                case MapHatchStyle.Percent90: return hPercent90;
                case MapHatchStyle.Plaid: return hPlaid;
                case MapHatchStyle.Shingle: return hShingle;
                case MapHatchStyle.SmallCheckerBoard: return hSmallCheckerBoard;
                case MapHatchStyle.SmallConfetti: return hSmallConfetti;
                case MapHatchStyle.SmallGrid: return hSmallGrid;
                case MapHatchStyle.SolidDiamond: return hSolidDiamond;
                case MapHatchStyle.Sphere: return hSphere;
                case MapHatchStyle.Trellis: return hTrellis;
                case MapHatchStyle.Vertical: return hVertical;
                case MapHatchStyle.Wave: return hWave;
                case MapHatchStyle.Weave: return hWeave;
                case MapHatchStyle.WideDownwardDiagonal: return hWideDownwardDiagonal;
                case MapHatchStyle.WideUpwardDiagonal: return hWideUpwardDiagonal;
                case MapHatchStyle.ZigZag: return hZigZag;
            }
            return string.Empty;
        }
    }
}