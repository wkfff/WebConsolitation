using System.Linq;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Params
{
    public class UserRoleProvider : IParameterValueProvider
    {
        private readonly IScheme scheme;

        public UserRoleProvider(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public string GetValue()
        {
            object[] allowedObjects = scheme.UsersManager.GetViewObjectsNamesAllowedForCurrentUser();

            return allowedObjects.Contains(ConsolidationExtensionInstaller.AdminIdentifier) ? "Admin" : "User";
        }
    }
}
