using System;

namespace Krista.FM.Domain
{
	public class D_Services_ConsumerType : ClassifierTable
	{
		public static readonly string Key = "e91ed003-d49f-4a62-837c-bbbc0b3f0831";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
