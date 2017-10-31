using System;

namespace Krista.FM.Domain
{
	public class D_Organizations_Analysis : ClassifierTable
	{
		public static readonly string Key = "fe60e1ea-b8ea-4bfa-8da2-1a1b6ebb455a";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual string Name { get; set; }
		public virtual int Code { get; set; }
		public virtual string INN20 { get; set; }
		public virtual int? OKATO { get; set; }
		public virtual string LegalAddress { get; set; }
		public virtual string Address { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_Regions_Analysis RefRegion { get; set; }
		public virtual B_Organizations_Bridge RefBridge { get; set; }
	}
}
