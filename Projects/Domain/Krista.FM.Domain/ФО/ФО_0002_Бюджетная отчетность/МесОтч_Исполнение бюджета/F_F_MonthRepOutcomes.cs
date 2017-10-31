using System;

namespace Krista.FM.Domain
{
	public class F_F_MonthRepOutcomes : FactTable
	{
		public static readonly string Key = "ecd2edd2-030f-4b17-9fdf-0b61ea99de80";

		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? YearPlanReport { get; set; }
		public virtual Decimal? QuarterPlanReport { get; set; }
		public virtual Decimal? MonthPlanReport { get; set; }
		public virtual Decimal? FactReport { get; set; }
		public virtual Decimal? YearPlan { get; set; }
		public virtual Decimal? QuarterPlan { get; set; }
		public virtual Decimal? MonthPlan { get; set; }
		public virtual Decimal? Fact { get; set; }
		public virtual Decimal? SpreadFactYearPlan { get; set; }
		public virtual Decimal? SpreadFactMonthPlan { get; set; }
		public virtual Decimal? SpreadFactYearPlanReport { get; set; }
		public virtual Decimal? SpreadFactMonthPlanReport { get; set; }
		public virtual Decimal? ExcSumP { get; set; }
		public virtual Decimal? ExcSumPRep { get; set; }
		public virtual Decimal? ExcSumF { get; set; }
		public virtual Decimal? ExcSumFRep { get; set; }
		public virtual FX_MeansType_SKIF RefMeansType { get; set; }
		public virtual FX_BdgtLevels_SKIF RefBdgtLevels { get; set; }
		public virtual D_Regions_MonthRep RefRegions { get; set; }
		public virtual D_R_MonthRep RefFKR { get; set; }
		public virtual D_EKR_MonthRep RefEKR { get; set; }
		public virtual FX_Date_YearDayUNV RefYearDayUNV { get; set; }
		public virtual D_KVSR_MonthRep RefKVSR { get; set; }
	}
}
