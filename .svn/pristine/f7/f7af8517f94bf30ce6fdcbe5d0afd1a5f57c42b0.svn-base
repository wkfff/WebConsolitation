using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using Ext.Net;

namespace Krista.FM.RIA.Core.Controllers.Binders
{
    public class FiltersBinder : IModelBinder
    {
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

            StringBuilder combinedFilters = new StringBuilder()
                .Append(values["gridfilters"])
                .Append(values["systemGridFilters"])
                .Replace("}{", ",");

            return new FilterConditions(combinedFilters.ToString());
        }
    }
}
