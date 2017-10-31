using System;

namespace Krista.FM.RIA.Core.Extensions
{
    public class Expression
    {
        private readonly IParametersService parametersService;

        public Expression(IParametersService parametersService)
        {
            this.parametersService = parametersService;
        }

        public string Eval(string value)
        {
            NCalc.Expression expression = new NCalc.Expression(value.Replace("$(", "("));
            expression.EvaluateParameter += EvaluateParameter;
            return Convert.ToString(expression.Evaluate());
        }

        private void EvaluateParameter(string name, NCalc.ParameterArgs args)
        {
            args.HasResult = true;
            args.Result = parametersService.GetParameterValue(name);
        }
    }
}
