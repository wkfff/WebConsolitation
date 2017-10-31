using System;

namespace Krista.FM.Domain
{
	public class D_KD_Street : ClassifierTable
	{
		public static readonly string Key = "e8f8c26c-64a8-45ee-89fb-30a75d863dda";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
