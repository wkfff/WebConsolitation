using System.Collections.Generic;

namespace Krista.FM.Common.Calculator
{
    public interface IConstant
    {
        string Name { get; }

        /// <summary>
        /// Формулы, которые зависят от значения данной константы.
        /// Заполняется препроцессором при анализе зависимостей (IPreprocessor.AnalizeFormulas)
        /// </summary>
        ICollection<IFormula> DependentFormulas { get; }
    }
}
