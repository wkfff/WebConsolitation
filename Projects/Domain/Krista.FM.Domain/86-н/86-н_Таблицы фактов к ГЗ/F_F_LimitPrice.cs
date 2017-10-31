using System;

namespace Krista.FM.Domain
{
	public class F_F_LimitPrice : FactTable
	{
		public static readonly string Key = "25a5e8de-7232-4a6c-a76e-079f545a3ba3";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Name { get; set; }
		public virtual string Price { get; set; }
		public virtual F_F_GosZadanie RefFactGZ { get; set; }
	}
}
