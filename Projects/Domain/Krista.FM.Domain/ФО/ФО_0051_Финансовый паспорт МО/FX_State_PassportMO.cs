using System;

namespace Krista.FM.Domain
{
	public class FX_State_PassportMO : ClassifierTable
	{
		public static readonly string Key = "b13713b8-8a40-4f74-9772-405f7eb41e9b";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
