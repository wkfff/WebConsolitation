using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Krista.FM.Common.Calculator.Exceptions;
using NCalc;

namespace Krista.FM.Common.Calculator.Utils
{
    public static class CalculatorExtensions
    {
        public const int MaxDependencyDepth = 100;

        public static void Calculate(this ICalculator calculator, IList<IFormula> sequence)
        {
            foreach (var formula in sequence)
            {
                calculator.Calculate(formula);
            }
        }

        /// <summary>
        /// Ищет узел с указанным именем во всем дереве, подчиненном указанному узлу.
        /// </summary>
        public static IResultNode FindNodeInTree(this IResultNode root, string nodeName)
        {
            if (root.Name == nodeName)
            {
                return root;
            }

            foreach (var child in root.Children)
            {
                var candidate = child.FindNodeInTree(nodeName);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return null;
        }

        /// <summary>
        /// Возвращает неповторяющийся список узлов, состоящий из указанного узла и всех подчиненных ему.
        /// </summary>
        public static IList<IResultNode> AsPlainList(this IResultNode root)
        {
            var result = new List<IResultNode> { root };

            foreach (var child in root.Children)
            {
                result.AddRange(child.AsPlainList());
            }

            return result;
        }

        /// <summary>
        /// Возвращает неповторяющийся список результатов переданого узла и всех его подчиненных.
        /// Значение каждого узла включается в список, если valueItemTester на этом значении возвращает True.
        /// </summary>
        public static IList<IValueItem> ExtractValues(this IResultNode root, Func<IValueItem, bool> valueItemTester)
        {
            var result = new List<IValueItem>();
            DoExtractValues(result, root, valueItemTester, 0);
            return result;
        }

        /// <summary>
        /// Оставляет в исходном дереве только те узлы дерева, для которых nodeTester возвращает True.
        /// (!) Удаление элементов происходит из исходного дерева.
        /// </summary>
        public static IResultNode FilterNodes(this IResultNode root, Func<IResultNode, bool> nodeTester)
        {
            DoFilterNodes(root, nodeTester, 0);
            return root;
        }

        /// <summary>
        /// Объединяет две коллекции, не допуская повторений элементов. Исходная коллекция на наличие повторений не проверяется.
        /// </summary>
        public static ICollection<IFormula> Union(this ICollection<IFormula> formulas, ICollection<IFormula> sourceFormulas)
        {
            foreach (var formula in sourceFormulas.Where(formula => !formulas.Contains(formula)))
            {
                formulas.Add(formula);
            }

            return formulas;
        }

        /// <summary>
        /// Объединяет две коллекции, не допуская повторений элементов. Исходная коллекция на наличие повторений не проверяется.
        /// </summary>
        public static ICollection<IConstant> Union(this ICollection<IConstant> constants, ICollection<IConstant> sourceConstants)
        {
            foreach (var constant in sourceConstants.Where(formula => !constants.Contains(formula)))
            {
                constants.Add(constant);
            }

            return constants;
        }

        /// <summary>
        /// Объединяет две коллекции, не допуская повторений элементов. Исходная коллекция на наличие повторений не проверяется.
        /// </summary>
        public static ICollection<IResultNode> Union(this ICollection<IResultNode> nodes, ICollection<IResultNode> sourceNodes)
        {
            foreach (var resultNode in sourceNodes.Where(formula => !nodes.Contains(formula)))
            {
                nodes.Add(resultNode);
            }

            return nodes;
        }

        public static object EvaluateOrNullOnFail(this Expression expression)
        {
            object result = null;

            try
            {
                result = expression.Evaluate();
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка вычисления выражения {0}: {1}", expression.ParsedExpression, e.Message);
            }

            return result;
        }

        public static void DoExtractValues(IList<IValueItem> buffer, IResultNode root, Func<IValueItem, bool> valueItemTester, int level)
        {
            if (level > MaxDependencyDepth)
            {
                throw new CircularReferenceException();
            }

            if (valueItemTester(root.ValueItem) && !buffer.Contains(root.ValueItem))
            {
                buffer.Add(root.ValueItem);
            }

            foreach (var child in root.Children)
            {
                DoExtractValues(buffer, child, valueItemTester, level + 1);
            }
        }

        public static void DoFilterNodes(IResultNode root, Func<IResultNode, bool> nodeTester, int level)
        {
            if (level > MaxDependencyDepth)
            {
                throw new CircularReferenceException();
            }

            /* Корневой узел не может быть удален */
            if (root == null)
            {
                return;
            }

            var children = root.Children.ToList();
            for (var i = children.Count - 1; i >= 0; i--)
            {
                if (!nodeTester(children[i]))
                {
                    root.Children.Remove(children[i]);
                    children.RemoveAt(i);
                }
            }

            foreach (var child in root.Children)
            {
                DoFilterNodes(child, nodeTester, level + 1);
            }
        }
    }
}
