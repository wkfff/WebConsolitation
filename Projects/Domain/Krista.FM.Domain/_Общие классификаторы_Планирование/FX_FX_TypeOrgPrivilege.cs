using System;

namespace Krista.FM.Domain
{
	public class FX_FX_TypeOrgPrivilege : ClassifierTable
	{
		public static readonly string Key = "398172ca-21e1-472b-9723-6eff3c892272";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
