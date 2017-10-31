using System;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class Calculator : CalculatorAbstract
    {
        public IValueResolver ValueResolver { get; set; }

        protected override void EvaluateExpression(IFormula formula)
        {
            object result = null;

            try
            {
                result = formula.ParsedExpression.Evaluate();
            }
            catch (NullReferenceException)
            {
                // Считать нормальной ситуацией
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка вычисления выражения {0} = {1}: {2}", formula.Name, formula.Expression, e.Message);
            }

            ValueResolver.SetValue(formula.Name, result);
        }

        protected override object EvaluateParameter(string name)
        {
            return ValueResolver.GetValue(name);
        }
    }
}
