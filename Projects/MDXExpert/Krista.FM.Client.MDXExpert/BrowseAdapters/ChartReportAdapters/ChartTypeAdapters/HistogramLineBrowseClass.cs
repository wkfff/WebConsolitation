using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Линии в диаграмме HistogramChart
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HistogramLineBrowseClass
    {
        #region Поля

        private HistogramLineAppearance histogramLineApperance;
        private PaintElementBrowseClass paintElementBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Стиль линий
        /// </summary>
        [Category("Линии")]
        [Description("Стиль линий")]
        [DisplayName("Стиль")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return histogramLineApperance.DrawStyle; }
            set { histogramLineApperance.DrawStyle = value; }
        }

        /// <summary>
        /// Начало линии
        /// </summary>
        [Category("Линии")]
        [Description("Начало линии")]
        [DisplayName("Начало линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return histogramLineApperance.StartStyle; }
            set { histogramLineApperance.StartStyle = value; }
        }

        /// <summary>
        /// Конец линии
        /// </summary>
        [Category("Линии")]
        [Description("Конец линии")]
        [DisplayName("Конец линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return histogramLineApperance.EndStyle; }
            set { histogramLineApperance.EndStyle = value; }
        }

        /// <summary>
        /// Закрашивать область под линией
        /// </summary>
        [Category("Линии")]
        [Description("Закрашивать область под линией")]
        [DisplayName("Заливка")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool FillArea
        {
            get { return histogramLineApperance.FillArea; }
            set { histogramLineApperance.FillArea = value; }
        }

        /// <summary>
        /// Формат метки линии
        /// </summary>
        [Category("Линии")]
        [Description("Формат метки линии")]
        [DisplayName("Формат метки")]
        [Browsable(true)]
        public string LineLabel
        {
            get { return histogramLineApperance.LineLabel; }
            set { histogramLineApperance.LineLabel = value; }
        }

        /// <summary>
        /// Отображение линий в легенде диаграммы
        /// </summary>
        [Category("Колонки")]
        [Description("Отображение линий в легенде диаграммы")]
        [DisplayName("Отображение в легенде")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ShowInLegend
        {
            get { return histogramLineApperance.ShowInLegend; }
            set { histogramLineApperance.ShowInLegend = value; }
        }

        /// <summary>
        /// Видимость линий в диаграмме
        /// </summary>
        [Category("Колонки")]
        [Description("Показывать линии в диаграмме")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return histogramLineApperance.Visible; }
            set { histogramLineApperance.Visible = value; }
        }

        /// <summary>
        /// Стиль элемента отображения в диаграмме
        /// </summary>
        [Category("Колонки")]
        [Description("Стиль элемента отображения в диаграмме")]
        [DisplayName("Стиль элемента отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        #endregion

        public HistogramLineBrowseClass(HistogramLineAppearance histogramLineApperance)
        {
            this.histogramLineApperance = histogramLineApperance;
            paintElementBrowse = new PaintElementBrowseClass(histogramLineApperance.PE);
        }

        public override string ToString()
        {
            return LineLabel + "; " + LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + BooleanTypeConverter.ToString(Visible);
        }
    }
}