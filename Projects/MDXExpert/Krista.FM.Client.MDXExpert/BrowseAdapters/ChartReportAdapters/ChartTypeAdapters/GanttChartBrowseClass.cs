using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GanttChartBrowseClass
    {
        #region ����

        private GanttChartAppearance ganttChartAppearance;
        private PaintElementBrowseClass completePercentagesPE;
        private PaintElementBrowseClass emptyPercentagesPE;
        private LabelStyleBrowseClass labelStyleBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ���������� ����� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ����� �������")]
        [DisplayName("���������� ����� �������")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int SeriesSpacing
        {
            get { return ganttChartAppearance.SeriesSpacing; }
            set { ganttChartAppearance.SeriesSpacing = value; }
        }

        /// <summary>
        /// ���������� ����� ����������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ����� ����������")]
        [DisplayName("���������� ����� ����������")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int ItemSpacing
        {
            get { return ganttChartAppearance.ItemSpacing; }
            set { ganttChartAppearance.ItemSpacing = value; }
        }

        /// <summary>
        /// ���������� ������ ���������� �����������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ������ ���������� ���������")]
        [DisplayName("���������� ������ ���������� ���������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowCompletePercentages
        {
            get { return ganttChartAppearance.ShowCompletePercentages; }
            set { ganttChartAppearance.ShowCompletePercentages = value; }
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� �����")]
        [DisplayName("���������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowLinks
        {
            get { return ganttChartAppearance.ShowLinks; }
            set { ganttChartAppearance.ShowLinks = value; }
        }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ����������")]
        [DisplayName("���������� ����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowOwners
        {
            get { return ganttChartAppearance.ShowOwners; }
            set { ganttChartAppearance.ShowOwners = value; }
        }

        /// <summary>
        /// ������� ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ����� �����")]
        [DisplayName("������� ����� �����")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int LinkLineWidth
        {
            get { return ganttChartAppearance.LinkLineWidth; }
            set { ganttChartAppearance.LinkLineWidth = value; }
        }

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� �����")]
        [DisplayName("����� ����� �����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return ganttChartAppearance.LinkLineStyle.DrawStyle; }
            set { ganttChartAppearance.LinkLineStyle.DrawStyle = value; }
        }

        /// <summary>
        /// ������ ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ����� �����")]
        [DisplayName("������ ����� �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.NoAnchor)]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return ganttChartAppearance.LinkLineStyle.StartStyle; }
            set { ganttChartAppearance.LinkLineStyle.StartStyle = value; }
        }

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� �����")]
        [DisplayName("����� ����� �����")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.ArrowAnchor)]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return ganttChartAppearance.LinkLineStyle.EndStyle; }
            set { ganttChartAppearance.LinkLineStyle.EndStyle = value; }
        }

        /// <summary>
        /// ����������� ������������� ����� �� ����� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������������� ����� �� ����� �����")]
        [DisplayName("����������� ������������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return ganttChartAppearance.LinkLineStyle.MidPointAnchors; }
            set { ganttChartAppearance.LinkLineStyle.MidPointAnchors = value; }
        }

        /// <summary>
        /// ����������� ������� ����������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������� ����������� ���������")]
        [DisplayName("����������� ������� ����������� ���������")]
        [Browsable(true)]
        public PaintElementBrowseClass CompletePercentagesPE
        {
            get { return completePercentagesPE; }
            set { completePercentagesPE = value; }
        }

        /// <summary>
        /// ����������� ������� ����������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������� ����������� ���������")]
        [DisplayName("����������� ������� ����������� ���������")]
        [Browsable(true)]
        public PaintElementBrowseClass EmptyPercentagesPE
        {
            get { return emptyPercentagesPE; }
            set { emptyPercentagesPE = value; }
        }

        /// <summary>
        /// ����� ����� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� ���������")]
        [DisplayName("����� ����� ���������")]
        [Browsable(true)]
        public LabelStyleBrowseClass LabelStyleBrowse
        {
            get { return labelStyleBrowse; }
            set { labelStyleBrowse = value; }
        }

        #endregion

        public GanttChartBrowseClass(GanttChartAppearance ganttChartAppearance)
        {
            this.ganttChartAppearance = ganttChartAppearance;

            completePercentagesPE = new PaintElementBrowseClass(ganttChartAppearance.CompletePercentagesPE);
            emptyPercentagesPE = new PaintElementBrowseClass(ganttChartAppearance.EmptyPercentagesPE);
            labelStyleBrowse = new LabelStyleBrowseClass(ganttChartAppearance.OwnersLabelStyle);
        }

        public override string ToString()
        {
            return SeriesSpacing + "; " + ItemSpacing;
        }
    }
}