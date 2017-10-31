using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class AnnotationLocationTypeConverter : EnumConverter
    {
        const string alPercentage = "Проценты";
        const string alPixels = "Пиксели";
        const string alRowColumn = "Ряды и категории";
        const string alDataValues = "Значения данных";

        public AnnotationLocationTypeConverter(Type type)
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
                case alPercentage: return LocationType.Percentage;
                case alPixels: return LocationType.Pixels;
                case alRowColumn: return LocationType.RowColumn;
                case alDataValues: return LocationType.DataValues;
            }
            return LocationType.Pixels;
        }

        public static string ToString(object value)
        {
            switch ((LocationType)value)
            {
                case LocationType.Percentage: return alPercentage;
                case LocationType.Pixels: return alPixels;
                case LocationType.RowColumn: return alRowColumn;
                case LocationType.DataValues: return alDataValues;
            }
            return string.Empty;
        }
    }
}