using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook.Params
{
    public class UserRegionTypeValueProvider : IParameterValueProvider
    {
        private readonly IDebtBookExtension extension;

        public UserRegionTypeValueProvider(IDebtBookExtension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            return extension.UserRegionType.ToString();
        }
    }
}
