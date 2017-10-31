using System;
using System.Web;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public static class SchemeAccessor
    {
        public static IScheme GetScheme()
        {
            var scheme = HttpContext.Current.Session[ConnectionHelper.SchemeKeyName] as IScheme;
            if (scheme == null)
            {
                throw new Exception("Вызов метода SchemeAccessor.GetScheme() вне контекста сессии.");
            }

            return scheme;
        }
    }
}
