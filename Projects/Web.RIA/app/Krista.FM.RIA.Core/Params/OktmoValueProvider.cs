using System;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Params
{
    public class OktmoValueProvider : IParameterValueProvider
    {
        private readonly IScheme scheme;

        private string oktmo;

        public OktmoValueProvider(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public string GetValue()
        {
            return this.oktmo ?? (this.oktmo = Convert.ToString(this.scheme.GlobalConstsManager.Consts["OKTMO"].Value));
        }
    }
}
