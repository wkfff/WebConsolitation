using System;

namespace Krista.FM.Domain
{
	public class D_KVR_FOYR : ClassifierTable
	{
		public static readonly string Key = "4b4df36e-1182-4222-965e-e12485993e60";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual B_KVR_Bridge RefBridge { get; set; }
	}
}
