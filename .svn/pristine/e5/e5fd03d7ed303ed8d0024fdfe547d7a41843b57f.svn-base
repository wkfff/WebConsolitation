using System;
using System.Web;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Спользуется для установки текущему потоку серверного контекста для вызова серверных методов.
    /// </summary>
    /// <example>
    /// using(new ServerContext())
    /// {
    ///     server.SomeMethod();
    /// }
    /// </example>
    public class ServerContext : IDisposable
    {
        private readonly LogicalCallContextData globalContext;

        public ServerContext()
        {
            globalContext = LogicalCallContextData.GetContext();
            if (globalContext == null)
            {
                // Устанавливаем контекст для передачи серверным методам.
                LogicalCallContextData.SetContext(
                    (LogicalCallContextData)
                    HttpContext.Current.Session[ConnectionHelper.LogicalCallContextDataKeyName]);
            }
        }

        public IScheme Scheme
        {
            get
            {
                return (IScheme)HttpContext.Current.Session[ConnectionHelper.SchemeKeyName];
            }
        }

        public void Dispose()
        {
            if (globalContext == null)
            {
                // Удаляем контекст вызова серверных методов.
                System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", null);
            }
        }
    }
}
