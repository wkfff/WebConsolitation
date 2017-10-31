using System;

namespace Krista.FM.Domain
{
	public class D_OKFS_OKFS : ClassifierTable
	{
		public static readonly string Key = "3bf7a41a-c7f7-4b01-aa93-8a7e92e2fe5b";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual int Code { get; set; }
	}
}
