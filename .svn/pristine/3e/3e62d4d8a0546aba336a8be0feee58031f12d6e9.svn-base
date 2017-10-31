using System;

namespace Krista.FM.Domain
{
	public class F_Marks_Privilege : FactTable
	{
		public static readonly string Key = "0601e5bd-4e65-43cd-88d0-0ea95bc76e54";

		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? PreviousFact { get; set; }
		public virtual Decimal? Fact { get; set; }
		public virtual Decimal? Estimate { get; set; }
		public virtual Decimal? Forecast { get; set; }
		public virtual FX_Date_YearDayUNV RefYearDayUNV { get; set; }
		public virtual D_Marks_Privilege RefMarks { get; set; }
		public virtual D_Org_CategoryTaxpayer RefCategory { get; set; }
	}
}
