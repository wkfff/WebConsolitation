using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ParetoChartBrowseClass
    {
        #region ����

        private ParetoChartAppearance paretoChartAppearance;
        private ParetoLineBrowseClass paretoLineBrowse;
        private PaintElementBrowseClass paintElementBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ���������� ����� ��������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ����� ��������� ���������")]
        [DisplayName("���������� ����� ���������")]
        [DefaultValue(typeof(double), "0.5")]
        [Browsable(true)]
        public double ColumnSpacing
        {
            get { return paretoChartAppearance.ColumnSpacing; }
            set { paretoChartAppearance.ColumnSpacing = value; }
        }

        /// <summary>
        /// ����� ����� � �������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� � ������� ���������")]
        [DisplayName("����� ����� � �������")]
        [DefaultValue("Running Total")]
        [Browsable(true)]
        public string LineLabel
        {
            get { return paretoChartAppearance.LineLabel; }
            set { paretoChartAppearance.LineLabel = value; }
        }

        /// <summary>
        /// ��������� ����� � ������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ����� � ������� ���������")]
        [DisplayName("���������� ����� � �������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowLineInLegend
        {
            get { return paretoChartAppearance.ShowLineInLegend; }
            set { paretoChartAppearance.ShowLineInLegend = value; }
        }

        /// <summary>
        /// ����� ����� � ������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� ���������")]
        [DisplayName("����� �����")]
        [Browsable(true)]
        public ParetoLineBrowseClass ParetoLineBrowse
        {
            get { return paretoLineBrowse; }
            set { paretoLineBrowse = value; }
        }

        /// <summary>
        /// ����� �������� ����������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �������� ����������� ���������")]
        [DisplayName("����� �������� �����������")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
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
            get { return paretoChartAppearance.NullHandling; }
            set { paretoChartAppearance.NullHandling = value; }
        }

        #endregion

        public ParetoChartBrowseClass(ParetoChartAppearance paretoChartAppearance)
        {
            this.paretoChartAppearance = paretoChartAppearance;
            paretoLineBrowse = new ParetoLineBrowseClass(paretoChartAppearance.LineStyle);
            paintElementBrowse = new PaintElementBrowseClass(paretoChartAppearance.LinePE);
        }

        public override string ToString()
        {
            return ColumnSpacing + "; " + LineLabel;
        }
    }
}