using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class FunnelLabelLocationTypeConverter : EnumConverter
    {
        const string flLeft = "По левому краю";
        const string flRight = "По правому краю";

        public FunnelLabelLocationTypeConverter(Type type)
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
                case flLeft: return FunnelLabelLocation.Left;
                case flRight: return FunnelLabelLocation.Right;
            }
            return FunnelLabelLocation.Left;
        }

        public static string ToString(object value)
        {
            switch ((FunnelLabelLocation)value)
            {
                case FunnelLabelLocation.Left: return flLeft;
                case FunnelLabelLocation.Right: return flRight;
            }
            return string.Empty;
        }
    }
}