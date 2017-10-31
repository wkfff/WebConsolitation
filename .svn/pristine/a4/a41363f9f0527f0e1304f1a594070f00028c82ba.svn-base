using System.Web.Mvc;

namespace Krista.FM.RIA.Core
{
    public class ClearLogicalCallContextAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", null);
            base.OnActionExecuted(filterContext);
        }
    }
}
