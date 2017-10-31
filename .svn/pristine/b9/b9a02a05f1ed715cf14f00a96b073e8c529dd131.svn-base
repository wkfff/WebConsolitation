using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AreaChartBrowseClass
    {
        #region Поля

        private AreaChartAppearance areaChartAppearanece;
        // private ChartTextCollectionBrowseClass areaChartTextCollection;

        #endregion

        #region Свойства

        /// <summary>
        /// Толщина линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(2)]
        [Browsable(true)]
        public int LineThickness
        {
            get { return areaChartAppearanece.LineThickness; }
            set { areaChartAppearanece.LineThickness = value; }
        }

        /// <summary>
        /// Стиль линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль линии")]
        [DisplayName("Стиль линии")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle LineDrawStyle
        {
            get { return areaChartAppearanece.LineDrawStyle; }
            set { areaChartAppearanece.LineDrawStyle = value; }
        }

        /// <summary>
        /// Начало линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Начало линии")]
        [DisplayName("Начало линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle LineStartCapStyle
        {
            get { return areaChartAppearanece.LineStartCapStyle; }
            set { areaChartAppearanece.LineStartCapStyle = value; }
        }

        /// <summary>
        /// Конец линии
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Конец линии")]
        [DisplayName("Конец линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle LineEndCapStyle
        {
            get { return areaChartAppearanece.LineEndCapStyle; }
            set { areaChartAppearanece.LineEndCapStyle = value; }
        }

        /// <summary>
        /// Отображение меток между соседними сегментами диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение меток между соседними сегментами диаграммы")]
        [DisplayName("Отображение промежуточных меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return areaChartAppearanece.MidPointAnchors; }
            set { areaChartAppearanece.MidPointAnchors = value; }
        }

        /// <summary>
        /// Отображение пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение пустых значений")]
        [DisplayName("Пустые значения")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [DefaultValue(NullHandling.Zero)]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return areaChartAppearanece.NullHandling; }
            set { areaChartAppearanece.NullHandling = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return areaChartAppearanece.ChartText; }
        }

        /// <summary>
        /// Стили отображения пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стили отображения пустых значений")]
        [DisplayName("Стили отображения пустых значений")]
        [Editor(typeof(EmptyAppearanceCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public EmptyAppearanceCollection EmptyStyles
        {
            get { return areaChartAppearanece.EmptyStyles; }
        }

        /// <summary>
        /// Стили линий
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стили линий")]
        [DisplayName("Стили линий")]
        [Editor(typeof(LineAppearanceCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public LineAppearanceCollection LineAppearances
        {
            get { return areaChartAppearanece.LineAppearances; }
        }

        #endregion

        public AreaChartBrowseClass(AreaChartAppearance areaChartAppearanece)
        {
            this.areaChartAppearanece = areaChartAppearanece;
            //  this.areaChartTextCollection = new ChartTextCollectionBrowseClass(areaChartAppearanece.ChartComponent);
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(LineDrawStyle) + "; " + LineThickness;
        }
    }
}