using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Krista.FM.Common.Consolidation.Forms.ExcelMapping
{
    [DataContract]
    public class Sheet : Element
    {
        public Sheet()
        {
            Sections = new List<Section>();
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2, IsRequired = false)]
        public RequisiteMap HeaderRequisites { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public List<Section> Sections { get; set; }

        [DataMember(Order = 4, IsRequired = false)]
        public RequisiteMap FooterRequisites { get; set; }
    }
}
