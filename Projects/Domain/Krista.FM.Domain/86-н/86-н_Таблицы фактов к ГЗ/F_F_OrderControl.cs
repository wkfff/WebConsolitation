using System;

namespace Krista.FM.Domain
{
	public class F_F_OrderControl : FactTable
	{
		public static readonly string Key = "4332901c-584a-4bc2-ae4a-d44595f6ee1c";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Form { get; set; }
		public virtual string Rate { get; set; }
		public virtual string Supervisor { get; set; }
		public virtual F_F_ParameterDoc RefFactGZ { get; set; }
	}
}
