using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class MarginLocationTypeConvterter : EnumConverter
    {
        const string mlRowColumn = "Ряды/категории";
        const string mlDataValues = "Значения данных";
        const string mlPixels = "Пиксели";
        const string mlPercentage = "Проценты";


        public MarginLocationTypeConvterter(Type type)
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
                case mlRowColumn: return LocationType.RowColumn;
                case mlDataValues: return LocationType.DataValues;
                case mlPixels: return LocationType.Pixels;
                case mlPercentage: return LocationType.Percentage;
            }
            return LocationType.Percentage;
        }

        public static string ToString(object value)
        {
            switch ((LocationType)value)
            {
                case LocationType.RowColumn: return mlRowColumn;
                case LocationType.DataValues: return mlDataValues;
                case LocationType.Pixels: return mlPixels;
                case LocationType.Percentage: return mlPercentage;
            }
            return string.Empty;
        }
    }
}
