using System;

namespace Krista.FM.Domain
{
	public class D_Org_Good : ClassifierTable
	{
		public static readonly string Key = "cdb1abde-8f13-47b5-9d10-9175d1b217f7";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_Units_OKEI RefUnits { get; set; }
		public virtual B_Org_ProductBridge RefProductBridge { get; set; }
		public virtual FX_Org_ProdGroup RefProdGroup { get; set; }
	}
}
