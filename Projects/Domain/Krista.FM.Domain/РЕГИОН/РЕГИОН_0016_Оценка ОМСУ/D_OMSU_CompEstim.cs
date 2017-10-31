using System;

namespace Krista.FM.Domain
{
	public class D_OMSU_CompEstim : ClassifierTable
	{
		public static readonly string Key = "4d6f3175-708b-4d97-854b-f0ef9f179ca2";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual Decimal GravCoef { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
