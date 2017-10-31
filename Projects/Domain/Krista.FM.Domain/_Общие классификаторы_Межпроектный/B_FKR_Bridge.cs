using System;

namespace Krista.FM.Domain
{
	public class B_FKR_Bridge : ClassifierTable
	{
		public static readonly string Key = "1acb453b-dd86-438a-83b9-c27ce4fd8bda";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_FKR_Bridge RefFKRBridge { get; set; }
	}
}
