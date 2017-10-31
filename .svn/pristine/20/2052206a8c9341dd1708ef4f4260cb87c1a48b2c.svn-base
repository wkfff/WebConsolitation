using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class MapColorPaletteConverter : EnumConverter
    {
        const string cpDundas = "Оригинальная";
        const string cpLight = "Светлая";
        const string cpRandom = "Произвольная";
        const string cpSemiTransparent = "Полупрозрачная";

        public MapColorPaletteConverter(Type type)
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
                case cpDundas: return MapColorPalette.Dundas;
                case cpLight: return MapColorPalette.Light;
                case cpRandom: return MapColorPalette.Random;
                case cpSemiTransparent: return MapColorPalette.SemiTransparent;
            }
            return MapColorPalette.Random;
        }

        public static string ToString(object value)
        {
            switch ((MapColorPalette)value)
            {
                case MapColorPalette.Dundas: return cpDundas;
                case MapColorPalette.Light: return cpLight;
                case MapColorPalette.Random: return cpRandom;
                case MapColorPalette.SemiTransparent: return cpSemiTransparent;
            }
            return string.Empty;
        }
    }
}