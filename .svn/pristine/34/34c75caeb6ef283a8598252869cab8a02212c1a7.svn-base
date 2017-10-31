using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    /// <summary>
    /// Отображает список имен правил сопоставления для ассоциации сопоставления.
    /// </summary>
    public class AssociateRulesConverter : TypeConverter
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
            IBridgeAssociation association = (IBridgeAssociation)context.Instance.GetType().InvokeMember("Parent", System.Reflection.BindingFlags.GetProperty, null, context.Instance, null);
            IDictionary<string, IAssociateRule> propertyValue = association.AssociateRules;
            s = new string[propertyValue.Count + 1];
            s[0] = "Правило не назначено";
            int i = 1;
            foreach (IAssociateRule item in propertyValue.Values)
            {
                s[i] = item.Name;
                i++;
            }
            defaultRelations = new StandardValuesCollection(s);

            return defaultRelations;
        }
    }
}