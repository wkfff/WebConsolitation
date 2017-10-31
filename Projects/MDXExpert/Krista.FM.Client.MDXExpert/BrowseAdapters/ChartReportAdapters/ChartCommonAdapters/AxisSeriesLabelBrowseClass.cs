using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Метка серий на осях координат
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisSeriesLabelBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private AxisSeriesLabelAppearance axisSeriesLabelAppearance;
        private AxisAppearance axisAppearance;
        private ChartFormatBrowseClass labelFormat;

        #endregion

        #region Свойства


        [Browsable(false)]
        public bool IsX_AxisRadarOrPolarChart
        {
            get 
            {
                return (((axisAppearance.ChartComponent.ChartType == ChartType.RadarChart) || 
                         (axisAppearance.ChartComponent.ChartType == ChartType.PolarChart)) && 
                        (axisAppearance.axisNumber == AxisNumber.X_Axis));
            }
        }

        [Browsable(false)]
        public bool DisplayOrientationAngle
        {
            get
            {
                return !(Is3DChart(axisAppearance.ChartComponent.ChartType) && (axisAppearance.axisNumber == AxisNumber.Y_Axis)) &&
                       !IsX_AxisRadarOrPolarChart &&
                        Orientation == TextOrientation.Custom;
            }
        }

        [Browsable(false)]
        public bool DisplaySeriesLabelsPadding
        {
            get
            {
                return InfragisticsUtils.IsAvaibleSeriesLabelsPadding(axisAppearance.ChartComponent.ChartType,
                                                                     axisAppearance.axisNumber,
                                                                     axisSeriesLabelAppearance);
            }
        }

        [Browsable(false)]
        public bool DisplayOrientation
        {
            get
            {
                return !Is3DChart(axisAppearance.ChartComponent.ChartType)&&
                       !IsX_AxisRadarOrPolarChart;
                    
            }
        }

        [Browsable(false)]
        public bool DisplayChart3DOrientation
        {
            get
            {
                return (Is3DChart(axisAppearance.ChartComponent.ChartType) && (axisAppearance.axisNumber != AxisNumber.Y_Axis))&&
                       !IsX_AxisRadarOrPolarChart;
            }
        }

        [Browsable(false)]
        public bool DisplayHAlingment
        {
            get
            {
                return InfragisticsUtils.IsAvaibleHAlingmentSeriesLabelAxis(axisAppearance.ChartComponent.ChartType, axisAppearance.axisNumber) &&
                    !IsX_AxisRadarOrPolarChart;
            }
        }

        [Browsable(false)]
        public bool DisplayVAlingment
        {
            get
            {
                return InfragisticsUtils.IsAvaibleVAlingmentSeriesLabelAxis(axisAppearance.ChartComponent.ChartType, axisAppearance.axisNumber) &&
                    !IsX_AxisRadarOrPolarChart;
            }
        }

        [Browsable(false)]
        public bool DisplayAlignDescriptionLabelAxis
        {
            get
            {
                if (InfragisticsUtils.IsAvaibleAlignDescriptionLabelAxis(axisAppearance.ChartComponent.ChartType,
                                                               axisAppearance.axisNumber))
                {
                    return !IsX_AxisRadarOrPolarChart;
                }
                else
                {
                    return Orientation == TextOrientation.Custom && !IsX_AxisRadarOrPolarChart;
                }
            }
        }

        /// <summary>
        /// Зеркальное отражение метки
        /// </summary>
        [Category("Метки")]
        [Description("Работает только при пользовательской ориентации.")]
        [DisplayName("Зеркальное отражение")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("DisplayOrientationAngle", "True")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Flip
        {
            get { return axisSeriesLabelAppearance.Flip; }
            set { axisSeriesLabelAppearance.Flip = value; }
        }

        /// <summary>
        /// Обращение текста метки
        /// </summary>
        [Category("Метки")]
        [Description("Обращение текста метки")]
        [DisplayName("Обращение текста")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsX_AxisRadarOrPolarChart", "False")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ReverseText
        {
            get { return axisSeriesLabelAppearance.ReverseText; }
            set { axisSeriesLabelAppearance.ReverseText = value; }
        }

        /// <summary>
        /// Горизонатальное выравнивание метки
        /// </summary>
        [Category("Метки")]
        [Description("Горизонатальное выравнивание")]
        [DisplayName("Горизонатальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentColumnHorizontalConverter))]
        [DynamicPropertyFilter("DisplayHAlingment", "True")]
        [DefaultValue(StringAlignment.Near)]
        [Browsable(true)]
        public StringAlignment HorizontalAlign
        {
            get { return axisSeriesLabelAppearance.HorizontalAlign; }
            set { axisSeriesLabelAppearance.HorizontalAlign = value; }
        }

        /// <summary>
        /// Вертикальное выравнивание метки
        /// </summary>
        [Category("Метки")]
        [Description("Вертикальное выравнивание")]
        [DisplayName("Вертикальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentColumnVerticalConverter))]
        [DynamicPropertyFilter("DisplayVAlingment", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalAlign
        {
            get { return axisSeriesLabelAppearance.VerticalAlign; }
            set { axisSeriesLabelAppearance.VerticalAlign = value; }
        }

        /// <summary>
        /// Шрифт метки
        /// </summary>
        [Category("Метки")]
        [Description("Шрифт метки")]
        [DisplayName("Шрифт метки")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font Font
        {
            get { return axisSeriesLabelAppearance.Font; }
            set { axisSeriesLabelAppearance.Font = value; }
        }

        /// <summary>
        /// Цвет шрифта метки
        /// </summary>
        [Category("Метки")]
        [Description("Цвет шрифта метки")]
        [DisplayName("Цвет шрифта")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return axisSeriesLabelAppearance.FontColor; }
            set { axisSeriesLabelAppearance.FontColor = value; }
        }

        /// <summary>
        /// Ориентация метки
        /// </summary>
        [Category("Метки")]
        [Description("Ориентация метки")]
        [DisplayName("Ориентация")]
        [TypeConverter(typeof(TextOrientationTypeConverter))]
        [DynamicPropertyFilter("DisplayOrientation", "True")]
        [DefaultValue(TextOrientation.VerticalLeftFacing)]
        [Browsable(true)]
        public TextOrientation Orientation
        {
            get { return axisSeriesLabelAppearance.Orientation; }
            set { axisSeriesLabelAppearance.Orientation = value; }
        }

        /// <summary>
        /// Ориентация метки
        /// </summary>
        [Category("Метки")]
        [Description("Ориентация метки")]
        [DisplayName("Ориентация")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayChart3DOrientation", "True")]
        [DefaultValue(AxisLabelBrowseClass.Chart3DTextOrientation.Auto)]
        [Browsable(true)]
        public Chart3DTextOrientation Chart3DOrientation
        {
            get 
            {
                if (axisSeriesLabelAppearance.Orientation == TextOrientation.Custom)
                {
                    return Chart3DTextOrientation.Custom;
                }
                else
                {
                    return Chart3DTextOrientation.Auto;
                }
            }
            set 
            {
                if (value == Chart3DTextOrientation.Custom)
                {
                    axisSeriesLabelAppearance.Orientation = TextOrientation.Custom;
                }
                else
                {
                    if ((axisAppearance.axisNumber == AxisNumber.X_Axis) || (axisAppearance.axisNumber == AxisNumber.X2_Axis))
                    {
                        axisSeriesLabelAppearance.Orientation = TextOrientation.VerticalLeftFacing;
                    }
                    else
                    {
                        axisSeriesLabelAppearance.Orientation = TextOrientation.Horizontal;
                    }
                }
            }
        }


        /// <summary>
        /// Угол поворота метки
        /// </summary>
        [Category("Метки")]
        [Description("Угол поворота.")]
        [DisplayName("Угол поворота")]
        //[Editor(typeof(ShadowControlEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("DisplayOrientationAngle", "True")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int OrientationAngle
        {
            get { return axisSeriesLabelAppearance.OrientationAngle; }
            set { axisSeriesLabelAppearance.OrientationAngle = value; }
        }

        /// <summary>
        /// Формат метки
        /// </summary>
        [Category("Метки")]
        [Description("Тип формата метки")]
        [DisplayName("Тип формата")]
        // [DefaultValue(AxisItemLabelFormat.ItemLabel)]
        [Browsable(false)]
        public AxisSeriesLabelFormat Format
        {
            get 
            {
                return axisSeriesLabelAppearance.Format; 
            }
            set 
            {
                axisSeriesLabelAppearance.Format = value;
                this.ItemFormatString = axisSeriesLabelAppearance.FormatString;

            }
        }

        /// <summary>
        /// Строка формата метки
        /// </summary>
        [Category("Метки")]
        [Description("Строка формата метки")]
        [DisplayName("Формат (строка)")]
        // [DefaultValue("<ITEM_LABEL>")]
        [Browsable(true)]
        public string ItemFormatString
        {
            get 
            {
                return axisSeriesLabelAppearance.FormatString; 
            }
            set 
            { 
                LabelFormat.FormatString = value;
                axisSeriesLabelAppearance.FormatString = value;
            }
        }

        [Category("Метки")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата метки")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public SeriesLabelFormatPattern SeriesLabelPattern
        {
            get
            {
                return this.labelFormat.SeriesLabelPattern;
            }
            set
            {
                this.labelFormat.SeriesLabelPattern = value;
            }
        }


        [Category("Метки")]
        [Description("Формат метки")]
        [DisplayName("Формат")]
        [Browsable(true)]
        public ChartFormatBrowseClass LabelFormat
        {
            get
            {
                return this.labelFormat;
                
            }
            set
            {
                this.labelFormat = value;
            }
        }


        /// <summary>
        /// Видимость метки
        /// </summary>
        [Category("Метки")]
        [Description("Показывать метку")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool Visible
        {
            get { return axisSeriesLabelAppearance.Visible; }
            set { axisSeriesLabelAppearance.Visible = value; }
        }

        /// <summary>
        /// Отступ метки
        /// </summary>
        [Category("Метки")]
        [Description("Отступ.")]
        [DisplayName("Отступ")]
        [DynamicPropertyFilter("DisplaySeriesLabelsPadding", "True")]
        [DefaultValue(2)]
        [Browsable(true)]
        public int Padding
        {
            get { return axisSeriesLabelAppearance.Layout.Padding; }
            set { axisSeriesLabelAppearance.Layout.Padding = value; }
        }

        #endregion

        public AxisSeriesLabelBrowseClass(AxisSeriesLabelAppearance axisSeriesLabelAppearance, AxisAppearance axisAppearance)
        {
            this.axisSeriesLabelAppearance = axisSeriesLabelAppearance;
            this.axisAppearance = axisAppearance;

            this.labelFormat = new ChartFormatBrowseClass(this.axisSeriesLabelAppearance.FormatString, 
                                                            ChartFormatBrowseClass.LabelType.SeriesLabel,
                                                            this.axisAppearance.ChartComponent);

            this.labelFormat.FormatChanged += new ValueFormatEventHandler(labelFormat_FormatChanged);
            this.labelFormat.FormatStringChanged += new ValueFormatEventHandler(labelFormat_FormatStringChanged);
        }

        private bool Is3DChart(ChartType chartType)
        {
            switch (chartType)
            {
                case ChartType.AreaChart3D:
                case ChartType.BarChart3D:
                case ChartType.BubbleChart3D:
                case ChartType.ColumnChart3D:
                case ChartType.ConeChart3D:
                case ChartType.CylinderBarChart3D:
                case ChartType.CylinderColumnChart3D:
                case ChartType.CylinderStackBarChart3D:
                case ChartType.CylinderStackColumnChart3D:
                case ChartType.DoughnutChart3D:
                case ChartType.FunnelChart3D:
                case ChartType.HeatMapChart3D:
                case ChartType.LineChart3D:
                case ChartType.PieChart3D:
                case ChartType.PointChart3D:
                case ChartType.PyramidChart3D:
                case ChartType.SplineAreaChart3D:
                case ChartType.SplineChart3D:
                case ChartType.Stack3DBarChart:
                case ChartType.Stack3DColumnChart:
                    return true;
            }

            return false;
        }

        private void labelFormat_FormatChanged()
        {
            axisSeriesLabelAppearance.FormatString = this.labelFormat.FormatString;
        }

        private void labelFormat_FormatStringChanged()
        {
            this.labelFormat.FormatString = axisSeriesLabelAppearance.FormatString;
        }

        public override string ToString()
        {
            return Font.Name + "; " + TextOrientationTypeConverter.ToString(Orientation) + "; " + Format;
        }

        public enum Chart3DTextOrientation
        {
            [Description("Автоматическая")]
            Auto,
            [Description("Пользовательская")]
            Custom
        }

    }
}
