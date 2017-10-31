using System;

namespace Krista.FM.Domain
{
	public class F_Fin_othGrantFunds : FactTable
	{
		public static readonly string Key = "41740bd4-6520-45dd-b9d2-f0d5441d6724";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? funds { get; set; }
		public virtual D_Fin_OtherGant RefOtherGrant { get; set; }
		public virtual string KOSGY { get; set; }
		public virtual F_F_ParameterDoc RefParametr { get; set; }
	}
}
