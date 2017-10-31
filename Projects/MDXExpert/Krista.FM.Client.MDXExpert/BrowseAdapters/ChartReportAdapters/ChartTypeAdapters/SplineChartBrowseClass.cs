using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineChartBrowseClass
    {
        #region ����

        private SplineChartAppearance splineChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("�����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return splineChartAppearance.DrawStyle; }
            set { splineChartAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// ������ ����� �������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ����� �������� ���������")]
        [DisplayName("������ �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return splineChartAppearance.StartStyle; }
            set { splineChartAppearance.StartStyle = value; }
        }

        /// <summary>
        /// ����� ����� �������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� �������� ���������")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return splineChartAppearance.EndStyle; }
            set { splineChartAppearance.EndStyle = value; }
        }

        /// <summary>
        /// ����������� ����� ����� ��������� ���������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ����� ����� ��������� ���������� ���������")]
        [DisplayName("����������� ������������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return splineChartAppearance.MidPointAnchors; }
            set { splineChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// ������� ����� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ����� ���������")]
        [DisplayName("������� �����")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int Thickness
        {
            get { return splineChartAppearance.Thickness; }
            set { splineChartAppearance.Thickness = value; }
        }

        /// <summary>
        /// ��������� ����� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ����� ���������")]
        [DisplayName("��������� �����")]
        [Browsable(true)]
        public float SplineTension
        {
            get { return splineChartAppearance.SplineTension; }
            set { splineChartAppearance.SplineTension = value; }
        }

        /// <summary>
        /// ����������� ������ ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������ ��������")]
        [DisplayName("������ ��������")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return splineChartAppearance.NullHandling; }
            set { splineChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� �����")]
        [DisplayName("��������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool HighLightLines
        {
            get { return splineChartAppearance.HighLightLines; }
            set { splineChartAppearance.HighLightLines = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ������")]
        [DisplayName("������� ������")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return splineChartAppearance.ChartText; }
        }

        /// <summary>
        /// ����� ����������� ������ ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����������� ������ ��������")]
        [DisplayName("����� ����������� ������ ��������")]
        [Browsable(true)]
        public EmptyAppearanceCollection EmptyStyles
        {
            get { return splineChartAppearance.EmptyStyles; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Browsable(true)]
        public LineAppearanceCollection LineAppearances
        {
            get { return splineChartAppearance.LineAppearances; }
        }

        #endregion

        public SplineChartBrowseClass(SplineChartAppearance splineChartAppearance)
        {
            this.splineChartAppearance = splineChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + ";" + Thickness + ";" + BooleanTypeConverter.ToString(MidPointAnchors);
        }
    }
}