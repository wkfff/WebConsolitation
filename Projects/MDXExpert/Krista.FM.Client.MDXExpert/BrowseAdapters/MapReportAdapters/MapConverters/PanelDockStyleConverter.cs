using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class PanelDockStyleConverter : EnumConverter
    {
        const string dsBottom = "Снизу";
        const string dsLeft = "Слева";
        const string dsNone = "Пользовательское";
        const string dsRight = "Справа";
        const string dsTop = "Сверху";

        public PanelDockStyleConverter(Type type)
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
                case dsBottom: return PanelDockStyle.Bottom;
                case dsLeft: return PanelDockStyle.Left;
                case dsNone: return PanelDockStyle.None;
                case dsRight: return PanelDockStyle.Right;
                case dsTop: return PanelDockStyle.Top;
            }
            return PanelDockStyle.Right;
        }

        public static string ToString(object value)
        {
            switch ((PanelDockStyle)value)
            {
                case PanelDockStyle.Bottom: return dsBottom;
                case PanelDockStyle.Left: return dsLeft;
                case PanelDockStyle.None: return dsNone;
                case PanelDockStyle.Right: return dsRight;
                case PanelDockStyle.Top: return dsTop;
            }
            return string.Empty;
        }
    }
}