using System;

namespace Krista.FM.Domain
{
	public class FX_Marks_TrihedrAlagr : ClassifierTable
	{
		public static readonly string Key = "e01178f2-ab9a-439d-9755-3b9c33087c36";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
