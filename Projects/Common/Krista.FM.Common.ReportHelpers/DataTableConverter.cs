using System;
using System.Data;
using System.Reflection;

namespace Krista.FM.Common.ReportHelpers
{
    internal class DataTableConverter
    {
        /// <summary>
        /// Преобразовать тип данных .NET в тип данных ADODB
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
                    return ADODB.DataTypeEnum.adCurrency;

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

                default:
                    return ADODB.DataTypeEnum.adVarChar;
            }
        }

        /// <summary>
        /// Преобразовать DataTable в ADODB.RecordSet
        /// </summary>
        /// <param name="inTable">DataTable  с исходными данными</param>
        /// <returns>ADODB.RecordSet</returns>
        static public ADODB.Recordset ConvertToRecordset(DataTable inTable)
        {
            // создаем новый объект ADODB.Recordset
            ADODB.Recordset result = new ADODB.Recordset();
            result.CursorLocation = ADODB.CursorLocationEnum.adUseClient;

            ADODB.Fields resultFields = result.Fields;
            DataColumnCollection inColumns = inTable.Columns;
            // Создаем в нем поля
            foreach (DataColumn inColumn in inColumns)
            {
                resultFields.Append(inColumn.ColumnName,
                    TranslateType(inColumn.DataType),
                    inColumn.MaxLength,
                    inColumn.AllowDBNull ? ADODB.FieldAttributeEnum.adFldIsNullable :
                                           ADODB.FieldAttributeEnum.adFldUnspecified,
                    null);
            }

            result.Open(Missing.Value,
                Missing.Value,
                ADODB.CursorTypeEnum.adOpenStatic,
                ADODB.LockTypeEnum.adLockOptimistic,
                0);
            // Переносим данные из DataTable в RecordSet
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
}
