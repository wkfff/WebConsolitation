using System;

namespace Krista.FM.Domain
{
	public class FX_FX_TypeOrganization : ClassifierTable
	{
		public static readonly string Key = "ebe56bf4-b51e-4988-8fcd-8e77496f560d";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
