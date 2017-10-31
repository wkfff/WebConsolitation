using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class OffsetConverter : TypeConverter
    {
        public OffsetConverter()
            : base()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return (destType == typeof(MapPoint));
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destType)
        {
            return ((MapPoint)value).X.ToString() + "; " +((MapPoint) value).Y.ToString();
        }


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }
        
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string[] points = ((string) value).Split(';');
            double x = Double.Parse(points[0]);
            double y = Double.Parse(points[1]);
            MapPoint mapPoint = new MapPoint(x, y);
            return mapPoint;
        }

        
        public static string ToString(object value)
        {
            return value.ToString();
        }
    }
}