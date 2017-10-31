using System;

namespace Krista.FM.Domain
{
	public class F_F_GZYslPotr2016 : FactTable
	{
		public static readonly string Key = "08f3c17b-3efd-4b09-97f0-40a38ba26d83";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual F_F_GosZadanie2016 RefFactGZ { get; set; }
		public virtual F_F_ServiceConsumersCategory RefConsumersCategory { get; set; }
    }
}
