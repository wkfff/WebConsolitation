using System;

namespace Krista.FM.Domain
{
	public class D_Doc_TypeDoc : ClassifierTable
	{
		public static readonly string Key = "f1066db2-a354-48ae-b11d-b6e0fe2ef3d6";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
	}
}
