using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_Creators : ClassifierTable
	{
		public static readonly string Key = "4b80bb78-1ce7-4352-9a79-2fc1c289994c";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string LegalAddress { get; set; }
		public virtual string FactAddress { get; set; }
		public virtual string Phone { get; set; }
		public virtual string Note { get; set; }
		public virtual string Login { get; set; }
	}
}
