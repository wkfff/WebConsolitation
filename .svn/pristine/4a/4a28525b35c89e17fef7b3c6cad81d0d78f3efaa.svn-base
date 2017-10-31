using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.FO41.Helpers
{
    public class Calculator : ICalculator
    {
        protected IEnumerable<Node> CalcTree { get; set; }

        protected string Pattern { get; set; }

        public void Calc(IList<object> facts, int regionId, bool calculatePrognoz)
        {
            Calc(facts, regionId, calculatePrognoz);
        }

        protected static void BuildDependTree(IEnumerable<Node> data)
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

        protected static IEnumerable<Node> SearchSolutions(IEnumerable<Node> marks)
        {
            // Находим оптимальную последовательность для выполнения вычислений
            var sequence = new List<Node>();
            BuildCalculationSequence(marks, sequence);

            return sequence;
        }

        protected void FindDependency(IEnumerable<Node> data)
        {
            foreach (var node in data)
            {
                if (node.Formula.IsNullOrEmpty())
                {
                    continue;
                }

                var matches = Regex.Matches(node.Formula, Pattern);

                foreach (Match match in matches)
                {
                    if (match.Value == "if")
                    {
                        continue;
                    }

                    var match1 = match;

                    Node dependedNode = GetDependedNode(data, match1);

                    if (dependedNode == null)
                    {
                        Console.WriteLine(@"Показатель с символом '{0}' не найден для формулы '{1}' ID={2}.", match.Value, node.Formula, node.ID);
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

        protected virtual Node GetDependedNode(IEnumerable<Node> data, Match match1)
        {
            return data.FirstOrDefault(x => x.Symbol == match1.Value.Substring(0, match1.Value.IndexOf("[")));
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
    }
}
