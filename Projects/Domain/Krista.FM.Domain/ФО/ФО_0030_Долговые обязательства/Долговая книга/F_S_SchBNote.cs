using System;

namespace Krista.FM.Domain
{
	public class F_S_SchBNote : FactTable
	{
		public static readonly string Key = "f6b4929b-8c0e-4bae-8d8a-b94e56c69aa9";

		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int TaskID { get; set; }
		public virtual byte[] Note { get; set; }
		public virtual D_Variant_Schuldbuch RefVariant { get; set; }
		public virtual D_Regions_Analysis RefRegion { get; set; }
	}
}
