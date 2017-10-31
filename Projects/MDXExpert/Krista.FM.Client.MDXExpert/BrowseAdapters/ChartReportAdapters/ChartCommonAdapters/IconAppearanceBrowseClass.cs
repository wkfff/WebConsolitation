using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Граница
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class IconAppearanceBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private IconAppearance _iconAppearance;
        private PaintElementBrowseClass _paintElementBrowse;

        #endregion

        #region Свойства

        [Category("Вид иконки")]
        [Description("Символ")]
        [DisplayName("Символ")]
        [Browsable(true)]
        public char Character
        {
            get { return this._iconAppearance.Character; }
            set { this._iconAppearance.Character = value; }
        }

        [Category("Вид иконки")]
        [Description("Шрифт символа")]
        [DisplayName("Шрифт символа")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font CharacterFont
        {
            get { return this._iconAppearance.CharacterFont; }
            set { this._iconAppearance.CharacterFont = value; }
        }

        [Category("Вид иконки")]
        [Description("Иконка")]
        [DisplayName("Иконка")]
        [TypeConverter(typeof(SymbolIconTypeConverter))]
        [Browsable(true)]
        public SymbolIcon Icon
        {
            get { return this._iconAppearance.Icon; }
            set { this._iconAppearance.Icon = value; }
        }

        [Category("Вид иконки")]
        [Description("Размер иконки")]
        [DisplayName("Размер иконки")]
        [TypeConverter(typeof(SymbolIconSizeTypeConverter))]
        [Browsable(true)]
        public SymbolIconSize IconSize
        {
            get { return this._iconAppearance.IconSize; }
            set { this._iconAppearance.IconSize = value; }
        }

        [Category("Вид иконки")]
        [Description("Стиль элемента отображения")]
        [DisplayName("Стиль элемента отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PE
        {
            get { return this._paintElementBrowse; }
            set { this._paintElementBrowse = value; }
        }

        #endregion

        public IconAppearanceBrowseClass(IconAppearance iconAppearance)
        {
            this._iconAppearance = iconAppearance;
            this._paintElementBrowse = new PaintElementBrowseClass(iconAppearance.PE);
        }

        public override string ToString()
        {
            return "";
        }
    }
}