namespace Krista.FM.Common.Calculator.Exceptions
{
    public class CircularReferenceException : CalculatorException
    {
        public CircularReferenceException()
            : base("Обнаружена кольцевая зависимость")
        {
        }
    }
}
