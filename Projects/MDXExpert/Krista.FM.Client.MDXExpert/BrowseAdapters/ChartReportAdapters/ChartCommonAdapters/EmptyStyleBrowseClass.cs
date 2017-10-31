using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EmptyStyleBrowseClass
    {
        #region ����

        private EmptyAppearance emptyAppearance;
        private LineStyleBrowseClass lineStyleBrowse;
        private PaintElementBrowseClass paintElementBrowse;
        private PaintElementBrowseClass pointPaintElementBrowse;
        private PointStyleBrowseClass pointStyleBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ��������� ����� �����
        /// </summary>
        [Description("��������� ����� �����")]
        [DisplayName("��������� ����� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EnableLineStyle
        {
            get { return emptyAppearance.EnableLineStyle; }
            set { emptyAppearance.EnableLineStyle = value; }
        }

        /// <summary>
        /// ��������� �������� �����������
        /// </summary>
        [Description("��������� �������� �����������")]
        [DisplayName("��������� �������� �����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EnablePE
        {
            get { return emptyAppearance.EnablePE; }
            set { emptyAppearance.EnablePE = value; }
        }

        /// <summary>
        /// ��������� ����������� �����
        /// </summary>
        [Description("��������� ����������� �����")]
        [DisplayName("��������� ����������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EnablePoint
        {
            get { return emptyAppearance.EnablePoint; }
            set { emptyAppearance.EnablePoint = value; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Browsable(true)]
        public LineStyleBrowseClass LineStyleBrowse
        {
            get { return lineStyleBrowse; }
            set { lineStyleBrowse = value; }
        }

        /// <summary>
        /// ����� �������� �����������
        /// </summary>
        [Description("����� �������� �����������")]
        [DisplayName("����� �������� �����������")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        /// <summary>
        /// ����� �������� ����������� �����
        /// </summary>
        [Description("����� �������� ����������� �����")]
        [DisplayName("����� �������� ����������� �����")]
        [Browsable(true)]
        public PaintElementBrowseClass PointPaintElementBrowse
        {
            get { return pointPaintElementBrowse; }
            set { pointPaintElementBrowse = value; }
        }

        /// <summary>
        /// ����� �����
        /// </summary>
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Browsable(true)]
        public PointStyleBrowseClass PointStyleBrowse
        {
            get { return pointStyleBrowse; }
            set { pointStyleBrowse = value; }
        }

        /// <summary>
        /// ��� �������� � �������
        /// </summary>
        [Description("��� �������� � �������")]
        [DisplayName("��� �������� � �������")]
        [DefaultValue(LegendEmptyDisplayType.PE)]
        [TypeConverter(typeof(LegendEmptyDisplayTypeConverter))]
        [Browsable(true)]
        public LegendEmptyDisplayType LegendDisplayType
        {
            get { return emptyAppearance.LegendDisplayType; }
            set { emptyAppearance.LegendDisplayType = value; }
        }

        /// <summary>
        /// ����������� � �������
        /// </summary>
        [Description("����������� � �������")]
        [DisplayName("����������� � �������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowInLegend
        {
            get { return emptyAppearance.ShowInLegend; }
            set { emptyAppearance.ShowInLegend = value; }
        }

        /// <summary>
        /// ���������� ����
        /// </summary>
        [Description("���������� ����")]
        [DisplayName("���������� ����")]
        [Browsable(true)]
        public string Key
        {
            get { return emptyAppearance.Key; }
            set { emptyAppearance.Key = value; }
        }

        /// <summary>
        /// ������� ������� ��������
        /// </summary>
        [Description("������� ������� ��������")]
        [DisplayName("������� ������� ��������")]
        [DefaultValue("������")]
        [Browsable(true)]
        public string Text
        {
            get { return emptyAppearance.Text; }
            set { emptyAppearance.Text = value; }
        }

        #endregion

        public EmptyStyleBrowseClass(EmptyAppearance emptyAppearance)
        {
            this.emptyAppearance = emptyAppearance;

            lineStyleBrowse = new LineStyleBrowseClass(emptyAppearance.LineStyle);
            paintElementBrowse = new PaintElementBrowseClass(emptyAppearance.PE);
            pointPaintElementBrowse = new PaintElementBrowseClass(emptyAppearance.PointPE);
            pointStyleBrowse = new PointStyleBrowseClass(emptyAppearance.PointStyle);
            emptyAppearance.Text = "������";
        }

        public override string ToString()
        {
            return Text;
        }
    }
}