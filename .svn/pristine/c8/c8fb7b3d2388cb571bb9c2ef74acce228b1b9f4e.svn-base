using System;
using System.Web.Mvc;

namespace Krista.FM.RIA.Core
{
    public static class ControllerExtensions
    {
        public static bool IsController(Type type)
        {
            return typeof(IController).IsAssignableFrom(type) && 
                (((type != null) 
                    && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)) 
                    && !type.IsAbstract);
        }
    }
}
