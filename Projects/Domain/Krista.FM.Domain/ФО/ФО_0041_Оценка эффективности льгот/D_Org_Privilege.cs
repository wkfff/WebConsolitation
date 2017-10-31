using System;

namespace Krista.FM.Domain
{
	public class D_Org_Privilege : ClassifierTable
	{
		public static readonly string Key = "7427a790-79fc-4eec-8144-c2416034c2fb";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual long? Code { get; set; }
		public virtual string INN20 { get; set; }
		public virtual string LegalAddress { get; set; }
		public virtual string Address { get; set; }
		public virtual int? Unit { get; set; }
		public virtual string Register { get; set; }
		public virtual long? OKATO { get; set; }
		public virtual int? UserID { get; set; }
		public virtual B_Regions_Bridge RefBridgeRegions { get; set; }
	}
}
