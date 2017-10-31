using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HierarchicalChart3DLabelsApperanceBrowseClass
    {
        #region Поля

        private HierarchicalChart3DLabelsAppearance chart3DLabelApperance;
        private ChartFormatBrowseClass chart3DLabelFormat;

        #endregion

        #region Свойства

        /// <summary>
        /// Цвет границы подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Цвет границы подписи")]
        [DisplayName("Цвет границы")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return chart3DLabelApperance.BorderColor; }
            set { chart3DLabelApperance.BorderColor = value; }
        }

        /// <summary>
        /// Стиль границы подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Стиль границы подписи")]
        [DisplayName("Стиль границы")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle BorderDrawStyle
        {
            get { return chart3DLabelApperance.BorderDrawStyle; }
            set { chart3DLabelApperance.BorderDrawStyle = value; }
        }

        /// <summary>
        /// Толщина границы подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Толщина границы подписи")]
        [DisplayName("Толщина границы")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int BorderThickness
        {
            get { return chart3DLabelApperance.BorderThickness; }
            set { chart3DLabelApperance.BorderThickness = value; }
        }

        /// <summary>
        /// Цвет фона подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Цвет фона подписи")]
        [DisplayName("Цвет фона")]
        [Browsable(true)]
        public Color FillColor
        {
            get { return chart3DLabelApperance.FillColor; }
            set { chart3DLabelApperance.FillColor = value; }
        }

        /// <summary>
        /// Шрифт текста подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Шрифт текста подписи")]
        [DisplayName("Шрифт текста")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return chart3DLabelApperance.Font; }
            set { chart3DLabelApperance.Font = value; }
        }

        /// <summary>
        /// Цвет шрифта текта подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Цвет шрифта текта подписи")]
        [DisplayName("Цвет шрифта")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return chart3DLabelApperance.FontColor; }
            set { chart3DLabelApperance.FontColor = value; }
        }

        /// <summary>
        /// Формат подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Строка формата подписи")]
        [DisplayName("Формат (строка)")]
        [Browsable(true)]
        public string FormatString
        {
            get 
            { 
                return chart3DLabelApperance.FormatString; 
            }
            set 
            {
                Chart3DLabelFormat.FormatString = value;
                chart3DLabelApperance.FormatString = value;
            }
        }

        [Category("Подписи")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата метки")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public AxisLabelFormatPattern AxisLabelPattern
        {
            get
            {
                return this.chart3DLabelFormat.AxisLabelPattern;
            }
            set
            {
                this.chart3DLabelFormat.AxisLabelPattern = value;
            }
        }


        [Category("Подписи")]
        [Description("Формат подписи")]
        [DisplayName("Формат")]
        [Browsable(true)]
        public ChartFormatBrowseClass Chart3DLabelFormat
        {
            get
            {
                return this.chart3DLabelFormat;

            }
            set
            {
                this.chart3DLabelFormat = value;
            }
        }


        /// <summary>
        /// Стиль линии подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Стиль линии подписи")]
        [DisplayName("Стиль линии")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle LeaderDrawStyle
        {
            get { return chart3DLabelApperance.LeaderLineDrawStyle; }
            set { chart3DLabelApperance.LeaderLineDrawStyle = value; }
        }

        /// <summary>
        /// Стиль окончания линии подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Стиль окончания линии подписи")]
        [DisplayName("Стиль окончания линии")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle LeaderLineEndStyle
        {
            get { return chart3DLabelApperance.LeaderLineEndStyle; }
            set { chart3DLabelApperance.LeaderLineEndStyle = value; }
        }

        /// <summary>
        /// Цвет линии подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Цвет линии подписи")]
        [DisplayName("Цвет линии")]
        [Browsable(true)]
        public Color LeaderLineColor
        {
            get { return chart3DLabelApperance.LeaderLineColor; }
            set { chart3DLabelApperance.LeaderLineColor = value; }
        }

        /// <summary>
        /// Толщина линии подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Толщина линии подписи")]
        [DisplayName("Толщина линии")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int LeaderLineThickness
        {
            get { return chart3DLabelApperance.LeaderLineThickness; }
            set { chart3DLabelApperance.LeaderLineThickness = value; }
        }

        /// <summary>
        /// Видимость линии подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Показывать линии подписи")]
        [DisplayName("Показывать линии")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool LeaderLinesVisible
        {
            get { return chart3DLabelApperance.LeaderLinesVisible; }
            set { chart3DLabelApperance.LeaderLinesVisible = value; }
        }

        /// <summary>
        /// Видимость подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Показывать подписи")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return chart3DLabelApperance.Visible; }
            set { chart3DLabelApperance.Visible = value; }
        }

        /// <summary>
        /// Горизонтальное выравнивание
        /// </summary>
        [Category("Подписи")]
        [Description("Горизонтальное выравнивание")]
        [DisplayName("Горизонтальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [Browsable(true)]
        public StringAlignment HorizontalAlign
        {
            get { return chart3DLabelApperance.HorizontalAlign; }
            set { chart3DLabelApperance.HorizontalAlign = value; }
        }

        /// <summary>
        /// Вертикальное выравнивание
        /// </summary>
        [Category("Подписи")]
        [Description("Вертикальное выравнивание")]
        [DisplayName("Вертикальное выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarVerticalConverter))]
        [Browsable(true)]
        public StringAlignment VerticalAlign
        {
            get { return chart3DLabelApperance.VerticalAlign; }
            set { chart3DLabelApperance.VerticalAlign = value; }
        }

        #endregion

        public HierarchicalChart3DLabelsApperanceBrowseClass(HierarchicalChart3DLabelsAppearance chart3DLabelApperance)
        {
            this.chart3DLabelApperance = chart3DLabelApperance;

            this.chart3DLabelFormat = new ChartFormatBrowseClass(chart3DLabelApperance.FormatString, 
                                                                    ChartFormatBrowseClass.LabelType.AxisLabel,
                                                                    chart3DLabelApperance.ChartComponent);

            this.Chart3DLabelFormat.FormatChanged += new ValueFormatEventHandler(Chart3DLabelFormat_FormatChanged);
        }

        private void Chart3DLabelFormat_FormatChanged()
        {
            /*if (chart3DLabelFormat.FormatType == FormatType.Auto)
            {
                // chart3DLabelApperance.FormatString = "<ITEM_LABEL>";
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(HierarchicalChart3DLabelsAppearance));
                pdc["FormatString"].ResetValue(chart3DLabelApperance);

            }
            else*/
            {
                chart3DLabelApperance.FormatString = chart3DLabelFormat.FormatString;
            }
        }

        public override string ToString()
        {
            return FormatString;
        }
    }
}