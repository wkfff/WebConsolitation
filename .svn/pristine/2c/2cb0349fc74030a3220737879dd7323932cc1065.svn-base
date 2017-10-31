using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Model;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Param
{
    using System;

    public sealed class AccountsOrgTypeProvider : IParameterValueProvider
    {
        private readonly IAuthService auth;

        public AccountsOrgTypeProvider(IAuthService auth)
        {
            this.auth = auth;
        }

        public string GetValue()
        {
            if (auth.IsAdmin())
            {
                return AccountsRole.Administrator;
            }

            if (auth.IsSpectator())
            {
                return AccountsRole.Spectator;
            }

            if (auth.Profile == null)
            {
                throw new NullReferenceException("У пользователя не определен профиль(таблица d_Org_UserProfile)");
            }

            return auth.Profile.RefUchr.RefTipYc.Name;
        }
    }
}
