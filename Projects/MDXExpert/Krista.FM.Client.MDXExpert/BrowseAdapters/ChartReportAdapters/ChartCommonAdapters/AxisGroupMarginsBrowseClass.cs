using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ���� ���
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisGroupMarginsBrowseClass
    {
        #region ���� 

        private AxisMarginsBrowseClass axisNearMarginsBrowse;
        private AxisMarginsBrowseClass axisFarMarginsBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ������� ���� ���
        /// </summary>
        [Category("���")]
        [Description("������� ���� ���")]
        [DisplayName("������� ����")]
        [Browsable(true)]
        public AxisMarginsBrowseClass AxisNearMarginsBrowse
        {
            get { return axisNearMarginsBrowse; }
            set { axisNearMarginsBrowse = value; }
        }

        /// <summary>
        /// ������� ���� ���
        /// </summary>
        [Category("���")]
        [Description("������� ���� ���")]
        [DisplayName("������� ����")]
        [Browsable(true)]
        public AxisMarginsBrowseClass AxisFarMarginsBrowse
        {
            get { return axisFarMarginsBrowse; }
            set { axisFarMarginsBrowse = value; }
        }

        #endregion

        public AxisGroupMarginsBrowseClass(AxisAppearance axisAppearance)
        {
            axisNearMarginsBrowse = new AxisMarginsBrowseClass(axisAppearance.Margin.Near);
            axisFarMarginsBrowse = new AxisMarginsBrowseClass(axisAppearance.Margin.Far);
        }

        public override string ToString()
        {
            return axisNearMarginsBrowse.Value + "; " + axisFarMarginsBrowse.Value;
        }
    }
}
