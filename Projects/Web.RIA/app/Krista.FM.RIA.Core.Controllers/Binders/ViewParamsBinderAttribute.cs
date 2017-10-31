using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Controllers.Binders
{
    public class ViewParamsBinderAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new ViewParamsBinder();
        }
    }
}