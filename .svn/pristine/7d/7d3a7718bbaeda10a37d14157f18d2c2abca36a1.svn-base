using System;
using System.Collections;
using System.Collections.Generic;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters.CalumMcLellan.StructuredStorage
{
    public class EnumeratorAdapter : IEnumerator<PropertyAdapter>
    {
        private readonly object instance;

        public EnumeratorAdapter(object instance)
        {
            this.instance = instance;
        }

        public void Dispose()
        {
            ReflectionHelper.CallMethod(instance, "Dispose");
        }

        public bool MoveNext()
        {
            return Convert.ToBoolean(ReflectionHelper.CallMethod(instance, "MoveNext"));
        }

        public void Reset()
        {
            ReflectionHelper.CallMethod(instance, "Reset");
        }

        public PropertyAdapter Current
        {
            get
            {
                return new PropertyAdapter(ReflectionHelper.CallMethod(instance, "get_Current"));
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
