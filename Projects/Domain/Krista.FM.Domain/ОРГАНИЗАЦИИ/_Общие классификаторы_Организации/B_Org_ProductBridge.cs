using System;

namespace Krista.FM.Domain
{
	public class B_Org_ProductBridge : ClassifierTable
	{
		public static readonly string Key = "87b164ac-296c-4264-9333-6e8aaf5cacfb";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Characteristic { get; set; }
		public virtual D_OK_OKDP RefOK { get; set; }
		public virtual D_Units_OKEI RefUnits { get; set; }
		public virtual D_OK_OKP RefOKOKP { get; set; }
		public virtual string ShortName { get; set; }
	}
}
