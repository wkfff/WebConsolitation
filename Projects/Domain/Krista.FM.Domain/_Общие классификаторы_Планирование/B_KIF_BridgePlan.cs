using System;

namespace Krista.FM.Domain
{
	public class B_KIF_BridgePlan : ClassifierTable
	{
		public static readonly string Key = "6e166f3c-cfee-47fa-8c26-4b9673902f87";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual string CodeStr { get; set; }
		public virtual int? Code1 { get; set; }
		public virtual int? Code2 { get; set; }
		public virtual int? Code3 { get; set; }
		public virtual int? Code4 { get; set; }
		public virtual int? Code5 { get; set; }
		public virtual int? Code6 { get; set; }
		public virtual int? Code7 { get; set; }
		public virtual int? Code8 { get; set; }
		public virtual int? Code9 { get; set; }
		public virtual int? Code10 { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual FX_KIF_Direction RefKIF { get; set; }
		public virtual FX_KIF_ClsAspect RefKIFClsAspect { get; set; }
	}
}
