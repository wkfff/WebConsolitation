using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.OrgGKH
{
    public class UserRoleGKHValueProvider : IParameterValueProvider
    {
        private readonly IOrgGkhExtension extension;

        public UserRoleGKHValueProvider(IOrgGkhExtension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            if (extension.UserGroup == OrgGKHExtension.GroupMo)
            {
                return "MO";
            }

            if (extension.UserGroup == OrgGKHExtension.GroupAudit)
            {
                return "Audit";
            }

            if (extension.UserGroup == OrgGKHExtension.GroupOrg)
            {
                return "Org";
            }

            if (extension.UserGroup == OrgGKHExtension.GroupIOGV)
            {
                return "Iogv";
            }

            return string.Empty;
        }
    }
}
