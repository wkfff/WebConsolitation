using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    class ColoringModeConverter : EnumConverter
    {
        const string cmColorRange = "Цветовой диапазон";
        const string cmDistinctColors = "Различные цвета";

        public ColoringModeConverter(Type type)
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
                case cmColorRange: return ColoringMode.ColorRange;
                case cmDistinctColors: return ColoringMode.DistinctColors;
            }
            return ColoringMode.ColorRange;
        }

        public static string ToString(object value)
        {
            switch ((ColoringMode)value)
            {
                case ColoringMode.ColorRange: return cmColorRange;
                case ColoringMode.DistinctColors: return cmDistinctColors;
            }
            return string.Empty;
        }
    }
}