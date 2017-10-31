using System;

namespace Krista.FM.Domain
{
	public class F_Marks_DataPrivilege : FactTable
	{
		public static readonly string Key = "0e922623-e00a-4aaa-95c4-245db3819aad";

		public virtual int SourceID { get; set; }
		public virtual int PumpID { get; set; }
		public virtual int? SourceKey { get; set; }
		public virtual int TaskID { get; set; }
		public virtual Decimal? Fact { get; set; }
		public virtual Decimal? Estimate { get; set; }
		public virtual Decimal? Forecast { get; set; }
		public virtual Decimal? PreviousFact { get; set; }
		public virtual D_Marks_Privilege RefMarks { get; set; }
		public virtual D_Application_Privilege RefApplication { get; set; }
	}
}
