using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineAreaChartBrowseClass
    {
        #region ����

        private SplineAreaChartAppearance splineAreaChartAppearance;

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
        public LineDrawStyle LineDrawStyle
        {
            get { return splineAreaChartAppearance.LineDrawStyle; }
            set { splineAreaChartAppearance.LineDrawStyle = value; }
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
        public LineCapStyle LineStartCapStyle
        {
            get { return splineAreaChartAppearance.LineStartCapStyle; }
            set { splineAreaChartAppearance.LineStartCapStyle = value; }
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
        public LineCapStyle LineEndStyle
        {
            get { return splineAreaChartAppearance.LineEndCapStyle; }
            set { splineAreaChartAppearance.LineEndCapStyle = value; }
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
            get { return splineAreaChartAppearance.MidPointAnchors; }
            set { splineAreaChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// ������� ����� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ����� ���������")]
        [DisplayName("������� �����")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int LineThickness
        {
            get { return splineAreaChartAppearance.LineThickness; }
            set { splineAreaChartAppearance.LineThickness = value; }
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
            get { return splineAreaChartAppearance.SplineTension; }
            set { splineAreaChartAppearance.SplineTension = value; }
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
            get { return splineAreaChartAppearance.NullHandling; }
            set { splineAreaChartAppearance.NullHandling = value; }
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
            get { return splineAreaChartAppearance.ChartText; }
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
            get { return splineAreaChartAppearance.EmptyStyles; }
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
            get { return splineAreaChartAppearance.LineAppearances; }
        }

        #endregion

        public SplineAreaChartBrowseClass(SplineAreaChartAppearance splineAreaChartAppearance)
        {
            this.splineAreaChartAppearance = splineAreaChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(LineDrawStyle) + ";" + LineThickness + ";" + BooleanTypeConverter.ToString(MidPointAnchors);
        }
    }
}