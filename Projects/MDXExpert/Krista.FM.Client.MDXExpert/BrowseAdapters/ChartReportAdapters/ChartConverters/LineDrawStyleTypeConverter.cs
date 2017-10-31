using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class LineDrawStyleTypeConverter : EnumConverter
    {
        const string lsSolid = "Сплошная";
        const string lsDash = "Штриховая";
        const string lsDashDot = "Штрихпунктирная";
        const string lsDashDotDot = "Штрихдвухпунктирная";
        const string lsDot = "Пунктирная";

        public LineDrawStyleTypeConverter(Type type)
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
                case lsSolid: return LineDrawStyle.Solid;
                case lsDash: return LineDrawStyle.Dash;
                case lsDashDot: return LineDrawStyle.DashDot;
                case lsDashDotDot: return LineDrawStyle.DashDotDot;
                case lsDot: return LineDrawStyle.Dot;
            }
            return LineDrawStyle.Solid;
        }

        public static string ToString(object value)
        {
            switch ((LineDrawStyle)value)
            {
                case LineDrawStyle.Solid: return lsSolid;
                case LineDrawStyle.Dash: return lsDash;
                case LineDrawStyle.DashDot: return lsDashDot;
                case LineDrawStyle.DashDotDot: return lsDashDotDot;
                case LineDrawStyle.Dot: return lsDot;
            }
            return string.Empty;
        }
    }
}