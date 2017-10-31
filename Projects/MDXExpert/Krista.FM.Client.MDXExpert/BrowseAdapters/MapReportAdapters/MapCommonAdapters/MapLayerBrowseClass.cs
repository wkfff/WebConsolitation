using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;
using System.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapLayerBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private Layer layer;
        private List<Shape> shapes = new List<Shape>();
        private bool displayEmptyShapeNames;
        private MapControl map = null;
        private MapReportElement mapElement;
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

        [Description("Подписи объектов слоя")]
        [DisplayName("Подписи объектов")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public string Text
        {
            get { return GetText(); }
            set { SetText(value); }
        }

        [Description("Шрифт для подписей объектов слоя")]
        [DisplayName("Шрифт")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return GetFont(); }
            set { SetFont(value); }
        }

        [Description("Цвет шрифта для подписей объектов слоя")]
        [DisplayName("Цвет шрифта")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return GetFontColor(); }
            set { SetFontColor(value); }
        }


        [Description("Подсказки объектов слоя")]
        [DisplayName("Подсказки объектов")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public string Tooltip
        {
            get { return GetToolTip(); }
            set { SetToolTip(value); }
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

        [Category("Границы объектов")]
        [Description("Цвет границы объектов")]
        [DisplayName("Цвет")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return GetBorderColor(); }
            set { SetBorderColor(value); }
        }

        [Category("Границы объектов")]
        [Description("Вид границы объектов")]
        [DisplayName("Вид")]
        [TypeConverter(typeof(MapDashStyleConverter))]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public MapDashStyle BorderStyle
        {
            get { return GetBorderStyle(); }
            set { SetBorderStyle(value); }
        }

        [Category("Границы объектов")]
        [Description("Толщина границы объектов")]
        [DisplayName("Толщина")]
        [DynamicPropertyFilter("LayerHasShapes", "True")]
        [Browsable(true)]
        public int BorderWidth
        {
            get { return GetBorderWidth(); }
            set { SetBorderWidth(value); }
        }


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
        

        #endregion
        
        public MapLayerBrowseClass(Layer layer)
        {
            this.layer = layer;
            //this.map = (MapCore)this.layer.ParentElement;
            GetShapes();
        }

        public MapLayerBrowseClass(Layer layer, MapReportElement mapElement)
        {
            this.layer = layer;
            this.mapElement = mapElement;
            this.map = mapElement.Map;
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



        private void SetText(string text)
        {
            string currShapeCaption = this.mapElement.GetShapeFieldCaption();
            foreach (Shape sh in this.shapes)
            {
                if (sh[currShapeCaption] == null)
                    continue;
                /*
                if (sh.Name.Contains("Shape") || sh.Name.Contains("(no data)"))
                {
                    continue;
                }*/
                sh.Text = text;
            }
        }

        private Font GetFont()
        {
            return LayerHasShapes ? this.shapes[0].Font : null;
        }

        private void SetFont(Font font)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.Font = font;
            }
        }


        private Color GetFontColor()
        {
            return LayerHasShapes ? this.shapes[0].TextColor : Color.Black;
        }

        private void SetFontColor(Color fontColor)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.TextColor = fontColor;
            }
        }


        private string GetText()
        {
            return LayerHasShapes ? this.shapes[0].Text : String.Empty;
        }


        private void SetToolTip(string toolTip)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.ToolTip = toolTip;
            }
        }

        private string GetToolTip()
        {
            return LayerHasShapes ? this.shapes[0].ToolTip : String.Empty;
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

        private void SetBorderColor(Color color)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.BorderColor  = color;
            }
        }

        private Color GetBorderColor()
        {
            return LayerHasShapes ? this.shapes[0].BorderColor : Color.DarkGray;
        }

        private void SetBorderStyle(MapDashStyle style)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.BorderStyle = style;
            }
        }

        private MapDashStyle GetBorderStyle()
        {
            return LayerHasShapes ? this.shapes[0].BorderStyle : MapDashStyle.Solid;
        }

        private void SetBorderWidth(int width)
        {
            foreach (Shape sh in this.shapes)
            {
                sh.BorderWidth = width;
            }
        }

        private int GetBorderWidth()
        {
            return LayerHasShapes ? this.shapes[0].BorderWidth : 1;
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

        private void SetDisplayEmptyShapeNames(bool value)
        {
            string currShapeCaption = this.mapElement.GetShapeFieldCaption();
            foreach (Shape sh in this.shapes)
            {
                if (sh[currShapeCaption] == null)
                {
                    sh.Text = "";
                    sh.ToolTip = "#NAME";
                    continue;
                }

                if (IsNullShape((string)sh["CODE"]))
                {
                    //если объект с пустыми данными, то добавляем пробел в шаблон, чтоб на него не действовала заливка
                    sh.Text = value ? "#" + this.mapElement.GetShapeFieldCaption() + " " : "";
                    sh.ToolTip = "#NAME ";
                }
                else
                {
                    sh.Text = "#" + this.mapElement.GetShapeFieldCaption();
                    sh.ToolTip = "";
                }
            }
            this.mapElement.SetFillShapesByDefault();
        }

        private bool GetDisplayEmptyShapeNames()
        {
            if (this.shapes.Count > 0)
            {
                return (this.shapes[0].Text != "");
            }
            return true;
        }

        public bool IsNullShape(string shapeCode)
        {
            if ((this.map == null) || (this.map.DataSource == null))
            {
                return false;
            }

            foreach (DataTable dt in ((DataSet)this.Map.DataSource).Tables)
            {
                DataRow row = MapHelper.GetTableRowByObjCode(dt, shapeCode);
                if (row != null)
                {
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }



        #endregion



    }


}
