using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BubbleChartBrowseClass
    {
        #region Поля

        private BubbleChartAppearance bubbleChartAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Форма пузырька
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Форма пузырька")]
        [DisplayName("Форма пузырька")]
        [TypeConverter(typeof(BubbleShapeTypeConverter))]
        [DefaultValue(BubbleShape.Circle)]
        [Browsable(true)]
        public BubbleShape BubbleShape
        {
            get { return bubbleChartAppearance.BubbleShape; }
            set { bubbleChartAppearance.BubbleShape = value; }
        }

        /// <summary>
        /// Сортировка пузырей по радиусу
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Сортировка пузырей по радиусу")]
        [DisplayName("Сортировка по радиусу")]
        [TypeConverter(typeof(ChartSortTypeConverter))]
        [DefaultValue(ChartSortType.None)]
        [Browsable(true)]
        public ChartSortType SortByRadius
        {
            get { return bubbleChartAppearance.SortByRadius; }
            set { bubbleChartAppearance.SortByRadius = value; }
        }

        /// <summary>
        /// Индекс цвета пузырьков
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс цвета пузырьков")]
        [DisplayName("Индекс цвета")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColorCueColumn
        {
            get { return bubbleChartAppearance.ColorCueColumn; }
            set { bubbleChartAppearance.ColorCueColumn = value; }
        }

        /// <summary>
        /// Индекс оси X
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс оси X пузырьков")]
        [DisplayName("Индекс оси X")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int ColumnX
        {
            get { return bubbleChartAppearance.ColumnX; }
            set { bubbleChartAppearance.ColumnX = value; }
        }

        /// <summary>
        /// Индекс оси Y
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс оси Y пузырьков")]
        [DisplayName("Индекс оси Y")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int ColumnY
        {
            get { return bubbleChartAppearance.ColumnY; }
            set { bubbleChartAppearance.ColumnY = value; }
        }

        /// <summary>
        /// Индекс оси Z
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс оси Z пузырьков")]
        [DisplayName("Индекс оси Z")]
        [DefaultValue(2)]
        [Browsable(true)]
        public int ColumnZ
        {
            get { return bubbleChartAppearance.ColumnZ; }
            set { bubbleChartAppearance.ColumnZ = value; }
        }

        /// <summary>
        /// Отображение пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение пустых значений")]
        [DisplayName("Пустые значения")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [DefaultValue(NullHandling.Zero)]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return bubbleChartAppearance.NullHandling; }
            set { bubbleChartAppearance.NullHandling = value; }
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
            get { return bubbleChartAppearance.ChartText; }
        }

        #endregion

        public BubbleChartBrowseClass(BubbleChartAppearance bubbleChartAppearance)
        {
            this.bubbleChartAppearance = bubbleChartAppearance;
        }

        public override string ToString()
        {
            return BubbleShapeTypeConverter.ToString(BubbleShape) + "; " + ChartSortTypeConverter.ToString(SortByRadius);
        }
    }
}