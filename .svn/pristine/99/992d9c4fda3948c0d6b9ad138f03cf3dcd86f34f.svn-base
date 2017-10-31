using System;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Params
{
    public class OKTMOValueProvider : IParameterValueProvider
    {
        private readonly IScheme scheme;

        public OKTMOValueProvider(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public string GetValue()
        {
            return Convert.ToString(scheme.GlobalConstsManager.Consts["OKTMO"].Value);
        }
    }
}
