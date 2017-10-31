using System;
using System.ComponentModel;
using System.Drawing.Text;
using System.Globalization;

namespace Krista.FM.Client.MDXExpert
{
    class TextRenderingHintTypeConverter : EnumConverter
    {
        const string trSystemDefault = "По умолчанию";
        const string trSingleBitPerPixelGridFit = "Глиф с выравниванием";
        const string trSingleBitPerPixel = "Глиф";
        const string trAntiAliasGridFit = "Антиалиасинг с выравниванием";
        const string trAntiAlias = "Антиалиасинг"; 
        const string trClearTypeGridFit = "Высокое качество";
        
        public TextRenderingHintTypeConverter(Type type)
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
                case trSystemDefault: return TextRenderingHint.SystemDefault;
                case trSingleBitPerPixelGridFit: return TextRenderingHint.SingleBitPerPixelGridFit;
                case trSingleBitPerPixel: return TextRenderingHint.SingleBitPerPixel;
                case trAntiAliasGridFit: return TextRenderingHint.AntiAliasGridFit;
                case trAntiAlias: return TextRenderingHint.AntiAlias;
                case trClearTypeGridFit: return TextRenderingHint.ClearTypeGridFit;
            }
            return TextRenderingHint.AntiAlias;
        }

        public static string ToString(object value)
        {
            switch ((TextRenderingHint)value)
            {
                case TextRenderingHint.SystemDefault: return trSystemDefault;
                case TextRenderingHint.SingleBitPerPixelGridFit: return trSingleBitPerPixelGridFit;
                case TextRenderingHint.SingleBitPerPixel: return trSingleBitPerPixel;
                case TextRenderingHint.AntiAliasGridFit: return trAntiAliasGridFit;
                case TextRenderingHint.AntiAlias: return trAntiAlias;
                case TextRenderingHint.ClearTypeGridFit: return trClearTypeGridFit;
            }
            return string.Empty;
        }
    }
}