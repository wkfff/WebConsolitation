using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class AxisRangeTypeConverter : EnumConverter
    {
        const string rtAutomatic = "Авто";
        const string rtCustom = "Пользовательский";

        public AxisRangeTypeConverter(Type type)
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
                case rtAutomatic: return AxisRangeType.Automatic;
                case rtCustom: return AxisRangeType.Custom;
            }
            return AxisRangeType.Automatic;
        }

        public static string ToString(object value)
        {
            switch ((AxisRangeType)value)
            {
                case AxisRangeType.Automatic: return rtAutomatic;
                case AxisRangeType.Custom: return rtCustom;
            }
            return string.Empty;
        }
    }
}