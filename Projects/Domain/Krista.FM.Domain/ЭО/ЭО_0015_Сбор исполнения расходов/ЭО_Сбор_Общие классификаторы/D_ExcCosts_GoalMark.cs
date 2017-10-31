using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_GoalMark : ClassifierTable
	{
		public static readonly string Key = "c9feb013-413e-4cc5-9dc5-2f9a79172843";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual string ReasonDec { get; set; }
		public virtual string Suggestion { get; set; }
		public virtual D_Units_OKEI RefOKEI { get; set; }
		public virtual FX_ExcCosts_TypeMark RefTypeMark { get; set; }
		public virtual D_ExcCosts_Tasks RefTask { get; set; }
	}
}
