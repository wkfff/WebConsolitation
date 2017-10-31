using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomShadowEffect : FilterablePropertyBase
    {
        private ShadowEffect _shadowEffect;
        /// <summary>
        /// Угол
        /// </summary>
        [Category("Свойства")]
        [Description("Угол падения тени")]
        [DisplayName("Угол")]
        [Browsable(true)]
        public double Angle
        {
            get { return this._shadowEffect.Angle; }
            set { this._shadowEffect.Angle = value; }
        }

        /// <summary>
        /// Глубина
        /// </summary>
        [Category("Свойства")]
        [Description("Глубина тени")]
        [DisplayName("Глубина")]
        [Browsable(true)]
        public int Depth
        {
            get { return this._shadowEffect.Depth; }
            set { this._shadowEffect.Depth = value; }
        }

        /// <summary>
        /// Цвет
        /// </summary>
        [Category("Свойства")]
        [Description("Цвет тени")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return this._shadowEffect.Color; }
            set { this._shadowEffect.Color = value; }
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
            get { return this._shadowEffect.Enabled; }
            set { this._shadowEffect.Enabled = value; }
        }

        public CustomShadowEffect(ShadowEffect shadowEffect)
        {
            this._shadowEffect = shadowEffect;
        }

        public override string ToString()
        {
            return "Тень";
        }
    }
}