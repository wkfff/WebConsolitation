using System;

namespace Krista.FM.Domain
{
    public class F_S_SchBLimit : DebtorBookFactBase
	{
		public const string Key = "8ae51f5d-32a8-402e-a0c2-9d292dee76d6";

		public virtual int PumpID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal ConsMunDebt1 { get; set; }
		public virtual Decimal MunDebt1 { get; set; }
		public virtual Decimal PosDebt1 { get; set; }
		public virtual Decimal ConsMunGrnt1 { get; set; }
		public virtual Decimal MunGrnt1 { get; set; }
		public virtual Decimal PosGrnt1 { get; set; }
		public virtual Decimal ConsMunService1 { get; set; }
		public virtual Decimal MunService1 { get; set; }
		public virtual Decimal PosService1 { get; set; }
		public virtual Decimal MunIssue1 { get; set; }
		public virtual Decimal ConsMunDebt2 { get; set; }
		public virtual Decimal MunDebt2 { get; set; }
		public virtual Decimal PosDebt2 { get; set; }
		public virtual Decimal ConsMunGrnt2 { get; set; }
		public virtual Decimal MunGrnt2 { get; set; }
		public virtual Decimal PosGrnt2 { get; set; }
		public virtual Decimal ConsMunService2 { get; set; }
		public virtual Decimal MunService2 { get; set; }
		public virtual Decimal PosService2 { get; set; }
		public virtual Decimal MunIssue2 { get; set; }
		public virtual Decimal ConsMunDebt3 { get; set; }
		public virtual Decimal MunDebt3 { get; set; }
		public virtual Decimal PosDebt3 { get; set; }
		public virtual Decimal ConsMunGrnt3 { get; set; }
		public virtual Decimal MunGrnt3 { get; set; }
		public virtual Decimal PosGrnt3 { get; set; }
		public virtual Decimal ConsMunService3 { get; set; }
		public virtual Decimal MunService3 { get; set; }
		public virtual Decimal PosService3 { get; set; }
		public virtual Decimal MunIssue3 { get; set; }
		public virtual Decimal? OwnBudgRevns { get; set; }
		public virtual string NormAct { get; set; }
	}
}
