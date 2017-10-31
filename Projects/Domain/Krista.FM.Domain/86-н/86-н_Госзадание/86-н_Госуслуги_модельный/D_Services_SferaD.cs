using System;

namespace Krista.FM.Domain
{
	public class D_Services_SferaD : ClassifierTable
	{
		public static readonly string Key = "f8e1b820-054b-47e4-9c7c-e044991ec9ad";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }

	    public const int FX_FX_UNDEFINED_ID = -1;
	}
}
