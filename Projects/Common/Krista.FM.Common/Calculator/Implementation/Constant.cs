using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class Constant : IConstant
    {
        public Constant(string name)
        {
            Name = name;
            DependentFormulas = new Collection<IFormula>();
        }

        #region IConstant

        public string Name { get; private set; }

        public ICollection<IFormula> DependentFormulas { get; private set; }

        #endregion
    }
}