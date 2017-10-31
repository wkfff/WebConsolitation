using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class SymbolIconSizeTypeConverter : EnumConverter
    {
        const string isAuto = "Авто";
        const string isSmall = "Маленький";
        const string isMedium = "Средний";
        const string isLarge = "Большой";

        public SymbolIconSizeTypeConverter(Type type)
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
                case isAuto: return SymbolIconSize.Auto;
                case isSmall: return SymbolIconSize.Small;
                case isMedium: return SymbolIconSize.Medium;
                case isLarge: return SymbolIconSize.Large;
            }
            return SymbolIconSize.Auto;
        }

        public static string ToString(object value)
        {
            switch ((SymbolIconSize)value)
            {
                case SymbolIconSize.Auto: return isAuto;
                case SymbolIconSize.Small: return isSmall;
                case SymbolIconSize.Medium: return isMedium;
                case SymbolIconSize.Large: return isLarge;
            }
            return string.Empty;
        }
    }
}