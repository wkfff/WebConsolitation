using System;

namespace Krista.FM.Domain
{
	public class FX_ExcCosts_TypeMark : ClassifierTable
	{
		public static readonly string Key = "13359b6f-58de-436a-98cd-dea2cd916342";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
	}
}
