using System;
using System.Web;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core
{
    public class SessionLifetimeManager<T> : LifetimeManager, IDisposable
    {
        public override object GetValue()
        {
            return HttpContext.Current.Session[typeof(T).AssemblyQualifiedName];
        }
        
        public override void RemoveValue()
        {
            HttpContext.Current.Session.Remove(typeof(T).AssemblyQualifiedName);
        }
        
        public override void SetValue(object newValue)
        {
            HttpContext.Current.Session[typeof(T).AssemblyQualifiedName] = newValue;
        }
        
        public void Dispose()
        {
            RemoveValue();
        }
    }
}
