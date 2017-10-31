using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Krista.FM.Client.MDXExpert
{
    class SmoothingModeTypeConverter : EnumConverter
    {
        const string smInvalid = "Неполное отображение";
        const string smDefault = "По умолчанию";
        const string smHighSpeed = "Высокая скорость";
        const string smHighQuality = "Высокое качество";
        const string smNone = "Нет";
        const string smAntiAlias = "Антиалиасинг";

        public SmoothingModeTypeConverter(Type type)
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
                case smInvalid: return SmoothingMode.Invalid;
                case smDefault: return SmoothingMode.Default;
                case smHighSpeed: return SmoothingMode.HighSpeed;
                case smHighQuality: return SmoothingMode.HighQuality;
                case smNone: return SmoothingMode.None;
                case smAntiAlias: return SmoothingMode.AntiAlias;
            }
            return SmoothingMode.AntiAlias;
        }

        public static string ToString(object value)
        {
            switch ((SmoothingMode)value)
            {
                case SmoothingMode.Invalid: return smInvalid;
                case SmoothingMode.Default: return smDefault;
                case SmoothingMode.HighSpeed: return smHighSpeed;
                case SmoothingMode.HighQuality: return smHighQuality;
                case SmoothingMode.None: return smNone;
                case SmoothingMode.AntiAlias: return smAntiAlias;
            }
            return string.Empty;
        }
    }
}