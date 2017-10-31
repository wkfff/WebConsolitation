using System;

namespace Krista.FM.Domain
{
	public class D_Marks_PassportMO : ClassifierTable
	{
		public static readonly string Key = "05e7ad82-8d16-4563-ab7e-5f5d00e81aab";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual bool? PlanPeriod { get; set; }
		public virtual bool? FactPeriod { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual B_Marks_PassportMO RefPasMOBridge { get; set; }
		public virtual D_Units_OKEI RefOKEI { get; set; }
		public virtual FX_FX_TypeMark RefTypeMark { get; set; }
		public virtual D_OMSU_ResponsOIV RefOGV { get; set; }
	}
}
