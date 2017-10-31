using System;

namespace Krista.FM.Domain
{
	public class FX_FX_TypeMark : ClassifierTable
	{
		public static readonly string Key = "0effd2c8-891b-4205-a075-c4cb05e4cd1f";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
