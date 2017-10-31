using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColumnChartBrowseClass
    {
        #region ����

        private ColumnChartAppearance columnChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ����������� ����� �����������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ����� �����������")]
        [DisplayName("����������� ����� �����������")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int ColumnSpacing
        {
            get { return columnChartAppearance.ColumnSpacing; }
            set { columnChartAppearance.ColumnSpacing = value; }
        }

        /// <summary>
        /// ����������� ����� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ����� ������")]
        [DisplayName("����������� ����� ������")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int SeriesSpacing
        {
            get { return columnChartAppearance.SeriesSpacing; }
            set { columnChartAppearance.SeriesSpacing = value; }
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
            get { return columnChartAppearance.NullHandling; }
            set { columnChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ������")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [DisplayName("������� ������")]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return columnChartAppearance.ChartText; }
        }

        #endregion

        public ColumnChartBrowseClass(ColumnChartAppearance columnChartAppearance)
        {
            this.columnChartAppearance = columnChartAppearance;
        }

        public override string ToString()
        {
            return ColumnSpacing + "; " + SeriesSpacing;
        }
    }
}