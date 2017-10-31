using System;

namespace Krista.FM.Domain
{
	public class D_Forecast_PVars : ClassifierTable
	{
		public static readonly string Key = "1de3ff6b-eb81-41d1-8932-3d3827718988";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual int UserID { get; set; }
		public virtual string XMLString { get; set; }
		public virtual int Status { get; set; }
		public virtual int Method { get; set; }
		public virtual int? Period { get; set; }
		public virtual D_Forecast_PParams RefParam { get; set; }
		public virtual FX_Date_YearDayUNV RefDate { get; set; }
	}
}
