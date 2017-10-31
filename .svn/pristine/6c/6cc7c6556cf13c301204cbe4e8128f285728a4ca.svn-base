using System.ComponentModel;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TreeMapChartBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private TreeMapChartAppearance treeMapChartAppearance;
        private LabelStyleBrowseClass labelStyleBrowse;
        private PaintElementBrowseClass paintElementBrowse;
        private UltraChart chart;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип диаграммы
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
        }

        /// <summary>
        /// Цвет границы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Цвет границы")]
        [DisplayName("Цвет границы")]
        [DefaultValue(typeof(Color), "Gray")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return treeMapChartAppearance.BorderColor; }
            set { treeMapChartAppearance.BorderColor = value; }
        }

        /// <summary>
        /// Ширина границы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Ширина границы")]
        [DisplayName("Ширина границы")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int BorderWidth
        {
            get { return treeMapChartAppearance.BorderWidth; }
            set { treeMapChartAppearance.BorderWidth = value; }
        }

        /// <summary>
        /// Заголовок корневого элемента
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Заголовок корневого элемента")]
        [DisplayName("Заголовок корневого элемента")]
        [DefaultValue("")]
        [Browsable(true)]
        public string ChartTitle
        {
            get { return treeMapChartAppearance.ChartTitle; }
            set { treeMapChartAppearance.ChartTitle = value; }
        }

        /// <summary>
        /// Индекс цвета
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс цвета")]
        [DisplayName("Индекс цвета")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColorValueIndex
        {
            get { return treeMapChartAppearance.ColorValueIndex; }
            set
            {
                treeMapChartAppearance.ColorValueIndex = ChartReportElement.CheckColumnIndexValue((DataTable)chart.DataSource, value);
                treeMapChartAppearance.ColorValueLabel = ChartReportElement.GetColumnName((DataTable)chart.DataSource, treeMapChartAppearance.ColorValueIndex);
            }
        }

        /// <summary>
        /// Индекс размера
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индекс размера")]
        [DisplayName("Индекс размера")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int SizeValueIndex
        {
            get { return treeMapChartAppearance.SizeValueIndex; }
            set
            {
                treeMapChartAppearance.SizeValueIndex = ChartReportElement.CheckColumnIndexValue((DataTable)chart.DataSource, value);
                treeMapChartAppearance.SizeValueLabel = ChartReportElement.GetColumnName((DataTable)chart.DataSource, treeMapChartAppearance.SizeValueIndex);
            }
        }

        /// <summary>
        /// Метка цвета
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Метка цвета")]
        [DisplayName("Метка цвета")]
        [DefaultValue("COLOR_LABEL")]
        [Browsable(true)]
        public string ColorValueLabel
        {
            get { return treeMapChartAppearance.ColorValueLabel; }
            set { treeMapChartAppearance.ColorValueLabel = value; }
        }

        /// <summary>
        /// Метка размера
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Метка размера")]
        [DisplayName("Метка размера")]
        [DefaultValue("SIZE_LABEL")]
        [Browsable(true)]
        public string SizeValueLabel
        {
            get { return treeMapChartAppearance.SizeValueLabel; }
            set { treeMapChartAppearance.SizeValueLabel = value; }
        }

        /// <summary>
        /// Игнорировать значения цвета
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Игнорировать значения цвета")]
        [DisplayName("Игнорировать значения цвета")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool DisableColorValues
        {
            get { return treeMapChartAppearance.DisableColorValues; }
            set { treeMapChartAppearance.DisableColorValues = value; }
        }

        /// <summary>
        /// Индексы порядка
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Индексы порядка")]
        [DisplayName("Индексы порядка")]
        [Browsable(true)]
        public int[] IndexOrder
        {
            get { return treeMapChartAppearance.IndexOrder; }
            set { treeMapChartAppearance.IndexOrder = value; }
        }

        /// <summary>
        /// Отступ
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отступ")]
        [DisplayName("Отступ")]
        [DefaultValue(6)]
        [Browsable(true)]
        public int Margin
        {
            get { return treeMapChartAppearance.Margin; }
            set { treeMapChartAppearance.Margin = value; }
        }

        /// <summary>
        ///  Угол поворота
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Угол поворота")]
        [DisplayName("Угол поворота")]
        [DefaultValue(typeof(double), "45")]
        [DynamicPropertyFilter("TreeMapType", "Circular")]
        [Browsable(true)]
        public double Rotation
        {
            get { return treeMapChartAppearance.Rotation; }
            set { treeMapChartAppearance.Rotation = value; }
        }

        /// <summary>
        ///  Тип
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Тип")]
        [DisplayName("Тип")]
        [DefaultValue(typeof(TreeMapType), "Rectangular")]
        [TypeConverter(typeof(TreeMapTypeConverter))]
        [Browsable(true)]
        public TreeMapType TreeMapType
        {
            get { return treeMapChartAppearance.TreeMapType; }
            set { treeMapChartAppearance.TreeMapType = value; }
        }

        /// <summary>
        ///  Показывать ось цвета в легенде
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать ось цвета в легенде")]
        [DisplayName("Показывать ось цвета в легенде")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowColorIntervalAxis
        {
            get { return treeMapChartAppearance.ShowColorIntervalAxis; }
            set { treeMapChartAppearance.ShowColorIntervalAxis = value; }
        }

        /// <summary>
        ///  Показывать метки
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать метки")]
        [DisplayName("Показывать метки")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowLabels
        {
            get { return treeMapChartAppearance.ShowLabels; }
            set { treeMapChartAppearance.ShowLabels = value; }
        }

        /// <summary>
        ///  Показывать заголовки для точек без меток
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать заголовки для точек без меток")]
        [DisplayName("Показывать заголовки для точек без меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowHeadersForUnlabeledPoints
        {
            get { return treeMapChartAppearance.ShowHeadersForUnlabeledPoints; }
            set { treeMapChartAppearance.ShowHeadersForUnlabeledPoints = value; }
        }

        /// <summary>
        ///  Стиль меток
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль меток")]
        [DisplayName("Стиль меток")]
        [Browsable(true)]
        public LabelStyleBrowseClass LabelStyleBrowse
        {
            get { return labelStyleBrowse; }
            set { labelStyleBrowse  = value; }
        }

        /// <summary>
        ///  Дополнительный элемент отображения
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Дополнительный элемент отображения")]
        [DisplayName("Дополнительный элемент отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        /// <summary>
        ///  Использовать дополнительный элемент отображения
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Использовать дополнительный элемент отображения")]
        [DisplayName("Использовать дополнительный элемент отображения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool UseStaticLeafPE
        {
            get { return treeMapChartAppearance.UseStaticLeafPE; }
            set { treeMapChartAppearance.UseStaticLeafPE = value; }
        }

        /// <summary>
        ///  Коллекция элементов отображения
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Коллекция элементов отображения")]
        [DisplayName("Коллекция элементов отображения")]
        [Browsable(true)]
        public PaintElementCollection PEs
        {
            get { return treeMapChartAppearance.PEs; }
        }

        #endregion

        public TreeMapChartBrowseClass(TreeMapChartAppearance treeMapChartAppearance, UltraChart chart)
        {
            this.treeMapChartAppearance = treeMapChartAppearance;
            this.chart = chart;

            labelStyleBrowse = new LabelStyleBrowseClass(treeMapChartAppearance.LabelStyle);
            paintElementBrowse = new PaintElementBrowseClass(treeMapChartAppearance.StaticLeafPE);
        }

        public override string ToString()
        {
            return TreeMapTypeConverter.ToString(TreeMapType) + "; " + BooleanTypeConverter.ToString(ShowLabels);
        }
    }
}