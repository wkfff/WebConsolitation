using System;
using System.Runtime.InteropServices;
using System.Data;
using System.Reflection;
using ADODB;

namespace Krista.FM.PlaningProviderCOMWrapper
{
    #region �������������� System.DataTable � ADODB.Recordset
    [ComVisible(false)]
    public struct DataTableConverter
    {
        /// <summary>
        /// ������������� ��� ������ .NET � ��� ������ ADODB
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private static ADODB.DataTypeEnum TranslateType(Type columnType)
        {
            switch (columnType.UnderlyingSystemType.ToString())
            {
                case "System.Boolean":
                    return ADODB.DataTypeEnum.adBoolean;

                case "System.Byte":
                    return ADODB.DataTypeEnum.adUnsignedTinyInt;

                case "System.Char":
                    return ADODB.DataTypeEnum.adChar;

                case "System.DateTime":
                    return ADODB.DataTypeEnum.adDate;

                case "System.Decimal":
                    return ADODB.DataTypeEnum.adDouble;

                case "System.Double":
                    return ADODB.DataTypeEnum.adDouble;

                case "System.Int16":
                    return ADODB.DataTypeEnum.adSmallInt;

                case "System.Int32":
                    return ADODB.DataTypeEnum.adInteger;

                case "System.Int64":
                    return ADODB.DataTypeEnum.adBigInt;

                case "System.SByte":
                    return ADODB.DataTypeEnum.adTinyInt;

                case "System.Single":
                    return ADODB.DataTypeEnum.adSingle;

                case "System.UInt16":
                    return ADODB.DataTypeEnum.adUnsignedSmallInt;

                case "System.UInt32":
                    return ADODB.DataTypeEnum.adUnsignedInt;

                case "System.UInt64":
                    return ADODB.DataTypeEnum.adUnsignedBigInt;

                case "System.String":
                default:
                    return ADODB.DataTypeEnum.adVarChar;
            }
        }

        /// <summary>
        /// ������������� DataTable � ADODB.RecordSet
        /// </summary>
        /// <param name="inTable">DataTable  � ��������� �������</param>
        /// <returns>ADODB.RecordSet</returns>
        static public ADODB.Recordset ConvertToRecordset(ref DataTable inTable)
        {
            // ������� ����� ������ ADODB.Recordset
            ADODB.Recordset result = new Recordset();
            result.CursorLocation = ADODB.CursorLocationEnum.adUseClient;

            ADODB.Fields resultFields = result.Fields;
            DataColumnCollection inColumns = inTable.Columns;
            // ������� � ��� ����
            for (int i = 0; i <= inColumns.Count - 1; i++)
            {
                // ��� ���� "Code" ������ ������� ��������� ������ � ����������
                resultFields.Append(inColumns[i].ColumnName, string.Compare(inColumns[i].ColumnName, "code", true) == 0 ?
                    DataTypeEnum.adVarChar : TranslateType(inColumns[i].DataType),
                    inColumns[i].MaxLength,
                    inColumns[i].AllowDBNull ? ADODB.FieldAttributeEnum.adFldIsNullable :
                                               ADODB.FieldAttributeEnum.adFldUnspecified,
                        null);
            }
            /*
                foreach (DataColumn inColumn in inColumns)
                {
                    resultFields.Append(inColumn.ColumnName,
                        TranslateType(inColumn.DataType),
                        inColumn.MaxLength,
                        inColumn.AllowDBNull ? ADODB.FieldAttributeEnum.adFldIsNullable :
                                               ADODB.FieldAttributeEnum.adFldUnspecified,
                        null);
                }*/

            result.Open(Missing.Value,
                Missing.Value,
                ADODB.CursorTypeEnum.adOpenStatic,
                ADODB.LockTypeEnum.adLockOptimistic,
                0);
            // ��������� ������ �� DataTable � RecordSet
            foreach (DataRow dr in inTable.Rows)
            {
                result.AddNew(Missing.Value, Missing.Value);
                for (int columnIndex = 0; columnIndex < inColumns.Count; columnIndex++)
                {
                    resultFields[columnIndex].Value = dr[columnIndex];
                }
            }
            return result;
        }
    }
    #endregion
}