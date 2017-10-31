using System;

namespace Krista.FM.Domain
{
	public class D_MassMedia_Period : ClassifierTable
	{
		public static readonly string Key = "b2f55a5c-eddd-4e58-90b4-9fda9004883d";

		public virtual int RowType { get; set; }
		public virtual string PeriodName { get; set; }
		public virtual string OtherName { get; set; }
	}
}
