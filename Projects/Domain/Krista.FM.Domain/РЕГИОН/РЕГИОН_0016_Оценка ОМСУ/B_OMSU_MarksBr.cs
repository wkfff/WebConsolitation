using System;

namespace Krista.FM.Domain
{
	public class B_OMSU_MarksBr : ClassifierTable
	{
		public static readonly string Key = "c168d26c-476c-43a1-a2b5-fed20759052d";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Symbol { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_Units_OKEI RefUnits { get; set; }
	}
}
