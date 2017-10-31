using Krista.FM.Utils.DTSGenerator.SMOObjects;

namespace Krista.FM.Utils.DTSGenerator.TreeObjects
{
    public class SSISMajorTreeBase<T> : SSISBaseTreeNode
    {
        /// <summary>
        /// Управляющий объект
        /// </summary>
        private T controlOblect;

        public SSISMajorTreeBase(T controlObject, SSISSMO smoObject)
            : base (smoObject)
        {
            this.controlOblect = controlObject;
        }

        public T ControlOblect
        {
            get { return controlOblect; }
            set { controlOblect = value; }
        }
    }
}