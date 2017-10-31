using System;

namespace Krista.FM.Domain
{
	public class F_F_ServiceInstitutionType : FactTable
	{
		public static readonly string Key = "10efac09-9378-4052-92c1-aff482ce6a66";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string GUID { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual D_Services_Service RefService { get; set; }
	}
}
