using System;

namespace Krista.FM.Domain
{
	public class D_Arrears_Debt : ClassifierTable
	{
		public static readonly string Key = "3f7931ec-19e8-4b1a-9eca-19d825235f31";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual string Code { get; set; }
		public virtual int CodeNumber { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_FKR_Bridge RefnFKRBridge { get; set; }
		public virtual B_EKR_Bridge RefnEKRBridge { get; set; }
	}
}
