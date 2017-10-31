using System;

namespace Krista.FM.Domain
{
	public class F_Marks_PassportMO : FactTable
	{
		public static readonly string Key = "bdefaedf-2eab-4bbb-ada3-725ad36533f3";

		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? OrigPlan { get; set; }
		public virtual Decimal? RefinPlan { get; set; }
		public virtual Decimal? Fact { get; set; }
		public virtual Decimal? FactPeriod { get; set; }
		public virtual Decimal? ScoreMO { get; set; }
		public virtual Decimal? ScoreS { get; set; }
		public virtual Decimal? Liability { get; set; }
		public virtual Decimal? OutstLiability { get; set; }
		public virtual Decimal? PlanPeriod { get; set; }
		public virtual Decimal? FactLastYear { get; set; }
		public virtual string Note { get; set; }
		public virtual Decimal? NoteFact { get; set; }
		public virtual D_Marks_PassportMO RefPassportMO { get; set; }
		public virtual FX_MeansType_SKIF RefPasMeans { get; set; }
		public virtual FX_Date_YearDayUNV RefPasPeriod { get; set; }
		public virtual D_Regions_Analysis RefPasRegions { get; set; }
		public virtual FX_BdgtLevels_SKIF RefPasBdgt { get; set; }
		public virtual FX_State_PassportMO RefStatePasMO { get; set; }
		public virtual D_PMO_Variant RefVar { get; set; }
	}
}
