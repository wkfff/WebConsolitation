using System;

namespace Krista.FM.Domain
{
	public class FX_FX_KindOfForecasts : ClassifierTable
	{
		public static readonly string Key = "0fba51b7-6f02-41c9-8c14-c874044939b9";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
