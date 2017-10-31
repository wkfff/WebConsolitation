using System.Collections.Generic;

namespace Krista.FM.Common.Calculator
{
    /// <summary>
    /// Узел результата. Предназначен для представления иерархии выходных значений.
    /// </summary>
    public interface IResultNode
    {
        string Name { get; }

        /// <summary>
        /// Уровень вложенности относительно корня.
        /// Заполняется в IPostprocessor.CreateDependOnTree.
        /// </summary>
        int? Level { get; set; }

        /// <summary>
        /// Значение, назначенное узлу
        /// </summary>
        IValueItem ValueItem { get; set; }

        IResultNode Parent { get; set; }

        ICollection<IResultNode> Children { get; }
    }
}
