using System;
using System.Runtime.Remoting.Messaging;
using Krista.FM.Common;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Спользуется для временного удаления из текущего потока серверного контекста вызова серверных методов.
    /// </summary>
    /// <example>
    /// using(new WithoutServerContext())
    /// {
    ///     SomeMethod();
    /// }
    /// </example>
    public class WithoutServerContext : IDisposable
    {
        private readonly LogicalCallContextData context;

        public WithoutServerContext()
        {
            // Сохраняем контекст вызова серверных методов.
            context = LogicalCallContextData.GetContext();

            // Удаляем контекст вызова серверных методов.
            CallContext.SetData("Authorization", null);
        }

        public void Dispose()
        {
            // Устанавливаем контекст для передачи серверным методам.
            LogicalCallContextData.SetContext(context);
        }
    }
}
