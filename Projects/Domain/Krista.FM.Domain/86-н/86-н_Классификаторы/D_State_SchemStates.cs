using System;

namespace Krista.FM.Domain
{
	public class D_State_SchemStates : ClassifierTable
	{
		public static readonly string Key = "65d63ddf-cf61-4d8e-b627-5131f9bd77d4";

		public virtual int RowType { get; set; }
		public virtual bool? IsStart { get; set; }
		public virtual D_State_SchemTransitions RefSchemStateTransitions { get; set; }
		public virtual FX_Org_SostD RefStates { get; set; }
	}
}
