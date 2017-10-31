using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class LegendEmptyDisplayTypeConverter : EnumConverter
    {
        const string dtPaintElement = "Элемент отображения";
        const string dtPoint = "Точка";

        public LegendEmptyDisplayTypeConverter(Type type)
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
                case dtPaintElement: return LegendEmptyDisplayType.PE;
                case dtPoint: return LegendEmptyDisplayType.Point;
            }
            return LegendEmptyDisplayType.PE;
        }

        public static string ToString(object value)
        {
            switch ((LegendEmptyDisplayType)value)
            {
                case LegendEmptyDisplayType.PE: return dtPaintElement;
                case LegendEmptyDisplayType.Point: return dtPoint;
            }
            return string.Empty;
        }
    }
}