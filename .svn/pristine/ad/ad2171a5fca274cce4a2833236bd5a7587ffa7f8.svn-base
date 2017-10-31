using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomStrokeEffect : StrokeEffect
    {
        /// <summary>
        /// Прозрачность штриха
        /// </summary>
        [Category("Свойства")]
        [Description("Прозрачность штриха")]
        [DisplayName("Прозрачность")]
        [Browsable(true)]
        public new byte StrokeOpacity
        {
            get { return base.StrokeOpacity; }
            set { base.StrokeOpacity = value; }
        }

        /// <summary>
        /// Ширина штриха
        /// </summary>
        [Category("Свойства")]
        [Description("Ширина штриха")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public new int StrokeWidth
        {
            get { return base.StrokeWidth; }
            set { base.StrokeWidth = value; }
        }

        /// <summary>
        /// Цвет штриха
        /// </summary>
        [Category("Свойства")]
        [Description("Цвет штриха")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public new Color StrokeColor
        {
            get { return base.StrokeColor; }
            set { base.StrokeColor = value; }
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

        public CustomStrokeEffect()
        {

        }

        public override string ToString()
        {
            return "Штриховка";
        }
    }
}