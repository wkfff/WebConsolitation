using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Метка на осях координат
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisLabelBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private AxisAppearance axisAppearance;
        private ChartFormatBrowseClass labelFormat;
        private bool isLabelFormatRefreshing;


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
        public bool DisplayLabelsPadding
        {
            get
            {
                return InfragisticsUtils.IsAvaibleLabelsPadding(axisAppearance.ChartComponent.ChartType);
            }
        }

        [Browsable(false)]
        public bool DisplayAlignDescriptionLabelAxis
        {
            get
            {
                if  (InfragisticsUtils.IsAvaibleAlignDescriptionLabelAxis(axisAppearance.ChartComponent.ChartType,
                                                               axisAppearance.axisNumber) )
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
        [Description("Зеркальное отражение.")]
        [DisplayName("Зеркальное отражение")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("DisplayOrientationAngle", "True")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Flip
        {
            get { return axisAppearance.Labels.Flip; }
            set { axisAppearance.Labels.Flip = value; }
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
            get { return axisAppearance.Labels.ReverseText; }
            set { axisAppearance.Labels.ReverseText = value; }
        }

        /// <summary>
        /// Горизонатальное выравнивание метки
        /// </summary>
        [Category("Метки")]
        [Description("Горизонатальное выравнивание.")]
        [DisplayName("Горизонатальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentColumnHorizontalConverter))]
        [DynamicPropertyFilter("DisplayAlignDescriptionLabelAxis", "True")]
        [DefaultValue(StringAlignment.Near)]
        [Browsable(true)]
        public StringAlignment HorizontalAlign
        {
            get { return axisAppearance.Labels.HorizontalAlign; }
            set { axisAppearance.Labels.HorizontalAlign = value; }
        }

        /// <summary>
        /// Вертикальное выравнивание метки
        /// </summary>
        [Category("Метки")]
        [Description("Вертикальное выравнивание.")]
        [DisplayName("Вертикальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentColumnVerticalConverter))]
        [DynamicPropertyFilter("DisplayAlignDescriptionLabelAxis", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalAlign
        {
            get { return axisAppearance.Labels.VerticalAlign; }
            set { axisAppearance.Labels.VerticalAlign = value; }
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
            get { return axisAppearance.Labels.Font; }
            set { axisAppearance.Labels.Font = value; }
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
            get { return axisAppearance.Labels.FontColor; }
            set { axisAppearance.Labels.FontColor = value; }
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
            get { return axisAppearance.Labels.Orientation; }
            set { axisAppearance.Labels.Orientation = value; }
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
                if (axisAppearance.Labels.Orientation == TextOrientation.Custom)
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
                    axisAppearance.Labels.Orientation = TextOrientation.Custom;
                }
                else
                {
                    if ((axisAppearance.axisNumber == AxisNumber.X_Axis) || (axisAppearance.axisNumber == AxisNumber.X2_Axis))
                    {
                        axisAppearance.Labels.Orientation = TextOrientation.VerticalLeftFacing;
                    }
                    else
                    {
                        axisAppearance.Labels.Orientation = TextOrientation.Horizontal;
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
        [DynamicPropertyFilter("DisplayOrientationAngle", "True")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int OrientationAngle
        {
            get { return axisAppearance.Labels.OrientationAngle; }
            set { axisAppearance.Labels.OrientationAngle = value; }
        }

        /// <summary>
        /// Формат метки
        /// </summary>
        [Category("Метки")]
        [Description("Тип формата метки")]
        [DisplayName("Тип формата")]
        // [DefaultValue(AxisItemLabelFormat.ItemLabel)]
        [Browsable(false)]
        public AxisItemLabelFormat Format
        {
            get 
            { 
                return axisAppearance.Labels.ItemFormat; 
            }
            set 
            { 
                axisAppearance.Labels.ItemFormat = value;
                this.ItemFormatString = axisAppearance.Labels.ItemFormatString;

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
                return axisAppearance.Labels.ItemFormatString; 
            }
            set 
            { 
                LabelFormat.FormatString = value;
                axisAppearance.Labels.ItemFormatString = value;
            }
        }

        [Category("Метки")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата метки")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public AxisLabelFormatPattern AxisLabelPattern
        {
            get
            {
                return this.labelFormat.AxisLabelPattern;
            }
            set
            {
                this.labelFormat.AxisLabelPattern = value;
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
            get { return axisAppearance.Labels.Visible; }
            set { axisAppearance.Labels.Visible = value; }
        }

        /// <summary>
        /// Отступ метки
        /// </summary>
        [Category("Метки")]
        [Description("Отступ")]
        [DisplayName("Отступ")]
        [DynamicPropertyFilter("DisplayLabelsPadding", "True")]
        [DefaultValue(2)]
        [Browsable(true)]
        public int Padding
        {
            get { return axisAppearance.Labels.Layout.Padding; }
            set { axisAppearance.Labels.Layout.Padding = value; }
        }

        #endregion

        public AxisLabelBrowseClass(AxisAppearance axisAppearance)
        {
            this.axisAppearance = axisAppearance;
            this.isLabelFormatRefreshing = false;

            this.labelFormat = new ChartFormatBrowseClass(this.axisAppearance.Labels.ItemFormatString, 
                                                            ChartFormatBrowseClass.LabelType.AxisLabel,
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
            /*
            if (labelFormat.FormatType == FormatType.Auto)
            {
                //для оси Z значение по умолчанию почему то выставляется некорректное, будем устанавливать свое
                if (this.axisAppearance.axisNumber == AxisNumber.Z_Axis)
                {
                    axisAppearance.Labels.ItemFormatString = "<ITEM_LABEL>";
                }
                else
                {
                    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(AxisLabelAppearance));
                    pdc["ItemFormatString"].ResetValue(axisAppearance.Labels);
                }
            }
            else*/

            if (!this.isLabelFormatRefreshing)
            {
                axisAppearance.Labels.ItemFormatString = this.labelFormat.FormatString;
            }
        }

        private void labelFormat_FormatStringChanged()
        {
            if (!this.isLabelFormatRefreshing)
                this.labelFormat.FormatString = axisAppearance.Labels.ItemFormatString;
        }

        /// <summary>
        /// Обновление формата
        /// </summary>
        public void RefreshFormat()
        {
            this.isLabelFormatRefreshing = true;
            this.labelFormat.FormatString = axisAppearance.Labels.ItemFormatString;
            this.isLabelFormatRefreshing = false;
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