using System;

namespace Krista.FM.Domain
{
	public class T_Note_ApplicationOGV : DomainObject
	{
		public static readonly string Key = "38075372-a650-460e-b09d-57e50b64157f";

		public virtual string Executor { get; set; }
		public virtual DateTime NoteDate { get; set; }
		public virtual string Text { get; set; }
		public virtual D_Application_FromOGV RefApplicOGV { get; set; }
		public virtual FX_State_ApplicOGV RefStateOGV { get; set; }
	}
}
