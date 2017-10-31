using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomShadowEffect : ShadowEffect
    {
        /// <summary>
        /// Угол
        /// </summary>
        [Category("Свойства")]
        [Description("Угол падения тени")]
        [DisplayName("Угол")]
        [Browsable(true)]
        public new double Angle
        {
            get { return base.Angle; }
            set { base.Angle = value; }
        }

        /// <summary>
        /// Глубина
        /// </summary>
        [Category("Свойства")]
        [Description("Глубина тени")]
        [DisplayName("Глубина")]
        [Browsable(true)]
        public new int Depth
        {
            get { return base.Depth; }
            set { base.Depth = value; }
        }

        /// <summary>
        /// Цвет
        /// </summary>
        [Category("Свойства")]
        [Description("Цвет тени")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public new Color Color
        {
            get { return base.Color; }
            set { base.Color = value; }
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

        public CustomShadowEffect(IChartComponent component)
            : base(component)
        {

        }

        public override string ToString()
        {
            return "Тень";
        }
    }
}