using System;

namespace Krista.FM.Domain
{
	public class B_KVSR_Department : ClassifierTable
	{
		public static readonly string Key = "d3cf4ac1-090f-43a1-be98-0361e7b38b1a";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
