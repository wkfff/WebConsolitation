using System;
using System.Linq;
using System.Reflection;

using Krista.FM.Domain;

namespace Krista.FM.Common.Consolidation.Data
{
    public class ReportDataRecord : IRecord
    {
        private readonly object record;

        private ReportDataRecordState state;

        public ReportDataRecord(object record)
            : this(record, false)
        {
        }

        public ReportDataRecord(object record, bool isNewRecord)
        {
            this.record = record;
            state = isNewRecord ? ReportDataRecordState.Added : ReportDataRecordState.Unchanged;
        }

        public object Value
        {
            get { return record; }
        }

        public int MetaRowId
        {
            get { return ((D_Report_Row)record).RefFormRow.ID; }
        }

        public ReportDataRecordState State
        {
            get { return state; }
        }

        public bool IsMultiplicity()
        {
            return ((D_Report_Row)record).RefFormRow.Multiplicity;
        }

        public object Get(string column)
        {
            foreach (PropertyInfo pi in record.GetType().GetProperties().Where(pi => pi.Name == column))
            {
                return pi.GetValue(record, null);
            }

            throw new InvalidOperationException(String.Format("Графа с именем {0} не найдена.", column));
        }

        public void Set(string column, object value)
        {
            foreach (PropertyInfo pi in record.GetType().GetProperties().Where(pi => pi.Name == column))
            {
                if (value is DBNull)
                {
                    value = null;
                }

                if (value is string && String.IsNullOrEmpty((string)value))
                {
                    value = null;
                }

                object convertedValue;

                Type nullableType = Nullable.GetUnderlyingType(pi.PropertyType);
                if (nullableType != null)
                {
                    convertedValue = value == null ? null : Convert.ChangeType(value, nullableType);
                }
                else
                {
                    convertedValue = Convert.ChangeType(value, pi.PropertyType);
                }

                value = convertedValue;
                pi.SetValue(record, value, null);
                SetModified();
                return;
            }

            throw new InvalidOperationException(String.Format("Графа с именем {0} не найдена.", column));
        }

        public void AssignRecord(object assignRecord)
        {
            foreach (PropertyInfo pi in record.GetType().GetProperties())
            {
                if (pi.Name != "ID")
                {
                    object value = pi.GetValue(assignRecord, null);
                    pi.SetValue(record, value, null);
                }
            }
        }

        public void Delete()
        {
            state = ReportDataRecordState.Deleted;
        }

        private void SetModified()
        {
            if (State != ReportDataRecordState.Added && State != ReportDataRecordState.Deleted)
            {
                state = ReportDataRecordState.Modified;
            }
        }
    }
}
