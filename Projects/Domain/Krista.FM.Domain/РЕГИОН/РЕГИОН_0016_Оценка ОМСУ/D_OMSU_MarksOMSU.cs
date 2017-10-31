using System;

namespace Krista.FM.Domain
{
	public class D_OMSU_MarksOMSU : ClassifierTable
	{
		public static readonly string Key = "ee3b7206-305a-4aca-af5e-d6e7bb9b3a89";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual string Symbol { get; set; }
		public virtual string InfoSource { get; set; }
		public virtual string CalcMark { get; set; }
		public virtual bool FlagRev { get; set; }
		public virtual bool IncRep { get; set; }
		public virtual bool MO { get; set; }
		public virtual int? CodeRep { get; set; }
		public virtual string CodeEstim { get; set; }
		public virtual bool IncEstim { get; set; }
		public virtual string Formula { get; set; }
		public virtual bool Grouping { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string CodeStructure { get; set; }
		public virtual int? Capacity { get; set; }
		public virtual string WayIneffExp { get; set; }
		public virtual string CodeRepDouble { get; set; }
		public virtual bool PrognMO { get; set; }
		public virtual int? CubeParentID { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_Units_OKEI RefOKEI { get; set; }
		public virtual D_OMSU_CompRep RefCompRep { get; set; }
		public virtual D_OMSU_ResponsOIV RefResponsOIV { get; set; }
		public virtual D_OMSU_CompEstim RefCopmEstim { get; set; }
		public virtual D_OMSU_TypeMark RefTypeMark { get; set; }
		public virtual D_OMSU_ReachIneff RefReach { get; set; }
		public virtual B_OMSU_MarksBr RefMarksB { get; set; }
	}
}
