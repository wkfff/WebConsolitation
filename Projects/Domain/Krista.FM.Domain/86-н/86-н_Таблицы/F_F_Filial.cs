using System;

namespace Krista.FM.Domain
{
	public class F_F_Filial : FactTable
	{
		public static readonly string Key = "a4a33d4f-68e1-49c8-9ec3-7f2a334012fa";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual int? Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Nameshot { get; set; }
		public virtual string KPP { get; set; }
		public virtual string INN { get; set; }
		public virtual D_Org_TipFil RefTipFi { get; set; }
		public virtual F_Org_Passport RefPassport { get; set; }
	}
}
