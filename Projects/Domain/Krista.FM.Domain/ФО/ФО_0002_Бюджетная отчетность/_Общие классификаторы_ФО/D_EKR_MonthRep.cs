using System;

namespace Krista.FM.Domain
{
	public class D_EKR_MonthRep : ClassifierTable
	{
		public static readonly string Key = "86f29f0a-5e84-48ba-8f64-1375439498f9";

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
		public virtual B_EKR_Bridge RefBridgeEKR { get; set; }
		public virtual B_Marks_PassportMO RefBridgePassEKR { get; set; }
	}
}
