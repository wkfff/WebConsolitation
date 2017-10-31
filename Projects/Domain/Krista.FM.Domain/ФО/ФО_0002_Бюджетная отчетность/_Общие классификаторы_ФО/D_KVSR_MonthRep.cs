using System;

namespace Krista.FM.Domain
{
	public class D_KVSR_MonthRep : ClassifierTable
	{
		public static readonly string Key = "ac763971-1e48-438e-9b47-7ff69e0deeda";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual int KL { get; set; }
		public virtual int KST { get; set; }
		public virtual B_KVSR_MonthRepBridge RefBridgeKVSR { get; set; }
		public virtual B_KVSR_Bridge RefKVSR { get; set; }
	}
}
