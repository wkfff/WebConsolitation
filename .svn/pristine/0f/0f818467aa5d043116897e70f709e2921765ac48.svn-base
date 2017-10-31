using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class LocationTypeConverter : EnumConverter
    {
        const string lcTop = "Сверху";
        const string lcLeft = "Слева";
        const string lsRight = "Справа";
        const string lsBottom = "Снизу";

        public LocationTypeConverter(Type type)
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
                case lcTop: return LegendLocation.Top;
                case lcLeft: return LegendLocation.Left;
                case lsRight: return LegendLocation.Right;
                case lsBottom: return LegendLocation.Bottom;
            }
            return LegendLocation.Right;
        }

        public static string ToString(object value)
        {
            switch ((LegendLocation)value)
            {
                case LegendLocation.Top: return lcTop;
                case LegendLocation.Left: return lcLeft;
                case LegendLocation.Right: return lsRight;
                case LegendLocation.Bottom: return lsBottom;
            }
            return string.Empty;
        }
    }
}