using System;

namespace Krista.FM.Domain
{
	public class D_CD_CollectTask : ClassifierTable
	{
		public static readonly string Key = "69f69748-7e7a-4174-bec0-eface1415a65";

		public virtual int RowType { get; set; }
		public virtual D_CD_Period RefPeriod { get; set; }
		public virtual D_CD_Subjects RefSubject { get; set; }
		public virtual DateTime EndPeriod { get; set; }
		public virtual DateTime ProvideDate { get; set; }
	}
}
