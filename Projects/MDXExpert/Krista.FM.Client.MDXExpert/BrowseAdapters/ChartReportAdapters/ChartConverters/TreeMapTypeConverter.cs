using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class TreeMapTypeConverter : EnumConverter
    {
        const string tmRectangular = "Прямоугольный";
        const string tmCircular = "Круговой";
        const string tmRings = "Кольцевой";

        public TreeMapTypeConverter(Type type)
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
                case tmRectangular: return TreeMapType.Rectangular;
                case tmCircular: return TreeMapType.Circular;
                case tmRings: return TreeMapType.Rings;
            }
            return TreeMapType.Rectangular;
        }

        public static string ToString(object value)
        {
            switch ((TreeMapType)value)
            {
                case TreeMapType.Rectangular: return tmRectangular;
                case TreeMapType.Circular: return tmCircular;
                case TreeMapType.Rings: return tmRings;
            }
            return string.Empty;
        }
    }
}