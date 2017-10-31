using System;

namespace Krista.FM.Domain
{
	public class F_Fin_Smeta : FactTable
	{
		public static readonly string Key = "34afe5ce-8e8c-4fea-8fe5-7bc9cee6a4b1";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? Funds { get; set; }
		public virtual Decimal FundsOneYear { get; set; }
		public virtual Decimal? FundsTwoYear { get; set; }
		public virtual string CelStatya { get; set; }
		public virtual D_Fin_nsiBudget RefBudget { get; set; }
		public virtual F_F_ParameterDoc RefParametr { get; set; }
		public virtual D_Fin_KbkBudget RefKbkBudget { get; set; }
		public virtual D_Fin_RazdPodr RefRazdPodr { get; set; }
		public virtual D_Fin_VidRash RefVidRash { get; set; }
		public virtual D_KOSGY_KOSGY RefKosgy { get; set; }
		public virtual string Event { get; set; }
	}
}
