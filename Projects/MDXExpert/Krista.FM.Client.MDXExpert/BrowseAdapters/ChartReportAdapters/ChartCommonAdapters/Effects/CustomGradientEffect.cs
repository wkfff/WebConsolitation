using System.ComponentModel;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomGradientEffect : FilterablePropertyBase
    {
        private GradientEffect _gradientEffect;
        /// <summary>
        /// Гамма
        /// </summary>
        [Category("Свойства")]
        [Description("Гамма")]
        [DisplayName("Гамма")]
        [TypeConverter(typeof(GradientColoringStyleConverter))]
        [Browsable(true)]
        public new GradientColoringStyle Coloring
        {
            get { return this._gradientEffect.Coloring; }
            set { this._gradientEffect.Coloring = value; }
        }

        /// <summary>
        /// Стиль
        /// </summary>
        [Category("Свойства")]
        [Description("Стиль")]
        [DisplayName("Стиль")]
        [Editor(typeof(GradientEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(GradientStyleConverter))]
        [Browsable(true)]
        public GradientStyle Style
        {
            get { return this._gradientEffect.Style; }
            set { this._gradientEffect.Style = value; }
        }

        /// <summary>
        /// Применить
        /// </summary>
        [Category("Свойства")]
        [Description("Применить")]
        [DisplayName("Применить")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public new bool Enabled
        {
            get { return this._gradientEffect.Enabled; }
            set { this._gradientEffect.Enabled = value; }
        }

        public CustomGradientEffect(GradientEffect gradientEffect)
        {
            this._gradientEffect = gradientEffect;
        }

        public override string ToString()
        {
            return "Градиент";
        }
    }
}