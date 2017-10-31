using System.Collections.Generic;
using NCalc;

namespace Krista.FM.Common.Calculator
{
    public interface IFormula
    {
        string Name { get; }

        string Expression { get; }

        /// <summary>
        /// Подготовленное к рассчету выражение.
        /// </summary>
        Expression ParsedExpression { get; }

        /// <summary>
        /// Формулы, от которых зависит данная формула.
        /// Заполняется препроцессором при анализе зависимостей (IPreprocessor.AnalizeFormulas)
        /// </summary>
        ICollection<IFormula> DependOnFormulas { get; }

        /// <summary>
        /// Константы, от которых зависит данная формула.
        /// Заполняется препроцессором при анализе зависимостей (IPreprocessor.AnalizeFormulas)
        /// </summary>
        ICollection<IConstant> DependOnConstants { get; }

        /// <summary>
        /// Формулы, которые зависят от данной.
        /// Заполняется препроцессором при анализе зависимостей (IPreprocessor.AnalizeFormulas)
        /// </summary>
        ICollection<IFormula> DependentFormulas { get; }
    }
}
