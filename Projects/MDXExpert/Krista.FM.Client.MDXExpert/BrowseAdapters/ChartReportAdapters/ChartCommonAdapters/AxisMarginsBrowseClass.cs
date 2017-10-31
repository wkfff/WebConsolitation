using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ���� ���������
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisMarginsBrowseClass
    {
        #region ����

        private AxisMargin axisMargin;

        #endregion

        #region ��������

        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        [Category("����")]
        [Description("������� ��������� ����")]
        [DisplayName("������� ���������")]
        [TypeConverter(typeof(MarginLocationTypeConvterter))]
        [DefaultValue(LocationType.Percentage)]
        [Browsable(true)]
        public LocationType MarginType
        {
            get { return axisMargin.MarginType; }
            set { axisMargin.MarginType = value; }
        }

        /// <summary>
        /// �������� ����
        /// </summary>
        [Category("����")]
        [Description("�������� ����")]
        [DisplayName("��������")]
        [DefaultValue(typeof(double), "0")]
        [Browsable(true)]
        public double Value
        {
            get { return axisMargin.Value; }
            set { axisMargin.Value = value; }
        }

        #endregion

        public AxisMarginsBrowseClass(AxisMargin axisMargin)
        {
            this.axisMargin = axisMargin;
        }

        public override string ToString()
        {
            return Value + "; " + MarginLocationTypeConvterter.ToString(MarginType);
        }
    }
}