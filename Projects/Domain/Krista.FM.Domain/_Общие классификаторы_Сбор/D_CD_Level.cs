using System;

namespace Krista.FM.Domain
{
	public class D_CD_Level : ClassifierTable
	{
		public static readonly string Key = "6c10913e-024b-442c-9fe5-4f707e8df73d";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual string ReportScope { get; set; }
	}
}
