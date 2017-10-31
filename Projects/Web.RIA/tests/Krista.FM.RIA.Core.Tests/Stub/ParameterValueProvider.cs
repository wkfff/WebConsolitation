using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Core.Tests.Stub
{
    public class ParameterValueProvider : IParameterValueProvider
    {
        public string GetValue()
        {
            return "Subject";
        }
    }
}
