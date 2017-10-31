using System;

namespace Krista.FM.Domain
{
	public class B_D_Group : ClassifierTable
	{
		public static readonly string Key = "a1d76f2c-9b80-476c-a7c8-072d44824ea6";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
