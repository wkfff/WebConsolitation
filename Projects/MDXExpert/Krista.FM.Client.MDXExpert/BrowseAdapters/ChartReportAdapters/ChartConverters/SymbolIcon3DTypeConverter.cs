using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class SymbolIcon3DTypeConverter : EnumConverter
    {
        const string si3DCube = "Куб";
        const string si3DDiamond = "Октаэдр";
        const string si3DPiramid = "Пирамида";
        const string si3DPiramidDown = "Перевернутая пирамида";
        const string si3DSphere = "Сфера";

        public SymbolIcon3DTypeConverter(Type type)
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
                case si3DCube: return SymbolIcon3D.Cube;
                case si3DDiamond: return SymbolIcon3D.Diamond;
                case si3DPiramid: return SymbolIcon3D.Pyramid;
                case si3DPiramidDown: return SymbolIcon3D.PyramidDown;
                case si3DSphere: return SymbolIcon3D.Sphere;
            }
            return SymbolIcon3D.Sphere;
        }

        public static string ToString(object value)
        {
            switch ((SymbolIcon3D)value)
            {
                case SymbolIcon3D.Cube: return si3DCube;
                case SymbolIcon3D.Diamond: return si3DDiamond;
                case SymbolIcon3D.Pyramid: return si3DPiramid;
                case SymbolIcon3D.PyramidDown: return si3DPiramidDown;
                case SymbolIcon3D.Sphere: return si3DSphere;
            }
            return string.Empty;
        }
    }
}