using System;

namespace Krista.FM.Domain
{
	public class D_CD_Task : ClassifierTable
	{
		public static readonly string Key = "e0375948-f46d-46ed-b65f-4d85e1b75340";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime BeginDate { get; set; }
		public virtual DateTime EndDate { get; set; }
		public virtual int OwnerUserId { get; set; }
		public virtual DateTime Deadline { get; set; }
		public virtual D_CD_Subjects RefSubject { get; set; }
		public virtual D_CD_Templates RefTemplate { get; set; }
		public virtual FX_Date_Year RefYear { get; set; }
		public virtual FX_FX_FormStatus RefStatus { get; set; }
		public virtual DateTime? LastChangeDate { get; set; }
		public virtual string LastChangeUser { get; set; }
        public virtual D_CD_CollectTask RefCollectTask { get; set; }
    }
}
