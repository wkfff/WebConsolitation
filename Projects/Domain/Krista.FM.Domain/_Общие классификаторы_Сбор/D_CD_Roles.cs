using System;

namespace Krista.FM.Domain
{
	public class D_CD_Roles : ClassifierTable
	{
		public static readonly string Key = "eb8503df-bef8-46a3-80f5-d775afba2d8d";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual string ReportScope { get; set; }
	}
}
