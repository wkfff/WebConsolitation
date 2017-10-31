using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PolarChartBrowseClass
    {
        #region Поля

        private PolarChartAppearance polarChartAppearance;
        private SplineApperanceBrowseClass polarLineBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Единица измерения углов
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Единица измерения углов")]
        [DisplayName("Единица измерения углов")]
        [TypeConverter(typeof(AngleUnitTypeConverter))]
        [Browsable(true)]
        public AngleUnit AngleUnit
        {
            get { return polarChartAppearance.AngleUnit; }
            set { polarChartAppearance.AngleUnit = value; }
        }

        /// <summary>
        /// Символ метки
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Символ метки")]
        [DisplayName("Символ метки")]
        [Browsable(true)]
        public Char Character
        {
            get { return polarChartAppearance.Character; }
            set { polarChartAppearance.Character = value; }
        }

        /// <summary>
        /// Шрифт символа
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Шрифт символа")]
        [DisplayName("Шрифт символа")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font CharacterFont
        {
            get { return polarChartAppearance.CharacterFont; }
            set { polarChartAppearance.CharacterFont = value; }
        }

        /// <summary>
        /// Индекс оси X
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс оси X")]
        [DisplayName("Индекс оси X")]
        [Browsable(true)]
        public int ColumnX
        {
            get { return polarChartAppearance.ColumnX; }
            set { polarChartAppearance.ColumnX = value; }
        }

        /// <summary>
        /// Индекс оси Y
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс оси Y")]
        [DisplayName("Индекс оси Y")]
        [Browsable(true)]
        public int ColumnY
        {
            get { return polarChartAppearance.ColumnY; }
            set { polarChartAppearance.ColumnY = value; }
        }

        /// <summary>
        /// Соединение меток линией
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Соединение меток линией")]
        [DisplayName("Соединение меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ConnectWithLines
        {
            get { return polarChartAppearance.ConnectWithLines; }
            set { polarChartAppearance.ConnectWithLines = value; }
        }

        /// <summary>
        /// Заливка области
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Заливка области")]
        [DisplayName("Заливка области")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool FillArea
        {
            get { return polarChartAppearance.FillArea; }
            set { polarChartAppearance.FillArea = value; }
        }

        /// <summary>
        /// Обращение меток
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Обращение меток")]
        [DisplayName("Обращение меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool EnableLabelFlipping
        {
            get { return polarChartAppearance.EnableLabelFlipping; }
            set { polarChartAppearance.EnableLabelFlipping = value; }
        }

        /// <summary>
        /// Начальный угол обращения меток
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Начальный угол обращения меток")]
        [DisplayName("Начальный угол обращения меток")]
        [Browsable(true)]
        public int LabelFlippingStartAngle
        {
            get { return polarChartAppearance.LabelFlippingStartAngle; }
            set { polarChartAppearance.LabelFlippingStartAngle = value; }
        }

        /// <summary>
        /// Конечный угол обращения меток
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Конечный угол обращения меток")]
        [DisplayName("Конечный угол обращения меток")]
        [Browsable(true)]
        public int LabelFlippingEndAngle
        {
            get { return polarChartAppearance.LabelFlippingEndAngle; }
            set { polarChartAppearance.LabelFlippingEndAngle = value; }
        }

        /// <summary>
        /// Вид значка
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Вид значка")]
        [DisplayName("Значок")]
        [TypeConverter(typeof(SymbolIconTypeConverter))]
        [Browsable(true)]
        public SymbolIcon Icon
        {
            get { return polarChartAppearance.Icon; }
            set { polarChartAppearance.Icon = value; }
        }

        /// <summary>
        /// Размер значка
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Размер значка")]
        [DisplayName("Размер значка")]
        [TypeConverter(typeof(SymbolIconSizeTypeConverter))]
        [Browsable(true)]
        public SymbolIconSize IconSize
        {
            get { return polarChartAppearance.IconSize; }
            set { polarChartAppearance.IconSize = value; }
        }

        /// <summary>
        /// Индекс группировки по колонкам
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс группировки по колонкам")]
        [DisplayName("Индекс группировки по колонкам")]
        [Browsable(true)]
        public int GroupByColumn
        {
            get { return polarChartAppearance.GroupByColumn; }
            set { polarChartAppearance.GroupByColumn = value; }
        }

        /// <summary>
        /// Группировка по колонкам
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Группировка по колонкам")]
        [DisplayName("Группировка по колонкам")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool UseGroupByColumn
        {
            get { return polarChartAppearance.UseGroupByColumn; }
            set { polarChartAppearance.UseGroupByColumn = value; }
        }

        /// <summary>
        /// Свободное установление осей
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Свободное установление осей")]
        [DisplayName("Свободное установление осей")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool FreeStandingAxis
        {
            get { return polarChartAppearance.FreeStandingAxis; }
            set { polarChartAppearance.FreeStandingAxis = value; }
        }

        /// <summary>
        /// Стиль линии в диаграмме
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль линии в диаграмме")]
        [DisplayName("Линия")]
        [Browsable(true)]
        public SplineApperanceBrowseClass PolarLineBrowse
        {
            get { return polarLineBrowse; }
            set { polarLineBrowse = value; }
        }

        /// <summary>
        /// Отображение пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение пустых значений")]
        [DisplayName("Пустые значения")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return polarChartAppearance.NullHandling; }
            set { polarChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// Подписи данных
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return polarChartAppearance.ChartText; }
        }

        #endregion

        public PolarChartBrowseClass(PolarChartAppearance polarChartAppearance)
        {
            this.polarChartAppearance = polarChartAppearance;
            polarLineBrowse = new SplineApperanceBrowseClass(polarChartAppearance.LineAppearance);
        }

        public override string ToString()
        {
            return SymbolIconTypeConverter.ToString(Icon) + "; " + AngleUnitTypeConverter.ToString(AngleUnit) + "; " + LineDrawStyleTypeConverter.ToString(polarLineBrowse.DrawStyle);
        }
    }
}