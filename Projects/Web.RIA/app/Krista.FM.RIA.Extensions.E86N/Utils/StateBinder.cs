using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    public class StateBinder : IModelBinder
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
                .Append(values["state"])
                .Replace("}{", ",");

            return new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ReportsHelper.ColumnsAndSorts>(
                combinedFilters.ToString());
        }
    }
}
