using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapSymbolBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private Symbol symbol;
        private SymbolOffsetBrowse symbolOffsetBrowse;

        #endregion

        #region Свойства

        [Description("Расположение подписи")]
        [DisplayName("Расположение подписи")]
        [TypeConverter(typeof(TextAlignmentConverter))]
        [Browsable(true)]
        public TextAlignment TextAlignment
        {
            get { return symbol.TextAlignment; }
            set { symbol.TextAlignment = value; }
        }

        [Description("Смещение значка")]
        [DisplayName("Смещение значка")]
        [Editor(typeof(OffsetPointEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(OffsetConverter))]
        [Browsable(true)]
        public MapPoint Offset
        {
            get {
                return GetSymbolOffset(); 
                }
            set
            {
                SetSymbolOffset(value);
            }
            /*
            get{ return this.symbolOffsetBrowse;}
            set{ this.symbolOffsetBrowse = value;}
            */
        }

        [Description("Шрифт")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return symbol.Font; }
            set { symbol.Font = value; }
        }

        [Description("Цвет шрифта")]
        [DisplayName("Цвет шрифта")]
        [Browsable(true)]
        public Color TextColor
        {
            get { return symbol.TextColor; }
            set { symbol.TextColor = value; }
        }

        #endregion

        public MapSymbolBrowseClass(Symbol symbol)
        {
            this.symbol = symbol;
            //this.symbolOffsetBrowse = new SymbolOffsetBrowse(this.symbol.Offset);
        }

        private void SetSymbolOffset(MapPoint offset)
        {
            double intervalX = 100;
            double intervalY = 100;

            symbol.Offset.X = intervalX * offset.X;
            symbol.Offset.Y = intervalY * offset.Y;
        }

        private MapPoint GetSymbolOffset()
        {
            double intervalX = 100;
            double intervalY = 100;

            double offsetX = symbol.Offset.X / intervalX;
            double offsetY = symbol.Offset.Y / intervalY;

            /*
            Offset cpOffset = new Offset();
            cpOffset.X = offsetX;
            cpOffset.Y = offsetY;
            return cpOffset; */
            return new MapPoint(offsetX, offsetY);
        }


        public override string ToString()
        {
            return "";
        }

    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SymbolOffsetBrowse : TypeConverter
    {
        #region Поля

        private MapPoint offset;

        #endregion

        #region Свойства

        [Description("Смещение значка по горизонтали (от -50 до 50)")]
        [DisplayName("X")]
        [Browsable(true)]
        public double Height
        {
            get { return this.offset.X; }
            set { this.offset.X = value; }
        }

        [Description("Смещение значка по вертикали (от -50 до 50)")]
        [DisplayName("Y")]
        [Browsable(true)]
        public double Width
        {
            get { return this.offset.Y; }
            set { this.offset.Y = value; }
        }

        #endregion

        public SymbolOffsetBrowse(MapPoint offset)
        {
            this.offset = offset;
        }


        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return (destType == typeof(MapPoint));
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destType)
        {
            return ((MapPoint)value).X.ToString() + "; " + ((MapPoint)value).Y.ToString();
        }


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string[] points = ((string)value).Split(';');
            double x = Double.Parse(points[0]);
            double y = Double.Parse(points[1]);
            MapPoint mapPoint = new MapPoint(x, y);
            return mapPoint;
        }


        public static string ToString(object value)
        {
            return value.ToString();
        }

    }


}
