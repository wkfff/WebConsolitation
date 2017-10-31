using System;

namespace Krista.FM.Domain
{
	public class D_Org_GRBS : ClassifierTable
	{
		public static readonly string Key = "9c6e7a8d-bbca-4f48-8c37-bbcd416d587c";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual D_Org_PPO RefOrgPPO { get; set; }
	}
}
