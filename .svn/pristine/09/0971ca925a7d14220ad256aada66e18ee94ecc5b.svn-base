using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.Win.UltraWinChart;
using Infragistics.Win.UltraWinDock;
using Infragistics.UltraChart.Shared.Styles;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    class ChartReportElementBrowseAdapter : CustomReportElementBrowseAdapter, IChartComponent
    {
        #region Поля

        private UltraChart chart;
        private ChartReportElement activeReportElement;
        
        private TooltipsBrowseClass tooltipsBrowse;
        private TitleBrowseClass titleBrowse;
        private ColorModelBrowseClass colorModelBrowse;
        private AxiesBrowseClass axiesBrowse;
        private BorderBrowseClass borderBrowse;
        private LegendBrowseClass legendBrowse;
        private CompositeLegendBrowseClass compositeLegendBrowse;
        private Transform3DBrowseClass transform3DBrowse;
        private AnnotationBrowseClass annotationBrowse;
        private DataApperanceBrowseClass dataApperanceBrowse;
        private EffectsBrowseClass effectsBrowse;

        private AreaChartBrowseClass areaChartBrowse;
        private BarChartBrowseClass barChartBrowse;
        private BarChart3DBrowseClass barChart3DBrowse;
        private BoxChartBrowseClass boxChartBrowse;
        private BubbleChartBrowseClass bubbleChartBrowse;
        private ColumnChartBrowseClass columnChartBrowse;
        private ColumnChart3DBrowseClass columnChart3DBrowse;
        private DoughnutChartBrowseClass doughnutChartBrowse;
        private LineChartBrowseClass lineChartBrowse;
        private ColumnLineChartBrowseClass columnLineChartBrowse;
        private HeatMapChartBrowseClass heatMapChartBrowse;
        private HistogramChartBrowseClass histogramChartBrowse;
        private ParetoChartBrowseClass paretoChartBrowse;
        private PieChartBrowseClass pieChartBrowse;
        private PolarChartBrowseClass polarChartBrowse;
        private ProbabilityChartBrowseClass probabilityChartBrowse;
        private RadarChartBrowseClass radarChartBrowse;
        private ScatterChartBrowseClass scatterChartBrowse;
        private ScatterLineChartBrowseClass scatterLineChartBrowse;
        private SplineChartBrowseClass splineChartBrowse;
        private SplineAreaChartBrowseClass splineAreaChartBrowse;
        private ConeChart3DBrowseClass coneChart3DBrowse;
        private FunnelChart3DBrowseClass funnelChart3DBrowse;
        private PointChart3DBrowseClass pointChart3DBrowse;
        private PyramidChart3DBrowseClass pyramidChart3DBrowse;
        private SplineAreaChart3DBrowseClass splineAreaChart3DBrowse;
        private SplineChart3DBrowseClass splineChart3DBrowse;
        private FunnelChartBrowseClass funnelChartBrowse;
        private PyramidChartBrowseClass pyramidChartBrowse;
        private StackAreaChartBrowseClass stackAreaChartBrowse;
        private StackBarChartBrowseClass stackBarChartBrowse;
        private Stack3DBarChartBrowseClass stack3DBarChartBrowse;
        private StackColumnChartBrowseClass stackColumnChartBrowse;
        private Stack3DColumnChartBrowseClass stack3DColumnChartBrowse;
        private StackLineChartBrowseClass stackLineChartBrowse;
        private StackSplineChartBrowseClass stackSplineChartBrowse;
        private StackSplineAreaChartBrowseClass stackSplineAreaChartBrowse;
        private CandleChartBrowseClass candleChartBrowse;
        private GanttChartBrowseClass ganttChartBrowse;
        private CompositeChartBrowseClass compositeChartBrowse;
        private TreeMapChartBrowseClass treeMapChartBrowse;
        private ChartSize chartSize;

        private OverrideAppearance @override = null;
        private string[] userLayerIndex = null;
        private ISite site = null;
        private bool dataBindMe;
        private RenderingType renderingType = 0;
        private SmoothingMode smoothingMode;
        private bool isSavingPreset = false;
        private TextRenderingHint textRenderingHint;

        #endregion

        #region Свойства

        /// <summary>
        /// Композитная ли диаграмма
        /// </summary>
        [Browsable(false)]
        public bool IsComposite
        {
            get { return chart.ChartType == ChartType.Composite; }
        }

        [Browsable(false)]
        private MainForm MainForm
        {
            get { return activeReportElement.MainForm; }
        }

        /// <summary>
        /// Тип диаграммы
        /// </summary>
        [Category("Вид")]
        [Description("Выбор подходящего типа диаграммы для отображения данных")]
        [DisplayName("Тип диаграммы")]
        [RefreshProperties(RefreshProperties.All)]
        [Editor(typeof(ChartTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(ChartTypeConverter))]
        [DynamicPropertyFilter("IsComposite", "False")]
        [DefaultValue(ChartType.ColumnChart)]
        [Browsable(true)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
            set
            {
                if ((value == ChartType.HeatMapChart) &&
                        !InfragisticsUtils.CheckChangeToHeatmapChart(chart, value))
                {
                    CommonUtils.ProcessException(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."), true);
                    //FormException.ShowErrorForm(new Exception("Index was out of range. Must be non-negative and less than the size of the collection."));
                    return;
                }
                
                // если диаграмма является дочерней и новый тип несовместим
                if (MainForm.IsCompositeChildChart(activeReportElement.UniqueName) &&
                    !CompositeChartUtils.IsCompositeCompatibleType(value))
                {
                    string elementText = MainForm.GetReportElementText(activeReportElement.UniqueName);
                    string curChartTypeText = ChartTypeConverter.GetLocalizedChartType(chart.ChartType);
                    string newChartTypeText = ChartTypeConverter.GetLocalizedChartType(value);
                    string msg = string.Format("Диаграмма \"{0}\" используется в качестве слоя композитной диаграммы. " + 
                        "Тип \"{1}\" является несовместимым для композитной диаграммы. Удалить слой \"{0}({2})\" из композитной диаграммы?",
                        elementText, newChartTypeText,  curChartTypeText);
                    if (System.Windows.Forms.MessageBox.Show(msg, "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        // выводим подтверждение
                        msg = string.Format("Вы действительно хотите удалить слой \"{0}({1})\" из композитной диаграммы?",
                            elementText, curChartTypeText);
                        if (System.Windows.Forms.MessageBox.Show(msg, "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                        else
                        {
                            // удаляем все слои, куда входила дочерняя
                            MainForm.RemoveCompositeChildKey(activeReportElement.UniqueName);
                        }
                    }
                }

                Common.InfragisticsUtils.ChangeChartType(chart, value);
                ChartFormatBrowseClass.DoFormatStringChange();
            }
        }
        /*
        /// <summary>
        /// Тип диаграммы для композитной диаграммы
        /// </summary>
        [Category("Вид")]
        [Description("Диаграмма")]
        [DisplayName("Диаграмма")]
        [Browsable(true)]
        public UltraChart Chart
        {
            // делаем только для чтения
            get { return this.chart; }
        }
        */
        /// <summary>
        /// Тип диаграммы для композитной диаграммы
        /// </summary>
        [Category("Вид")]
        [Description("Тип диаграммы")]
        [DisplayName("Тип диаграммы")]
        [Editor(typeof(ChartTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(ChartTypeConverter))]
        [DynamicPropertyFilter("IsComposite", "True")]
        [DefaultValue(ChartType.Composite)]
        [Browsable(true)]
        public ChartType CompositeChartType
        {
            // делаем только для чтения
            get { return chart.ChartType; }
        }

        /// <summary>
        /// Цвет фона
        /// </summary>
        [Category("Вид")]
        [Description("Цвет фона")]
        [DisplayName("Цвет фона")]
        [DefaultValue(typeof(Color), "White")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return chart.BackColor; }
            set { chart.BackColor = value; }
        }

        /// <summary>
        /// Фоновое изображение
        /// </summary>
        [Category("Вид")]
        [Description("Фоновое изображение")]
        [DisplayName("Фоновое изображение")]
        [Editor(typeof(ImageSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(TypeConverter))]
        [Browsable(true)]
        public Image BackgroundImage
        {
            get { return chart.BackgroundImage; }
            set { chart.BackgroundImage = value; }
        }

        /// <summary>
        /// Стиль фонового изображения
        /// </summary>
        [Category("Вид")]
        [Description("Стиль фонового изображения")]
        [DisplayName("Стиль фонового изображения")]
        [TypeConverter(typeof(ImageFitStyleTypeConverter))]
        [DefaultValue(ImageFitStyle.StretchedFit)]
        [Browsable(true)]
        public ImageFitStyle BackgroundImageStyle
        {
            get { return chart.BackgroundImageStyle; }
            set { chart.BackgroundImageStyle = value; }
        }

        /// <summary>
        /// Фоновое изображение для полосы прокрутки
        /// </summary>
        [Category("Вид")]
        [Description("Фоновое изображение для полосы прокрутки осей диаграммы 2D")]
        [DisplayName("Фоновое изображение для полосы прокрутки")]
        [DynamicPropertyFilter("ChartType",
            "ColumnChart, BarChart, AreaChart, LineChart, BubbleChart," +
            "ScatterChart, HeatMapChart, CandleChart, StackBarChart, StackColumnChart," +
            "RadarChart, SplineChart, SplineAreaChart, ColumnLineChart," +
            "ScatterLineChart, StepLineChart, StepAreaChart, GanttChart, PolarChart, ParetoChart," +
            "StackAreaChart, StackLineChart, StackSplineChart, StackSplineAreaChart, BoxChart," +
            "ProbabilityChart, HistogramChart")]
        [Editor(typeof(ImageSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(TypeConverter))]
        [Browsable(true)]
        public Bitmap ScrollBarImage
        {
            get { return chart.ScrollBarImage; }
            set { chart.ScrollBarImage = value; }
        }

        /// <summary>
        /// Подсказки
        /// </summary>
        [Category("Вид")]
        [Description("Подсказки")]
        [DisplayName("Подсказки")]
        [Browsable(true)]
        public TooltipsBrowseClass TooltipsBrowse
        {
            get { return tooltipsBrowse; }
            set { tooltipsBrowse = value; }
        }

        /// <summary>
        /// Надписи
        /// </summary>
        [Category("Вид")]
        [Description("Надписи")]
        [DisplayName("Надписи")]
        [Browsable(true)]
        public TitleBrowseClass TitleBrowse
        {
            get { return titleBrowse; }
            set { titleBrowse = value; }
        }

        /// <summary>
        /// Цветовая схема
        /// </summary>
        [Category("Вид")]
        [Description("Цветовая схема")]
        [DisplayName("Цветовая схема")]
        [Browsable(true)]
        public ColorModelBrowseClass ColorModelBrowse
        {
            get { return colorModelBrowse; }
            set { colorModelBrowse = value; }
        }

        /// <summary>
        /// Оси координат
        /// </summary>
        [Category("Вид")]
        [Description("Оси координат")]
        [DisplayName("Оси координат")]
        [DynamicPropertyFilter("ChartType", "AreaChart3D, BarChart3D, BubbleChart3D, "
            + "ColumnChart3D, CylinderBarChart3D, CylinderColumnChart3D, "
            + "CylinderStackBarChart3D, CylinderStackColumnChart3D, "
            + "HeatMapChart3D, LineChart3D, PointChart3D, "
            + "SplineAreaChart3D, SplineChart3D, Stack3DBarChart, Stack3DColumnChart, "
            + "AreaChart, BarChart, BoxChart, BubbleChart, CandleChart, "
            + "ColumnChart, ColumnLineChart, HeatMapChart, HistogramChart, GanttChart, "
            + "LineChart, ScatterChart, ScatterLineChart, StackAreaChart, StackBarChart, "
            + "StackColumnChart, StackLineChart, StackSplineAreaChart, StackSplineChart, StepAreaChart, "
            + "StepLineChart, SplineAreaChart, SplineChart, ParetoChart, ProbabilityChart, PolarChart, RadarChart")]
        [Browsable(true)]
        public AxiesBrowseClass AxiesBrowse
        {
            get { return axiesBrowse; }
            set { axiesBrowse = value; }
        }

        /// <summary>
        /// Граница
        /// </summary>
        [Category("Вид")]
        [Description("Граница")]
        [DisplayName("Граница")]
        [Browsable(true)]
        public BorderBrowseClass BorderBrowse
        {
            get { return borderBrowse; }
            set { borderBrowse = value; }
        }

        /// <summary>
        /// Легенда
        /// </summary>
        [Category("Вид")]
        [Description("Легенда")]
        [DisplayName("Легенда")]
        [DynamicPropertyFilter("IsComposite", "False")]
        [Browsable(true)]
        public LegendBrowseClass LegendBrowse
        {
            get { return legendBrowse; }
            set { legendBrowse = value; }
        }

        /// <summary>
        /// Легенда
        /// </summary>
        [Category("Вид")]
        [Description("Легенда")]
        [DisplayName("Легенда")]
        [DynamicPropertyFilter("IsComposite", "True")]
        [Browsable(true)]
        public CompositeLegendBrowseClass CompositeLegendBrowse
        {
            get { return compositeLegendBrowse; }
            set { compositeLegendBrowse = value; }
        }

        /// <summary>
        /// Настройка 3D вида
        /// </summary> 
        [Category("Вид")]
        [Description("Настройка вида для 3D диаграмм")]
        [DisplayName("Настройка 3D вида")]
        [DynamicPropertyFilter("ChartType", "AreaChart3D, BarChart3D, BubbleChart3D, "
            + "ColumnChart3D, ConeChart3D, CylinderBarChart3D, CylinderColumnChart3D, "
            + "CylinderStackBarChart3D, CylinderStackColumnChart3D, DoughnutChart3D, "
            + "FunnelChart3D, HeatMapChart3D, LineChart3D, PieChart3D, PointChart3D, "
            + "PyramidChart3D,SplineAreaChart3D, SplineChart3D, Stack3DBarChart, Stack3DColumnChart")]
        [Editor(typeof(View3DEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(Transform3DTypeConverter))]
        [Browsable(true)]
        public View3DAppearance Transform3D
        {
            get { return chart.Transform3D; }
            set { chart.Transform3D = value; }
        }
        /*public Transform3DBrowseClass Transform3DBrowse
        {
            get { return transform3DBrowse; }
            set { transform3DBrowse = value; }
        }*/

        /// <summary>
        /// Отображение данных
        /// </summary>
        [Category("Вид")]
        [Description("Отображение данных")]
        [DisplayName("Отображение данных")]
        [DynamicPropertyFilter("IsComposite", "False")]
        [Browsable(true)]
        public DataApperanceBrowseClass DataApperanceBrowse
        {
            get { return dataApperanceBrowse; }
            set { dataApperanceBrowse = value; }
        }

        /// <summary>
        /// Прицел для позиционирования
        /// </summary>
        [Category("Вид")]
        [Description("Прицел для позиционирования")]
        [DisplayName("Прицел")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("ChartType", "ColumnChart, BarChart, AreaChart, LineChart, BubbleChart, "
            + "ScatterChart, HeatMapChart, StackBarChart, StacKColumnChart, "
            + "SplineChart, SplineAreaChart, ColumnLineChart, ScatterLineChart, ParetoChart, "
            + "StackAreaChart, StackLineChart, StackSplineChart, StackSplineAreaChart, HistogramChart, "
            + "ProbabilityChart, BoxChart, GanttChart")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EnableCrossHair
        {
            get { return chart.EnableCrossHair; }
            set { chart.EnableCrossHair = value; }
        }

        /// <summary>
        /// Аннотации
        /// </summary>
        [Category("Вид")]
        [Description("Аннотации")]
        [DisplayName("Аннотации")]
        [Browsable(true)]
        public AnnotationBrowseClass AnnotationBrowse
        {
            get { return annotationBrowse; }
            set { annotationBrowse = value; }
        }

        /// <summary>
        /// Эффекты
        /// </summary>
        [Category("Вид")]
        [Description("Эффекты")]
        [DisplayName("Эффекты")]
        [Browsable(true)]
        public EffectsBrowseClass EffectsBrowse
        {
            get { return effectsBrowse; }
            set { effectsBrowse = value; }
        }

        /*
        /// <summary>
        /// Серии
        /// </summary>
        [Category("Вид")]
        [Description("Серии")]
        [DisplayName("Серии")]
        [Editor(typeof(CustomSeriesCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public SeriesCollection Series
        {
            get { return chart.Series; }
        }
        */

        //// <summary>
        /// Режим сглаживания
        /// </summary>
        [Category("Вид")]
        [Description("Действует на границы полигонов в 3D диаграммах, линии в 2D диаграммах, линии осей.")]
        [DisplayName("Режим сглаживания")]
        [TypeConverter(typeof(SmoothingModeTypeConverter))]
        [DefaultValue(SmoothingMode.AntiAlias)]
        [Browsable(true)]
        public SmoothingMode SmoothMode
        {
            get { return chart.SmoothingMode; }
            set { chart.SmoothingMode = value; }
        }

        /// <summary>
        /// Режим визуализации текста
        /// </summary>
        [Category("Вид")]
        [Description("Действует на текстовые элементы 2D и 3D диаграмм: надписи, подсказки, подписи в легенде, подписи на осях.")]
        [DisplayName("Режим визуализации текста")]
        [TypeConverter(typeof(TextRenderingHintTypeConverter))]
        [DefaultValue(TextRenderingHint.SystemDefault)]
        [Browsable(true)]
        public TextRenderingHint TextRenderHint
        {
            get { return chart.TextRenderingHint; }
            set
            {
                chart.TextRenderingHint = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// Растягивать по размеру окна
        /// </summary>
        [Category("Окно элемента")]
        [Description("Растягивать диаграмму по окну элемента, игнорируюя ее собственные размеры")]
        [DisplayName("Растягивать диаграмму")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool FillAlignment
        {
            get
            {
                return (chart.Dock == DockStyle.Fill);
            }
            set
            {
                if (value)
                {
                    chart.Dock = DockStyle.Fill;
                }
                else
                {
                    chart.Dock = DockStyle.None;
                    chart.Size = chart.Parent.Size;
                }
            }
        }

        [Category("Окно элемента")]
        [DisplayName("Размеры диаграммы")]
        [Description("Учитываются, когда она не растянута по окну элемента")]
        [Browsable(true)]
        public ChartSize ChartSize
        {
            get
            {
                return chartSize;
            }
            set
            {
                chartSize = value;
            }
        }


        [Category("Управление данными")]
        [DisplayName("Синхронизация")]
        [Description("Синхронизация")]
        [Editor(typeof(SyncEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("IsComposite", "False")]
        [Browsable(true)]
        public ChartSynchronization Synchronization
        {
            get
            {
                return this.activeReportElement.Synchronization;
            }
            set
            {
                this.activeReportElement.Synchronization = value;
            }
        }


        #region Типы диаграмм

        /// <summary>
        /// Диаграмма AreaChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "AreaChart")]
        [Browsable(true)]
        public AreaChartBrowseClass AreaChartBrowse
        {
            get { return areaChartBrowse; }
            set { areaChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма BarChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "BarChart")]
        [Browsable(true)]
        public BarChartBrowseClass BarChartBrowse
        {
            get { return barChartBrowse; }
            set { barChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма BarChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "BarChart3D, CylinderBarChart3D")]
        [Browsable(true)]
        public BarChart3DBrowseClass BarChart3DBrowse
        {
            get { return barChart3DBrowse; }
            set { barChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма BoxChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "BoxChart")]
        [Browsable(true)]
        public BoxChartBrowseClass BoxChartBrowse
        {
            get { return boxChartBrowse; }
            set { boxChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма BubbleChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "BubbleChart")]
        [Browsable(true)]
        public BubbleChartBrowseClass BubbleChartBrowse
        {
            get { return bubbleChartBrowse; }
            set { bubbleChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ColumnChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ColumnChart")]
        [Browsable(true)]
        public ColumnChartBrowseClass ColumnChartBrowse
        {
            get { return columnChartBrowse; }
            set { columnChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ColumnChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ColumnChart3D, CylinderColumnChart3D")]
        [Browsable(true)]
        public ColumnChart3DBrowseClass ColumnChart3DBrowse
        {
            get { return columnChart3DBrowse; }
            set { columnChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма DoughnutChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "DoughnutChart, DoughnutChart3D")]
        [Browsable(true)]
        public DoughnutChartBrowseClass DoughnutChartBrowse
        {
            get { return doughnutChartBrowse; }
            set { doughnutChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма LineChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "LineChart")]
        [Browsable(true)]
        public LineChartBrowseClass LineChartBrowse
        {
            get { return lineChartBrowse; }
            set { lineChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ColumnLineChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ColumnLineChart")]
        [Browsable(true)]
        public ColumnLineChartBrowseClass ColumnLineChartBrowse
        {
            get { return columnLineChartBrowse; }
            set { columnLineChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма HeatMapChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "HeatMapChart, HeatMapChart3D")]
        [Browsable(true)]
        public HeatMapChartBrowseClass HeatMapChartBrowse
        {
            get { return heatMapChartBrowse; }
            set { heatMapChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма HistogramChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "HistogramChart")]
        [Browsable(true)]
        public HistogramChartBrowseClass HistogramChartBrowse
        {
            get { return histogramChartBrowse; }
            set { histogramChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ParetoChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ParetoChart")]
        [Browsable(true)]
        public ParetoChartBrowseClass ParetoChartBrowse
        {
            get { return paretoChartBrowse; }
            set { paretoChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма PieChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "PieChart, PieChart3D")]
        [Browsable(true)]
        public PieChartBrowseClass PieChartBrowse
        {
            get { return pieChartBrowse; }
            set { pieChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма PolarChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "PolarChart")]
        [Browsable(true)]
        public PolarChartBrowseClass PolarChartBrowse
        {
            get { return polarChartBrowse; }
            set { polarChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ProbabilityChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ProbabilityChart")]
        [Browsable(true)]
        public ProbabilityChartBrowseClass ProbabilityrChartBrowse
        {
            get { return probabilityChartBrowse; }
            set { probabilityChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма RadarChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "RadarChart")]
        [Browsable(true)]
        public RadarChartBrowseClass RadarChartBrowse
        {
            get { return radarChartBrowse; }
            set { radarChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ScatterChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ScatterChart")]
        [Browsable(true)]
        public ScatterChartBrowseClass ScatterChartBrowse
        {
            get { return scatterChartBrowse; }
            set { scatterChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ScatterLineChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ScatterLineChart")]
        [Browsable(true)]
        public ScatterLineChartBrowseClass ScatterLineChartBrowse
        {
            get { return scatterLineChartBrowse; }
            set { scatterLineChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма SplineChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "SplineChart")]
        [Browsable(true)]
        public SplineChartBrowseClass SplineChartBrowse
        {
            get { return splineChartBrowse; }
            set { splineChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма SplineAreaChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "SplineAreaChart")]
        [Browsable(true)]
        public SplineAreaChartBrowseClass SplineAreaChartBrowse
        {
            get { return splineAreaChartBrowse; }
            set { splineAreaChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма ConeChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "ConeChart3D")]
        [Browsable(true)]
        public ConeChart3DBrowseClass ConeChart3DBrowse
        {
            get { return coneChart3DBrowse; }
            set { coneChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма FunnelChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "FunnelChart3D")]
        [Browsable(true)]
        public FunnelChart3DBrowseClass FunnelChart3DBrowse
        {
            get { return funnelChart3DBrowse; }
            set { funnelChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма PointChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "PointChart3D")]
        [Browsable(true)]
        public PointChart3DBrowseClass PointChart3DBrowse
        {
            get { return pointChart3DBrowse; }
            set { pointChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма PyramidChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "PyramidChart3D")]
        [Browsable(true)]
        public PyramidChart3DBrowseClass PyramidChart3DBrowse
        {
            get { return pyramidChart3DBrowse; }
            set { pyramidChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма SplineAreaChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "SplineAreaChart3D")]
        [Browsable(true)]
        public SplineAreaChart3DBrowseClass SplineAreaChart3DBrowse
        {
            get { return splineAreaChart3DBrowse; }
            set { splineAreaChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма SplineChart3D
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "SplineChart3D")]
        [Browsable(true)]
        public SplineChart3DBrowseClass SplineChart3DBrowse
        {
            get { return splineChart3DBrowse; }
            set { splineChart3DBrowse = value; }
        }

        /// <summary>
        /// Диаграмма FunnelChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "FunnelChart")]
        [Browsable(true)]
        public FunnelChartBrowseClass FunnelChartBrowse
        {
            get { return funnelChartBrowse; }
            set { funnelChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма PyramidChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "PyramidChart")]
        [Browsable(true)]
        public PyramidChartBrowseClass PyramidChartBrowse
        {
            get { return pyramidChartBrowse; }
            set { pyramidChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма StackAreaChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "StackAreaChart")]
        [Browsable(true)]
        public StackAreaChartBrowseClass StackAreaChartBrowse
        {
            get { return stackAreaChartBrowse; }
            set { stackAreaChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма StackBarChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "StackBarChart")]
        [Browsable(true)]
        public StackBarChartBrowseClass StackBarChartBrowse
        {
            get { return stackBarChartBrowse; }
            set { stackBarChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма Stack3DBarChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "Stack3DBarChart, CylinderStackBarChart3D")]
        [Browsable(true)]
        public Stack3DBarChartBrowseClass Stack3DBarChartBrowse
        {
            get { return stack3DBarChartBrowse; }
            set { stack3DBarChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма StackColumnChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "StackColumnChart")]
        [Browsable(true)]
        public StackColumnChartBrowseClass StackColumnChartBrowse
        {
            get { return stackColumnChartBrowse; }
            set { stackColumnChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма Stack3DColumnChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "Stack3DColumnChart, CylinderStackColumnChart3D")]
        [Browsable(true)]
        public Stack3DColumnChartBrowseClass Stack3DColumnChartBrowse
        {
            get { return stack3DColumnChartBrowse; }
            set { stack3DColumnChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма StackLineChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "StackLineChart")]
        [Browsable(true)]
        public StackLineChartBrowseClass StackLineChartBrowse
        {
            get { return stackLineChartBrowse; }
            set { stackLineChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма StackSplineChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "StackSplineChart")]
        [Browsable(true)]
        public StackSplineChartBrowseClass StackSplineChartBrowse
        {
            get { return stackSplineChartBrowse; }
            set { stackSplineChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма StackSplineAreaChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "StackSplineAreaChart")]
        [Browsable(true)]
        public StackSplineAreaChartBrowseClass StackSplineAreaChartBrowse
        {
            get { return stackSplineAreaChartBrowse; }
            set { stackSplineAreaChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма CandleChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "CandleChart")]
        [Browsable(true)]
        public CandleChartBrowseClass CandleChart
        {
            get { return candleChartBrowse; }
            set { candleChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма GanttChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "GanttChart")]
        [Browsable(true)]
        public GanttChartBrowseClass GanttChart
        {
            get { return ganttChartBrowse; }
            set { ganttChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма CompositeChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "Composite")]
        [Browsable(true)]
        public CompositeChartBrowseClass CompositeChartBrowse
        {
            get { return compositeChartBrowse; }
            set { compositeChartBrowse = value; }
        }

        /// <summary>
        /// Диаграмма TreeMapChart
        /// </summary>
        [Category("Вид")]
        [Description("Вид области диаграммы")]
        [DisplayName("Область диаграммы")]
        [DynamicPropertyFilter("ChartType", "TreeMapChart")]
        [Browsable(true)]
        public TreeMapChartBrowseClass TreeMapChartBrowse
        {
            get { return treeMapChartBrowse; }
            set { treeMapChartBrowse = value; }
        }

        #endregion

        #endregion

        public ChartReportElementBrowseAdapter(DockableControlPane dcPane)
            : base(dcPane)
        {
            activeReportElement = (ChartReportElement)dcPane.Control;
            chart = activeReportElement.Chart;
            //chart.Show();
            chart.BeginInit();

            tooltipsBrowse = new TooltipsBrowseClass(chart.Tooltips);
            titleBrowse = new TitleBrowseClass(chart);
            colorModelBrowse = new ColorModelBrowseClass(chart.ColorModel);
            axiesBrowse = new AxiesBrowseClass(chart.Axis, chart);
            borderBrowse = new BorderBrowseClass(chart.Border);
            legendBrowse = new LegendBrowseClass(chart.Legend);
            if (chart.CompositeChart.Legends.Count != 0)
            {
                compositeLegendBrowse = new CompositeLegendBrowseClass(chart);
            }
            transform3DBrowse = new Transform3DBrowseClass(chart.Transform3D);
            annotationBrowse = new AnnotationBrowseClass(chart.Annotations);
            dataApperanceBrowse = new DataApperanceBrowseClass(chart.Data, chart);
            effectsBrowse = new EffectsBrowseClass(chart.Effects);

            areaChartBrowse = new AreaChartBrowseClass(chart.AreaChart);
            barChartBrowse = new BarChartBrowseClass(chart.BarChart);
            barChart3DBrowse = new BarChart3DBrowseClass(chart.BarChart3D);
            boxChartBrowse = new BoxChartBrowseClass(chart.BoxChart);
            bubbleChartBrowse = new BubbleChartBrowseClass(chart.BubbleChart);
            columnChartBrowse = new ColumnChartBrowseClass(chart.ColumnChart);
            columnChart3DBrowse = new ColumnChart3DBrowseClass(chart.ColumnChart3D);
            doughnutChartBrowse = new DoughnutChartBrowseClass(chart.DoughnutChart, chart);
            lineChartBrowse = new LineChartBrowseClass(chart.LineChart);
            columnLineChartBrowse = new ColumnLineChartBrowseClass(chart.ColumnLineChart.Column, chart.ColumnLineChart.Line);
            heatMapChartBrowse = new HeatMapChartBrowseClass(chart.HeatMapChart, chart);
            histogramChartBrowse = new HistogramChartBrowseClass(chart.HistogramChart);
            paretoChartBrowse = new ParetoChartBrowseClass(chart.ParetoChart);
            pieChartBrowse = new PieChartBrowseClass(chart.PieChart, chart);
            polarChartBrowse = new PolarChartBrowseClass(chart.PolarChart);
            probabilityChartBrowse = new ProbabilityChartBrowseClass(chart.ProbabilityChart);
            radarChartBrowse = new RadarChartBrowseClass(chart.RadarChart);
            scatterChartBrowse = new ScatterChartBrowseClass(chart.ScatterChart);
            scatterLineChartBrowse = new ScatterLineChartBrowseClass(chart.ScatterLineChart.Scatter, chart.ScatterLineChart.Line);
            splineChartBrowse = new SplineChartBrowseClass(chart.SplineChart);
            splineAreaChartBrowse = new SplineAreaChartBrowseClass(chart.SplineAreaChart);
            coneChart3DBrowse = new ConeChart3DBrowseClass(chart.ConeChart3D);
            funnelChart3DBrowse = new FunnelChart3DBrowseClass(chart.FunnelChart3D);
            pointChart3DBrowse = new PointChart3DBrowseClass(chart.PointChart3D);
            pyramidChart3DBrowse = new PyramidChart3DBrowseClass(chart.PyramidChart3D);
            splineAreaChart3DBrowse = new SplineAreaChart3DBrowseClass(chart.SplineAreaChart3D);
            splineChart3DBrowse = new SplineChart3DBrowseClass(chart.SplineChart3D);
            funnelChartBrowse = new FunnelChartBrowseClass(chart.FunnelChart);
            pyramidChartBrowse = new PyramidChartBrowseClass(chart.PyramidChart);
            stackAreaChartBrowse = new StackAreaChartBrowseClass(chart.AreaChart, chart.StackChart);
            stackBarChartBrowse = new StackBarChartBrowseClass(chart.BarChart, chart.StackChart);
            stack3DBarChartBrowse = new Stack3DBarChartBrowseClass(chart.BarChart3D, chart.StackChart);
            stackColumnChartBrowse = new StackColumnChartBrowseClass(chart.ColumnChart, chart.StackChart);
            stack3DColumnChartBrowse = new Stack3DColumnChartBrowseClass(chart.ColumnChart3D, chart.StackChart);
            stackLineChartBrowse = new StackLineChartBrowseClass(chart.LineChart, chart.StackChart);
            stackSplineChartBrowse = new StackSplineChartBrowseClass(chart.SplineChart, chart.StackChart);
            stackSplineAreaChartBrowse = new StackSplineAreaChartBrowseClass(chart.SplineAreaChart, chart.StackChart);
            candleChartBrowse = new CandleChartBrowseClass(chart.CandleChart);
            ganttChartBrowse = new GanttChartBrowseClass(chart.GanttChart);
            compositeChartBrowse = new CompositeChartBrowseClass(chart.CompositeChart);
            treeMapChartBrowse = new TreeMapChartBrowseClass(chart.TreeMapChart, chart);
            chartSize = new ChartSize(chart);

            chart.DataBind();
            chart.EndInit();
        }

        #region IChartComponent Members

        [Browsable(false)]
        public OverrideAppearance Override
        {
            get { return @override; }
        }

        [Browsable(false)]
        public string[] UserLayerIndex
        {
            get { return userLayerIndex; }
            set { userLayerIndex = value; }
        }

        [Browsable(false)]
        public ISite Site
        {
            get { return site; }
        }

        [Browsable(false)]
        public bool DataBindMe
        {
            get { return dataBindMe; }
            set { dataBindMe = value; }
        }

        [Browsable(false)]
        public RenderingType RenderingType
        {
            get { return renderingType; }
        }

        [Browsable(false)]
        public SmoothingMode SmoothingMode
        {
            get { return smoothingMode; }
            set { smoothingMode = value; }
        }

        [Browsable(false)]
        public bool IsSavingPreset
        {
            get { return isSavingPreset; }
        }

        [Browsable(false)]
        public TextRenderingHint TextRenderingHint
        {
            get { return textRenderingHint; }
            set { textRenderingHint = value; }
        }

        public void ResizeAll()
        {
        }

        public void Invalidate(CacheLevel cacheLevel)
        {
        }

        public ChartDataEventArgs GetChartInfoFromPoint(Point point)
        {
            return null;
        }

        public ChartAppearance GetChartAppearance(ChartAppearanceTypes appearanceType)
        {
            return null;
        }

        public void OnAppearanceChanged()
        {
        }

        public bool IsDesignMode()
        {
            return false;
        }

        public void ResetDemoData()
        {
        }

        public object GetProperty(string propName)
        {
            return null;
        }

        public object SetProperty(string propName, object newValue)
        {
            return null;
        }

        public void DataValidity(bool validity, string info)
        {
        }

        public void DataValidity(bool validity, ChartDataInvalidEventArgs invalidArgs)
        {
        }

        public object GetComponentInfo()
        {
            return null;
        }

        public void Subscribe(string evtName, EventHandler handler)
        {
        }

        public void OnFillSceneGraph(FillSceneGraphEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnInterpolateValues(InterpolateValuesEventArgs e)
        {
        }

        public void OnUpdated()
        {
        }

        [Browsable(false)]
        public SeriesCollection Series
        {
            get { return null; }
        }
        
        #endregion

    }
}