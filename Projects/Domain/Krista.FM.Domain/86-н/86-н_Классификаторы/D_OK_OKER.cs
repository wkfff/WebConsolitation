using System;

namespace Krista.FM.Domain
{
	public class D_OK_OKER : ClassifierTable
	{
		public static readonly string Key = "96ee3be0-54ee-4362-aaf5-ca23b98be415";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime DataVk { get; set; }
		public virtual DateTime DataIsk { get; set; }
		public virtual string Status { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_OKATO_OKATO RefOKATO { get; set; }
	}
}
