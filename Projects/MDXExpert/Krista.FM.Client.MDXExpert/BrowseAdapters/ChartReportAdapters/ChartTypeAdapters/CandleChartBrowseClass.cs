using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CandleChartBrowseClass
    {
        #region ����

        private CandleChartAppearance candleChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ��������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ������")]
        [DisplayName("���������� ������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool HighLowVisible
        {
            get { return candleChartAppearance.HighLowVisible; }
            set { candleChartAppearance.HighLowVisible = value; }
        }

        /// <summary>
        /// ���� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("���� ������")]
        [DisplayName("���� ������")]
        [DefaultValue(typeof(Color), "Blue")]
        [Browsable(true)]
        public Color WickColor
        {
            get { return candleChartAppearance.WickColor; }
            set { candleChartAppearance.WickColor = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ������")]
        [DisplayName("������� ������")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int WickThickness
        {
            get { return candleChartAppearance.WickThickness; }
            set { candleChartAppearance.WickThickness = value; }
        }

        /// <summary>
        /// ������ �� �������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������ �� �������� �����")]
        [DisplayName("������ �� �������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool WicksInForeground
        {
            get { return candleChartAppearance.WicksInForeground; }
            set { candleChartAppearance.WicksInForeground = value; }
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ����")]
        [DisplayName("���������� ����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool VolumeVisible
        {
            get { return candleChartAppearance.VolumeVisible; }
            set { candleChartAppearance.HighLowVisible = value; }
        }

        /// <summary>
        /// ���� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("���� �����")]
        [DisplayName("���� �����")]
        [DefaultValue(typeof(Color), "Beige")]
        [Browsable(true)]
        public Color VolumeColor
        {
            get { return candleChartAppearance.VolumeColor; }
            set { candleChartAppearance.VolumeColor = value; }
        }

        /// <summary>
        /// ���� ������������� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("���� ������������� �������")]
        [DisplayName("���� ������������� �������")]
        [DefaultValue(typeof(Color), "White")]
        [Browsable(true)]
        public Color PositiveRangeColor
        {
            get { return candleChartAppearance.PositiveRangeColor; }
            set { candleChartAppearance.PositiveRangeColor = value; }
        }

        /// <summary>
        /// ���� ������������� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("���� ������������� �������")]
        [DisplayName("���� ������������� �������")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color NegativeRangeColor
        {
            get { return candleChartAppearance.NegativeRangeColor; }
            set { candleChartAppearance.NegativeRangeColor = value; }
        }

        /// <summary>
        /// ��������� �������� � �������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� �������� � �������� ������ ���������")]
        [DisplayName("��������� �������� � �������� ������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool OpenCloseVisible
        {
            get { return candleChartAppearance.OpenCloseVisible; }
            set { candleChartAppearance.OpenCloseVisible = value; }
        }

        /// <summary>
        /// ����� ��������� ����� ����� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ��������� ����� ����� ������")]
        [DisplayName("����� ���������")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int SkipN
        {
            get { return candleChartAppearance.SkipN; }
            set { candleChartAppearance.SkipN = value; }
        }

        #endregion

        public CandleChartBrowseClass(CandleChartAppearance candleChartAppearance)
        {
            this.candleChartAppearance = candleChartAppearance;
        }

        public override string ToString()
        {
            return WickColor.Name + "; " + PositiveRangeColor.Name + "; " + NegativeRangeColor.Name;
        }
    }
}