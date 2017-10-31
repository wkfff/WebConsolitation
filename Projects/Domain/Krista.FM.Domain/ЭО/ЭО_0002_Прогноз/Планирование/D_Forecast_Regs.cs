using System;

namespace Krista.FM.Domain
{
	public class D_Forecast_Regs : ClassifierTable
	{
		public static readonly string Key = "e6a8aaba-89e4-4f68-b555-3412022b4354";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Descr { get; set; }
		public virtual D_Units_OKEI RefUnits { get; set; }
        public virtual string Signat { get; set; }
	}
}
