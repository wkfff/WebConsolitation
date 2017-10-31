using System;

namespace Krista.FM.Domain
{
	public class F_Forecast_VariantsOIV : FactTable
	{
		public static readonly string Key = "3f9ecb33-5405-49a7-815f-3e0325ce5ff2";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Name { get; set; }
		public virtual F_Forecast_VariantsMO RefVarMO { get; set; }
	}
}
