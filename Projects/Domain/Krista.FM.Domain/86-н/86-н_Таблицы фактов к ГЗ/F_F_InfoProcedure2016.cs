using System;

namespace Krista.FM.Domain
{
	public class F_F_InfoProcedure2016 : FactTable
	{
		public static readonly string Key = "4abb9ce3-cc93-4814-ab1f-acbf6ea9c433";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Method { get; set; }
		public virtual string Content { get; set; }
		public virtual string Rate { get; set; }
		public virtual F_F_GosZadanie2016 RefFactGZ { get; set; }
	}
}
