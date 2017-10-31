using System;

namespace Krista.FM.Domain
{
	public class F_F_ServiceProvider : FactTable
	{
		public static readonly string Key = "4cf0f93b-9319-4283-a375-018e42f9f7c4";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual D_Org_Structure RefProvider { get; set; }
		public virtual D_Services_ServiceRegister RefService { get; set; }
	}
}
