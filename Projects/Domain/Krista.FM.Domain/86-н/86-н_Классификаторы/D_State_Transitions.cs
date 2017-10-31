using System;

namespace Krista.FM.Domain
{
	public class D_State_Transitions : ClassifierTable
	{
		public static readonly string Key = "5fa5c6df-001a-4c04-8e3a-0955cdd45cfd";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Ico { get; set; }
		public virtual string Note { get; set; }
		public virtual string Action { get; set; }
		public virtual string TransitionClass { get; set; }
		public virtual D_State_SchemTransitions RefSchemStateTransitions { get; set; }
		public virtual D_State_SchemStates RefSchemStates { get; set; }
	}
}
