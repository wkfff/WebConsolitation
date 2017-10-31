using System;

namespace Krista.FM.Domain
{
	public class D_FK_TOFK : ClassifierTable
	{
		public static readonly string Key = "dfbec171-8247-4602-b7a0-a4967f3dab49";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
