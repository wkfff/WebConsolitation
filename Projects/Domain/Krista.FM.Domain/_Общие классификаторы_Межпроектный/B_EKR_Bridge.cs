using System;

namespace Krista.FM.Domain
{
	public class B_EKR_Bridge : ClassifierTable
	{
		public static readonly string Key = "9a85c0c8-390d-41cb-839c-f57ef54f7ff3";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_EKR_Bridge RefEKRBridge { get; set; }
	}
}
