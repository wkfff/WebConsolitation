using System;

namespace Krista.FM.Domain
{
	public class D_Forecast_PMethods : ClassifierTable
	{
		public static readonly string Key = "2be87bb3-2c5f-45b5-a3d1-6034346dda46";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual string TextName { get; set; }
		public virtual string TextCode { get; set; }
		public virtual string Descr { get; set; }
		public virtual int? PCount { get; set; }
		public virtual string Coeffs { get; set; }
		public virtual byte[] ImageFile { get; set; }
		public virtual string ImageFileName { get; set; }
		public virtual int? ImageFileType { get; set; }
		public virtual string ImageFileFileName { get; set; }
		public virtual string XMLString { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
