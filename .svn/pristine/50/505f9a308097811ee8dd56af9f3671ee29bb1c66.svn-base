using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Krista.FM.Common;

using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core
{
    public class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly IUnityContainer container;

        public UnityControllerFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public override void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            container.Teardown(controller);

            // Удаляем контекст вызова серверных методов.
            System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", null);
        }

        public ControllerSessionState GetControllerSessionState(RequestContext requestContext, string controllerName)
        {
            var type = GetControllerType(requestContext, controllerName);
            var attr = type.GetCustomAttributes(typeof(ControllerSessionStateAttribute), true)
                .OfType<ControllerSessionStateAttribute>().FirstOrDefault();
            return (attr != null) ? attr.Mode : ControllerSessionState.Default;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(0x194, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", requestContext.HttpContext.Request.Path));
            }

            if (controllerType.BaseType == typeof(SchemeBoundController))
            {
                // Для всех контроллеров устанавливаем контекст для передачи серверным методам.
                var logicalCallContext =
                    requestContext.HttpContext.Session[ConnectionHelper.LogicalCallContextDataKeyName];
                if (logicalCallContext != null)
                {
                    string ea = requestContext.HttpContext.Request.Form["__EVENTARGUMENT"];
                    if (ea == null || (!ea.Contains("postback") && !ea.Contains("event")))
                    {
                        LogicalCallContextData.SetContext(
                            (LogicalCallContextData)
                            requestContext.HttpContext.Session[ConnectionHelper.LogicalCallContextDataKeyName]);
                    }
                }
            }

            try
            {
                return (IController)container.Resolve(controllerType);
            }
            catch (ResolutionFailedException e)
            {
                LogicalCallContextData.SetContext(null);
                throw new ApplicationException(e.Message, e);
            }
        }
    }
}