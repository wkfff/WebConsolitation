using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.FO41.Helpers
{
    public interface ICalculator
    {
        void Calc(IList<object> facts, int regionId, bool calculatePrognoz);
    }
}
