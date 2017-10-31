using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DoughnutLabelClass : FilterablePropertyBase
    {
        #region Поля

        private PieLabelAppearance pieLabelApperance;
        private UltraChart chart;
        private ChartFormatBrowseClass pieLabelFormat;

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
        /// Цвет границы Выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Цвет границы выноски")]
        [DisplayName("Цвет границы")]
        [DynamicPropertyFilter("ChartType", "DoughnutChart")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return pieLabelApperance.BorderColor; }
            set { pieLabelApperance.BorderColor = value; }
        }

        /// <summary>
        /// Стиль границы выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Стиль границы выноски")]
        [DisplayName("Стиль границы")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DynamicPropertyFilter("ChartType", "DoughnutChart")]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle BorderDrawStyle
        {
            get { return pieLabelApperance.BorderDrawStyle; }
            set { pieLabelApperance.BorderDrawStyle = value; }
        }

        /// <summary>
        /// Толщина границы выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Толщина границы выноски")]
        [DynamicPropertyFilter("ChartType", "DoughnutChart")]
        [DisplayName("Толщина границы")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(0)]
        [Browsable(true)]
        public int BorderThickness
        {
            get { return pieLabelApperance.BorderThickness; }
            set { pieLabelApperance.BorderThickness = value; }
        }

        /// <summary>
        /// Цвет фона выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Цвет фона выноски")]
        [DynamicPropertyFilter("ChartType", "DoughnutChart")]
        [DisplayName("Цвет фона")]
        [DefaultValue(typeof(Color), "Transparent")]
        [Browsable(true)]
        public Color FillColor
        {
            get { return pieLabelApperance.FillColor; }
            set { pieLabelApperance.FillColor = value; }
        }

        /// <summary>
        /// Шрифт текста выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Шрифт текста выноски")]
        [DisplayName("Шрифт текста")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font Font
        {
            get { return pieLabelApperance.Font; }
            set { pieLabelApperance.Font = value; }
        }

        /// <summary>
        /// Цвет шрифта текта выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Цвет шрифта текста выноски")]
        [DisplayName("Цвет шрифта")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return pieLabelApperance.FontColor; }
            set { pieLabelApperance.FontColor = value; }
        }

        /// <summary>
        /// Формат выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Тип формата выноски")]
        [DisplayName("Тип формата")]
        [DefaultValue(Infragistics.UltraChart.Shared.Styles.PieLabelFormat.Custom)]
        [Browsable(false)]
        public PieLabelFormat Format
        {
            get 
            { 
                return pieLabelApperance.Format; 
            }
            set 
            { 
                pieLabelApperance.Format = value;
                FormatString = pieLabelApperance.FormatString;
            }
        }

        /// <summary>
        /// Строка формата выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Строка формата выноски")]
        [DisplayName("Формат (строка)")]
        [DefaultValue("<PERCENT_VALUE:#0.00>%")]
        [Browsable(true)]
        public string FormatString
        {
            get 
            { 
                return pieLabelApperance.FormatString; 
            }
            set 
            {
                PieLabelFormat.FormatString = value;
                pieLabelApperance.FormatString = value;
            }
        }

        [Category("Выноски")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата выноски")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public PieLabelFormatPattern PieLabelPattern
        {
            get
            {
                return this.pieLabelFormat.PieLabelPattern;
            }
            set
            {
                this.pieLabelFormat.PieLabelPattern = value;
            }
        }


        [Category("Выноски")]
        [Description("Формат выноски")]
        [DisplayName("Формат")]
        [Browsable(true)]
        public ChartFormatBrowseClass PieLabelFormat
        {
            get
            {
                return this.pieLabelFormat;

            }
            set
            {
                this.pieLabelFormat = value;
            }
        }


        /// <summary>
        /// Стиль линии выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Стиль линии выноски")]
        [DisplayName("Стиль линии")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Dot)]
        [Browsable(true)]
        public LineDrawStyle LeaderDrawStyle
        {
            get { return pieLabelApperance.LeaderDrawStyle; }
            set { pieLabelApperance.LeaderDrawStyle = value; }
        }

        /// <summary>
        /// Окончание линии выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Стиль окончания линии выноски")]
        [DisplayName("Стиль окончания линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.ArrowAnchor)]
        [Browsable(true)]
        public LineCapStyle LeaderEndStyle
        {
            get { return pieLabelApperance.LeaderEndStyle; }
            set { pieLabelApperance.LeaderEndStyle = value; }
        }

        /// <summary>
        /// Цвет линии выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Цвет линии выноски")]
        [DisplayName("Цвет линии")]
        [DefaultValue(typeof(Color), "White")]
        [Browsable(true)]
        public Color LeaderLineColor
        {
            get { return pieLabelApperance.LeaderLineColor; }
            set
            {
                pieLabelApperance.LeaderLineColor = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// Толщина линии выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Толщина линии выноски")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int LeaderLineThickness
        {
            get { return pieLabelApperance.LeaderLineThickness; }
            set { pieLabelApperance.LeaderLineThickness = value; }
        }

        /// <summary>
        /// Видимость линии выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Показывать линии выноски")]
        [DisplayName("Показывать линии")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool LeaderLinesVisible
        {
            get { return pieLabelApperance.LeaderLinesVisible; }
            set { pieLabelApperance.LeaderLinesVisible = value; }
        }

        /// <summary>
        /// Видимость выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Показывать выноски")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool Visible
        {
            get { return pieLabelApperance.Visible; }
            set { pieLabelApperance.Visible = value; }
        }

        #endregion

        public DoughnutLabelClass(PieLabelAppearance pieLabelApperance, UltraChart chart)
        {
            this.pieLabelApperance = pieLabelApperance;
            this.chart = chart;

            this.pieLabelFormat = new ChartFormatBrowseClass(pieLabelApperance.FormatString, 
                                                                ChartFormatBrowseClass.LabelType.PieLabel,
                                                                chart);

            this.pieLabelFormat.FormatChanged += new ValueFormatEventHandler(pieLabelFormat_FormatChanged);
        }

        private void pieLabelFormat_FormatChanged()
        {
            /*if (pieLabelFormat.FormatType == FormatType.Auto)
            {
                // pieLabelApperance.FormatString = "<PERCENT_VALUE:#0.00>%";
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(PieLabelAppearance));
                pdc["FormatString"].ResetValue(pieLabelApperance);

            }
            else*/
            {
                pieLabelApperance.FormatString = pieLabelFormat.FormatString;
            }
        }

        public override string ToString()
        {
            return pieLabelApperance.FormatString;
        }
    }
}