using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class TooltipDisplayTypeConverter : EnumConverter
    {
        const string tdNever = "Никогда";
        const string tdMouseMove = "По движению мыши";
        const string tdMouseClick = "По щелчку мыши";

        public TooltipDisplayTypeConverter(Type type)
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
                case tdNever: return TooltipDisplay.Never;
                case tdMouseMove: return TooltipDisplay.MouseMove;
                case tdMouseClick: return TooltipDisplay.MouseClick;
            }
            return TooltipDisplay.MouseMove;
        }

        public static string ToString(object value)
        {
            switch ((TooltipDisplay)value)
            {
                case TooltipDisplay.Never: return tdNever;
                case TooltipDisplay.MouseMove: return tdMouseMove;
                case TooltipDisplay.MouseClick: return tdMouseClick;
            }
            return string.Empty;
        }
    }
}