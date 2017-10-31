using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class StackStyleTypeConverter : EnumConverter
    {
        const string ssNormal = "Обычный";
        const string ssComplete = "Полный";

        public StackStyleTypeConverter(Type type)
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
                case ssNormal: return StackStyle.Normal;
                case ssComplete: return StackStyle.Complete;
            }
            return StackStyle.Normal;
        }

        public static string ToString(object value)
        {
            switch ((StackStyle)value)
            {
                case StackStyle.Normal: return ssNormal;
                case StackStyle.Complete: return ssComplete;
            }
            return string.Empty;
        }
    }
}