using System;

namespace Krista.FM.Domain
{
	public class D_Forecast_IdicPlan : ClassifierTable
	{
		public static readonly string Key = "45ba8d46-efa7-4e86-81f5-f613020f3f8a";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual int UserId { get; set; }
		public virtual int Parent { get; set; }
	}
}
