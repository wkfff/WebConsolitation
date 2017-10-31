using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Krista.FM.Common.Consolidation.Forms.ExcelMapping
{
    /// <summary>
    /// Метаданные разметки реквизитов.
    /// </summary>
    [DataContract]
    public class RequisiteMap
    {
        public RequisiteMap()
        {
            Requisites = new List<Element>();
        }

        [DataMember(Order = 1)]
        public string Region { get; set; }

        [DataMember(Order = 2)]
        public List<Element> Requisites { get; set; }
    }
}
