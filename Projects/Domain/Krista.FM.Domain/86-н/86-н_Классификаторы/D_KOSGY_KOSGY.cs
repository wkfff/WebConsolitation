using System;

namespace Krista.FM.Domain
{
	public class D_KOSGY_KOSGY : ClassifierTable
	{
		public static readonly string Key = "55bfa17d-ea3e-450b-838b-8e1e748f5a1b";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
