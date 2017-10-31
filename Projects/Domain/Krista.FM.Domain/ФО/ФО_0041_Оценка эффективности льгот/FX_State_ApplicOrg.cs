using System;

namespace Krista.FM.Domain
{
	public class FX_State_ApplicOrg : ClassifierTable
	{
		public static readonly string Key = "1dc99340-44ab-4552-b9f2-b916cc38fee3";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
	}
}
