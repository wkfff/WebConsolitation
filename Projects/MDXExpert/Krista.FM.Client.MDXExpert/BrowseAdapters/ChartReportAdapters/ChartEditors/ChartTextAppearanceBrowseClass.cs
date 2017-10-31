using System;
using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Подпись данных
    /// </summary>
    public class ChartAppearanceBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private ChartTextAppearance appearance;
        private IChartComponent chart;
        private ChartFormatBrowseClass itemLabelFormat;

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
        /// Является ли выравнивание bar типом
        /// </summary>
        [Browsable(false)]
        public bool BarAlignEnable
        {
            get
            {
                return ChartType == ChartType.BarChart || ChartType == ChartType.StackBarChart ||
                       ChartType == ChartType.GanttChart || ChartType == ChartType.FunnelChart ||
                       ChartType == ChartType.PyramidChart;
            }
        }

        /// <summary>
        /// Является ли выравнивание column типом
        /// </summary>
        [Browsable(false)]
        public bool ColumnAlignEnable
        {
            get
            {
                return !BarAlignEnable;
            }
        }

        [DisplayName("Шрифт")]
        [Description("Шрифт подписи")]
        [Category("Подпись")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Arial, 7pt")]
        [Browsable(true)]
        public Font ChartTextFont
        {
            get { return appearance.ChartTextFont; }
            set { appearance.ChartTextFont = value; }
        }

        [DisplayName("Цвет шрифта")]
        [Description("Цвет шрифта подписи")]
        [Category("Подпись")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return appearance.FontColor; }
            set { appearance.FontColor = value; }
        }

        [DisplayName("Категория")]
        [Description("Номер категории")]
        [Category("Подпись")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Column
        {
            get { return appearance.Column; }
            set { appearance.Column = value; }
        }

        [DisplayName("Ряд")]
        [Description("Номер ряда")]
        [Category("Подпись")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Row
        {
            get { return appearance.Row; }
            set { appearance.Row = value; }
        }

        [DisplayName("Горизонтальное выравнивание")]
        [Description("Горизонтальное выравнивание подписи")]
        [Category("Подпись")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [DynamicPropertyFilter("BarAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment HorizontalBarAlign
        {
            get { return appearance.HorizontalAlign; }
            set { appearance.HorizontalAlign = value; }
        }

        [DisplayName("Вертикальное выравнивание")]
        [Description("Вертикальное выравнивание подписи")]
        [Category("Подпись")]
        [TypeConverter(typeof(StringAlignmentBarVerticalConverter))]
        [DynamicPropertyFilter("BarAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalBarAlign
        {
            get { return appearance.VerticalAlign; }
            set { appearance.VerticalAlign = value; }
        }

        [DisplayName("Горизонтальное выравнивание")]
        [Description("Горизонтальное выравнивание подписи")]
        [Category("Подпись")]
        [TypeConverter(typeof(StringAlignmentColumnHorizontalConverter))]
        [DynamicPropertyFilter("ColumnAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment HorizontalColumnAlign
        {
            get { return appearance.HorizontalAlign; }
            set { appearance.HorizontalAlign = value; }
        }

        [DisplayName("Вертикальное выравнивание")]
        [Description("Вертикальное выравнивание подписи")]
        [Category("Подпись")]
        [TypeConverter(typeof(StringAlignmentColumnVerticalConverter))]
        [DynamicPropertyFilter("ColumnAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalColumnAlign
        {
            get { return appearance.VerticalAlign; }
            set { appearance.VerticalAlign = value; }
        }

        [DisplayName("Формат (строка)")]
        [Description("Строка формата подписи")]
        [Category("Подпись")]
        [DefaultValue("<DATA_VALUE:00.##>")]
        [Browsable(true)]
        public String ItemFormatString
        {
            get 
            { 
                return appearance.ItemFormatString; 
            }
            set 
            {
                ItemLabelFormat.FormatString = value;
                appearance.ItemFormatString = value;
            }
        }

        [Description("Формат подписи")]
        [DisplayName("Формат")]
        [Category("Подпись")]
        [Browsable(true)]
        public ChartFormatBrowseClass ItemLabelFormat
        {
            get
            {
                return this.itemLabelFormat;

            }
            set
            {
                this.itemLabelFormat = value;
            }
        }

        [Category("Подпись")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата подписи")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(TooltipFormatPattern.DataValue)]
        [Browsable(true)]
        public TooltipFormatPattern ItemLabelPattern
        {
            get
            {
                return itemLabelFormat.TooltipPattern;
            }
            set
            {
                this.itemLabelFormat.TooltipPattern = value;
            }
        }

        [DisplayName("Расстояние от центра")]
        [Description("Расстояние от центра")]
        [Category("Подпись")]
        [DynamicPropertyFilter("ChartType", "PieChart, DoughnutChart")]
        [DefaultValue(50)]
        [Browsable(true)]
        public int PositionFromRadius
        {
            get { return appearance.PositionFromRadius; }
            set { appearance.PositionFromRadius = value; }
        }

        [DisplayName("Показывать")]
        [Description("Показывать подпись")]
        [Category("Подпись")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Visible
        {
            get { return appearance.Visible; }
            set { appearance.Visible = value; }
        }

        #endregion

        public ChartAppearanceBrowseClass(ChartTextAppearance appearance, IChartComponent chart)
        {
            this.appearance = appearance;
            this.chart = chart;

            itemLabelFormat = new ChartFormatBrowseClass(appearance.ItemFormatString,
                                                                ChartFormatBrowseClass.LabelType.Tooltip,
                                                                chart);
            itemLabelFormat.FormatChanged += new ValueFormatEventHandler(itemLabelFormat_FormatChanged);
        }

        private void itemLabelFormat_FormatChanged()
        {
            appearance.ItemFormatString = itemLabelFormat.FormatString;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(Visible) + "; " + Column + "; " + Row;
        }
    }
}
