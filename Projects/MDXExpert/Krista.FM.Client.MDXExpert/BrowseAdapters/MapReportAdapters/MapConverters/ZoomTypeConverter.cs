using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class ZoomTypeConverter : EnumConverter
    {
        const string zExponential = "Экспоненциальный";
        const string zLinear = "Линейный";
        const string zQuadratic = "Квадратический";

        public ZoomTypeConverter(Type type)
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
                case zExponential: return ZoomType.Exponential;
                case zLinear: return ZoomType.Linear;
                case zQuadratic: return ZoomType.Quadratic;
            }
            return ZoomType.Linear;
        }

        public static string ToString(object value)
        {
            switch ((ZoomType)value)
            {
                case ZoomType.Exponential: return zExponential;
                case ZoomType.Linear: return zLinear;
                case ZoomType.Quadratic: return zQuadratic;
            }
            return string.Empty;
        }
    }
}