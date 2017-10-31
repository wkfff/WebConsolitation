using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomTextureEffect : TextureEffect
    {
        /// <summary>
        /// Применить к
        /// </summary>
        [Category("Свойства")]
        [Description("Применить к")]
        [DisplayName("Применить к")]
        [Browsable(true)]
        public new EffectApplicationMode ApplyTo
        {
            get { return base.ApplyTo; }
            set { base.ApplyTo = value; }
        }

        /// <summary>
        /// Изображение
        /// </summary>
        [Category("Свойства")]
        [Description("Изображение")]
        [DisplayName("Изображение")]
        [TypeConverter(typeof(TypeConverter))]
        [Browsable(true)]
        public new Image CustomImage
        {
            get { return base.CustomImage; }
            set { base.CustomImage = value; }
        }

        /// <summary>
        /// Стиль
        /// </summary>
        [Category("Свойства")]
        [Description("Стиль")]
        [DisplayName("Стиль")]
        [Browsable(true)]
        public new TexturePresets Texture
        {
            get { return base.Texture; }
            set { base.Texture = value; }
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

        public CustomTextureEffect(IChartComponent component)
            : base(component)
        {

        }

        public override string ToString()
        {
            return "Текстура";
        }
    }
}