using System;
using System.Configuration;
using System.Globalization;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Param
{
    public sealed class IsMainPpoProvider : IParameterValueProvider
    {
        private readonly IAuthService auth;

        public IsMainPpoProvider(IAuthService auth)
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
                        ppo => ppo.Code.Equals(ConfigurationManager.AppSettings["ClientLocationOKATOCode"] + "000000000")
                                   ? true.ToString(CultureInfo.InvariantCulture)
                                   : false.ToString(CultureInfo.InvariantCulture),
                        false.ToString(CultureInfo.InvariantCulture)),
                    false.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}