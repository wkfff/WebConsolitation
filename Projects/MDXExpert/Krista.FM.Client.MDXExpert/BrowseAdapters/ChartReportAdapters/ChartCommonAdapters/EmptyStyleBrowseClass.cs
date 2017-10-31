using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EmptyStyleBrowseClass
    {
        #region Поля

        private EmptyAppearance emptyAppearance;
        private LineStyleBrowseClass lineStyleBrowse;
        private PaintElementBrowseClass paintElementBrowse;
        private PaintElementBrowseClass pointPaintElementBrowse;
        private PointStyleBrowseClass pointStyleBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Изменение стиля линии
        /// </summary>
        [Description("Изменение стиля линии")]
        [DisplayName("Изменение стиля линии")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EnableLineStyle
        {
            get { return emptyAppearance.EnableLineStyle; }
            set { emptyAppearance.EnableLineStyle = value; }
        }

        /// <summary>
        /// Изменение элемента отображения
        /// </summary>
        [Description("Изменение элемента отображения")]
        [DisplayName("Изменение элемента отображения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EnablePE
        {
            get { return emptyAppearance.EnablePE; }
            set { emptyAppearance.EnablePE = value; }
        }

        /// <summary>
        /// Изменение отображения точки
        /// </summary>
        [Description("Изменение отображения точки")]
        [DisplayName("Изменение отображения точки")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EnablePoint
        {
            get { return emptyAppearance.EnablePoint; }
            set { emptyAppearance.EnablePoint = value; }
        }

        /// <summary>
        /// Стиль линии
        /// </summary>
        [Description("Стиль линии")]
        [DisplayName("Стиль линии")]
        [Browsable(true)]
        public LineStyleBrowseClass LineStyleBrowse
        {
            get { return lineStyleBrowse; }
            set { lineStyleBrowse = value; }
        }

        /// <summary>
        /// Стиль элемента отображения
        /// </summary>
        [Description("Стиль элемента отображения")]
        [DisplayName("Стиль элемента отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        /// <summary>
        /// Стиль элемента отображения точки
        /// </summary>
        [Description("Стиль элемента отображения точки")]
        [DisplayName("Стиль элемента отображения точки")]
        [Browsable(true)]
        public PaintElementBrowseClass PointPaintElementBrowse
        {
            get { return pointPaintElementBrowse; }
            set { pointPaintElementBrowse = value; }
        }

        /// <summary>
        /// Стиль точки
        /// </summary>
        [Description("Стиль точки")]
        [DisplayName("Стиль точки")]
        [Browsable(true)]
        public PointStyleBrowseClass PointStyleBrowse
        {
            get { return pointStyleBrowse; }
            set { pointStyleBrowse = value; }
        }

        /// <summary>
        /// Тип элемента в легенде
        /// </summary>
        [Description("Тип элемента в легенде")]
        [DisplayName("Тип элемента в легенде")]
        [DefaultValue(LegendEmptyDisplayType.PE)]
        [TypeConverter(typeof(LegendEmptyDisplayTypeConverter))]
        [Browsable(true)]
        public LegendEmptyDisplayType LegendDisplayType
        {
            get { return emptyAppearance.LegendDisplayType; }
            set { emptyAppearance.LegendDisplayType = value; }
        }

        /// <summary>
        /// Отображение в легенде
        /// </summary>
        [Description("Отображение в легенде")]
        [DisplayName("Отображение в легенде")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowInLegend
        {
            get { return emptyAppearance.ShowInLegend; }
            set { emptyAppearance.ShowInLegend = value; }
        }

        /// <summary>
        /// Уникальный ключ
        /// </summary>
        [Description("Уникальный ключ")]
        [DisplayName("Уникальный ключ")]
        [Browsable(true)]
        public string Key
        {
            get { return emptyAppearance.Key; }
            set { emptyAppearance.Key = value; }
        }

        /// <summary>
        /// Подпись пустого значения
        /// </summary>
        [Description("Подпись пустого значения")]
        [DisplayName("Подпись пустого значения")]
        [DefaultValue("Пустое")]
        [Browsable(true)]
        public string Text
        {
            get { return emptyAppearance.Text; }
            set { emptyAppearance.Text = value; }
        }

        #endregion

        public EmptyStyleBrowseClass(EmptyAppearance emptyAppearance)
        {
            this.emptyAppearance = emptyAppearance;

            lineStyleBrowse = new LineStyleBrowseClass(emptyAppearance.LineStyle);
            paintElementBrowse = new PaintElementBrowseClass(emptyAppearance.PE);
            pointPaintElementBrowse = new PaintElementBrowseClass(emptyAppearance.PointPE);
            pointStyleBrowse = new PointStyleBrowseClass(emptyAppearance.PointStyle);
            emptyAppearance.Text = "Пустое";
        }

        public override string ToString()
        {
            return Text;
        }
    }
}