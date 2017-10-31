using System;

namespace Krista.FM.Domain
{
	public class FX_Org_StatusD : ClassifierTable
	{
		public static readonly string Key = "f5e8a5af-23d0-4ad6-a70f-3ff61811fa97";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
