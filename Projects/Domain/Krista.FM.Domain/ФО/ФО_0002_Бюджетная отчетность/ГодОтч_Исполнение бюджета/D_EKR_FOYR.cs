using System;

namespace Krista.FM.Domain
{
	public class D_EKR_FOYR : ClassifierTable
	{
		public static readonly string Key = "e03d01a7-4956-4017-8d14-84b39cdebaff";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual int? Code4 { get; set; }
		public virtual int? Code5 { get; set; }
		public virtual string Name { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_EKR_Bridge RefEKRBridge { get; set; }
		public virtual B_Marks_PassportMO RefPEKRBridge { get; set; }
	}
}
