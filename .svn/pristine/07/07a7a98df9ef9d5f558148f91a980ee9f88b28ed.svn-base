using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class LegendItemTypeConverter : EnumConverter
    {
        const string iAuto = "Автоматический";
        const string iPoint = "Точки";
        const string iSeries = "Ряды";

        public LegendItemTypeConverter(Type type)
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
                case iAuto: return LegendItemType.Auto;
                case iPoint: return LegendItemType.Point;
                case iSeries: return LegendItemType.Series;
            }
            return LegendItemType.Auto;
        }

        public static string ToString(object value)
        {
            switch ((LegendItemType)value)
            {
                case LegendItemType.Auto: return iAuto;
                case LegendItemType.Point: return iPoint;
                case LegendItemType.Series: return iSeries;
            }
            return string.Empty;
        }
    }
}