using System;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Client.MDXExpert
{
    class RadialLabelOrientationConverter : EnumConverter
    {
        const string loAngular = "Угловая";
        const string loHorizontal = "Горизонтальная";
        const string loOutward = "Наружная";

        public RadialLabelOrientationConverter(Type type)
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
                case loAngular: return RadialLabelOrientation.Angular;
                case loHorizontal: return RadialLabelOrientation.Horizontal;
                case loOutward: return RadialLabelOrientation.Outward;
            }
            return RadialLabelOrientation.Horizontal;
        }

        public static string ToString(object value)
        {
            switch ((RadialLabelOrientation)value)
            {
                case RadialLabelOrientation.Angular: return loAngular;
                case RadialLabelOrientation.Horizontal: return loHorizontal;
                case RadialLabelOrientation.Outward: return loOutward;
            }
            return string.Empty;
        }
    }
}