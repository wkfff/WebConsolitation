using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF
{
    public class UserRoleEO10ValueProvider : IParameterValueProvider
    {
        private readonly IEO10Extension extension;

        public UserRoleEO10ValueProvider(IEO10Extension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            return extension.UserGroup == 1 ? "User" : string.Empty;
        }
    }
}
