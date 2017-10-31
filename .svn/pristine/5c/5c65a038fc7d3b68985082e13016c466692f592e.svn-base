using System.Reflection;
using Ext.Net;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetComponentListenersExtensions
    {
        public static ComponentListener AddListerer<T>(this T listeners, string name, string handler)
            where T : ComponentListeners
        {
            PropertyInfo[] propertyInfos = typeof(T)
                .GetProperties();

            ComponentListener listener = null;
            foreach (PropertyInfo pi in propertyInfos)
            {
                if (pi.Name == name && pi.PropertyType.Equals(typeof(ComponentListener)))
                {
                    listener = pi.GetValue(listeners, BindingFlags.GetProperty, null, null, null)
                        as ComponentListener;
                }
            }

            if (listener != null)
            {
                listener.Handler = handler;
            }

            return listener;
        }
    }
}
