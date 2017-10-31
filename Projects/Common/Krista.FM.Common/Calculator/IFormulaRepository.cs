using System.Collections.Generic;

namespace Krista.FM.Common.Calculator
{
    public interface IFormulaRepository
    {
        IEnumerable<IFormula> FindAll();

        IFormula FindOne(string name);
    }
}
