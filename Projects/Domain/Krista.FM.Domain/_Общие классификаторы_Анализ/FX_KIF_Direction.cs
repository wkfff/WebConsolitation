using System;

namespace Krista.FM.Domain
{
	public class FX_KIF_Direction : ClassifierTable
	{
		public static readonly string Key = "bf627e8d-d6a9-49a1-9a11-56d48de63d7b";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
	}
}
