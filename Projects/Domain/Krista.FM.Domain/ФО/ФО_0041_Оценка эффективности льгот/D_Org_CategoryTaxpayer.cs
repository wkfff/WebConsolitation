using System;

namespace Krista.FM.Domain
{
	public class D_Org_CategoryTaxpayer : ClassifierTable
	{
		public static readonly string Key = "c4803331-f034-4d1e-8c98-1f368285a6c9";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual Decimal? CorrectIndex { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string OKVED { get; set; }
		public virtual FX_FX_TypeTaxpayer RefTypeTaxpayer { get; set; }
		public virtual D_OMSU_ResponsOIV RefOGV { get; set; }
		public virtual FX_FX_TypeTax RefTypeTax { get; set; }
	}
}
