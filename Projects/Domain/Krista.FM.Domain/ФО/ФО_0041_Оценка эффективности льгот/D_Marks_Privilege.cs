using System;

namespace Krista.FM.Domain
{
	public class D_Marks_Privilege : ClassifierTable
	{
		public static readonly string Key = "8892dd12-f6e7-4012-bd62-d421cb00ae2e";

		public virtual int RowType { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Symbol { get; set; }
		public virtual string Formula { get; set; }
		public virtual string NumberString { get; set; }
		public virtual D_Units_OKEI RefOKEI { get; set; }
		public virtual FX_FX_TypeMark RefTypeMark { get; set; }
		public virtual FX_FX_TypeTax RefTypeTax { get; set; }
	}
}
