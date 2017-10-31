using System;

namespace Krista.FM.Domain
{
	public class D_Org_DFunction : ClassifierTable
	{
		public static readonly string Key = "1bc195a8-d629-4436-b1bf-bd9d1b74cab6";

		public virtual int RowType { get; set; }
		public virtual string FunctionName { get; set; }
		public virtual string OtherName { get; set; }
	}
}
