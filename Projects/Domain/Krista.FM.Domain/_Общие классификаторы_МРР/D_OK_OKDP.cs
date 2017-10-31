using System;

namespace Krista.FM.Domain
{
	public class D_OK_OKDP : ClassifierTable
	{
		public static readonly string Key = "decc2e77-6234-4b20-9a92-0888946c7057";

		public virtual int RowType { get; set; }
		public virtual string CodeStr { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
