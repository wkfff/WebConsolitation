using System;

namespace Krista.FM.Domain
{
	public class D_Report_TrihedrAgr : ClassifierTable
	{
		public static readonly string Key = "c4bfced8-47a8-4f21-9a09-03c55b71bd35";

		public virtual int RowType { get; set; }
		public virtual D_CD_Task RefTask { get; set; }
	}
}
