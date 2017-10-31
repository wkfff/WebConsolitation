using System;

namespace Krista.FM.Domain
{
	public class D_Forecast_RevValues : ClassifierTable
	{
		public static readonly string Key = "1e6a1969-8699-48f6-a851-d1aadcdece2e";

		public virtual int RowType { get; set; }
		public virtual Decimal Value { get; set; }
		public virtual D_Forecast_RevPlan RefRev { get; set; }
		public virtual D_Forecast_PParams RefParam { get; set; }
		public virtual FX_Date_YearDayUNV RefDate { get; set; }
	}
}
