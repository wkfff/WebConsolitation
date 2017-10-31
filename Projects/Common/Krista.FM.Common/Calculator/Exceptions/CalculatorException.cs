using System;

namespace Krista.FM.Common.Calculator.Exceptions
{
    public class CalculatorException : Exception
    {
        public CalculatorException(string message)
            : base (message)
        {
        }
    }
}
