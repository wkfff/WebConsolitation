using System;

namespace Krista.FM.Domain
{
	public class T_Doc_ApplicationOGV : DomainObject
	{
		public static readonly string Key = "87d89545-39ce-47d4-80c7-772464bc30de";

		public virtual string Name { get; set; }
		public virtual byte[] Doc { get; set; }
		public virtual string Note { get; set; }
		public virtual DateTime DateDoc { get; set; }
		public virtual string Executor { get; set; }
		public virtual D_Application_FromOGV RefApplicOGV { get; set; }
	}
}
