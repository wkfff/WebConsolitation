using System;

namespace Krista.FM.Domain
{
	public class D_Services_CatPotr : ClassifierTable
	{
		public static readonly string Key = "7e260d82-8087-4096-ac9d-01c25ae897b9";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime? DataVk { get; set; }
		public virtual DateTime? DataIsk { get; set; }
		public virtual string Status { get; set; }
		public virtual string Code { get; set; }
	}
}
