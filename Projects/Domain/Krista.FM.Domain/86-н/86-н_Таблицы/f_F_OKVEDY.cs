using System;

namespace Krista.FM.Domain
{
	public class F_F_OKVEDY : FactTable
	{
		public static readonly string Key = "b77e2439-cafa-4858-9294-c8651c380af7";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Name { get; set; }
		public virtual D_Org_PrOKVED RefPrOkved { get; set; }
		public virtual D_OKVED_OKVED RefOKVED { get; set; }
		public virtual F_Org_Passport RefPassport { get; set; }
	}
}
