using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.FO41.Services;

using NCalc;

namespace Krista.FM.RIA.Extensions.FO41.Helpers
{
    public class FO41EstimateFormulaEvaluter
    {
        private readonly ILinqRepository<F_Marks_Privilege> factRepository;
        private readonly IndicatorService indicatorRepository;
        private readonly FactResolverDelegat factResolver;
        private readonly IRepository<D_Marks_NormPrivilege> normRepository;
        private readonly CategoryTaxpayerService categoryRepository;
        private readonly Node node;
        private readonly IEnumerable<Node> calcTree;
        private readonly int categoryId;
        private readonly int sourceId;
        private readonly int periodId;
        private ParameterResolverDelegat parameterResolver;

        public FO41EstimateFormulaEvaluter(
            Node node, 
            IEnumerable<Node> calcTree,
            ILinqRepository<F_Marks_Privilege> factRepository, 
            IRepository<D_Marks_NormPrivilege> normRepository,
            CategoryTaxpayerService categoryRepository,
            int categoryId, 
            int sourceId,
            int periodId,
            FactResolverDelegat factResolver,
            IndicatorService indicatorRepository)
        {
            this.node = node;
            this.calcTree = calcTree;
            this.factRepository = factRepository;
            this.categoryId = categoryId;
            this.sourceId = sourceId;
            this.categoryId = categoryId;
            this.periodId = periodId;
            this.factResolver = factResolver;
            this.indicatorRepository = indicatorRepository;
            this.normRepository = normRepository;
            this.categoryRepository = categoryRepository;
        }

        public delegate F_Marks_Privilege FactResolverDelegat(
            int markId, 
            int applicationId, 
            int sourceId, 
            int periodId);

        public delegate decimal? ParameterResolverDelegat(F_Marks_Privilege fact);

        public delegate void ParameterSetterDelegat(decimal? result, F_Marks_Privilege fact);

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

            var fact = factResolver(node.ID, categoryId, sourceId, periodId);

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
            // если показатель - Корректирующий коэффициент необходимо брать из кл-ра 
            // «Организации.Категория налогоплательщика по льготам (d.Org.CategoryTaxpayer)» 
            if (name.ToUpper().Equals("KORK"))
            {
                args.Result = Convert.ToDouble(categoryRepository.Get(categoryId).CorrectIndex);
            }
            else
            {
                var parameterNode = calcTree.FirstOrDefault(x => x.Symbol == name);

                if (parameterNode == null)
                {
                    Trace.TraceError("Параметр {0} не найден.", name);
                    args.Result = Convert.ToDouble(0);
                    return;
                }

                var fact = factResolver(parameterNode.ID, categoryId, sourceId, periodId);
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
        }

        private void OnEvaluateFunction(string name, FunctionArgs args)
        {
            if (name.ToUpper() == "GET")
            {
                // символ показателя
                var symbol = args.Parameters[0].ParsedExpression.ToString().Trim();
                if (symbol.Contains("["))
                {
                    symbol = symbol.Replace("[", string.Empty);
                }

                if (symbol.Contains("]"))
                {
                    symbol = symbol.Replace("]", string.Empty);
                }

                // название периода из параметров функции
                var periodName = args.Parameters[1].ParsedExpression.ToString().Trim()
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                // если показатель - величина прожиточного минимума, 
                // берем из классификатора «Показатели.Нормативы по оценке эффективности льгот (d.Marks.NormPrivilege)»
                if (symbol.ToUpper().Equals("MIN"))
                {
                    var fact = normRepository.GetAll()
                        .FirstOrDefault(f => f.Symbol.Equals("MIN") && f.Year == periodId / 10000);
                    if (fact == null)
                    {
                        args.HasResult = false;
                    }
                    else
                    {
                        // в результат - соответствующее значение из факта))
                        if (periodName.ToUpper().Equals("FACT"))
                        {
                            args.Result = (double)fact.Fact;
                        }
                        else
                        {
                            if (periodName.ToUpper().Equals("PREVIOUSFACT"))
                            {
                                args.Result = (double)fact.PreviousFact;
                            }
                            else
                            {
                                if (periodName.ToUpper().Equals("ESTIMATE"))
                                {
                                    args.Result = (double)fact.Estimate;
                                }
                                else
                                {
                                    if (periodName.ToUpper().Equals("FORECAST"))
                                    {
                                        args.Result = (double)fact.Forecast;
                                    }
                                    else
                                    {
                                        args.HasResult = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // определить id показателя, у которого символ равен первому параметру
                    var indicatorId = indicatorRepository.GetIdBySymbol(symbol);

                    // найти соответствующий факт
                    var fact = factResolver(indicatorId, categoryId, sourceId, periodId);
                    
                    // в результат - соответствующее значение из факта
                    if (periodName.ToUpper().Equals("FACT"))
                    {
                        args.Result = (fact.Fact != null) ? (double)fact.Fact : 0.0;
                    }
                    else
                    {
                        if (periodName.ToUpper().Equals("PREVIOUSFACT"))
                        {
                            args.Result = (fact.PreviousFact != null) ? (double)fact.PreviousFact : 0.0;
                        }
                        else
                        {
                            if (periodName.ToUpper().Equals("ESTIMATE"))
                            {
                                args.Result = (fact.Estimate != null) ? (double)fact.Estimate : 0.0;
                            }
                            else
                            {
                                if (periodName.ToUpper().Equals("FORECAST"))
                                {
                                    args.Result = (fact.Forecast != null) ? (double)fact.Forecast : 0.0;
                                }
                                else
                                {
                                    args.HasResult = false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                args.HasResult = false;
            }
        }
    }
}
