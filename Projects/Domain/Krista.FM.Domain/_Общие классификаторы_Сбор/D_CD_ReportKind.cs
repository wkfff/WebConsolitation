using System;

namespace Krista.FM.Domain
{
	public class D_CD_ReportKind : ClassifierTable
	{
		public static readonly string Key = "ba543b8d-51de-437d-bce8-d59cb40e581b";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual int PeriodType { get; set; }
	}
}
