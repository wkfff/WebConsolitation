using System;

namespace Krista.FM.Domain
{
	public class F_F_ServiceLegalAct : FactTable
	{
		public static readonly string Key = "0562d814-4e3c-456f-8f5c-d49b4c81dc5a";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Kind { get; set; }
		public virtual string LANumber { get; set; }
		public virtual string Name { get; set; }
		public virtual string ApprovedBy { get; set; }
		public virtual DateTime? EffectiveFrom { get; set; }
		public virtual DateTime? ApprvdAt { get; set; }
		public virtual DateTime? DatetEnd { get; set; }
		public virtual string MJnumber { get; set; }
		public virtual DateTime? MJregdate { get; set; }
		public virtual D_Services_Service RefService { get; set; }
	}
}
