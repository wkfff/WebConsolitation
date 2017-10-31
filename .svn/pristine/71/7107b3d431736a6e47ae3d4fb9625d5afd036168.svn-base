using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    class CoordinateUnitConverter : EnumConverter
    {
        const string cuPercent = "Проценты";
        const string cuPixel = "Пиксели";

        public CoordinateUnitConverter(Type type)
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
                case cuPercent: return CoordinateUnit.Percent;
                case cuPixel: return CoordinateUnit.Pixel;
            }
            return CoordinateUnit.Percent;
        }

        public static string ToString(object value)
        {
            switch ((CoordinateUnit)value)
            {
                case CoordinateUnit.Percent: return cuPercent;
                case CoordinateUnit.Pixel: return cuPixel;
            }
            return string.Empty;
        }
    }
}