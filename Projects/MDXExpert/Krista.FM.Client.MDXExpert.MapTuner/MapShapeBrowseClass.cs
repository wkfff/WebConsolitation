using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapShapeBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private Shape shape;

        #endregion

        #region Свойства
        /*
        [Description("Цвет объекта")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return shape.Color; }
            set { shape.Color = value; }
        }

        [Description("Подпись объекта")]
        [DisplayName("Подпись")]
        [Browsable(true)]
        public string Text
        {
            get { return shape.Text; }
            set { shape.Text = value; }
        }
        
        [Description("Подсказка")]
        [DisplayName("Подсказка")]
        [Browsable(true)]
        public string Tooltip
        {
            get { return shape.ToolTip; }
            set { shape.ToolTip = value; }
        }
        */
        [Description("Расположение подписи")]
        [DisplayName("Расположение подписи")]
        [TypeConverter(typeof(ContentAlignmentConverter))]
        [Browsable(true)]
        public ContentAlignment TextAlignment
        {
            get { return shape.TextAlignment; }
            set { shape.TextAlignment = value; }
        }

        
        [Description("Видимость подписи")]
        [DisplayName("Видимость подписи")]
        [TypeConverter(typeof(TextVisibilityConverter))]
        [Browsable(true)]
        public TextVisibility TextVisibility
        {
            get { return shape.TextVisibility; }
            set { shape.TextVisibility = value; }
        }
        

        [Description("Смещение центральной точки объекта")]
        [DisplayName("Смещение центра")]
        [Editor(typeof(OffsetPointEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(OffsetConverter))]
        [Browsable(true)]
        public MapPoint CentralPointOffset
        {
            get {
                return GetCentralPointOffset(); 
                }
            set
            {
                SetCentralPointOffset(value);
            }
        }

        /*
        [Description("Шрифт")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return shape.Font; }
            set { shape.Font = value; }
        }

        [Description("Цвет шрифта")]
        [DisplayName("Цвет шрифта")]
        [Browsable(true)]
        public Color TextColor
        {
            get { return shape.TextColor; }
            set { shape.TextColor = value; }
        }
        */
        #endregion

        public MapShapeBrowseClass(Shape shape)
        {
            this.shape = shape;
        }

        private void SetCentralPointOffset(MapPoint offset)
        {
            double intervalX = (shape.ShapeData.MaximumExtent.X - shape.ShapeData.MinimumExtent.X) / 2;
            double intervalY = (shape.ShapeData.MaximumExtent.Y - shape.ShapeData.MinimumExtent.Y) / 2;

            shape.CentralPointOffset.X = intervalX * offset.X;
            shape.CentralPointOffset.Y = intervalY * offset.Y;
        }

        private MapPoint GetCentralPointOffset()
        {
            double intervalX = (shape.ShapeData.MaximumExtent.X - shape.ShapeData.MinimumExtent.X) / 2;
            double intervalY = (shape.ShapeData.MaximumExtent.Y - shape.ShapeData.MinimumExtent.Y) / 2;

            double offsetX = shape.CentralPointOffset.X / intervalX;
            double offsetY = shape.CentralPointOffset.Y / intervalY;

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


}
