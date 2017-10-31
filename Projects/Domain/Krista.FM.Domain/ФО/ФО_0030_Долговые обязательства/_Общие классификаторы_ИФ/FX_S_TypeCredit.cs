using System;

namespace Krista.FM.Domain
{
	public class FX_S_TypeCredit : ClassifierTable
	{
		public static readonly string Key = "77cefe87-ab26-4fde-bac0-a694dfc38464";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
