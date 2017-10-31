using System;

namespace Krista.FM.Domain
{
	public class F_F_ServiceConsumer : FactTable
	{
		public static readonly string Key = "c4dab4bb-0783-4e84-b51f-7922c91b1dca";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual D_Services_ServiceRegister RefService { get; set; }
		public virtual D_Services_ConsumerType RefConsumer { get; set; }
	}
}
