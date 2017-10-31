using System;

namespace Krista.FM.Domain
{
	public class T_Forecast_ValuesMO : DomainObject
	{
		public static readonly string Key = "8d638452-ba42-4ddf-94b4-831a481c2c75";

		public virtual Decimal? R1 { get; set; }
		public virtual Decimal? R2 { get; set; }
		public virtual Decimal? Est { get; set; }
		public virtual Decimal? Y1v1 { get; set; }
		public virtual Decimal? Y1v2 { get; set; }
		public virtual Decimal? Y2v1 { get; set; }
		public virtual Decimal? Y2v2 { get; set; }
		public virtual Decimal? Y3v1 { get; set; }
		public virtual Decimal? Y3v2 { get; set; }
		public virtual D_Forecast_ParamsMO RefParams { get; set; }
		public virtual F_Forecast_VariantsMO RefVars { get; set; }
	}
}
