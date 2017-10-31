using System;

namespace Krista.FM.Domain
{
	public class F_ResultWork_CashReceipts : FactTable
	{
		public static readonly string Key = "305f70e8-0dd8-427a-aa56-46fe27ddf5ee";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal TaskGrant { get; set; }
		public virtual Decimal ActionGrant { get; set; }
		public virtual Decimal BudgetFunds { get; set; }
		public virtual Decimal PaidServices { get; set; }
		public virtual Decimal Total { get; set; }
		public virtual F_F_ParameterDoc RefParametr { get; set; }
	}
}
