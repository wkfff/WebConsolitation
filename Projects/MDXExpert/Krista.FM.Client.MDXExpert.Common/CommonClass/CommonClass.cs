using System;
using System.Collections.Generic;
using System.Text;

//������������ ���� ��� ����� ������������ � ������ � �������
namespace Krista.FM.Client.MDXExpert.CommonClass
{
    /// <summary>
    /// ������������ ������������ ��� ������ ������� ������� � �����
    /// </summary>
    public enum SelectionType
    {
        GeneralArea,
        Columns,
        Rows,
        Filters,
        Totals,
        SingleObject
    }

    /// <summary>
    /// ������� ��������� ������
    /// </summary>
    public enum ReportElementSubType
    {
        /// <summary>
        /// �����������
        /// </summary>
        Standart,
        /// <summary>
        /// ����������������
        /// </summary>
        CustomMDX,
        /// <summary>
        /// �����������
        /// </summary>
        Composite,
        /// <summary>
        /// �� ������ ������������
        /// </summary>
        CustomData,
        /// <summary>
        /// �������������
        /// </summary>
        Multiple

    }
}
