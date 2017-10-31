using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Krista.FM.RIA.Core
{
    public class DynamicRouteHandler : IRouteHandler
    {
        private ControllerBuilder controllerBuilder;

        internal ControllerBuilder ControllerBuilder
        {
            get
            {
                return controllerBuilder ?? ControllerBuilder.Current;
            }

            set
            {
                controllerBuilder = value;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string controllerName = requestContext.RouteData.GetRequiredString("controller");

            ControllerSessionState mode = ControllerSessionState.Default;
            var factory = ControllerBuilder.GetControllerFactory() as UnityControllerFactory;
            if (factory != null)
            {
                mode = factory.GetControllerSessionState(requestContext, controllerName);
            }

            switch (mode)
            {
                case ControllerSessionState.Disabled:
                    return new MvcDynamicSessionHandler(new MvcHandler(requestContext));

                case ControllerSessionState.ReadOnly:
                    return new MvcReadOnlySessionHandler(new MvcHandler(requestContext));

                default:
                    // Даем MvcHandler обработать все остальное.
                    return new MvcHandler(requestContext);
            }
        }
    }
}