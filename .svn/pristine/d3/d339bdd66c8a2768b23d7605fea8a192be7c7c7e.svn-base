using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.FO41.Services;

using NCalc;

namespace Krista.FM.RIA.Extensions.FO41.Helpers
{
    public class FO41FormulaEvaluter
    {
        private readonly FactsService factRepository;
        private readonly IndicatorService indicatorRepository;
        private readonly FactResolverDelegat factResolver;
        private readonly Node node;
        private readonly IEnumerable<Node> calcTree;
        private readonly int applicationId;
        private ParameterResolverDelegat parameterResolver;

        public FO41FormulaEvaluter(
            Node node, 
            IEnumerable<Node> calcTree, 
            FactsService factRepository, 
            int applicationId, 
            FactResolverDelegat factResolver,
            IndicatorService indicatorRepository)
        {
            this.node = node;
            this.calcTree = calcTree;
            this.factRepository = factRepository;
            this.applicationId = applicationId;
            this.factResolver = factResolver;
            this.indicatorRepository = indicatorRepository;
        }

        public delegate F_Marks_DataPrivilege FactResolverDelegat(int markId, int applicationId);

        public delegate decimal? ParameterResolverDelegat(F_Marks_DataPrivilege fact);

        public delegate void ParameterSetterDelegat(decimal? result, F_Marks_DataPrivilege fact);

        public void Calc(ParameterResolverDelegat resolver, ParameterSetterDelegat setter)
        {
            parameterResolver = resolver;

            var exp = new Expression(node.Formula);
            exp.EvaluateParameter += OnEvaluateParameter;
            exp.EvaluateFunction += OnEvaluateFunction;
            double result = 0;
            bool hasResult = true;
            try
            {
                var expr = exp.Evaluate();
                result = Convert.ToDouble(expr);
            }
            catch (ArgumentException)
            {
                hasResult = false;
            }
            catch (EvaluationException e)
            {
                Trace.TraceError(
                    "Ошибка при вычислении формулы {0}: {1}",
                    node.Formula,
                    e.Message);

                hasResult = false;
            }
            catch (Exception)
            {
                hasResult = false;
            }

            if (Double.IsNaN(result) || Double.IsInfinity(result))
            {
                result = 0;
            }

            var fact = factResolver(node.ID, applicationId);

            if (hasResult)
            {
                setter(Convert.ToDecimal(result), fact);
            }
            else
            {
                setter(null, fact);
            }

            factRepository.Save(fact);
        }

        private void OnEvaluateParameter(string name, ParameterArgs args)
        {
            var parameterNode = calcTree.FirstOrDefault(x => x.Symbol == name);

            if (parameterNode == null)
            {
                Trace.TraceError("Параметр {0} не найден.", name);
                args.Result = Convert.ToDouble(0);
                return;
            }

            var fact = factResolver(parameterNode.ID, applicationId);
            if (fact == null)
            {
                return;
            }

            var val = parameterResolver(fact);
            if (val == null)
            {
                return;
            }

            args.Result = Convert.ToDouble(val);
        }

        private void OnEvaluateFunction(string name, FunctionArgs args)
        {
            if (name.ToUpper() == "GET")
            {
                // символ показателя
                var symbol = args.Parameters[0].ParsedExpression.ToString().Trim()
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                // определить id показателя, у которого символ равен первому параметру
                var indicatorId = indicatorRepository.GetIdBySymbol(symbol);
                
                // найти соответствующий факт
                var fact = factResolver(indicatorId, applicationId);
                
                // название периода из параметров функции
                var periodName = args.Parameters[1].ParsedExpression.ToString().Trim()
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);
                
                // в результат - соответствующее значение из факта
                if (periodName.ToUpper().Equals("FACT"))
                {
                    args.Result = fact.Fact ?? 0;
                }
                else
                    if (periodName.ToUpper().Equals("PREVIOUSFACT"))
                    {
                        args.Result = fact.PreviousFact ?? 0;
                    }
                    else
                        if (periodName.ToUpper().Equals("ESTIMATE"))
                        {
                            args.Result = fact.Estimate ?? 0;
                        }
                        else
                            if (periodName.ToUpper().Equals("FORECAST"))
                            {
                                args.Result = fact.Forecast ?? 0;
                            }
                            else
                            {
                                args.HasResult = false;
                            }
            }
            else
            {
                args.HasResult = false;
            }
        }
    }
}
