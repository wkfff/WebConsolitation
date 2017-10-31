using System;

namespace Krista.FM.Domain
{
	public class F_Org_Price : FactTable
	{
		public static readonly string Key = "62b88e8a-5b2c-4e7c-b3b5-eaa71ac31b31";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? Price { get; set; }
		public virtual Decimal? MedPrice { get; set; }
		public virtual Decimal? ConsumRate { get; set; }
		public virtual Decimal? ExpComp { get; set; }
		public virtual Decimal? MinPrice { get; set; }
		public virtual Decimal? MaxPrice { get; set; }
		public virtual Decimal? Value { get; set; }
		public virtual D_Org_TypePrice RefOrg { get; set; }
		public virtual D_Org_RegistrOrg RefOrgRegistrOrg { get; set; }
		public virtual D_Territory_RF RefTerritory { get; set; }
		public virtual D_Org_Good RefGoodOrg { get; set; }
		public virtual FX_Date_YearDayUNV RefDay { get; set; }
		public virtual FX_Org_State RefState { get; set; }
	}
}
