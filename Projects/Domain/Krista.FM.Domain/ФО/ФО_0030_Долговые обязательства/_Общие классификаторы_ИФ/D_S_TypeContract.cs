using System;

namespace Krista.FM.Domain
{
	public class D_S_TypeContract : ClassifierTable
	{
		public static readonly string Key = "86eaa846-10fd-4645-adc3-ae19c3f69353";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
