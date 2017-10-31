using System;

namespace Krista.FM.Domain
{
	public class D_Forecast_RevPlan : ClassifierTable
	{
		public static readonly string Key = "dc2d1b56-f08d-4226-b7f5-a8798da32a71";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual D_Forecast_PVars RefPVar { get; set; }
	}
}
