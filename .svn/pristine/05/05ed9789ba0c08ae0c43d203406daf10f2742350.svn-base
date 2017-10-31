using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.ViewModel
{
    public class EntityBookViewModel
    {
        public enum ShowModeType 
        { 
            /// <summary>
            /// обычное отображение
            /// </summary>
            Normal, 
           
            /// <summary>
            /// не отображать дочерние элементы
            /// </summary>
            WithoutHierarchy 
        }

        public IClassifier Entity { get; set; }
        
        public int SourceId { get; set; }
        
        public string Filter { get; set; }

        /// <summary>
        /// noHierarchy - отображать иерархические классификаторы без иерархии.
        /// normal - отображать в обычном режиме.
        /// </summary>
        public ShowModeType ShowMode { get; set; }
    }
}
