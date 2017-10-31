using System;

namespace Krista.FM.Domain
{
	public class D_OKV_Currency : ClassifierTable
	{
		public static readonly string Key = "72f22b5d-fea9-4c6b-8c8d-a6df7ddc4db0";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string CodeLetter { get; set; }
		public virtual string Name { get; set; }
		public virtual string Country { get; set; }
		public virtual string Note { get; set; }
	}
}
