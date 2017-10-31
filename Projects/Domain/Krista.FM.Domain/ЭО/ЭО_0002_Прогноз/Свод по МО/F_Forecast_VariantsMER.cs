using System;

namespace Krista.FM.Domain
{
	public class F_Forecast_VariantsMER : FactTable
	{
		public static readonly string Key = "63649506-2722-462f-9259-d664f79cee9b";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Name { get; set; }
		public virtual F_Forecast_VariantsMO RefVarMO { get; set; }
	}
}
