using System;

namespace Krista.FM.Domain
{
	public class D_OKATO_OKATO : ClassifierTable
	{
		public static readonly string Key = "77373201-9a3a-471b-b0f1-2570a4bb1050";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual int? Code4 { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
