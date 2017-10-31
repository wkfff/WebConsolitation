using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;

using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    [JsonObject]
	public class D_Form_TableColumn : ClassifierTable
	{
		public static readonly string Key = "f3b8171e-b7f8-47c6-a206-dc82594db982";

        private IList<D_Form_TableCell> cells;

        public D_Form_TableColumn()
        {
            cells = new List<D_Form_TableCell>();
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Code { get; set; }
		public virtual int Ord { get; set; }
		public virtual string DataType { get; set; }
        public virtual int? DataTypeSize { get; set; }
        public virtual int? DataTypeScale { get; set; }
        public virtual bool ReadOnly { get; set; }
        [JsonIgnore]
        public virtual string InputMask { get; set; }
        [JsonIgnore]
        public virtual string DisplayMask { get; set; }
        [JsonIgnore]
        public virtual bool IsPersistent { get; set; }
		public virtual bool Required { get; set; }
		public virtual int? SortOrder { get; set; }
		public virtual int? SortDir { get; set; }
        [JsonIgnore]
        public virtual string DereferenceFields { get; set; }
        [JsonIgnore]
        public virtual string SearchFields { get; set; }
        [JsonIgnore]
        public virtual string Filter { get; set; }
        [JsonIgnore]
        public virtual string GroupTag { get; set; }
        [JsonIgnore]
        public virtual int GroupLevel { get; set; }
        public virtual string InternalName { get; set; }
        [JsonIgnore]
        public virtual D_Form_Part RefPart { get; set; }

	    [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefColumn")]
        public virtual IList<D_Form_TableCell> Cells
	    {
	        get { return cells; }
	        set { cells = value; }
	    }
	}
}
