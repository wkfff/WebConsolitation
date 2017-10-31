using Microsoft.Web.Mvc;

namespace Krista.FM.RIA.Core
{
    public class JsonValueProviderFactoryWraper : JsonValueProviderFactory
    {
        public override System.Web.Mvc.IValueProvider GetValueProvider(System.Web.Mvc.ControllerContext controllerContext)
        {
            using (new WithoutServerContext())
            {
                return base.GetValueProvider(controllerContext);
            }
        }
    }
}
