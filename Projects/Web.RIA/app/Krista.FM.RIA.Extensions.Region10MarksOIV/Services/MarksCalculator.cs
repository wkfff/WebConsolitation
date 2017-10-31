using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using NCalc;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public class MarksCalculator : IMarksCalculator
    {
        private readonly IMarksRepository marksRepository;
        private readonly IFactRepository factRepository;

        private IEnumerable<Node> calcTree;

        private Dictionary<long, F_OIV_REG10Qual> cache;

        public MarksCalculator(
            IMarksRepository marksRepository,
            IFactRepository factRepository)
        {
            this.marksRepository = marksRepository;
            this.factRepository = factRepository;

            cache = new Dictionary<long, F_OIV_REG10Qual>();
        }

        public void Calc(IList<F_OIV_REG10Qual> facts, int territoryId, bool calculatePrognoz)
        {
            // Этап 1. Вычисление зависимостей показателей
            if (calcTree == null)
            {
                calcTree = PrepareCalcTree(marksRepository);
            }

            // Этап 2. Нахождение оптимальной последовательности вычислений
            var marks = facts.Select(fact => calcTree.FirstOrDefault(x => x.ID == fact.RefOIV.ID)).ToList();
            marks = marks.Where(x => x != null).ToList();
            var sequence = SearchSolutions(marks);

            // Этап 3. Выполнение вычислений
            foreach (var node in sequence)
            {
                if (node.Formula.IsNullOrEmpty())
                {
                    continue;
                }

                var formula = new FormulaEvaluter(node, calcTree, factRepository, territoryId, FactResolver);
                formula.Calc(fact => fact.Fact, (value, fact) => fact.Fact = value);
                if (calculatePrognoz)
                {
                    formula.Calc(fact => fact.Forecast, (value, fact) => fact.Forecast = value);
                    formula.Calc(fact => fact.Forecast2, (value, fact) => fact.Forecast2 = value);
                    formula.Calc(fact => fact.Forecast3, (value, fact) => fact.Forecast3 = value);
                }
            }
        }

        public F_OIV_REG10Qual FactResolver(int markId, int regionId)
        {
            long key = markId + (regionId << 16);

            F_OIV_REG10Qual fact;

            cache.TryGetValue(key, out fact);

            if (fact == null)
            {
                fact = factRepository.GetFactForMarkTerritory(markId, regionId);
                cache.Add(key, fact);
            }

            return fact;
        }

        private static void BuildCalculationSequence(IEnumerable<Node> root, List<Node> sequence)
        {
            var next = new List<Node>();

            foreach (var node in root)
            {
                if (!sequence.Contains(node))
                {
                    sequence.Add(node);
                }

                foreach (var node1 in node.Depended)
                {
                    if (sequence.Contains(node1))
                    {
                        sequence.Remove(node1);
                    }

                    if (!next.Contains(node1))
                    {
                        next.Add(node1);
                    }
                }
            }

            if (next.Count > 0)
            {
                BuildCalculationSequence(next, sequence);
            }
        }

        private static void BuildDependTree(IEnumerable<Node> data)
        {
            foreach (var node in data)
            {
                if (node.DependsOn.Count > 0)
                {
                    foreach (var depend in node.DependsOn)
                    {
                        if (!depend.Depended.Contains(node))
                        {
                            depend.Depended.Add(node);
                        }
                    }
                }
            }
        }

        private static void FindDependency(IEnumerable<Node> data)
        {
            foreach (var node in data)
            {
                if (node.Formula.IsNullOrEmpty())
                {
                    continue;
                }

                var matches = Regex.Matches(node.Formula, "([A-Za-zА-Яа-я]+[A-Za-zА-Яа-я0-9]*)");
                foreach (Match match in matches)
                {
                    if (match.Value == "if")
                    {
                        continue;
                    }

                    Match match1 = match;
                    var dependedNode = data.FirstOrDefault(x => x.Symbol == match1.Value);
                    if (dependedNode == null)
                    {
                        Trace.TraceWarning("Показатель с символом '{0}' не найден для формулы '{1}' ID={2}.", match.Value, node.Formula, node.ID);
                    }
                    else
                    {
                        if (!node.DependsOn.Any(x => x.ID == dependedNode.ID))
                        {
                            node.DependsOn.Add(dependedNode);
                        }
                    }
                }
            }
        }

        private static IEnumerable<Node> SearchSolutions(IEnumerable<Node> marks)
        {
            // Находим оптимальную последовательность для выполнения вычислений
            var sequence = new List<Node>();
            BuildCalculationSequence(marks, sequence);

            return sequence;
        }

        private static IEnumerable<Node> PrepareCalcTree(IMarksRepository marksRepository)
        {
            var data = (from m in marksRepository.FindAll()
                        where m.Symbol != null
                        select new Node { ID = m.ID, Symbol = m.Symbol, Formula = m.Formula })
                        .ToList();

            // По формулам определяем от кого зависит вычисляемый элемент
            FindDependency(data);

            // По зависимостям вычисляемых элементов строим обратные зависимости
            BuildDependTree(data);
            return data;
        }

        public class FormulaEvaluter
        {
            private readonly Node node;
            private readonly IEnumerable<Node> calcTree;
            private readonly IFactRepository factRepository;
            private readonly int regionId;
            private readonly FactResolverDelegat factResolver;

            private ParameterResolverDelegat parameterResolver;

            public FormulaEvaluter(Node node, IEnumerable<Node> calcTree, IFactRepository factRepository, int regionId, FactResolverDelegat factResolver)
            {
                this.node = node;
                this.calcTree = calcTree;
                this.factRepository = factRepository;
                this.regionId = regionId;
                this.factResolver = factResolver;
            }

            public delegate F_OIV_REG10Qual FactResolverDelegat(int markId, int regionId);

            public delegate decimal? ParameterResolverDelegat(F_OIV_REG10Qual fact);

            public delegate void ParameterSetterDelegat(decimal? result, F_OIV_REG10Qual fact);

            public void Calc(ParameterResolverDelegat resolver, ParameterSetterDelegat setter)
            {
                parameterResolver = resolver;

                Expression exp = new Expression(node.Formula);
                exp.EvaluateParameter += OnEvaluateParameter;
                double result = 0;
                bool hasResult = true;
                try
                {
                    result = Convert.ToDouble(exp.Evaluate());
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

                if (Double.IsNaN(result) || Double.IsInfinity(result))
                {
                    result = 0;
                }

                var fact = factResolver(node.ID, regionId);

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

                var fact = factResolver(parameterNode.ID, regionId);
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

        public class Node
        {
            public Node()
            {
                DependsOn = new List<Node>();
                Depended = new List<Node>();
            }

            public int ID { get; set; }

            public string Symbol { get; set; }

            public string Formula { get; set; }

            public List<Node> DependsOn { get; set; }

            public List<Node> Depended { get; set; }

            public int DependentCount()
            {
                return Depended.Sum(node => node.DependentCount() + 1);
            }

            public void Print(string indent)
            {
                Console.Write(indent + Symbol);
                foreach (var node in Depended)
                {
                    node.Print(indent + " ---> ");
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(Symbol);
                if (Formula.IsNotNullOrEmpty())
                {
                    sb.Append(" DependsOn=").Append(DependsOn.Count);
                    sb.Append(" = ").Append(Formula);
                }

                return sb.ToString();
            }
        }
    }
}
