using System;

namespace Krista.FM.Domain
{
	public class D_Org_Report : ClassifierTable
	{
		public static readonly string Key = "84e56672-8c53-4f41-bb36-873a6d3bd275";

		public virtual int RowType { get; set; }
		public virtual D_CD_Task RefTask { get; set; }
	}
}
