using System;

namespace Krista.FM.Domain
{
	public class D_MissivePRF_Execut : ClassifierTable
	{
		public static readonly string Key = "6fb2b646-4944-4972-805f-e641baa75c40";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual string Note { get; set; }
		public virtual string Curator { get; set; }
		public virtual string Details { get; set; }
		public virtual int? UserID { get; set; }
	}
}
