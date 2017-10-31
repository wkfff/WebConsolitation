using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RadarChartBrowseClass
    {
        #region ����

        private RadarChartAppearance radarChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("�����")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle LineDrawStyle
        {
            get { return radarChartAppearance.LineDrawStyle; }
            set { radarChartAppearance.LineDrawStyle = value; }
        }

        /// <summary>
        /// ������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������� �����")]
        [DisplayName("������� �����")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int LineThickness
        {
            get { return radarChartAppearance.LineThickness; }
            set { radarChartAppearance.LineThickness = value; }
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� �����")]
        [DisplayName("��������� �����")]
        [Browsable(true)]
        public float SplineTension
        {
            get { return radarChartAppearance.SplineTension; }
            set { radarChartAppearance.SplineTension = value; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [Browsable(true)]
        public LineCapStyle LineEndCapStyle
        {
            get { return radarChartAppearance.LineEndCapStyle; }
            set { radarChartAppearance.LineEndCapStyle = value; }
        }

        /// <summary>
        /// ����������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� �����")]
        [DisplayName("����������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return radarChartAppearance.MidPointAnchors; }
            set { radarChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// ������� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� �������")]
        [DisplayName("������� �������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ColorFill
        {
            get { return radarChartAppearance.ColorFill; }
            set { radarChartAppearance.ColorFill = value; }
        }

        /// <summary>
        /// ���������� ������ ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ������ ���������")]
        [DisplayName("���������� ������ ���������")]
        [Browsable(true)]
        public int SpacingAroundChart
        {
            get { return radarChartAppearance.SpacingAroundChart; }
            set { radarChartAppearance.SpacingAroundChart = value; }
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
            get { return radarChartAppearance.NullHandling; }
            set { radarChartAppearance.NullHandling = value; }
        }

        #endregion

        public RadarChartBrowseClass(RadarChartAppearance radarChartAppearance)
        {
            this.radarChartAppearance = radarChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(LineDrawStyle) + "; " + LineThickness + ": " + LineCapStyleTypeConverter.ToString(LineEndCapStyle);
        }
    }
}