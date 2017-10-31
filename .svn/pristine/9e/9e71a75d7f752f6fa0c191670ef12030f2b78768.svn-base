using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class ZoomPanelStyleConverter : EnumConverter
    {
        const string zpCircular = "Круглые кнопки";
        const string zpRectangular = "Прямоугольные кнопки";

        public ZoomPanelStyleConverter(Type type)
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
                case zpCircular: return ZoomPanelStyle.CircularButtons;
                case zpRectangular: return ZoomPanelStyle.RectangularButtons;
            }
            return ZoomPanelStyle.CircularButtons;
        }

        public static string ToString(object value)
        {
            switch ((ZoomPanelStyle)value)
            {
                case ZoomPanelStyle.CircularButtons: return zpCircular;
                case ZoomPanelStyle.RectangularButtons: return zpRectangular;
            }
            return string.Empty;
        }
    }
}