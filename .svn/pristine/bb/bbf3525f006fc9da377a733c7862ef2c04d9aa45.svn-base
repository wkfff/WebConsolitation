using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomTextureEffect : FilterablePropertyBase
    {
        private TextureEffect _textureEffect;
        /// <summary>
        /// Применить к
        /// </summary>
        [Category("Свойства")]
        [Description("Применить к")]
        [DisplayName("Применить к")]
        [TypeConverter(typeof(EffectApplicationModeConverter))]
        [Browsable(true)]
        public EffectApplicationMode ApplyTo
        {
            get { return this._textureEffect.ApplyTo; }
            set { this._textureEffect.ApplyTo = value; }
        }

        /// <summary>
        /// Изображение
        /// </summary>
        [Category("Свойства")]
        [Description("Изображение")]
        [DisplayName("Изображение")]
        [TypeConverter(typeof(TypeConverter))]
        [Browsable(true)]
        public Image CustomImage
        {
            get { return this._textureEffect.CustomImage; }
            set { this._textureEffect.CustomImage = value; }
        }

        /// <summary>
        /// Стиль
        /// </summary>
        [Category("Свойства")]
        [Description("Стиль")]
        [DisplayName("Стиль")]
        [Browsable(true)]
        public TexturePresets Texture
        {
            get { return this._textureEffect.Texture; }
            set { this._textureEffect.Texture = value; }
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
            get { return this._textureEffect.Enabled; }
            set { this._textureEffect.Enabled = value; }
        }

        public CustomTextureEffect(TextureEffect textureEffect)
        {
            this._textureEffect = textureEffect;
        }

        public override string ToString()
        {
            return "Текстура";
        }
    }
}