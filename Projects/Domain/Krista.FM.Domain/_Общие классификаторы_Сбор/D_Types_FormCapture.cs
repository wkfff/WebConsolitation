using System;

namespace Krista.FM.Domain
{
	public class D_Types_FormCapture : ClassifierTable
	{
		public static readonly string Key = "68781991-c18c-4d97-915d-7ab9edf26d9c";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string DateGrant { get; set; }
		public virtual string Commentary { get; set; }
		public virtual string Symbol { get; set; }
	}
}
