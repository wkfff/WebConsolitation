using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomStrokeEffect : FilterablePropertyBase
    {
        private StrokeEffect _strokeEffect;
        /// <summary>
        /// Прозрачность штриха
        /// </summary>
        [Category("Свойства")]
        [Description("Прозрачность штриха")]
        [DisplayName("Прозрачность")]
        [Browsable(true)]
        public byte StrokeOpacity
        {
            get { return this._strokeEffect.StrokeOpacity; }
            set { this._strokeEffect.StrokeOpacity = value; }
        }

        /// <summary>
        /// Ширина штриха
        /// </summary>
        [Category("Свойства")]
        [Description("Ширина штриха")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public int StrokeWidth
        {
            get { return this._strokeEffect.StrokeWidth; }
            set { this._strokeEffect.StrokeWidth = value; }
        }

        /// <summary>
        /// Цвет штриха
        /// </summary>
        [Category("Свойства")]
        [Description("Цвет штриха")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color StrokeColor
        {
            get { return this._strokeEffect.StrokeColor; }
            set { this._strokeEffect.StrokeColor = value; }
        }

        /// <summary>
        /// Применить
        /// </summary>
        [Category("Свойства")]
        [Description("Применить")]
        [DisplayName("Применить")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Enabled
        {
            get { return this._strokeEffect.Enabled; }
            set { this._strokeEffect.Enabled = value; }
        }

        public CustomStrokeEffect(StrokeEffect strokeEffect)
        {
            this._strokeEffect = strokeEffect;
        }

        public override string ToString()
        {
            return "Штриховка";
        }
    }
}