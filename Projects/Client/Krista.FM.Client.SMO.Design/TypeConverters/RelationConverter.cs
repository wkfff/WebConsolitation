using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class RelationConverter : TypeConverter
    {
        private static StandardValuesCollection defaultRelations;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            Assembly assembly = Assembly.Load("Krista.FM.Client.SchemeEditor");
            Type t = assembly.GetType("Krista.FM.Client.SchemeEditor.SchemeEditor");
            PropertyInfo pi = t.GetProperty("Instance");
            object schemeEditor = pi.GetValue(t, null);

            assembly = Assembly.Load("Krista.FM.Client.Design");
            t = assembly.GetType("Krista.FM.Client.Design.ISchemeEditor");
            IScheme scheme = t.InvokeMember("Scheme", BindingFlags.GetProperty, null, schemeEditor, null) as IScheme;

            //IScheme scheme = (IScheme)context.Instance.GetType().InvokeMember("Scheme", System.Reflection.BindingFlags.GetProperty, null, context.Instance, null);
            IDictionary<string, string> propertyValue = scheme.Semantics;
            List<string> sColl = new List<string>();
            foreach (KeyValuePair<string, string> item in propertyValue)
            {
                sColl.Add(item.Value + " (" + item.Key + ")");
            }
            sColl.Sort();
            defaultRelations = new StandardValuesCollection(sColl);

            return defaultRelations;
        }
    }
}
