using Infragistics.Win.UltraWinTree;
using Krista.FM.Utils.DTSGenerator.SMOObjects;

namespace Krista.FM.Utils.DTSGenerator.TreeObjects
{
    /// <summary>
    /// Базовый класс для объектов дерева
    /// </summary>
    public class SSISBaseTreeNode : UltraTreeNode
    {
        /// <summary>
        /// Свойства объекта
        /// </summary>
        private SSISSMO smoObject;

        public SSISBaseTreeNode(SSISSMO smoObject)
        {
            this.smoObject = smoObject;
        }

        /// <summary>
        /// Свойства объекта
        /// </summary>
        public SSISSMO SmoObject
        {
            get { return smoObject; }
            set { smoObject = value; }
        }
    }
}