using System;

namespace Krista.FM.Domain
{
	public class B_Regions_BridgePlan : ClassifierTable
	{
		public static readonly string Key = "24962405-0ac5-48ed-83f6-127134116703";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual int? OKATO { get; set; }
		public virtual int? MapObjCode { get; set; }
		public virtual string ShortName { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual FX_FX_TerritorialPartitionType RefTerrType { get; set; }
	}
}
