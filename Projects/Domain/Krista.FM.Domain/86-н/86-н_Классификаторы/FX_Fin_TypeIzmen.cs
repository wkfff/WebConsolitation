using System;

namespace Krista.FM.Domain
{
	public class FX_Fin_TypeIzmen : ClassifierTable
	{
		public static readonly string Key = "248409d3-3c19-4979-8799-8e52eb7a0ec5";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
	}
}
