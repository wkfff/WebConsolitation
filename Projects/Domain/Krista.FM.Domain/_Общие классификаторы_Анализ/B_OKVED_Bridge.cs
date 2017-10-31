using System;

namespace Krista.FM.Domain
{
	public class B_OKVED_Bridge : ClassifierTable
	{
		public static readonly string Key = "a5f87962-8af2-4419-8b5a-746a1e3540e8";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual int? Code4 { get; set; }
		public virtual int? Code5 { get; set; }
		public virtual int? Code6 { get; set; }
		public virtual int? Code7 { get; set; }
		public virtual string Name { get; set; }
		public virtual string Section { get; set; }
		public virtual string SubSection { get; set; }
		public virtual string ShortName { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_KVSR_Department RefKVSRDepartment { get; set; }
		public virtual B_OKVED_Bridge RefOKVEDBridge { get; set; }
	}
}
