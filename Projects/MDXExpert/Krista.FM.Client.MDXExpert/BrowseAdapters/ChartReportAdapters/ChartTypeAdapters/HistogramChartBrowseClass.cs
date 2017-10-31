using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HistogramChartBrowseClass
    {
        #region ����

        private HistogramColumnBrowseClass histogramColumnBrowse;
        private HistogramLineBrowseClass histogramLineBrowse;
        private HistogramChartAppearance histogramChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// �������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������� � ���������")]
        [DisplayName("�������")]
        [Browsable(true)]
        public HistogramColumnBrowseClass HistogramColumnBrowse
        {
            get { return histogramColumnBrowse; }
            set { histogramColumnBrowse = value; }
        }

        /// <summary>
        /// �����
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ����� � ���������")]
        [DisplayName("�����")]
        [Browsable(true)]
        public HistogramLineBrowseClass HistogramLineBrowse
        {
            get { return histogramLineBrowse; }
            set { histogramLineBrowse = value; }
        }

        /// <summary>
        /// ������ ������� ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ������� ���������")]
        [DisplayName("������ �������")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int ColumnIndex
        {
            get { return histogramChartAppearance.ColumnIndex; }
            set { histogramChartAppearance.ColumnIndex = value; }
        }

        #endregion

        public HistogramChartBrowseClass(HistogramChartAppearance histogramChartAppearance)
        {
            this.histogramChartAppearance = histogramChartAppearance;
            histogramColumnBrowse = new HistogramColumnBrowseClass(histogramChartAppearance.ColumnAppearance);
            histogramLineBrowse = new HistogramLineBrowseClass(histogramChartAppearance.LineAppearance);
        }

        public override string ToString()
        {
            return HistogramLineBrowse + "; " + HistogramColumnBrowse;
        }
    }
}