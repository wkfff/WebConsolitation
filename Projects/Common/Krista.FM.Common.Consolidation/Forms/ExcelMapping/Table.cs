using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Krista.FM.Common.Consolidation.Forms.ExcelMapping
{
    /// <summary>
    /// Метаданные разметки раздела формы.
    /// </summary>
    [DataContract]
    public class Table
    {
        public Table()
        {
            Columns = new List<Element>();
            Rows = new List<Element>();
        }

        [DataMember(Order = 1, IsRequired = true)]
        public string Region { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string HeaderRegion { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string RowsRegion { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public List<Element> Columns { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public List<Element> Rows { get; set; }
    }
}
