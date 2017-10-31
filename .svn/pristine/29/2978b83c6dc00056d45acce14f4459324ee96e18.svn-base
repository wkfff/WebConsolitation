using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class LocationOffsetModeConverter : EnumConverter
    {
        const string omAutomatic = "Автоматический";
        const string omManual = "Ручной";

        public LocationOffsetModeConverter(Type type)
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
                case omAutomatic: return LocationOffsetMode.Automatic;
                case omManual: return LocationOffsetMode.Manual;
            }
            return LocationOffsetMode.Automatic;
        }

        public static string ToString(object value)
        {
            switch ((LocationOffsetMode)value)
            {
                case LocationOffsetMode.Automatic: return omAutomatic;
                case LocationOffsetMode.Manual: return omManual;
            }
            return string.Empty;
        }
    }
}