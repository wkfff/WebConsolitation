using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.FO51PassportMO
{
    public class UserRoleFO51Provider : IParameterValueProvider
    {
        private readonly IFO51Extension extension;

        public UserRoleFO51Provider(IFO51Extension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            if (extension.UserGroup == FO51Extension.GroupMo)
            {
                return "МО";
            }

            if (extension.UserGroup == FO51Extension.GroupOGV)
            {
                return "OGV";
            }

            return "OTHER";
        }
    }
}
