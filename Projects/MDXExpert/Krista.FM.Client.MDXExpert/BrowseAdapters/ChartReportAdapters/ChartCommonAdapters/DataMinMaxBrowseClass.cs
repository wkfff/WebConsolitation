using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ����������� �� ������������ � ������������� ���������
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataMinMaxBrowseClass
    {
        #region ����

        private DataAppearance dataAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ������������ ��������
        /// </summary>
        [Category("���")]
        [Description("������������ ��������. �����������, ����� �������� �������� \"�����������\"")]
        [DisplayName("������������ ��������")]
        [DefaultValue(1.7976931348623157E+308)]
        [Browsable(true)]
        public double MaxValue
        {
            get { return dataAppearance.MaxValue; }
            set { dataAppearance.MaxValue = value; }
        }

        /// <summary>
        /// ����������� ��������
        /// </summary>
        [Category("���")]
        [Description("����������� ��������. �����������, ����� �������� �������� \"�����������\"")]
        [DisplayName("����������� ��������")]
        [DefaultValue(-1.7976931348623157E+308)]
        [Browsable(true)]
        public double MinValue
        {
            get { return dataAppearance.MinValue; }
            set { dataAppearance.MinValue = value; }
        }

        /// <summary>
        /// ���� �����������
        /// </summary>
        [Category("���")]
        [Description("����������� �� ������������ � ������������� ���������")]
        [DisplayName("���� �����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool UseMinMax
        {
            get { return dataAppearance.UseMinMax; }
            set { dataAppearance.UseMinMax = value; }
        }

        #endregion

        public DataMinMaxBrowseClass(DataAppearance dataAppearance)
        {
            this.dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(UseMinMax) + "; " + MinValue + "; " + MaxValue;
        }
    }
}
