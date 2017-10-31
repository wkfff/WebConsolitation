using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Param
{
    public sealed class IsStaffAccountsProvider : IParameterValueProvider
    {
        private readonly IAuthService auth;

        public IsStaffAccountsProvider(IAuthService auth)
        {
            this.auth = auth;
        }

        #region IParameterValueProvider Members

        public string GetValue()
        {
            return 
                !auth.IsAdmin() ? 
                    auth.Profile.IsAdmin ? 
                        true.ToString() : 
                        false.ToString() : 
                    true.ToString();
        }

        #endregion
    }
}