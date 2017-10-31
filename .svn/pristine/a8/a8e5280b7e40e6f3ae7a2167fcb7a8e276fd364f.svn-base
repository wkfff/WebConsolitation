using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Легенда
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CompositeLegendBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private LabelStyleBrowseClass labelStyleBrowse;
        private UltraChart chart;

        #endregion

        #region Свойства

        /// <summary>
        /// Цвет границы легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Цвет границы легенды")]
        [DisplayName("Цвет границы")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color BorderColor
        {
            get
            {
                if (GetLegend != null)
                {
                    return GetLegend.Border.Color;
                }
                return Color.Black;
            }
            set
            {
                if (GetLegend != null)
                {
                    GetLegend.Border.Color = value;
                }
            }
        }

        /// <summary>
        /// Толщина границы легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Толщина границы легенды")]
        [DisplayName("Толщина границы")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int BorderThickness
        {
            get
            {
                if (GetLegend != null)
                {
                    return GetLegend.Border.Thickness;
                }
                return 1;
            }
            set
            {
                if (GetLegend != null)
                {
                    GetLegend.Border.Thickness = value;
                }
            }
        }

        /// <summary>
        /// Стиль границы легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Стиль границы легенды")]
        [DisplayName("Стиль границы")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle BorderStyle
        {
            get
            {
                if (GetLegend != null)
                {
                    return GetLegend.Border.DrawStyle;
                }
                return LineDrawStyle.Solid;
            }
            set
            {
                if (GetLegend != null)
                {
                    GetLegend.Border.DrawStyle = value;
                }
            }
        }

        /// <summary>
        /// Цвет фона легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Цвет фона легенды")]
        [DisplayName("Цвет фона")]
        [DefaultValue(typeof(Color), "FloralWhite")]
        [Browsable(true)]
        public Color BackColor
        {
            get
            {
                if (GetLegend != null)
                {
                    return GetLegend.PE.Fill;
                }
                return Color.FloralWhite;
            }
            set
            {
                if (GetLegend != null)
                {
                    GetLegend.PE.Fill = value;
                    chart.InvalidateLayers();
                }
            }
        }

        /// <summary>
        /// Уровень прозрачности фона
        /// </summary>
        [Category("Легенда")]
        [Description("Уровень прозрачности фона легенды")]
        [DisplayName("Уровень прозрачности фона")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "150")]
        [Browsable(true)]
        public byte AlphaLevel
        {
            get
            {
                if (GetLegend != null)
                {
                    return GetLegend.PE.FillOpacity;
                }
                return 150;
            }
            set
            {
                if (GetLegend != null)
                {
                    GetLegend.PE.FillOpacity = value;
                }
            }
        }

        /// <summary>
        /// Видимость легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Показывать легенду")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Visible
        {
            get
            {
                if (GetLegend != null)
                {
                    return GetLegend.Visible;
                }
                return false;
            }
            set
            {
                if (GetLegend != null)
                {
                    GetLegend.Visible = value;
                }
            }
        }

        /// <summary>
        /// Расположение легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Расположение легенды")]
        [DisplayName("Расположение")]
        [TypeConverter(typeof(LocationTypeConverter))]
        [DefaultValue(LegendLocation.Right)]
        [Browsable(true)]
        public LegendLocation Location
        {
            get
            {
                try
                {
                    return ((ICompositeLegendParams)chart.Parent.Parent.Parent).CompositeLegendLocation;
                }
                catch
                {
                    return LegendLocation.Right;
                }
            }
            set
            {
                try
                {
                    ((ICompositeLegendParams)chart.Parent.Parent.Parent).CompositeLegendLocation = value;
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Размер
        /// </summary>
        [Category("Легенда")]
        [Description("Размер в процентах от ширины/высоты области диаграммы")]
        [DisplayName("Размер")]
        [DefaultValue(25)]
        [Browsable(true)]
        public int SpanPercentage
        {
            get
            {
                try
                {
                    return ((ICompositeLegendParams)chart.Parent.Parent.Parent).CompositeLegendExtent;
                }
                catch
                {
                    return 25;
                }
            }
            set
            {
                try
                {
                    ((ICompositeLegendParams)chart.Parent.Parent.Parent).CompositeLegendExtent = value;
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Стиль метки
        /// </summary>
        [Category("Легенда")]
        [Description("Стиль метки")]
        [DisplayName("Стиль")]
        [Browsable(true)]
        public LabelStyleBrowseClass LabelStyleBrowse
        {
            get { return labelStyleBrowse; }
            set { labelStyleBrowse = value; }
        }

        /// <summary>
        /// Радиус углов границы
        /// </summary>
        [Category("Легенда")]
        [Description("Радиус углов границы")]
        [DisplayName("Радиус углов")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int CornerRadius
        {
            get { return GetLegend.Border.CornerRadius; }
            set { GetLegend.Border.CornerRadius = value; }
        }

        /// <summary>
        /// Выпуклость границы
        /// </summary>
        [Category("Легенда")]
        [Description("Выпуклость границы")]
        [DisplayName("Выпуклость")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool RaisedBrowse
        {
            get { return GetLegend.Border.Raised; }
            set { GetLegend.Border.Raised = value; }
        }

        /// <summary>
        /// Слои
        /// </summary>
        [Category("Граница")]
        [Description("Слои")]
        [DisplayName("Слои")]
        [Editor(typeof(ChartLayerPickerEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public ChartLayerCollection ChartLayers
        {
            get { return GetLegend.ChartLayers; }
        }

        [Browsable(false)]
        private CompositeLegend GetLegend
        {
            get
            {
                if (chart.CompositeChart.Legends.Count != 0)
                {
                    return chart.CompositeChart.Legends[0];
                }
                return null;
            }
        }

        #endregion

        public CompositeLegendBrowseClass(UltraChart chart)
        {
            this.chart = chart;
            
            labelStyleBrowse = new LabelStyleBrowseClass(GetLegend.LabelStyle);
        }

        public override string ToString()
        {
            return LocationTypeConverter.ToString(Location) + "; " + SpanPercentage;
        }
    }
}