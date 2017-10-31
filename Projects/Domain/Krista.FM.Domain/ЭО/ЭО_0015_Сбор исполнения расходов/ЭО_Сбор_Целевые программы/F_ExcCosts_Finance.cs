using System;

namespace Krista.FM.Domain
{
	public class F_ExcCosts_Finance : FactTable
	{
		public static readonly string Key = "2067127c-3683-478b-9edf-a93a4775b7e1";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? Target { get; set; }
		public virtual Decimal? Fact { get; set; }
		public virtual D_ExcCosts_Events RefEvent { get; set; }
		public virtual D_ExcCosts_Finances RefFin { get; set; }
		public virtual FX_Date_YearDayUNV RefUNV { get; set; }
	}
}
