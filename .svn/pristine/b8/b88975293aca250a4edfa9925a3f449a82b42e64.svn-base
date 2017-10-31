using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using NCalc;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class MarksCalculator : IMarksCalculator
    {
        public const int Precision = 4;

        private readonly IMarksRepository marksRepository;
        private readonly IMarksOmsuRepository factRepository;

        private IEnumerable<Node> calcTree;

        private Dictionary<long, F_OMSU_Reg16> cache;

        public MarksCalculator(
            IMarksRepository marksRepository,
            IMarksOmsuRepository factRepository)
        {
            this.marksRepository = marksRepository;
            this.factRepository = factRepository;

            cache = new Dictionary<long, F_OMSU_Reg16>();
        }

        public void Calc(IList<F_OMSU_Reg16> facts, int regionId, bool calculatePrognoz)
        {
            DoCalc(facts, regionId, calculatePrognoz, false, false);
        }

        public void CalcForceProtected(IList<F_OMSU_Reg16> facts, int regionId, bool calculatePrognoz)
        {
            DoCalc(facts, regionId, calculatePrognoz, true, false);
        }

        public void CalcForceProtectedDoNotSave(IList<F_OMSU_Reg16> facts, int regionId, bool calculatePrognoz)
        {
            DoCalc(facts, regionId, calculatePrognoz, true, true);
        }

        public IDictionary<D_OMSU_MarksOMSU, int> GetMarkCalculationPlan(int markId)
        {
            if (calcTree == null)
            {
                calcTree = PrepareCalcTree(marksRepository);
            }

            IEnumerable<Node> targetNode = calcTree.Where(x => (x != null) && (x.ID == markId));
            if (targetNode.Count() == 0)
            {
                throw new InvalidDataException("Found no target mark with id=" + markId);
            }

            if (targetNode.Count() > 1)
            {
                throw new InvalidDataException("Found multiple target marks with id=" + markId);
            }

            var sequence = new List<Node>();
            var hierarchy = new Dictionary<Node, int>();
            WalkDependencesDistinct(targetNode.First(), 0, sequence, hierarchy);

            return hierarchy.ToDictionary(leveledNode => marksRepository.FindOne(leveledNode.Key.ID), leveledNode => leveledNode.Value);
        }

        public F_OMSU_Reg16 FactResolver(int markId, int regionId)
        {
            long key = markId + (regionId << 16);

            F_OMSU_Reg16 fact;

            cache.TryGetValue(key, out fact);

            if (fact == null)
            {
                fact = factRepository.GetFactForMarkRegion(markId, regionId);
                cache.Add(key, fact);
            }

            return fact;
        }

        private static bool FactIsReadonly(F_OMSU_Reg16 fact)
        {
            return fact.RefStatusData.ID == (int)OMSUStatus.OnEdit
                   || fact.RefStatusData.ID == (int)OMSUStatus.OnReview
                   || fact.RefStatusData.ID == (int)OMSUStatus.Undefined;
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

        private void WalkDependencesDistinct(Node root, int baseLevel, IList<Node> outputBuffer, IDictionary<Node, int> outputNodeHierarchy)
        {
            foreach (var dependOnNode in root.DependsOn.Where(dependOnNode => !outputBuffer.Contains(dependOnNode)))
            {
                outputBuffer.Add(dependOnNode);
                outputNodeHierarchy.Add(new KeyValuePair<Node, int>(dependOnNode, baseLevel));
                var mark = marksRepository.FindOne(dependOnNode.ID);
                if (mark.RefTypeMark.ID != (int)TypeMark.Gather)
                {
                    WalkDependencesDistinct(dependOnNode, baseLevel + 1, outputBuffer, outputNodeHierarchy);
                }                
            }
        }

        private void DoCalc(IEnumerable<F_OMSU_Reg16> facts, int regionId, bool calculatePrognoz, bool forceProtectedRecalc, bool doNotSave)
        {
            // Этап 1. Вычисление зависимостей показателей
            if (calcTree == null)
            {
                calcTree = PrepareCalcTree(marksRepository);
            }

            // Этап 2. Нахождение оптимальной последовательности вычислений
            var marks = facts.Select(fact => calcTree.FirstOrDefault(x => x.ID == fact.RefMarksOMSU.ID)).ToList();
            marks = marks.Where(x => x != null).ToList();
            var sequence = SearchSolutions(marks);

            // Этап 3. Выполнение вычислений
            foreach (var node in sequence)
            {
                if (node.Formula.IsNullOrEmpty())
                {
                    continue;
                }

                var formula = new FormulaEvaluter(node, calcTree, factRepository, regionId, FactResolver);

                var allowPrognozCalc = calculatePrognoz && !marksRepository.FindAll().Any(x => x.ID == node.ID && x.PrognMO);

                formula.Calc(fact => fact.CurrentValue, (value, fact) => fact.CurrentValue = forceProtectedRecalc || FactIsReadonly(fact) ? value : fact.CurrentValue, doNotSave);
                formula.Calc(fact => fact.PriorValue, (value, fact) => fact.PriorValue = forceProtectedRecalc || FactIsReadonly(fact) ? value : fact.PriorValue, doNotSave);
                if (allowPrognozCalc)
                {
                    formula.Calc(fact => fact.Prognoz1, (value, fact) => fact.Prognoz1 = forceProtectedRecalc || FactIsReadonly(fact) ? value : fact.Prognoz1, doNotSave);
                    formula.Calc(fact => fact.Prognoz2, (value, fact) => fact.Prognoz2 = forceProtectedRecalc || FactIsReadonly(fact) ? value : fact.Prognoz2, doNotSave);
                    formula.Calc(fact => fact.Prognoz3, (value, fact) => fact.Prognoz3 = forceProtectedRecalc || FactIsReadonly(fact) ? value : fact.Prognoz3, doNotSave);
                }
            }
        }

        public class FormulaEvaluter
        {
            private readonly Node node;
            private readonly IEnumerable<Node> calcTree;
            private readonly IMarksOmsuRepository factRepository;
            private readonly int regionId;
            private readonly FactResolverDelegat factResolver;

            private ParameterResolverDelegat parameterResolver;

            public FormulaEvaluter(Node node, IEnumerable<Node> calcTree, IMarksOmsuRepository factRepository, int regionId, FactResolverDelegat factResolver)
            {
                this.node = node;
                this.calcTree = calcTree;
                this.factRepository = factRepository;
                this.regionId = regionId;
                this.factResolver = factResolver;
            }

            public delegate F_OMSU_Reg16 FactResolverDelegat(int markId, int regionId);

            public delegate decimal? ParameterResolverDelegat(F_OMSU_Reg16 fact);

            public delegate void ParameterSetterDelegat(decimal? result, F_OMSU_Reg16 fact);

            public void Calc(ParameterResolverDelegat resolver, ParameterSetterDelegat setter, bool doNotSave)
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

                if (!doNotSave)
                {
                    factRepository.Save(fact);
                }                
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

                var val = parameterResolver(fact) ?? 0;

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

            public int ID { get; set; } //// Совпадает с ID показателя D_OMSU_MarksOMSU

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
