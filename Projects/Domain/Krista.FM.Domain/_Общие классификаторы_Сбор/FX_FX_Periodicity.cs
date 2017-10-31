using System;

namespace Krista.FM.Domain
{
	public class FX_FX_Periodicity : ClassifierTable
	{
		public static readonly string Key = "cf5a3e87-8313-4a33-bb49-25f929ebecf7";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
	}
}
