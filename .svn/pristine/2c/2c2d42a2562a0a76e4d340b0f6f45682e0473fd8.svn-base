using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public enum CustomPaintElementType
    {
        [Description("Нет")]
        cpNone,
        [Description("Сплошная заливка")]
        cpSolidFill,
        [Description("Штриховка")]
        cpHatch,
        [Description("Изображение")]
        cpImage,
        [Description("Градиент")]
        cpGradient,
        [Description("Текстура")]
        cpTexture
    }

    public class PaintElementTypeConverter : EnumConverter
    {
        const string teNone = "Нет";
        const string teSolidFill = "Сплошная заливка";
        const string teHatch = "Штриховка";
        const string teImage = "Изображение";
        const string teGradient = "Градиент";
        const string teTexture = "Текстура";
        const string teCustomBrush = "Пользовательская заливка";

        public PaintElementTypeConverter(Type type)
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
                case teNone: return PaintElementType.None;
                case teSolidFill: return PaintElementType.SolidFill;
                case teHatch: return PaintElementType.Hatch;
                case teImage: return PaintElementType.Image;
                case teGradient: return PaintElementType.Gradient;
                case teCustomBrush: return PaintElementType.CustomBrush;
                case teTexture: return PaintElementType.Texture;
            }
            return PaintElementType.SolidFill;
        }

        public static PaintElementType ConvertFromCustom(CustomPaintElementType cPE)
        {
            switch (cPE)
            {
                case CustomPaintElementType.cpNone: return PaintElementType.None;
                case CustomPaintElementType.cpSolidFill: return PaintElementType.SolidFill;
                case CustomPaintElementType.cpHatch: return PaintElementType.Hatch;
                case CustomPaintElementType.cpImage: return PaintElementType.Image;
                case CustomPaintElementType.cpGradient: return PaintElementType.Gradient;
                case CustomPaintElementType.cpTexture: return PaintElementType.Texture;
            }
            return PaintElementType.SolidFill;
        }

        public static CustomPaintElementType ConvertToCustom(PaintElementType PE)
        {
            switch (PE)
            {
                case PaintElementType.None: return CustomPaintElementType.cpNone;
                case PaintElementType.SolidFill: return CustomPaintElementType.cpSolidFill;
                case PaintElementType.Hatch: return CustomPaintElementType.cpHatch;
                case PaintElementType.Image: return CustomPaintElementType.cpImage;
                case PaintElementType.Gradient: return CustomPaintElementType.cpGradient;
                case PaintElementType.Texture: return CustomPaintElementType.cpTexture;
            }
            return CustomPaintElementType.cpSolidFill;
        }

        public static string CustomToString(object value)
        {
            switch ((CustomPaintElementType)value)
            {
                case CustomPaintElementType.cpNone: return teNone;
                case CustomPaintElementType.cpSolidFill: return teSolidFill;
                case CustomPaintElementType.cpHatch: return teHatch;
                case CustomPaintElementType.cpImage: return teImage;
                case CustomPaintElementType.cpGradient: return teGradient;
                case CustomPaintElementType.cpTexture: return teTexture;
            }
            return string.Empty;
        }

        public static string ToString(object value)
        {
            switch ((PaintElementType)value)
            {
                case PaintElementType.None: return teNone;
                case PaintElementType.SolidFill: return teSolidFill;
                case PaintElementType.Hatch: return teHatch;
                case PaintElementType.Image: return teImage;
                case PaintElementType.Gradient: return teGradient;
                case PaintElementType.Texture: return teTexture;
                case PaintElementType.CustomBrush: return teCustomBrush;
            }
            return string.Empty;
        }
    }
}