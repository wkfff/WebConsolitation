using System.Collections.Generic;
using System.Collections.ObjectModel;
using NCalc;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class Formula : IFormula
    {
        public Formula(string name, string expression)
        {
            Name = name;
            Expression = expression;
            ParsedExpression = new Expression(expression);
            DependOnFormulas = new Collection<IFormula>();
            DependOnConstants = new Collection<IConstant>();
            DependentFormulas = new Collection<IFormula>();
        }

        #region IFormula

        public string Name { get; private set; }

        public string Expression { get; private set; }

        public Expression ParsedExpression { get; private set; }

        public ICollection<IFormula> DependOnFormulas { get; private set; }

        public ICollection<IConstant> DependOnConstants { get; private set; }

        public ICollection<IFormula> DependentFormulas { get; private set; }

        #endregion
    }
}