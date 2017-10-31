using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Controllers.Binders
{
    public class DefaultBinderAttribute : CustomModelBinderAttribute
    {
        public DefaultBinderAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get; set; }

        public override IModelBinder GetBinder()
        {
            return new DefaultBinder(DefaultValue);
        }
    }
}