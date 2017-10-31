using System;

namespace Krista.FM.Domain
{
	public class FX_OIV_StatusData : ClassifierTable
	{
		public static readonly string Key = "e196b9e3-f4cf-4db9-a04e-1b480d152885";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
