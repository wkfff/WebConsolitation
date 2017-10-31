using System;
using System.Reflection;

using Krista.FM.Common.Consolidation.Data;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests.Helpers
{
    public class RecordBase : IRecord
    {
        public virtual int MetaRowId
        {
            get { throw new NotImplementedException(); }
        }

        public virtual ReportDataRecordState State
        {
            get { throw new NotImplementedException(); }
        }

        public virtual object Value
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool IsMultiplicity()
        {
            throw new NotImplementedException();
        }

        public virtual object Get(string column)
        {
            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                if (pi.Name.ToUpper() == column.ToUpper())
                {
                    return pi.GetValue(this, null);
                }
            }

            throw new InvalidOperationException(String.Format("Колонка с именем {0} не найдена.", column));
        }

        public virtual void Set(string column, object value)
        {
            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                if (pi.Name.ToUpper() == column.ToUpper())
                {
                    object convertedValue;
                    Type nullableType = Nullable.GetUnderlyingType(pi.PropertyType);
                    if (nullableType != null)
                    {
                        if (value == null)
                        {
                            convertedValue = null;
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(value, nullableType);
                        }
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(value, pi.PropertyType);
                    }

                    pi.SetValue(this, convertedValue, null);
                    break;
                }
            }
        }

        public virtual void AssignRecord(object assignRecord)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
