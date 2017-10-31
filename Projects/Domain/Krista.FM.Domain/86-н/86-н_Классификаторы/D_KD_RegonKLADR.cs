using System;

namespace Krista.FM.Domain
{
	public class D_KD_RegonKLADR : ClassifierTable
	{
		public static readonly string Key = "b1cbe6e3-a9a4-4e9a-af1d-9a96f97e306d";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
