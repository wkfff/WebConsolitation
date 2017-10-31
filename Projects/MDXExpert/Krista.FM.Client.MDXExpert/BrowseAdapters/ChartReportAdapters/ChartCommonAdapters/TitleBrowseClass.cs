using System.ComponentModel;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// �������
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TitleBrowseClass
    {
        #region ����

        private TitleSideBrowseClass titleLeftBrowse;
        private TitleSideBrowseClass titleRightBrowse;
        private TitleSideBrowseClass titleTopBrowse;
        private TitleSideBrowseClass titleBottomBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ������� �� ������ ����
        /// </summary>
        [Category("�������")]
        [Description("������� �� ������ ����")]
        [DisplayName("������� �� ������ ����")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleLeftBrowse
        {
            get { return titleLeftBrowse; }
            set { titleLeftBrowse = value; }
        }

        /// <summary>
        /// ������� �� ������� ����
        /// </summary>
        [Category("�������")]
        [Description("������� �� ������� ����")]
        [DisplayName("������� �� ������� ����")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleRightBrowse
        {
            get { return titleRightBrowse; }
            set { titleRightBrowse = value; }
        }

        /// <summary>
        /// ������� �� �������� ����
        /// </summary>
        [Category("�������")]
        [Description("������� �� �������� ����")]
        [DisplayName("������� �� �������� ����")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleTopBrowse
        {
            get { return titleTopBrowse; }
            set { titleTopBrowse = value; }
        }

        /// <summary>
        /// ������� �� ������� ����
        /// </summary>
        [Category("�������")]
        [Description("������� �� ������� ����")]
        [DisplayName("������� �� ������� ����")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleBottomBrowse
        {
            get { return titleBottomBrowse; }
            set { titleBottomBrowse = value; }
        }

        #endregion

        public TitleBrowseClass(UltraChart ultraChart)
        {
            titleLeftBrowse = new TitleSideBrowseClass(ultraChart.TitleLeft);
            titleRightBrowse = new TitleSideBrowseClass(ultraChart.TitleRight);
            titleTopBrowse = new TitleSideBrowseClass(ultraChart.TitleTop);
            titleBottomBrowse = new TitleSideBrowseClass(ultraChart.TitleBottom);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}