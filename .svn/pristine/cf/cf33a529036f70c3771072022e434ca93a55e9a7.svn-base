using System;
using System.Web.Mvc;
using Krista.FM.RIA.Core.Controllers.Helpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Controllers.Filters
{
    public class ViewEntityAuthorizationFilterAttribute : ActionFilterAttribute
    {
        private readonly IScheme scheme;

        public ViewEntityAuthorizationFilterAttribute()
        {
            scheme = Resolver.Get<IScheme>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string objectKey = Convert.ToString(filterContext.ActionParameters["objectKey"]);
            scheme.RootPackage.FindEntityByName(objectKey).CanViewData(scheme.UsersManager, true);
        }
    }
}
