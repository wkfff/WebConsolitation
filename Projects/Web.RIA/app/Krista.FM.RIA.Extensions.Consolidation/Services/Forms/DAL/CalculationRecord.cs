using System;
using System.Data;

using Krista.FM.Common.Consolidation;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public class CalculationRecord : IRecord
    {
        private const int IsMultiplicityColumnIndex = 3;

        private readonly DataRow row;

        public CalculationRecord(DataRow row)
        {
            this.row = row;
        }

        public bool IsMultiplicity()
        {
            return Convert.ToBoolean(row[IsMultiplicityColumnIndex]);
        }

        public object Get(string column)
        {
            var result = row[column];
            if (result == DBNull.Value)
            {
                return null;
            }

            return result;
        }

        public void Set(string column, object value)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }

            row[column] = value;
        }

        /// <summary>
        /// Только для внутреннего использования.
        /// </summary>
        internal DataRow GetDataRow()
        {
            return row;
        }
    }
}
