using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ��������������� � ��������� ���
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ScrollScaleBrowseClass : FilterablePropertyBase
    {
        #region ����

        private ScrollScaleAppearance scrollScaleAppearance;
        private AxisAppearance axisAppearance;
        private UltraChart chart;

        #endregion

        #region ��������

        /// <summary>
        /// ����� ���
        /// </summary>
        [Browsable(false)]
        public AxisNumber AxisNumber
        {
            get { return axisAppearance.axisNumber; }
        }

        /// <summary>
        /// ��� ���������
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
        }

        /*/// <summary>
        /// ������ ������ ���������
        /// </summary>
        [Category("���������������")]
        [Description("������ ������ ���������")]
        [DisplayName("������ ������ ���������")]
        [DynamicPropertyFilter("HeightEnable", "True")]
        [DefaultValue(10)]
        [Browsable(true)]
        public int Height
        {
            get { return scrollScaleAppearance.Height; }
            set { scrollScaleAppearance.Height = value; }
        }*/

        /// <summary>
        /// ������ ������ ���������
        /// </summary>
        [Category("���������������")]
        [Description("������ ������ ���������")]
        [DisplayName("������ ������ ���������")]
        [DefaultValue(15)]
        [Browsable(true)]
        public int Width
        {
            get { return scrollScaleAppearance.Width; }
            set { scrollScaleAppearance.Width = value; }
        }

        /// <summary>
        /// ������� ���
        /// </summary>
        [Category("���������������")]
        [Description("������� ���")]
        [DisplayName("�������")]
        [DefaultValue(typeof(double), "1")]
        [Browsable(true)]
        public double Scale
        {
            get { return scrollScaleAppearance.Scale; }
            set { scrollScaleAppearance.Scale = value; }
        }

        /// <summary>
        /// ��������� ���
        /// </summary>
        [Category("���������������")]
        [Description("��������� ���")]
        [DisplayName("���������")]
        [DefaultValue(typeof(double), "0")]
        [Browsable(true)]
        public double Scroll
        {
            get { return scrollScaleAppearance.Scroll; }
            set { scrollScaleAppearance.Scroll = value; }
        }

        /// <summary>
        /// ��������� ������ ���������
        /// </summary>
        [Category("���������������")]
        [Description("���������� ������ ���������")]
        [DisplayName("���������� ������ ���������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Visible
        {
            get { return scrollScaleAppearance.Visible; }
            set { scrollScaleAppearance.Visible = value; }
        }

        #endregion

        public ScrollScaleBrowseClass(ScrollScaleAppearance scrollScaleAppearance, AxisAppearance axisAppearance, UltraChart chart)
        {
            this.scrollScaleAppearance = scrollScaleAppearance;
            this.axisAppearance = axisAppearance;
            this.chart = chart;
        }

        public override string ToString()
        {
            return Scale + "; " + Scroll;
        }
    }
}