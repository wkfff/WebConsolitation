using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LabelStyleBrowseClass
    {
        #region Поля

        private LabelStyle labelStyle;

        #endregion

        #region Свойства

        /// <summary>
        /// Обрезание текста
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Обрезание текста")]
        [DisplayName("Обрезание текста")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ClipText
        {
            get { return labelStyle.ClipText; }
            set { labelStyle.ClipText = value; }
        }

        /// <summary>
        /// Отражение текста
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отражение текста")]
        [DisplayName("Отражение текста")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Flip
        {
            get { return labelStyle.Flip; }
            set { labelStyle.Flip = value; }
        }

        /// <summary>
        /// Смещение текста по оси X
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Смещение текста по оси X")]
        [DisplayName("Смещение по оси X")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Dx
        {
            get { return labelStyle.Dx; }
            set { labelStyle.Dx = value; }
        }

        /// <summary>
        /// Смещение текста по оси Y
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Смещение текста по оси Y")]
        [DisplayName("Смещение по оси Y")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Dy
        {
            get { return labelStyle.Dy; }
            set { labelStyle.Dy = value; }
        }

        /// <summary>
        /// Шрифт текста подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Шрифт текста подписи")]
        [DisplayName("Шрифт текста")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font Font
        {
            get { return labelStyle.Font; }
            set { labelStyle.Font = value; }
        }

        /// <summary>
        /// Цвет шрифта текта подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Цвет шрифта текта подписи")]
        [DisplayName("Цвет шрифта")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return labelStyle.FontColor; }
            set { labelStyle.FontColor = value; }
        }

        /// <summary>
        /// Горизонтальное выравнивание
        /// </summary>
        [Category("Подписи")]
        [Description("Горизонтальное выравнивание")]
        [DisplayName("Горизонтальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [DefaultValue(StringAlignment.Near)]
        [Browsable(true)]
        public StringAlignment HorizontalAlign
        {
            get { return labelStyle.HorizontalAlign; }
            set { labelStyle.HorizontalAlign = value; }
        }

        /// <summary>
        /// Вертикальное выравнивание
        /// </summary>
        [Category("Подписи")]
        [Description("Вертикальное выравнивание")]
        [DisplayName("Вертикальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarVerticalConverter))]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalAlign
        {
            get { return labelStyle.VerticalAlign; }
            set { labelStyle.VerticalAlign = value; }
        }

        /// <summary>
        /// Ориентация
        /// </summary>
        [Category("Подписи")]
        [Description("Ориентация")]
        [DisplayName("Ориентация")]
        [TypeConverter(typeof(TextOrientationTypeConverter))]
        [DefaultValue(TextOrientation.Horizontal)]
        [Browsable(true)]
        public TextOrientation Orientation
        {
            get { return labelStyle.Orientation; }
            set { labelStyle.Orientation = value; }
        }

        /// <summary>
        /// Обращенный текст
        /// </summary>
        [Category("Подписи")]
        [Description("Обращенный текст")]
        [DisplayName("Обращенный текст")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ReverseText
        {
            get { return labelStyle.ReverseText; }
            set { labelStyle.ReverseText = value; }
        }

        /// <summary>
        /// Угол поворота
        /// </summary>
        [Category("Подписи")]
        [Description("Угол поворота")]
        [DisplayName("Угол поворота")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int RotationAngle
        {
            get { return labelStyle.RotationAngle; }
            set { labelStyle.RotationAngle = value; }
        }

        /// <summary>
        /// Перенос
        /// </summary>
        [Category("Подписи")]
        [Description("Перенос")]
        [DisplayName("Перенос")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool WrapText
        {
            get { return labelStyle.WrapText; }
            set { labelStyle.WrapText = value; }
        }

        /// <summary>
        /// Автовыравнивание
        /// </summary>
        [Category("Подписи")]
        [Description("Автовыравнивание")]
        [DisplayName("Автовыравнивание")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool FontSizeBestFit
        {
            get { return labelStyle.FontSizeBestFit; }
            set { labelStyle.FontSizeBestFit = value; }
        }

        /// <summary>
        /// Подрезка
        /// </summary>
        [Category("Подписи")]
        [Description("Подрезка")]
        [DisplayName("Подрезка")]
        [DefaultValue(StringTrimming.None)]
        [Browsable(true)]
        public StringTrimming Trimming
        {
            get { return labelStyle.Trimming; }
            set { labelStyle.Trimming = value; }
        }

        #endregion

        public LabelStyleBrowseClass(LabelStyle labelStyle)
        {
            this.labelStyle = labelStyle;
        }

        public override string ToString()
        {
            return Font.Name + "; " + TextOrientationTypeConverter.ToString(Orientation) + "; " + Dx + "; " + Dy;
        }
    }
}