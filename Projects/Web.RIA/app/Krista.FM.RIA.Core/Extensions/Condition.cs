using System.Xml.Linq;
using NCalc;

namespace Krista.FM.RIA.Core.Extensions
{
    public class Condition
    {
        private readonly IParametersService parametersService;

        public Condition(IParametersService parametersService)
        {
            this.parametersService = parametersService;
        }

        public bool Test(XElement xElement)
        {
            var xAttribute = xElement.Attribute("condition");
            if (xAttribute == null)
            {
                // Если условие для элемента не указанно, то считаем, 
                // что выражение истино и элемент должен быть обработан.
                return true;
            }

            NCalc.Expression expression = new NCalc.Expression(xAttribute.Value.Replace("$(", "("));
            expression.EvaluateParameter += EvaluateParameter;
            object result = expression.Evaluate();

            return (bool)result;
        }

        private void EvaluateParameter(string name, ParameterArgs args)
        {
            args.HasResult = true;
            args.Result = parametersService.GetParameterValue(name);
        }
    }
}
