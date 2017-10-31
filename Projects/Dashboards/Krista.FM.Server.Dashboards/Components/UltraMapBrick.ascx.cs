using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class UltraMapBrick : System.Web.UI.UserControl
    {
        #region Поля

        private DataTable mapDt = new DataTable();
        private Font defaultChartFont = new Font("Verdana", 8);
        private Font titleChartFont = new Font("Verdana", 10);
        private string dataFormatString;
        private string dataItemCaption;
        private string legendCaption;

        private MapRegionType mapRegionType;

        #endregion

        #region Свойства

        public DataTable DataTable
        {
            set { mapDt = value; }
            get { return mapDt; }
        }

        public string DataFormatString
        {
            set { dataFormatString = value; }
            get { return dataFormatString; }
        }

        public string DataItemCaption
        {
            set { dataItemCaption = value; }
            get { return dataItemCaption; }
        }

        public string LegendCaption
        {
            set { legendCaption = value; }
            get { return legendCaption; }
        }

        public Unit Width
        {
            set { MapControl.Width = CRHelper.GetChartWidth(value.Value); }
            get { return MapControl.Width; }
        }

        public Unit Height
        {
            set { MapControl.Height = CRHelper.GetChartHeight(value.Value); }
            get { return MapControl.Height; }
        }

        public MapControl Map
        {
            get { return MapControl; }
        }

        public MapRegionType MapRegionType
        {
            set { mapRegionType = value; }
            get { return mapRegionType; }
        }

        #endregion

        private Dictionary<string, CRHelper.MapShapeType> layerList = new Dictionary<string, CRHelper.MapShapeType>();
        private Dictionary<string, Type> shapeFieldList = new Dictionary<string, Type>();
        private Collection<ShapeRule> shapeRuleList = new Collection<ShapeRule>();

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            LoadShapes();
            LoadShapeFields();
            LoadShapeRules();
            SetMapAppearance();
            
            DataBind();
        }

        public virtual void DataBind()
        {

        }
        


        #region Загрузка полей форм

        private void LoadShapeFields()
        {
            MapControl.ShapeFields.Clear();
            foreach (string field in shapeFieldList.Keys)
            {
                MapControl.ShapeFields.Add(field);
                MapControl.ShapeFields[field].Type = shapeFieldList[field];
            }
        }
        
        public void AddShapeField(string fieldName, Type type)
        {
            if (!shapeFieldList.ContainsKey(fieldName))
            {
                shapeFieldList.Add(fieldName, type);
            }
        }
        
        #endregion
        
        #region Загрузка элементов карты

        private void LoadShapes()
        {
            MapControl.Shapes.Clear();
            foreach (string layer in layerList.Keys)
            {
                AddMapShape(layer, layerList[layer]);
            }
        }

        public void AddMapLayer(string layerFileName, CRHelper.MapShapeType shapeType)
        {
            if (!layerList.ContainsKey(layerFileName))
            {
                layerList.Add(layerFileName, shapeType);
            }
        }

        private void AddMapShape(string shapeFileName, CRHelper.MapShapeType shapeType)
        {
            if (!File.Exists(shapeFileName))
            {
                return;
            }

            int oldShapesCount = MapControl.Shapes.Count;

            MapControl.LoadFromShapeFile(shapeFileName, "NAME", true);
            MapControl.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < MapControl.Shapes.Count; i++)
            {
                Shape shape = MapControl.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        #endregion

        #region Загрузка правил закраски

        private void LoadShapeRules()
        {
            MapControl.ShapeRules.Clear();
            foreach (ShapeRule rule in shapeRuleList)
            {
                AddShapeRule(rule);
            }
        }

        public void AddShapeRule(ShapeRule rule)
        {
            if (!shapeRuleList.Contains(rule))
            {
                shapeRuleList.Add(rule);
            }
        }

        #endregion

        #region Настройка внешного вида

        protected virtual void SetMapAppearance()
        {
            MapControl.Meridians.Visible = false;
            MapControl.Parallels.Visible = false;
            MapControl.ZoomPanel.Visible = true;
            MapControl.NavigationPanel.Visible = true;
            MapControl.Viewport.EnablePanning = true;

            // добавляем легенду
            MapControl.Legends.Clear();



        }

        private void AddMapLegend(string legendId)
        {
            Legend legend = new Legend(legendId);
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = legendCaption;
            legend.AutoFitMinFontSize = 7;
            MapControl.Legends.Add(legend);
        }

        #endregion

        #region Обработчики карты

        protected void ChartControl_DataBinding(object sender, EventArgs e)
        {

        }

        #endregion

    }

    /// <summary>
    /// Цветовая раскраска
    /// </summary>
    public enum ColoringRule
    {
        // простая раскраска диапазоном
        SimpleRangeColoring,
        // более сложная раскраска диапазоном
        ComplexRangeColoring,
        // положительные/отрицательные
        PositiveNegativeColoring
    }

    /// <summary>
    /// Положение легенды
    /// </summary>
    public enum LegendPosition
    {
        LeftTop,
        LeftBottom,
        RightTop,
        RightBottom
    }

    /// <summary>
    /// Положение легенды
    /// </summary>
    public enum MapRegionType
    {
        // карта по федеральным данным
        Federal,
        // карта по региональным данным (территории, соседние, города, водные)
        Regional
    }
}