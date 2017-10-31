using System.Web;
using System.Web.SessionState;

namespace Krista.FM.RIA.Core
{
    internal class MvcReadOnlySessionHandler : MvcDynamicSessionHandler, IReadOnlySessionState 
    {
        public MvcReadOnlySessionHandler(IHttpAsyncHandler originalHandler)
            : base(originalHandler) 
        {
        }
    }
}
