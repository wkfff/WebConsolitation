using System;

namespace Krista.FM.Domain
{
	public class D_OMSU_ResponsOIVUser : ClassifierTable
	{
		public static readonly string Key = "21342f8a-666a-4222-b358-53c16d2e5744";

		public virtual int RowType { get; set; }
		public virtual int RefUser { get; set; }
		public virtual D_OMSU_ResponsOIV RefResponsOIV { get; set; }
	}
}
