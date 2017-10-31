using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_Crit : ClassifierTable
	{
		public static readonly string Key = "1a5215d9-f12a-4ffb-8f52-2e10f6456967";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual Decimal Weight { get; set; }
		public virtual Decimal Point { get; set; }
		public virtual string Comments { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
