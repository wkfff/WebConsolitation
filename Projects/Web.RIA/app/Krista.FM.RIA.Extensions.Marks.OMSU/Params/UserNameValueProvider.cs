using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Params
{
    public class UserNameValueProvider : IParameterValueProvider
    {
        private readonly IMarksOmsuExtension extension;

        public UserNameValueProvider(IMarksOmsuExtension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            return extension.User.Name;
        }
    }
}
