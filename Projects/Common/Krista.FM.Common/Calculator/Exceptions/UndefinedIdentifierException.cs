using Krista.FM.Extensions;

namespace Krista.FM.Common.Calculator.Exceptions
{
    public class UndefinedIdentifierException : CalculatorException
    {
        public UndefinedIdentifierException(string identifier)
            : base("Идентификатор '{0}' не определен".FormatWith(identifier))
        {
        }
    }
}
