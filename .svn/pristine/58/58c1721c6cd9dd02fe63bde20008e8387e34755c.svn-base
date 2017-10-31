using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Krista.FM.Common.Calculator.Exceptions;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class FormulaRepository : IFormulaRepository
    {
        private readonly ICollection<IFormula> data;

        public FormulaRepository()
        {
            data = new Collection<IFormula>();
        }

        #region IFormulaRepositry

        public IEnumerable<IFormula> FindAll()
        {
            return data.AsEnumerable();
        }

        public IFormula FindOne(string name)
        {
            return data.FirstOrDefault(x => x.Name == name);
        }

        #endregion

        public void Add(IFormula formula)
        {
            if (FindOne(formula.Name) != null)
            {
                throw new DuplicateIdentifierException(formula.Name);
            }
            data.Add(formula);
        }

        public void Drop(IFormula formula)
        {
            if (FindOne(formula.Name) == null)
            {
                throw new UndefinedIdentifierException(formula.Name);
            }
            data.Add(formula);
        }
    }
}
