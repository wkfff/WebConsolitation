using System;

namespace Krista.FM.Domain
{
	public class F_F_BaseTermination2016 : FactTable
	{
		public static readonly string Key = "d8f555dd-c927-4412-9f3c-caed6303debe";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string EarlyTerminat { get; set; }
		public virtual F_F_ParameterDoc RefFactGZ { get; set; }
	}
}
