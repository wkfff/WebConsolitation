using System;

namespace Krista.FM.Domain
{
	public class D_MassMedia_Kind : ClassifierTable
	{
		public static readonly string Key = "9380eac8-a944-44ec-b07b-ac637a9ade86";

		public virtual int RowType { get; set; }
		public virtual string KindName { get; set; }
		public virtual string OtherName { get; set; }
		public virtual int Code { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
