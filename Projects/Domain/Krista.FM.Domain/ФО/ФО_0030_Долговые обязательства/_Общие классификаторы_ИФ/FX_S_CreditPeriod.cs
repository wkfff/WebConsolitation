using System;

namespace Krista.FM.Domain
{
	public class FX_S_CreditPeriod : ClassifierTable
	{
		public static readonly string Key = "208702db-2a42-4db8-a757-f0f3238fd67b";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
