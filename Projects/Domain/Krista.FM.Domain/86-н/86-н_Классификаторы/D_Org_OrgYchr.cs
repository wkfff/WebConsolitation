using System;

namespace Krista.FM.Domain
{
	public class D_Org_OrgYchr : ClassifierTable
	{
		public static readonly string Key = "019230a4-efda-4017-9144-aeb18c16a71a";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual D_Org_NsiOGS RefNsiOgs { get; set; }
	}
}
