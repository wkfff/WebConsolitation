using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class LegendSeparatorTypeConverter : EnumConverter
    {
        const string stDashLine = "Пунктирная линия";
        const string stDotLine = "Точечная линия";
        const string stDoubleLine = "Двойная линия";
        const string stGradientLine = "Градиентная линия";
        const string stLine = "Линия";
        const string stNone = "Нет";
        const string stThickGradientLine = "Широкая градиентная линия";
        const string stThickLine = "Широкая линия";

        public LegendSeparatorTypeConverter(Type type)
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
                case stDashLine: return LegendSeparatorType.DashLine;
                case stDotLine: return LegendSeparatorType.DotLine;
                case stDoubleLine: return LegendSeparatorType.DoubleLine;
                case stGradientLine: return LegendSeparatorType.GradientLine;
                case stLine: return LegendSeparatorType.Line;
                case stNone: return LegendSeparatorType.None;
                case stThickGradientLine: return LegendSeparatorType.ThickGradientLine;
                case stThickLine: return LegendSeparatorType.ThickLine;
            }
            return LegendSeparatorType.None;
        }

        public static string ToString(object value)
        {
            switch ((LegendSeparatorType)value)
            {
                case LegendSeparatorType.DashLine: return stDashLine;
                case LegendSeparatorType.DotLine: return stDotLine;
                case LegendSeparatorType.DoubleLine: return stDoubleLine;
                case LegendSeparatorType.GradientLine: return stGradientLine;
                case LegendSeparatorType.Line: return stLine;
                case LegendSeparatorType.None: return stNone;
                case LegendSeparatorType.ThickGradientLine: return stThickGradientLine;
                case LegendSeparatorType.ThickLine: return stThickLine;
            }
            return string.Empty;
        }
    }
}