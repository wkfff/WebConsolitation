using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    class Transform3DTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(
            ITypeDescriptorContext context, Type destType)
        {
            return true; //destType == typeof(string);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destType)
        {
            View3DAppearance view3D = (View3DAppearance)value;
            return view3D.Perspective.ToString() + "; " +
                   view3D.Scale.ToString() + "; " +
                   view3D.XRotation.ToString() + "; " +
                   view3D.YRotation.ToString();
        }
    }
}