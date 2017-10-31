using System;

namespace Krista.FM.Domain
{
	public class T_Forecast_ValuesOIV : DomainObject
	{
		public static readonly string Key = "07dfad01-05a2-4fd8-8eec-91b0d39bb520";

		public virtual D_Forecast_ParamsMO RefParams { get; set; }
		public virtual F_Forecast_VariantsOIV RefVars { get; set; }
		public virtual Decimal? R1 { get; set; }
		public virtual Decimal? R2 { get; set; }
		public virtual Decimal? Est { get; set; }
		public virtual Decimal? Y1v1 { get; set; }
		public virtual Decimal? Y1v2 { get; set; }
		public virtual Decimal? Y2v1 { get; set; }
		public virtual Decimal? cfe1254 { get; set; }
		public virtual Decimal? Y3v1 { get; set; }
		public virtual Decimal? Y3v2 { get; set; }
		public virtual string Note { get; set; }
	}
}
