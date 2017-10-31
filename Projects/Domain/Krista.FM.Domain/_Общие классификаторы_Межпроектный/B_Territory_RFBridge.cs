using System;

namespace Krista.FM.Domain
{
	public class B_Territory_RFBridge : ClassifierTable
	{
		public static readonly string Key = "3d6959a5-3ce6-439d-a5f1-b17aeeedfa37";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual string OKATO { get; set; }
		public virtual string Name { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string Note { get; set; }
		public virtual string Chortcut { get; set; }
		public virtual int? IdNumber { get; set; }
		public virtual int? MapObjCode { get; set; }
		public virtual bool? Abolition { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual FX_FX_TerritorialPartitionType RefPartitionType { get; set; }
	}
}
