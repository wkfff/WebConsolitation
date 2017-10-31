using System;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters.CalumMcLellan.StructuredStorage
{
    public class PropertyAdapter
    {
        private readonly object instance;

        public PropertyAdapter(object instance)
        {
            this.instance = instance;
        }

        public string Name 
        { 
            get
            {
                return Convert.ToString(ReflectionHelper.CallMethod(instance, "get_Name"));
            }
        }
        
        public object Value
        {
            get
            {
                return ReflectionHelper.CallMethod(instance, "get_Value");
            }
            set
            {
                ReflectionHelper.CallMethod(instance, "set_Value", value);
            }
        }

        public void Delete()
        {
            ReflectionHelper.CallMethod(instance, "Delete");
        }
    }
}
