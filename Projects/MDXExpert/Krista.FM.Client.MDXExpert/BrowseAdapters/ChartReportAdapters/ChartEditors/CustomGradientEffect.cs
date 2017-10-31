using System.ComponentModel;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomGradientEffect : GradientEffect
    {
        /// <summary>
        /// Гамма
        /// </summary>
        [Category("Свойства")]
        [Description("Гамма")]
        [DisplayName("Гамма")]
        [Browsable(true)]
        public new GradientColoringStyle Coloring
        {
            get { return base.Coloring; }
            set { base.Coloring = value; }
        }

        /// <summary>
        /// Стиль
        /// </summary>
        [Category("Свойства")]
        [Description("Стиль")]
        [DisplayName("Стиль")]
        [Browsable(true)]
        public new GradientStyle Style
        {
            get { return base.Style; }
            set { base.Style = value; }
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
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }

        public CustomGradientEffect(IChartComponent component)
            : base(component)
        {

        }

        public override string ToString()
        {
            return "Градиент";
        }
    }
}