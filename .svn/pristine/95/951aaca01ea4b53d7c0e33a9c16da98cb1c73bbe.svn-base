using System;
using System.Web.Mvc;
using System.Web.Security;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    [System.Diagnostics.DebuggerStepThrough]
    [ControllerSessionState(ControllerSessionState.ReadOnly)]
    public abstract class SchemeBoundController : Controller
    {
        public new BasePrincipal User
        {
            get { return (BasePrincipal)base.User; }
        }

        protected IScheme Scheme { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            IScheme scheme = filterContext.RequestContext.HttpContext.Session[ConnectionHelper.SchemeKeyName] as IScheme;
            if (scheme == null)
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", null);
                filterContext.Result = this.RedirectToAction("LogOn", "Account");
                FormsAuthentication.SignOut();
                Session.Abandon();
            }
            else
            {
                // Установка атрибутов пользователя в HTTPContext сессии, чтобы им можно было пользоваться в наследуемых контроллерах
                Resolver.Get<IPrincipalProvider>().SetBasePrincipal();
            }

            Scheme = scheme;
        }

        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            try
            {
                base.Execute(requestContext);
            }
            catch (Exception e)
            {
                Trace.TraceCritical("Ошибка выполнения: {0}", e.ExpandException());
                
                throw new ApplicationException("BUG", e);
            }
            finally
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", null);
            }
        }
    }
}
