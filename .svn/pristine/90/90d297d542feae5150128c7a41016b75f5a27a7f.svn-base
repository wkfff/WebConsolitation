using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Krista.FM.Common.Consolidation.Forms.ExcelMapping
{
    /// <summary>
    /// Метаданные разметки формы.
    /// </summary>
    [DataContract]
    public class Form
    {
        public Form()
        {
            Sheets = new List<Sheet>();
        }

        [DataMember(IsRequired = true)]
        public List<Sheet> Sheets { get; set; }
    }
}
