using System;

namespace Krista.FM.Domain
{
	public class D_OK_OKFS : ClassifierTable
	{
		public static readonly string Key = "9fe23ac5-4de3-4e33-a430-595b750c194d";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
