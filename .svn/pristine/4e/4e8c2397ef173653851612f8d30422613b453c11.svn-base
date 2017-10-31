using System;

namespace Krista.FM.Common
{
    public class LogicalCallWithoutContext : IDisposable
    {
        private readonly LogicalCallContextData lccd;

        public LogicalCallWithoutContext()
        {
            lccd = LogicalCallContextData.GetContext();
            LogicalCallContextData.SetContext(null);
        }

        public void Dispose()
        {
            LogicalCallContextData.SetContext(lccd);
        }
    }
}
