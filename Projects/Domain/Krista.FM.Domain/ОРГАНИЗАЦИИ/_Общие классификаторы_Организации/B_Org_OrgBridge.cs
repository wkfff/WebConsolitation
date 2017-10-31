using System;

namespace Krista.FM.Domain
{
	public class B_Org_OrgBridge : ClassifierTable
	{
		public static readonly string Key = "b7b50e8c-3fa1-4eeb-bd80-f47ec4848575";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual string Name { get; set; }
		public virtual int Code { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string Note { get; set; }
	}
}
