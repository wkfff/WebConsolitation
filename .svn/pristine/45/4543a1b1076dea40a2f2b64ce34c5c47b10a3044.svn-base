using System;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Principal;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Params
{
    public class UserCanSeeIneffExpenses : IParameterValueProvider
    {
        public string GetValue()
        {
            try
            {
                return ((BasePrincipal)System.Web.HttpContext.Current.User).IsInAnyRoles(MarksOMSUConstants.IneffGkhWatchRole, MarksOMSUConstants.IneffGkhCalculateRole) ? "1" : "0";
            }
            catch
            {
                return "0";
            }
        }
    }
}
