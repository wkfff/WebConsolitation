using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LineChartBrowseClass
    {
        #region ����

        private LineChartAppearance lineChartAppearance;

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
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return lineChartAppearance.DrawStyle; }
            set { lineChartAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// ������ ����� �������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ����� �������� ���������")]
        [DisplayName("������ �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return lineChartAppearance.StartStyle; }
            set { lineChartAppearance.StartStyle = value; }
        }

        /// <summary>
        /// ����� ����� �������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� �������� ���������")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return lineChartAppearance.EndStyle; }
            set { lineChartAppearance.EndStyle = value; }
        }

        /// <summary>
        /// ����������� ����� ����� ��������� ���������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ����� ����� ��������� ���������� ���������")]
        [DisplayName("����������� ������������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return lineChartAppearance.MidPointAnchors; }
            set { lineChartAppearance.MidPointAnchors = value; }
        }

        /// <summary>
        /// ������� ����� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ����� ���������")]
        [DisplayName("������� �����")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(3)]
        [Browsable(true)]
        public int Thickness
        {
            get { return lineChartAppearance.Thickness; }
            set { lineChartAppearance.Thickness = value; }
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� �����")]
        [DisplayName("��������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool HighLightLines
        {
            get { return lineChartAppearance.HighLightLines; }
            set { lineChartAppearance.HighLightLines = value; }
        }

        /// <summary>
        /// ����������� ������ ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������ ��������")]
        [DisplayName("������ ��������")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [DefaultValue(NullHandling.Zero)]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return lineChartAppearance.NullHandling; }
            set { lineChartAppearance.NullHandling = value; }
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
            get { return lineChartAppearance.ChartText; }
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
            get { return lineChartAppearance.EmptyStyles; }
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
            get { return lineChartAppearance.LineAppearances; }
        }

        #endregion

        public LineChartBrowseClass(LineChartAppearance lineChartAppearance)
        {
            this.lineChartAppearance = lineChartAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + Thickness + "; " + BooleanTypeConverter.ToString(MidPointAnchors);
        }
    }
}