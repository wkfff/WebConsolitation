using System;

using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Params
{
    public class CurrentUserIdValueProvider : IParameterValueProvider
    {
        private readonly IScheme scheme;

        private string userId;

        public CurrentUserIdValueProvider(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public string GetValue()
        {
            return this.userId ?? (this.userId = Convert.ToString(this.scheme.UsersManager.GetCurrentUserID()));
        }
    }
}
