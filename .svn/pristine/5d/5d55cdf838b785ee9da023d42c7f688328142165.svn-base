using System;

using Krista.FM.Common.Consolidation.Data;

namespace Krista.FM.Common.Consolidation.Calculations.Helpers
{
    public static class RecordExtensions
    {
        public static decimal GetDecimal(this IRecord record, string column)
        {
            object value = record.Get(column);
            if (value == null)
            {
                value = 0;
            }

            return Convert.ToDecimal(value);
        }
    }
}
