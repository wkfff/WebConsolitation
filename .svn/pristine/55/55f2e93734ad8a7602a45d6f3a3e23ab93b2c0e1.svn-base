using System;

namespace Krista.FM.Domain
{
	public class F_Arrears_Liability : FactTable
	{
		public static readonly string Key = "6e3dbded-055f-49c3-8c6b-b24de9b63b23";

		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? Fact { get; set; }
		public virtual D_Arrears_Marks RefLiabilityMarks { get; set; }
		public virtual FX_Arrears_LiabilityType RefLiabilityType { get; set; }
		public virtual D_KVSR_Analysis RefLiabilityKVSR { get; set; }
		public virtual FX_Date_YearDayUNV RefLiabilityPeriod { get; set; }
		public virtual D_Arrears_Debt RefDebt { get; set; }
		public virtual D_FKR_Analysis RefFKR { get; set; }
		public virtual FX_MeansType_SKIF RefMeans { get; set; }
		public virtual FX_BdgtLevels_SKIF RefBdgt { get; set; }
	}
}
