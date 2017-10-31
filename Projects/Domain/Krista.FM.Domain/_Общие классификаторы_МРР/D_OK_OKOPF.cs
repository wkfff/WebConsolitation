using System;

namespace Krista.FM.Domain
{
	public class D_OK_OKOPF : ClassifierTable
	{
		public static readonly string Key = "c9fdd3b4-4902-4147-860e-c28ca25ae8fe";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
