using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Model;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Param
{
    public sealed class AccountsRoleProvider : IParameterValueProvider
    {
        private readonly IAuthService auth;

        public AccountsRoleProvider(IAuthService auth)
        {
            this.auth = auth;
        }

        public string GetValue()
        {
            if (auth.IsKristaRu())
            {
                return AccountsRole.KristaRu;
            }

            if (auth.IsAdmin())
            {
                return AccountsRole.Administrator;
            }

            if (auth.IsPpoUser())
            {
                return AccountsRole.SuperProvider;
            }

            if (auth.IsGrbsUser())
            {
                return AccountsRole.Provider;
            }

            if (auth.IsSpectator())
            {
                return AccountsRole.Spectator;
            }

            return AccountsRole.Consumer;
            //// TODO: обработать юзера без прав
            //// throw new NotImplementedException();  
        }
    }
}
