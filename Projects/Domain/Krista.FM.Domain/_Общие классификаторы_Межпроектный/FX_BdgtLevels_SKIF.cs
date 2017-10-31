using System;

namespace Krista.FM.Domain
{
	public class FX_BdgtLevels_SKIF : ClassifierTable
	{
		public static readonly string Key = "15f3c149-0658-4e29-9ce8-37b7f9b9349a";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string FullName { get; set; }
	}
}
