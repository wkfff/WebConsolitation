using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Линии полос оси
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StripLineBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private UltraChart chart;
        private StripLineAppearance stripLineAppearance;
        private StripLineBorderBrowseClass stripLineBorderBrowse;
        private StripLineAreaBrowseClass stripLineAreaBrowse;
        private StripLineArea3DBrowseClass stripLineArea3DBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип диаграммы
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
        }

        /// <summary>
        /// Объемная ли диаграмма с осями
        /// </summary>
        [Browsable(false)]
        public bool Is3D
        {
            get
            {
                return
                    (ChartType == ChartType.BarChart3D || ChartType == ChartType.CylinderBarChart3D ||
                     ChartType == ChartType.CylinderStackBarChart3D || ChartType == ChartType.Stack3DBarChart ||
                     ChartType == ChartType.AreaChart3D || ChartType == ChartType.SplineAreaChart3D ||
                     ChartType == ChartType.ColumnChart3D || ChartType == ChartType.CylinderColumnChart3D ||
                     ChartType == ChartType.CylinderStackColumnChart3D || ChartType == ChartType.Stack3DColumnChart ||
                     ChartType == ChartType.HeatMapChart3D || ChartType == ChartType.LineChart3D ||
                     ChartType == ChartType.SplineChart3D || ChartType == ChartType.BubbleChart3D ||
                     ChartType == ChartType.PointChart3D);
            }
        }

        /// <summary>
        /// Интервал между линиями
        /// </summary>
        [Category("Линии пересечения")]
        [Description("Интервал между линиями")]
        [DisplayName("Интервал")]
        [DefaultValue(2)]
        [Browsable(true)]
        public int Interval
        {
            get { return stripLineAppearance.Interval; }
            set { stripLineAppearance.Interval = value; }
        }

        /// <summary>
        /// Видимость
        /// </summary>
        [Category("Линии пересечения")]
        [Description("Показывать линии")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Visible
        {
            get { return stripLineAppearance.Visible; }
            set { stripLineAppearance.Visible = value; }
        }

        /// <summary>
        /// Стиль границы  
        /// </summary>
        [Category("Линии полос")]
        [Description("Стиль границы")]
        [DisplayName("Граница")]
        [DynamicPropertyFilter("Is3D", "False")]
        [Browsable(true)]
        public StripLineBorderBrowseClass StripLineBorder
        {
            get { return stripLineBorderBrowse; }
            set
            {
                stripLineBorderBrowse = value;
                chart.InvalidateLayers();
            }
        }
        
        /// <summary>
        /// Способы заливки области для 2D диаграмм
        /// </summary>
        [Category("Линии полос")]
        [Description("Способы заливки области")]
        [DisplayName("Способы заливки")]
        [DynamicPropertyFilter("Is3D", "False")]
        [Browsable(true)]
        public StripLineAreaBrowseClass StripLineAreaBrowse
        {
            get { return stripLineAreaBrowse; }
            set
            {
                stripLineAreaBrowse = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// Способы заливки области для 3D диаграмм
        /// </summary>
        [Category("Линии полос")]
        [Description("Способы заливки области")]
        [DisplayName("Способы заливки")]
        [DynamicPropertyFilter("Is3D", "True")]
        [Browsable(true)]
        public StripLineArea3DBrowseClass StripLineAreaBrowse3D 
        {
            get { return stripLineArea3DBrowse; }
            set
            {
                stripLineArea3DBrowse = value;
                chart.InvalidateLayers();
            }
        }
        
        #endregion

        public StripLineBrowseClass(StripLineAppearance stripLineAppearance, UltraChart chart)
        {
            this.stripLineAppearance = stripLineAppearance;
            this.chart = chart;
            stripLineBorderBrowse = new StripLineBorderBrowseClass(stripLineAppearance);
            stripLineAreaBrowse = new StripLineAreaBrowseClass(stripLineAppearance, chart);
            stripLineArea3DBrowse = new StripLineArea3DBrowseClass(stripLineAppearance, chart);
        }

        public override string ToString()
        {
            string str = Is3D
                             ? stripLineArea3DBrowse.ToString()
                             : PaintElementTypeConverter.CustomToString(stripLineAreaBrowse.ElementType);
            return BooleanTypeConverter.ToString(Visible) + "; " + Interval + "; " + str;
        }
    }
}