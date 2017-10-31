using System;

namespace Krista.FM.Domain
{
	public class D_Territory_RF : ClassifierTable
	{
		public static readonly string Key = "66b9a66d-85ca-41de-910e-f9e6cb483960";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual string OKATO { get; set; }
		public virtual string Name { get; set; }
		public virtual string ShortName { get; set; }
		public virtual int? Sorting { get; set; }
		public virtual string Recital { get; set; }
		public virtual string Tenancy { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_Territory_RFBridge RefBridge { get; set; }
		public virtual FX_FX_TerritorialPartitionType RefTerritorialPartType { get; set; }
		public virtual B_Regions_Bridge RefRegionsBridge { get; set; }
		public virtual B_Regions_BridgePlan RefBridgeRegionsPlan { get; set; }
	}
}
