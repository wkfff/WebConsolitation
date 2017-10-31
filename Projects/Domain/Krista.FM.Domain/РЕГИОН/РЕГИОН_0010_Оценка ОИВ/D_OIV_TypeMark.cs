using System;

namespace Krista.FM.Domain
{
	public class D_OIV_TypeMark : ClassifierTable
	{
		public static readonly string Key = "a74088a1-ad63-461e-bc7c-2229e416b93f";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
