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
    public class DataRowLabelsColumnBrowseClass
    {
        #region ����

        private DataAppearance dataAppearance;

        #endregion

        #region ��������
        
        /// <summary>
        /// ���� ����������� ���������
        /// </summary>
        [Category("���")]
        [Description("���� ����������� ���������")]
        [DisplayName("���� ����������� ���������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool UseRowLabelsColumn
        {
            get { return dataAppearance.UseRowLabelsColumn; }
            set { dataAppearance.UseRowLabelsColumn = value; }
        }

        /// <summary>
        /// ������ ����������� ���������
        /// </summary>
        [Category("���")]
        [Description("������ ����������� ���������")]
        [DisplayName("������ ����������� ���������")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int RowLabelsColumn
        {
            get { return dataAppearance.RowLabelsColumn; }
            set { dataAppearance.RowLabelsColumn = value; }
        }

        #endregion

        public DataRowLabelsColumnBrowseClass(DataAppearance dataAppearance)
        {
            this.dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(UseRowLabelsColumn) + "; " + RowLabelsColumn;
        }
    }
}
