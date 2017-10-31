using System;
using System.Globalization;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Param
{
    public sealed class IsMunicipalityProvider : IParameterValueProvider
    {
        private readonly IAuthService auth;

        public IsMunicipalityProvider(IAuthService auth)
        {
            this.auth = auth;
        }

        #region IParameterValueProvider Members

        public string GetValue()
        {
            if (auth.Profile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            return
                auth.Profile.Return(
                    profile => profile.RefUchr.RefOrgPPO.Return(
                        ppo => ppo.Code.EndsWith("000000") &&
                               (ppo.Name.ToLowerInvariant().Contains("район") ||
                                ppo.Name.ToLowerInvariant().Contains("городской округ"))
                                   ? true.ToString(CultureInfo.InvariantCulture)
                                   : false.ToString(CultureInfo.InvariantCulture),
                        false.ToString(CultureInfo.InvariantCulture)),
                    false.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}