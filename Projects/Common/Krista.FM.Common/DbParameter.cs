using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Krista.FM.Common
{
    
    [Serializable()]
    public class DbParameterDescriptor : IDbDataParameter
    {
        private string paramName;
        private object paramValue;
        private DbType dbType;
        private ParameterDirection paramDirection;
        private string sourceColumn;
        private bool isNullable;
        private DataRowVersion dataSourceVersion;
        private byte precision;
        private byte scale;
        private int size;

        public DbParameterDescriptor(string paramName, object paramValue)
        {
            this.paramName = paramName;
            this.paramValue = paramValue;
            Type tp = paramValue.GetType();
            switch (tp.FullName)
            {
                case "System.Boolean":
                    DbType = DbType.Boolean;
                    break;
                case "System.Byte":
                    DbType = DbType.Byte;
                    break;
                case "System.SByte":
                    DbType = DbType.SByte;
                    break;
                case "System.Char":
                    DbType = DbType.String;
                    break;
                case "System.Decimal":
                    DbType = DbType.Decimal;
                    break;
                case "System.Double":
                    // из за проблем с округлением, меняем тип на Decimal
                    DbType = DbType.Decimal;//DbType.Double;
                    break;
                case "System.Single":
                    DbType = DbType.Single;
                    break;
                case "System.Int32":
                    DbType = DbType.Int32;
                    break;
                case "System.UInt32":
                    DbType = DbType.UInt32;
                    break;
                case "System.Int64":
                    DbType = DbType.Int64;
                    break;
                case "System.UInt64":
                    DbType = DbType.UInt64;
                    break;
                case "System.Int16":
                    DbType = DbType.Int16;
                    break;
                case "System.UInt16":
                    DbType = DbType.UInt16;
                    break;
                case "System.String":
                    // ******************************************** 
                    // Тип DbType.String хранит строки в виде UNICODE
                    // и если длина параметра значения больше 2000 элементов 
                    // - при записи данных в базу происходит исключение о превышении размера
                    // очевидно это свзано с какими-то преобразованиями UNICODE -> ANSI
                    // Избавимся от такого поведения, заменив тип DbType.String на DbType.AnsiString
                    // Не следует забывать о том что максимальная длина строки для параметров такого типа 
                    // 8000 символов
                    // ********************************************
                    //parameter.DbType = DbType.String;
                    DbType = DbType.AnsiString;
                    break;
                case "System.DateTime":
                    DbType = DbType.DateTime;
                    break;
                case "System.DBNull":
                    DbType = DbType.AnsiString;
                    break;
                default:
                    throw new InvalidCastException(String.Format("Параметр {0} имеет неизвестный тип значения {1}", paramName, tp));
            }
        }
        
        public DbParameterDescriptor(string paramName, object paramValue, DbType dbType)
            : this(paramName, paramValue)
        {
            this.dbType = dbType;
        }

        public DbParameterDescriptor(string paramName, object paramValue, DbType dbType, ParameterDirection paramDirection)
            : this(paramName, paramValue, dbType)
        {
            this.paramDirection = paramDirection;
        }

        /// <summary>
        /// Имя параметра
        /// </summary>
        public string ParameterName
        {
            get { return paramName; }
            set { paramName = value; }
        }

        /// <summary>
        /// Значение параметра
        /// </summary>
        public object Value
        {
            get { return paramValue; }
            set { paramValue = value; }
        }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public DbType DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        /// <summary>
        /// Указывает тип параметра в запросе
        /// </summary>
        public ParameterDirection Direction
        {
            get { return paramDirection; }
            set { paramDirection = value; }
        }

        public string SourceColumn
        {
            get { return sourceColumn; }
            set { sourceColumn = value; }
        }

        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }

        public DataRowVersion SourceVersion
        {
            get { return dataSourceVersion; }
            set { dataSourceVersion = value; }
        }

        public byte Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public byte Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

    }
}
