using System;

namespace Krista.FM.Domain
{
	public class D_MissivePRF_GoalArrang : ClassifierTable
	{
		public static readonly string Key = "4fbd9e15-38ff-45f9-9428-8cf1e8c1b6dd";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string SpaceDisch { get; set; }
		public virtual string Note { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
