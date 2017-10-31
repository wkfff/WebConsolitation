using System.Runtime.Serialization;

namespace Krista.FM.Common.Consolidation.Forms.ExcelMapping
{
    /// <summary>
    /// Метаданные разметки раздела формы.
    /// </summary>
    [DataContract]
    public class Section : Element
    {
        [DataMember(Order = 1, IsRequired = false)]
        public RequisiteMap HeaderRequisites { get; set; }

        [DataMember(Order = 2)]
        public Table Table { get; set; }

        [DataMember(Order = 3, IsRequired = false)]
        public RequisiteMap FooterRequisites { get; set; }
    }
}
