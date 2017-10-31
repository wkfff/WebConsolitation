using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class MapOrientationTypeConverter : EnumConverter
    {
        const string moAuto = "Автоматическая";
        const string moHorizontal = "Горизонтальная";
        const string moVertical = "Вертикальная";

        public MapOrientationTypeConverter(Type type)
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
                case moAuto: return Orientation.Auto;
                case moHorizontal: return Orientation.Horizontal;
                case moVertical: return Orientation.Vertical;
            }
            return Orientation.Auto;
        }

        public static string ToString(object value)
        {
            switch ((Orientation)value)
            {
                case Orientation.Auto: return moAuto;
                case Orientation.Horizontal: return moHorizontal;
                case Orientation.Vertical: return moVertical;
            }
            return string.Empty;
        }
    }
}