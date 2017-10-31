using System;

namespace Krista.FM.Domain
{
	public class D_Org_VidOrg : ClassifierTable
	{
		public static readonly string Key = "270b0ce3-8dcc-46c9-a08e-28678ca9ac92";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Code { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual string BusinessStatus { get; set; }
	}
}
