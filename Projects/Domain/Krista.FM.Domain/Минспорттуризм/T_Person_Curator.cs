using System;

namespace Krista.FM.Domain
{
	public class T_Person_Curator : DomainObject
	{
		public static readonly string Key = "cb32cc07-7b51-4742-8e71-a6fb69b9606c";

		public virtual string Surname { get; set; }
		public virtual string FirstName { get; set; }
		public virtual string Patronimyc { get; set; }
		public virtual string JobPosition { get; set; }
		public virtual string Telephone { get; set; }
		public virtual string Email { get; set; }
		public virtual byte[] Photo { get; set; }
		public virtual byte[] Information { get; set; }
		public virtual D_Territory_RF Territory { get; set; }
	}
}
