using Krista.FM.Common.Calculator.Exceptions;
using Krista.FM.Common.Calculator.Utils;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class Postprocessor : IPostprocessor
    {
        #region IPostprocessor

        public IValueItemStore ValueItemStore { get; set; }

        public IResultNode CreateNode(IFormula formula)
        {
            return new ResultNode(formula.Name) { Level = 0, Parent = null };
        }

        public IResultNode CreateDependOnTree(IFormula formula)
        {
            return BuildDependOnTree(formula, null, 0);
        }

        public void BindResults(IResultNode root)
        {
            if (ValueItemStore == null)
            {
                throw new InvalidConfigurationException("Не указан источник данных");
            }

            var nodes = root.AsPlainList();
            foreach (var resultNode in nodes)
            {
                BindResultToNode(resultNode);
            }
        }

        #endregion

        private static IResultNode BuildDependOnTree(IFormula formula, IResultNode parent, int level)
        {
            if (level > CalculatorExtensions.MaxDependencyDepth)
            {
                throw new CircularReferenceException();
            }

            var node = new ResultNode(formula.Name) { Level = level, Parent = parent };

            foreach (var dependOnConstant in formula.DependOnConstants)
            {
                node.Children.Add(new ResultNode(dependOnConstant.Name) { Level = level + 1, Parent = node });
            }

            foreach (var dependOnFormula in formula.DependOnFormulas)
            {
                node.Children.Add(BuildDependOnTree(dependOnFormula, node, level + 1));
            }

            return node;
        }

        protected void BindResultToNode(IResultNode node)
        {
            node.ValueItem = ValueItemStore.Get(node.Name);
        }
    }
}
