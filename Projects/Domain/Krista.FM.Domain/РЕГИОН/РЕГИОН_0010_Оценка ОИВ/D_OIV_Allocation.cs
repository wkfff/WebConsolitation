using System;

namespace Krista.FM.Domain
{
	public class D_OIV_Allocation : ClassifierTable
	{
		public static readonly string Key = "5fff1811-e86b-4fb9-835f-7d3e76e0bd27";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
	}
}
