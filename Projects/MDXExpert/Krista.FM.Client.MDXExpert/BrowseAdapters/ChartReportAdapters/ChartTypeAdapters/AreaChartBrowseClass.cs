using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AreaChartBrowseClass
    {
        #region ����

        private AreaChartAppearance areaChartAppearanece;
        // private ChartTextCollectionBrowseClass areaChartTextCollection;

        #endregion

        #region ��������

        /// <summary>
        /// ������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������� �����")]
        [DisplayName("������� �����")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(2)]
        [Browsable(true)]
        public int LineThickness
        {
            get { return areaChartAppearanece.LineThickness; }
            set { areaChartAppearanece.LineThickness = value; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle LineDrawStyle
        {
            get { return areaChartAppearanece.LineDrawStyle; }
            set { areaChartAppearanece.LineDrawStyle = value; }
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������ �����")]
        [DisplayName("������ �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle LineStartCapStyle
        {
            get { return areaChartAppearanece.LineStartCapStyle; }
            set { areaChartAppearanece.LineStartCapStyle = value; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.DiamondAnchor)]
        [Browsable(true)]
        public LineCapStyle LineEndCapStyle
        {
            get { return areaChartAppearanece.LineEndCapStyle; }
            set { areaChartAppearanece.LineEndCapStyle = value; }
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
            get { return areaChartAppearanece.MidPointAnchors; }
            set { areaChartAppearanece.MidPointAnchors = value; }
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
            get { return areaChartAppearanece.NullHandling; }
            set { areaChartAppearanece.NullHandling = value; }
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
            get { return areaChartAppearanece.ChartText; }
        }

        /// <summary>
        /// ����� ����������� ������ ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����������� ������ ��������")]
        [DisplayName("����� ����������� ������ ��������")]
        [Editor(typeof(EmptyAppearanceCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public EmptyAppearanceCollection EmptyStyles
        {
            get { return areaChartAppearanece.EmptyStyles; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Editor(typeof(LineAppearanceCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public LineAppearanceCollection LineAppearances
        {
            get { return areaChartAppearanece.LineAppearances; }
        }

        #endregion

        public AreaChartBrowseClass(AreaChartAppearance areaChartAppearanece)
        {
            this.areaChartAppearanece = areaChartAppearanece;
            //  this.areaChartTextCollection = new ChartTextCollectionBrowseClass(areaChartAppearanece.ChartComponent);
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(LineDrawStyle) + "; " + LineThickness;
        }
    }
}