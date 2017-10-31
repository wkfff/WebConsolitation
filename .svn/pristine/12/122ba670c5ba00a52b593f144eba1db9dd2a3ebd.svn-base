using System.Runtime.Serialization;

namespace Krista.FM.Common.Consolidation.Forms.ExcelMapping
{
    /// <summary>
    /// Элемент разметки формы на документе Ecxel.
    /// </summary>
    [DataContract]
    public class Element
    {
        /// <summary>
        /// Код элемента.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Code { get; set; }

        /// <summary>
        /// Имя диапазона.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Region { get; set; }

        public override string ToString()
        {
            return Region + '(' + Code + ')';
        }
    }
}
