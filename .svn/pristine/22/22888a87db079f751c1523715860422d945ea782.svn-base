using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class EffectApplicationModeConverter : EnumConverter
    {
        const string amBackground = "װמם";
        const string amChartImage = "װמם ט הטאדנאללא";
        const string amSkin = "ִטאדנאללא";

        public EffectApplicationModeConverter(Type type)
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
                case amBackground: return EffectApplicationMode.Background;
                case amChartImage: return EffectApplicationMode.ChartImage;
                case amSkin: return EffectApplicationMode.Skin;
            }
            return EffectApplicationMode.Background;
        }

        public static string ToString(object value)
        {
            switch ((EffectApplicationMode)value)
            {
                case EffectApplicationMode.Background: return amBackground;
                case EffectApplicationMode.ChartImage: return amChartImage;
                case EffectApplicationMode.Skin: return amSkin;
            }
            return string.Empty;
        }
    }
}