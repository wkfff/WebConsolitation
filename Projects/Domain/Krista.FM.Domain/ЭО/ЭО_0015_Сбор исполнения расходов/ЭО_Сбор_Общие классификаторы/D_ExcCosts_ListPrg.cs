using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_ListPrg : ClassifierTable
	{
		public static readonly string Key = "be610c07-bf1d-449e-9d75-5c794921bd18";

		public virtual int RowType { get; set; }
		public virtual int? Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string Note { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_ExcCosts_Creators RefCreators { get; set; }
		public virtual FX_ExcCosts_TpPrg RefTypeProg { get; set; }
		public virtual FX_Date_YearDayUNV RefApYear { get; set; }
		public virtual FX_Date_YearDayUNV RefBegDate { get; set; }
		public virtual FX_Date_YearDayUNV RefEndDate { get; set; }
		public virtual D_Territory_RF RefTerritory { get; set; }
	}
}
