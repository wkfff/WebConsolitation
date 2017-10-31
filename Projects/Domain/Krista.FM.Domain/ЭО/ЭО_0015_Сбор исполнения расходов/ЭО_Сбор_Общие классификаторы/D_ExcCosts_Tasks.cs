using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_Tasks : ClassifierTable
	{
		public static readonly string Key = "fbb19e7e-daa0-43c5-822d-982c85e50aa8";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual D_ExcCosts_Goals RefGoal { get; set; }
	}
}
