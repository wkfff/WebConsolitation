using System;
using Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters.CalumMcLellan.StructuredStorage;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters
{
    public class StructuredStorageAdapter : OfficeCustomPropertiesAdapter
    {
        private readonly PropertySetsAdapter propertySets;
        private readonly PropertySetAdapter propertySet;

        public StructuredStorageAdapter(string file)
        {
            propertySets = new PropertySetsAdapter(file);
            propertySet = GetStructuredStoragePropertySet(
                    propertySets, "D5CDD505-2E9C-101B-9397-08002B2CF9AE");
        }

        public override object GetProperty(string name)
        {
            if (propertySet.Contains(name))
                return propertySet[name].Value;
            return null;
        }

        public override void SetProperty(string name, object value)
        {
            if (propertySet.Contains(name))
            {
                propertySet[name].Value = value;
            }
            else
            {
                propertySet.Add(name, value);
            }
        }

        public override void Clear()
        {
            foreach (PropertyAdapter property in propertySet)
            {
                if (property.Name.StartsWith("fm."))
                    property.Delete();
            }
        }

        public override void Save()
        {
        }

        public override void Dispose()
        {
            if (propertySet != null) 
                propertySet.Dispose();
            if (propertySets != null) 
                propertySets.Dispose();
        }

        private static PropertySetAdapter GetStructuredStoragePropertySet(PropertySetsAdapter propertySets, string formatIdentifier)
        {
            PropertySetAdapter propertySet = !propertySets.Contains(new Guid(formatIdentifier)) 
                ? propertySets.Add(new Guid(formatIdentifier), false) 
                : propertySets[new Guid(formatIdentifier)];
            return propertySet;
        }
    }
}
