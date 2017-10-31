using System;

namespace Krista.FM.Domain
{
	public class T_Doc_ApplicationOrg : DomainObject
	{
		public static readonly string Key = "5673083a-61e5-4f10-a6ff-8e508cfa8905";

		public virtual string Name { get; set; }
		public virtual byte[] Doc { get; set; }
		public virtual string Note { get; set; }
		public virtual DateTime DateDoc { get; set; }
		public virtual string Executor { get; set; }
		public virtual D_Application_Privilege RefApplicOrg { get; set; }
	}
}
