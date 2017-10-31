using System;

namespace Krista.FM.Domain
{
	public class B_D_GroupKD : ClassifierTable
	{
		public static readonly string Key = "1621d4a3-9a5d-4f36-81e7-17c8a43a3c34";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string CodeStr { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
