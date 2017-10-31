using System;

namespace Krista.FM.Domain
{
	public class FX_FX_Status : ClassifierTable
	{
		public static readonly string Key = "704db7bb-3e2f-4fa2-9103-2e0ad2db71f6";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
	}
}
