using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    class MarkerTypeConverter : EnumConverter
    {
        const string msCircle = "Круг";
        const string msDiamond = "Ромб";
        const string msNone = "Нет";
        const string msPentagon = "Пятиугольник";
        const string msRectangle = "Прямоугольник";
        const string msStar = "Звезда";
        const string msTrapezoid = "Трапеция";
        const string msTriangle = "Треугольник";
        const string msWedge = "Призма";

        public MarkerTypeConverter(Type type)
            : base(type)
        {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(
          ITypeDescriptorContext context)
        {
            List<MarkerStyle> markerStyles = new List<MarkerStyle>();
            markerStyles.Add(MarkerStyle.Circle);
            markerStyles.Add(MarkerStyle.Diamond);
            markerStyles.Add(MarkerStyle.Pentagon);
            markerStyles.Add(MarkerStyle.Rectangle);
            markerStyles.Add(MarkerStyle.Star);
            markerStyles.Add(MarkerStyle.Trapezoid);
            markerStyles.Add(MarkerStyle.Triangle);
            markerStyles.Add(MarkerStyle.Wedge);
            return new StandardValuesCollection(markerStyles);
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
                case msCircle: return MarkerStyle.Circle;
                case msDiamond: return MarkerStyle.Diamond;
                //case msNone: return MarkerStyle.None;
                case msPentagon: return MarkerStyle.Pentagon;
                case msRectangle: return MarkerStyle.Rectangle;
                case msStar: return MarkerStyle.Star;
                case msTrapezoid: return MarkerStyle.Trapezoid;
                case msTriangle: return MarkerStyle.Triangle;
                case msWedge: return MarkerStyle.Wedge;
            }
            return MarkerStyle.None;
        }

        public static string ToString(object value)
        {
            switch ((MarkerStyle)value)
            {
                case MarkerStyle.Circle: return msCircle;
                case MarkerStyle.Diamond: return msDiamond;
                //case MarkerStyle.None: return msNone;
                case MarkerStyle.Pentagon: return msPentagon;
                case MarkerStyle.Rectangle: return msRectangle;
                case MarkerStyle.Star: return msStar;
                case MarkerStyle.Trapezoid: return msTrapezoid;
                case MarkerStyle.Triangle: return msTriangle;
                case MarkerStyle.Wedge: return msWedge;
            }
            return string.Empty;
        }
    }
}