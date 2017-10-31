using System;

namespace Krista.FM.Domain
{
	public class D_Regions_MonthRep : ClassifierTable
	{
		public static readonly string Key = "e6f74010-8362-4480-a999-279f5155dc9a";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual string CodeStr { get; set; }
		public virtual string Name { get; set; }
		public virtual string BudgetKind { get; set; }
		public virtual string BudgetName { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_Regions_Bridge RefRegionsBridge { get; set; }
		public virtual B_Territory_RFBridge RefTerrBridge { get; set; }
		public virtual B_KVSR_Bridge RefBridgeKVSR { get; set; }
		public virtual FX_DocType_SKIF RefDocType { get; set; }
		public virtual B_Regions_BridgePlan RefBridgeRegionsPlan { get; set; }
	}
}
