using System;

namespace Krista.FM.Domain
{
	public class D_OK_OKVED : ClassifierTable
	{
		public static readonly string Key = "f12036fc-e62c-4176-845e-9b7979b03c64";

		public virtual int RowType { get; set; }
		public virtual string Part { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual int? Code4 { get; set; }
		public virtual int? Code5 { get; set; }
		public virtual int? Code6 { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_OKVED_Bridge RefOKVED { get; set; }
	}
}
