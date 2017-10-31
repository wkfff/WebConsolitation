namespace Krista.FM.Common.Calculator
{
    public interface IPostprocessor
    {
        IValueItemStore ValueItemStore { get; set; }

        /// <summary>
        /// Создает пустой узел.
        /// </summary>
        IResultNode CreateNode(IFormula formula);

        /// <summary>
        /// Создает пустой узел, а также строит полное дерево подчиненных ему узлов (пустых).
        /// </summary>
        IResultNode CreateDependOnTree(IFormula formula);

        /// <summary>
        /// Наполняет переданное дерево значениями.
        /// </summary>
        void BindResults(IResultNode root);
    }
}
