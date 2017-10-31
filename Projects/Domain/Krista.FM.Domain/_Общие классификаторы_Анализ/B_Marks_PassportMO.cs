using System;

namespace Krista.FM.Domain
{
	public class B_Marks_PassportMO : ClassifierTable
	{
		public static readonly string Key = "3a0255cd-71b3-4da5-8726-0bc1ebf818df";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual int? CodeLine { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
