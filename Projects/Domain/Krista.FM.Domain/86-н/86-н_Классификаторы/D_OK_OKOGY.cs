using System;

namespace Krista.FM.Domain
{
	public class D_OK_OKOGY : ClassifierTable
	{
		public static readonly string Key = "3b49e7da-8729-4d72-9a3c-f7b53c963bb5";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime DataVk { get; set; }
		public virtual DateTime DataIsk { get; set; }
		public virtual string Status { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
