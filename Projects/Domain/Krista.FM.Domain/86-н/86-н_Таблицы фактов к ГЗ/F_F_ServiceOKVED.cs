using System;

namespace Krista.FM.Domain
{
	public class F_F_ServiceOKVED : FactTable
	{
		public static readonly string Key = "5079121c-caa0-4415-bb25-d997ca02f5ec";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual D_Services_Service RefService { get; set; }
	}
}
