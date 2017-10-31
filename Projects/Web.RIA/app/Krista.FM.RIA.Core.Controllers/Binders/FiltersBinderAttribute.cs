using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Controllers.Binders
{
    public class FiltersBinderAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new FiltersBinder();
        }
    }
}