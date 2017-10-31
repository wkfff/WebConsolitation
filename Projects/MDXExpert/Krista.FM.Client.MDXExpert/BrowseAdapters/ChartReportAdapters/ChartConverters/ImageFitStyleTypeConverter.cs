using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class ImageFitStyleTypeConverter : EnumConverter
    {
        const string fsCentered = "По центру";
        const string fsStretchedFit = "Растянуть";
        const string fsTiled = "Замостить";

        public ImageFitStyleTypeConverter(Type type)
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
                case fsCentered: return ImageFitStyle.Centered;
                case fsStretchedFit: return ImageFitStyle.StretchedFit;
                case fsTiled: return ImageFitStyle.Tiled;
            }
            return ImageFitStyle.StretchedFit;
        }

        public static string ToString(object value)
        {
            switch ((ImageFitStyle)value)
            {
                case ImageFitStyle.Centered: return fsCentered;
                case ImageFitStyle.StretchedFit: return fsStretchedFit;
                case ImageFitStyle.Tiled: return fsTiled;
            }
            return string.Empty;
        }
    }
}