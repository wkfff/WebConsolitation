using System;

namespace Krista.FM.Domain
{
	public class B_KVSR_Bridge : ClassifierTable
	{
		public static readonly string Key = "eb64ed07-4635-4b25-8452-0b0d119458e3";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string FIO { get; set; }
		public virtual string Post { get; set; }
		public virtual string WebSite { get; set; }
		public virtual string Email { get; set; }
		public virtual string Telephone { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string AddressSkype { get; set; }
		public virtual string AddressFaceTime { get; set; }
		public virtual int? CodeLine { get; set; }
		public virtual B_KVSR_Bridge RefKVSRBridge { get; set; }
	}
}
