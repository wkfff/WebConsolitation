using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;

using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    [JsonObject]
    public class D_Form_TableRow : ClassifierTable
	{
		public static readonly string Key = "653f8f68-d08a-42fd-a041-b167fdc5dc33";

        private IList<D_Form_TableCell> cells;

        public D_Form_TableRow()
        {
            cells = new List<D_Form_TableCell>();
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual bool Multiplicity { get; set; }
		public virtual bool ReadOnly { get; set; }
		public virtual int Ord { get; set; }
        [JsonIgnore]
		public virtual D_Form_Part RefPart { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefRow")]
        public virtual IList<D_Form_TableCell> Cells
        {
            get { return cells; }
            set { cells = value; }
        }
	}
}
