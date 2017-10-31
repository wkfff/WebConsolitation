using System;

namespace Krista.FM.Domain
{
	public class FX_FX_TypeTaxpayer : ClassifierTable
	{
		public static readonly string Key = "ce19b2d7-ed02-4c03-ace0-1bc21507c3b9";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
