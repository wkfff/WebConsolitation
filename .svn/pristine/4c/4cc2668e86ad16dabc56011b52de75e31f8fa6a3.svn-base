using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Цветовая схема
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorModelBrowseClass
    {
        #region Поля

        private ColorAppearance colorApperance;

        #endregion

        #region Свойства

        /// <summary>
        /// Уровень прозрачности
        /// </summary>
        [Category("Цветовая схема")]
        [Description("Уровень прозрачности")]
        [DisplayName("Уровень прозрачности")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte AlphaLevel
        {
            get { return colorApperance.AlphaLevel; }
            set { colorApperance.AlphaLevel = value; }
        }

        /// <summary>
        /// Начальный цвет
        /// </summary>
        [Category("Цветовая схема")]
        [Description("Начальный цвет")]
        [DisplayName("Начальный цвет")]
        [DefaultValue(typeof(Color), "DarkGoldenRod")]
        [Browsable(true)]
        public Color ColorBegin
        {
            get { return colorApperance.ColorBegin; }
            set { colorApperance.ColorBegin = value; }
        }

        /// <summary>
        /// Конечный цвет
        /// </summary>
        [Category("Цветовая схема")]
        [Description("Конечный цвет")]
        [DisplayName("Конечный цвет")]
        [DefaultValue(typeof(Color), "Navy")]
        [Browsable(true)]
        public Color ColorEnd
        {
            get { return colorApperance.ColorEnd; }
            set { colorApperance.ColorEnd = value; }
        }

        /// <summary>
        /// Градация серого
        /// </summary>
        [Category("Цветовая схема")]
        [Description("Градация серого")]
        [DisplayName("Градация серого")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool GrayScale
        {
            get { return colorApperance.Grayscale; }
            set { colorApperance.Grayscale = value; }
        }

        /// <summary>
        /// Стиль цветовой схемы
        /// </summary>
        [Category("Цветовая схема")]
        [Description("Стиль цветовой схемы")]
        [DisplayName("Стиль")]
        [DefaultValue(ColorModels.PureRandom)]
        [Browsable(true)]
        public ColorModels ModelStyle
        {
            get { return colorApperance.ModelStyle; }
            set { colorApperance.ModelStyle = value; }
        }

        /// <summary>
        /// Стиль шкалы цветов
        /// </summary>
        [Category("Цветовая схема")]
        [Description("Стиль шкалы цветов")]
        [DisplayName("Стиль шкалы цветов")]
        [DefaultValue(ColorScaling.None)]
        [TypeConverter(typeof(ColorScalingTypeConverter))]
        [Browsable(true)]
        public ColorScaling Scaling
        {
            get { return colorApperance.Scaling; }
            set { colorApperance.Scaling = value; }
        }

        #endregion

        public ColorModelBrowseClass(ColorAppearance colorApperance)
        {
            this.colorApperance = colorApperance;
        }

        public override string ToString()
        {
            return ModelStyle + "; " + ColorBegin.Name + "; " + ColorEnd.Name + "; " + AlphaLevel;
        }
    }
}