using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;
using System.Data;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapLayerBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private Layer layer;
        private List<Shape> shapes = new List<Shape>();
        private bool displayEmptyShapeNames;
        private MapControl map = null;

        #endregion

        #region Свойства

        [Browsable(false)]
        public bool LayerHasShapes
        {
            get { return this.shapes.Count > 0; }
        }

        [Browsable(false)]
        public MapControl Map
        {
            get { return this.map; }
            set { this.map = value; }
        }

        [Description("Цвет слоя")]
        [DisplayName("Цвет")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public Color Color
        {
            get { return GetColor(); }
            set { SetColor(value); }
        }


        [Description("Расположение подписей в объектах")]
        [DisplayName("Расположение подписей")]
        [TypeConverter(typeof(ContentAlignmentConverter))]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public ContentAlignment TextAlignment
        {
            get { return GetTextAlignment(); }
            set { SetTextAlignment(value); }
        }
        
        [Description("Видимость подписей в объектах")]
        [DisplayName("Видимость подписей")]
        [TypeConverter(typeof(TextVisibilityConverter))]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public TextVisibility TextVisibility
        {
            get { return GetTextVisibility(); }
            set { SetTextVisibility(value); }
        }
        /*
        [Description("Отображать слой")]
        [DisplayName("Отображать слой")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get 
            { 
                return GetVisible(); 
            }
            set 
            { 
                SetVisible(value); 
            }
        }
        */
        [Description("Смещение центральной точки объекта")]
        [DisplayName("Смещение центра")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Editor(typeof(OffsetPointEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public MapPoint CentralPointOffset
        {
            get
            {
                return GetCentralPointOffset();
            }
            set
            {
                SetCentralPointOffset(value);
            }
        }
        /*
        [Description("Показывать имена объектов с пустыми данными")]
        [DisplayName("Показывать имена объектов с пустыми данными")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool DisplayEmptyShapeNames
        {
            get
            {
                return (bool)this.layer.Tag;
            }
            set
            {
                this.layer.Tag = value;
                SetDisplayEmptyShapeNames(value);
            }
        }
        */

        #endregion
        
        public MapLayerBrowseClass(Layer layer)
        {
            this.layer = layer;
            //this.map = (MapCore)this.layer.ParentElement;
            GetShapes();
        }

        public MapLayerBrowseClass(Layer layer, MapControl map)
        {
            this.layer = layer;
            this.map = map;
            GetShapes();
        }


        private void GetShapes()
        {
            if (this.map == null)
            {
                return;
            }

            foreach (Shape sh in map.Shapes)
            {
                if (this.layer.Name == sh.Layer)
                {
                    this.shapes.Add(sh);
                }
            }
        }

        public override string ToString()
        {
            return "";
        }

        #region геттеры и сеттеры

        private Group GetShapeGroup()
        {
            MapCore map = (MapCore)this.layer.ParentElement;

            if (map == null)
            {
                return null;
            }

            return (Group)map.Groups.GetByName(layer.Name);
        }

        private void SetVisible(bool visible)
        {
            this.layer.Visibility = visible ? LayerVisibility.Shown : LayerVisibility.Hidden;
            
            Group group = GetShapeGroup();

            if (group != null)
            {
                group.Visible = visible;
            }

        }

        private bool GetVisible()
        {
            return (!(this.layer.Visibility == LayerVisibility.Hidden));
        }

        private void SetColor(Color color)
        {
            Group group = GetShapeGroup();

            if (group != null)
            {
                group.Color = color;
            }

            /*
            foreach (Shape sh in this.shapes)
            {
                sh.Color = color;
            }
             */
        }

        private Color GetColor()
        {
            MapCore map = (MapCore)this.layer.ParentElement;

            if (map == null)
            {
                return Color.Empty;
            }

            Group group = (Group)map.Groups.GetByName(layer.Name);

            if (group != null)
            {
                return group.Color;
            }

            return Color.Empty;

            /*
            return LayerHasShapes ? this.shapes[0].Color : Color.Empty;
             */
        }


        private void SetTextAlignment(ContentAlignment alignment)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.TextAlignment = alignment;
            }
        }

        private ContentAlignment GetTextAlignment()
        {
            return LayerHasShapes ? this.shapes[0].TextAlignment : ContentAlignment.MiddleCenter;
        }

        private void SetTextVisibility(TextVisibility visibility)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.TextVisibility = visibility;
            }
        }

        private TextVisibility GetTextVisibility()
        {
            return LayerHasShapes ? this.shapes[0].TextVisibility : TextVisibility.Auto;
        }


        private void SetCentralPointOffset(MapPoint offset)
        {
            double intervalX, intervalY;

            foreach (Shape sh in this.shapes)
            {
                intervalX = (sh.ShapeData.MaximumExtent.X - sh.ShapeData.MinimumExtent.X) / 2;
                intervalY = (sh.ShapeData.MaximumExtent.Y - sh.ShapeData.MinimumExtent.Y) / 2;

                sh.CentralPointOffset.X = intervalX * offset.X;
                sh.CentralPointOffset.Y = intervalY * offset.Y;
            }
        }

        private MapPoint GetCentralPointOffset()
        {
            if (!LayerHasShapes)
            {
                return new MapPoint(0, 0);
            }

            Shape sh = this.shapes[0];

            double intervalX = (sh.ShapeData.MaximumExtent.X - sh.ShapeData.MinimumExtent.X) / 2;
            double intervalY = (sh.ShapeData.MaximumExtent.Y - sh.ShapeData.MinimumExtent.Y) / 2;

            double offsetX = sh.CentralPointOffset.X / intervalX;
            double offsetY = sh.CentralPointOffset.Y / intervalY;


            return new MapPoint(offsetX, offsetY);
        }


        #endregion



    }

    public class TextVisibilityConverter : EnumConverter
    {
        const string tvAuto = "Автоматическая";
        const string tvHidden = "Скрывать";
        const string tvShown = "Показывать";

        public TextVisibilityConverter(Type type)
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
                case tvAuto: return TextVisibility.Auto;
                case tvHidden: return TextVisibility.Hidden;
                case tvShown: return TextVisibility.Shown;
            }
            return TextVisibility.Auto;
        }

        public static string ToString(object value)
        {
            switch ((TextVisibility)value)
            {
                case TextVisibility.Auto: return tvAuto;
                case TextVisibility.Hidden: return tvHidden;
                case TextVisibility.Shown: return tvShown;
            }
            return string.Empty;
        }
    }

}
