using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.FO41.Params
{
    public class FO41OKTMOValueProvider : IParameterValueProvider
    {
        private readonly IFO41Extension extension;

        public FO41OKTMOValueProvider(IFO41Extension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            return extension.OKTMO;
        }
    }
}
