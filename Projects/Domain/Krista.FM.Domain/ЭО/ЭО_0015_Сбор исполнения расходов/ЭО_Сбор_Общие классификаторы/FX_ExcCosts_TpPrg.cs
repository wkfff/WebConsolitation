using System;

namespace Krista.FM.Domain
{
	public class FX_ExcCosts_TpPrg : ClassifierTable
	{
		public static readonly string Key = "b20add91-6c56-426c-9c3b-fe8f2874e4a5";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
	}
}
