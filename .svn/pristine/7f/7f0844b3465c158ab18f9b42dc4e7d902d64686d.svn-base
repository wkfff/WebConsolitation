using System;
using NCalc;

namespace Krista.FM.Common.Calculator.Implementation
{
    public abstract class CalculatorAbstract : ICalculator
    {
        #region ICalculator

        public void Calculate(IFormula formula)
        {
            formula.ParsedExpression.EvaluateParameter += OnEvaluateParameter;
            formula.ParsedExpression.EvaluateFunction += OnEvaluateFunction;
            EvaluateExpression(formula);
        }

        #endregion

        protected abstract void EvaluateExpression(IFormula formula);

        protected abstract object EvaluateParameter(string name);

        protected virtual void OnEvaluateParameter(string name, ParameterArgs args)
        {
            args.Result = EvaluateParameter(name);
        }

        protected virtual void OnEvaluateFunction(string name, FunctionArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
