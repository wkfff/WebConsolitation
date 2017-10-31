using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Krista.FM.Providers.OracleDataAccess
{
    public class OracleDbParameter : DbParameter
    {
        private OracleParameter parameter = null;
        public OracleParameter OracleParameter
        {
            get { return parameter; }
        }

        public OracleDbParameter()
        {
            parameter = new OracleParameter();
        }

        public OracleDbParameter(string parameterName, object obj)
        {
            parameter = new OracleParameter(parameterName, obj);
        }

        public OracleDbParameter(string parameterName, DbType type, int size)
        {
            parameter = new OracleParameter();
            this.ParameterName = parameterName;
            this.DbType = type;
            this.Size = size;
        }

        public OracleDbParameter(string parameterName, DbType type, ParameterDirection direction)
        {
            parameter = new OracleParameter();
            this.ParameterName = parameterName;
            this.DbType = type;
            this.Direction = direction;
        }

        public OracleDbParameter(string parameterName, DbType type, int size, string strColumn)
        {
            parameter = new OracleParameter();
            this.ParameterName = parameterName;
            this.DbType = type;
            this.Size = size;
            this.SourceColumn = strColumn;
        }

        public OracleDbParameter(string parameterName, DbType type, int size, object obj, ParameterDirection direction)
        {
            parameter = new OracleParameter(parameterName, obj);
            this.DbType = type;
            this.Size = size;
            this.Direction = direction;
        }

        public OracleDbParameter(string parameterName, DbType type, int size, ParameterDirection direction, bool isNullable, byte precision)
        {
            parameter = new OracleParameter();
            this.ParameterName = parameterName;
            this.DbType = type;
            this.Size = size;
            this.Direction = direction;
            this.IsNullable = isNullable;
            parameter.Precision = precision;
        }

        public OracleDbParameter(OracleParameter parameter)
        {
            this.parameter = parameter;
        }

        public OracleDbParameter(IDbDataParameter parameter)
        {
            this.parameter = new OracleParameter();
            this.parameter.DbType = parameter.DbType;
            this.parameter.Direction = parameter.Direction;
            this.parameter.ParameterName = parameter.ParameterName;
            this.parameter.Size = parameter.Size;
            this.parameter.SourceColumn = parameter.SourceColumn;
            this.parameter.SourceVersion = parameter.SourceVersion;
            this.parameter.Value = parameter.Value;
        }

        public override DbType DbType
        {
            get
            {
                return parameter.DbType;
            }
            set
            {
                if (value == DbType.Boolean)
                {
                    parameter.OracleDbType = OracleDbType.Int16;
                }
                else
                {
                    int intType = (int)value;
                    if (Enum.IsDefined(typeof(DbType), intType))
                        parameter.DbType = value;
                    else
                        parameter.OracleDbType = (OracleDbType)(int)value;
                }
            }
        }

        public override ParameterDirection Direction
        {
            get
            {
                return parameter.Direction;
            }
            set
            {
                parameter.Direction = value;
            }
        }

        public override bool IsNullable
        {
            get
            {
                return parameter.IsNullable;
            }
            set
            {
                parameter.IsNullable = value;
            }
        }

        public override string ParameterName
        {
            get
            {
                return parameter.ParameterName;
            }
            set
            {
                parameter.ParameterName = value;
            }
        }

        public override int Size
        {
            get
            {
                return parameter.Size;
            }
            set
            {
                parameter.Size = value;
            }
        }

        public override object Value
        {
            get
            {
                return parameter.Value;
            }
            set
            {
                if (value != DBNull.Value && (parameter.DbType == DbType.DateTime || parameter.DbType == DbType.Date || parameter.OracleDbType == OracleDbType.Date))
                    parameter.Value = (DateTime)value;
                else
                    parameter.Value = value;
            }
        }

        public override void ResetDbType()
        {
            //parameter.();
        }

        public override string SourceColumn
        {
            get
            {
                return parameter.SourceColumn;
            }
            set
            {
                parameter.SourceColumn = value;
            }
        }

        public override bool SourceColumnNullMapping
        {
            get
            {
                return parameter.IsNullable;
            }
            set
            {
                parameter.IsNullable = value;
            }
        }

        public override DataRowVersion SourceVersion
        {
            get
            {
                return parameter.SourceVersion;
            }
            set
            {
                parameter.SourceVersion = value;
            }
        }
    }
    
    public class OracleDbParametersCollection : DbParameterCollection
    {
        private OracleParameterCollection collection = null;

        public OracleDbParametersCollection(OracleParameterCollection collection)
        {
            this.collection = collection;
        }

        public override int Add(object value)
        {
            OracleDbParameter param = value as OracleDbParameter;
            if (param != null)
            {
                return collection.Add((object)param.OracleParameter);
            }
            else
            {
                IDbDataParameter dbParam = value as IDbDataParameter;
                if (dbParam != null)
                {
                    param = new OracleDbParameter(dbParam);
                    return collection.Add((object)param.OracleParameter);
                }
            }
            return -1;
        }

        public override int Count
        {
            get { return collection.Count; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsFixedSize
        {
            get { return false; }
        }

        public override bool IsSynchronized
        {
            get { return true; }
        }

        public override object SyncRoot
        {
            get { return null; }
        }

        public override void AddRange(Array values)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool Contains(object value)
        {
            OracleDbParameter param = value as OracleDbParameter;
            if (param != null)
            {
                return collection.Contains(param.OracleParameter);
            }
            return false;
        }

        public override bool Contains(string value)
        {
            return collection.Contains(value);
        }

        public override void CopyTo(Array array, int index)
        {
            collection.CopyTo(array, index);
        }

        public override void Clear()
        {
            collection.Clear();
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        protected override DbParameter GetParameter(int index)
        {
            return new OracleDbParameter(collection[index]);
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int IndexOf(object value)
        {
            OracleDbParameter param = value as OracleDbParameter;
            if (param != null)
            {
                return collection.IndexOf(param.OracleParameter);
            }
            return -1;
        }

        public override int IndexOf(string parameterName)
        {
            return collection.IndexOf(parameterName);
        }

        public override void Insert(int index, object value)
        {
            OracleDbParameter param = value as OracleDbParameter;
            if (param != null)
            {
                collection.Insert(index, param.OracleParameter);
            }
        }

        public override void Remove(object value)
        {
            OracleDbParameter param = value as OracleDbParameter;
            if (param != null)
            {
                collection.Remove(param.OracleParameter);
            }
        }

        public override void RemoveAt(int index)
        {
            collection.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            collection.RemoveAt(parameterName);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
