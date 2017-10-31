using System;

namespace Krista.FM.Domain
{
	public class F_F_ServiceOKPD : FactTable
	{
		public static readonly string Key = "77f04f9e-8757-4e33-a19d-72f8d9cef48e";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual D_Services_Service RefService { get; set; }
	}
}
