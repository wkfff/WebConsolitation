using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BoxChartBrowseClass
    {
        #region ����

        private BoxChartAppearance boxChartAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ���� ������ �����
        /// </summary>
        [Category("������� ���������")]
        [Description("���� ������ ����� ���������")]
        [DisplayName("���� ������ �����")]
        [DefaultValue(typeof(double), "1")]
        [Browsable(true)]
        public double BoxWidthFactor
        {
            get { return boxChartAppearance.BoxWidthFactor; }
            set { boxChartAppearance.BoxWidthFactor = value; }
        }

        #endregion

        public BoxChartBrowseClass(BoxChartAppearance boxChartAppearance)
        {
            this.boxChartAppearance = boxChartAppearance;
        }

        public override string ToString()
        {
            return BoxWidthFactor.ToString();
        }
    }
}