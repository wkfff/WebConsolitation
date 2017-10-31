using Krista.FM.Extensions;

namespace Krista.FM.Common.Calculator.Exceptions
{
    public class InvalidConfigurationException : CalculatorException
    {
        public InvalidConfigurationException(string message)
            : base("Некорректная настройка: {0}".FormatWith(message))
        {
        }
    }
}
