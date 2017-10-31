using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Надписи по краям
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TitleSideBrowseClass
    {
        #region Поля

        private TitleAppearance titleSide;
        private MarginsBrowseClass marginsBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Шрифт текста надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Шрифт текста надписи")]
        [DisplayName("Шрифт текста")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font Font
        {
            get { return titleSide.Font; }
            set { titleSide.Font = value; }
        }

        /// <summary>
        /// Цвет шрифта текста надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Цвет шрифта текста надписи")]
        [DisplayName("Цвет шрифта")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontСolor
        {
            get { return titleSide.FontColor; }
            set { titleSide.FontColor = value; }
        }

        /// <summary>
        /// Текст надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Текст надписи")]
        [DisplayName("Текст")]
        [Browsable(true)]
        public string Text
        {
            get { return titleSide.Text; }
            set { titleSide.Text = value; }
        }

        /// <summary>
        /// Ориентация текста надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Ориентация текста надписи")]
        [DisplayName("Ориентация текста")]
        [TypeConverter(typeof(TextOrientationTypeConverter))]
        [DefaultValue(TextOrientation.VerticalLeftFacing)]
        [Browsable(true)]
        public TextOrientation Orientation
        {
            get { return titleSide.Orientation; }
            set { titleSide.Orientation = value; }
        }

        /// <summary>
        /// Угол поворота метки
        /// </summary>
        [Category("Метки")]
        [Description("Угол поворота метки")]
        [DisplayName("Угол поворота")]
        //[Editor(typeof(ShadowControlEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(0)]
        [Browsable(true)]
        public int OrientationAngle
        {
            get { return titleSide.OrientationAngle; }
            set { titleSide.OrientationAngle = value; }
        }

        /// <summary>
        /// Автоопределение размера надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Автоопределение размера надписи")]
        [DisplayName("Автоопределение размера")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool BestSize
        {
            get { return titleSide.FontSizeBestFit; }
            set { titleSide.FontSizeBestFit = value; }
        }

        /// <summary>
        /// Видимость надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Показывать надписи")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Visible
        {
            get { return titleSide.Visible; }
            set { titleSide.Visible = value; }
        }

        /// <summary>
        /// Горизонтальное выравнивание надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Горизонтальное выравнивание надписи")]
        [DisplayName("Горизонтальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [DefaultValue(StringAlignment.Near)]
        [Browsable(true)]
        public StringAlignment HorizontalAlign
        {
            get { return titleSide.HorizontalAlign; }
            set { titleSide.HorizontalAlign = value; }
        }

        /// <summary>
        /// Вертикальное выравнивание надписи
        /// </summary>
        [Category("Надписи")]
        [Description("Вертикальное выравнивание надписи")]
        [DisplayName("Вертикальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarVerticalConverter))]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalAlign
        {
            get { return titleSide.VerticalAlign; }
            set { titleSide.VerticalAlign = value; }
        }

        /// <summary>
        /// Обрезка текста
        /// </summary>
        [Category("Надписи")]
        [Description("Обрезка текста")]
        [DisplayName("Обрезка текста")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ClipText
        {
            get { return titleSide.ClipText; }
            set { titleSide.ClipText = value; }
        }

        /// <summary>
        /// Перенос текста
        /// </summary>
        [Category("Надписи")]
        [Description("Перенос текста")]
        [DisplayName("Перенос текста")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool WrapText
        {
            get { return titleSide.WrapText; }
            set { titleSide.WrapText = value; }
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
            get { return titleSide.Flip; }
            set { titleSide.Flip = value; }
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
            get { return titleSide.ReverseText; }
            set { titleSide.ReverseText = value; }
        }

        /// <summary>
        /// Размер
        /// </summary>
        [Category("Подписи")]
        [Description("Размер")]
        [DisplayName("Размер")]
        [DefaultValue(26)]
        [Browsable(true)]
        public int Extent
        {
            get { return titleSide.Extent; }
            set { titleSide.Extent = value; }
        }

        /// <summary>
        /// Поля
        /// </summary>
        [Category("Подписи")]
        [Description("Поля")]
        [DisplayName("Поля")]
        [Browsable(true)]
        public MarginsBrowseClass MarginsBrowse
        {
            get { return marginsBrowse; }
            set { marginsBrowse = value; }
        }

        #endregion

        public TitleSideBrowseClass(TitleAppearance titleSide)
        {
            this.titleSide = titleSide;

            marginsBrowse = new MarginsBrowseClass(titleSide.Margins);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}