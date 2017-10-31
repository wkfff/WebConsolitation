using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class ResultNode : IResultNode
    {
        public ResultNode(string name)
        {
            Name = name;
            Level = null;
            Parent = null;
            Children = new Collection<IResultNode>();
        }

        #region IResultNode

        public string Name { get; private set; }

        public int? Level { get; set; }

        public IResultNode Parent { get; set; }

        public ICollection<IResultNode> Children { get; private set; }

        public IValueItem ValueItem { get; set; }

        #endregion
    }
}