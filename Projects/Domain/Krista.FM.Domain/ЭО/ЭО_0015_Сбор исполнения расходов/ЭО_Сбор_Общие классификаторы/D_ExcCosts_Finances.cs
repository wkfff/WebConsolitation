using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_Finances : ClassifierTable
	{
		public static readonly string Key = "8befa9da-279a-457f-bec3-7c2275fe15ca";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
	}
}
