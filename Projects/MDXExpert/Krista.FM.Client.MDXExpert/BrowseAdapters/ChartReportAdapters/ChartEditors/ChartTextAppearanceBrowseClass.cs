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
    /// ������� ������
    /// </summary>
    public class ChartAppearanceBrowseClass : FilterablePropertyBase
    {
        #region ����

        private ChartTextAppearance appearance;
        private IChartComponent chart;
        private ChartFormatBrowseClass itemLabelFormat;

        #endregion

        #region ��������

        /// <summary>
        /// ��� ���������
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
        }


        /// <summary>
        /// �������� �� ������������ bar �����
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
        /// �������� �� ������������ column �����
        /// </summary>
        [Browsable(false)]
        public bool ColumnAlignEnable
        {
            get
            {
                return !BarAlignEnable;
            }
        }

        [DisplayName("�����")]
        [Description("����� �������")]
        [Category("�������")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Arial, 7pt")]
        [Browsable(true)]
        public Font ChartTextFont
        {
            get { return appearance.ChartTextFont; }
            set { appearance.ChartTextFont = value; }
        }

        [DisplayName("���� ������")]
        [Description("���� ������ �������")]
        [Category("�������")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return appearance.FontColor; }
            set { appearance.FontColor = value; }
        }

        [DisplayName("���������")]
        [Description("����� ���������")]
        [Category("�������")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Column
        {
            get { return appearance.Column; }
            set { appearance.Column = value; }
        }

        [DisplayName("���")]
        [Description("����� ����")]
        [Category("�������")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Row
        {
            get { return appearance.Row; }
            set { appearance.Row = value; }
        }

        [DisplayName("�������������� ������������")]
        [Description("�������������� ������������ �������")]
        [Category("�������")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [DynamicPropertyFilter("BarAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment HorizontalBarAlign
        {
            get { return appearance.HorizontalAlign; }
            set { appearance.HorizontalAlign = value; }
        }

        [DisplayName("������������ ������������")]
        [Description("������������ ������������ �������")]
        [Category("�������")]
        [TypeConverter(typeof(StringAlignmentBarVerticalConverter))]
        [DynamicPropertyFilter("BarAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalBarAlign
        {
            get { return appearance.VerticalAlign; }
            set { appearance.VerticalAlign = value; }
        }

        [DisplayName("�������������� ������������")]
        [Description("�������������� ������������ �������")]
        [Category("�������")]
        [TypeConverter(typeof(StringAlignmentColumnHorizontalConverter))]
        [DynamicPropertyFilter("ColumnAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment HorizontalColumnAlign
        {
            get { return appearance.HorizontalAlign; }
            set { appearance.HorizontalAlign = value; }
        }

        [DisplayName("������������ ������������")]
        [Description("������������ ������������ �������")]
        [Category("�������")]
        [TypeConverter(typeof(StringAlignmentColumnVerticalConverter))]
        [DynamicPropertyFilter("ColumnAlignEnable", "True")]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalColumnAlign
        {
            get { return appearance.VerticalAlign; }
            set { appearance.VerticalAlign = value; }
        }

        [DisplayName("������ (������)")]
        [Description("������ ������� �������")]
        [Category("�������")]
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

        [Description("������ �������")]
        [DisplayName("������")]
        [Category("�������")]
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

        [Category("�������")]
        [DisplayName("������ (������)")]
        [Description("������ ������� �������")]
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

        [DisplayName("���������� �� ������")]
        [Description("���������� �� ������")]
        [Category("�������")]
        [DynamicPropertyFilter("ChartType", "PieChart, DoughnutChart")]
        [DefaultValue(50)]
        [Browsable(true)]
        public int PositionFromRadius
        {
            get { return appearance.PositionFromRadius; }
            set { appearance.PositionFromRadius = value; }
        }

        [DisplayName("����������")]
        [Description("���������� �������")]
        [Category("�������")]
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
