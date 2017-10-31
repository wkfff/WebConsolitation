using System;

namespace Krista.FM.Domain
{
	public class F_R_FOYROutcomes : FactTable
	{
		public static readonly string Key = "6385095a-5d23-48ef-861f-1df393dad0cf";

		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? AssignedReport { get; set; }
		public virtual Decimal? PerformedReport { get; set; }
		public virtual Decimal? Assigned { get; set; }
		public virtual Decimal? Performed { get; set; }
		public virtual FX_BdgtLevels_SKIF RefBdgtLevels { get; set; }
		public virtual FX_MeansType_SKIF RefMeansType { get; set; }
		public virtual D_Regions_FOYR RefRegions { get; set; }
		public virtual D_KVR_FOYR RefKVR { get; set; }
		public virtual D_KCSR_FOYR RefKCSR { get; set; }
		public virtual D_FKR_FOYR RefFKR { get; set; }
		public virtual D_EKR_FOYR RefEKRFOYR { get; set; }
		public virtual FX_Date_YearDayUNV RefYearDayUNV { get; set; }
		public virtual D_KVSR_FOYR RefKVSRYR { get; set; }
		public virtual D_R_FOYR RefR { get; set; }
	}
}
