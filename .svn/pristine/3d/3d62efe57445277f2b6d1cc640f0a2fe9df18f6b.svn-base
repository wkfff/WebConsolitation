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
    public class PieLabelAppearanceBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private PieLabelAppearance pieLabelAppearance;
        private UltraChart ultraChart;
        private ChartFormatBrowseClass pieLabelFormat;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип диаграммы
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return ultraChart.ChartType; }
        }

        /// <summary>
        /// Цвет границы выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Цвет границы выноски")]
        [DisplayName("Цвет границы")]
        [DynamicPropertyFilter("ChartType", "PieChart")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return pieLabelAppearance.BorderColor; }
            set { pieLabelAppearance.BorderColor = value; }
        }

        /// <summary>
        /// Стиль границы выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Стиль границы выноски")]
        [DisplayName("Стиль границы")]
        [DynamicPropertyFilter("ChartType", "PieChart")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle BorderDrawStyle
        {
            get { return pieLabelAppearance.BorderDrawStyle; }
            set { pieLabelAppearance.BorderDrawStyle = value; }
        }

        /// <summary>
        /// Толщина границы выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Толщина границы выноски")]
        [DisplayName("Толщина границы")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ChartType", "PieChart")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int BorderThickness
        {
            get { return pieLabelAppearance.BorderThickness; }
            set { pieLabelAppearance.BorderThickness = value; }
        }

        /// <summary>
        /// Цвет фона выноски
        /// </summary>
        [Category("Выноски")]
        [Description("Цвет фона выноски")]
        [DisplayName("Цвет фона")]
        [DynamicPropertyFilter("ChartType", "PieChart")]
        [DefaultValue(typeof(Color), "Transparent")]
        [Browsable(true)]
        public Color FillColor
        {
            get { return pieLabelAppearance.FillColor; }
            set { pieLabelAppearance.FillColor = value; }
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
            get { return pieLabelAppearance.Font; }
            set { pieLabelAppearance.Font = value; }
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
            get { return pieLabelAppearance.FontColor; }
            set { pieLabelAppearance.FontColor = value; }
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
                return pieLabelAppearance.Format; 
            }
            set 
            { 
                pieLabelAppearance.Format = value;
                this.FormatString = pieLabelAppearance.FormatString;
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
                return pieLabelAppearance.FormatString; 
            }
            set 
            {
                PieLabelFormat.FormatString = value;
                pieLabelAppearance.FormatString = value;
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
            get { return pieLabelAppearance.LeaderDrawStyle; }
            set { pieLabelAppearance.LeaderDrawStyle = value; }
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
            get { return pieLabelAppearance.LeaderEndStyle; }
            set { pieLabelAppearance.LeaderEndStyle = value; }
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
            get { return pieLabelAppearance.LeaderLineColor; }
            set
            {
                pieLabelAppearance.LeaderLineColor = value;
                ultraChart.InvalidateLayers();
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
            get { return pieLabelAppearance.LeaderLineThickness; }
            set { pieLabelAppearance.LeaderLineThickness = value; }
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
            get { return pieLabelAppearance.LeaderLinesVisible; }
            set { pieLabelAppearance.LeaderLinesVisible = value; }
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
            get { return pieLabelAppearance.Visible; }
            set { pieLabelAppearance.Visible = value; }
        }

        #endregion

        public PieLabelAppearanceBrowseClass(PieLabelAppearance pieLabelAppearance, UltraChart ultraChart)
        {
            this.pieLabelAppearance = pieLabelAppearance;
            this.ultraChart = ultraChart;

            this.pieLabelFormat = new ChartFormatBrowseClass(pieLabelAppearance.FormatString, 
                                                                ChartFormatBrowseClass.LabelType.PieLabel,
                                                                ultraChart);

            this.pieLabelFormat.FormatChanged += new ValueFormatEventHandler(pieLabelFormat_FormatChanged);
        }

        private void pieLabelFormat_FormatChanged()
        {
           /* if (pieLabelFormat.FormatType == FormatType.Auto)
            {
                //pieLabelAppearance.FormatString = "<PERCENT_VALUE:#0.00>%";
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(PieLabelAppearance));
                pdc["FormatString"].ResetValue(pieLabelAppearance);


            }
            else*/
            {
                pieLabelAppearance.FormatString = pieLabelFormat.FormatString;
            }
        }

        public override string ToString()
        {
            return pieLabelAppearance.FormatString;
        }
    }
}