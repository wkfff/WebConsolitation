using System;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Controllers.Binders
{
    public class DefaultBinder : IModelBinder
    {
        public DefaultBinder(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get; set; }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Common.LogicalCallContextData lccd = Common.LogicalCallContextData.GetContext();
            NameValueCollection values;
            try
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", null);
                values = controllerContext.HttpContext.Request.Params;
            }
            finally
            {
                Common.LogicalCallContextData.SetContext(lccd);
            }

            object model = values[bindingContext.ModelName];
            if (model == null)
            {
                model = Convert.ToString(DefaultValue);
            }

            return model;
        }
    }
}
