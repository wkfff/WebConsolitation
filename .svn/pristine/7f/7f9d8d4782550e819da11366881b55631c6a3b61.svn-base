using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Controllers.Binders
{
    public class ViewParamsBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (string item in controllerContext.HttpContext.Request.QueryString)
            {
                if (item != null)
                {
                    string value = bindingContext.ValueProvider.GetValue(item).AttemptedValue;
                    parameters.Add(item, value);
                }
            }

            return parameters;
        }
    }
}
