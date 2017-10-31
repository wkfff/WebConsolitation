using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_Input : ClassifierTable
	{
		public static readonly string Key = "a97eb608-3cfa-430a-a4ad-dc309fa7ba46";

		public virtual int RowType { get; set; }
		public virtual string Value { get; set; }
		public virtual FX_Date_YearDayUNV RefYearDayUNV { get; set; }
		public virtual D_ExcCosts_CObject RefCObject { get; set; }
		public virtual D_ExcCosts_AIPMark RefAIPMark { get; set; }
		public virtual D_ExcCosts_StatusD RefStatusD { get; set; }
	}
}
