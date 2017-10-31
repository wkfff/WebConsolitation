using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    public class SortBinderAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new SortBinder();
        }
    }
}
