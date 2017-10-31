using System;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook.Params
{
    public class UserRegionIdValueProvider : IParameterValueProvider
    {
        private readonly IDebtBookExtension extension;

        public UserRegionIdValueProvider(IDebtBookExtension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            return Convert.ToString(extension.UserRegionId);
        }
    }
}
