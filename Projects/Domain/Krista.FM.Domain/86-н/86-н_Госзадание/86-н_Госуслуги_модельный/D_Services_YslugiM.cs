using System;

namespace Krista.FM.Domain
{
	public class D_Services_YslugiM : ClassifierTable
	{
		public static readonly string Key = "a6888c2a-6d97-481a-a282-5e6d4f0889f3";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime? DataVkl { get; set; }
		public virtual DateTime? DataIskl { get; set; }
		public virtual string Status { get; set; }
		public virtual string Code { get; set; }
		public virtual string Note { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_Services_SferaD RefSfD { get; set; }
	}
}
