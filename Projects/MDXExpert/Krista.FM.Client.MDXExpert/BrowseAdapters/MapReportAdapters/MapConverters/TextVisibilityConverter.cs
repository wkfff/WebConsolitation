using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class TextVisibilityConverter : EnumConverter
    {
        const string tvAuto = "Автоматическая";
        const string tvHidden = "Скрывать";
        const string tvShown = "Показывать";

        public TextVisibilityConverter(Type type)
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
                case tvAuto: return TextVisibility.Auto;
                case tvHidden: return TextVisibility.Hidden;
                case tvShown: return TextVisibility.Shown;
            }
            return TextVisibility.Auto;
        }

        public static string ToString(object value)
        {
            switch ((TextVisibility)value)
            {
                case TextVisibility.Auto: return tvAuto;
                case TextVisibility.Hidden: return tvHidden;
                case TextVisibility.Shown: return tvShown;
            }
            return string.Empty;
        }
    }
}