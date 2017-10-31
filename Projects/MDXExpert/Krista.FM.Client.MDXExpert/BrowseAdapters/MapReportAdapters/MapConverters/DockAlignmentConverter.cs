using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class DockAlignmentConverter : EnumConverter
    {
        const string daCenter = "По центру";
        const string daFar = "В дальнем углу";
        const string daNear = "В ближнем углу";

        public DockAlignmentConverter(Type type)
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
                case daCenter: return DockAlignment.Center;
                case daFar: return DockAlignment.Far;
                case daNear: return DockAlignment.Near;
            }
            return DockAlignment.Near;
        }

        public static string ToString(object value)
        {
            switch ((DockAlignment)value)
            {
                case DockAlignment.Center: return daCenter;
                case DockAlignment.Far: return daFar;
                case DockAlignment.Near: return daNear;
            }
            return string.Empty;
        }
    }
}