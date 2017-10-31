using System;
using System.Collections;
using System.Collections.Generic;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters.CalumMcLellan.StructuredStorage
{
    public class PropertySetAdapter : IEnumerable<PropertyAdapter>, IDisposable

    {
        private readonly object instance;

        public PropertySetAdapter(object instance)
        {
            this.instance = instance;
        }

        public bool Contains(string name)
        {
            return Convert.ToBoolean(
                ReflectionHelper.CallMethod(instance, "Contains", name));
        }

        public PropertyAdapter Add(string name, object value)
        {
            return new PropertyAdapter(
                ReflectionHelper.CallMethod(instance, "Add", name, value));
        }

        public PropertyAdapter this[string name]
        {
            get
            {
                return new PropertyAdapter(
                    ReflectionHelper.CallMethod(instance, "get_Item", name));
            }
        }

        public void Dispose()
        {
            ReflectionHelper.CallMethod(instance, "Dispose");
        }

        public IEnumerator<PropertyAdapter> GetEnumerator()
        {
            return new EnumeratorAdapter(ReflectionHelper.CallMethod(instance, "GetEnumerator"));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
