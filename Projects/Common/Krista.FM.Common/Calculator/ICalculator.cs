using System.Collections.Generic;

namespace Krista.FM.Common.Calculator
{
    public interface ICalculator
    {
        /// <summary>
        /// Рассчитывает указанную формулу
        /// </summary>
        void Calculate(IFormula formula);
    }
}
