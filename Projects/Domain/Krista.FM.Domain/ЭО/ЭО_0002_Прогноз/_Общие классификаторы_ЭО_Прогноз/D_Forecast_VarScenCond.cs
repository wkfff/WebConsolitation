using System;

namespace Krista.FM.Domain
{
	public class D_Forecast_VarScenCond : ClassifierTable
	{
		public static readonly string Key = "c18e994f-a1ab-4491-91dd-5f31dc8b1288";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Symbol { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
	}
}
