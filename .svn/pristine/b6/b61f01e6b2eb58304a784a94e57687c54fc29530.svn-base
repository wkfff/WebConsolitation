using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class MapDashStyleConverter : EnumConverter
    {
        const string dsDash = "Штрих";
        const string dsDashDot = "Штрих-пунктир";
        const string dsDashDotDot = "Штрих-двойной пунктир";
        const string dsDot = "Точки";
        const string dsNone = "Нет";
        const string dsSolid = "Сплошная линия";
        

        public MapDashStyleConverter(Type type)
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
                case dsDash: return MapDashStyle.Dash;
                case dsDashDot: return MapDashStyle.DashDot;
                case dsDashDotDot: return MapDashStyle.DashDotDot;
                case dsDot: return MapDashStyle.Dot;
                case dsNone: return MapDashStyle.None;
                case dsSolid: return MapDashStyle.Solid;
            }
            return MapDashStyle.Solid;
        }

        public static string ToString(object value)
        {
            switch ((MapDashStyle)value)
            {
                case MapDashStyle.Dash: return dsDash;
                case MapDashStyle.DashDot: return dsDashDot;
                case MapDashStyle.DashDotDot: return dsDashDotDot;
                case MapDashStyle.Dot: return dsDot;
                case MapDashStyle.None: return dsNone;
                case MapDashStyle.Solid: return dsSolid;
            }
            return string.Empty;
        }
    }
}