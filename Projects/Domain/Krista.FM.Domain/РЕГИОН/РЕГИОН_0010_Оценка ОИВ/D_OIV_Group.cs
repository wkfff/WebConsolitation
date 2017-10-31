using System;

namespace Krista.FM.Domain
{
	public class D_OIV_Group : ClassifierTable
	{
		public static readonly string Key = "a6d879cb-12ce-4e03-b2d8-1933b1426456";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
	}
}
