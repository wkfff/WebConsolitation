using System;

namespace Krista.FM.Domain
{
	public class D_KCSR_FOYR : ClassifierTable
	{
		public static readonly string Key = "3e1f3525-5ebf-439a-bc7b-0e7a51a6e6c3";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual string Name { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_KCSR_Bridge RefBridge { get; set; }
	}
}
