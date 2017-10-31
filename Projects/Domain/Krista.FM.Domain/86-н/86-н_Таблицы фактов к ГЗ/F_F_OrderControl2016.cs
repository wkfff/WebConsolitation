using System;

namespace Krista.FM.Domain
{
	public class F_F_OrderControl2016 : FactTable
	{
		public static readonly string Key = "d88903a4-01fe-4611-9cb3-15bb6466d4ba";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Form { get; set; }
		public virtual string Rate { get; set; }
		public virtual string Supervisor { get; set; }
		public virtual F_F_ParameterDoc RefFactGZ { get; set; }
	}
}
