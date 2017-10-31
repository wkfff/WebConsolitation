using System;

namespace Krista.FM.Domain
{
	public class D_Application_Privilege : ClassifierTable
	{
		public static readonly string Key = "68cd3b28-0196-4f3b-bb34-e44c37f1f265";

		public virtual int RowType { get; set; }
		public virtual string Executor { get; set; }
		public virtual DateTime RequestDate { get; set; }
		public virtual D_Org_Privilege RefOrgPrivilege { get; set; }
		public virtual D_Org_CategoryTaxpayer RefOrgCategory { get; set; }
		public virtual FX_Date_YearDayUNV RefYearDayUNV { get; set; }
		public virtual FX_State_ApplicOrg RefStateOrg { get; set; }
		public virtual D_Application_FromOGV RefApplicOGV { get; set; }
		public virtual FX_FX_TypeTax RefTypeTax { get; set; }
	}
}
