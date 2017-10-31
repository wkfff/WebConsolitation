using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    /// <summary>
    /// Отображает список lookup-атрибутов для атрибута ассоциации
    /// </summary>
    public class LookupAttributesListConverter : TypeConverter
    {
        private string[] s;

        private static StandardValuesCollection defaultRelations;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return value;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IAssociation association = (IAssociation)context.Instance.GetType().InvokeMember("Parent", System.Reflection.BindingFlags.GetProperty, null, context.Instance, null);
            IDictionary<string, IDataAttribute> propertyValue = association.RoleBridge.Attributes;
            s = new string[propertyValue.Count];
            int i = 0;
            foreach (IDataAttribute item in propertyValue.Values)
            {
                s[i] = item.Caption + " (" + item.Name + ")";
                i++;
            }
            defaultRelations = new StandardValuesCollection(s);

            return defaultRelations;
        }
    }
}
