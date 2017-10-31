using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ����� � ��������� HistogramChart
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HistogramLineBrowseClass
    {
        #region ����

        private HistogramLineAppearance histogramLineApperance;
        private PaintElementBrowseClass paintElementBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("�����")]
        [Description("����� �����")]
        [DisplayName("�����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return histogramLineApperance.DrawStyle; }
            set { histogramLineApperance.DrawStyle = value; }
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        [Category("�����")]
        [Description("������ �����")]
        [DisplayName("������ �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return histogramLineApperance.StartStyle; }
            set { histogramLineApperance.StartStyle = value; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("�����")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return histogramLineApperance.EndStyle; }
            set { histogramLineApperance.EndStyle = value; }
        }

        /// <summary>
        /// ����������� ������� ��� ������
        /// </summary>
        [Category("�����")]
        [Description("����������� ������� ��� ������")]
        [DisplayName("�������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool FillArea
        {
            get { return histogramLineApperance.FillArea; }
            set { histogramLineApperance.FillArea = value; }
        }

        /// <summary>
        /// ������ ����� �����
        /// </summary>
        [Category("�����")]
        [Description("������ ����� �����")]
        [DisplayName("������ �����")]
        [Browsable(true)]
        public string LineLabel
        {
            get { return histogramLineApperance.LineLabel; }
            set { histogramLineApperance.LineLabel = value; }
        }

        /// <summary>
        /// ����������� ����� � ������� ���������
        /// </summary>
        [Category("�������")]
        [Description("����������� ����� � ������� ���������")]
        [DisplayName("����������� � �������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ShowInLegend
        {
            get { return histogramLineApperance.ShowInLegend; }
            set { histogramLineApperance.ShowInLegend = value; }
        }

        /// <summary>
        /// ��������� ����� � ���������
        /// </summary>
        [Category("�������")]
        [Description("���������� ����� � ���������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return histogramLineApperance.Visible; }
            set { histogramLineApperance.Visible = value; }
        }

        /// <summary>
        /// ����� �������� ����������� � ���������
        /// </summary>
        [Category("�������")]
        [Description("����� �������� ����������� � ���������")]
        [DisplayName("����� �������� �����������")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        #endregion

        public HistogramLineBrowseClass(HistogramLineAppearance histogramLineApperance)
        {
            this.histogramLineApperance = histogramLineApperance;
            paintElementBrowse = new PaintElementBrowseClass(histogramLineApperance.PE);
        }

        public override string ToString()
        {
            return LineLabel + "; " + LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + BooleanTypeConverter.ToString(Visible);
        }
    }
}