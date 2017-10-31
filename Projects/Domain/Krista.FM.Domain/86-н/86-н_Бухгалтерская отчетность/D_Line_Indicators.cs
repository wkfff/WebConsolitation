using System;

namespace Krista.FM.Domain
{
	public class D_Line_Indicators : ClassifierTable
	{
		public static readonly string Key = "3f87d0ed-5e14-44bd-b8b9-bb657ba475d3";

		public virtual int RowType { get; set; }
		public virtual string LineCode { get; set; }
		public virtual string Name { get; set; }
		public virtual int Code { get; set; }
	}
}
