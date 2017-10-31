using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Krista.FM.Client.MDXExpert
{
    class WrapModeTypeConverter : EnumConverter
    {
        const string wmTile = "Мозаика";
        const string wmTileFlipX = "Горизонтальное";
        const string wmTileFlipY = "Вертикальное";
        const string wmTileFlipXY = "Горизонтальное и вертикальное";
        const string wmClamp = "Фиксированное";

        public WrapModeTypeConverter(Type type)
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
                case wmTile: return WrapMode.Tile;
                case wmTileFlipX: return WrapMode.TileFlipX;
                case wmTileFlipY: return WrapMode.TileFlipY;
                case wmTileFlipXY: return WrapMode.TileFlipXY;
                case wmClamp: return WrapMode.Clamp;
            }
            return WrapMode.Tile;
        }

        public static string ToString(object value)
        {
            switch ((WrapMode)value)
            {
                case WrapMode.Tile: return wmTile;
                case WrapMode.TileFlipX: return wmTileFlipX;
                case WrapMode.TileFlipY: return wmTileFlipY;
                case WrapMode.TileFlipXY: return wmTileFlipXY;
                case WrapMode.Clamp: return wmClamp;
            }
            return string.Empty;
        }
    }
}