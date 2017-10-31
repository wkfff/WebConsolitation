using System;

namespace Krista.FM.Domain
{
	public class B_Org_Privilege : ClassifierTable
	{
		public static readonly string Key = "a73a2317-d968-4988-a23b-8b48ce7fa0a8";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual Decimal? CorrectIndex { get; set; }
		public virtual FX_FX_TypeOrgPrivilege RefPrivilege { get; set; }
	}
}
