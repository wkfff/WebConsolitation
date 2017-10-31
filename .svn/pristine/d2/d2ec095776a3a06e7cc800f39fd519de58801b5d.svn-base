using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Controllers.Binders
{
    public class IntegerArrayModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == null)
            {
                return new int[0];
            }

            string attemptedValue = valueProviderResult.AttemptedValue;
            if (string.IsNullOrEmpty(attemptedValue))
            {
                return new int[0];
            }

            var integers = attemptedValue
                .Trim(new[] { '[', ']' })
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<int>(integers.Length);
            foreach (string integer in integers)
            {
                int tmp;
                
                if (int.TryParse(integer, out tmp))
                {
                    list.Add(tmp);
                }
            }

            return list.ToArray();
        }
    }
}
