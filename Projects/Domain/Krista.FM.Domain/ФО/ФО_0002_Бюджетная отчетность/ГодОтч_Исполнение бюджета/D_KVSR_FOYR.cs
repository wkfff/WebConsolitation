using System;

namespace Krista.FM.Domain
{
	public class D_KVSR_FOYR : ClassifierTable
	{
		public static readonly string Key = "b28be109-b805-45bf-9a9c-e4346c02b22e";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int? Code { get; set; }
		public virtual string Name { get; set; }
		public virtual B_KVSR_Bridge RefKVSRYR { get; set; }
	}
}
